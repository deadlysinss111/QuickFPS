using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkConnectionHandler : NetworkBehaviour
{
    static public bool _soloMode = false;

    public static NetworkConnectionHandler GetInstance()
    {
        return GameObject.Find("ConnectionHandler").GetComponent<NetworkConnectionHandler>();
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_NetworkApprovalCallback;
        NetworkManager.Singleton.StartHost();
    }

    public void StartSolo()
    {
        _soloMode = true;
        NetworkManager.NetworkConfig.ConnectionApproval = false;
        NetworkManager.Singleton.StartHost();
    }

    private void NetworkManager_NetworkApprovalCallback(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        response.Approved = true;
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }
}
