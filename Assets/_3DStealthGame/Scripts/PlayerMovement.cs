using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;  // for Keyboard
using UnityEngine.UI;          // for Slider

public class PlayerMovement : MonoBehaviour
{
    Animator m_Animator;

    [Header("Input Actions")]
    public InputAction MoveAction;      // same as before (already working)

    [Header("Movement")]
    public float walkSpeed = 1.0f;
    public float turnSpeed = 20f;
    public float sprintMultiplier = 1.8f;

    [Header("Sprint / Stamina")]
    public float maxStamina = 5f;                  // max “seconds” of sprint
    public float staminaDrainPerSecond = 1f;       // how fast it drains
    public float staminaRecoveryPerSecond = 0.75f; // how fast it refills
    public Slider sprintBar;                       // UI slider (0–1)

    Rigidbody m_Rigidbody;
    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;

    float m_CurrentStamina;

    void OnEnable()
    {
        MoveAction.Enable();
    }

    void OnDisable()
    {
        MoveAction.Disable();
    }

    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Animator = GetComponent<Animator>();

        m_CurrentStamina = maxStamina;

        if (sprintBar != null)
        {
            sprintBar.minValue = 0f;
            sprintBar.maxValue = 1f;
            sprintBar.value = 1f;
        }
        else
        {
            Debug.LogWarning("PlayerMovement: sprintBar not assigned in Inspector.");
        }
    }

    void Update()
    {
        UpdateSprintBar();
    }

    void FixedUpdate()
    {
        // --- Movement input ---
        Vector2 pos = MoveAction.ReadValue<Vector2>();
        float horizontal = pos.x;
        float vertical = pos.y;

        m_Movement.Set(horizontal, 0f, vertical);
        m_Movement.Normalize();

        bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);
        bool isWalking = hasHorizontalInput || hasVerticalInput;

        m_Animator.SetBool("IsWalking", isWalking);

        // --- Sprint logic: use Left Shift directly ---
        float currentSpeed = walkSpeed;

        bool wantsSprint = Keyboard.current != null &&
                           Keyboard.current.leftShiftKey.isPressed;

        if (wantsSprint && isWalking && m_CurrentStamina > 0f)
        {
            currentSpeed *= sprintMultiplier;

            m_CurrentStamina -= staminaDrainPerSecond * Time.deltaTime;
            if (m_CurrentStamina < 0f)
                m_CurrentStamina = 0f;
        }
        else
        {
            // Recover stamina while not sprinting
            if (m_CurrentStamina < maxStamina)
            {
                m_CurrentStamina += staminaRecoveryPerSecond * Time.deltaTime;
                if (m_CurrentStamina > maxStamina)
                    m_CurrentStamina = maxStamina;
            }
        }

        // --- Apply movement & rotation ---
        if (m_Movement.sqrMagnitude > 0.001f)
        {
            Vector3 desiredForward =
                Vector3.RotateTowards(transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);
            m_Rotation = Quaternion.LookRotation(m_Movement);
            m_Rigidbody.MoveRotation(m_Rotation);
        }

        m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement * currentSpeed * Time.deltaTime);
    }

    void UpdateSprintBar()
    {
        if (sprintBar != null && maxStamina > 0f)
        {
            sprintBar.value = m_CurrentStamina / maxStamina;
        }
    }
}

