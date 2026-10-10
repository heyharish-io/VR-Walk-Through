using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering.Universal;

public class CheckCamera
{
    [MenuItem("Tools/Check Camera Settings")]
    public static void CheckCameraSettings()
    {
        var cam = GameObject.Find("/XR Origin (XR Rig)/Camera Offset/Main Camera");
        if (cam == null)
        {
            Debug.LogError("Camera not found!");
            return;
        }
        
        var camera = cam.GetComponent<Camera>();
        var uad = cam.GetComponent<UniversalAdditionalCameraData>();
        var tpd = cam.GetComponent("UnityEngine.XR.Interaction.Toolkit.TrackedPoseDriver");
        
        Debug.Log("=== Camera Settings ===");
        Debug.Log("Camera enabled: " + camera.enabled);
        Debug.Log("Camera stereoTargetEye: " + camera.stereoTargetEye);
        Debug.Log("Camera nearClipPlane: " + camera.nearClipPlane);
        Debug.Log("Camera farClipPlane: " + camera.farClipPlane);
        Debug.Log("Camera clearFlags: " + camera.clearFlags);
        Debug.Log("Camera backgroundColor: " + camera.backgroundColor);
        Debug.Log("Camera cullingMask: " + camera.cullingMask);
        Debug.Log("Camera renderingPath: " + camera.renderingPath);
        Debug.Log("Camera targetTexture: " + camera.targetTexture);
        Debug.Log("Camera depth: " + camera.depth);
        
        Debug.Log("\n=== UniversalAdditionalCameraData ===");
        if (uad != null)
        {
            Debug.Log("UAD renderType: " + uad.renderType);
            Debug.Log("UAD requiresDepthOption: " + uad.requiresDepthOption);
            Debug.Log("UAD requiresColorOption: " + uad.requiresColorOption);
            Debug.Log("UAD cameraStack: " + uad.cameraStack);
            Debug.Log("UAD volumeLayerMask: " + uad.volumeLayerMask);
            Debug.Log("UAD volumeTrigger: " + uad.volumeTrigger);
            Debug.Log("UAD renderPostProcessing: " + uad.renderPostProcessing);
            Debug.Log("UAD stopNaN: " + uad.stopNaN);
            Debug.Log("UAD dithering: " + uad.dithering);
            Debug.Log("UAD allowXRRendering: " + uad.allowXRRendering);
        }
        else
        {
            Debug.Log("No UniversalAdditionalCameraData component!");
        }
        
        Debug.Log("\n=== TrackedPoseDriver ===");
        if (tpd != null)
        {
            Debug.Log("TPD component: " + tpd.GetType().FullName);
            var enabledProp = tpd.GetType().GetProperty("enabled");
            if (enabledProp != null) Debug.Log("TPD enabled: " + enabledProp.GetValue(tpd));
            var poseProviderProp = tpd.GetType().GetProperty("poseProvider");
            if (poseProviderProp != null) Debug.Log("TPD poseProvider: " + poseProviderProp.GetValue(tpd));
            var trackingTypeProp = tpd.GetType().GetProperty("trackingType");
            if (trackingTypeProp != null) Debug.Log("TPD trackingType: " + trackingTypeProp.GetValue(tpd));
            var useRelativeTransformProp = tpd.GetType().GetProperty("useRelativeTransform");
            if (useRelativeTransformProp != null) Debug.Log("TPD useRelativeTransform: " + useRelativeTransformProp.GetValue(tpd));
        }
        else
        {
            Debug.Log("No TrackedPoseDriver component!");
        }
        
        // Check XR Origin
        var xrOrigin = GameObject.Find("/XR Origin (XR Rig)");
        if (xrOrigin != null)
        {
            var xrOriginComp = xrOrigin.GetComponent("UnityEngine.XR.Interaction.Toolkit.XROrigin");
            if (xrOriginComp != null)
            {
                Debug.Log("\n=== XR Origin ===");
                var cameraProp = xrOriginComp.GetType().GetProperty("Camera");
                if (cameraProp != null) Debug.Log("XR Origin Camera: " + cameraProp.GetValue(xrOriginComp));
                var floorOffsetProp = xrOriginComp.GetType().GetProperty("CameraFloorOffsetObject");
                if (floorOffsetProp != null) Debug.Log("XR Origin CameraFloorOffsetObject: " + floorOffsetProp.GetValue(xrOriginComp));
            }
        }
        
        // Check for other cameras in scene
        var allCameras = Camera.allCameras;
        Debug.Log("\n=== All Cameras in Scene ===");
        foreach (var c in allCameras)
        {
            Debug.Log("Camera: " + c.name + " enabled: " + c.enabled + " depth: " + c.depth + " targetEye: " + c.stereoTargetEye);
        }
    }
}