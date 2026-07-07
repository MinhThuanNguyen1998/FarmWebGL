using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerMovementController : MonoBehaviour
{
    [Header("Joystick Integration")]
    private Joystick m_Joystick; // <-- Xóa [SerializeField], chuyển thành private

    [Header("Movement Settings")]
    public float m_Velocity = 5f;
    public float m_Gravity = 9.8f;

    // Inputs
    float m_InputHorizontal;
    float m_InputVertical;

    [SerializeField] Animator m_Animator;
    [SerializeField] CharacterController m_CharacterController;

    [Inject]
    public void Construct(Joystick joystick)
    {
        m_Joystick = joystick;
    }

    void Start()
    {
        if (m_Animator == null)
            Debug.LogWarning("Hey buddy, you don't have the Animator component in your player. Without it, the animations won't work.");
    }

    void Update()
    {
        // --- SIMULTANEOUS JOYSTICK AND WASD SUPPORT ---
        float keyboardH = Input.GetAxis("Horizontal");
        float keyboardV = Input.GetAxis("Vertical");

        float joystickH = (m_Joystick != null) ? m_Joystick.Horizontal : 0f;
        float joystickV = (m_Joystick != null) ? m_Joystick.Vertical : 0f;

        m_InputHorizontal = Mathf.Clamp(keyboardH + joystickH, -1f, 1f);
        m_InputVertical = Mathf.Clamp(keyboardV + joystickV, -1f, 1f);
        // ----------------------------------------------

        if (m_CharacterController.isGrounded && m_Animator != null)
        {
            float minimumSpeed = 0.9f;
            m_Animator.SetBool("run", m_CharacterController.velocity.magnitude > minimumSpeed);
        }
    }

    private void FixedUpdate()
    {
        float directionX = m_InputHorizontal * m_Velocity * Time.fixedDeltaTime;
        float directionZ = m_InputVertical * m_Velocity * Time.fixedDeltaTime;
        float directionY = -m_Gravity * Time.fixedDeltaTime;

        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        forward = forward * directionZ;
        right = right * directionX;

        if (directionX != 0 || directionZ != 0)
        {
            float angle = Mathf.Atan2(forward.x + right.x, forward.z + right.z) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(0, angle, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.15f);
        }

        Vector3 verticalDirection = Vector3.up * directionY;
        Vector3 horizontalDirection = forward + right;

        Vector3 movement = verticalDirection + horizontalDirection;
        m_CharacterController.Move(movement);
    }
}
