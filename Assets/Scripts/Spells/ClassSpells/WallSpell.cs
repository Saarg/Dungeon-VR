using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSpell : Spell {

	[SerializeField] private GameObject wallPrefab;
	[SerializeField] private Transform spawningPoint;

	protected override void Effects(){
//		Instantiate(wallPrefab, spawningPoint.transform.position, spawningPoint.transform.rotation);

		spawningPoint = caster.transform;
		Instantiate(wallPrefab, spawningPoint.position + spawningPoint.transform.forward * range, spawningPoint.rotation);
	}
}
