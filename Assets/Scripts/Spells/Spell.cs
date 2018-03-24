using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public abstract class Spell : NetworkBehaviour {

	[Header("Description")]
	[SerializeField] protected float lastActivation;
	[SerializeField] protected float manaCost;
	[SerializeField] protected float range;		//if range is 0, spell will be apply to caster
	[SerializeField] protected Living _caster;
	public Living caster { get { return _caster; } set{ 
		if (_caster != null)
			_caster.OnDamage -= Cancel;
		value.OnDamage += Cancel;
		_caster = value;

		if (targetingSystem != null)
			targetingSystem.callingObject = _caster;
	} }
	[SerializeField] protected Targeting targetingSystem;
	[SerializeField] protected Vector3 target; //if necessary
	[SerializeField] protected Quaternion targetRotation;
	[SerializeField] protected GameObject placeholder;

	[Header("Timers")]
	[SerializeField] protected float castingTime;
	[SerializeField] protected float cooldown;
	[SerializeField] protected MoveStatus targetingMovement = MoveStatus.Casting;
	[SerializeField] protected MoveStatus castingMovement = MoveStatus.Casting;
	[SerializeField] protected MoveStatus effectMovement = MoveStatus.Ralenti;
	

	private Animator _animator;
    private NetworkAnimator _netAnimator;

	public Image castingBar { protected get; set;}

	private Coroutine castingCoroutine;

	protected virtual void Start(){
		lastActivation = cooldown;

		_animator = GetComponent<Animator>();
        _netAnimator = GetComponent<NetworkAnimator>();
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
		if (caster.isTargeting) {
			Debug.Log ("Already targeting for a spell");
			return;
		}

		caster.ApplyMoveStatus (targetingMovement);

		if (range > 0) {
			caster.isTargeting = true;
			StartCoroutine (targetingSystem.AcquireTarget (range, () => {
				CmdCast(targetingSystem.getTarget (), targetingSystem.getTargetRotation ());
			}, () => {
				caster.ApplyMoveStatus (MoveStatus.Free);
			}));
		} else {
			CmdCast(Vector3.zero, Quaternion.identity);
		}
	}

	[Command]
	void CmdCast(Vector3 t, Quaternion r){
		RpcStartCasting(t, r);
	}

	[ClientRpc]
	void RpcStartCasting(Vector3 t, Quaternion r) {	
		castingCoroutine = StartCoroutine (Casting (t, r));		
	}

	protected IEnumerator Casting(Vector3 t, Quaternion r){
		float castingProgress = 0;
		if (targetingSystem == null || !targetingSystem.Canceled ()) {
			if (castingBar != null) {
				castingBar.fillAmount = castingProgress;
				castingBar.gameObject.SetActive (true);				
			}

			caster.isCasting = true;

			_netAnimator.SetTrigger("Cast");
			if (caster is PlayerController)
				_animator.SetInteger("anim", (int)((caster as PlayerController).playerClassID) );

			caster.isCasting = true;
			caster.ApplyMoveStatus (castingMovement);

			//TODO add animation
			//TODO add sound

			if(range > 0){
				target = t;
				targetRotation = r;
				placeholder = targetingSystem.getPlaceholder();
			}

			while(castingProgress < 1f){
				castingProgress += Time.deltaTime / castingTime;

				if (castingBar != null)
					castingBar.fillAmount = castingProgress;
				
				yield return 0;
			}

			ApplyEffect ();

			if (castingBar != null)
				castingBar.gameObject.SetActive (false);
			caster.isCasting = false;
		}
	}

	public void ApplyEffect (){
		if (isServer)
			caster.curMana -= manaCost;

		Effects ();
		lastActivation = 0;
	}

	protected virtual void Effects() {
		if (hasAuthority) {
			caster.ApplyMoveStatus (MoveStatus.Free);
		}
	}

	protected virtual void EndEffects() {
	}

	public void Cancel(int damage = 0, Bullet.DamageTypeEnum damageType = Bullet.DamageTypeEnum.physical) {
		if (isLocalPlayer || hasAuthority)
			CmdCancel();
		else
			RpcCancel();
	}

	[Command]
	void CmdCancel() {
		RpcCancel();
	}

	[ClientRpc]
	void RpcCancel() {
		if (targetingSystem != null)
			targetingSystem.Cancel();

		EndEffects();

		if (castingCoroutine != null) {
			StopCoroutine(castingCoroutine);

			if (castingBar != null)
				castingBar.gameObject.SetActive (false);
			caster.isCasting = false;
		}

		_netAnimator.SetTrigger("Cancelled");
		caster.ApplyMoveStatus (MoveStatus.Free);
	}
}
