using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// Bridges to a small JavaScript plugin (WebGLDeviceDetector.jslib) that inspects
/// the browser's user-agent and touch capability to reliably tell whether the
/// current WebGL build is running on a mobile/tablet browser or a desktop browser.
///
/// Application.isMobilePlatform and SystemInfo.deviceType are NOT reliable enough
/// for this on WebGL, so this talks to the browser directly.
/// </summary>
public static class WebGLDeviceDetector
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern int IsMobileBrowser();
#endif

    /// <summary>
    /// Returns true if the current build is WebGL and it's running inside a
    /// mobile/tablet browser. Always returns false outside of WebGL (non-editor) builds,
    /// so callers should combine this with Application.isMobilePlatform for native builds.
    /// </summary>
    public static bool IsMobile()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        return IsMobileBrowser() == 1;
#else
        return false;
#endif
    }
}
