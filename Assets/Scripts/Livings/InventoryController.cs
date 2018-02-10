using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class InventoryController : NetworkBehaviour {

    public Transform weaponGrip;
    public GameUI gameUI;
    public Vector3 weaponDetectionRange;
    public PlayerController player;
    public ShootingController shootingController;

    Weapon.WeaponTypeEnum currentWeapon = Weapon.WeaponTypeEnum.Base;
    public Weapon.WeaponTypeEnum CurrentWeapon { get { return currentWeapon; } }

    Dictionary<Weapon.WeaponTypeEnum, GameObject> weaponDictionary = new Dictionary<Weapon.WeaponTypeEnum, GameObject>()
    {
        {Weapon.WeaponTypeEnum.Base, null},
        {Weapon.WeaponTypeEnum.ShortRange, null},
        {Weapon.WeaponTypeEnum.LongRange, null}
    };

    List<Weapon.WeaponTypeEnum> weaponTypeList = new List<Weapon.WeaponTypeEnum>()
    {
        Weapon.WeaponTypeEnum.Base,
        Weapon.WeaponTypeEnum.ShortRange,
        Weapon.WeaponTypeEnum.LongRange,
    };

    int weaponTypeIndex;

    public void InitializeWeaponInformation(GameObject weaponObj, PlayerClassDesignation playerClass)
    {
        if (playerClass != null)
        {
            Weapon weapon;
            if (weaponObj != null)
            {
                weapon = weaponObj.GetComponent<Weapon>();
                weaponDictionary[weapon.WeaponType] = weaponObj;
            }

            PlayerClassDesignation cd = playerClass.GetComponent<PlayerClassDesignation>();

            cd.transform.SetParent(transform);
            cd.transform.localPosition = Vector3.zero;
            cd.transform.localRotation = Quaternion.identity;

            weaponGrip = cd.weaponGrip;

            weaponObj.transform.SetParent(weaponGrip);
            weaponObj.transform.localPosition = Vector3.zero;
            weaponObj.transform.localRotation = Quaternion.identity;
            
            weapon = weaponObj.GetComponent<Weapon>();
        }
    }

    // Use this for initialization
    void Start () {
        GameObject ui = GameObject.Find("GameUI");
        if (ui != null)
            gameUI = GameObject.Find("GameUI").GetComponent<GameUI>();

        weaponTypeIndex = (int)currentWeapon;
	}
	
	// Update is called once per frame
	void Update () {
        UpdateInput();
        CheckForWeapon();
    }

    void UpdateInput()
    {
        if (Input.GetButtonDown("Weapon1"))
            if (weaponDictionary[Weapon.WeaponTypeEnum.Base] != null)
                SwitchEquippedWeapon(Weapon.WeaponTypeEnum.Base);

        if (Input.GetButtonDown("Weapon2"))
            if (weaponDictionary[Weapon.WeaponTypeEnum.ShortRange] != null)
                SwitchEquippedWeapon(Weapon.WeaponTypeEnum.ShortRange);

        if (Input.GetButtonDown("Weapon3"))
            if (weaponDictionary[Weapon.WeaponTypeEnum.LongRange] != null)
                SwitchEquippedWeapon(Weapon.WeaponTypeEnum.LongRange);

        if (Input.GetAxis("Mouse ScrollWheel") > 0f) 
        {
            int index = weaponTypeIndex;
            index++;
            if (index == weaponTypeList.Count)
                index = 0;

            if (weaponDictionary[weaponTypeList[index]] != null)
            {
                SwitchEquippedWeapon(weaponTypeList[index]);
                weaponTypeIndex = index;
            }
            else
            {
                index++;
                if (index == weaponTypeList.Count)
                    index = 0;

                if (weaponDictionary[weaponTypeList[index]] != null)
                {
                    SwitchEquippedWeapon(weaponTypeList[index]);
                    weaponTypeIndex = index;
                }
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            int index = weaponTypeIndex;
            index--;
            if (index < 0)
                index = weaponTypeList.Count - 1;

            if (weaponDictionary[weaponTypeList[index]] != null)
            {
                SwitchEquippedWeapon(weaponTypeList[index]);
                weaponTypeIndex = index;
            }
            else
            {
                index--;
                if (index < 0)
                    index = weaponTypeList.Count - 1;

                if (weaponDictionary[weaponTypeList[index]] != null)
                {
                    SwitchEquippedWeapon(weaponTypeList[index]);
                    weaponTypeIndex = index;
                }
            }
        }
    }

    void SwitchEquippedWeapon(Weapon.WeaponTypeEnum weaponType)
    {
        if (weaponDictionary[weaponType] != null)
        {
            if (weaponDictionary[currentWeapon] != null)
                weaponDictionary[currentWeapon].SetActive(false);

            currentWeapon = weaponType;
            weaponDictionary[currentWeapon].SetActive(true);
            shootingController.weapon = weaponDictionary[currentWeapon].GetComponent<Weapon>();
            gameUI.SetWeaponImages(weaponDictionary);
        }
    }

    void DropWeapon(Weapon.WeaponTypeEnum weaponType)
    {
        if (weaponDictionary[weaponType] != null)
        {
            GameObject dropWeapon = weaponDictionary[weaponType];
            dropWeapon.transform.SetParent(null);
            dropWeapon.transform.localPosition = new Vector3(dropWeapon.transform.localPosition.x, 0, dropWeapon.transform.localPosition.z);
            dropWeapon.transform.rotation = Quaternion.identity;
            dropWeapon.SetActive(true);
            dropWeapon.AddComponent<DroppedWeapon>();
            weaponDictionary[weaponType] = null;
        } 
    }

    public Weapon GetWeapon(Weapon.WeaponTypeEnum weaponType)
    {
        if (weaponDictionary[weaponType] == null)
            return null;

        return weaponDictionary[weaponType].GetComponent<Weapon>();
    }

    /// <summary>
    /// Ask the server to pickup the weapon
    /// </summary>    
    [Command]
    public void CmdPickupWeapon(NetworkInstanceId weaponNetId)
    {
        RpcPickupWeapon(weaponNetId);
    }

    /// <summary>
    /// Pickup weapon on ALL CLIENTS!
    /// </summary> 
    [ClientRpc]
    void RpcPickupWeapon(NetworkInstanceId weaponNetId)
    {
        GameObject closestObj = ClientScene.FindLocalObject(weaponNetId);

        if (closestObj == null)
        {
            Debug.LogError("Weapon: " + weaponNetId + " not found!");
            return;
        }

        Weapon weapon = closestObj.GetComponent<Weapon>();

        if (weaponDictionary[weapon.WeaponType] != null)
            DropWeapon(weapon.WeaponType);

        Destroy(closestObj.GetComponent<DroppedWeapon>());

        GameObject WeaponObject = closestObj;
        WeaponObject.transform.SetParent(weaponGrip);
        WeaponObject.transform.localPosition = Vector3.zero;
        WeaponObject.transform.localRotation = Quaternion.identity;
        weapon = WeaponObject.GetComponent<Weapon>();
        weaponDictionary[weapon.WeaponType] = WeaponObject;
        shootingController.weapon = weapon;
        shootingController.WeaponObject = WeaponObject;
        SwitchEquippedWeapon(weapon.WeaponType);
    }

    /// <summary>
    /// Ask the server to switch equipped weapon
    /// </summary>    
    [Command]
    void CmdSwitchEquippedWeapon()
    {

    }

    /// <summary>
    /// Switch equipped weapon on ALL CLIENTS!
    /// </summary> 
    [ClientRpc]
    void RpcSwithEquippedWeapon()
    {

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
                if (closestWeapon.CanEquip(player.playerClassID))
                {
                    shootingController.StopContinuouFire();
                    CmdPickupWeapon(closestObj.GetComponent<NetworkIdentity>().netId);
                }
            }
        }
    }
}
