using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class BuildAPK
{
    [UnityEditor.MenuItem("Tools/Build APK")]
    public static void Build()
    {
        var buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] { "Assets/Scenes/Main_WalkThrough.unity" };
        buildPlayerOptions.locationPathName = "N:/VR-Projects/VR-Walk-Through/Build/VR-Walkthrough-APK-v.01.apk";
        buildPlayerOptions.target = BuildTarget.Android;
        buildPlayerOptions.options = BuildOptions.None;
        
        var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        
        if (report.summary.result == BuildResult.Succeeded)
        {
            UnityEngine.Debug.Log("Build succeeded! APK: " + buildPlayerOptions.locationPathName);
        }
        else
        {
            UnityEngine.Debug.LogError("Build failed: " + report.summary.result);
            foreach (var step in report.steps)
            {
                foreach (var message in step.messages)
                {
                    if (message.type == LogType.Error)
                    {
                        UnityEngine.Debug.LogError("Build Error: " + message.content);
                    }
                }
            }
        }
    }
}