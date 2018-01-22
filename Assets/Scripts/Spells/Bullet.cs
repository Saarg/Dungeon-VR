using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>  
/// 	Basic bullet script destroying the gameobject on collision or 10s
/// </summary>
public class Bullet : MonoBehaviour {

	/// <summary>  
	/// 	Destroy the object avec 10s
	/// </summary>
	void Start() {
		Destroy(gameObject, 10);
	}

	/// <summary>  
	/// 	Destroy on collision
	/// </summary>
	void OnCollisionEnter (Collision col) {
		Destroy(gameObject);
	}
}
