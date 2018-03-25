using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Living {

	[SerializeField] protected Animator animator;

    BaseAI ai;

	void Start(){
        ai = gameObject.GetComponent<BaseAI>();
        animator = ai.getAnimator();

	}

	override public void TakeDamage(int damage, Bullet.DamageTypeEnum damageType){

		if (animator != null)
			animator.SetTrigger ("hit");

        ai.InterruptAction();

		base.TakeDamage (damage, damageType);
	}
}
