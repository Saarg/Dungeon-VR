using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Lobby {
	/// <summary>
	/// Script used to control the player's ui during class selection in the Lobby
	/// </summary>
	public class PlayerUI : NetworkLobbyPlayer {

		[SerializeField, HideInInspector]
		private Sprite GMClass;
		[SerializeField, HideInInspector]
		private Sprite tankClass;
		[SerializeField, HideInInspector]
		private Sprite healerClass;
		[SerializeField, HideInInspector]
		private Sprite assasinClass;
		[SerializeField, HideInInspector]
		private Sprite mageClass;

		[SerializeField, HideInInspector]
		private Texture tankCam;
		[SerializeField, HideInInspector]
		private Texture healerCam;
		[SerializeField, HideInInspector]
		private Texture assasinCam;
		[SerializeField, HideInInspector]
		private Texture mageCam;

		public int curPlayer;
		public int curClass = 5;

		public static PlayerUI localPlayer;
		/// <remark>True if the gamemaster is in lobby</remark>
		public static bool gameMaster;
		public static int minPlayers;
		public static int playerCount = 0;

		public GameObject classButtons;
		public GameObject readyButtons;

		public Text pName;
		public Image classLogo;
		public RawImage playerFace;

		void Awake()
		{
			if (NetworkManager.singleton is CustomNetworkManager) {
				(NetworkManager.singleton as CustomNetworkManager).playerConnectDelegate += AddClient;
			} else if (NetworkManager.singleton is LobbyManager) {
				(NetworkManager.singleton as LobbyManager).playerConnectDelegate += AddClient;
			}
		}
		
		void Start () {
			transform.SetParent(GameObject.Find("UI").transform);

			curPlayer = playerCount++;

			readyButtons.SetActive(false);				

			if (!isLocalPlayer) {
				classButtons.SetActive(false);
			}

			if (curPlayer == 0) {
				curClass = 4;

				pName.transform.parent.localPosition = new Vector3(-300, 50, 0);
				pName.text = "GameMaster";
				gameObject.name = "GameMaster";

				PlayerPrefs.SetInt("isGameMaster", 1);

				gameMaster = true;

				classButtons.SetActive(false);
			} else {
				curClass = curPlayer - 1;				

				pName.transform.parent.localPosition = new Vector3(130 * (curPlayer-1), 50, 0);
				pName.text = "Player " + curPlayer;
				gameObject.name = "Player " + curPlayer;

				PlayerPrefs.SetInt("isGameMaster", 0);				
			}	

			if (isLocalPlayer) {
				if (minPlayers <= playerCount)
					readyButtons.SetActive(true);
				CmdSelectClass(curClass);
			}
		}

		/// <summary>
		/// OnStartLocalPlayer set the static localplayer for the client
		/// </summary>
		public override void OnStartLocalPlayer() {
			localPlayer = this;
		}

		void Update()
		{
			if (isLocalPlayer && minPlayers <= playerCount)
				readyButtons.SetActive(true);
		}

		/// <summary>
		/// Command to switch class 
		/// </summary>
		/// <param name="c">class integer tank, healer, assasin, mage, gm</param>  
		[Command]	
		public void CmdSelectClass(int c) {
			RpcUpdateSprite(c);
		}

		[TargetRpc]
		void TargetUpdateSprite(NetworkConnection target, int c) {
			UpdateSprite(c);	
		}

		[ClientRpc]
		void RpcUpdateSprite(int c) {
			UpdateSprite(c);		
		}

		void UpdateSprite(int c) {
			if (c < 0) {
				c = curClass;
			}

			switch (c) {
				case 0:
					classLogo.sprite = tankClass;
					playerFace.texture = tankCam;
					break;
				case 1:
					classLogo.sprite = healerClass;
					playerFace.texture = healerCam;
					break;
				case 2:
					classLogo.sprite = assasinClass;
					playerFace.texture = assasinCam;
					break;
				case 3:
					classLogo.sprite = mageClass;
					playerFace.texture = mageCam;
					break;
				case 4:
					classLogo.sprite = GMClass;
					break;
				default:
					classLogo.sprite = null;
					break;
			}

			if (curClass >= 0 && curClass <= 3)
				classButtons.transform.GetChild(curClass).GetComponent<Button>().interactable = true;
			if (c >= 0 && c <= 3)		
				classButtons.transform.GetChild(c).GetComponent<Button>().interactable = false;
			
			curClass = c;		
		}

		public void AddClient(NetworkConnection conn) {
			StartCoroutine(WaitForConnectionIsReady(conn, () => {
				TargetUpdateSprite(conn, curClass);
			}));
		}

		IEnumerator WaitForConnectionIsReady(NetworkConnection conn, Action cb) {
			while(!conn.isReady) {
				yield return new WaitForEndOfFrame();
			}
			cb();
		}
	}
}