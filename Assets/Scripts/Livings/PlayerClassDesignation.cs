using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerClassDesignation : MonoBehaviour {

    public GameObject Weapongrip;
    public GameObject DefaultWeapon;

    [Header("Life")]
    public float maxLife = 100;

    [SyncVar(hook = "UpdateLife")]
    public float curLife = 100f;

    [Header("Mana")]
    public float maxMana = 100;

    [SyncVar(hook = "UpdateMana")]
    public float curMana = 100f;

    [Header("Movement")]
    public float speed = 1f;
    public float JumpSpeed = 1.0f;

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

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
