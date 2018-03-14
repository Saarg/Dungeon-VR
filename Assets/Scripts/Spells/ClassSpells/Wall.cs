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
	[SerializeField] private float spawningTime;

	private float height = 2.5f;
	private bool isBuilt = false;
	private bool isDestroying = false;

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
		this.GetComponent<Rigidbody> ().isKinematic = true;

		CreateWall ();
	}
		
	void Update () {
		if(isBuilt)
			DecreaseLifetime ();

		if ((lifetime <= 0 || health <= 0) && !isDestroying)
			StartCoroutine ("Destroying");
	}

	private void DecreaseLifetime(){
		lifetime -= Time.deltaTime;
		lifetimeBar.Progress (Time.deltaTime * -1);
	}

	public void TakeDamage(float damages){
		health -= damages;
		healthBar.Progress (damages * -1);
	}

	private void CreateWall(){
		transform.position = new Vector3 (transform.position.x, transform.position.y - height, transform.position.z);

		StartCoroutine ("Creating");
	}

	protected IEnumerator Creating(){
		float t = 0;
		while (t < spawningTime) {
			transform.position = new Vector3 (transform.position.x, transform.position.y + height*Time.deltaTime, transform.position.z);
			t += Time.deltaTime;
			yield return 0;
		}

		isBuilt = true;
		this.GetComponent<Rigidbody> ().isKinematic = true;
	}

	protected IEnumerator Destroying(){
		isDestroying = true;
		isBuilt = false;
		this.GetComponent<Rigidbody> ().isKinematic = true;

		float t = 0;
		while (t < spawningTime) {
			transform.position = new Vector3 (transform.position.x, transform.position.y - height*Time.deltaTime, transform.position.z);
			t += Time.deltaTime;
			yield return 0;
		}

		GameObject.DestroyObject(this.gameObject);
	}

	public void OnTriggerEnter(Collider _col){
		if (_col.gameObject.GetComponent<Bullet> ()) {
			GameObject.DestroyObject (_col.gameObject);
			TakeDamage (_col.gameObject.GetComponent<Bullet> ().Damage);
		}
	}
}
