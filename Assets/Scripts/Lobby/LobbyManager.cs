using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using VRTK.Examples.Archery;
using VRTK;

namespace Lobby
{
	/// <summary>
	/// Script used to control manage the lobby, ready, not ready, selectclass and other
	/// </summary>
	public class LobbyManager : NetworkLobbyManager {

		public static LobbyManager instance;

		[SerializeField]
        private GameObject VR_SDK;
        [SerializeField]
        private GameObject VR_Hand;

		[SerializeField]
        private GameObject lobbyUI;
		[SerializeField]
        private GameObject loadingUI;

		public override void OnLobbyStartHost()
		{
			// Debug.Log("OnLobbyStartHost");

			instance = this;
		}

		public override void OnLobbyStopHost()
		{
			// Debug.Log("OnLobbyStopHost");
		}
		
		public override void OnLobbyStartServer()
		{
			// Debug.Log("OnLobbyStartServer");

			instance = this;
		}
		
		public override void OnLobbyServerConnect(NetworkConnection conn)
		{
			// Debug.Log("OnLobbyServerConnect");
		}
		
		public override void OnLobbyServerDisconnect(NetworkConnection conn)
		{
			// Debug.Log("OnLobbyServerDisconnect");
		}
		
		public override void OnLobbyServerSceneChanged(string sceneName)
		{
			// Debug.Log("OnLobbyServerSceneChanged");	
		}
		
		public override GameObject OnLobbyServerCreateLobbyPlayer(NetworkConnection conn, short playerControllerId)
		{
			// Debug.Log("OnLobbyServerCreateLobbyPlayer");
			return Instantiate(lobbyPlayerPrefab.gameObject);			
		}
		
		public override GameObject OnLobbyServerCreateGamePlayer(NetworkConnection conn, short playerControllerId)
		{
			// Debug.Log("OnLobbyServerCreateGamePlayer");
			return Instantiate(gamePlayerPrefab);
		}
		
		public override void OnLobbyServerPlayerRemoved(NetworkConnection conn, short playerControllerId)
		{
			// Debug.Log("OnLobbyServerPlayerRemoved");
		}
		
		public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
		{
			// Debug.Log("OnLobbyServerSceneLoadedForPlayer");

			PlayerController pc = gamePlayer.GetComponent<PlayerController>();
			PlayerUI pui = lobbyPlayer.GetComponent<PlayerUI>();

			return true;
		}
		
		public override void OnLobbyServerPlayersReady()
		{
			// Debug.Log("OnLobbyServerPlayersReady");
			
			loadingUI.SetActive(true);
			ServerChangeScene(playScene);
		}
		
		public override void OnLobbyClientEnter()
		{
			// Debug.Log("OnLobbyClientEnter");

			lobbyUI.SetActive(true);
			loadingUI.SetActive(false);					
		}
		
		public override void OnLobbyClientExit()
		{
			// Debug.Log("OnLobbyClientExit");

			lobbyUI.SetActive(true);			
			lobbyUI.SetActive(false);
		}
		
		public override void OnLobbyClientConnect(NetworkConnection conn)
		{
			// Debug.Log("OnLobbyClientConnect");
		}
		
		public override void OnLobbyClientDisconnect(NetworkConnection conn)
		{
			// Debug.Log("OnLobbyClientDisconnect");
		}
		
		public override void OnLobbyStartClient(NetworkClient client)
		{
			// Debug.Log("OnLobbyStartClient");

			instance = this;
		}
		
		public override void OnLobbyStopClient()
		{
			// Debug.Log("OnLobbyStopClient");
		}
		
		public override void OnLobbyClientSceneChanged(NetworkConnection conn)
		{
			// Debug.Log("OnLobbyClientSceneChanged");	

			loadingUI.SetActive(false);				
		}
		
		public override void OnLobbyClientAddPlayerFailed()
		{
			// Debug.Log("OnLobbyClientAddPlayerFailed");
		}
		
		public delegate void OnPlayerConnect(NetworkConnection conn);
    	public OnPlayerConnect playerConnectDelegate;

		public override void OnServerConnect(NetworkConnection conn)
		{
			if (playerConnectDelegate != null)
				playerConnectDelegate(conn);

			base.OnServerConnect(conn);
		}
	}
}