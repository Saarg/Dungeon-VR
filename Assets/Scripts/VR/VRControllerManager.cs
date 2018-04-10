using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRControllerManager : MonoBehaviour {

    private VRTK.VRTK_ControllerEvents controllerEvents;
    private VRTK.VRTK_InteractGrab interactGrab;
    private VRTK.VRTK_ControllerReference controllerReference;

    public GameObject triggerSphereModel;

    private void Awake()
    {
        controllerEvents = GetComponent<VRTK.VRTK_ControllerEvents>();
        interactGrab = GetComponent<VRTK.VRTK_InteractGrab>();
    }

    private void OnEnable()
    {
        controllerEvents.TriggerPressed += ControllerEvents_TriggerPressed;
        controllerEvents.TriggerReleased += ControllerEvents_TriggerReleased;
    }

    private void OnDisable()
    {
        controllerEvents.TriggerPressed -= ControllerEvents_TriggerPressed;
        controllerEvents.TriggerReleased -= ControllerEvents_TriggerReleased;
    }

    private void ControllerEvents_TriggerPressed(object sender, VRTK.ControllerInteractionEventArgs e)
    {        
        triggerSphereModel.SetActive(false);
        if (controllerReference == null)
        {
            controllerReference = e.controllerReference;
        }
    }

    private void ControllerEvents_TriggerReleased(object sender, VRTK.ControllerInteractionEventArgs e)
    {
        triggerSphereModel.SetActive(true);
        interactGrab.grabButton = VRTK.VRTK_ControllerEvents.ButtonAlias.GripPress;
    }

    public void PlayHaptic(float strength, float duration, float interval)
    {
        VRTK.VRTK_ControllerHaptics.TriggerHapticPulse(controllerReference, strength, duration, interval);
    }
}
