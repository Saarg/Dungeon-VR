﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targeting : MonoBehaviour {

	[SerializeField] private Living callingObject;
	[SerializeField] private Projector projo;
	[SerializeField] private Vector3 target;

	public Vector3 getTarget(){ return target; }

	private bool targeting;
	private float height; 		//height of the projector

	void Start () {
		projo.gameObject.SetActive (false);
	}
	
	void Update () {}

	public IEnumerator AcquireTarget(float range, KeyCode key){
		targeting = true;
		projo.transform.localPosition = new Vector3(0, height, range);
		projo.gameObject.SetActive (true);

		while (!Input.GetKeyDown (key)) {
			yield return 0;
		}

		printTarget ();

		targeting = false;
		projo.gameObject.SetActive (false);
		target = projo.transform.position + new Vector3(0,-height,0);
		callingObject.isTargeting = false;
	}

	private void printTarget(){
		GameObject newborn = Instantiate (projo.gameObject, projo.transform.position, projo.transform.rotation);
	}
}


