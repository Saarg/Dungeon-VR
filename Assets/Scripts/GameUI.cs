using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text;
using System.Collections;

public class GameUI : MonoBehaviour {
    [SerializeField] Canvas mainCanvas;

    [Header("Player")]
    [SerializeField]
    Sprite[] classSprites;    
    [SerializeField]
    Image playerClassImage;
    [SerializeField]
    Text playerName;
    [SerializeField]
    Canvas playerUI;
    [SerializeField]
    Image healthBar;

    [SerializeField]
    Image manaBar;

    [SerializeField]
    Image targetBar;
    [SerializeField]
    Image castingBar;
    public Image CastingBar { get { return castingBar; } }

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
    RectTransform[] teammatesUIPosition;
    [SerializeField]
    Canvas teamUI;
    [SerializeField]
    List<PlayerController> team = new List<PlayerController>();
    List<GameObject> teamPlayerUI = new List<GameObject>();
    int currentPos = 0;
    int maxPos;
    [SerializeField]
    GameObject teamPlayerPrefab;

    [Header("Death")]
    [SerializeField]
    Canvas deathUI;

    [Header("Obs")]
    [SerializeField]
    Canvas observerUI;

    [Header("VR")]
    [SerializeField] Canvas vrUI;
    [SerializeField] Vector3 pos;
    [SerializeField] Vector3 rot;   
    [SerializeField] Vector3 scale;
    [SerializeField] GameObject waveUIPrefab;
    [SerializeField] WaveSpawner waveSpawner;
    [SerializeField] bool _isVr;

    [SerializeField] GameObject[] hideInVR;
    public bool isVr {
        get { return _isVr; }
        set {
            vrUI.gameObject.SetActive(value);

            mainCanvas.renderMode = value ? RenderMode.WorldSpace : RenderMode.ScreenSpaceOverlay;
            mainCanvas.transform.position = pos;
            mainCanvas.transform.rotation = Quaternion.Euler(rot);
            mainCanvas.transform.localScale = scale;

            if (value && waveSpawner != null) {
                for (int i = 0; i < waveSpawner.GetWaveLenth(); i++) {
                    WaveUI waveUI = Instantiate(waveUIPrefab, vrUI.transform).GetComponent<WaveUI>();
                    waveUI.wave = waveSpawner.GetWave(i);
                    waveUI.setPos(i);
                }
            }

            foreach(GameObject go in hideInVR) {
                if (go != null)
                    go.SetActive(!value);
            }

            _isVr = value;
        }
    }

    [SerializeField]
    Text goldText;
    [SerializeField]
    Slider goldBar;

    [Header("MenuUI")]
    [SerializeField] Canvas menuUI; 
	[SerializeField] Canvas graphicsMenu;
    [SerializeField] Canvas audioMenu;
	[SerializeField] MenusEnum currentMenu;

	public enum MenusEnum {none, main, sounds, graphics}

    public delegate void QuitEvent();
    public event QuitEvent OnQuit;

    Vector3 SelectedWeaponScale = new Vector3(1.25f, 1.25f, 1.25f);
    Vector3 UnselectedWeaponScale = Vector3.one;

    [Header("Win/Loss")]
    [SerializeField]
    Canvas winUI;
    [SerializeField]
    RectTransform[] teamResumeUIPosition;
    [SerializeField]    
    Canvas lossUI;

    void Start()
    {
        gameObject.name = "GameUI";

        if (waveSpawner == null) {
            waveSpawner = FindObjectOfType<WaveSpawner>();
        }

        ToggleMenu(false);
		graphicsMenu.gameObject.SetActive (false);

		currentMenu = MenusEnum.none;
    }

	void Update () {
        if (player != null && !player.dead) {
            healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, (float)player.curLife / (float)player.maxLife, Time.deltaTime * 2f);
            manaBar.fillAmount = Mathf.Lerp(manaBar.fillAmount, (float)player.CurrentMana / (float)player.MaxMana, Time.deltaTime * 2f);

            targetBar.gameObject.SetActive(player.HasTarget());
            if(player.HasTarget())
                targetBar.fillAmount = Mathf.Lerp(targetBar.fillAmount, (float)player.TargetCurLife() / (float)player.TargetMaxLife(), Time.deltaTime * 2f);
        
            playerName.text = player.name;
            playerClassImage.sprite = classSprites[(int)player.playerClassID];
        } else if (playerUI.gameObject.activeSelf) {
            playerUI.gameObject.SetActive(false);
        }

