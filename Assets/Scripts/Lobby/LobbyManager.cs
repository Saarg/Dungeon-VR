using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

namespace Lobby
{
	/// <summary>
	/// Script used to control manage the lobby, ready, not ready, selectclass and other
	/// </summary>
	public class LobbyManager : NetworkLobbyManager {

		public static LobbyManager instance;

		/// <summary>
		/// Start is called on the frame when a script is enabled just before
		/// any of the Update methods is called the first time.
		/// </summary>
		void Start()
		{
			instance = this;
		}

		/// <summary>
		/// SERVER: if not on the lobby, spawn player
		/// </summary>
		public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
		{
			Debug.Log("OnServerAddPlayer");

			// Give access to PlayerUI to minPlayers
			PlayerUI.minPlayers = minPlayers;

			if (SceneManager.GetActiveScene().name == "NetworkTest") {
				GameObject player = GameObject.Instantiate(gamePlayerPrefab);

				NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
			}

			base.OnServerAddPlayer(conn, playerControllerId);
		}

		/// <summary>
		/// CLIENT: position player and apply lobby settings
		/// </summary>
		public override void OnLobbyClientSceneChanged(NetworkConnection conn) {
			Debug.Log("OnLobbyClientSceneChanged to " + SceneManager.GetActiveScene().name);

			base.OnLobbyClientSceneChanged(conn);
		}

		public override void OnLobbyServerSceneChanged(string sceneName) {
			Debug.Log("OnLobbyServerSceneChanged");

			base.OnLobbyServerSceneChanged(sceneName);
		}

		public override void OnLobbyServerPlayersReady() {
			Debug.Log("OnLobbyServerPlayersReady");	

			base.OnLobbyServerPlayersReady();
		}

		public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer) {
			Debug.Log("OnLobbyServerSceneLoadedForPlayer");	

			return base.OnLobbyServerSceneLoadedForPlayer(lobbyPlayer, gamePlayer);
		}

		public delegate void OnPlayerConnect(NetworkConnection conn);
    	public OnPlayerConnect playerConnectDelegate;

		public override void OnServerConnect(NetworkConnection conn)
		{
			if (playerConnectDelegate != null)
				playerConnectDelegate(conn);
		}
	}
}