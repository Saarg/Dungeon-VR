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
	Bullet.DamageTypeEnum damageType = Bullet.DamageTypeEnum.physical;

	Living lastLiving;

	private void OnTriggerEnter(Collider col)
    {
        if (col.isTrigger)
            return;

		Living comp = col.gameObject.GetComponent<Living>();
		if (comp != null && comp.tag.Equals("Player")) {
			comp.TakeDamage(damage, damageType);

			lastLiving = comp;
		}
		
		if (lastLiving != null && lastLiving.tag.Equals("Player")) {
			Debug.Log(col.name);			

			Vector3 dir = transform.position - col.transform.position;
			dir.Normalize();
			lastLiving.HitReaction(col, dir , col.ClosestPoint(transform.position));
		}
    }
}
