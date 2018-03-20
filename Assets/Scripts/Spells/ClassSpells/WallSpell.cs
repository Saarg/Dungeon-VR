using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSpell : Spell {

	[SerializeField] private GameObject wallPrefab;
	[SerializeField] private Transform spawningPoint;

	protected override void Effects(){
		if (placeholder != null)
			Destroy (placeholder.gameObject);
		
		Instantiate(wallPrefab, target, targetRotation);

		caster.ApplyMoveStatus (MoveStatus.Free);
	}
}
