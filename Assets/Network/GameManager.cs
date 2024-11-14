using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor.Rendering.Universal;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    [SerializeField] GameObject _playerPrefab;
    NetworkVariable<ulong> _playerId = new(0);
    GameObject[] _exceptList = new GameObject[100];

    private void Awake()
    {
        //NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += PlayerJoin;
        NetworkManager.Singleton.OnClientConnectedCallback += PlayerJoin;


        // CODE TO COMMENT TO HAVE SERVER
        //GameObject player = Instantiate(_playerPrefab);
        //player.GetComponent<NetworkObject>().SpawnWithOwnership(_playerId.Value, true);
        //player.GetComponent<CharacterController>()._playerId.Value = (int)_playerId.Value;
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
        GameObject found = FindATile(_exceptList);


        GameObject player = Instantiate(_playerPrefab, found.transform.position + new Vector3(0, 1, 0), Quaternion.identity);
        //player.GetComponent<NetworkObject>().SpawnAsPlayerObject(id, true);
        //player.GetComponent<NetworkObject>().ChangeOwnership(id);
        player.GetComponent<NetworkObject>().SpawnWithOwnership(_playerId.Value, true);
        player.GetComponent<CharacterController>()._playerId.Value = (int)_playerId.Value;
        ++_playerId.Value;
    }

    private GameObject FindATile(GameObject[] exceptList)
    {
        GameObject[] list = GameObject.FindGameObjectsWithTag("SpawnTile");
        GameObject found = null;
        bool keepGoing;
        do
        {
            keepGoing = false;
            found = list[UnityEngine.Random.Range(0, list.Length)];
            foreach (GameObject go in exceptList)
            {
                if (found == go)
                {
                    keepGoing = true;
                }
            };

        } while (keepGoing);

        return found;
    }

    public void RespawnPlayer(GameObject player)
    {
        GameObject[] list = GameObject.FindGameObjectsWithTag("SpawnTile");
        GameObject found = list[UnityEngine.Random.Range(0, list.Length)];
        player.transform.position = found.transform.position + new Vector3(0, 1, 0);
        player.GetComponent<CharacterController>().HealSelf();
    }
}
