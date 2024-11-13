using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.Rendering.Universal;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    [SerializeField] GameObject _playerPrefab;
    NetworkVariable<ulong> _playerId = new(0);

    private void Awake()
    {
        //NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += PlayerJoin;
        NetworkManager.Singleton.OnClientConnectedCallback += PlayerJoin;


        // CODE TO COMMENT TO HAVE SERVER
        //GameObject player = Instantiate(_playerPrefab);
        //player.GetComponent<NetworkObject>().SpawnWithOwnership(_playerId.Value, true);
        //player.GetComponent<CharacterController>()._playerId = (int)_playerId.Value;
        //++_playerId.Value;
    }

    private void PlayerJoin(ulong obj)
    {
        SpawnPlayerRpc(_playerId.Value);
    }

    //private void PlayerJoin(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    //{
    //    GameObject player = Instantiate(_playerPrefab);
    //    player.GetComponent<NetworkObject>().SpawnAsPlayerObject(_playerID, true);
    //    ++_playerID;
    //}

    [Rpc(SendTo.Server)]
    private void SpawnPlayerRpc(ulong id)
    {
        GameObject player = Instantiate(_playerPrefab);
        //player.GetComponent<NetworkObject>().SpawnAsPlayerObject(id, true);
        //player.GetComponent<NetworkObject>().ChangeOwnership(id);
        player.GetComponent<NetworkObject>().SpawnWithOwnership(_playerId.Value, true);
        player.GetComponent<CharacterController>()._playerId.Value = (int)_playerId.Value;
        ++_playerId.Value;
    }
}
