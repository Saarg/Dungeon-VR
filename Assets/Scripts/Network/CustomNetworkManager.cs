using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class CustomNetworkManager: NetworkManager {
    public delegate void OnPlayerConnect(NetworkConnection conn);
    public OnPlayerConnect playerConnectDelegate;

    public override void OnServerConnect(NetworkConnection conn)
    {
        if (playerConnectDelegate != null)
				playerConnectDelegate(conn);
    }
}