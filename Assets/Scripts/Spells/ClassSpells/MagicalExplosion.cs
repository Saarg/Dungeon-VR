using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicalExplosion : MonoBehaviour {

	[SerializeField] private GameObject explosion;
	[SerializeField] private GameObject firering;

	[SerializeField] private int damage;
	[SerializeField] private bool isBurning = false;
	[SerializeField] private float burningTime;

	void Start(){
		Instantiate (explosion, gameObject.transform);
		GameObject ring = Instantiate (firering, gameObject.transform);
		ring.GetComponent<DigitalRuby.PyroParticles.FireConstantBaseScript> ().Duration = burningTime;
		StartCoroutine ("Burning");
	}

	void Update(){}

	IEnumerator Burning(){
		isBurning = true;
		yield return new WaitForSeconds (burningTime);
		DestroyExplosion ();
	}

	void OnTriggerEnter(Collider _col){
		if (_col.gameObject.CompareTag ("Player") && isBurning) {
			_col.gameObject.GetComponent<Living> ().TakeDamage(damage, Bullet.DamageTypeEnum.fire);
		}
	}

	private void DestroyExplosion(){
		isBurning = false;
		StartCoroutine ("Destruction");	
	}

	IEnumerator Destruction(){
		yield return new WaitForSeconds (5); //just to be sure that all the fire effects have faded
		Destroy (this.gameObject);
	}
}
