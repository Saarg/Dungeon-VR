using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour {

	[SerializeField] private float maxHealth;		//damages the wall can take before being destroyed
	[SerializeField] private float health;
	[SerializeField] private float maxLifetime;		//in sec, time the wall will resist if not destroyed by damages
	[SerializeField] private float lifetime;

	[SerializeField] private ProgressBar healthBar;
	[SerializeField] private ProgressBar lifetimeBar;

	void Start(){
		healthBar.MaxValue = maxHealth;
		healthBar.StartValue = maxHealth;
		healthBar.CurrentValue = maxHealth;
		health = maxHealth;

		lifetimeBar.MaxValue = maxLifetime;
		lifetimeBar.StartValue = maxLifetime;
		lifetimeBar.CurrentValue = maxLifetime;
		lifetime = maxLifetime;

		lifetimeBar.gameObject.SetActive (true);
	}

	void Update () {
		DecreaseLifetime ();

		if (lifetime <= 0 || health <= 0)
			DestroyWall ();
	}

	private void DecreaseLifetime(){
		lifetime -= Time.deltaTime;
		lifetimeBar.Progress (Time.deltaTime * -1);
	}

	public void TakeDamage(float damages){
		health -= damages;
		healthBar.Progress (damages * -1);
	}

	protected void DestroyWall(){
		//TODO make the wall disapear smoohthly
		GameObject.DestroyObject(this.gameObject);
	}

	public void OnTriggerEnter(Collider _col){
		if (_col.gameObject.GetComponent<Bullet> ()) {
			GameObject.DestroyObject (_col.gameObject);
			TakeDamage (_col.gameObject.GetComponent<Bullet> ().Damage);
		}
	}
}
