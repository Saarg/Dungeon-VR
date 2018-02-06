using UnityEngine;
using UnityEngine.Networking;

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

    [SerializeField]
    [SyncVar] float velocity;

    [SyncVar] Vector3 direction;
    public Vector3 Direction
    {
        get { return direction; }
        set { direction = value; }
    }

    [SerializeField]
    [SyncVar] int damage;
    public int Damage { get { return damage; } }

    [SerializeField]
    [SyncVar] float lifeTime;

    [SerializeField]
    bool destroyOnHit;

    [SerializeField]
    bool continuousDamage;

    [SerializeField]
    float damageTickInterval;

    [SerializeField]
    BoxCollider boxCollider;

    [SerializeField]
    bool followSpellOrigin;

    Weapon spellOrigin;
    public Weapon SpellOrigin { get { return spellOrigin; } set { spellOrigin = value; } }

    public string OwnerTag { get; set; }
    
    [SyncVar] public NetworkInstanceId spawnedBy;

    [SerializeField]
    DamageTypeEnum damageType;
    public DamageTypeEnum DamageType { get { return damageType; } }

    float lastDamageTick;

    /// <summary>  
    /// 	Destroy the object avec 10s
    /// </summary>
    void Start() {
        lastDamageTick = Time.time;
	}

    public override void OnStartClient() {
        Physics.IgnoreCollision(GetComponent<Collider>(), ClientScene.FindLocalObject(spawnedBy).GetComponent<Collider>());

        GetComponent<Rigidbody>().velocity = Direction * velocity;        

		Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (followSpellOrigin)
            if (spellOrigin != null)
                transform.position = spellOrigin.SpellOrigin.position;
        
        if (continuousDamage)
            DamageTick();
    }

    /// <summary>  
    /// 	Destroy on collision
    /// </summary>
    void OnCollisionEnter (Collision col) {
        if (col.gameObject.tag == OwnerTag)
            return;

        if (continuousDamage)
            return;

        var comp = col.gameObject.GetComponent<Living>();
        if (comp != null)
            comp.TakeDamage(Damage, DamageType);

        if(destroyOnHit)
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

    private void OnDrawGizmos()
    {        
        /*
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position + transform.forward * (boxCollider.size.z/2), boxCollider.size);
        */
    }
}
