using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class TriggerUtility : MonoBehaviour {

    public struct TriggerEventArgs
    {
        public GameObject triggeredObject;
    }

    public delegate void TriggerEventHandler(object sender, TriggerEventArgs e);

    public event TriggerEventHandler TriggerEntered;
    public event TriggerEventHandler TriggerExited;

    /// <summary>
    /// The types of element that can be checked against.
    /// </summary>
    public enum CheckTypes
    {
        Tag = 1,
        Script = 2,
    }

    [Tooltip("The element type on the game object to check against.")]
    public CheckTypes checkType = CheckTypes.Tag;
    [Tooltip("A list of identifiers to check for against the given check type (either tag or script).")]
    public string identifiers = "";


    private void OnTriggerEnter(Collider other)
    {        
        if (checkType == CheckTypes.Script && identifiers != "" && other.GetComponent( Type.GetType( identifiers))  != null 
            || checkType == CheckTypes.Tag && other.tag == identifiers)
        {
            //emit event
            TriggerEventArgs args = new TriggerEventArgs();
            args.triggeredObject = other.gameObject;
            TriggerEntered(this, args);
        }
            
    }

    private void OnTriggerExit(Collider other)
    {
        if (checkType == CheckTypes.Script && other.GetComponent(Type.GetType(identifiers)) != null
            || checkType == CheckTypes.Tag && other.tag == identifiers)
        {
            //emit event
            TriggerEventArgs args = new TriggerEventArgs();
            args.triggeredObject = other.gameObject;
            TriggerExited(this, args);
        }

    }
}
