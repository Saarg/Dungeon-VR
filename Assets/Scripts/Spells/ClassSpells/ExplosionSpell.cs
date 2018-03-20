using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionSpell : Spell {

	[SerializeField] private GameObject explosionPrefab;

	protected override void Effects(){
		if (placeholder != null)
			Destroy (placeholder.gameObject);
		
		Instantiate(explosionPrefab, target, targetRotation);

		caster.ApplyMoveStatus (MoveStatus.Free);
	}
}
