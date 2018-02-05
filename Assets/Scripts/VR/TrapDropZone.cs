using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapDropZone : MonoBehaviour {

    public Trap placedTrap;
    public TrapType allowedTrapType;

    public void AttachTrapToDropZone(object o, VRTK.InteractableObjectEventArgs e)
    {      
        if (placedTrap == null && e.interactingObject.GetComponent<TrapControllerManager>().selectedTrap.trapType == allowedTrapType)
        {
            // Need to add a way to rotate the trap (Touchpad should be find for that)
            placedTrap = Instantiate(e.interactingObject.GetComponent<TrapControllerManager>().selectedTrap, transform.position, transform.rotation);
            placedTrap.transform.parent = transform;
        }
        else
        {
            // error feedback for impossible drop
        }
        
    }

    public void RemoveTrapFromDropZone(object o, VRTK.InteractableObjectEventArgs e)
    {
        Destroy(placedTrap.gameObject);
    }
}

public enum TrapType
{
    Ground,
    Wall,
    Ceiling
}
