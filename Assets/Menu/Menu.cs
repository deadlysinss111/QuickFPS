using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] Button _hostBtn;
    [SerializeField] Button _joinBtn;

    private void Awake()
    {
        _hostBtn.onClick.AddListener(() => {
            NetworkConnectionHandler.GetInstance().StartHost();
            SceneLoader.Load(SceneLoader.Scene.PlayGround);
            
        });
        _joinBtn.onClick.AddListener(() => {
            NetworkConnectionHandler.GetInstance().StartClient();
            //SceneLoader.Load(SceneLoader.Scene.FistLevel);
        });
    }
}
