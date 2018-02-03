using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>  
/// 	Enum used to know the movement capacity of the entity
/// </summary>
public enum MoveStatus {Free, Slow, Heavy, None};  

/// <summary>  
/// 	Mother class of every living entity
/// </summary>
public class Living : NetworkBehaviour {

	[Header("Life")]
	public float maxLife = 100;

	[SyncVar(hook="UpdateLife")]
	public float curLife = 100f;

	[Header("Movement")]	
	public float speed = 1f;
	public bool canJump = true;
	public MoveStatus moveStatus;

	[Header("Weakness/Strength")]
	[Range(0f, 2f)]
	public float fire = 1f;
	[Range(0f, 2f)]	
	public float ice = 1f;
	[Range(0f, 2f)]	
	public float lightning = 1f;
	[Range(0f, 2f)]	
	public float poison = 1f;

	// Events
	public delegate void DeathEvent();
    public event DeathEvent OnDeath;

	/// <summary>  
	/// 	curLife hook, sends death event if life goes to 0 and clamps the life between 0 and maxlife
	/// </summary>
	/// <remarks>
	/// 	Use this function to update entity's life
	/// </remarks>
	void UpdateLife(float life) {
		curLife = Mathf.Clamp(life, 0, maxLife);

		if (curLife == 0f) {
            if(OnDeath != null)
			    OnDeath();
            Destroy(gameObject);
		}
	}

    public void TakeDamage(int damage, Bullet.DamageTypeEnum damageType)
    {
        UpdateLife(curLife - CalculateResisance(damage, damageType));
    }

    int CalculateResisance(int damage, Bullet.DamageTypeEnum damageType)
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
}
