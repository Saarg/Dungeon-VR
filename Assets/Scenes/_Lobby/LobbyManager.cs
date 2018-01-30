using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LobbyManager : MonoBehaviour {
	public string VRPlayScene;

	public GameObject classButtons;

	public void SelectClass(int c) {
		Debug.Log(PlayerUI.localPlayer.name);

		PlayerUI.localPlayer.CmdSelectClass(c);
	}

	public void Ready() {
		PlayerUI.localPlayer.SendReadyToBeginMessage();
	}

	public void NotReady() {
		PlayerUI.localPlayer.SendNotReadyToBeginMessage();
	}

	void Update () {
		if (PlayerUI.localPlayer != null && PlayerUI.localPlayer.curPlayer == 0) {
			classButtons.SetActive(false);

			GetComponent<NetworkLobbyManager>().playScene = VRPlayScene;
		} else {
			classButtons.SetActive(true);			
		}
	}
}
