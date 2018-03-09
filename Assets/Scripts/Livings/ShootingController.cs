using UnityEngine;
using UnityEngine.Networking;

public class ShootingController : NetworkBehaviour {

    const float RAY_LENGTH = 20f;

    public Transform cam;

    [Header("Weapon")]
    public Living owner;
    
    public Weapon weapon;
    float lastShotTime = 0;
    bool firing;
    Bullet persistentBullet;
    float lastManaDrain;

    float offsetValue;
    Vector3 offset;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (weapon == null)
            return;

        if (isLocalPlayer)
            UpdateFire(); 
    }

    void UpdateFire()
    {
        if (!owner.dead)
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
                CmdStopContinousFire();
            }

            if (firing)
            {
                if (Time.time - lastManaDrain > weapon.FiringInterval)
                {
                    if (owner.curMana - weapon.ManaCost > 0)
                    {
                        owner.UpdateMana(owner.curMana - weapon.ManaCost);
                        CmdUpdatePersistentBullet();
                    }
                    else
                    {
                        owner.UpdateMana(0);
                        CmdStopContinousFire();
                    }
                    lastManaDrain = Time.time;
                }
            }
        }
    }

    [Command]
    void CmdUpdatePersistentBullet()
    {
        RpcUpdatePersistentBullet();
    }

    [ClientRpc]
    void RpcUpdatePersistentBullet()
    {
        if(persistentBullet != null)
            persistentBullet.UpdatePosition();
    }

    [Command]
    public void CmdStopContinousFire()
    {
        RpcStopContinousFire();
    }

    [ClientRpc]
    void RpcStopContinousFire()
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
    /// Client side compute fire data
    /// </summary>
    void Fire()
    {
        if (weapon.UseMana && owner.curMana < weapon.ManaCost)
            return;
        else if (weapon.UseMana)
            owner.UpdateMana(owner.curMana - weapon.ManaCost);

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

        offset = Vector3.zero;
        if (weapon.shootingOffset)
            offset = new Vector3(0, offsetValue / RAY_LENGTH, 0);

        if (weapon.SpreadBullet)
        {
            for (int i = 0; i < weapon.NumberOfBullet; i++)
            {
                Vector3 end = new Vector3(endPoint.x + UnityEngine.Random.Range(-weapon.SpreadAngle, weapon.SpreadAngle), endPoint.y + UnityEngine.Random.Range(-weapon.SpreadAngle, weapon.SpreadAngle), endPoint.z + UnityEngine.Random.Range(-weapon.SpreadAngle, weapon.SpreadAngle));
                Vector3 spreadDirection = (end - weapon.SpellOrigin.position).normalized;
                var spreadRot = Quaternion.LookRotation(spreadDirection, Vector3.up);
                CmdFire(spreadDirection, spreadRot, offset);
            }
        }
        else
            CmdFire(direction, rot, offset);

        weapon.PlaySound();
        lastShotTime = Time.time;
    }

    /// <summary>  
    /// 	Instanciate and spawn bullet
    /// </summary>
    [Command]
    void CmdFire(Vector3 direction, Quaternion rot, Vector3 shootingOffset)
    {
        GameObject bulletObj = Instantiate(weapon.Bullet, weapon.SpellOrigin.position, rot);
        Bullet bullet = bulletObj.GetComponent<Bullet>();

        Physics.IgnoreCollision(bulletObj.GetComponent<Collider>(), GetComponentInParent<Collider>(), true);
        bullet.OwnerTag = gameObject.tag;
        bullet.spawnedBy = netId;

        offset = Vector3.zero;
        if (weapon.shootingOffset)
            offset = new Vector3(0, offsetValue / RAY_LENGTH, 0);

        bullet.Direction = direction + shootingOffset;
        bullet.GetComponent<Bullet>().SpellOrigin = weapon;

        if (weapon.DrainMana)
            persistentBullet = bullet;

        NetworkServer.Spawn(bulletObj);

        RpcFire(bullet.netId, weapon.gameObject.GetComponent<NetworkIdentity>().netId);
    }

    [ClientRpc]
    void RpcFire(NetworkInstanceId bulletId, NetworkInstanceId weaponId)
    {
        GameObject bulletObj = ClientScene.FindLocalObject(bulletId);
        GameObject weaponObj = ClientScene.FindLocalObject(weaponId);
        Bullet bullet = bulletObj.GetComponent<Bullet>();
        bullet.OwnerTag = gameObject.tag;
        if (weaponObj.GetComponent<Weapon>().DrainMana)
            persistentBullet = bullet;
        bullet.SpellOrigin = weaponObj.GetComponent<Weapon>();
    }
}
