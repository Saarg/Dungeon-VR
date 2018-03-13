using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targeting : MonoBehaviour {

	Living _callingObject;
	public Living callingObject { 
		internal get { return _callingObject; }
		set { 
			if (_callingObject != null)
				_callingObject.OnDamage -= Cancel;
			value.OnDamage += Cancel;
			_callingObject = value;
		}
	}
	[SerializeField] private Projector projo;
	[SerializeField] private Vector3 target;
	[SerializeField] private Quaternion targetRotation;

	private GameObject placeholder;
	private bool canceled;

	public Vector3 getTarget(){ return target; }
	public Quaternion getTargetRotation(){ return targetRotation; }
	public GameObject getPlaceholder(){ return placeholder; }
	public bool Canceled(){ return canceled; }
	public void Cancel(int damage = 0, Bullet.DamageTypeEnum damageType = Bullet.DamageTypeEnum.physical){ if (targeting) { canceled = true; } }

	private bool targeting;
	private float height; 		//height of the projector

	void Start () {
		projo.gameObject.SetActive (false);
		canceled = false;
	}
	
	void Update () {}

	public IEnumerator AcquireTarget(float range, Action cast, Action cancel){
		targeting = true;
		canceled = false;
		projo.transform.localPosition = new Vector3(0, height, range);
		projo.gameObject.SetActive (true);

		while (!Input.GetButtonDown ("Fire3")) {
			if(Input.GetKeyDown(KeyCode.Escape) || canceled){
				canceled = true;
				cancel();

				break;
			}
			
			yield return 0;
		}

		if(Input.GetButtonDown ("Fire3") && !canceled){
			//print a clone of the projector so the player knows where is the targeting point
			placeholder = Instantiate (projo.gameObject, projo.transform.position, projo.transform.rotation);
			target = projo.transform.position + new Vector3(0,-height,0);
			targetRotation = callingObject.transform.rotation;
		}

		targeting = false;
		projo.gameObject.SetActive (false);
		callingObject.isTargeting = false;

		cast();
	}
}


