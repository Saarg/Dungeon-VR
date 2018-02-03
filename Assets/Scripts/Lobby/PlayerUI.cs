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

		public int curPlayer;
		public int curClass = 5;

		public static PlayerUI localPlayer;
		public static PlayerUI GameMaster;
		public static int minPlayers;
		public static int playerCount = 0;

		public GameObject classButtons;
		public GameObject readyButtons;

		public Text pName;
		public Image classLogo;

		/// <summary>
		/// Start is used to setup parent, name, classLogo and position
		/// </summary>
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

				GameMaster = this;

				classButtons.SetActive(false);
			} else {
				curClass = curPlayer - 1;				

				pName.transform.parent.localPosition = new Vector3(110 * (curPlayer-1), 50, 0);
				pName.text = "Player " + curPlayer;
				gameObject.name = "Player " + curPlayer;

				PlayerPrefs.SetInt("isGameMaster", 0);				
			}	

			StartCoroutine(UpdateData());
		}

		/// <summary>
		/// This function is called when the MonoBehaviour will be destroyed.
		/// </summary>
		void OnDestroy()
		{
			StopAllCoroutines();
		}

		/// <summary>
		/// Update is called every frame, if the MonoBehaviour is enabled.
		/// </summary>
		IEnumerator UpdateData()
		{
			while(isLocalPlayer) {
				CmdSelectClass(curClass);

				// Hardcoded 5 since minPlayers not accesible from here
				if (NetworkLobbyManager.singleton.numPlayers >= minPlayers) {
					readyButtons.SetActive(true);
				}
				yield return new WaitForSeconds(0.5f);
			}
		}

		/// <summary>
		/// OnStartLocalPlayer set the static localplayer for the client
		/// </summary>
		public override void OnStartLocalPlayer() {
			localPlayer = this;
		}

		/// <summary>
		/// Command to switch class 
		/// </summary>
		/// <param name="c">class integer tank, healer, assasin, mage, gm</param>  
		[Command]	
		public void CmdSelectClass(int c) {
			RpcUpdateSprite(c);
		}

		/// <summary>
		/// Rpc updating the player's class
		/// </summary>
		/// <param name="c">class integer tank, healer, assasin, mage, gm or -1 for self update</param>  
		[ClientRpc]
		void RpcUpdateSprite(int c) {
			if (c < 0) {
				c = curClass;
			}

			switch (c) {
				case 0:
					classLogo.sprite = tankClass;
					break;
				case 1:
					classLogo.sprite = healerClass;
					break;
				case 2:
					classLogo.sprite = assasinClass;
					break;
				case 3:
					classLogo.sprite = mageClass;
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
	}
}