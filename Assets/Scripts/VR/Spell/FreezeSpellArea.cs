using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeSpellArea : AreaSpell {

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

    }

    protected override void StartAffect(Living living)
    {
        living.ApplyMoveStatus(MoveStatus.Ralenti);
    }

    protected override void Affect()
    {

    }

    protected override void StopAffect(Living living)
    {
        living.ApplyMoveStatus(MoveStatus.Free);
    }
}
