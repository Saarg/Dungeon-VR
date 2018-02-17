using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

/// <summary>  
/// 	Player controller
/// </summary>
public class PlayerController : Living
{
    const float RAY_LENGTH = 20f;

    public Animator _animator;
    public NetworkAnimator _netAnimator;
    private Rigidbody rigidBody;

    public Transform cam;

    public enum PlayerClassEnum
    {
        Tank,        
        Healer,
        Assassin,
        Sorcerer,
    };

    public GameUI gameUI;
    Spell spell;

    [Header("Weapon")]
    public Transform weaponGrip;
    [SyncVar] public PlayerClassEnum playerClassID;
    public Weapon weapon;
    [SyncVar] public GameObject WeaponObject;
    float lastShotTime = 0;
    public Vector3 weaponDetectionRange;
    bool firing;
    Bullet persistentBullet;
    float lastManaDrain;

    [Header("Mana")]
    float manaFillRate = 0.2f;
    float lastManaFill = 0;

    [SerializeField]
    ProgressBar castingBar;

    public int MaxMana { get { return (int)maxMana; } }
    public int CurrentMana { get { return (int)curMana; } }

    [Header("NetworkData")]
    [SyncVar]
    public int playerId;

    GameObject target = null;
    float offsetValue;
    Vector3 offset;

    [SerializeField]
    GameObject[] classPrefab;

    /// <summary>  
    /// 	Fetch animator
    ///		Destroy camera if not localplayer
    /// </summary>
    void Start()
    {
        GameObject ui = GameObject.Find("GameUI");
        if (ui != null)
        { 
            gameUI = GameObject.Find("GameUI").GetComponent<GameUI>();
            gameUI.SetPlayerController(this);
            gameUI.enabled = true;
        }

        
        rigidBody = GetComponent<Rigidbody>();
        
        if (!isLocalPlayer)
        {
            Destroy(cam.gameObject);
        }
    }

    public override void OnStartLocalPlayer() {
        CmdApplyMoveStatus(MoveStatus.Free);
    }

    public override void OnStartClient() { 
        if (currentClassObject != null) {
            _animator = currentClassObject.GetComponent<Animator>();
            _netAnimator = currentClassObject.GetComponent<NetworkAnimator>();

            PlayerClassDesignation cd = currentClassObject.GetComponent<PlayerClassDesignation>();
        
            cd.transform.SetParent(transform);
            cd.transform.localPosition = Vector3.zero;
            cd.transform.localRotation = Quaternion.identity;

            weaponGrip = cd.weaponGrip;
            spell = cd.GetComponent<Spell>();
            spell.caster = this;
            spell.castingBar = castingBar;

            WeaponObject.transform.SetParent(weaponGrip);
            WeaponObject.transform.localPosition = Vector3.zero;
            WeaponObject.transform.localRotation = Quaternion.identity;
            weapon = WeaponObject.GetComponent<Weapon>();
        }
    }

    /// <summary>  
    /// 	Translate and rotate player
    ///		Updates animator
    /// </summary>
    public override void Update()
    {
		base.Update();
        if (isLocalPlayer)
        {
            if (Input.GetButtonDown("Fire2")) spell.CmdCast();

            UpdateJump();
            FillMana();
            CheckForWeapon();
            UpdateTarget();
            UpdateFire();

            Vector3 locVel = transform.InverseTransformDirection(rigidBody.velocity);

            if (_animator != null) {
                _animator.SetFloat("SpeedZ", locVel.z);
                _animator.SetFloat("SpeedX", locVel.x);
            }  
        } else if (rigidBody != null) {
            Destroy(rigidBody);
        }
    }

