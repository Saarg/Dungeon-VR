using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerClassDesignation : MonoBehaviour {

    public GameObject Weapongrip;
    public GameObject DefaultWeapon;

    [Header("Life")]
    public float maxLife = 100;

    [Header("Mana")]
    public float maxMana = 100;

    [Header("Movement")]
    public float speed = 6f;
    public float JumpSpeed = 6f;

    [Header("Weakness/Strength")]
    [Range(0f, 2f)]
    public float fire = 1f;
    [Range(0f, 2f)]
    public float ice = 1f;
    [Range(0f, 2f)]
    public float lightning = 1f;
    [Range(0f, 2f)]
    public float poison = 1f;
    [Range(0f, 2f)]
    public float physical = 1f;
}
