using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(VRTK.VRTK_InteractableObject))]
public class VR_Spell : MonoBehaviour {

    public SpellSpawner SpellSpawner { get; set; }
    public AreaSpell AreaSpellPrefab;

    public VRTK.VRTK_InteractGrab interactGrab;
    private VRTK.VRTK_InteractableObject interactableObject;

    public void Awake()
    {
        interactableObject = GetComponent<VRTK.VRTK_InteractableObject>();
    }

    public void OnEnable()
    {
        interactableObject.InteractableObjectGrabbed += VR_Spell_InteractableObjectGrabbed;
        interactableObject.InteractableObjectUngrabbed += VR_Spell_InteractableObjectUngrabbed;
    }

    public void OnDisable()
    {
        interactableObject.InteractableObjectGrabbed -= VR_Spell_InteractableObjectGrabbed;
        interactableObject.InteractableObjectUngrabbed -= VR_Spell_InteractableObjectUngrabbed;
    }

    private void VR_Spell_InteractableObjectGrabbed(object sender, VRTK.InteractableObjectEventArgs e)
    {
        interactGrab = e.interactingObject.GetComponentInParent<VRTK.VRTK_InteractGrab>();
        this.SpellSpawner.CurrentSpell = null;
        this.GetComponent<SphereCollider>().radius = 2f; //  for a more accurate collision

        SpellSpawner.spawners.ForEach(spawner => {
            if (spawner != this.SpellSpawner) {
                spawner.SmallCooldown();
            }
        });
    }

    private void VR_Spell_InteractableObjectUngrabbed(object sender, VRTK.InteractableObjectEventArgs e)
    {
        interactGrab = null;
        interactableObject.isGrabbable = false;
        VRSpellManager.ThrowSpell(gameObject);
    }

    public void OnTriggerEnter(Collider other)
    {
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
