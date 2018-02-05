using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Animator))]

/// <summary>  
/// 	Player controller
/// </summary>
public class PlayerController : Living
{
    private Animator _animator;
    private NetworkAnimator _netAnimator;
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

    [Header("Weapon")]
    public GameObject defaultWeapon;
    public Transform weaponGrip;
    public PlayerClassEnum playerClassID;
    public Weapon weapon;
    [SyncVar] public GameObject WeaponObject;
    float lastShotTime = 0;
    public Vector3 weaponDetectionRange;    

    [Header("Mana")]
    float manaFillRate = 0.2f;
    float lastManaFill = 0;

    public int MaxMana { get { return (int)maxMana; } }
    public int CurrentMana { get { return (int)curMana; } }

    [Header("Movement")]   
    private float turnSpeed = 50;     

    [SerializeField]
    [Range(1.0f, 3.0f)]
    private float RunFactor = 2;
    
    [Header("Jump")]
    [SerializeField]
    [Range(1.0f, 3.0f)] 
    private float JumpFactor = 2;
    public bool isGrounded;

    [Header("NetworkData")]
    [SyncVar]
    public int playerId;
    [SyncVar(hook="UpdateClass")]    
    public int playerClass;

    GameObject target = null;

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

        _animator = GetComponent<Animator>();
        _netAnimator = GetComponent<NetworkAnimator>();
        rigidBody = GetComponent<Rigidbody>();
       
        if (!isLocalPlayer)
        {
            Destroy(cam.gameObject);
        } else {
            CmdApplyMoveStatus(MoveStatus.Free); 

            Lobby.LobbyManager.curGamePlayer = gameObject;
        }
    }

    public override void OnStartLocalPlayer() {
        playerClassID = (PlayerClassEnum)(Mathf.Clamp(playerClass-1, 0, 3));
    }

    public override void OnStartServer() {
        // if no weapon spawn default 
        if (WeaponObject == null) {
            GameObject w = Instantiate(defaultWeapon, weaponGrip);
            Destroy(w.GetComponent<DroppedWeapon>());

            NetworkServer.Spawn(w);

            WeaponObject = w;
        }
    }

    /// <summary>  
    /// 	Translate and rotate player
    ///		Updates animator
    /// </summary>
    void Update()
    {
        if (isServer && WeaponObject != null && weapon == null) {
            RpcPickupWeapon(WeaponObject.GetComponent<NetworkIdentity>().netId);            
        }

        if (isLocalPlayer)
        {
            UpdateJump();

            FillMana();
            CheckForWeapon();
            UpdateTarget();

            if (Input.GetButtonDown("Fire1"))
            {
                if (weapon != null && Time.time - lastShotTime > weapon.FiringInterval)
                    Fire();
            }
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
                if (lowJump)
                {
                    rigidBody.AddForce(Vector3.up * JumpSpeed / JumpFactor, ForceMode.VelocityChange);
                }
                else
                {
                    rigidBody.AddForce(Vector3.up * JumpSpeed, ForceMode.VelocityChange);
                }
            }
        }
    }

    void UpdateTarget()
    {
        var hits = Physics.RaycastAll(cam.gameObject.transform.position, cam.gameObject.transform.forward, 20f);
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

            Vector3 lookDir = cam.forward;
            lookDir.y = 0;
            lookDir.Normalize();

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), turnSpeed);            

            if (canMove)
            {
                if ((Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0))
                {
                    if (canRun && rigidBody.velocity.magnitude < speed)
                    {
                        rigidBody.AddForce(dir, ForceMode.VelocityChange);                 
                    }
                    else if (rigidBody.velocity.magnitude < speed / RunFactor)
                    {
                        rigidBody.AddForce(dir, ForceMode.VelocityChange);                                         
                    }
                }
            }
            
            rigidBody.velocity = new Vector3(rigidBody.velocity.x * 0.9f, rigidBody.velocity.y, rigidBody.velocity.z * 0.9f);
        }

        Vector3 locVel = transform.InverseTransformDirection(rigidBody.velocity);

        _animator.SetFloat("SpeedZ", locVel.z);
        _animator.SetFloat("SpeedX", locVel.x);        
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

        var hits = Physics.RaycastAll(cam.gameObject.transform.position, cam.gameObject.transform.forward, 20f);
        var endPoint = cam.gameObject.transform.position + cam.gameObject.transform.forward * 20f;
        Vector3 direction = Vector3.zero;

        foreach (var hit in hits)
        {
            if (hit.collider.tag == "Player")
                continue;

            direction = (hit.point - weapon.SpellOrigin.position).normalized;
            break;
        }

        if (direction == Vector3.zero)
            direction = (endPoint - weapon.SpellOrigin.position).normalized;
       
        var rot = Quaternion.LookRotation(direction, Vector3.up);

        CmdFire(direction, rot);
        lastShotTime = Time.time;
    }

    /// <summary>  
    /// 	Use to allow player another jump after hitting the ground
    /// </summary>
    void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.tag.Equals("Ground"))
        {
            isGrounded = true;
            _animator.ResetTrigger("Jump");
        }
    }

    /// <summary>  
    /// 	Use to prevent player another jump in air
    /// </summary>
    void OnCollisionExit(Collision coll)
    {
        if (coll.collider.tag.Equals("Ground"))
        {
            isGrounded = false;        
        }
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
        GameObject bullet = Instantiate(weapon.Bullet, weapon.SpellOrigin.position, rot);

        Physics.IgnoreCollision(bullet.GetComponent<Collider>(), GetComponentInParent<Collider>(), true);

        bullet.GetComponent<Bullet>().OwnerTag = gameObject.tag;
        bullet.GetComponent<Bullet>().spawnedBy = netId;
        bullet.GetComponent<Bullet>().Direction = direction;
        bullet.GetComponent<Bullet>().SpellOrigin = weapon;

        NetworkServer.Spawn(bullet);    
    }

    [Command]
    public void CmdUpdatePlayerId(int id) {
        RpcUpdatePlayerId(id);
    }

    [ClientRpc]
    public void RpcUpdatePlayerId(int id) {
        playerId = id;
    }

    [Command]
    public void CmdUpdatePlayerClass(int id) {
        RpcUpdatePlayerClass(id);
    }

    [ClientRpc]
    public void RpcUpdatePlayerClass(int id) {
        playerClass = id;
    }

    // SyncVar hooks //

    void UpdateClass(int c) {
        playerClass = c;
        playerClassID = (PlayerClassEnum)(Mathf.Clamp(playerClass-1, 0, 3));
    }
}
