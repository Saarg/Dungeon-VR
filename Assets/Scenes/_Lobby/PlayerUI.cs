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

	public static PlayerUI localPlayer;

	private Text pName;
	private Image classLogo;

	void Start () {
		curPlayer = playerCount++;

		pName = transform.Find("Name").GetComponent<Text>();
		classLogo = transform.Find("ClassLogo").GetComponent<Image>();

		if (curPlayer == 0) {
			CmdSelectClass(4);

			transform.position = new Vector3(-300, 50, 800);
			pName.text = "GameMaster";
			gameObject.name = "GameMaster";
		} else {
			classLogo.sprite = null;

			transform.position = new Vector3(100 * (curPlayer-1), 50, 800);
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
	}
}
