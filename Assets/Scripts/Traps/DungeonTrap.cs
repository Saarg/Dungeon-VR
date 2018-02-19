﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>  
/// 	Mother class of every trap
/// </summary>
public abstract class DungeonTrap : MonoBehaviour {

	[Header("Description")]
	public bool isBuilt = false;		//if true the trap is set
	public bool isActive = false; 		//if false, the trap will never be triggered
	public bool isDestroyable;			//if true, the trap can be attacked and destroyed 
	public bool isDesactivable;			//if true, the trap can be desactivated
	public bool isDestroyedAfterUse;	//if true, the trap will be destroyed after effects have been applied
	public float health;
	public float maxHealth;
	public int damage;

	[Header("Timers")]
	public float buildingTime;		//time needed for the trap to be built
	public float activationTime;	//time between the trigger is fired and the effects are applied
	public float desactivationTime;	//time needed for the trap to be desactivated either by the DM or an adventurer
	public float cooldownTime;		//min time between 2 trap activation
	public float lastActivation;

	[Header("Progress Bars")]
	public ProgressBar healthBar;
	public ProgressBar buildingBar;
	public ProgressBar activationBar;
	public ProgressBar desactivationBar;

	void Start(){
		healthBar.MaxValue = maxHealth;
		healthBar.StartValue = maxHealth;
		healthBar.CurrentValue = maxHealth;
		health = maxHealth;

		buildingBar.MaxValue = buildingTime;
		buildingBar.CurrentValue = 0;

		activationBar.MaxValue = activationTime;
		activationBar.CurrentValue = 0;

		desactivationBar.MaxValue = desactivationTime;
		desactivationBar.CurrentValue = 0;

		lastActivation = 0;
	}

	protected virtual void Update(){
		if (Input.GetKeyDown ("1") && !this.isBuilt) StartCoroutine ("Building");
		if (Input.GetKeyDown ("2") && this.isBuilt && !this.isActive) StartCoroutine ("Activation");
		if (Input.GetKeyDown ("3") && this.isBuilt && this.isActive && this.isDesactivable) StartCoroutine ("Desactivation");
		if (Input.GetKeyDown ("4")) Damage(10);

		lastActivation += Time.deltaTime;
	}

	IEnumerator Building(){
		buildingBar.gameObject.SetActive (true);
		while(!buildingBar.Complete){
			buildingBar.Progress (Time.deltaTime);
			yield return 0;
		}
		this.isBuilt = true;
		buildingBar.gameObject.SetActive (false);
	}

	IEnumerator Activation(){
		activationBar.gameObject.SetActive (true);
		while(!activationBar.Complete){
			activationBar.Progress (Time.deltaTime);
			yield return 0;
		}
		this.isActive = true;
		activationBar.gameObject.SetActive (false);
		desactivationBar.CurrentValue = 0;
	}

	IEnumerator Desactivation(){
		desactivationBar.gameObject.SetActive (true);
		while(!desactivationBar.Complete){
			desactivationBar.Progress (Time.deltaTime);
			yield return 0;
		}
		this.isActive = false;
		desactivationBar.gameObject.SetActive (false);
		activationBar.CurrentValue = 0;
	}

	public void Damage(float damage){
		healthBar.gameObject.SetActive (true);
		health -= damage;
		healthBar.Progress (damage * -1);

		if (health <= 0) DestroyTrap ();
	}

	public void DestroyTrap(){
		if (TrapSpawner.singleton != null) {
			TrapSpawner.singleton.DestroyTrap(gameObject);
		} else {
			// DestroyObject (this.gameObject);
		}
	}

	public bool IsReady(){
		return ((lastActivation >= cooldownTime) && this.isActive);
	}

	public void ApplyEffect(){
		if (this.IsReady ()) {
			Effects ();
			lastActivation = 0;

			if (isDestroyedAfterUse) DestroyTrap ();
		}
	}

	protected virtual void Effects(){}

	public virtual void OnDamageAreaTriggerEnter(Collider _col){
		if (this.isDestroyable && _col.gameObject.GetComponent<Bullet> ()) {
			float damage = _col.gameObject.GetComponent<Bullet> ().Damage;

			if (TrapSpawner.singleton != null) {
				TrapSpawner.singleton.DamageTrap(gameObject, damage);
			} else {
				// this.Damage (damage);
			}
		}
	}

	public virtual void OnHurtingAreaTriggerEnter(Collider _col){
		if (this.isActive && _col.CompareTag ("Player")) {
			_col.gameObject.GetComponent<PlayerController>().TakeDamage(this.damage, Bullet.DamageTypeEnum.physical);
		}
	}
}