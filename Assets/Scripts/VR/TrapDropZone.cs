using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapDropZone : MonoBehaviour {

    public DungeonTrap placedTrap;

    public TrapControllerManager trapControllerManager;   
    public DungeonTrap preview;
    public string previewName;

    public TrapType allowedTrapType;


    /// <summary>
    /// On trigger enter, show a preview of what the trap will look like with you buy it.
    /// </summary>
    public void ShowPreview(TrapControllerManager _trapControllerManager)
    {
        if (trapControllerManager == null)
        {
            trapControllerManager = _trapControllerManager;

            Debug.Log(placedTrap);
            if (placedTrap == null && trapControllerManager.selectedTrap != null)
            {
                preview = Instantiate(trapControllerManager.selectedTrap);
                previewName = trapControllerManager.selectedTrap.name;

                if (transform.childCount > 0)
                {
                    preview.transform.position = transform.GetChild(0).transform.position;
                    preview.transform.rotation = transform.GetChild(0).transform.rotation;
                }
                else
                {
                    // if no attach point it get more complicated because of the scaling
                    preview.transform.position = transform.position + new Vector3(0, 2, 0);
                    preview.transform.rotation = transform.rotation;
                }

                trapControllerManager.GetComponent<VRTK.VRTK_ControllerEvents>().TriggerPressed += OnTriggerPressed;
            }
        }
    }

    public void RotatePreview(float angle)
    {
        if(preview != null)
        {
            preview.transform.Rotate(new Vector3(0, angle, 0));
        }
    }

    /// <summary>
    /// OnAttach trap from player player controller -> Check transform + ask server to add it.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnTriggerPressed(object sender, VRTK.ControllerInteractionEventArgs e)
    {
        //Debug.Log("TrapDropZone attach request");

        if (preview != null)
        {
            TrapSpawn s = new TrapSpawn();

            s.path = previewName;
            s.position = preview.transform.position;
            s.rotation = preview.transform.rotation;

            DestroyPreview();
            placedTrap = TrapSpawner.singleton.AddTrap(s);
            //VRPlayerManager.instance.Buy(placedTrap.);
            placedTrap.trap.TrapRemoved += OnTrapRemoved;
            trapControllerManager = null;
        }
    }

    /// <summary>
    /// Reset the trap drop zone when trap is grabbed then destroyed.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnTrapRemoved(Trap.TrapEventArgs e)
    {
        placedTrap.trap.TrapRemoved -= OnTrapRemoved;
        placedTrap = null;
        trapControllerManager = null;
    }

    public void DestroyPreview()
    {
        //Debug.Log("destroying");
        if(preview != null && trapControllerManager != null)
        {
            trapControllerManager.GetComponent<VRTK.VRTK_ControllerEvents>().TriggerPressed -= OnTriggerPressed;
            Destroy(preview.gameObject);
            preview = null;
            previewName = null;
            trapControllerManager = null;
        }
    }

}

public enum TrapType
{
    Ground,
    Wall,
    Ceiling
}
