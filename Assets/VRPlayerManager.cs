﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRPlayerManager : MonoBehaviour {

    public static VRPlayerManager instance;

    GameUI gameUI;

    public int maxGold;
    public int totalGold = 10;

	public VRPlayerManager()
    {
        if(instance== null)
        {
            instance = this;
        }
        else
        {
            throw new System.Exception("Can't have multiple instance of VRPlayerManager");
        }
    }

    public bool Buy(int price)
    {
        if (price <= totalGold)
        {
            totalGold -= price;
            return true;
        }
        else
        {
            return false;
        }
    }

    void Start()
    {
        gameUI = GameObject.Find("GameUI").GetComponent<GameUI>();
    }

    void Update()
    {
        gameUI.UpdateVRUI(this);
    }
}