        team.RemoveAll((item) => item == null);
        teamPlayerUI.RemoveAll((item) => item == null);
        int i = 0;
        teamPlayerUI.ForEach((item) => {
            item.transform.SetParent(teammatesUIPosition[i++]);
            (item.transform as RectTransform).anchoredPosition3D = Vector2.zero;
            (item.transform as RectTransform).localScale = Vector3.one;
        });

		if (Input.GetButtonDown ("Menu") && !_isVr && (currentMenu == MenusEnum.none))
			ShowMenu (MenusEnum.main);
		else if (Input.GetButtonDown ("Menu") && (currentMenu == MenusEnum.main)) {
			HideMenu (MenusEnum.main);
			currentMenu = MenusEnum.none;
		} else if (Input.GetButtonDown ("Menu") && (currentMenu == MenusEnum.graphics)) {
			HideMenu (MenusEnum.graphics);
			ShowMenu (MenusEnum.main);
		} else if (Input.GetButtonDown ("Menu") && (currentMenu == MenusEnum.sounds)) {
			HideMenu (MenusEnum.sounds);
			ShowMenu (MenusEnum.main);
		}
    }

    // Player UI
    public void SetPlayerController(PlayerController playerController)
    {
        player = playerController;
        playerName.text = player.name;
        playerClassImage.sprite = classSprites[(int)player.playerClassID];

        playerUI.gameObject.SetActive(player != null && !isVr);
        ToggleMenu(false);        
    }

    public PlayerController GetPlayerController()
    {
        return player;
    }

    public void SetDeathUI(bool val)
    {
        deathUI.gameObject.SetActive(val);
    }

    public void SetObserverUI(bool val)
    {
        observerUI.gameObject.SetActive(val);
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

            BulletSpec bullet = weapon.Bullet;

            damage.text = bullet.Damage.ToString();
            fireRate.text = weapon.FiringInterval.ToString() + " Sec";
            manaCost.text = weapon.ManaCost.ToString();

            Weapon inventoryWeapon = player.GetWeapon(weapon.WeaponType);

            damageModifier.text = string.Empty;
            fireRateModifier.text = string.Empty;
            manaCostModifier.text = string.Empty;

            if (inventoryWeapon != null && inventoryWeapon.Bullet.Damage > bullet.Damage)
                damageModifier.color = Color.red;
            else if (inventoryWeapon != null && inventoryWeapon.Bullet.Damage == bullet.Damage)
                damageModifier.color = Color.black;
            else
                damageModifier.color = Color.green;

            if (inventoryWeapon != null) {
                int damageDifference = inventoryWeapon.Bullet.Damage - bullet.Damage;
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

    // Gamemode UI
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

        if (time < 5) {
            timer.color = Color.red;

            timer.transform.localScale = Vector3.one * (1f + time%1 * (5 - (int)(time)) / 10);
        } else {
            timer.color = Color.black;

            timer.transform.localScale = Vector3.one * (1f + time%1  / 10);         
        }
    }

    // Team UI
    public void AddTeamMate(PlayerController mate) {
        if (mate != null) {
            GameObject tm = Instantiate(teamPlayerPrefab);
            tm.transform.SetParent(teammatesUIPosition[team.Count]);
            (tm.transform as RectTransform).anchoredPosition = Vector3.zero;
            (tm.transform as RectTransform).localScale = Vector3.one;            

            team.Add(mate);

            tm.GetComponent<UITeamPlayer>().SetPlayercontroller(mate);
            teamPlayerUI.Add(tm);
        }
    }

    public void RemoveTeamMate(PlayerController mate) {
        if (mate != null) {
            team.Remove(mate);
        }
    }

    public void InitilisePos()
    {
        maxPos = team.Count -1;
    }

    public void FocusAliveMate() {
        Cursor.lockState = CursorLockMode.None;

        InitilisePos();
        PlayerController alive = team.Find((mate) => { return !mate.dead; });
        if (player.isLocalPlayer && alive != null) {
            player.cam.GetComponent<CameraControl>().character = alive.transform;
            deathUI.gameObject.SetActive(false);
            observerUI.gameObject.SetActive(true);

            alive.OnDeath += () => {
                SetDeathUI(true);
                SetObserverUI(false);
            };
        }
    }

    public void NextMate()
    {
        if(currentPos != maxPos)
        {
            player.cam.GetComponent<CameraControl>().character = team[currentPos + 1].transform;
            currentPos += 1;
        }
        else
        {
            player.cam.GetComponent<CameraControl>().character = team[0].transform;
            currentPos = 0;
        }
    }

    public void PreviousMate()
    {
        if(currentPos != 0)
        {
            player.cam.GetComponent<CameraControl>().character = team[currentPos - 1].transform;
            currentPos -= 1;
        }
        else
        {
            player.cam.GetComponent<CameraControl>().character = team[maxPos].transform;
            currentPos = maxPos;
        }
        
    }

    // VR UI
    public void UpdateVRUI(VRPlayerManager pm) {
        goldBar.maxValue = pm.maxGold;
        goldBar.value = pm.totalGold;

        StringBuilder sb = new StringBuilder();

        sb.Append(pm.totalGold.ToString());        
        sb.Append("/");        
        sb.Append(pm.maxGold.ToString());        

        goldText.text = sb.ToString();
    } 

    // Menu UI
    public void ToggleMenu(bool display) {
        player.GetShootingController.canShoot = !display;
        menuUI.gameObject.SetActive(display);

		if (display == true)
			currentMenu = MenusEnum.main;
		else
			currentMenu = MenusEnum.none;

        Cursor.visible = display;
        if (player != null && !player.dead)
            Cursor.lockState = display ? CursorLockMode.None : CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.None;            
    }

    public void Win() {
        winUI.gameObject.SetActive(true);
        lossUI.gameObject.SetActive(false);
        teamUI.gameObject.SetActive(false);
        deathUI.gameObject.SetActive(false);
        observerUI.gameObject.SetActive(false);
        vrUI.gameObject.SetActive(false);
        playerUI.gameObject.SetActive(false);
        gamemodeUI.gameObject.SetActive(false);

        if (isVr)
            return;

        for (int i = 0; i < team.Count; i++) {
            GameObject tm = Instantiate(teamPlayerPrefab, teamResumeUIPosition[i]);

            tm.GetComponent<UITeamPlayer>().SetPlayercontroller(team[i]);
        }
    }

    public void Lose() {
        winUI.gameObject.SetActive(false);
        lossUI.gameObject.SetActive(true);
        teamUI.gameObject.SetActive(false);
        deathUI.gameObject.SetActive(false);
        observerUI.gameObject.SetActive(false);
        vrUI.gameObject.SetActive(false);
        playerUI.gameObject.SetActive(false);
        gamemodeUI.gameObject.SetActive(false);
}

	//Graphics UI
	public void DisplayGraphicsMenu(){
		menuUI.gameObject.SetActive (false);
		graphicsMenu.gameObject.SetActive (true);
		currentMenu = MenusEnum.graphics;
	}

	//Audio UI
	public void DisplayAudioMenu()
	{
		menuUI.gameObject.SetActive(false);
		audioMenu.gameObject.SetActive(true);
		currentMenu = MenusEnum.sounds;
	}

    public void Quit() {
        if (OnQuit != null) {
            OnQuit.Invoke();
        }

        Application.Quit();
    }

	//ALL UI
	public void ShowMenu(MenusEnum menu){
        player.GetShootingController.canShoot = false;
        
		switch (menu) {
		case MenusEnum.main:
			ToggleMenu (true);
			break;
		case MenusEnum.graphics:
			DisplayGraphicsMenu ();
			break;
		case MenusEnum.sounds:
			DisplayAudioMenu ();
			break;
        default:
            player.GetShootingController.canShoot = true;            
            break;
		}
	}

	public void HideMenu(MenusEnum menu){
        player.GetShootingController.canShoot = true;
        
		switch (menu) {
		case MenusEnum.main:
			ToggleMenu (false);
			break;
		case MenusEnum.graphics:
			graphicsMenu.gameObject.SetActive (false);
			break;
		case MenusEnum.sounds:
			audioMenu.gameObject.SetActive (false);
			break;
        default:
            player.GetShootingController.canShoot = false;            
            break;
		}
	}

	public void BackToMainMenu(){
		switch (currentMenu) {
		case MenusEnum.graphics:
			graphicsMenu.GetComponent<GraphicsMenu> ().BackToMainMenu ();
			break;
		case MenusEnum.sounds:
			audioMenu.GetComponent<AudioMenu> ().BackToMainMenu ();
			break;
		}
		ToggleMenu (true);
	}
}
