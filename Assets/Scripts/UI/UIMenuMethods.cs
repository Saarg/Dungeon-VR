using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 	Class used in menus tu give simple acces in unityevents to some methods
/// </summary>
public class UIMenuMethods : MonoBehaviour {

	/// <summary>
	/// 	Load async scene, default single mode
	/// </summary>
	/// <param name="name">scene name or path</param>  
	public void LoadScene(string name) {
		SceneManager.LoadSceneAsync(name, LoadSceneMode.Single);
	}

	/// <summary>
	/// 	Set ip in player pref
	/// </summary>
	/// <param name="ip">ip for remote server</param>  
	public void SetServerIp(string ip) {
		PlayerPrefs.SetString("ip", ip);
	}

	/// <summary>
	/// 	Application quit
	/// </summary>
	public void Quit() {
		Application.Quit();
	}
}
