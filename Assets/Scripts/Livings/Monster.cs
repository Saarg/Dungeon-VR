using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Living {

	[SerializeField] protected Animator animator;

	override public void TakeDamage(int damage, Bullet.DamageTypeEnum damageType){
		if (animator != null)
			animator.SetTrigger ("hit");

		base.TakeDamage (damage, damageType);
	}
}
