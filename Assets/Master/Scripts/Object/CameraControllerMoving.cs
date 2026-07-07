using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllerMoving : MonoBehaviour
{
    [Tooltip("Enable to move the camera by holding the right mouse button.")]
    public bool m_ClickToMoveCamera = false;
    [Tooltip("Enable zoom in/out when scrolling the mouse wheel.")]
    public bool m_CanZoom = true;
    [Space]
    [Tooltip("The higher it is, the faster the camera moves.")]
    public float m_Sensitivity = 5f;

    [Tooltip("Camera Y rotation limits. The X axis is the maximum it can go up and the Y axis is the maximum it can go down.")]
    public Vector2 m_CameraLimit = new Vector2(-45, 40);

    [SerializeField] float m_MouseX = -90f;
    [SerializeField] float m_MouseY = 0f;

    float m_OffsetDistanceY;
    Transform m_Player;

    void Start()
    {
        // Find the player object in the scene and get its transform
        m_Player = GameObject.FindWithTag("Player").transform;

        // Store the initial Y offset distance relative to the player
        m_OffsetDistanceY = transform.position.y;

        // Lock and hide the cursor if the click-to-move option is disabled
        if (!m_ClickToMoveCamera)
        {
            // Only lock the cursor on PC platforms (Standalone or Editor)
#if UNITY_STANDALONE || UNITY_EDITOR
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;
#endif
        }
    }

    void Update()
    {
        // Safety check in case the player object is missing
        if (m_Player == null) return;

        // Follow the player's position with the maintained Y offset
        transform.position = m_Player.position + new Vector3(0, m_OffsetDistanceY, 0);

        // Adjust the camera's Field of View (Zoom) based on the mouse scroll wheel
        if (m_CanZoom && Input.GetAxis("Mouse ScrollWheel") != 0)
            Camera.main.fieldOfView -= Input.GetAxis("Mouse ScrollWheel") * m_Sensitivity * 2;

        // If right-click is required to move camera but right-click is not held, stop processing
        if (m_ClickToMoveCamera && Input.GetAxisRaw("Fire2") == 0)
            return;

        // --- PC MOUSE INPUT HANDLING ---
        float m_DeltaX = Input.GetAxis("Mouse X") * m_Sensitivity;
        float m_DeltaY = Input.GetAxis("Mouse Y") * m_Sensitivity;

        // Accumulate the calculated movement into rotation values
        m_MouseX += m_DeltaX;
        m_MouseY += m_DeltaY;

        // Clamp the vertical rotation (Y-axis) within the specified limits
        m_MouseY = Mathf.Clamp(m_MouseY, m_CameraLimit.x, m_CameraLimit.y);

        // Apply the calculated rotation to the camera transform
        transform.rotation = Quaternion.Euler(-m_MouseY, m_MouseX, 0);
    }
}

