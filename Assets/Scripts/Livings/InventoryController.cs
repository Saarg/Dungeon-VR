using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class InventoryController : NetworkBehaviour {

    public Transform weaponGrip;
    public GameUI gameUI;
    public Vector3 weaponDetectionRange;
    public PlayerController player;
    public ShootingController shootingController;
    
    [SyncVar] int baseId;
    [SyncVar] int shortId;
    [SyncVar] int longId;

    Weapon.WeaponTypeEnum currentWeapon = Weapon.WeaponTypeEnum.ShortRange;
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
        Weapon.WeaponTypeEnum.LongRange,
        Weapon.WeaponTypeEnum.ShortRange,
    };

    [SyncVar]
    int weaponTypeIndex;

    public void InitializedOtherClient()
    {
        if(baseId != 0)
            InitializeOtherClientWeapon(Weapon.WeaponTypeEnum.Base, baseId);

        if (shortId != 0)
            InitializeOtherClientWeapon(Weapon.WeaponTypeEnum.ShortRange, shortId);

        if (longId != 0)
            InitializeOtherClientWeapon(Weapon.WeaponTypeEnum.LongRange, longId);
    }

    void InitializeOtherClientWeapon(Weapon.WeaponTypeEnum weaponType, int id)
    {
        GameObject weaponObj = ClientScene.FindLocalObject(new NetworkInstanceId((uint)id));
        weaponObj.transform.SetParent(weaponGrip.transform);
        Destroy(weaponObj.GetComponent<DroppedWeapon>());
        weaponDictionary[weaponType] = weaponObj;
        weaponObj.SetActive(weaponTypeIndex == (int)weaponType);
        if (weaponTypeIndex == (int)weaponType)
        {
            currentWeapon = weaponType;
            shootingController.weapon = weaponDictionary[currentWeapon].GetComponent<Weapon>();
        }
    }

    public void InitializeWeaponInformation(NetworkInstanceId weaponId, PlayerClassDesignation playerClass)
    {
        if (playerClass != null)
            CmdInitializeWeapon(weaponId, player.netId, playerClass.netId);
    }

    /// <summary>
    /// Ask the server to initialize weapon
    /// </summary>    
    [Command]
    void CmdInitializeWeapon(NetworkInstanceId weaponNetId, NetworkInstanceId playerNetId, NetworkInstanceId playerClassNetId)
    {
        RpcInitializeWeapon(weaponNetId, playerNetId, playerClassNetId);
    }

    /// <summary>
    /// Switch initialize weapon on ALL CLIENTS!
    /// </summary> 
    [ClientRpc]
    void RpcInitializeWeapon(NetworkInstanceId weaponNetId, NetworkInstanceId playerNetId, NetworkInstanceId playerClassNetId)
    {
        GameObject weaponObj = ClientScene.FindLocalObject(weaponNetId);
        GameObject playerObj = ClientScene.FindLocalObject(playerNetId);
        GameObject playerClassObj = ClientScene.FindLocalObject(playerClassNetId);
        playerObj.GetComponent<PlayerController>().inventory.InitializeWeapon(weaponObj, playerClassObj.GetComponent<PlayerClassDesignation>());
    }

    public void InitializeWeapon(GameObject weaponObj, PlayerClassDesignation playerClass)
    {
        Weapon weapon;
        if (weaponObj != null)
        {
            weapon = weaponObj.GetComponent<Weapon>();
            weaponDictionary[weapon.WeaponType] = weaponObj;
        }
        if(weaponObj.GetComponent<DroppedWeapon>() != null)
            Destroy(weaponObj.GetComponent<DroppedWeapon>());
        PlayerClassDesignation cd = playerClass.GetComponent<PlayerClassDesignation>();

        cd.transform.SetParent(transform);
        cd.transform.localPosition = Vector3.zero;
        cd.transform.localRotation = Quaternion.identity;

        weaponGrip = cd.weaponGrip;

        weaponObj.transform.SetParent(weaponGrip);
        weaponObj.transform.localPosition = Vector3.zero;
        weaponObj.transform.localRotation = Quaternion.identity;

        weapon = weaponObj.GetComponent<Weapon>();
        uint id = weaponObj.GetComponent<NetworkIdentity>().netId.Value;
        shootingController.weapon = weaponDictionary[currentWeapon].GetComponent<Weapon>();
        SetId(id, weapon.WeaponType);
        if (player.isLocalPlayer)
            gameUI.SetWeaponImages(weaponDictionary);
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
        if (player.isLocalPlayer)
        {
            UpdateInput();
            CheckForWeapon();
        }
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
            }
            else
            {
                index++;
                if (index == weaponTypeList.Count)
                    index = 0;

                if (weaponDictionary[weaponTypeList[index]] != null)
                {
                    SwitchEquippedWeapon(weaponTypeList[index]);
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
            }
            else
            {
                index--;
                if (index < 0)
                    index = weaponTypeList.Count - 1;

                if (weaponDictionary[weaponTypeList[index]] != null)
                {
                    SwitchEquippedWeapon(weaponTypeList[index]);
                }
            }
        }
    }

    void SwitchEquippedWeapon(Weapon.WeaponTypeEnum weaponType)
    {
        if (weaponDictionary[weaponType] != null)
            CmdSwitchEquippedWeapon(weaponDictionary[weaponType].GetComponent<NetworkIdentity>().netId, player.netId); 
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
    public void CmdPickupWeapon(NetworkInstanceId weaponNetId, NetworkInstanceId playerNetId)
    {
        RpcPickupWeapon(weaponNetId, playerNetId);
    }

    /// <summary>
    /// Pickup weapon on ALL CLIENTS!
    /// </summary> 
    [ClientRpc]
    void RpcPickupWeapon(NetworkInstanceId weaponNetId, NetworkInstanceId playerNetId)
    {
        GameObject weapon = ClientScene.FindLocalObject(weaponNetId);
        GameObject player = ClientScene.FindLocalObject(playerNetId);

        player.GetComponent<PlayerController>().inventory.PickUpWeapon(weapon);
    }

    public void PickUpWeapon(GameObject weaponObj)
    {
        uint id = weaponObj.GetComponent<NetworkIdentity>().netId.Value;
        
        Weapon weapon = weaponObj.GetComponent<Weapon>();

        if (weaponDictionary[weapon.WeaponType] != null)
            DropWeapon(weapon.WeaponType);

        Destroy(weaponObj.GetComponent<DroppedWeapon>());

        GameObject WeaponObject = weaponObj;
        WeaponObject.transform.SetParent(weaponGrip);
        WeaponObject.transform.localPosition = Vector3.zero;
        WeaponObject.transform.localRotation = Quaternion.identity;
        weapon = WeaponObject.GetComponent<Weapon>();
        weaponDictionary[weapon.WeaponType] = WeaponObject;
        shootingController.weapon = weapon;
        if(player.isLocalPlayer)
            SwitchEquippedWeapon(weapon.WeaponType);
        SetId(id, weapon.WeaponType);
    }

    /// <summary>
    /// Ask the server to switch equipped weapon
    /// </summary>    
    [Command]
    void CmdSwitchEquippedWeapon(NetworkInstanceId weaponNetId, NetworkInstanceId playerNetId)
    {
        RpcSwithEquippedWeapon(weaponNetId, playerNetId);
    }

    /// <summary>
    /// Switch equipped weapon on ALL CLIENTS!
    /// </summary> 
    [ClientRpc]
    void RpcSwithEquippedWeapon(NetworkInstanceId weaponNetId, NetworkInstanceId playerNetId)
    {
        GameObject weaponObj = ClientScene.FindLocalObject(weaponNetId);
        GameObject playerObj = ClientScene.FindLocalObject(playerNetId);
        playerObj.GetComponent<PlayerController>().inventory.SwitchWeapon(weaponObj);
    }

    public void SwitchWeapon(GameObject weaponObj)
    {
        Weapon weapon = weaponObj.GetComponent<Weapon>();

        if (weaponDictionary[currentWeapon] != null)
            weaponDictionary[currentWeapon].SetActive(false);

        currentWeapon = weapon.WeaponType;
        weaponTypeIndex = (int)currentWeapon;
        weaponDictionary[currentWeapon].SetActive(true);
        shootingController.weapon = weaponDictionary[currentWeapon].GetComponent<Weapon>();
        if(player.isLocalPlayer)
            gameUI.SetWeaponImages(weaponDictionary);
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
                    shootingController.CmdStopContinousFire();
                    CmdPickupWeapon(closestObj.GetComponent<NetworkIdentity>().netId, player.netId);
                }
            }
        }
    }

    void SetId(uint id, Weapon.WeaponTypeEnum weaponType)
    {
        if (weaponType == Weapon.WeaponTypeEnum.Base)
            baseId = (int)id;
        else if (weaponType == Weapon.WeaponTypeEnum.ShortRange)
            shortId = (int)id;
        else if (weaponType == Weapon.WeaponTypeEnum.LongRange)
            longId = (int)id;
    }
}
