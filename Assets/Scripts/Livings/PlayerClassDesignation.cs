using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerClassDesignation : NetworkBehaviour {

    public Transform weaponGrip;
    public WeaponSpec defaultWeapon;

    [Header("Life")]
    public float maxLife = 100;

    [Header("Mana")]
    public float maxMana = 100;

    [Header("Movement")]
    public float speed = 6f;
	public float walkSpeed = 3f;
	public float sprintSpeed = 8f;
    public float jumpHeight = 6f;

    [SerializeField]
    [Range(1.0f, 3.0f)] 
    private float jumpFactor = 2;
    public bool isGrounded;

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

    [Header("Collider")]
    public Vector3 center;
    public float radius = 1f;
    public float height = 2f;
}
