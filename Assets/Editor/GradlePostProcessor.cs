using System.IO;
using UnityEditor.Android;

public class GradlePostProcessor : IPostGenerateGradleAndroidProject
{
    public int callbackOrder => 0;

    public void OnPostGenerateGradleAndroidProject(string path)
    {
        string launcherBuildGradle = Path.Combine(path, "launcher", "build.gradle");
        if (File.Exists(launcherBuildGradle))
        {
            string content = File.ReadAllText(launcherBuildGradle);
            
            // Check if packagingOptions already has our exclusions
            if (!content.Contains("libopenxr_loader.so"))
            {
                // Add packagingOptions exclusions to the android block
                string exclusionBlock = @"
        packagingOptions {
            // Exclude duplicate libopenxr_loader.so from openxr_loader package
            // since Meta XR SDK's OVRPlugin already provides it
            exclude 'lib/arm64-v8a/libopenxr_loader.so'
            exclude 'lib/armeabi-v7a/libopenxr_loader.so'
            exclude 'lib/x86/libopenxr_loader.so'
            exclude 'lib/x86_64/libopenxr_loader.so'
        }";
                
                // Insert after the first "android {" block
                int androidIndex = content.IndexOf("android {");
                if (androidIndex >= 0)
                {
                    int insertIndex = content.IndexOf("\n", androidIndex) + 1;
                    content = content.Insert(insertIndex, exclusionBlock);
                    File.WriteAllText(launcherBuildGradle, content);
                    UnityEngine.Debug.Log("Added libopenxr_loader.so exclusions to launcher/build.gradle");
                }
                else
                {
                    UnityEngine.Debug.LogWarning("Could not find android block in launcher/build.gradle");
                }
            }
        }
    }
}