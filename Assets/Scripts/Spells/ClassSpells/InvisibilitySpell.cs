using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibilitySpell : Spell
{
    [SerializeField] Color colstart;
    [SerializeField] Color colend;
    [SerializeField] float duration;

    Renderer[] _renderers;

    protected override void Start()
    {
        lastActivation = cooldown;

        _renderers = caster.GetComponentsInChildren<Renderer>();

        if (_renderers.Length > 0) {
            colstart = Color.white;
            colstart.a = 0.1f;
            colend = Color.white;
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

        StartCoroutine("InvisibilityPeriode");
    }

    protected void EndEffects()
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
    }

    IEnumerator InvisibilityPeriode()
    {
        yield return new WaitForSeconds(duration);
        EndEffects();
    }


}
