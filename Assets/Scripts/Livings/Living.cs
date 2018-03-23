﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using RootMotion.FinalIK;

/// <summary>  
/// 	Enum used to know the movement capacity of the entity
/// </summary>
public enum MoveStatus {Free, Ralenti, Casting, Immobilisé };  

/// <summary>  
/// 	Mother class of every living entity
/// </summary>
public class Living : NetworkBehaviour {

	[Header("Life")]
	[SyncVar] public float maxLife = 100;

    [SyncVar(hook="UpdateLife")]
	public float curLife = 100f;

    [Header("Mana")]
    [SyncVar] public float maxMana = 100;

    [SyncVar(hook = "UpdateMana")]
    public float curMana = 100f;

    [Header("Movement")]	
	[SyncVar] public float speed = 3f;
	[SyncVar] public float walkSpeed = 6f;
	[SyncVar] public float sprintSpeed = 8f;
    [Header("Movement")]
    [SerializeField]
    [SyncVar] protected float turnSpeed = 50;     

    [SerializeField]
    [Range(1.0f, 3.0f)]
    [SyncVar] protected float runFactor = 2;

    [SyncVar] public float jumpHeight = 1.0f;
    [SyncVar] public float jumpFactor = 2.0f;

    public Vector3 drag;
    
    float lastGroundedCheck;
    public bool isGrounded;

    [Header("Etat movement")]
    public MoveStatus moveStatus;
    public bool canRun = true;
    public bool canJump = true;
    public bool canMove = false;
    public bool lowJump = false;

	[Header ("Spells")]
	public bool isCasting = false;
	public bool isTargeting = false;

    [Header("Weakness/Strength")]
	[Range(0f, 2f)]
	public float fire = 1f;
	[Range(0f, 2f)]	
	public float ice = 1f;
	[Range(0f, 2f)]	
	public float lightning = 1f;
	[Range(0f, 2f)]	
	public float poison = 1f;
	[Range(0f, 2f)]	
	public float physical = 1f;

    [SerializeField]
    protected HitReaction _hitReaction;
    [SerializeField]    
    protected ParticleSystem _bloodPrefab;
    public void HitReaction(Collider collider, Vector3 force, Vector3 point) { 
        if(_hitReaction != null) { 
            _hitReaction.Hit(collider, force, point);
        }
        if (_bloodPrefab != null) {
            Destroy(Instantiate(_bloodPrefab.gameObject, point, Quaternion.LookRotation(force)), _bloodPrefab.main.duration);
        }
    }
    Collider _collider;

    [SerializeField]
    public List<Collider> collidersList = new List<Collider>();

    public bool dead = false;
	// Events
	public delegate void DeathEvent();
    public event DeathEvent OnDeath;
    public delegate void DamageEvent(int damage, Bullet.DamageTypeEnum damageType);
    public event DamageEvent OnDamage;
    

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    public virtual void Update()
    {
        if (isLocalPlayer) {
            CheckIfGrounded();
        }
    }

    [Command]
    void CmdUpdateLife(float life) {
        UpdateLife(life);
    }

	/// <summary>  
	/// 	curLife hook, sends death event if life goes to 0 and clamps the life between 0 and maxlife
	/// </summary>
	/// <remarks>
	/// 	Use this function to update entity's life
	/// </remarks>
	void UpdateLife(float life) {
		curLife = Mathf.Clamp(life, 0, maxLife);

		if (curLife == 0f) {
            dead = true;

            if (OnDeath != null)
			    OnDeath.Invoke();
		}
	}

    bool CheckIfGrounded() {
        if (Time.realtimeSinceStartup - lastGroundedCheck < 0.05f)
            return isGrounded;

        if (_collider == null) {
            _collider = GetComponent<Collider>();
        }

        if (_collider is CapsuleCollider) {
            isGrounded = Physics.Raycast(transform.position + (_collider as CapsuleCollider).center, -transform.up, (_collider as CapsuleCollider).height/1.8f);
        }
        else if (_collider is BoxCollider)
            isGrounded = Physics.Raycast(transform.position + (_collider as BoxCollider).center, -transform.up, (_collider as BoxCollider).size.y/1.8f);
        else if (_collider is SphereCollider)
            isGrounded = Physics.Raycast(transform.position + (_collider as SphereCollider).center, -transform.up, (_collider as SphereCollider).radius/1.8f);

        return isGrounded;
    }

    [Command]
    public void CmdUpdateMana(float mana) {
        UpdateMana(mana);
    }
    /// <summary>  
	/// 	curMana hook, clamps the mana between 0 and maxMana
	/// </summary>
	/// <remarks>
	/// 	Use this function to update entity's mana
	/// </remarks>
    public void UpdateMana(float mana)
    {
        curMana = Mathf.Clamp(mana, 0, maxMana);     
    }

    public void ApplyMoveStatus(MoveStatus status)
    {
        if (isServer || isLocalPlayer)
		    CmdApplyMoveStatus(status);
	}

	/// <summary>  
    /// 	Command to update move status
    /// </summary>
	[Command]
    public void CmdApplyMoveStatus(MoveStatus status)
    {
		RpcApplyMoveStatus(status);
	}

    /// <summary>  
    /// 	Rpc in charge of updating move status
    /// </summary>
	[ClientRpc]
	private void RpcApplyMoveStatus(MoveStatus status)
    {
        moveStatus = status;
        switch (moveStatus)
        {
            case MoveStatus.Free:
                canRun = true;
                canJump = true;
                canMove = true;
                lowJump = false;
                break;
            case MoveStatus.Ralenti:
                canRun = false;
                canJump = true;
                canMove = true;
                lowJump = false;
                break;
            case MoveStatus.Casting:
                canRun = true;
                canJump = false;
                canMove = true;
                lowJump = true;
                break;
            case MoveStatus.Immobilisé:
                canRun = false;
                canJump = false;
                canMove = false;
                lowJump = false;
                break;
            default:
                break;
        }
    }

    public virtual void TakeDamage(int damage, Bullet.DamageTypeEnum damageType)
    {
        CmdUpdateLife(curLife - CalculateResistance(damage, damageType));

        if (OnDamage != null)
            OnDamage.Invoke(damage, damageType);
    }

    public void Heal(int heal)
    {
        UpdateLife(curLife + heal);
    }

    int CalculateResistance(int damage, Bullet.DamageTypeEnum damageType)
    {
        switch (damageType)
        {
            case Bullet.DamageTypeEnum.fire:
                return Mathf.FloorToInt((float)damage * fire);
            case Bullet.DamageTypeEnum.ice:
                return Mathf.FloorToInt((float)damage * ice);
            case Bullet.DamageTypeEnum.lightning:
                return Mathf.FloorToInt((float)damage * lightning);
            case Bullet.DamageTypeEnum.poison:
                return Mathf.FloorToInt((float)damage * poison);
			case Bullet.DamageTypeEnum.physical:
				return Mathf.FloorToInt((float)damage * physical);
            default:
                return damage;
        }
    }

    public float GetCurLife()
    {
        return curLife;
    }

    public float GetMaxLife()
    {
        return maxLife;
    }

    public virtual void Death()
    {
        //Play death animation and whatever is needed in child
        Destroy(gameObject);
    }
}
