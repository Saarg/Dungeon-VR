using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public abstract class Spell : NetworkBehaviour {

	[Header("Description")]
	[SerializeField] protected float lastActivation;
	[SerializeField] protected float manaCost;
	[SerializeField] protected float range;		//if range is 0, spell will be apply to caster
	[SerializeField] protected Living _caster;
	public Living caster { get { return _caster; } set{ 
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

	public ProgressBar castingBar { protected get; set;}

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

		if (hasAuthority) {
			caster.CmdApplyMoveStatus (targetingMovement);
		}

		if (range > 0) {
			caster.isTargeting = true;
			StartCoroutine (targetingSystem.AcquireTarget (range, () => {
				CmdCast(targetingSystem.getTarget (), targetingSystem.getTargetRotation ());
			}, () => {
				if (hasAuthority) {
					caster.CmdApplyMoveStatus (MoveStatus.Free);
				}
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
		StartCoroutine (Casting (t, r));		
	}

	protected IEnumerator Casting(Vector3 t, Quaternion r){
		if (targetingSystem == null || !targetingSystem.Canceled ()) {
			castingBar.MinValue = 0;
			castingBar.MaxValue = castingTime;
			castingBar.StartValue = 0;
			castingBar.CurrentValue = 0;
			castingBar.Complete = false;

			caster.isCasting = true;
			castingBar.gameObject.SetActive (true);

			_netAnimator.SetTrigger("Cast");
			if (caster is PlayerController)
				_animator.SetInteger("anim", (int)((caster as PlayerController).playerClassID) );

			caster.isCasting = true;
			caster.CmdApplyMoveStatus (castingMovement);

			castingBar.gameObject.SetActive (true);

			//TODO add animation
			//TODO add sound

			if(range > 0){
				target = t;
				targetRotation = r;
				placeholder = targetingSystem.getPlaceholder();
			}

			while(!castingBar.Complete){
				castingBar.Progress (Time.deltaTime);
				yield return 0;
			}

			ApplyEffect ();

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
			caster.CmdApplyMoveStatus (MoveStatus.Free);
		}
	}
}