    void UpdateFire()
    {
        if (!firing)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                if (weapon != null && Time.time - lastShotTime > weapon.FiringInterval)
                {
                    Fire();
                    if (weapon.DrainMana)
                        firing = true;
                }
            }
        }
        if (Input.GetButtonUp("Fire1"))
        {
            StopContinuouFire();
        }

        if (firing)
        {
            if (Time.time - lastManaDrain > weapon.FiringInterval)
            {
                if (CurrentMana - weapon.ManaCost > 0)
                {
                    UpdateMana(CurrentMana - weapon.ManaCost);
                }
                else
                {
                    UpdateMana(0);
                    StopContinuouFire();
                }
                lastManaDrain = Time.time;
            }
        }
    }

    void StopContinuouFire()
    {
        if (weapon.DrainMana)
        {
            if (persistentBullet != null)
                persistentBullet.DestroyPersistentBullet();
            persistentBullet = null;
            firing = false;
        }
    }

    /// <summary> 
    /// 	Jump player if input
    /// </summary>
    void UpdateJump()
    {
        if (isLocalPlayer)
        {
            if (Input.GetButtonDown("Jump") && isGrounded && canJump)
            {
                _netAnimator.SetTrigger("Jump");
                if (isServer)
                    _animator.ResetTrigger("Jump");

                // If not moving jump is in place, just play the anim
                if (rigidBody.velocity.sqrMagnitude < 0.5) {
                    return;
                }
                
                if (lowJump)
                {
                    rigidBody.AddForce(Vector3.up * jumpHeight / jumpFactor, ForceMode.VelocityChange);
                }
                else
                {
                    rigidBody.AddForce(Vector3.up * jumpHeight, ForceMode.VelocityChange);
                }
            }
        }
    }

    void UpdateTarget()
    {
        var hits = Physics.RaycastAll(cam.gameObject.transform.position, cam.gameObject.transform.forward, RAY_LENGTH);
        foreach (var hit in hits)
        {
            if (hit.collider.gameObject.tag == "Player")
                continue;

            Living living = hit.transform.gameObject.GetComponent<Living>();
            if (living != null)
            {
                target = hit.transform.gameObject;
                return;
            }
        }

        target = null;
    }

    /// <summary>  
    ///     Camera follow player and player orientation depend of the camera
    /// 	Translate and rotate player
    ///		Updates animator
    ///		Take move status into consideration
    /// </summary>
    void FixedUpdate()
    {
        if (isLocalPlayer) {
            Vector3 dir = (cam.right * Input.GetAxis("Horizontal") * Time.deltaTime) + (cam.forward * Input.GetAxis("Vertical") * Time.deltaTime);
            dir.y = 0;
            dir.Normalize();

            float angle = Vector3.Angle(cam.forward, dir);
            Vector3 lookDir = dir;
            lookDir.y = 0;

            if(lookDir.sqrMagnitude > 0.1f) {
                if (angle > 90)
                    lookDir = -lookDir;
                
            } else {
               lookDir = cam.forward;
               lookDir.y = 0;
            }

            lookDir.Normalize();            
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), turnSpeed);

            if (canMove)
            {
                if ((Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0))
                {
                    float s = speed * (1 - angle/360);
                    if (canRun && rigidBody.velocity.magnitude < s)
                    {
                        rigidBody.AddForce(dir, ForceMode.VelocityChange);                 
                    }
                    else if (rigidBody.velocity.magnitude < s / runFactor)
                    {
                        rigidBody.AddForce(dir, ForceMode.VelocityChange);                                         
                    }
                }
            }
            
            rigidBody.velocity = new Vector3(rigidBody.velocity.x * 0.9f, rigidBody.velocity.y, rigidBody.velocity.z * 0.9f);
        }  
    }

    public bool HasTarget()
    {
        return target != null;
    }

    public int TargetCurLife()
    {
        if(target == null)
            return 0;

        return (int)target.GetComponent<Living>().GetCurLife();
    }

    public int TargetMaxLife()
    {
        if (target == null)
            return 0;

        return (int)target.GetComponent<Living>().GetMaxLife();
    }

    void CheckForWeapon()
    {
        if (gameUI == null)
            return;

        var colliders = Physics.OverlapBox(transform.position, weaponDetectionRange);
        GameObject closestObj = null;
        float closestDistance = float.MaxValue;
        foreach (var collider in colliders)
        {
            var action = collider.GetComponent<DroppedWeapon>();
            if (action != null)
            {
                if ((collider.transform.position - transform.position).sqrMagnitude < closestDistance)
                {
                    closestDistance = (collider.transform.position - transform.position).sqrMagnitude;
                    closestObj = collider.gameObject;
                }
            }
        }

        if (closestObj == null)
        {
            gameUI.HideWeaponStats();
        }
        else
        {
            Weapon closestWeapon = closestObj.GetComponent<Weapon>();
            gameUI.ShowWeaponStats(closestWeapon);
            if (Input.GetButtonDown("PickUpWeapon"))
            {
                if (closestWeapon.CanEquip(playerClassID))
                {
                    StopContinuouFire();
                    CmdPickupWeapon(closestObj.GetComponent<NetworkIdentity>().netId);
                }
            }
        }
    }

    void FillMana()
    {
        if (Time.time - lastManaFill > manaFillRate)
        {
            UpdateMana(CurrentMana + 1);
            lastManaFill = Time.time;
        }
    }

    /// <summary>  
    /// Client side compute fire data
    /// </summary>
    void Fire() {
        if (weapon.UseMana && curMana < weapon.ManaCost)
            return;
        else if (weapon.UseMana)
            UpdateMana(CurrentMana - weapon.ManaCost);

        var hits = Physics.RaycastAll(cam.gameObject.transform.position, cam.gameObject.transform.forward, RAY_LENGTH);
        var endPoint = cam.gameObject.transform.position + cam.gameObject.transform.forward * RAY_LENGTH;
        var offsetPts = endPoint;
        Vector3 direction = Vector3.zero;
         
        foreach (var hit in hits)
        {
            if (hit.collider.tag == "Player")
                continue;

            if (hit.transform.gameObject.layer == 9)
                continue;

            offsetPts = hit.point;
            direction = (hit.point - weapon.SpellOrigin.position).normalized;
            break;
        }

        if (direction == Vector3.zero)
            direction = (endPoint - weapon.SpellOrigin.position).normalized;
        offsetValue = Mathf.Lerp(0, RAY_LENGTH, (offsetPts - weapon.SpellOrigin.position).magnitude / RAY_LENGTH);

        var rot = Quaternion.LookRotation(direction, Vector3.up);
 
        if (weapon.SpreadBullet)
        {
            for (int i = 0; i < weapon.NumberOfBullet; i++)
            {
                Vector3 end = new Vector3(endPoint.x + UnityEngine.Random.Range(-weapon.SpreadAngle, weapon.SpreadAngle), endPoint.y + UnityEngine.Random.Range(-weapon.SpreadAngle, weapon.SpreadAngle), endPoint.z + UnityEngine.Random.Range(-weapon.SpreadAngle, weapon.SpreadAngle));
                Vector3 spreadDirection = (end - weapon.SpellOrigin.position).normalized;
                var spreadRot = Quaternion.LookRotation(spreadDirection, Vector3.up);
                CmdFire(spreadDirection, spreadRot);
            }
        }
        else
            CmdFire(direction, rot);

        lastShotTime = Time.time;
    }

    // Commands and RPC //

    /// <summary>
    /// Ask the server to pickup the weapon
    /// </summary>    
    [Command]
    void CmdPickupWeapon(NetworkInstanceId weaponNetId) {
        RpcPickupWeapon(weaponNetId);
    }

    /// <summary>
    /// Pickup weapon on ALL CLIENTS!
    /// </summary> 
    [ClientRpc]
    void RpcPickupWeapon(NetworkInstanceId weaponNetId) {
        GameObject closestObj = ClientScene.FindLocalObject(weaponNetId);

        if (closestObj == null) {
            Debug.LogError("Weapon: " + weaponNetId + " not found!");
            return;
        }

        if (WeaponObject != null) {
            GameObject dropWeapon = WeaponObject;
            dropWeapon.transform.SetParent(null);
            dropWeapon.transform.localPosition = new Vector3(dropWeapon.transform.localPosition.x,0,dropWeapon.transform.localPosition.z);
            dropWeapon.transform.rotation = Quaternion.identity;
            dropWeapon.AddComponent<DroppedWeapon>();
        }

        Destroy(closestObj.GetComponent<DroppedWeapon>());        
        WeaponObject = closestObj;
        WeaponObject.transform.SetParent(weaponGrip);
        WeaponObject.transform.localPosition = Vector3.zero;
        WeaponObject.transform.localRotation = Quaternion.identity;
        weapon = WeaponObject.GetComponent<Weapon>();
    }

    /// <summary>  
    /// 	Instanciate and spawn bullet
    /// </summary>
    [Command]
    void CmdFire(Vector3 direction, Quaternion rot)
    {
        GameObject bulletObj = Instantiate(weapon.Bullet, weapon.SpellOrigin.position, rot);
        Bullet bullet = bulletObj.GetComponent<Bullet>();

        Physics.IgnoreCollision(bulletObj.GetComponent<Collider>(), GetComponentInParent<Collider>(), true);
        bullet.OwnerTag = gameObject.tag;
        bullet.spawnedBy = netId;

        offset = Vector3.zero;
        if (weapon.shootingOffset)
            offset = new Vector3(0, offsetValue / RAY_LENGTH, 0);

        bullet.Direction = direction + offset;
        bullet.GetComponent<Bullet>().SpellOrigin = weapon;

        if (weapon.DrainMana)
            persistentBullet = bullet;

        NetworkServer.Spawn(bulletObj);    
    }

    [Command]
    public void CmdUpdatePlayerId(int id) {
        RpcUpdatePlayerId(id);
    }

    [ClientRpc]
    public void RpcUpdatePlayerId(int id) {
        playerId = id;
    }

    [SyncVar] GameObject currentClassObject;
    [Command]
    public void CmdUpdatePlayerClass(int id) {
        if (currentClassObject != null) {
            NetworkServer.Destroy(currentClassObject);
        }

        GameObject go = Instantiate(classPrefab[(int)id], transform);

        _animator = go.GetComponent<Animator>();
        _netAnimator = go.GetComponent<NetworkAnimator>();

        PlayerClassDesignation cd = go.GetComponent<PlayerClassDesignation>();

        maxLife = cd.maxLife;
        maxMana = cd.maxMana;

        speed = cd.speed;
        jumpHeight = cd.jumpHeight;

        fire = cd.fire;
        ice = cd.ice;
        lightning = cd.lightning;
        poison = cd.poison;
        physical = cd.physical;

        weaponGrip = cd.weaponGrip;
        spell = cd.GetComponent<Spell>();
        spell.caster = this;
        spell.castingBar = castingBar;

        NetworkServer.SpawnWithClientAuthority(go, gameObject);
        currentClassObject = go;

        GameObject w = Instantiate(cd.defaultWeapon, weaponGrip);
        Destroy(w.GetComponent<DroppedWeapon>());

        NetworkServer.SpawnWithClientAuthority(w, gameObject);

        RpcClassUpdated(cd.netId, w.GetComponent<NetworkIdentity>().netId);
    }

    [ClientRpc]
    private void RpcClassUpdated(NetworkInstanceId classModelId, NetworkInstanceId weaponNetId){
        PlayerClassDesignation cd = ClientScene.FindLocalObject(classModelId).GetComponent<PlayerClassDesignation>();
        
        cd.transform.SetParent(transform);
        cd.transform.localPosition = Vector3.zero;
        cd.transform.localRotation = Quaternion.identity;

        _animator = cd.GetComponent<Animator>();
        _netAnimator = cd.GetComponent<NetworkAnimator>();

        weaponGrip = cd.weaponGrip;
        spell = cd.GetComponent<Spell>();
        spell.caster = this;
        spell.castingBar = castingBar;

        if (weaponNetId != null && isLocalPlayer)
            CmdPickupWeapon(weaponNetId);
    }
}
