using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering.Universal;
using UnityEditor.SceneManagement;
using System.Reflection;
using System.Linq;

public class FixCameraSettings
{
    [MenuItem("Tools/Fix Camera Settings for Quest 3")]
    public static void FixCameraSettingsForQuest()
    {
        var camOffset = GameObject.Find("/XR Origin (XR Rig)/Camera Offset");
        var cam = GameObject.Find("/XR Origin (XR Rig)/Camera Offset/Main Camera");
        
        if (cam == null)
        {
            Debug.LogError("Camera not found!");
            return;
        }
        
        var camera = cam.GetComponent<Camera>();
        var uad = cam.GetComponent<UniversalAdditionalCameraData>();
        
        Debug.Log("=== Fixing Camera Settings for Quest 3 ===");
        
        // Fix 1: TrackedPoseDriver on Camera Offset (for head tracking)
        if (camOffset != null)
        {
            // TrackedPoseDriver is in UnityEngine.SpatialTracking namespace (in UnityEngine.SpatialTracking assembly)
            var tpd = camOffset.GetComponent("UnityEngine.SpatialTracking.TrackedPoseDriver");
            if (tpd == null)
            {
                // Get the TrackedPoseDriver type via reflection since it's in a different assembly
                var spatialAssembly = System.AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(a => a.GetName().Name == "UnityEngine.SpatialTracking");
                var tpdType = spatialAssembly?.GetType("UnityEngine.SpatialTracking.TrackedPoseDriver");
                if (tpdType != null)
                {
                    tpd = camOffset.AddComponent(tpdType);
                    Debug.Log("Added TrackedPoseDriver to Camera Offset");
                }
                else
                {
                    Debug.LogError("TrackedPoseDriver type not found in UnityEngine.SpatialTracking assembly");
                }
            }
            
            // Configure TrackedPoseDriver for head tracking
            if (tpd != null)
            {
                var trackingTypeProp = tpd.GetType().GetProperty("trackingType");
                if (trackingTypeProp != null)
                {
                    // TrackingType enum: 0 = Head, 1 = LeftHand, 2 = RightHand, 3 = CenterEye
                    trackingTypeProp.SetValue(tpd, 0); // Head
                    Debug.Log("Set TrackedPoseDriver trackingType to Head (0)");
                }
                
                var poseProviderProp = tpd.GetType().GetProperty("poseProvider");
                if (poseProviderProp != null)
                {
                    // 0 = UnityXR, 1 = LegacyXR
                    poseProviderProp.SetValue(tpd, 0); // UnityXR
                    Debug.Log("Set TrackedPoseDriver poseProvider to UnityXR (0)");
                }
                
                var useRelativeProp = tpd.GetType().GetProperty("useRelativeTransform");
                if (useRelativeProp != null)
                {
                    useRelativeProp.SetValue(tpd, true);
                    Debug.Log("Set TrackedPoseDriver useRelativeTransform to true");
                }
                
                var enabledProp = tpd.GetType().GetProperty("enabled");
                if (enabledProp != null)
                {
                    enabledProp.SetValue(tpd, true);
                    Debug.Log("Enabled TrackedPoseDriver");
                }
            }
        }
        
        // Fix 2: Camera clearFlags to Solid Color (for VR)
        camera.clearFlags = CameraClearFlags.SolidColor;
        Debug.Log("Set Camera clearFlags to SolidColor");
        
        // Fix 3: Background color to opaque black
        camera.backgroundColor = new Color(0, 0, 0, 1);
        Debug.Log("Set Camera backgroundColor to opaque black (0,0,0,1)");
        
        // Fix 4: Near clip plane for Quest (0.01 is good, but 0.02 is safer for Quest 3)
        if (camera.nearClipPlane < 0.02f)
        {
            camera.nearClipPlane = 0.02f;
            Debug.Log("Set Camera nearClipPlane to 0.02");
        }
        
        // Fix 5: Ensure UAD allowXRRendering is true
        if (uad != null)
        {
            uad.allowXRRendering = true;
            Debug.Log("Set UAD allowXRRendering to true");
            
            // Fix 6: Ensure renderPostProcessing is false for performance
            uad.renderPostProcessing = false;
            Debug.Log("Set UAD renderPostProcessing to false");
            
            // Fix 7: Set renderType to Base (already is)
            if (uad.renderType != CameraRenderType.Base)
            {
                uad.renderType = CameraRenderType.Base;
                Debug.Log("Set UAD renderType to Base");
            }
        }
        
        // Fix 8: Ensure camera depth is 0 (default for main camera)
        if (camera.depth != 0)
        {
            camera.depth = 0;
            Debug.Log("Set Camera depth to 0");
        }
        
        // Fix 9: Ensure stereoTargetEye is Both
        if (camera.stereoTargetEye != StereoTargetEyeMask.Both)
        {
            camera.stereoTargetEye = StereoTargetEyeMask.Both;
            Debug.Log("Set Camera stereoTargetEye to Both");
        }
        
        Debug.Log("=== Camera Settings Fixed for Quest 3 ===");
        EditorUtility.SetDirty(cam);
        if (camOffset != null) EditorUtility.SetDirty(camOffset);
        EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
    }
}