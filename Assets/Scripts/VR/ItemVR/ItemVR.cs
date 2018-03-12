using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(VRTK.VRTK_InteractableObject))]
public class ItemVR : MonoBehaviour {

    private ItemVR_UI itemVR_UI;

    public string Name = "None";
    public int Price = 10;

    protected virtual void OnEnable()
    {
        GetComponent<VRTK.VRTK_InteractableObject>().InteractableObjectTouched += ItemVR_InteractableObjectTouched;
        GetComponent<VRTK.VRTK_InteractableObject>().InteractableObjectUntouched += ItemVR_InteractableObjectUntouched;
    }

    protected virtual void OnDisable()
    {
        GetComponent<VRTK.VRTK_InteractableObject>().InteractableObjectTouched -= ItemVR_InteractableObjectTouched;
        GetComponent<VRTK.VRTK_InteractableObject>().InteractableObjectUntouched -= ItemVR_InteractableObjectUntouched;
    }

    // Use this for initialization
    protected virtual void Start () {
        try
        {
            this.itemVR_UI = GetComponentInChildren<ItemVR_UI>();
        }
        catch
        {
            new System.Exception("No 'ItemVR_UI' found.");
        }
    }

    protected bool HasEnoughMoney()
    {
        if (VRPlayerManager.instance.totalGold >= Price)
        {
            this.itemVR_UI.ShowEnoughMoneyFeedback();          
            return true;
        }
        else
        {
            this.itemVR_UI.ShowLowMoneyFeedback();           
            return false;
        }
    }

    private void ItemVR_InteractableObjectTouched(object sender, VRTK.InteractableObjectEventArgs e)
    {
        this.itemVR_UI.IsShown = true;
    }

    private void ItemVR_InteractableObjectUntouched(object sender, VRTK.InteractableObjectEventArgs e)
    {
        this.itemVR_UI.IsShown = false;
    }


}
