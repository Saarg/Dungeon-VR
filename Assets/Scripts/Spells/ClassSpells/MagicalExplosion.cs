using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicalExplosion : MonoBehaviour {

	[SerializeField] private GameObject explosion;
	[SerializeField] private GameObject firering;

	[SerializeField] private int damage = 20;
	[SerializeField] private bool isBurning = false;
	[SerializeField] private float burningTime = 7f;
    int interval = 1;
    float nextTime = 0;

    void Start(){
		Instantiate (explosion, gameObject.transform);
		GameObject ring = Instantiate (firering, gameObject.transform);
		ring.GetComponent<DigitalRuby.PyroParticles.FireConstantBaseScript> ().Duration = burningTime;
        StartCoroutine ("Burning");
    }

	void FixedUpdate(){
        if (isBurning && Time.time >= nextTime)
        {
            Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, 3);
            foreach (Collider col in hitColliders)
            {
                if (col.gameObject.CompareTag("Player"))
                {
                    col.GetComponent<Living>().TakeDamage(damage, Bullet.DamageTypeEnum.fire);
                }
            }
            nextTime += interval;
        }
    }

	IEnumerator Burning(){
		isBurning = true;
        yield return new WaitForSeconds (burningTime);
		DestroyExplosion ();
	}

	void OnTriggerEnter (Collider _col){
		if (!_col.gameObject.CompareTag ("Player") && isBurning) {
			_col.GetComponent<Living>().TakeDamage(damage, Bullet.DamageTypeEnum.fire);
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
