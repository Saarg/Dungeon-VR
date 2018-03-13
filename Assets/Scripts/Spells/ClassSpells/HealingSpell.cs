using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingSpell : Spell
{
    [SerializeField] private GameObject healingPrefab;
    [SerializeField] private GameObject healingGameObject;
    [SerializeField] private Transform spawningPoint;


    protected override void Effects()
    {
        spawningPoint = caster.transform;
        healingGameObject = Instantiate(healingPrefab, spawningPoint.position, spawningPoint.rotation);
        healingGameObject.transform.parent = caster.transform;

        if (hasAuthority) {
			caster.CmdApplyMoveStatus (MoveStatus.Free);
		}
    }

    protected override void EndEffects() {
        Destroy(healingGameObject);
    }
}
