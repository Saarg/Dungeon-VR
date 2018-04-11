using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveUI : MonoBehaviour {

	[SerializeField, HideInInspector]
	GameObject monsterUIPrefab;

	[SerializeField]
	Text waveName;

	public Wave wave;

	void Start()
	{
		int i = 0;
		foreach (GameObject e in wave.enemy) {
			WaveMonsterUI mui = Instantiate(monsterUIPrefab, transform).GetComponent<WaveMonsterUI>();
			mui.setMonster(e.name);
			mui.setPos(i++);
		}
	}

	public void setPos(int i) {
		name = "Wave " + i.ToString();
		waveName.text = name;

		(transform as RectTransform).anchoredPosition = new Vector3(350 * i, 0, 0); 
	}
}
