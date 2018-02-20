using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSpike : Trap
{
    public Animator anim;

    public override void StartTrap()
    {
        anim.SetTrigger("DTP_AnimTrigger_WallTraps_WallSpikeGrate_Impale");
    }

    public override void StopTrap()
    {

    }

}
