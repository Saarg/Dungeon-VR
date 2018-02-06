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

	public override void OnHurtingAreaTriggerEnter(Collider _col){
		if (_col.gameObject.CompareTag ("Player")) {
			target = _col.gameObject.GetComponent<PlayerController>();
			Effects ();
		}
	}

	public override void OnDamageAreaTriggerEnter(Collider _col){
		if (_col.gameObject.GetComponent<Bullet> ()) {
			this.Damage (_col.gameObject.GetComponent<Bullet> ().Damage);
		}
	}

	protected override void Effects(){
		target.TakeDamage (damage, Bullet.DamageTypeEnum.physical);
	}

}
