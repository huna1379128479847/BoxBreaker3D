using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;

namespace BlockBreaker3D.Models.Sounds
{
    public class AudioLoader
    {
        private const string BGMPath = "Data/Sounds/BGM/";
        private const string SEPath = "Data/Sounds/SE/";
        private readonly Dictionary<string, AudioClip> _cache = new();

        public async UniTask<AudioClip> LoadBGMAsync(string name)
            => await LoadAsync(BGMPath + name);

        public async UniTask<AudioClip> LoadSEAsync(string name)
            => await LoadAsync(SEPath + name);


        private async UniTask<AudioClip> LoadAsync(string path)
        {
            if (_cache.ContainsKey(path))
            {
                return _cache[path];
            }
            var handle = Addressables.LoadAssetAsync<AudioClip>(path);
            await handle.Task;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _cache[path] = handle.Result;
                return handle.Result;
            }
            else
            {
                throw new Exception($"Failed to load AudioClip at path: {path}");
            }
        }
    }
}