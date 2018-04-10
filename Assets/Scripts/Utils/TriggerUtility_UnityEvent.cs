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
    public TriggerEvent OnStay = new TriggerEvent();
    public TriggerEvent OnExit = new TriggerEvent();

    TriggerUtility triggerUtility;


    public void OnEnable()
    {
        triggerUtility = GetComponent<TriggerUtility>();

        triggerUtility.TriggerEnter += Enter;
        triggerUtility.TriggerStay += Stay;
        triggerUtility.TriggerExit += Exit;
    }

    public void OnDisable()
    {
        triggerUtility.TriggerEnter -= Enter;
        triggerUtility.TriggerStay += Stay;
        triggerUtility.TriggerExit -= Exit;
    }


    private void Enter(object o, TriggerUtility.TriggerEventArgs e)
    {
        OnEnter.Invoke(o, e);
    }

    private void Stay(object o, TriggerUtility.TriggerEventArgs e)
    {
        OnStay.Invoke(o, e);
    }

    private void Exit(object o, TriggerUtility.TriggerEventArgs e)
    {
        OnExit.Invoke(o, e);
    }
}
