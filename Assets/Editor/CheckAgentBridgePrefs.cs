using UnityEngine;
using UnityEditor;
using System.Linq;

public class CheckAgentBridgePrefs
{
    [MenuItem("Tools/Check AgentBridge Prefs")]
    public static void CheckPrefs()
    {
        Debug.Log("=== AgentBridge EditorPrefs Check ===");
        Debug.Log("RemoteServer_Port: " + EditorPrefs.GetInt("RemoteServer_Port", -1));
        Debug.Log("RemoteServer_AutoStart: " + EditorPrefs.GetBool("RemoteServer_AutoStart", false));
        Debug.Log("RemoteServer_AccessToken: " + EditorPrefs.GetString("RemoteServer_AccessToken", "NOT SET"));
        Debug.Log("AgentBridge_Enabled: " + EditorPrefs.GetBool("AgentBridge_Enabled", false));
        Debug.Log("AgentBridge_SelectedServiceId: " + EditorPrefs.GetString("AgentBridge_SelectedServiceId", "NOT SET"));
        Debug.Log("AgentBridge_VerboseLogging: " + EditorPrefs.GetBool("AgentBridge_VerboseLogging", false));
        
        // Check SessionState
        var wasRunning = SessionState.GetBool("AgentBridge.RemoteServer.WasRunning", false);
        Debug.Log("SessionState WasRunning: " + wasRunning);
        
        // Check port listening
        var port = EditorPrefs.GetInt("RemoteServer_Port", 48735);
        var isListening = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties()
            .GetActiveTcpListeners()
            .Any(endpoint => endpoint.Port == port);
        Debug.Log("OS reports port " + port + " listening: " + isListening);
    }
}