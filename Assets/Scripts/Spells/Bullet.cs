using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using RootMotion.FinalIK;

/// <summary>  
/// 	Basic bullet script destroying the gameobject on collision or 10s
/// </summary>
public class Bullet : NetworkBehaviour {

    public enum DamageTypeEnum
    {
        fire,
        lightning,
        ice,
        poison,
		physical
    };

    public BulletSpec spec;

    GameObject model;
    public GameObject Model { get { return model; } }

    [SerializeField]
    float velocity { get { return spec.Velocity; } }

    [SyncVar] Vector3 direction;
    public Vector3 Direction
    {
        get { return direction; }
        set { direction = value; }
    }

    public int Damage { get { return spec.Damage; } }

    float lifeTime { get { return spec.Damage; } }

    bool persistentBullet { get { return spec.PersistentBullet; } }

    bool destroyOnHit { get { return spec.DestroyOnHit; } }

    bool explodeOnHit { get { return spec.ExplodeOnHit; } }

    bool continuousDamage { get { return spec.ContinuousDamage; } }

    float damageTickInterval { get { return spec.DamageTickInterval; } }

    [SerializeField]
    BoxCollider boxCollider;

    bool followSpellOrigin { get { return spec.FollowSpellOrigin; } }

    GameObject spawnOnImpact { get { return spec.SpawnOnImpact; } }

    Weapon spellOrigin;
    public Weapon SpellOrigin { get { return spellOrigin; } set { spellOrigin = value; } }

    public string OwnerTag { get; set; }
    
    [SyncVar] public NetworkInstanceId spawnedBy;

    public DamageTypeEnum DamageType { get { return spec.DamageType; } }

    float lastDamageTick;
    bool loadSpecsNeeded = true;
    bool destroying = false;

    Rigidbody rigidbody;

    Living lastLiving;

    void Start() {
        lastDamageTick = Time.time;

        BoxCollider bc = GetComponent<BoxCollider>();
        bc.center = spec.ColliderCenter;
        bc.size = spec.ColliderSize;

        rigidbody = GetComponent<Rigidbody>();
	}

    public override void OnStartClient() {
        Physics.IgnoreCollision(GetComponent<Collider>(), ClientScene.FindLocalObject(spawnedBy).GetComponent<Collider>());
    }

    void Update()
    {
        if (loadSpecsNeeded && spec != null) {
            if (rigidbody != null) {
                rigidbody.velocity = Direction * velocity;
                rigidbody.useGravity = spec.UseGravity;
            } else {
                Debug.LogWarning("Bullet " + spec.name + " has no rigidbody on prefab " + spec.BulletPrefab.name);
            }

            model = Instantiate(spec.Model, transform);
            model.transform.localPosition = Vector3.zero;
            model.transform.localRotation = Quaternion.identity;

            if (!persistentBullet)
		        Destroy(gameObject, lifeTime);

            loadSpecsNeeded = false;
        }

        if (destroying)
            return;
        
        if (continuousDamage)
            DamageTick();
    }

    public void UpdatePosition()
    {
        if (followSpellOrigin)
            if (spellOrigin != null)
                transform.position = spellOrigin.SpellOrigin.position;
    }

    /// <summary>  
    /// 	Destroy on trigger
    /// </summary>
    private void OnTriggerEnter(Collider col)
    {
        Debug.Log(col.name);
        if (lastLiving != null) {
            Vector3 dir = transform.position - col.transform.position;
            dir.Normalize();
            lastLiving.HitReaction(col, dir * rigidbody.mass, col.ClosestPoint(transform.position));
        }	

        if (col.gameObject.tag == OwnerTag)
            return;

        if (col.isTrigger)
            return;

        if (continuousDamage)
            return;

        if (explodeOnHit)
            Explode();
        else
        {
            Living comp = col.gameObject.GetComponent<Living>();
            if (comp != null) {
                comp.TakeDamage(Damage, DamageType);

                lastLiving = comp;
            }

            if (destroyOnHit)
                Destroy(gameObject, 0.02f);
        }
    }

    private void Explode()
    {
        if (spawnOnImpact != null)
            Instantiate(spawnOnImpact, gameObject.transform.position, Quaternion.identity);

        var hits = Physics.OverlapSphere(transform.position, 3f);
        foreach (var hit in hits)
        {
            if (hit.gameObject.tag == OwnerTag)
                continue;

            var comp = hit.gameObject.GetComponent<Living>();
            if (comp != null)
                comp.TakeDamage(Damage, DamageType);
        }
        Destroy(gameObject);
    }

    private void DamageTick()
    {
        if (Time.time - lastDamageTick < damageTickInterval)
            return;

        if (boxCollider == null)
            return;

        var rot = Quaternion.identity;
        if(spellOrigin != null)
            rot = Quaternion.LookRotation(spellOrigin.SpellOrigin.forward, Vector3.up);

        if(followSpellOrigin)
            transform.rotation = rot;

        Vector3 half = new Vector3(boxCollider.size.x / 2, boxCollider.size.y / 2, boxCollider.size.z / 2);
        Collider[] colliders = Physics.OverlapBox(transform.position + transform.forward * (boxCollider.size.z / 2), half, rot);
        
        foreach (var col in colliders)
        {
            if (col.gameObject.tag == OwnerTag)
                continue;

            var comp = col.gameObject.GetComponent<Living>();
            if (comp != null)
                comp.TakeDamage(Damage, DamageType);
        }

        lastDamageTick = Time.time;
    }


    /// <summary>
    /// Disable particle emitter and damage
    /// </summary>
    public void DestroyPersistentBullet()
    {
        destroying = true;
        gameObject.GetComponentInChildren<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmitting);
        StartCoroutine(WaitForDestroy());
    }

    /// <summary>
    /// Wait for the particle to fade before destroying the gameobject
    /// </summary>
    IEnumerator WaitForDestroy()
    {
        yield return new WaitForSecondsRealtime(1f);
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {        
        /*
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position + transform.forward * (boxCollider.size.z/2), boxCollider.size);
        */
    }
}
