using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Animator))]

/// <summary>  
/// 	Player controller
/// </summary>
public class PlayerController : Living
{
    public Transform cam;

    public enum PlayerClassEnum
    {
        Assassin,
        Healer,
        Sorcerer,
        Tank,
    };

    public GameUI gameUI;

    [Header("Weapon")]
    public Transform weaponGrip;

    private Animator _animator;
    private NetworkAnimator _netAnimator;
    private Rigidbody rigidBody;
    public PlayerClassEnum playerClassID;
    public Weapon weapon;
    public GameObject WeaponObject;
    float lastShotTime = 0;

    public int MaxMana { get { return (int)maxMana; } }
    public int CurrentMana { get { return (int)curMana; } }

    float manaFillRate = 0.2f;
    float lastManaFill = 0;

    public Vector3 weaponDetectionRange;

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
    [SyncVar]    
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
        weapon = WeaponObject.GetComponent<Weapon>();
        var droppedWeapon = WeaponObject.GetComponent<DroppedWeapon>();
        if (droppedWeapon != null)
            Destroy(droppedWeapon);
       
        if (!isLocalPlayer)
        {
            Destroy(cam.gameObject);
        } else {
            CmdApplyMoveStatus(MoveStatus.Free); 

            Lobby.LobbyManager.curGamePlayer = gameObject;
        }
    }

    /// <summary>  
    /// 	Translate and rotate player
    ///		Updates animator
    /// </summary>
    void Update()
    {

        if (isLocalPlayer)
        {
            FillMana();
            CheckForWeapon();
            UpdateTarget();
        }
    }

    /// <summary>  
    ///     Camera follow player and player orientation depend of the camera
    /// 	Translate and rotate player
    /// 	Jump player if input
    ///		Updates animator
    ///		Take move status into consideration
    /// </summary>
    void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            Vector3 dir = (cam.right * Input.GetAxis("Horizontal") * Time.deltaTime) + (cam.forward * Input.GetAxis("Vertical") /** Time.deltaTime*/);
            dir.y = 0;
            if (canMove)
            {
                if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
                {
                    rigidBody.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), turnSpeed);
                    var v3 = Vector3.zero;
                    if (canRun)
                    {
                        v3 = transform.forward * speed;
                        v3.y = rigidBody.velocity.y;
                    }
                    else
                    {
                        v3 = transform.forward * speed/ RunFactor;
                        v3.y = rigidBody.velocity.y;
                    }
                    rigidBody.velocity = v3;
                    if (Input.GetAxis("Horizontal") != 0)
                    {
                        _animator.SetFloat("Speed", speed);
                    }

                    if (Input.GetAxis("Vertical") != 0)
                    {
                        _animator.SetFloat("Speed", speed);
                    }
                }
            }
            
            if (Input.GetButton("Fire1"))
            {
                if (Time.time - lastShotTime > weapon.FiringInterval)
                    Fire();
            }

            if (Input.GetButton("Jump") && isGrounded && canJump)
            {
                _netAnimator.SetTrigger("Jump");
                if (lowJump)
                {
                    rigidBody.AddForce(Vector3.up * JumpSpeed / JumpFactor, ForceMode.Impulse);
                }
                else
                {
                    rigidBody.AddForce(Vector3.up * JumpSpeed, ForceMode.Impulse);
                }
            }

            if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0 && !Input.GetButton("Jump"))
            {
                _animator.SetFloat("Speed", 0);
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
                    Destroy(closestObj.GetComponent<DroppedWeapon>());
                    GameObject dropWeapon = WeaponObject;
                    dropWeapon.transform.SetParent(null);
                    dropWeapon.transform.localPosition = new Vector3(dropWeapon.transform.localPosition.x,0,dropWeapon.transform.localPosition.z);
                    dropWeapon.transform.rotation = Quaternion.identity;
                    dropWeapon.AddComponent<DroppedWeapon>();
                    WeaponObject = closestObj;
                    WeaponObject.transform.SetParent(weaponGrip);
                    WeaponObject.transform.localPosition = Vector3.zero;
                    WeaponObject.transform.localRotation = Quaternion.identity;
                    weapon = WeaponObject.GetComponent<Weapon>();
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

            if ((endPoint - hit.point).sqrMagnitude > (endPoint - weapon.SpellOrigin.position).sqrMagnitude)
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

    /// <summary>  
    /// 	Use to allow player another jump after hitting the ground
    /// </summary>
    void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.tag.Equals("Ground"))
        {
            isGrounded = true;
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
}
