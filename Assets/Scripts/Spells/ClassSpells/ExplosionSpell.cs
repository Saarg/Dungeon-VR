using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionSpell : Spell {

	[SerializeField] private GameObject explosionPrefab;
	[SerializeField] private Transform spawningPoint;

	protected override void Effects(){
//		Instantiate(wallPrefab, spawningPoint.transform.position, spawningPoint.transform.rotation);

		spawningPoint = caster.transform;
		Instantiate(explosionPrefab, spawningPoint.position + spawningPoint.transform.forward * this.range, spawningPoint.rotation);
	}
}
