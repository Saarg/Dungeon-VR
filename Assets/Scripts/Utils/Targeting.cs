using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targeting : MonoBehaviour {

	[SerializeField] private Transform callingObject;
	[SerializeField] private Projector projo;
	public Vector3 lastPos;

	private bool targeting;

	void Start () {
		projo.gameObject.SetActive (false);
	}
	
	void Update () {
		if (!targeting && Input.GetKeyDown("3")){
			StartTargeting (5);
			return;
		}

		if (targeting && Input.GetKeyDown("3")) {
			Debug.Log (StopTargeting ());
		}
	}

	public void StartTargeting(float range){
		targeting = true;
		projo.transform.position = callingObject.transform.forward * range;
		projo.transform.position += new Vector3 (0, 2.1f, 0);
		projo.gameObject.SetActive (true);
	}

	public Vector3 StopTargeting(){
		targeting = false;
		projo.gameObject.SetActive (false);
		return projo.transform.position;
	}

}


