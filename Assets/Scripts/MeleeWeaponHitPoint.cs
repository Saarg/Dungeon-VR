using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class MeleeWeaponHitPoint : MonoBehaviour {

	[SerializeField]
	Living owner;
	[SerializeField]
	int damage = 5;
	[SerializeField]
	float cooldown = 0.2f;	
	float lastTrigger = 0;
	[SerializeField]
	Bullet.DamageTypeEnum damageType = Bullet.DamageTypeEnum.physical;

	Living lastLiving;

	private void OnTriggerEnter(Collider col)
    {
        if (col.isTrigger)
            return;

		Living comp = col.gameObject.GetComponent<Living>();
		if (comp != null && comp.tag.Equals("Player") && Time.realtimeSinceStartup - lastTrigger > cooldown) {
			comp.TakeDamage(damage, damageType);

			lastLiving = comp;
			lastTrigger = Time.realtimeSinceStartup;
		}
		
		if (lastLiving != null && lastLiving.tag.Equals("Player")) {
			Vector3 dir = transform.position - col.transform.position;
			dir.Normalize();
			lastLiving.HitReaction(col, dir , col.ClosestPoint(transform.position));
		}
    }
}
