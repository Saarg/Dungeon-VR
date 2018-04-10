using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapControllerManager : MonoBehaviour {

    private VRTK.VRTK_ControllerEvents controllerEvents;

    public Transform trapAttachPoint;
    private GameObject trapInHand;
    public DungeonTrap selectedTrap;
    private TrapDropZone trapDropZone;


    private void Awake()
    {
        controllerEvents = GetComponent<VRTK.VRTK_ControllerEvents>();
    }

    private void OnEnable()
    {   
        controllerEvents.GripPressed += ControllerEvents_GripPressed;
    }

    private void OnDisable()
    {       
        controllerEvents.GripPressed -= ControllerEvents_GripPressed;
    }

    private void ControllerEvents_GripPressed(object sender, VRTK.ControllerInteractionEventArgs e)
    {
        ReleaseFromHand();
    }

    private void ControllerEvents_ButtonTwoPressed(object sender, VRTK.ControllerInteractionEventArgs e)
    {
        trapDropZone.RotatePreview(45);
    }

    public void AttachToHand(GameObject trapInHandToInstantiate, DungeonTrap trap)
    {
        if(trapInHand != null)
        {
            ReleaseFromHand();
        }
        trapInHand = Instantiate(trapInHandToInstantiate, trapAttachPoint.transform.position, trapAttachPoint.transform.rotation);
        trapInHand.transform.parent = trapAttachPoint.transform;
        selectedTrap = trap;
    }

    public void ReleaseFromHand()
    {
        if (trapInHand != null)
        {
            Destroy(trapInHand.gameObject);
            selectedTrap = null;
        }
    }

    public void OnTrapDropZoneStay(object o, TriggerUtility.TriggerEventArgs e)
    {
        if (trapDropZone == null)
        {
            trapDropZone = e.triggeredObject.GetComponent<TrapDropZone>();
            trapDropZone.ShowPreview(this);
            controllerEvents.ButtonTwoPressed += ControllerEvents_ButtonTwoPressed;
        }
    }

    public void OnTrapDropZoneExit(object o, TriggerUtility.TriggerEventArgs e)
    {
        if (trapDropZone != null)
        {
            trapDropZone.DestroyPreview();
            trapDropZone = null;
            controllerEvents.ButtonTwoPressed -= ControllerEvents_ButtonTwoPressed;
        }
    }

}
