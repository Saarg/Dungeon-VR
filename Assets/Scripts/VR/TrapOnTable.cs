using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapOnTable : MonoBehaviour {

    public GameObject trapInHandPrefab;
    public Trap associatedTrap;

    // Use this for initialization
    void Start() {
        if (trapInHandPrefab == null)
        {
            Debug.LogError("Need to select a trap in hand prefab for this table trap.");
        }
    }

    // Update is called once per frame
    void Update() {

    }

    public void AttachToHand(object o, VRTK.InteractableObjectEventArgs e)
    {
        e.interactingObject.GetComponent<TrapControllerManager>().AttachToHand(trapInHandPrefab, associatedTrap);

    }
}

