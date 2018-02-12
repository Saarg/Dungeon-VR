using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicalExplosion : MonoBehaviour {

	[SerializeField] private ParticleSystem firering;
	[SerializeField] private ParticleSystem.ShapeModule fireringShape;
	[SerializeField] private SphereCollider aoe;
	[SerializeField] private int damage;
	[SerializeField] private bool isBurning = false;

	private float fireringMaxRadius = 7f;
	private float aoeMaxRadius = 1.8f;

//	[SerializeField] private float spawningTime;
	[SerializeField] private float burningTime;

	void Start(){
		fireringShape = firering.GetComponent<ParticleSystem> ().shape;

		fireringShape.radius = 0;
		aoe.radius = 0;
	}

	void Update(){
		if (fireringShape.radius < fireringMaxRadius) GrowRadius ();
		else StartCoroutine ("Burning");
	}

	private void GrowRadius(){
		fireringShape.radius += fireringMaxRadius * Time.deltaTime;
		aoe.radius += aoeMaxRadius * Time.deltaTime;
	}

	IEnumerator Burning(){
		isBurning = true;
		float t = 0;
		while (t < burningTime) {
			t += Time.deltaTime;
			yield return 0;
		}
		DestroyExplosion ();
	}

	void OnTriggerEnter(Collider _col){
		if (_col.gameObject.CompareTag ("Enemy") && isBurning) {
			_col.gameObject.GetComponent<Living> ().TakeDamage(damage, Bullet.DamageTypeEnum.fire);
		}
	}

	private void DestroyExplosion(){
		GameObject.Destroy (this.gameObject);
	}
}
