using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ParticleEffects : NetworkBehaviour {

	// Use this for initialization
	void Start () {
        Destroy(gameObject, 0.5f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
