using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

[Serializable]
public class ConnectionEvent : UnityEvent<NetworkConnection> {}

public class CustomNetworkManager: NetworkManager {

    public ConnectionEvent OnPlayerConnect;

    public override void OnServerConnect(NetworkConnection conn)
    {
        OnPlayerConnect.Invoke(conn);
    }
}