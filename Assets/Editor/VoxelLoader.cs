//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;
//using UnityEditor.SceneManagement;
//using System.IO;

//[ExecuteInEditMode]
//public class VoxelLoader : MonoBehaviour
//{
//    [MenuItem("Tools/Load Voxel File")]
//    public static void Load()
//    {
//        var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

//        string path = "Assets\\LoadVoxTests\\SceneLoadout";
//        string[] files = Directory.GetFiles(path);

        
//        foreach (string file in files)
//        {
//            GameObject loadout = AssetDatabase.LoadAssetAtPath<GameObject>(file);
//            GameObject loadoutInstance = (GameObject)PrefabUtility.InstantiatePrefab(loadout, newScene);
//        }
//        Transform roomAnchor = GameObject.Find("RoomAnchor").transform;

//        GameObject.Find("Canvas").GetComponent<Canvas>().worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
//        GameObject.Find("Main Camera").GetComponent<CameraController>()._target = GameObject.Find("CameraAnchor").transform;


//        foreach (Voxel voxel in VoxelReader.Read("Assets/LoadVoxTests/untitled.vox"))
//        {
//            GameObject prefab = FindAdaptedPrefab(voxel);
//            if(prefab != null)
//            {
//                GameObject prefabInstance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, newScene);
//                prefabInstance.transform.position = new Vector3(voxel.x, voxel.z, voxel.y)*2;
//                prefabInstance.transform.SetParent(roomAnchor);
//            }
//        }
//        print("yup");
//        EditorSceneManager.SaveScene(newScene, "Assets/LoadVoxTests/myNewScene.unity");
//    }

