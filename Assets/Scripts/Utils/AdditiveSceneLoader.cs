using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Script used to load array of additive scenes on Start. WARNING: gameobject gets destroyed at the end
/// </summary>
public class AdditiveSceneLoader : MonoBehaviour {

	public string[] scenes;

	/// <summary>
	/// Load additive scenes then destroy the gameobject
	/// </summary>
	void Start () {
		foreach (string s in scenes)
		{
			if (SceneManager.GetSceneByName(s).isLoaded == false)
				SceneManager.LoadSceneAsync(s, LoadSceneMode.Additive);
			else
				Debug.Log("Scene " + s + " already loaded");
		}

		Destroy(gameObject);
	}
}
