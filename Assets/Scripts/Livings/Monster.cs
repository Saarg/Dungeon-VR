using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Living {

	[SerializeField] protected Animator animator;

	void Start(){
		animator = gameObject.GetComponent<BaseAI> ().getAnimator ();
	}

	override public void TakeDamage(int damage, Bullet.DamageTypeEnum damageType){
		if (animator != null)
			animator.SetTrigger ("hit");

		base.TakeDamage (damage, damageType);
	}
}
