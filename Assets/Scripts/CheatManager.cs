using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheatManager : MonoBehaviour {
	
	[SerializeField]
	GameUI gameUI;

	void Start()
	{
		if (gameUI == null)
			gameUI = FindObjectOfType<GameUI>();
	}

	// Update is called once per frame
	void Update () {
		if (SceneManager.GetActiveScene().buildIndex == 0) {
			if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.T)) {
				SceneManager.LoadScene("PlayerTestScene");
			}
		} else if (gameUI != null) {
			if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.L)) {
				PlayerController pc = gameUI.GetPlayerController();
				pc.curLife = pc.maxLife;
			} else if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.M)) {
				PlayerController pc = gameUI.GetPlayerController();
				pc.curMana = pc.maxMana;
			}
		}
	}
}
