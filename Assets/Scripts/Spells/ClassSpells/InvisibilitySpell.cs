using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibilitySpell : Spell
{
    PlayerController playerController;

    [SerializeField] Color colstart;
    [SerializeField] Color colend;
    [SerializeField] float duration;

    List<Renderer> _renderers = new List<Renderer>();

    protected override void Start()
    {
        base.Start();
        
        _renderers.AddRange(transform.parent.GetComponentsInChildren<Renderer>());

        colstart = Color.white;
        colstart.a = 0.1f;
        colend = Color.white;

        playerController = GetComponentInParent<PlayerController>();
        if (playerController != null) {
            playerController.inventory.onPickWeapon += (Weapon w) => {
                _renderers.Add(w.Model.GetComponent<Renderer>());
            };

            playerController.inventory.onDropWeapon += (Weapon w) => {
                _renderers.Remove(w.Model.GetComponent<Renderer>());
            };
        } else {
            Debug.LogWarning("Invisibility spell could not find assigned playercontroller");
        }
    }

    protected override void Effects()
    {
        foreach (Renderer r in _renderers) {
            foreach (Material mat in r.materials) {
                mat.SetFloat("_Mode", 3.0f);
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = 3000;
                
                mat.color = colstart;
                mat.color = colstart;
            }
        }

        caster.ApplyMoveStatus (effectMovement);

        StartCoroutine("InvisibilityPeriode");
    }

    protected override void EndEffects()
    {
        foreach (Renderer r in _renderers) {
            foreach (Material mat in r.materials) {
                mat.SetFloat("_Mode", 0.0f);
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                mat.SetInt("_ZWrite", 1);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = -1;

                mat.color = colend;
                mat.color = colend;
            }
        }

		caster.ApplyMoveStatus (MoveStatus.Free);
    }

    IEnumerator InvisibilityPeriode()
    {
        yield return new WaitForSeconds(duration);
        EndEffects();
    }


}
