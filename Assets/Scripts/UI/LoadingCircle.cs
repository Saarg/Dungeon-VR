using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class LoadingCircle : MonoBehaviour {

	Image image;
	float startTime;

	// Use this for initialization
	void Start () {
		image = GetComponent<Image>();

		startTime = Time.realtimeSinceStartup;
	}
	
	// Update is called once per frame
	void Update () {
		float t = (Time.realtimeSinceStartup - startTime) % 2;

		image.fillClockwise = t < 1;
		image.fillAmount = Mathf.Abs(t - 1);
	}
}
