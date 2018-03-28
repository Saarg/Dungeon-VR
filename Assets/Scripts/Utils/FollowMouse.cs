using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour {

	[SerializeField]
	Transform[] objects;
	[SerializeField]
	Vector3 scale = Vector3.one;	

	void Update () {
		if (Time.frameCount%2 == 1)
			return;

		Vector3 locPos = (Input.mousePosition - new Vector3(Screen.width/2, Screen.height/2, 0)) / 100f + Vector3.forward * 5f;
		locPos.Scale(scale);

		foreach (Transform t in objects) {
			t.localPosition = locPos;
		}
	}
}
