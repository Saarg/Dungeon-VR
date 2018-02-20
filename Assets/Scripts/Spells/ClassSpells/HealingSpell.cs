using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingSpell : Spell
{
    [SerializeField] private GameObject healingPrefab;
    [SerializeField] private Transform spawningPoint;


    protected override void Effects()
    {
        spawningPoint = caster.transform;
        var aura = Instantiate(healingPrefab, spawningPoint.position, spawningPoint.rotation);
        aura.transform.parent = caster.transform;
    }

}
