using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AreaSpell : Spell {

	protected Collider areaOfEffect;
	protected float range;

	protected override void Update(){
		base.Update ();
	}
}
