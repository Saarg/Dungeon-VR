using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(VRTK.VRTK_InteractableObject))]
public class VR_Spell : MonoBehaviour {

    public SpellSpawner SpellSpawner { get; set; }
    public AreaSpell AreaSpellPrefab;


    public void OnEnable()
    {
        GetComponent<VRTK.VRTK_InteractableObject>().InteractableObjectGrabbed += VR_Spell_InteractableObjectGrabbed;
        GetComponent<VRTK.VRTK_InteractableObject>().InteractableObjectUngrabbed += VR_Spell_InteractableObjectUngrabbed;
    }

    public void OnDisable()
    {
        GetComponent<VRTK.VRTK_InteractableObject>().InteractableObjectGrabbed -= VR_Spell_InteractableObjectGrabbed;
        GetComponent<VRTK.VRTK_InteractableObject>().InteractableObjectUngrabbed -= VR_Spell_InteractableObjectUngrabbed;
    }

    private void VR_Spell_InteractableObjectGrabbed(object sender, VRTK.InteractableObjectEventArgs e)
    {
        this.SpellSpawner.CurrentSpell = null;
        this.GetComponent<SphereCollider>().radius = 2f; //  for a more accurate collision
    }

    private void VR_Spell_InteractableObjectUngrabbed(object sender, VRTK.InteractableObjectEventArgs e)
    {
        VRSpellManager.ThrowSpell(gameObject);
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if(other.gameObject.layer != LayerMask.NameToLayer("VRController"))
        {           
            Explode();
        }
    }

    private void Explode()
    {
        // do all the effects
        // do effect on player 
        Instantiate(AreaSpellPrefab, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
}
