using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour {

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

    Weapon lastShownWeapon = null;

    // Use this for initialization
    void Start () {
		
	}
	
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

            if (player.weapon.Bullet.GetComponent<Bullet>().Damage > bullet.Damage)
                damageModifier.color = Color.red;
            else if (player.weapon.Bullet.GetComponent<Bullet>().Damage == bullet.Damage)
                damageModifier.color = Color.black;
            else
                damageModifier.color = Color.green;

            int damageDifference = player.weapon.Bullet.GetComponent<Bullet>().Damage - bullet.Damage;
            if (damageDifference > 0)
                damageModifier.text = string.Format("(-{0})", damageDifference);
            else
                damageModifier.text = string.Format("(+{0})", Mathf.Abs(damageDifference));

            if (player.weapon.FiringInterval > weapon.FiringInterval)
                fireRateModifier.color = Color.green;
            else if (player.weapon.FiringInterval == weapon.FiringInterval)
                fireRateModifier.color = Color.black;
            else
                fireRateModifier.color = Color.red;

            float fireRateDifference = player.weapon.FiringInterval - weapon.FiringInterval;
            if (fireRateDifference < 0)
                fireRateModifier.text = string.Format("(+{0})", Mathf.Abs(fireRateDifference));
            else
                fireRateModifier.text = string.Format("(-{0})", fireRateDifference);

            if (player.weapon.ManaCost > weapon.ManaCost)
                manaCostModifier.color = Color.green;
            else if (player.weapon.ManaCost == weapon.ManaCost)
                manaCostModifier.color = Color.black;
            else
                manaCostModifier.color = Color.red;

            int manaCostDifference = player.weapon.ManaCost - weapon.ManaCost;
            if (manaCostDifference > 0)
                manaCostModifier.text = string.Format("(-{0})", manaCostDifference);
            else
                manaCostModifier.text = string.Format("(+{0})", Mathf.Abs(manaCostDifference));

            fireIcon.SetActive(bullet.DamageType == Bullet.DamageTypeEnum.fire);
            iceIcon.SetActive(bullet.DamageType == Bullet.DamageTypeEnum.ice);
            lightningIcon.SetActive(bullet.DamageType == Bullet.DamageTypeEnum.lightning);
            poisonIcon.SetActive(bullet.DamageType == Bullet.DamageTypeEnum.poison);

            weaponStats.SetActive(true);
            lastShownWeapon = weapon;
        }
    }
}
