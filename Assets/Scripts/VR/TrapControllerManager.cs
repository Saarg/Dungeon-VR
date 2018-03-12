using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapControllerManager : MonoBehaviour {

    public Transform trapAttachPoint;

    [Header("Trap in hand prefab.")]
    private GameObject trapInHand;
    public DungeonTrap selectedTrap;

    private TrapDropZone trapDropZone;

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

    public void OnTrapDropZoneEnter(object o, TriggerUtility.TriggerEventArgs e)
    {
        trapDropZone = e.triggeredObject.GetComponent<TrapDropZone>();
        trapDropZone.ShowPreview(this);
        GetComponent<VRTK.VRTK_ControllerEvents>().TouchpadTouchStart += OnTouchpadTouchStart;

    }

    private void OnTouchpadTouchStart(object sender, VRTK.ControllerInteractionEventArgs e)
    {
        trapDropZone.RotatePreview(45);
    }

    public void OnTrapDropZoneExit(object o, TriggerUtility.TriggerEventArgs e)
    {
        trapDropZone = e.triggeredObject.GetComponent<TrapDropZone>();
        trapDropZone.DestroyPreview();
        GetComponent<VRTK.VRTK_ControllerEvents>().TouchpadTouchStart -= OnTouchpadTouchStart;
    }

}
