using Cysharp.Threading.Tasks;
using HighElixir.Unity.Addressable.SceneManagement;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace BlockBreaker3D.Models.Resource
{
    public class SceneLoader
    {
        public enum Scene
        {
            MainMenu,
            Level1,
            Level2,
            Level3,
            GameOver,
            Victory
        }
        private static readonly Dictionary<Scene, string> _names = new()
        {
            { Scene.MainMenu, "Scene/Title" },
            { Scene.Level1, "Scene/Stage1" },
            { Scene.Level2, "Level2" },
            { Scene.Level3, "Level3" },
            { Scene.GameOver, "GameOver" },
            { Scene.Victory, "Victory" }
        };
        public async UniTask<SceneInstance> LoadSceneAsync(Scene scene, bool autoActive, CancellationToken token = default, IProgress<float> progress = null)
        {
            var targetSceneName = _names[scene];
            return await SceneLoaderAsync.LoadSceneAsync(targetSceneName, autoActive, token: token, progress: progress);
        }
    }
}