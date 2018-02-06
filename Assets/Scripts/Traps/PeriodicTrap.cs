using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeriodicTrap : DungeonTrap {

	public Trap activatedPart;

	protected override void Update () {
		base.Update ();

		if (cooldownTime < lastActivation)
			Activation ();
	}

	void Activation(){
		activatedPart.StartTrap ();
		lastActivation = 0;
	}
}
