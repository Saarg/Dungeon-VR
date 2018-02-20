using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeriodicTrap : DungeonTrap {

	protected override void Update () {
		base.Update ();

		if (cooldownTime < lastActivation)
            trap.StartTrap();
	}

}
