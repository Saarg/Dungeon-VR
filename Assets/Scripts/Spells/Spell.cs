using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spell : MonoBehaviour {

	[Header("Description")]
	protected float lastActivation = 0;
	public float manaCost;

	[Header("Timers")]
	public float castingTime;
	protected float cooldown;

	[Header("Progress Bars")]
	protected ProgressBar castingBar;

	public bool IsReady(){
		return (lastActivation >= cooldown);
	}
		
	public void ApplyEffect (GameObject caster){
		Effects (caster);
		lastActivation = 0;
	}

	protected abstract void Effects(GameObject caster);

	protected virtual void Update(){
		lastActivation += Time.deltaTime;
	}

	protected virtual void DestroySpell(){
		GameObject.Destroy(this.gameObject);
	}
}
