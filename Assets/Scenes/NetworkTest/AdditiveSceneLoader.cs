using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdditiveSceneLoader : MonoBehaviour {

	public string[] scenes;

	// Use this for initialization
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
