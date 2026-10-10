using UnityEngine;
using UnityEditor;
using UnityEditor.XR.Management;
using System.Collections.Generic;

public class CheckXRSettings
{
    [MenuItem("Tools/Check XR Settings")]
    public static void CheckXRSettingsForAndroid()
    {
        var settings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(BuildTargetGroup.Android);
        Debug.Log("=== XR Settings for Android ===");
        Debug.Log("XR General Settings: " + settings);
        
        if (settings != null)
        {
            Debug.Log("Manager: " + settings.Manager);
            if (settings.Manager != null)
            {
                Debug.Log("Active Loader: " + settings.Manager.activeLoader);
                if (settings.Manager.activeLoader != null)
                {
                    Debug.Log("Active Loader Name: " + settings.Manager.activeLoader.name);
                    Debug.Log("Active Loader Type: " + settings.Manager.activeLoader.GetType().FullName);
                }
                
                var loaders = settings.Manager.loaders;
                Debug.Log("Loaders count: " + loaders.Count);
                foreach (var l in loaders)
                {
                    Debug.Log("  Loader: " + l.name + " (" + l.GetType().FullName + ")");
                }
                
                Debug.Log("Init Manager On Start: " + settings.Manager.automaticLoading);
                Debug.Log("Auto Run: " + settings.Manager.automaticRunning);
            }
        }
        
        // Check OpenXR settings
        var openXRSettings = UnityEngine.XR.OpenXR.OpenXRSettings.GetSettingsForBuildTargetGroup(BuildTargetGroup.Android);
        if (openXRSettings != null)
        {
            Debug.Log("\n=== OpenXR Settings for Android ===");
            Debug.Log("Render Mode: " + openXRSettings.renderMode);
            // customLoaderName doesn't exist in this version
            
            var features = openXRSettings.GetFeatures();
            Debug.Log("Features count: " + features.Length);
            foreach (var f in features)
            {
                if (f != null && f.enabled)
                {
                    Debug.Log("  Enabled Feature: " + f.name + " (" + f.GetType().FullName + ")");
                }
            }
        }
        else
        {
            Debug.Log("\n=== OpenXR Settings for Android ===");
            Debug.Log("No OpenXR settings found for Android");
        }
        
        // Check Standalone settings too
        var standaloneSettings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(BuildTargetGroup.Standalone);
        if (standaloneSettings != null && standaloneSettings.Manager != null)
        {
            Debug.Log("\n=== XR Settings for Standalone ===");
            Debug.Log("Active Loader: " + standaloneSettings.Manager.activeLoader);
            if (standaloneSettings.Manager.activeLoader != null)
            {
                Debug.Log("Active Loader Name: " + standaloneSettings.Manager.activeLoader.name);
            }
        }
    }
}