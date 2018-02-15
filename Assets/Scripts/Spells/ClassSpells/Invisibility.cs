using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invisibility : Spell
{

    //[SerializeField] Texture invisibilityTexture = null;
    [SerializeField] Color colstart;
    [SerializeField] Color colend;
    [SerializeField] float Duration;

    protected override void Start()
    {
        lastActivation = cooldown;
        //basedTexture = caster.gameObject.transform.GetChild(2).gameObject.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().sharedMaterial;
        colstart = new Color(caster.gameObject.transform.GetChild(2).gameObject.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material.color.r,
            caster.gameObject.transform.GetChild(2).gameObject.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material.color.g,
            caster.gameObject.transform.GetChild(2).gameObject.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material.color.b,
            0.1f);
        colend = caster.gameObject.transform.GetChild(2).gameObject.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material.color;
    }

    protected override void Effects()
    {
        //Stupid bullshit engine
        caster.gameObject.transform.GetChild(2).gameObject.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material.SetFloat("_Mode", 3.0f);
        caster.gameObject.transform.GetChild(2).gameObject.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        caster.gameObject.transform.GetChild(2).gameObject.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        caster.gameObject.transform.GetChild(2).gameObject.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material.SetInt("_ZWrite", 0);
        caster.gameObject.transform.GetChild(2).gameObject.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material.DisableKeyword("_ALPHATEST_ON");
        caster.gameObject.transform.GetChild(2).gameObject.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material.EnableKeyword("_ALPHABLEND_ON");
        caster.gameObject.transform.GetChild(2).gameObject.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        caster.gameObject.transform.GetChild(2).gameObject.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material.renderQueue = 3000;

        caster.gameObject.transform.GetChild(2).gameObject.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material.SetFloat("_Mode", 3.0f);
        caster.gameObject.transform.GetChild(2).gameObject.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        caster.gameObject.transform.GetChild(2).gameObject.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        caster.gameObject.transform.GetChild(2).gameObject.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material.SetInt("_ZWrite", 0);
        caster.gameObject.transform.GetChild(2).gameObject.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material.DisableKeyword("_ALPHATEST_ON");
        caster.gameObject.transform.GetChild(2).gameObject.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material.EnableKeyword("_ALPHABLEND_ON");
        caster.gameObject.transform.GetChild(2).gameObject.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        caster.gameObject.transform.GetChild(2).gameObject.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material.renderQueue = 3000;

        caster.gameObject.transform.GetChild(2).gameObject.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material.color = colstart;
        caster.gameObject.transform.GetChild(2).gameObject.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material.color = colstart;
        StartCoroutine("InvisibilityPeriode");
    }

    protected void EndEffects()
    {
        //stupid bullshi engin 2
        caster.gameObject.transform.GetChild(2).gameObject.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material.SetFloat("_Mode", 0.0f);
        caster.gameObject.transform.GetChild(2).gameObject.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        caster.gameObject.transform.GetChild(2).gameObject.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        caster.gameObject.transform.GetChild(2).gameObject.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material.SetInt("_ZWrite", 1);
        caster.gameObject.transform.GetChild(2).gameObject.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material.DisableKeyword("_ALPHATEST_ON");
        caster.gameObject.transform.GetChild(2).gameObject.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material.EnableKeyword("_ALPHABLEND_ON");
        caster.gameObject.transform.GetChild(2).gameObject.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        caster.gameObject.transform.GetChild(2).gameObject.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material.renderQueue = -1;

        caster.gameObject.transform.GetChild(2).gameObject.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material.SetFloat("_Mode", 0.0f);
        caster.gameObject.transform.GetChild(2).gameObject.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        caster.gameObject.transform.GetChild(2).gameObject.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        caster.gameObject.transform.GetChild(2).gameObject.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material.SetInt("_ZWrite", 1);
        caster.gameObject.transform.GetChild(2).gameObject.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material.DisableKeyword("_ALPHATEST_ON");
        caster.gameObject.transform.GetChild(2).gameObject.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material.EnableKeyword("_ALPHABLEND_ON");
        caster.gameObject.transform.GetChild(2).gameObject.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        caster.gameObject.transform.GetChild(2).gameObject.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material.renderQueue = -1;

        caster.gameObject.transform.GetChild(2).gameObject.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material.color = colend;
        caster.gameObject.transform.GetChild(2).gameObject.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material.color = colend;
    }

    IEnumerator InvisibilityPeriode()
    {
        yield return new WaitForSeconds(Duration);
        EndEffects();
    }


}
