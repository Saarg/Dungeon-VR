using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targeting : MonoBehaviour {

	[SerializeField] private Living callingObject;
	[SerializeField] private Projector projo;
	[SerializeField] private Vector3 target;
	[SerializeField] private Quaternion targetRotation;

	private GameObject placeholder;
	private bool canceled;

	public Vector3 getTarget(){ return target; }
	public Quaternion getTargetRotation(){ return targetRotation; }
	public GameObject getPlaceholder(){ return placeholder; }
	public bool Canceled(){ return canceled; }

	private bool targeting;
	private float height; 		//height of the projector

	void Start () {
		projo.gameObject.SetActive (false);
		canceled = false;
	}
	
	void Update () {}

	public IEnumerator AcquireTarget(float range, KeyCode key){
		targeting = true;
		projo.transform.localPosition = new Vector3(0, height, range);
		projo.gameObject.SetActive (true);

		while (!Input.GetKeyDown (key) && !Input.GetKeyDown(KeyCode.Escape)) {
			yield return 0;
		}

		if(Input.GetKeyDown(key)){
			//print a clone of the projector so the player knows where is the targeting point
			placeholder = Instantiate (projo.gameObject, projo.transform.position, projo.transform.rotation);
			target = projo.transform.position + new Vector3(0,-height,0);
			targetRotation = callingObject.transform.rotation;
		}

		if(Input.GetKeyDown(KeyCode.Escape)){
			canceled = true;
		}

		targeting = false;
		projo.gameObject.SetActive (false);
		callingObject.isTargeting = false;
	}
}


