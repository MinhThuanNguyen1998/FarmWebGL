using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class JoystickPlatformTester : MonoBehaviour
{
    [Header("UI Configuration")]
    [SerializeField] private GameObject m_JoystickPanel; // Drag and drop the Joystick UI GameObject here

    [Header("Editor Testing Settings")]
    [SerializeField] private bool m_ForceShowInEditor = true; // Enable to simulate and test the joystick in the Unity Editor

    private void Start()
    {
        ConfigureJoystickPerPlatform();
    }

    /// <summary>
    /// Checks the current platform and device type to toggle the Joystick UI accordingly.
    /// </summary>
    private void ConfigureJoystickPerPlatform()
    {
        // Safety check to ensure the UI reference is assigned in the Inspector
        if (m_JoystickPanel == null)
        {
            Debug.LogError("[JoystickPlatformTester] m_JoystickPanel is not assigned! Please attach the UI GameObject.");
            return;
        }

        // 1. If running inside the Unity Editor (Development Environment)
        if (Application.isEditor)
        {
            m_JoystickPanel.SetActive(m_ForceShowInEditor);
            Debug.Log($"[JoystickPlatformTester] Running in Editor. Joystick visibility: {m_ForceShowInEditor}");
            return;
        }

        // 2. If running on a physical Mobile/Tablet device
        // This handles Native Apps (Android/iOS) AND WebGL running on mobile browsers
        if (Application.isMobilePlatform)
        {
            m_JoystickPanel.SetActive(true);
            Debug.Log("[JoystickPlatformTester] Mobile platform detected (Native or WebGL Mobile) -> SHOW Joystick.");
        }
        // 3. If running on PC/Console desktop environments (Windows, Mac, Linux, WebGL PC)
        else
        {
            m_JoystickPanel.SetActive(false);
            Debug.Log("[JoystickPlatformTester] Desktop platform detected (PC/Console or WebGL PC) -> HIDE Joystick.");
        }
    }
}
