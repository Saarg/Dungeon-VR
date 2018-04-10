using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropableItemVR : ItemVR {

    public GameObject trapInHandPrefab;
    public DungeonTrap associatedTrap;


    protected override void OnEnable()
    {
        base.OnEnable();
        GetComponent<VRTK.VRTK_InteractableObject>().InteractableObjectTouched += DropableItemVR_InteractableObjectTouched;
        GetComponent<VRTK.VRTK_InteractableObject>().InteractableObjectUsed += DropableItemVR_InteractableObjectUsed;
        
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        GetComponent<VRTK.VRTK_InteractableObject>().InteractableObjectTouched -= DropableItemVR_InteractableObjectTouched;
        GetComponent<VRTK.VRTK_InteractableObject>().InteractableObjectUsed -= DropableItemVR_InteractableObjectUsed;        
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        if (trapInHandPrefab == null)
        {
            Debug.LogError("Need to select a trap in hand prefab for this table trap.");
        }

    }

    private void DropableItemVR_InteractableObjectTouched(object sender, VRTK.InteractableObjectEventArgs e)
    {
        // Can't make it work and I dont know why ... -> Using Unity event the same way and it works
        //associatedTrap.GetComponent<Trap>().StartTrap();
    }

    private void DropableItemVR_InteractableObjectUsed(object sender, VRTK.InteractableObjectEventArgs e)
    {
        if (HasEnoughMoney())
        {
            associatedTrap.price = this.Price;
            e.interactingObject.GetComponent<TrapControllerManager>().AttachToHand(trapInHandPrefab, associatedTrap);
        }
    }
}

