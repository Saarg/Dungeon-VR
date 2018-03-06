using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "WeaponSpec", menuName = "Weapons/WeaponSpec", order = 1)]
public class WeaponSpec : ScriptableObject {

	[SerializeField]
    GameObject weaponPrefab;
	public GameObject WeaponPrefab { get { return weaponPrefab; } }

	[SerializeField]
    Sprite uiSprite;
    public Sprite UISprite { get { return uiSprite; } }

    [SerializeField]
    string weaponName;
    public string WeaponName { get { return weaponName; } }

    [SerializeField]
    Weapon.WeaponTypeEnum weaponType;
    public Weapon.WeaponTypeEnum WeaponType { get { return weaponType; } }

    [SerializeField]
    List<PlayerController.PlayerClassEnum> allowedClass;
	public List<PlayerController.PlayerClassEnum> AllowedClass { get { return allowedClass; } }

    [SerializeField]
    GameObject bullet;
    public GameObject Bullet { get { return bullet; } }

    [SerializeField]
    GameObject model;
    public GameObject Model { get { return model; } }

    [SerializeField]
    public bool shootingOffset;
    public bool ShootingOffset { get { return shootingOffset; } }	

    [SerializeField]
    bool spreadBullet;
    public bool SpreadBullet { get { return spreadBullet; } }

    [SerializeField]
    float spreadAngle;
    public float SpreadAngle { get { return spreadAngle; } }

    [SerializeField]
    int numberOfBullet;
    public int NumberOfBullet { get { return numberOfBullet; } }

    [SerializeField]
    float firingInterval;
    public float FiringInterval { get { return firingInterval; } }

    [SerializeField]
    int manaCost;
    public int ManaCost { get { return manaCost; } }

    [SerializeField]
    bool useMana;
    public bool UseMana { get { return useMana; } }

    [SerializeField]
    bool drainMana;
    public bool DrainMana { get { return drainMana; } }

	[SerializeField]
    AudioClip clip;
	public AudioClip Clip { get { return clip; } }
}
