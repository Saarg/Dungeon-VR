using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(NetworkManager))]

public class AutoConnect : MonoBehaviour {

	public bool startServer = false;

	void Start () {
		#if UNITY_EDITOR
			GetComponent<NetworkManagerHUD>().enabled = true;
		#else
			GetComponent<NetworkManagerHUD>().enabled = false;			
		#endif

		if (HeadlessModeDetection.IsHeadlessMode() || startServer) {
			GetComponent<NetworkManager>().StartServer();
		}
		else if (PlayerPrefs.GetString("ip") != null && PlayerPrefs.GetString("ip") != "") {
			NetworkManager nm = GetComponent<NetworkManager>();

			nm.networkAddress = PlayerPrefs.GetString("ip");
			nm.StartClient();
		}
	}

}
