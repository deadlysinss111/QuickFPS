using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkConnectionHandler : NetworkBehaviour
{
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

    private void NetworkManager_NetworkApprovalCallback(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        response.Approved = true;
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }
}
