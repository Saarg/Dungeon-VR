using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSpellArea : AreaSpell {

    [Range(0, 50)]
    public int DamagePerSecond = 5;

    // Use this for initialization
    protected override void Start () {
        base.Start();

	}

    // Update is called once per frame
    protected override void Update () {
        base.Update();

	}

    protected override void StartAffect(Living living)
    {

    }

    protected override void Affect()
    {
        //for each player : lose 10 health
        foreach(Living living in insideAreaLivings)
        {
            living.TakeDamage(DamagePerSecond, Bullet.DamageTypeEnum.fire);
        }
    }

    protected override void StopAffect(Living living)
    {

    }
}
