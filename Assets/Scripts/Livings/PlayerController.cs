using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Animator))]

/// <summary>  
/// 	Player controller
/// </summary>
public class PlayerController : Living {

    public enum PlayerClassEnum
    {
        Assassin,
        Soigneur,
        Sorcier,
        Tank,
    };

    public Camera cam;
    public GameUI gameUI;

	public Transform spellOrigin;
	public GameObject ammo;
    public Transform weaponGrip;

    public PlayerClassEnum playerClass;
    public Weapon weapon;
    public GameObject WeaponObject;
    float lastShotTime = 0;

    public int MaxMana { get { return maxMana; } }
    int maxMana = 100;
    public int CurrentMana { get { return currentMana; } }
    int currentMana = 100;

    float manaFillRate = 0.2f;
    float lastManaFill = 0;

    public Vector3 weaponDetectionRange;

    private Animator _animator;

	/// <summary>  
	/// 	Fetch animator
	///		Destroy camera if not localplayer
	/// </summary>
	void Start() {
		_animator = GetComponent<Animator>();
        weapon = WeaponObject.GetComponent<Weapon>();
        var droppedWeapon = WeaponObject.GetComponent<DroppedWeapon>();
        if (droppedWeapon != null)
            Destroy(droppedWeapon);
        /*
		if (!isLocalPlayer) {
			Destroy(cam.gameObject);
		}*/		
	}

	/// <summary>  
	/// 	Translate and rotate player
	///		Updates animator
	/// </summary>
	void Update () {

        //if (isLocalPlayer) 
        if (true)
        {
            float x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
			float z = Input.GetAxis("Vertical") * Time.deltaTime * speed;

			transform.Rotate(0, x, 0);
			transform.Translate(0, 0, z);

			if (Input.GetButtonDown("Fire1")) 
                if (Time.time - lastShotTime > weapon.FiringInterval)
                    Fire();

			if (Input.GetButtonDown("Jump") && canJump) {
				_animator.SetTrigger("Jump");
			}

			_animator.SetFloat("Speed", z);
		}

        FillMana();
        CheckForWeapon();
	}

    void CheckForWeapon()
    {
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
                if (closestWeapon.CanEquip(playerClass))
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
            currentMana += 1;
            currentMana = Mathf.Clamp(currentMana, 0, maxMana);
            lastManaFill = Time.time;
        }
    }



    void Fire()
    {
        if (weapon.UseMana && currentMana < weapon.ManaCost)
            return;
        else if (weapon.UseMana)
            currentMana -= weapon.ManaCost;

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
        
        GameObject bullet = Instantiate(weapon.Bullet, weapon.SpellOrigin.position, rot);
        Physics.IgnoreCollision(bullet.GetComponent<Collider>(), GetComponentInParent<Collider>(), true);
        bullet.GetComponent<Bullet>().Direction = direction;
        lastShotTime = Time.time;
    }
	/// <summary>  
	/// 	Instanciate and spawn bullet
	/// </summary>
	[Command]
	void CmdFire() {
		GameObject bullet = Instantiate(weapon.Bullet, weapon.SpellOrigin.position, weapon.SpellOrigin.rotation);
        Physics.IgnoreCollision(bullet.GetComponent<Collider>(), GetComponentInParent<Collider>(),true);
        bullet.GetComponent<Bullet>().OwnerTag = gameObject.tag;
		bullet.GetComponent<Rigidbody>().velocity = weapon.SpellOrigin.forward * 10;
        
		NetworkServer.Spawn(bullet);
	}
}
