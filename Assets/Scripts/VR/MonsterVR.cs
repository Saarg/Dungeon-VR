using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

[RequireComponent(typeof(VRTK.VRTK_InteractableObject))]
public class MonsterVR : MonoBehaviour {

    private VRTK.VRTK_InteractableObject interactable;
    private NavMeshAgent navMeshAgent;
    private Animator animator;

    public void Awake()
    {
        interactable = GetComponent<VRTK.VRTK_InteractableObject>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    public void OnEnable()
    {
        interactable.InteractableObjectGrabbed += Interactable_InteractableObjectGrabbed;
        interactable.InteractableObjectUngrabbed += Interactable_InteractableObjectUngrabbed;
    }

    public void OnDisable()
    {
        interactable.InteractableObjectGrabbed -= Interactable_InteractableObjectGrabbed;
        interactable.InteractableObjectUngrabbed -= Interactable_InteractableObjectUngrabbed;
    }

    private void Interactable_InteractableObjectGrabbed(object sender, VRTK.InteractableObjectEventArgs e)
    {
        navMeshAgent.enabled = false;
        animator.SetBool("IsGrabbed", true);
    }

    private void Interactable_InteractableObjectUngrabbed(object sender, VRTK.InteractableObjectEventArgs e)
    {
        navMeshAgent.enabled = true;
        animator.SetBool("IsGrabbed", false);
    }
}
