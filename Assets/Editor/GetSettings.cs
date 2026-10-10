using UnityEngine;
using UnityEditor;

public class GetSettings
{
    [MenuItem("Tools/Get Android Settings")]
    public static void GetAndroidSettings()
    {
        Debug.Log("minSdkVersion: " + PlayerSettings.Android.minSdkVersion);
        Debug.Log("targetSdkVersion: " + PlayerSettings.Android.targetSdkVersion);
        Debug.Log("targetArchitectures: " + PlayerSettings.Android.targetArchitectures);
        Debug.Log("bundleVersionCode: " + PlayerSettings.Android.bundleVersionCode);
        Debug.Log("applicationId: " + PlayerSettings.applicationIdentifier);
        Debug.Log("scriptingBackend: " + PlayerSettings.GetScriptingBackend(BuildTargetGroup.Android));
        Debug.Log("graphicsAPIs: " + string.Join(", ", PlayerSettings.GetGraphicsAPIs(BuildTarget.Android)));
        Debug.Log("colorSpace: " + PlayerSettings.colorSpace);
        Debug.Log("defaultScreenOrientation: " + PlayerSettings.defaultInterfaceOrientation);
    }
}