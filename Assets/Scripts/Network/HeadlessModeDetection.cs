using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(NetworkManager))]

/// <summary>  
/// 	Starts in server mode if the build is headless
/// </summary>
public class HeadlessModeDetection : MonoBehaviour {
	public static bool IsHeadlessMode()
    {
        return UnityEngine.SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null;
    }

	void Start() {
		if (IsHeadlessMode())
			GetComponent<NetworkManager>().StartServer();
	}
}
