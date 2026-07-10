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

        // 1. Running inside the Unity Editor
        if (Application.isEditor)
        {
            m_JoystickPanel.SetActive(m_ForceShowInEditor);
            Debug.Log($"[JoystickPlatformTester] Running in Editor. Joystick visibility: {m_ForceShowInEditor}");
            return;
        }

        bool showJoystick;

        // 2. Running as WebGL build -> ask the browser directly via jslib plugin.
        //    Application.isMobilePlatform and SystemInfo.deviceType are unreliable here.
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            bool isMobileBrowser = WebGLDeviceDetector.IsMobile();

            if (isMobileBrowser)
            {
                showJoystick = true;
                Debug.Log("[JoystickPlatformTester] WebGL Mobile browser detected -> SHOW Joystick.");
            }
            else
            {
                showJoystick = false;
                Debug.Log("[JoystickPlatformTester] WebGL PC/Desktop browser detected -> HIDE Joystick.");
            }
        }
        // 3. Native mobile build (Android/iOS, not WebGL)
        else if (Application.isMobilePlatform)
        {
            showJoystick = true;
            Debug.Log("[JoystickPlatformTester] Native Mobile platform detected -> SHOW Joystick.");
        }
        // 4. Native desktop/console build (Windows, Mac, Linux, consoles)
        else
        {
            showJoystick = false;
            Debug.Log("[JoystickPlatformTester] Native Desktop/Console platform detected -> HIDE Joystick.");
        }

        m_JoystickPanel.SetActive(showJoystick);
    }
}
