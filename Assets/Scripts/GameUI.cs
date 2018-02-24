using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text;

public class GameUI : MonoBehaviour {
    [SerializeField] Canvas mainCanvas;

    [Header("Player")]
    [SerializeField]
    Canvas playerUI;
    [SerializeField]
    Image healthBar;

    [SerializeField]
    Image manaBar;

    [SerializeField]
    Image targetBar;

    [SerializeField]
    PlayerController player;

    [SerializeField]
    GameObject weaponStats;

    [SerializeField]
    GameObject canEquipIcon;

    [SerializeField]
    Text weaponName;

    [SerializeField]
    Text weaponType;

    [SerializeField]
    Text damage;

    [SerializeField]
    Text fireRate;

    [SerializeField]
    Text manaCost;

    [SerializeField]
    GameObject fireIcon;

    [SerializeField]
    GameObject iceIcon;

    [SerializeField]
    GameObject lightningIcon;

    [SerializeField]
    GameObject poisonIcon;

    [SerializeField]
    Text damageModifier;

    [SerializeField]
    Text fireRateModifier;

    [SerializeField]
    Text manaCostModifier;

    [SerializeField]
    Image baseWeaponImage;

    [SerializeField]
    Image shortRangeWeaponImage;

    [SerializeField]
    Image longRangeWeaponImage;

    [SerializeField]
    GameObject baseWeaponIcon;

    [SerializeField]
    GameObject shortWeaponIcon;

    [SerializeField]
    GameObject longWeaponIcon;

    Weapon lastShownWeapon = null;

    [Header("GameMode")]
    [SerializeField]
    Canvas gamemodeUI;
    [SerializeField]
    Text gamemode;
    [SerializeField]
    Text timer;

    [Header("Team")]
    [SerializeField]
    Canvas teamUI;
    [SerializeField]
    List<PlayerController> team = new List<PlayerController>();
    [SerializeField]
    GameObject teamPlayerPrefab;

    [Header("VR")]
    [SerializeField] Canvas vrUI;
    [SerializeField] Vector3 pos;
    [SerializeField] Vector3 rot;   
    [SerializeField] Vector3 scale;
    [SerializeField] bool _isVr;
    public bool isVr {
        get { return _isVr; }
        set {
            vrUI.gameObject.SetActive(value);

            mainCanvas.renderMode = value ? RenderMode.WorldSpace : RenderMode.ScreenSpaceOverlay;
            mainCanvas.transform.position = pos;
            mainCanvas.transform.rotation = Quaternion.Euler(rot);
            mainCanvas.transform.localScale = scale;

            _isVr = value;
        }
    }

    Vector3 SelectedWeaponScale = new Vector3(1.25f, 1.25f, 1.25f);
    Vector3 UnselectedWeaponScale = Vector3.one;

	// Update is called once per frame
	void Update () {
        healthBar.fillAmount = (float)player.curLife / (float)player.maxLife;
        manaBar.fillAmount = (float)player.CurrentMana / (float)player.MaxMana;

        targetBar.gameObject.SetActive(player.HasTarget());
        if(player.HasTarget())
            targetBar.fillAmount = (float)player.TargetCurLife() / (float)player.TargetMaxLife();
	}

    public void SetPlayerController(PlayerController playerController)
    {
        player = playerController;

        playerUI.gameObject.SetActive(player != null && !isVr);
    }

    public void SetWeaponImages(Dictionary<Weapon.WeaponTypeEnum, GameObject> weapons)
    {
        baseWeaponIcon.transform.localScale = (player.inventory.CurrentWeapon == Weapon.WeaponTypeEnum.Base) ? SelectedWeaponScale : UnselectedWeaponScale;
        shortWeaponIcon.transform.localScale = (player.inventory.CurrentWeapon == Weapon.WeaponTypeEnum.ShortRange) ? SelectedWeaponScale : UnselectedWeaponScale;
        longWeaponIcon.transform.localScale = (player.inventory.CurrentWeapon == Weapon.WeaponTypeEnum.LongRange) ? SelectedWeaponScale : UnselectedWeaponScale;

        if (weapons[Weapon.WeaponTypeEnum.Base] != null)
        {
            baseWeaponImage.sprite = weapons[Weapon.WeaponTypeEnum.Base].GetComponent<Weapon>().UISprite;
            baseWeaponImage.enabled = true;
        }
        else
            baseWeaponImage.enabled = false;

        if (weapons[Weapon.WeaponTypeEnum.LongRange] != null)
        {
            longRangeWeaponImage.sprite = weapons[Weapon.WeaponTypeEnum.LongRange].GetComponent<Weapon>().UISprite;
            longRangeWeaponImage.enabled = true;
        }
        else
            longRangeWeaponImage.enabled = false;

        if (weapons[Weapon.WeaponTypeEnum.ShortRange] != null)
        {
            shortRangeWeaponImage.sprite = weapons[Weapon.WeaponTypeEnum.ShortRange].GetComponent<Weapon>().UISprite;
            shortRangeWeaponImage.enabled = true;
        }
        else
            shortRangeWeaponImage.enabled = false;
    }

