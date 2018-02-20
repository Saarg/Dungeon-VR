using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

[RequireComponent(typeof(TriggerUtility))]
public class TriggerUtility_UnityEvent : MonoBehaviour
{

    [Serializable]
    public class TriggerEvent : UnityEvent<object, TriggerUtility.TriggerEventArgs> { }

    public TriggerEvent OnEnter = new TriggerEvent();
    public TriggerEvent OnExit = new TriggerEvent();

    TriggerUtility triggerUtility;


    public void OnEnable()
    {
        triggerUtility = GetComponent<TriggerUtility>();

        triggerUtility.TriggerEntered += Enter;
        triggerUtility.TriggerExited += Exit;
    }

    public void OnDisable()
    {
        triggerUtility.TriggerEntered -= Enter;
        triggerUtility.TriggerExited -= Exit;
    }


    private void Enter(object o, TriggerUtility.TriggerEventArgs e)
    {
        OnEnter.Invoke(o, e);
    }

    private void Exit(object o, TriggerUtility.TriggerEventArgs e)
    {
        OnExit.Invoke(o, e);
    }
}
