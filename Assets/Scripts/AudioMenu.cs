using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioMenu : MonoBehaviour {

    [Header("Other menus")]
    [SerializeField]
    private GameObject mainMenu;
    [SerializeField] private Button backBtn;

    [Header("Sliders")]
    [SerializeField] private Slider Master;
    [SerializeField] private Slider Ambiant;
    [SerializeField] private Slider Weapons;
    [SerializeField] private Slider Living;


    void Start() { }
    void Update() { }


    public void BackToMainMenu()
    {
        this.gameObject.SetActive(false);
        mainMenu.SetActive(true);
    }
}
