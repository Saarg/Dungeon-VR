using UnityEngine;
using System.Collections;
using UnityEngine.UI;
 
 public class Fade : MonoBehaviour
{

    public GameObject fadeTexture;
    [Range(0.1f, 1f)]
    public float fadespeed;

    [SerializeField] float alpha = 1f;
    private float fadeDir = -1f;
    private Color col;
    private bool fade = true;
    // Use this for initialization
    void Start()
    {
        col = fadeTexture.GetComponent<Image>().color;
        fadespeed = 0.2f;
    }


    void OnGUI()
    {
        //Fade in
        if(fade)
        {
            alpha += fadeDir * fadespeed * Time.deltaTime;
            alpha = Mathf.Clamp01(alpha);

            Color newColor = GUI.color;
            newColor.a = alpha;

            fadeTexture.GetComponent<Image>().color = new Color(col.r, col.g, col.b, alpha);
            if (alpha <= 0)
            {
                fade = false;
            }
        }
        //Fade out
        if (!fade)
        {
            alpha -= fadeDir * fadespeed * Time.deltaTime;
            alpha = Mathf.Clamp01(alpha);

            Color newColor = GUI.color;
            newColor.a = alpha;

            fadeTexture.GetComponent<Image>().color = new Color(col.r, col.g, col.b, alpha);
            if(alpha >= 1)
            {
                fade = true;
            }
        }

    }



}