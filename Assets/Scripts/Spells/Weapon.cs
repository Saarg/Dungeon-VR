using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    public enum WeaponTypeEnum
    {
        Base,
        LongRange,
        ShortRange,
    };

    public WeaponSpec spec;

    public Sprite UISprite { get { return spec.UISprite; } }
    public string WeaponName { get { return spec.WeaponName; } }
    public WeaponTypeEnum WeaponType { get { return spec.WeaponType; } }

    [SerializeField]
    List<PlayerController.PlayerClassEnum> AllowedClass { get { return spec.AllowedClass; } }

    public BulletSpec Bullet { get { return spec.Bullet; } }
    
    [SerializeField]
    GameObject model;

    [SerializeField]
    public bool shootingOffset { get { return spec.shootingOffset; } }


    public bool SpreadBullet { get { return spec.SpreadBullet; } }

    public float SpreadAngle { get { return spec.SpreadAngle; } }

    public int NumberOfBullet { get { return spec.NumberOfBullet; } }

    public float FiringInterval { get { return spec.FiringInterval; } }

    public int ManaCost { get { return spec.ManaCost; } }

    public bool UseMana { get { return spec.UseMana; } }

    public bool DrainMana { get { return spec.DrainMana; } }

    [SerializeField]
    Transform spellOrigin;
    public Transform SpellOrigin { get { return spellOrigin; } }

    [SerializeField]
    AudioClip clip { get { return spec.Clip; } }

    void Start()
    {
        if (spec.Model != null) {
            model = Instantiate(spec.Model, transform);
            model.transform.localPosition = Vector3.zero;
            model.transform.localRotation = Quaternion.identity;
        }
    }

    public bool CanEquip(PlayerController.PlayerClassEnum playerClass)
    {
        return AllowedClass.Contains(playerClass);
    }

    public void PlaySound()
    {
        if(clip != null)
            AudioSource.PlayClipAtPoint(clip, transform.position);
    }

    void OnDrawGizmos()
    {
        if (model == null) {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position, 0.3f);
        }
    }
}
