//
// Copyright (C) 2014 Sixense Entertainment Inc.
// All Rights Reserved
//
// SixenseCore Unity Plugin
// Version 0.1
//

using UnityEditor;
using UnityEngine;

namespace SixenseCore
{
    public class CreatePrefabs
    {
        // Add menu item
        [MenuItem("GameObject/Create Other/SixenseCore Device")]

        static void Device()
        {
            Object obj = AssetDatabase.LoadAssetAtPath("Assets/SixenseCore/Prefabs/SixenseCoreDevice.prefab", typeof(UnityEngine.Object));
            GameObject go = PrefabUtility.InstantiatePrefab(obj) as GameObject;

            if (go != null)
            {
                Selection.activeObject = go;

                if (SceneView.lastActiveSceneView != null)
                    SceneView.lastActiveSceneView.FrameSelected();
    
                SceneView.RepaintAll();
            }
        }
    }
}