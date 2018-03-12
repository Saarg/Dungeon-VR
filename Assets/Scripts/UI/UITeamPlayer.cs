using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITeamPlayer : MonoBehaviour {

	[SerializeField, HideInInspector]
	Sprite tankClass;
	[SerializeField, HideInInspector]
	Sprite healerClass;
	[SerializeField, HideInInspector]
	Sprite assasinClass;
	[SerializeField, HideInInspector]
	Sprite mageClass;

	[SerializeField]
	Image life;
	[SerializeField]
	Image mana;
	[SerializeField]
	Text playerName;
	[SerializeField]
	Image classLogo;

	[SerializeField]
	PlayerController player;

	PlayerController.PlayerClassEnum classPlayer;

	public void SetPlayercontroller(PlayerController p) {
		player = p;

		classPlayer = player.playerClassID;
		switch(classPlayer) {
			case PlayerController.PlayerClassEnum.Tank:
				classLogo.sprite = tankClass;
				break;
			case PlayerController.PlayerClassEnum.Healer:
				classLogo.sprite = healerClass;
				break;
			case PlayerController.PlayerClassEnum.Assassin:
				classLogo.sprite = assasinClass;
				break;
			case PlayerController.PlayerClassEnum.Sorcerer:
				classLogo.sprite = mageClass;
				break;
		}
	}

	void Update()
	{
		life.fillAmount = Mathf.Lerp(life.fillAmount, (float)player.curLife / (float)player.maxLife, Time.deltaTime * 2f);
        mana.fillAmount = Mathf.Lerp(mana.fillAmount, (float)player.CurrentMana / (float)player.MaxMana, Time.deltaTime * 2f);

		if (!playerName.text.Equals(player.gameObject.name) )
			playerName.text = player.gameObject.name;

		if (classPlayer != player.playerClassID) {
			switch(player.playerClassID) {
				case PlayerController.PlayerClassEnum.Tank:
					classLogo.sprite = tankClass;
					break;
				case PlayerController.PlayerClassEnum.Healer:
					classLogo.sprite = healerClass;
					break;
				case PlayerController.PlayerClassEnum.Assassin:
					classLogo.sprite = assasinClass;
					break;
				case PlayerController.PlayerClassEnum.Sorcerer:
					classLogo.sprite = mageClass;
					break;
			}
		}
	}
}
