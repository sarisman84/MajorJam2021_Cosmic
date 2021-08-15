using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using MajorJam.System;
using Systems;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public InputActionReference movementInput, lookInput, jumpInput;
    [Header("Movement Settings")] public float maxMovementSpeed;
    public float movementAcceleration;
    public float rotationSmoothAmm;
    public float smoothMovementAmm;
    [Header("Jump Settings")] public float jumpHeight;
    public float lowJumpHeight;
    public float fallMultiplier;
    [Space] public LayerMask groundMask;
    public Vector3 groundColliderSize;

    private Animator m_PlayerAnimController;
    private Rigidbody m_PlayerRigidbody;
    private Vector3 m_PlayerRawInput;
    private Vector3 m_PlayerInput;
    private bool m_PlayerJumpInput;
    private Collider m_PlayerCollider;
    private Camera m_Camera;
    private CinemachineFreeLook m_CinemachineFreeLook;
    private bool m_HasAlreadyJumped;
    private bool m_IsNotAlreadyGrounded;

    private CinemachineFreeLook.Orbit m_CinemachineTopOrbit, m_CinemacineMidOrbitHeight, m_CinemachineBottomOrbit;
    private Vector3 m_CinemachineTopTOOffset, m_CinemachineMidTOOffset, m_CinemachineBottomTOOffset;
    private CinemachineComposer m_TopCComposer, m_MidCComposer, m_BottomCComposer;
    private Cinemachine.AxisState m_CinemachineVerticalInputAcc;

    private float m_CCurXSpeed, m_CCurYSpeed;


    private string m_CurrentAnimState;

    private const string PlayerIdle = "Idle",
        PlayerWalk = "Walking",
        PlayerRun = "Running",
        PlayerJump = "Jumping Up",
        PlayerFall = "Falling",
        PlayerLand = "Fall to Land";

    #region Public Player Events

    public event Action<Transform> ONPlayerJumpEvent, ONPlayerLandingEvent;
    public event Action<Vector3> OnPlayerMoveEvent;

    #endregion


    //Properties that expose some of the values that the player uses (handy for applying buffs such as speed or jump values)

    #region Public Properties to mod player stats

    public float CustomFallMultiplier { set; get; }
    public float CustomLowJumpHeight { set; get; }
    public float CustomJumpHeight { set; get; }
    public float CustomMaxMovementSpeed { set; get; }
    public float CustomAcceleration { set; get; }

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
                    !IsGravityReversed
                        ? (position.y - (m_PlayerCollider.bounds.size.y / 2f) -
                           (groundColliderSize.y / 2f))
                        : (position.y + (m_PlayerCollider.bounds.size.y / 2f) +
                           (groundColliderSize.y / 2f)) + 0.1f,
                    position.z);
            }

            return result;
        }
    }

    private Vector3 GroundColliderExtends
    {
        get { return new Vector3(groundColliderSize.x / 2f, groundColliderSize.y, groundColliderSize.z / 2f); }
    }


    public Rigidbody physics => m_PlayerRigidbody;
    public bool IsGravityReversed { private get; set; }

    #endregion

    private void OnEnable()
    {
        InputManager.SetInputActive(true, movementInput, lookInput, jumpInput);
    }


    private void Awake()
    {
        m_PlayerAnimController = GetComponentInChildren<Animator>();
        m_PlayerRigidbody = GetComponent<Rigidbody>();
        m_PlayerCollider = GetComponent<Collider>();
        m_Camera = Camera.main;
        m_CinemachineFreeLook = transform.parent.GetComponentInChildren<CinemachineFreeLook>();


        SetCursorActive(false);
        CinemachineCameraSetup();

        UIManager.Get.ONPauseEvent += OnPauseEvent;

        // ONPlayerJumpEvent += controller => Debug.Log($"{controller.gameObject.name} has Jumped!");
        // ONPlayerLandingEvent += controller => Debug.Log($"{controller.gameObject.name} has landed!");
    }

    private void OnPauseEvent(bool isPaused)
    {
        m_CCurXSpeed = m_CinemachineFreeLook.m_XAxis.m_MaxSpeed != 0
            ? m_CinemachineFreeLook.m_XAxis.m_MaxSpeed
            : m_CCurXSpeed;
        m_CCurYSpeed = m_CinemachineFreeLook.m_YAxis.m_MaxSpeed != 0
            ? m_CinemachineFreeLook.m_YAxis.m_MaxSpeed
            : m_CCurYSpeed;

        InputManager.SetInputActive(!isPaused, jumpInput, lookInput, movementInput);
        m_CinemachineFreeLook.m_XAxis.m_MaxSpeed = !isPaused ? m_CCurXSpeed : 0;
        m_CinemachineFreeLook.m_YAxis.m_MaxSpeed = !isPaused ? m_CCurYSpeed : 0;
    }

    private void CinemachineCameraSetup()
    {
        m_CinemachineTopOrbit = m_CinemachineFreeLook.m_Orbits[0];
        m_CinemacineMidOrbitHeight = m_CinemachineFreeLook.m_Orbits[1];
        m_CinemachineBottomOrbit = m_CinemachineFreeLook.m_Orbits[2];

        m_TopCComposer = m_CinemachineFreeLook.GetRig(0).GetCinemachineComponent<CinemachineComposer>();
        m_MidCComposer = m_CinemachineFreeLook.GetRig(1).GetCinemachineComponent<CinemachineComposer>();
        m_BottomCComposer = m_CinemachineFreeLook.GetRig(2).GetCinemachineComponent<CinemachineComposer>();


        m_CinemachineTopTOOffset = m_TopCComposer.m_TrackedObjectOffset;
        m_CinemachineMidTOOffset = m_MidCComposer.m_TrackedObjectOffset;
        m_CinemachineBottomTOOffset = m_BottomCComposer.m_TrackedObjectOffset;

        m_CinemachineVerticalInputAcc = m_CinemachineFreeLook.m_YAxis;
    }

    enum ASChangeType
    {
        None,
        CrossFade
    }

    private void ChangeAnimationState(string newState, ASChangeType type = ASChangeType.None, float duration = 0.5f)
    {
        if (m_CurrentAnimState == newState) return;

        switch (type)
        {
            case ASChangeType.None:
                m_PlayerAnimController.Play(newState);
                break;
            case ASChangeType.CrossFade:
                m_PlayerAnimController.CrossFade(newState, duration);
                break;
        }


        m_CurrentAnimState = newState;
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
        FetchPlayerInput();
        UpdateCameraOrientation();
    }

    private void FetchPlayerInput()
    {
        m_PlayerRawInput = movementInput.GetAxisRaw().ToVector3XZ();
        m_PlayerInput = m_Camera.transform.forward * m_PlayerRawInput.z + m_Camera.transform.right * m_PlayerRawInput.x;
        m_PlayerInput = new Vector3(m_PlayerInput.x, 0, m_PlayerInput.z);
        m_PlayerJumpInput = jumpInput.GetButton();
    }

    private void UpdateCameraOrientation()
    {
        //Invert the heights,radius and offsets on top and bottom orbits upon being upside down.


        //Update top orbit height and offset position based if the player is upsidedown or not.
        m_CinemachineFreeLook.m_Orbits[0] = !IsGravityReversed
            ? m_CinemachineTopOrbit
            : m_CinemachineBottomOrbit;
        m_TopCComposer.m_TrackedObjectOffset =
            !IsGravityReversed ? m_CinemachineTopTOOffset : -m_CinemachineBottomTOOffset;


        //Update bottom orbit height and offset position based if the player is upsidedown or not.
        m_CinemachineFreeLook.m_Orbits[2] = !IsGravityReversed
            ? m_CinemachineBottomOrbit
            : m_CinemachineTopOrbit;
        m_BottomCComposer.m_TrackedObjectOffset =
            !IsGravityReversed ? m_CinemachineBottomTOOffset : m_CinemachineTopTOOffset;


        m_CinemachineFreeLook.m_YAxis.m_InvertInput = !IsGravityReversed;
    }

    private void FixedUpdate()
    {
        ApplyMovementInputToRigidbody();
        JumpHandler();
        UpdateGravity();
    }

    private void ApplyMovementInputToRigidbody()
    {
        bool isGrounded = IsGrounded();
        Vector3 finalVelocity = Vector3.Lerp(m_PlayerRigidbody.velocity,
            m_PlayerInput * (maxMovementSpeed + (CustomMaxMovementSpeed + 1)),
            (movementAcceleration + CustomAcceleration + 1) * Time.fixedDeltaTime);
        m_PlayerRigidbody.velocity = new Vector3(finalVelocity.x, m_PlayerRigidbody.velocity.y, finalVelocity.z);


        if (isGrounded)
            if (m_PlayerRigidbody.velocity.magnitude.IsFloatWithinLimits(-0.01f, 0.01f))
            {
                if (!m_HasAlreadyJumped)
                    ChangeAnimationState(PlayerIdle, ASChangeType.CrossFade);
                OnPlayerMoveEvent?.Invoke(m_PlayerRigidbody.velocity);
            }
            else
            {
                if (!m_HasAlreadyJumped)
                    ChangeAnimationState(m_PlayerRigidbody.velocity.magnitude.IsFloatWithinLimits(-5, 5)
                        ? PlayerWalk
                        : PlayerRun, ASChangeType.CrossFade);
            }
    }

    private void JumpHandler()
    {
        bool isGrounded = IsGrounded();
        //Checks if the player has attempted to jump on the ground and has not already jumped.
        if (m_PlayerJumpInput && isGrounded && !m_HasAlreadyJumped)
        {
            ChangeAnimationState(PlayerJump);
            var velocity = m_PlayerRigidbody.velocity;
            velocity = new Vector3(velocity.x, 0, velocity.z);
            m_PlayerRigidbody.velocity = velocity;
            m_PlayerRigidbody.AddForce(
                (!IsGravityReversed ? Vector3.up : Vector3.down) * (jumpHeight + CustomJumpHeight),
                ForceMode.VelocityChange);
            m_HasAlreadyJumped = true;
            ONPlayerJumpEvent?.Invoke(transform);
            AudioManager.Manager.Play("Player_Jump");
        }

        //Checks if the player is in the air (is either falling or jumping)
        if (!m_PlayerRigidbody.velocity.y.IsFloatWithinLimits(-0.01f, 0.01f))
        {
            m_IsNotAlreadyGrounded = false;
        }

        bool isFalling = (!IsGravityReversed
            ? m_PlayerRigidbody.velocity.y < -0.00001f
            : m_PlayerRigidbody.velocity.y > 0.00001f);

        if (isFalling && !isGrounded)
        {
            ChangeAnimationState(PlayerFall, ASChangeType.CrossFade, 0.85f);
        }

        //Checks if the player has landed
        if (isFalling && isGrounded && !m_IsNotAlreadyGrounded)
        {
            ONPlayerLandingEvent?.Invoke(transform);
            m_HasAlreadyJumped = false;
            m_IsNotAlreadyGrounded = true;
            AudioManager.Manager.Play("Player_Land");
            ChangeAnimationState(PlayerLand);
        }
    }

    /// <summary>
    /// Boosts the fall direction of the rigidbody as well as implements a short-hop.
    /// </summary>
    private void UpdateGravity()
    {
        if (!IsGravityReversed ? m_PlayerRigidbody.velocity.y < 0 : m_PlayerRigidbody.velocity.y > 0)
        {
            m_PlayerRigidbody.velocity += transform.up.normalized *
                                          (Physics.gravity.y * ((fallMultiplier + CustomFallMultiplier) - 1) *
                                           Time.deltaTime);
        }
        else if ((!IsGravityReversed
            ? m_PlayerRigidbody.velocity.y > 0
            : m_PlayerRigidbody.velocity.y < 0) && !m_PlayerJumpInput)
        {
            m_PlayerRigidbody.velocity += transform.up.normalized *
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
        transform.position = Vector3.up;
    }

    private void OnDrawGizmos()
    {
        Matrix4x4 matrix4X4 = Matrix4x4.TRS(BottomPosition, transform.rotation, GroundColliderExtends);
        Gizmos.matrix = matrix4X4;
        Gizmos.color = (IsGrounded() ? Color.green : Color.red) - new Color(0, 0, 0, 0.5f);
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