using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    public enum WeaponTypeEnum
    {
        Base,
        LongRange,
        ShortRange,
    };

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
    float firingInterval;
    public float FiringInterval { get { return firingInterval; } }

    [SerializeField]
    int manaCost;
    public int ManaCost { get { return manaCost; } }

    [SerializeField]
    bool useMana;
    public bool UseMana { get { return useMana; } }

    [SerializeField]
    Transform spellOrigin;
    public Transform SpellOrigin { get { return spellOrigin; } }

    public bool CanEquip(PlayerController.PlayerClassEnum playerClass)
    {
        return AllowedClass.Contains(playerClass);
    }
}
