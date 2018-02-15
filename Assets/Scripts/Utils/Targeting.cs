using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targeting : MonoBehaviour {

	[SerializeField] private Living callingObject;
	[SerializeField] private Projector projo;
	[SerializeField] private Vector3 target;

	private bool targeting;

	void Start () {
		projo.gameObject.SetActive (false);
	}
	
	void Update () {}

	public IEnumerator AcquireTarget(float range, KeyCode key){
		targeting = true;
		projo.transform.position = callingObject.transform.forward * range;
		projo.transform.position += new Vector3 (0, 2.1f, 0);
		projo.gameObject.SetActive (true);

		while (!Input.GetKeyDown (key)) {
			yield return 0;
		}

		targeting = false;
		projo.gameObject.SetActive (false);
		target = projo.transform.position;
		callingObject.isTargeting = false;
	}

	public Vector3 getTarget(){
		//projection of the projo on the ground
		return target;
	}
}


