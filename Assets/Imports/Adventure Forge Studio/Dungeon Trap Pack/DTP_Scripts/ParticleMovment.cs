using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleMovment : MonoBehaviour {

	Transform Sawblade;

	// Use this for initialization
	void Start () 
	{
		Sawblade = transform.parent;
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.position = new Vector3(Sawblade.position.x, this.transform.position.y, Sawblade.position.z);
	}
}
