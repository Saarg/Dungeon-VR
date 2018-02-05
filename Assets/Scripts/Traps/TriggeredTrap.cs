using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TriggeredTrap : DungeonTrap {

	public Collider triggerZone;
	public TriggerEvent triggerEvent;

	protected override void Update(){
		base.Update ();

		//if triggerEvent && isReady
			//applyEffect
	}
}
