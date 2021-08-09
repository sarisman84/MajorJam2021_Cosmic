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
    public float jumpHeight;
    [Space] public LayerMask groundMask;
    public Vector3 groundColliderSize;

    private Rigidbody m_PlayerRigidbody;
    private Vector3 m_PlayerRawInput;
    private Vector3 m_PlayerInput;
    private bool m_PlayerJumpInput;
    private Collider m_PlayerCollider;
    private Camera m_Camera;


    private Vector3 BottomPosition
    {
        get
        {
            Vector3 result = transform.position;
            if (m_PlayerCollider)
            {
                var position = transform.position;
                result = new Vector3(position.x, position.y - (m_PlayerCollider.bounds.size.y / 2f),
                    position.z);
            }

            return result;
        }
    }

    private Vector3 GroundColliderExtends
    {
        get { return new Vector3(groundColliderSize.x / 2f, groundColliderSize.y, groundColliderSize.z / 2f); }
    }

    private void OnEnable()
    {
        InputManager.SetInputActive(true, movementInput, lookInput, jumpInput);
    }


    private void Awake()
    {
        m_PlayerRigidbody = GetComponent<Rigidbody>();
        m_PlayerCollider = GetComponent<Collider>();
        m_Camera = Camera.main;
    }

    private void Update()
    {
        m_PlayerRawInput = movementInput.GetAxisRaw().ToVector3XZ();
        m_PlayerInput = m_Camera.transform.forward * m_PlayerRawInput.z + m_Camera.transform.right * m_PlayerRawInput.x;
        m_PlayerJumpInput = jumpInput.GetButton();

        if (m_PlayerRawInput != Vector3.zero)
            transform.rotation = Quaternion.Lerp(transform.rotation,
                Quaternion.LookRotation(new Vector3(m_PlayerInput.x, 0, m_PlayerInput.z).normalized, Vector3.up),
                rotationSmoothAmm * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        Vector3 finalVelocity = Vector3.Lerp(m_PlayerRigidbody.velocity,
            m_PlayerInput * maxMovementSpeed, movementAcceleration * Time.fixedDeltaTime);
        m_PlayerRigidbody.velocity = new Vector3(finalVelocity.x, m_PlayerRigidbody.velocity.y, finalVelocity.z);

        if (m_PlayerJumpInput && IsGrounded())
            m_PlayerRigidbody.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
    }

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