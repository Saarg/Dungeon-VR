using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LobbyManager : MonoBehaviour {
	public string VRPlayScene;

	public GameObject classButtons;
	public GameObject readyButton;

	private NetworkLobbyManager nlm;

	void Start() {
		readyButton.SetActive(false);
		classButtons.SetActive(false);

		nlm = GetComponent<NetworkLobbyManager>();
	}

	public void SelectClass(int c) {
		PlayerUI.localPlayer.CmdSelectClass(c);
	}

	public void Ready() {
		PlayerUI.localPlayer.SendReadyToBeginMessage();
	}

	public void NotReady() {
		PlayerUI.localPlayer.SendNotReadyToBeginMessage();
	}

	void Update () {
		if (PlayerUI.localPlayer != null && PlayerUI.playerCount > nlm.minPlayers) {
			classButtons.SetActive(PlayerUI.localPlayer.curPlayer != 0);

			readyButton.SetActive(true);

			if (PlayerUI.localPlayer.curPlayer == 0)
				nlm.playScene = VRPlayScene;
		}
	}
}
