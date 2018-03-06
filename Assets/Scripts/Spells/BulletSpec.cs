using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "BulletSpec", menuName = "Weapons/BulletSpec", order = 1)]
public class BulletSpec : ScriptableObject {

    [SerializeField]
    float velocity;
    public float Velocity { get { return velocity; } }

    [SerializeField]
    int damage;
    public int Damage { get { return damage; } }

    [SerializeField]
    float lifeTime;
    public float LifeTime { get { return lifeTime; } }

    [SerializeField]
    bool persistentBullet;
    public bool PersistentBullet { get { return persistentBullet; } }    

    [SerializeField]
    bool destroyOnHit;
    public bool DestroyOnHit { get { return destroyOnHit; } }        

    [SerializeField]
    bool explodeOnHit;
    public bool ExplodeOnHit { get { return explodeOnHit; } }   

    [SerializeField]
    bool continuousDamage;
    public bool ContinuousDamage { get { return continuousDamage; } }          

    [SerializeField]
    float damageTickInterval;
    public float DamageTickInterval { get { return damageTickInterval; } }    

    [SerializeField]
    GameObject spawnOnImpact;
    public GameObject SpawnOnImpact { get { return spawnOnImpact; } }

    [SerializeField]
    Bullet.DamageTypeEnum damageType;
    public Bullet.DamageTypeEnum DamageType { get { return damageType; } }
}