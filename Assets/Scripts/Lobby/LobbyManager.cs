using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Lobby
{
	/// <summary>
	/// Script used to control manage the lobby, ready, not ready, selectclass and other
	/// </summary>
	public class LobbyManager : MonoBehaviour {
		public string VRPlayScene;

		public GameObject classButtons;
		public GameObject readyButton;

		private NetworkLobbyManager nlm;

		/// <summary>
		/// Init the menu
		/// </summary>
		void Start() {
			readyButton.SetActive(false);
			classButtons.SetActive(false);

			nlm = GetComponent<NetworkLobbyManager>();
		}

		/// <summary>
		/// Request class update for localplayer
		/// </summary>
		public void SelectClass(int c) {
			PlayerUI.localPlayer.CmdSelectClass(c);
		}

		/// <summary>
		/// Localplayer is ready
		/// </summary>
		public void Ready() {
			PlayerUI.localPlayer.SendReadyToBeginMessage();
		}

		/// <summary>
		/// Localplayer is not ready
		/// </summary>
		public void NotReady() {
			PlayerUI.localPlayer.SendNotReadyToBeginMessage();
		}

		/// <summary>
		/// show buttons or not according to players
		/// </summary>
		void Update () {
			if (PlayerUI.localPlayer != null && PlayerUI.playerCount > nlm.minPlayers) {
				classButtons.SetActive(PlayerUI.localPlayer.curPlayer != 0);

				readyButton.SetActive(true);

				if (PlayerUI.localPlayer.curPlayer == 0)
					nlm.playScene = VRPlayScene;
			}
		}
	}
}