//    private static GameObject FindAdaptedPrefab(Voxel vox)
//    {
//        GameObject found = null;
//        switch(vox.colorIndex)
//        {
//            case 250:
//                //found = AssetDatabase.LoadAssetAtPath<GameObject>("Assets\\Art\\test import\\tiles\\tile block\\rouille\\Prefab_Tile_block_rouille.prefab");
//                switch(UnityEngine.Random.Range(0, 5))
//                {
//                    case 0:
//                        switch (UnityEngine.Random.Range(0, 5))
//                        {
//                            case 0:
//                                found = AssetDatabase.LoadAssetAtPath<GameObject>("Assets\\GameObjects\\Rooms & Tiles\\Resources\\Radioactive Zone\\Topology\\Prefab_tile_block_rouille.prefab");
//                                break;
//                            case 1:
//                                found = AssetDatabase.LoadAssetAtPath<GameObject>("Assets\\GameObjects\\Rooms & Tiles\\Resources\\Radioactive Zone\\Topology\\Prefab_tile_block_rouille 1.prefab");
//                                break;
//                            case 2:
//                                found = AssetDatabase.LoadAssetAtPath<GameObject>("Assets\\GameObjects\\Rooms & Tiles\\Resources\\Radioactive Zone\\Topology\\Prefab_tile_block_rouille 2.prefab");
//                                break;
//                            case 3:
//                                found = AssetDatabase.LoadAssetAtPath<GameObject>("Assets\\GameObjects\\Rooms & Tiles\\Resources\\Radioactive Zone\\Topology\\Prefab_tile_block_rouille 3.prefab");
//                                break;
//                            case 4:
//                                found = AssetDatabase.LoadAssetAtPath<GameObject>("Assets\\GameObjects\\Rooms & Tiles\\Resources\\Radioactive Zone\\Topology\\Prefab_tile_block_rouille 4.prefab");
//                                break;
//                        }
//                        break;
//                    case 1:
//                        switch (UnityEngine.Random.Range(0, 5))
//                        {
//                            case 0:
//                                found = AssetDatabase.LoadAssetAtPath<GameObject>("Assets\\GameObjects\\Rooms & Tiles\\Resources\\Radioactive Zone\\Topology\\Prefab_tile_block_rouille.prefab");
//                                break;
//                            case 1:
//                                found = AssetDatabase.LoadAssetAtPath<GameObject>("Assets\\GameObjects\\Rooms & Tiles\\Resources\\Radioactive Zone\\Topology\\Prefab_tile_block_rouille 1.prefab");
//                                break;
//                            case 2:
//                                found = AssetDatabase.LoadAssetAtPath<GameObject>("Assets\\GameObjects\\Rooms & Tiles\\Resources\\Radioactive Zone\\Topology\\Prefab_tile_block_rouille 2.prefab");
//                                break;
//                            case 3:
//                                found = AssetDatabase.LoadAssetAtPath<GameObject>("Assets\\GameObjects\\Rooms & Tiles\\Resources\\Radioactive Zone\\Topology\\Prefab_tile_block_rouille 3.prefab");
//                                break;
//                            case 4:
//                                found = AssetDatabase.LoadAssetAtPath<GameObject>("Assets\\GameObjects\\Rooms & Tiles\\Resources\\Radioactive Zone\\Topology\\Prefab_tile_block_rouille 4.prefab");
//                                break;
//                        }
//                        break;
//                    case 2:
//                        found = AssetDatabase.LoadAssetAtPath<GameObject>("Assets\\GameObjects\\Rooms & Tiles\\Resources\\Radioactive Zone\\Topology\\Prefab_tile_block_rouille 2.prefab");
//                        break;
//                    case 3:
//                        found = AssetDatabase.LoadAssetAtPath<GameObject>("Assets\\GameObjects\\Rooms & Tiles\\Resources\\Radioactive Zone\\Topology\\Prefab_tile_block_rouille 3.prefab");
//                        break;
//                    case 4:
//                        found = AssetDatabase.LoadAssetAtPath<GameObject>("Assets\\GameObjects\\Rooms & Tiles\\Resources\\Radioactive Zone\\Topology\\Prefab_tile_block_rouille 4.prefab");
//                        break;
//                }
//                break;
//            case 240:
//                found = AssetDatabase.LoadAssetAtPath<GameObject>("Assets\\GameObjects\\Rooms & Tiles\\Resources\\Radioactive Zone\\Interactibles\\Prefabs\\ExitTile.prefab");
//                break;
//            case 230:
//                found = AssetDatabase.LoadAssetAtPath<GameObject>("Assets\\GameObjects\\Rooms & Tiles\\Resources\\Radioactive Zone\\Interactibles\\Prefabs\\SpawnTile.prefab");
//                break;
//            case 220:
//                found = AssetDatabase.LoadAssetAtPath<GameObject>("Assets\\Art\\test import\\tiles\\tile slope\\rouille\\tile_slope_prefab.prefab");
//                break;
//            case 79:
//                found = AssetDatabase.LoadAssetAtPath<GameObject>("Assets\\Art\\test import\\tiles\\tile block\\rouille\\tile_slope_prefab.prefab");
//                break;
//            case 32:
//                switch (UnityEngine.Random.Range(0, 2))
//                {
//                    case 0:
//                        found = AssetDatabase.LoadAssetAtPath<GameObject>("Assets\\GameObjects\\Rooms & Tiles\\Resources\\Radioactive Zone\\Topology\\Prefab_tile_block_sable.prefab");
//                        break;
//                    case 1:
//                        found = AssetDatabase.LoadAssetAtPath<GameObject>("Assets\\GameObjects\\Rooms & Tiles\\Resources\\Radioactive Zone\\Topology\\Prefab_tile_block_sable_foncé.prefab");
//                        break;
//                }
//                break;
//            case 41:
//                found = AssetDatabase.LoadAssetAtPath<GameObject>("Assets\\GameObjects\\Rooms & Tiles\\Resources\\Radioactive Zone\\Interactibles\\Prefabs\\DiveSpot.prefab");
//                break;
//            case 8:
//                found = AssetDatabase.LoadAssetAtPath<GameObject>("Assets\\GameObjects\\Rooms & Tiles\\Resources\\Radioactive Zone\\Topology\\Concrete.prefab");
//                break;
//            case 72:
//                found = AssetDatabase.LoadAssetAtPath<GameObject>("Assets\\GameObjects\\Rooms & Tiles\\Resources\\Radioactive Zone\\Topology\\Concrete.prefab");
//                break;
//            default:
//                print("you probably missed somewhere with color index : " + vox.colorIndex);
//                break;
//        }
//        return found;
//    }
//}