    public void HideWeaponStats()
    {
        weaponStats.SetActive(false);
    }

    public void ShowWeaponStats(Weapon weapon)
    {
        if (weapon == lastShownWeapon)
        {
            weaponStats.SetActive(true);
            return;
        }
        else
        {
            canEquipIcon.SetActive(!weapon.CanEquip(player.playerClassID));
            weaponName.text = weapon.WeaponName;

            switch (weapon.WeaponType)
            {
                case Weapon.WeaponTypeEnum.Base:
                    weaponType.text = "Base";
                    break;
                case Weapon.WeaponTypeEnum.ShortRange:
                    weaponType.text = "Short Range";
                    break;
                case Weapon.WeaponTypeEnum.LongRange:
                    weaponType.text = "Long Range";
                    break;
                default:
                    weaponType.text = "UNDEFINED";
                    break;
            }

            Bullet bullet = weapon.Bullet.GetComponent<Bullet>();

            damage.text = bullet.Damage.ToString();
            fireRate.text = weapon.FiringInterval.ToString() + " Sec";
            manaCost.text = weapon.ManaCost.ToString();

            Weapon inventoryWeapon = player.GetWeapon(weapon.WeaponType);

            damageModifier.text = string.Empty;
            fireRateModifier.text = string.Empty;
            manaCostModifier.text = string.Empty;

            if (inventoryWeapon != null && inventoryWeapon.Bullet.GetComponent<Bullet>().Damage > bullet.Damage)
                damageModifier.color = Color.red;
            else if (inventoryWeapon != null && inventoryWeapon.Bullet.GetComponent<Bullet>().Damage == bullet.Damage)
                damageModifier.color = Color.black;
            else
                damageModifier.color = Color.green;

            if (inventoryWeapon != null) {
                int damageDifference = inventoryWeapon.Bullet.GetComponent<Bullet>().Damage - bullet.Damage;
                if (damageDifference > 0)
                    damageModifier.text = string.Format("(-{0})", damageDifference);
                else
                    damageModifier.text = string.Format("(+{0})", Mathf.Abs(damageDifference));
            } else {
                damageModifier.text = "";
            }

            if (inventoryWeapon != null && inventoryWeapon.FiringInterval > weapon.FiringInterval)
                fireRateModifier.color = Color.green;
            else if (inventoryWeapon != null && inventoryWeapon.FiringInterval == weapon.FiringInterval)
                fireRateModifier.color = Color.black;
            else
                fireRateModifier.color = Color.red;

            if (inventoryWeapon != null) {            
                float fireRateDifference = inventoryWeapon.FiringInterval - weapon.FiringInterval;
                if (fireRateDifference < 0)
                    fireRateModifier.text = string.Format("(+{0})", Mathf.Abs(fireRateDifference));
                else
                    fireRateModifier.text = string.Format("(-{0})", fireRateDifference);
            } else {
                manaCostModifier.text = "";
            }

            if (inventoryWeapon != null && inventoryWeapon.ManaCost > weapon.ManaCost)
                manaCostModifier.color = Color.green;
            else if (inventoryWeapon != null && inventoryWeapon.ManaCost == weapon.ManaCost)
                manaCostModifier.color = Color.black;
            else
                manaCostModifier.color = Color.red;

            if (inventoryWeapon != null) {
                int manaCostDifference = inventoryWeapon.ManaCost - weapon.ManaCost;
                if (manaCostDifference > 0)
                    manaCostModifier.text = string.Format("(-{0})", manaCostDifference);
                else
                    manaCostModifier.text = string.Format("(+{0})", Mathf.Abs(manaCostDifference));
            } else {
                manaCostModifier.text = "";
            }

            fireIcon.SetActive(bullet.DamageType == Bullet.DamageTypeEnum.fire);
            iceIcon.SetActive(bullet.DamageType == Bullet.DamageTypeEnum.ice);
            lightningIcon.SetActive(bullet.DamageType == Bullet.DamageTypeEnum.lightning);
            poisonIcon.SetActive(bullet.DamageType == Bullet.DamageTypeEnum.poison);

            weaponStats.SetActive(true);
            lastShownWeapon = weapon;
        }
    }

    public void UpdateGamemodeUI(string mode, float time) {
        gamemodeUI.gameObject.SetActive(true);

        gamemode.text = mode;

        StringBuilder sb = new StringBuilder();

        if (time / 60 < 10) sb.Append("0");
        sb.Append(((int)(time / 60)).ToString());
        sb.Append(":");
        if (time % 60 < 10) sb.Append("0");
        sb.Append(((int)(time % 60)).ToString());

        timer.text = sb.ToString();
    }

    public void AddTeamMate(PlayerController mate) {
        if (mate != null) {
            team.Add(mate);

            GameObject tm = Instantiate(teamPlayerPrefab, teamUI.transform);

            tm.transform.Translate(0, -25f + (team.Count - 1) * -50f, 0);
            tm.GetComponent<UITeamPlayer>().SetPlayercontroller(mate);
        }
    }

    public void RemoveTeamMate(PlayerController mate) {
        if (mate != null) {
            team.Remove(mate);
        }
    }
}
