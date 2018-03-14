using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponHitPoint : MonoBehaviour {

	[SerializeField]
	Living owner;
	[SerializeField]
	int damage = 5;
	[SerializeField]
	Bullet.DamageTypeEnum damageType = Bullet.DamageTypeEnum.physical;

	private void OnTriggerEnter(Collider col)
    {
        if (col.isTrigger)
            return;

		Living comp = col.gameObject.GetComponent<Living>();
		if (comp != null && comp != owner)
			comp.TakeDamage(damage, damageType);
    }
}
