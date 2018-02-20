using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sawblade : Trap {

    public AnimatePosition animatePosition;

    public override void StartTrap()
    {
        animatePosition.StartAnimation();
    }

    public override void StopTrap()
    {

    }
}
