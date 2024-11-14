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
    int _exceptListAmount = 0;

    [SerializeField] GameObject _enemyPrefab;

    [NonSerialized] public NetworkVariable<ulong> _player1score = new(0);
    [NonSerialized] public NetworkVariable<ulong> _player2score = new(0);

    [SerializeField] float _gameDuration = 300;
    static public float _timeLeft = 0;

    [SerializeField] ulong _enemyCount = 2;

    private void Update()
    {
        _timeLeft = _gameDuration - Time.time;

        if(_timeLeft <= 0)
        {
            // TODO: Game Over
        }
    }

    private void Awake()
    {
        //NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += PlayerJoin;
        NetworkManager.Singleton.OnClientConnectedCallback += PlayerJoin;


        if(NetworkConnectionHandler._soloMode)
        {
            GameObject player = Instantiate(_playerPrefab);
            player.GetComponent<NetworkObject>().SpawnWithOwnership(_playerId.Value, true);
            player.GetComponent<CharacterController>()._playerId.Value = (int)_playerId.Value;
            ++_playerId.Value;
        }

        StartCoroutine(DelayEnemySpawn());
    }

    private IEnumerator DelayEnemySpawn()
    {
        yield return new WaitForSeconds(4);

        for(ulong i=0;  i < _enemyCount; i++)
        {
            GameObject found = FindATile(_exceptList);

            GameObject enemy = Instantiate(_enemyPrefab, found.transform.position + new Vector3(0, 1, 0), Quaternion.identity);
            enemy.GetComponent<NetworkObject>().Spawn(true);
        }
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
            exceptList[_exceptListAmount] = found;
            ++ _exceptListAmount;

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
    public void RespawnEnemy(GameObject enemy)
    {
        GameObject[] list = GameObject.FindGameObjectsWithTag("SpawnTile");
        GameObject found = list[UnityEngine.Random.Range(0, list.Length)];
        enemy.transform.position = found.transform.position + new Vector3(0, 1, 0);
        enemy.GetComponent<FollowPlayer>().Heal();
    }


    [Rpc(SendTo.Server)]
    public void IncrementScoreRpc(ulong id)
    {
        print(id + "score up");
        if (id == 0)
        {
            ++_player1score.Value;
        }
        else if (id == 0)
        {
            ++_player2score.Value;
        }
        else Debug.LogWarning("This is abnormal, you tried to increment score of a player that doesn't exist");
    }
}
