using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(NetworkManager))]

/// <summary>
/// autoconnect or not according to headless mode or playerprefs
/// </summary>
public class AutoConnect : MonoBehaviour {

	public bool startServer = false;

	void Start () {
		#if UNITY_EDITOR
			GetComponent<NetworkManagerHUD>().enabled = true;
		#else
			GetComponent<NetworkManagerHUD>().enabled = false;			
		#endif

		if (HeadlessModeDetection.IsHeadlessMode()) {
			if (GetComponent<NetworkManager>() != null)
				GetComponent<NetworkManager>().StartServer();
			else
				GetComponent<NetworkLobbyManager>().StartServer();
		}
		else if (startServer || PlayerPrefs.GetInt("isGameMaster") != 0) {
			if (GetComponent<NetworkManager>() != null)
				GetComponent<NetworkManager>().StartHost();
			else
				GetComponent<NetworkLobbyManager>().StartHost();
		}
		else if (PlayerPrefs.GetString("ip") != null && PlayerPrefs.GetString("ip") != "") {
			if (GetComponent<NetworkManager>() != null) {			
				NetworkManager nm = GetComponent<NetworkManager>();

				nm.networkAddress = PlayerPrefs.GetString("ip");
				nm.StartClient();
			} else {
				NetworkLobbyManager nm = GetComponent<NetworkLobbyManager>();

				nm.networkAddress = PlayerPrefs.GetString("ip");
				nm.StartClient();
			}
		}
	}

}
