using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemVR : MonoBehaviour {

    public int price = 10;

    public Image image;

    private Color baseColor;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    protected bool HasEnoughMoney()
    {
        if (VRPlayerManager.instance.totalGold >= price)
        {
            StartCoroutine("CanvasFeedback_LowMoney", new Color(0, 155, 0, 0.5f)); // Green
            return true;
        }
        else
        {
            StartCoroutine("CanvasFeedback_LowMoney", new Color(155,0,0,0.5f)); // Red
            return false;
        }
    }

    public IEnumerator CanvasFeedback_LowMoney(Color color)
    {
        if(baseColor == null)
        {
            baseColor = image.color;
        }
        image.color = color;
        yield return new WaitForSeconds(1);
        image.color = baseColor;
    }
}
