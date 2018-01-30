using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

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

	public static int playerCount = 0;
	public int curPlayer;
	public int curClass = 5;

	public static PlayerUI localPlayer;

	private Text pName;
	private Image classLogo;

	void Start () {
		transform.SetParent(GameObject.Find("UI").transform);

		curPlayer = playerCount++;

		pName = transform.Find("Name").GetComponent<Text>();
		classLogo = transform.Find("ClassLogo").GetComponent<Image>();

		if (curPlayer == 0) {
			classLogo.sprite = GMClass;
			CmdSelectClass(4);

			transform.localPosition = new Vector3(-300, 50, 0);
			pName.text = "GameMaster";
			gameObject.name = "GameMaster";
		} else {
			classLogo.sprite = null;

			transform.localPosition = new Vector3(110 * (curPlayer-1), 50, 0);
			pName.text = "Player " + curPlayer;
			gameObject.name = "Player " + curPlayer;
		}
	}

	public override void OnStartLocalPlayer() {
		localPlayer = this;
	}

	

	[Command]	
	public void CmdSelectClass(int c) {
		RpcUpdateSprite(Mathf.Clamp(c, 0, 5));
	}

	[ClientRpc]
	void RpcUpdateSprite(int c) {
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
			NetworkLobbyManager.singleton.GetComponent<LobbyManager>().classButtons.transform.GetChild(curClass).GetComponent<Button>().interactable = true;
		if (c >= 0 && c <= 3)		
			NetworkLobbyManager.singleton.GetComponent<LobbyManager>().classButtons.transform.GetChild(c).GetComponent<Button>().interactable = false;

		curClass = c;
	}
}
