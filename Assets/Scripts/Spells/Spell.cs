using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spell : MonoBehaviour {

	[Header("Description")]
	[SerializeField] protected float lastActivation;
	[SerializeField] protected float manaCost;
	[SerializeField] protected float range;		//if range is 0, spell will be apply to caster
	[SerializeField] protected Living caster;

	[Header("Timers")]
	[SerializeField] protected float castingTime;
	[SerializeField] protected float cooldown;

	[Header("Progress Bars")]
	[SerializeField] protected ProgressBar castingBar;

	protected virtual void Start(){
		lastActivation = cooldown;
	}

	protected virtual void Update(){
		lastActivation += Time.deltaTime;
	}

	public bool IsReady(){
		return (lastActivation >= cooldown);
	}

	protected virtual void DestroySpell(){
		GameObject.Destroy(this.gameObject);
	}

	public void Cast(){
		if (caster.isCasting) {
			Debug.Log ("Already casting");
			return;
		}
		if (caster.curMana < manaCost) {
			Debug.Log ("Not enough mana");
			return;
		}
		if (!IsReady ()) {
			Debug.Log ("Spell is not ready");
			return;
		}

		StartCoroutine (Casting ());
	}

	protected IEnumerator Casting(){
		castingBar.MinValue = 0;
		castingBar.MaxValue = castingTime;
		castingBar.StartValue = 0;
		castingBar.CurrentValue = 0;
		castingBar.Complete = false;

		caster.isCasting = true;
		this.GetComponent<Living> ().CmdApplyMoveStatus (MoveStatus.Casting);

		castingBar.gameObject.SetActive (true);

		//TODO add animation
		//TODO add sound

		while(!castingBar.Complete){
			castingBar.Progress (Time.deltaTime);
			yield return 0;
		}

		ApplyEffect ();

		castingBar.gameObject.SetActive (false);
		caster.isCasting = false;
	}

	public void ApplyEffect (){
		caster.curMana -= manaCost;
		Effects ();
		lastActivation = 0;
	}

	protected abstract void Effects();
}
