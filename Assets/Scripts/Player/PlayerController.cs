using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public InputActionReference movementInput, lookInput, jumpInput;
    [Space] public float maxMovementSpeed;
    public float movementAcceleration;
    public float rotationSmoothAmm;
    public float smoothMovementAmm;
    public float jumpHeight;
    public float lowJumpHeight;
    public float fallMultiplier;
    [Space] public LayerMask groundMask;
    public Vector3 groundColliderSize;

    private Rigidbody m_PlayerRigidbody;
    private Vector3 m_PlayerRawInput;
    private Vector3 m_PlayerInput;
    private bool m_PlayerJumpInput;
    private Collider m_PlayerCollider;
    private Camera m_Camera;
    private bool m_HasAlreadyJumped;
    private bool m_IsNotAlreadyGrounded;

    #region Public Player Events

    public event Action<Transform> ONPlayerJumpEvent, ONPlayerLandingEvent;
    public event Action<Vector3> OnPlayerMoveEvent;

    #endregion

    
    //Properties that expose some of the values that the player uses (handy for applying buffs such as speed or jump values)
    #region Public Properties to mod player stats
    public float CustomFallMultiplier { set; private get; }
    public float CustomLowJumpHeight { set; private get; }
    public float CustomJumpHeight { set; private get; }
    public float CustomMaxMovementSpeed { set; private get; }
    public float CustomAcceleration { set; private get; }

    #endregion


    #region Private Properties

    /// <summary>
    /// Calculates the bottom position of the player using the player's collider and custom ground collider size values.
    /// </summary>
    private Vector3 BottomPosition
    {
        get
        {
            Vector3 result = transform.position;
            if (m_PlayerCollider)
            {
                var position = transform.position;
                result = new Vector3(position.x,
                    position.y - (m_PlayerCollider.bounds.size.y / 2f) - (groundColliderSize.y / 2f),
                    position.z);
            }

            return result;
        }
    }

    private Vector3 GroundColliderExtends
    {
        get { return new Vector3(groundColliderSize.x / 2f, groundColliderSize.y, groundColliderSize.z / 2f); }
    }

    #endregion

    private void OnEnable()
    {
        InputManager.SetInputActive(true, movementInput, lookInput, jumpInput);
    }


    private void Awake()
    {
        m_PlayerRigidbody = GetComponent<Rigidbody>();
        m_PlayerCollider = GetComponent<Collider>();
        m_Camera = Camera.main;

        SetCursorActive(false);

        // ONPlayerJumpEvent += controller => Debug.Log($"{controller.gameObject.name} has Jumped!");
        // ONPlayerLandingEvent += controller => Debug.Log($"{controller.gameObject.name} has landed!");
    }

    public void SetCursorActive(bool value)
    {
        Cursor.visible = value;
        Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
    }

    private void Update()
    {
        UpdateInput();
        RotatePlayerTowardsForwardMovement();
    }

    private void RotatePlayerTowardsForwardMovement()
    {
        if (m_PlayerRawInput != Vector3.zero)
            transform.rotation = Quaternion.Lerp(transform.rotation,
                Quaternion.LookRotation(m_PlayerInput.normalized, Vector3.up),
                rotationSmoothAmm * Time.deltaTime);
    }

    private void UpdateInput()
    {
        m_PlayerRawInput = movementInput.GetAxisRaw().ToVector3XZ();
        m_PlayerInput = m_Camera.transform.forward * m_PlayerRawInput.z + m_Camera.transform.right * m_PlayerRawInput.x;
        m_PlayerInput = new Vector3(m_PlayerInput.x, 0, m_PlayerInput.z);
        m_PlayerJumpInput = jumpInput.GetButton();
    }

    private void FixedUpdate()
    {
        ApplyMovementInputToRigidbody();
        JumpHandler();
        UpdateGravity();
    }

    private void ApplyMovementInputToRigidbody()
    {
        Vector3 finalVelocity = Vector3.Lerp(m_PlayerRigidbody.velocity,
            m_PlayerInput * (maxMovementSpeed + (CustomMaxMovementSpeed + 1)),
            (movementAcceleration + CustomAcceleration + 1) * Time.fixedDeltaTime);
        m_PlayerRigidbody.velocity = new Vector3(finalVelocity.x, m_PlayerRigidbody.velocity.y, finalVelocity.z);

        if (m_PlayerRigidbody.velocity.magnitude.IsFloatWithinLimits(-0.01f, 0.01f))
        {
            OnPlayerMoveEvent?.Invoke(m_PlayerRigidbody.velocity);
        }
    }

    private void JumpHandler()
    {
        //Checks if the player has attempted to jump on the ground and has not already jumped.
        if (m_PlayerJumpInput && IsGrounded() && !m_HasAlreadyJumped)
        {
            var velocity = m_PlayerRigidbody.velocity;
            velocity = new Vector3(velocity.x, 0, velocity.z);
            m_PlayerRigidbody.velocity = velocity;
            m_PlayerRigidbody.AddForce(Vector3.up * (jumpHeight + CustomJumpHeight), ForceMode.VelocityChange);
            m_HasAlreadyJumped = true;
            ONPlayerJumpEvent?.Invoke(transform);
        }

        //Checks if the player is in the air (is either falling or jumping)
        if (!m_PlayerRigidbody.velocity.y.IsFloatWithinLimits(-0.01f, 0.01f))
        {
            m_IsNotAlreadyGrounded = false;
        }

        //Checks if the player has landed
        if (m_PlayerRigidbody.velocity.y < -0.01f && IsGrounded() && !m_IsNotAlreadyGrounded)
        {
            ONPlayerLandingEvent?.Invoke(transform);
            m_HasAlreadyJumped = false;
            m_IsNotAlreadyGrounded = true;
        }
    }

    /// <summary>
    /// Boosts the fall direction of the rigidbody as well as implements a short-hop.
    /// </summary>
    private void UpdateGravity()
    {
        if (m_PlayerRigidbody.velocity.y < 0)
        {
            m_PlayerRigidbody.velocity += Vector3.up *
                                          (Physics.gravity.y * ((fallMultiplier + CustomFallMultiplier) - 1) *
                                           Time.deltaTime);
        }
        else if (m_PlayerRigidbody.velocity.y > 0 && !m_PlayerJumpInput)
        {
            m_PlayerRigidbody.velocity += Vector3.up *
                                          (Physics.gravity.y * ((lowJumpHeight + CustomLowJumpHeight) - 1) *
                                           Time.deltaTime);
        }
    }

    /// <summary>
    /// Checks if the player is touching the ground or not.
    /// </summary>
    /// <returns></returns>
    private bool IsGrounded()
    {
        Collider[] collisionArray = new Collider[50];

        Physics.OverlapBoxNonAlloc(BottomPosition,
            GroundColliderExtends, collisionArray,
            transform.rotation,
            groundMask);
        return Array.Find(collisionArray, c => c && c.GetInstanceID() != m_PlayerCollider.GetInstanceID());
    }


    private void OnDisable()
    {
        InputManager.SetInputActive(false, movementInput, lookInput, jumpInput);
    }

    private void OnDrawGizmos()
    {
        Matrix4x4 matrix4X4 = Matrix4x4.TRS(BottomPosition, transform.rotation, GroundColliderExtends);
        Gizmos.matrix = matrix4X4;
        Gizmos.color = Color.magenta - new Color(0, 0, 0, 0.5f);
        Gizmos.DrawCube(Vector3.zero, Vector3.one);
    }
}


public static class FloatExtensions
{
    public static bool IsFloatWithinLimits(this float s, float min, float max)
    {
        return s > min && s < max;
    }
}