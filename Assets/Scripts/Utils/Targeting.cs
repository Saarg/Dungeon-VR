using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targeting : MonoBehaviour {

	[SerializeField] private Living callingObject;
	[SerializeField] private Projector projo;
	[SerializeField] private Vector3 target;

	private GameObject placeholder;

	public Vector3 getTarget(){ return target; }
	public GameObject getPlaceholder(){ return placeholder; }

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

		//print a clone of the projector so the player knows where is the targeting point
		placeholder = Instantiate (projo.gameObject, projo.transform.position, projo.transform.rotation);

		targeting = false;
		projo.gameObject.SetActive (false);
		target = projo.transform.position + new Vector3(0,-height,0);
		callingObject.isTargeting = false;
	}
}


