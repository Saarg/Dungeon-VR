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
    const float RAY_LENGTH = 50f;

    public Animator _animator;
    public NetworkAnimator _netAnimator;
    private Rigidbody rigidBody;
    private CapsuleCollider collider;

    public Transform cam;
    public InventoryController inventory;

    public enum PlayerClassEnum
    {
        Tank,
        Healer,
        Assassin,
        Sorcerer,
    };

    public GameUI gameUI;
    Spell spell;

    [SyncVar] public PlayerClassEnum playerClassID;

    [Header("Mana")]
    float manaFillRate = 0.2f;
    float lastManaFill = 0;

    public int MaxMana { get { return (int)maxMana; } }
    public int CurrentMana { get { return (int)curMana; } }

    [Header("NetworkData")]
    [SyncVar]
    public int playerId;

    GameObject target = null;

    [SerializeField]
    GameObject[] classPrefab;

    [SyncVar] GameObject currentClassObject;
    [SyncVar] int defaultWeaponId;

    [SyncVar] string playerName;

    Transform lookAt;

    /// <summary>
    /// 	Fetch animator
    ///		Destroy camera if not localplayer
    /// </summary>
    void Start()
    {
        if (isLocalPlayer)
        {
            GameObject ui = GameObject.Find("GameUI");
            if (ui != null)
            {
                gameUI = GameObject.Find("GameUI").GetComponent<GameUI>();
                gameUI.SetPlayerController(this);
                gameUI.enabled = true;
            }
        } else {
            Destroy(cam.gameObject);

            GameObject ui = GameObject.Find("GameUI");
            if (ui != null)
            {
                gameUI = GameObject.Find("GameUI").GetComponent<GameUI>();
                gameUI.AddTeamMate(this);
                gameUI.enabled = true;
            }
        }

        rigidBody = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();

        OnDeath += Death;

        name = playerName;
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
            inventory.weaponGrip = cd.weaponGrip;

            if (isLocalPlayer)
            {
                inventory.InitializeWeaponInformation(new NetworkInstanceId((uint)defaultWeaponId), cd);
                inventory.CmdPickupWeapon(new NetworkInstanceId((uint)defaultWeaponId), netId);
            } else {
                inventory.InitializedOtherClient();

                Weapon w =  inventory.GetWeapon(inventory.CurrentWeapon);
                if (w.spec == null)
                    w.spec = cd.defaultWeapon;
            }

            spell = cd.GetComponent<Spell>();
            spell.caster = this;
            if (isLocalPlayer)            
                spell.castingBar = gameUI.CastingBar;   

            lookAt = cd.transform.Find("LookAt");
        }
    }

    /// <summary>
    /// 	Translate and rotate player
    ///		Updates animator
    /// </summary>
    public override void Update()
    {
		base.Update();
        if (isLocalPlayer && !dead)
        {
            if (Input.GetButtonDown("Fire2"))
            {
                spell.Cast();
            }

            UpdateJump();
            FillMana();
            UpdateTarget();
        }
    }

    /// <summary>
    /// 	Jump player if input
    /// </summary>
    void UpdateJump()
    {
        if (isLocalPlayer && !dead)
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
        if (isLocalPlayer && !dead)
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
    }

    /// <summary>
    ///     Camera follow player and player orientation depend of the camera
    /// 	Translate and rotate player
    ///		Updates animator
    ///		Take move status into consideration
    /// </summary>
    void FixedUpdate()
    {
        if (isLocalPlayer && !dead) {
            float angle = Vector3.Angle(cam.forward, transform.forward);

            if (lookAt != null) {
                if (angle < 170) {
                    lookAt.localPosition = Vector3.Lerp(lookAt.localPosition, Vector3.up * 1.6f + lookAt.InverseTransformDirection(cam.forward * 5), Time.deltaTime * 2);
                }  else {
                    lookAt.localPosition = Vector3.Lerp(lookAt.localPosition, Vector3.up * 1.6f + lookAt.InverseTransformDirection(transform.forward * 5), Time.deltaTime * 2);                
                }

                Debug.DrawLine(transform.position + Vector3.up * 1.6f, lookAt.position, Color.red);
            }

            if (canMove)
            {
                if ((Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0))
                {
                    Vector3 dir = (cam.right * Input.GetAxis("Horizontal") * Time.deltaTime) + (cam.forward * Input.GetAxis("Vertical") * Time.deltaTime);
                    dir.y = 0;
                    dir.Normalize();

                    float a = Vector3.Angle(dir, transform.forward);

                    Vector3 lookDir = cam.forward;
                    lookDir.y = 0;
                    lookDir.Normalize();

                    if (isGrounded)
                        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), turnSpeed);
                    else if (a < 120)
                        transform.rotation = Quaternion.LookRotation(dir);
                    else 
                        transform.rotation = Quaternion.LookRotation(-dir);                        
                    
                    float s = speed * (1 - a/360);
                    // _amplifier.bodies[0].horizontalWeight = 1;
                    if (Input.GetButton("Walk") || !canRun)
                        s = walkSpeed * (1 - a/360);
                    else if (Input.GetButton("Sprint"))
                        s = sprintSpeed * (1 - a/360);
                    // _amplifier.bodies[0].horizontalWeight = 5;

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

            Vector3 locVel = transform.InverseTransformDirection(rigidBody.velocity);

            if (_animator != null) {
                _animator.SetFloat("SpeedZ", locVel.z);
                _animator.SetFloat("SpeedX", locVel.x);

                _animator.SetFloat("Speed", locVel.sqrMagnitude);
            }

            rigidBody.AddForce(-Vector3.Scale(rigidBody.velocity, drag), ForceMode.VelocityChange);
        }

        rigidBody.angularVelocity = Vector3.zero;
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

    void FillMana()
    {
        if (Time.time - lastManaFill > manaFillRate)
        {
            CmdUpdateMana(CurrentMana + 1);
            lastManaFill = Time.time;
        }
    }

    public Weapon GetWeapon(Weapon.WeaponTypeEnum weaponType)
    {
        return inventory.GetWeapon(weaponType);
    }

    // Commands and RPC //
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
        walkSpeed = cd.walkSpeed;
        sprintSpeed = cd.sprintSpeed;
        jumpHeight = cd.jumpHeight;

        fire = cd.fire;
        ice = cd.ice;
        lightning = cd.lightning;
        poison = cd.poison;
        physical = cd.physical;

        NetworkServer.SpawnWithClientAuthority(go, gameObject);
        currentClassObject = go;

        GameObject w = Instantiate(cd.defaultWeapon.WeaponPrefab, cd.weaponGrip);
        Destroy(w.GetComponent<DroppedWeapon>());
        w.GetComponent<Weapon>().spec = cd.defaultWeapon;
        defaultWeaponId = (int)w.GetComponent<NetworkIdentity>().netId.Value;

        NetworkServer.SpawnWithClientAuthority(w, gameObject);
        RpcClassUpdated(cd.netId, w.GetComponent<NetworkIdentity>().netId);

        playerClassID = (PlayerClassEnum)id;
    }

    [ClientRpc]
    private void RpcClassUpdated(NetworkInstanceId classModelId, NetworkInstanceId weaponNetId){
        PlayerClassDesignation cd = ClientScene.FindLocalObject(classModelId).GetComponent<PlayerClassDesignation>();
        GameObject weaponObj = ClientScene.FindLocalObject(weaponNetId);
        weaponObj.GetComponent<Weapon>().spec = cd.defaultWeapon;        
        defaultWeaponId = (int)weaponObj.GetComponent<NetworkIdentity>().netId.Value;
        cd.transform.SetParent(transform);
        cd.transform.localPosition = Vector3.zero;
        cd.transform.localRotation = Quaternion.identity;

        collider.center = cd.center;
        collider.radius = cd.radius;
        collider.height = cd.height;

        _animator = cd.GetComponent<Animator>();
        _netAnimator = cd.GetComponent<NetworkAnimator>();

        inventory.weaponGrip = cd.weaponGrip;
        inventory.InitializeWeaponInformation(weaponNetId, cd);

        spell = cd.GetComponent<Spell>();
        spell.caster = this;
        if (isLocalPlayer)        
            spell.castingBar = gameUI.CastingBar;

        lookAt = cd.transform.Find("LookAt");
    }

    [Command]
    public void CmdSetName(String n) {
        playerName = n;
        gameObject.name = n;
        RpcSetName(n);
    }

    [ClientRpc]
    void RpcSetName(String n) {
        gameObject.name = n;
    }

    public override void Death()
    {
        if (isLocalPlayer) {
            gameUI.SetDeathUI(true);
            Debug.Log("AHAH ! You're dead");
        }       
        
        collider.center = new Vector3 (0,1,0);
        _netAnimator.SetTrigger("Death");

        OnDeath -= Death;
    }
}
