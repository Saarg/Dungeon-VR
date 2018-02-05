using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawbladeTrap : PeriodicTrap {

	[Header("Sawblade specifications")]
	public int damage;
	public PlayerController target;
	
	protected override void Update () {
		base.Update ();

	}

	public void OnSawTriggerEnter(Collider _col){
		if (_col.gameObject.CompareTag ("Player")) {
			target = _col.gameObject.GetComponent<PlayerController>();
			Effect ();
		}
	}

	protected override void Effect(){
		target.TakeDamage (damage, Bullet.DamageTypeEnum.physical);
	}

}
