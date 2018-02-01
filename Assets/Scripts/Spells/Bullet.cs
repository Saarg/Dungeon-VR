using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>  
/// 	Basic bullet script destroying the gameobject on collision or 10s
/// </summary>
public class Bullet : MonoBehaviour {

    public enum DamageTypeEnum
    {
        fire,
        lightning,
        ice,
        poison,
    };

    [SerializeField]
    float velocity;

    public Vector3 Direction
    {
        get; set;
    }

    [SerializeField]
    int damage;
    public int Damage{ get { return damage; } }

    [SerializeField]
    float lifeTime;
    
    public string OwnerTag { get; set; }

    [SerializeField]
    DamageTypeEnum damageType;
    public DamageTypeEnum DamageType { get { return damageType; } }

    /// <summary>  
    /// 	Destroy the object avec 10s
    /// </summary>
    void Start() {
		Destroy(gameObject, lifeTime);
	}

    void Update()
    {
        GetComponent<Rigidbody>().velocity = Direction * velocity;
    }

    /// <summary>  
    /// 	Destroy on collision
    /// </summary>
    void OnCollisionEnter (Collision col) {
        if (col.gameObject.tag == OwnerTag)
            return;

        var comp = col.gameObject.GetComponent<Living>();
        if (comp != null)
            comp.TakeDamage(Damage, DamageType);

		Destroy(gameObject);
	}
}
