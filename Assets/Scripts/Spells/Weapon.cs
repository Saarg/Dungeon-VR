﻿using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    public enum WeaponTypeEnum
    {
        Base,
        LongRange,
        ShortRange,
    };

    [SerializeField]
    Sprite uiSprite;
    public Sprite UISprite { get { return uiSprite; } }

    [SerializeField]
    string weaponName;
    public string WeaponName { get { return weaponName; } }

    [SerializeField]
    WeaponTypeEnum weaponType;
    public WeaponTypeEnum WeaponType { get { return weaponType; } }

    [SerializeField]
    List<PlayerController.PlayerClassEnum> AllowedClass;

    [SerializeField]
    GameObject bullet;
    public GameObject Bullet { get { return bullet; } }

    [SerializeField]
    public bool shootingOffset;

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
    Transform spellOrigin;
    public Transform SpellOrigin { get { return spellOrigin; } }

    [SerializeField]
    AudioClip clip;

    public bool CanEquip(PlayerController.PlayerClassEnum playerClass)
    {
        return AllowedClass.Contains(playerClass);
    }

    public void PlaySound()
    {
        if(clip != null)
            AudioSource.PlayClipAtPoint(clip, transform.position);
    }
}
