using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSpell : AreaSpell {

	[SerializeField] private GameObject wallPrefab;

	protected override void Effects(GameObject caster, GameObject target){
		Vector3 wallSpawningPoint = caster.transform.position + caster.transform.forward*2;
		Instantiate(wallPrefab, wallSpawningPoint, caster.transform.rotation);
	}
}
