using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorBlade : Trap {

    public Animator anim1;
    public Animator anim2;
    public Animator anim3;
    public Animator anim4;
    public Animator anim5;

    public override void StartTrap()
    {
        anim1.SetTrigger("DTP_AnimTrigger_FloorTraps_FloorBlades_Slash");
        anim2.SetTrigger("DTP_AnimTrigger_FloorTraps_FloorBlades_Slash");
        anim3.SetTrigger("DTP_AnimTrigger_FloorTraps_FloorBlades_Slash");
        anim4.SetTrigger("DTP_AnimTrigger_FloorTraps_FloorBlades_Slash");
        anim5.SetTrigger("DTP_AnimTrigger_FloorTraps_FloorBlades_Slash");
    }

    public override void StopTrap()
    {

    }
}
