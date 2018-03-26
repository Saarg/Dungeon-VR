using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphicsMenu : MonoBehaviour {

	[Header("Other menus")]
	[SerializeField] private GameObject mainMenu;
	[SerializeField] private Button backBtn;

	[Header("Dropdowns")]
	[SerializeField] private Dropdown texturesDropdown;
	[SerializeField] private Dropdown aaDropdown;

	[Header("Toggle")]
	[SerializeField] private Toggle motionBlueToggle;
	[SerializeField] private Toggle aoToggle;


	void Start () {}
	void Update () {}

	public void ChangeTextures(){
		QualitySettings.SetQualityLevel (texturesDropdown.value);
	}

	public void ChangeAA(){
		QualitySettings.antiAliasing = (int) Mathf.Pow (2f, aaDropdown.value);
	}

	public void ToggleMB(){
		Debug.Log (motionBlueToggle.isOn);
	}

	public void ToggleAO(){
		Debug.Log (aoToggle.isOn);
	}

	public 	void BackToMainMenu(){
		this.gameObject.SetActive (false);
		mainMenu.SetActive (true);
	}
}
