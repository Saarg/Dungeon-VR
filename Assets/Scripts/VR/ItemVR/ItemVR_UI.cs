using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemVR_UI : MonoBehaviour {

    private GameObject headsetToTrack;

    // UI
    public Text nameText;
    public Text priceText;
    public Image backgroundImage;
    private Color backgroundImageBaseColor;


    private bool isShown = false;
    public bool IsShown {
        get
        {
            return isShown;
        }
        set
        {
            isShown = value;
            if(isShown == true)
            {
                this.GetComponent<Canvas>().enabled = true;
                this.headsetToTrack = GameObject.Find("Camera (eye)");               
            }
            else
            {
                this.GetComponent<Canvas>().enabled = false;
            }
            
        }
    }

    // Use this for initialization
    void Start () {

        ItemVR itemVR = GetComponentInParent<ItemVR>();
        this.nameText.text = itemVR.Name.ToString();
        this.priceText.text = itemVR.Price.ToString();            
    }
	
	// Update is called once per frame
	void Update () {
        if (IsShown && headsetToTrack != null)
        {
            transform.LookAt(headsetToTrack.transform.position);
        }
	}

    public void ShowEnoughMoneyFeedback()
    {
        StartCoroutine("CanvasFeedback_LowMoney", new Color(0, 155, 0, 0.5f)); // Green
    }

    public void ShowLowMoneyFeedback()
    {
        StartCoroutine("CanvasFeedback_LowMoney", new Color(155, 0, 0, 0.5f)); // Red
    }

    public IEnumerator CanvasFeedback_LowMoney(Color color)
    {
        if (this.backgroundImageBaseColor == null)
        {
            this.backgroundImageBaseColor = this.backgroundImage.color;
        }
        this.backgroundImage.color = color;
        yield return new WaitForSeconds(1);
        this.backgroundImage.color = this.backgroundImageBaseColor;
    }
}
