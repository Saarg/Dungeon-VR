using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapControllerManager : MonoBehaviour {

    public Transform trapAttachPoint;

    [Header("Trap in hand prefab.")]
    private GameObject trapInHand;
    public Trap selectedTrap;

    public void AttachToHand(GameObject trapInHandToInstantiate, Trap trap)
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
}
