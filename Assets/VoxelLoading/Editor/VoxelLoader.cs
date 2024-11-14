using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using System.Net.NetworkInformation;

[ExecuteInEditMode]
public class VoxelLoader : MonoBehaviour
{
    [MenuItem("Tools/Load Voxel File")]
    public static void Load()
    {
        var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        string path = "Assets\\VoxelLoading\\SceneLoadout.prefab";


        GameObject loadout = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        GameObject loadoutInstance = (GameObject)PrefabUtility.InstantiatePrefab(loadout, newScene);

        Transform roomAnchor = GameObject.Find("RoomAnchor").transform;



        foreach (Voxel voxel in VoxelReader.Read("Assets/VoxelLoading/untitled.vox"))
        {
            GameObject prefab = FindAdaptedPrefab(voxel);
            if (prefab != null)
            {
                GameObject prefabInstance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, newScene);
                prefabInstance.transform.position = new Vector3(voxel.x, voxel.z, voxel.y);
                prefabInstance.transform.SetParent(roomAnchor);
            }
        }
        print("yup");
        EditorSceneManager.SaveScene(newScene, "Assets/VoxelLoading/myNewScene.unity");
    }

    private static GameObject FindAdaptedPrefab(Voxel vox)
    {
        GameObject found = null;
        switch (vox.colorIndex)
        {
            case 248:
                found = AssetDatabase.LoadAssetAtPath<GameObject>("Assets\\Tiles\\Tile.prefab");
                break;
            case 144:
                found = AssetDatabase.LoadAssetAtPath<GameObject>("Assets\\Tiles\\Slope.prefab");
                break;
            case 217:
                found = AssetDatabase.LoadAssetAtPath<GameObject>("Assets\\Tiles\\SpawnTile.prefab");
                break;
            case 240:
                found = AssetDatabase.LoadAssetAtPath<GameObject>("Assets\\Tiles\\WeaponTileWatergun.prefab");
                break;
            case 255:
                found = AssetDatabase.LoadAssetAtPath<GameObject>("Assets\\Tiles\\WeaponTileRevolver.prefab");
                break;
            case 152:
                found = AssetDatabase.LoadAssetAtPath<GameObject>("Assets\\Tiles\\MedkitTile.prefab");
                break;
        }
        return found;
    }
}
