using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveMonsterUI : MonoBehaviour {

	[SerializeField, HideInInspector]
	Sprite casterSprite;
	[SerializeField, HideInInspector]
	Sprite leshenSprite;
	[SerializeField, HideInInspector]
	Sprite mutantSprite;
	[SerializeField, HideInInspector]
	Sprite oursGarouSprite;

	[SerializeField]
	Image image;
	[SerializeField]
	Text monsterName;

	public void setMonster(string monsterName) {
		this.monsterName.text = monsterName;

		if (monsterName.Contains("caster")) {
			image.sprite = casterSprite;
		} else if (monsterName.Contains("leshen")) {
			image.sprite = leshenSprite;
		} else if (monsterName.Contains("mutant")) {
			image.sprite = mutantSprite;
		} else if (monsterName.Contains("ours-garou")) {
			image.sprite = oursGarouSprite;
		}
	}

	public void setPos(int i) {
		(transform as RectTransform).anchoredPosition = new Vector3(0, -100 * (i + 1), 0);
	}
}
