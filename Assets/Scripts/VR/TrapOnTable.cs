using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapOnTable : ItemVR {

    public GameObject trapInHandPrefab;
    public DungeonTrap associatedTrap;

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

    public void OnTriggerPress(object o, VRTK.InteractableObjectEventArgs e)
    {
        if (HasEnoughMoney())
        {
            associatedTrap.price = price;
            e.interactingObject.GetComponent<TrapControllerManager>().AttachToHand(trapInHandPrefab, associatedTrap);
        }
        //return true;
    }
}

