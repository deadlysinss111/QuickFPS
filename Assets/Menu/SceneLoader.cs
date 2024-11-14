using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader
{
    public enum Scene
    {
        MainMenu,
        LoadingScene,
        PlayGround,
    }

    private static Scene _targetScene;

    public static void Load(Scene scene)
    {
        _targetScene = scene;

        NetworkManager.Singleton.SceneManager.LoadScene(scene.ToString(), LoadSceneMode.Single);
    }

    static void LoadCallback()
    {
        SceneManager.LoadScene(_targetScene.ToString());
    }
}
