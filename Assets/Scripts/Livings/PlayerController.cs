using System;
using System.Collections;
using UnityEngine;
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
            inventory.weaponGrip = cd.weaponGrip;
        }

        if (!isLocalPlayer)
        {
            inventory.InitializedOtherClient();
        }
        
        if (isLocalPlayer)
        {
            PlayerClassDesignation cd = currentClassObject.GetComponent<PlayerClassDesignation>();
            inventory.InitializeWeaponInformation(new NetworkInstanceId((uint)defaultWeaponId), cd);
            inventory.CmdPickupWeapon(new NetworkInstanceId((uint)defaultWeaponId), netId);
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
            UpdateJump();
            FillMana();
            UpdateTarget();

            Vector3 locVel = transform.InverseTransformDirection(rigidBody.velocity);

            if (_animator != null) {
                _animator.SetFloat("SpeedZ", locVel.z);
                _animator.SetFloat("SpeedX", locVel.x);
            }  
        } else if (rigidBody != null) {
            Destroy(rigidBody);
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
        if (isLocalPlayer)
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

    void OnDestroy()
    {
        Destroy(GameObject.Find("GameUI"));
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
            UpdateMana(CurrentMana + 1);
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
        jumpHeight = cd.jumpHeight;

        fire = cd.fire;
        ice = cd.ice;
        lightning = cd.lightning;
        poison = cd.poison;
        physical = cd.physical;

        NetworkServer.SpawnWithClientAuthority(go, gameObject);
        currentClassObject = go;

        GameObject w = Instantiate(cd.defaultWeapon, cd.weaponGrip);
        Destroy(w.GetComponent<DroppedWeapon>());
        defaultWeaponId = (int)w.GetComponent<NetworkIdentity>().netId.Value;
        
        NetworkServer.SpawnWithClientAuthority(w, gameObject);
        RpcClassUpdated(cd.netId, w.GetComponent<NetworkIdentity>().netId);
    }

    [ClientRpc]
    private void RpcClassUpdated(NetworkInstanceId classModelId, NetworkInstanceId weaponNetId){
        PlayerClassDesignation cd = ClientScene.FindLocalObject(classModelId).GetComponent<PlayerClassDesignation>();
        GameObject weaponObj = ClientScene.FindLocalObject(weaponNetId);
        defaultWeaponId = (int)weaponObj.GetComponent<NetworkIdentity>().netId.Value;
        cd.transform.SetParent(transform);
        cd.transform.localPosition = Vector3.zero;
        cd.transform.localRotation = Quaternion.identity;

        _animator = cd.GetComponent<Animator>();
        _netAnimator = cd.GetComponent<NetworkAnimator>();

        inventory.weaponGrip = cd.weaponGrip;
        inventory.InitializeWeaponInformation(weaponNetId, cd);
    }
}
