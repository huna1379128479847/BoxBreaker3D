using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
namespace BlockBreaker3D.Editor
{
    public class MapCreator : OdinEditorWindow
    {
        private Dictionary<string, GameObject> _prefabs = new();

        [MenuItem("BlockBreaker3D/Map Creator")]
        private static void OpenWindow()
        {
            var window = GetWindow<MapCreator>();
            window.titleContent = new GUIContent("Map Creator");
            window.Show();
        }

        [Button]
        private async UniTask CreateBlock()
        {
            const string path = "Map/Parts/Cube1";
            if (!_prefabs.ContainsKey(path))
                _prefabs[path] = await Addressables.LoadAssetAsync<GameObject>(path);
            Create(_prefabs[path]);
        }

        [Button]
        private async UniTask CreateWall()
        {
            const string path = "Map/Parts/Wall1";
            if (!_prefabs.ContainsKey(path))
                _prefabs[path] = await Addressables.LoadAssetAsync<GameObject>(path);
            Create(_prefabs[path]);
        }

        [Button]
        private async UniTask CreateNumber()
        {
            const string path = "Map/Parts/Number1";
            if (!_prefabs.ContainsKey(path))
                _prefabs[path] = await Addressables.LoadAssetAsync<GameObject>(path);
            Create(_prefabs[path]);
        }


        private void Create(Object pref)
        {
            var go = PrefabUtility.InstantiatePrefab(pref) as GameObject;
            Undo.RegisterCreatedObjectUndo(go, "Create Object");
            if (Selection.activeGameObject != null)
            {
                go.transform.parent = Selection.activeGameObject.transform;
            }
        }
    }
}