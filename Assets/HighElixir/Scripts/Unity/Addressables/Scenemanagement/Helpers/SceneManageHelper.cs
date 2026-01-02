using HighElixir.Unity.Addressable.SceneManagement.Internal;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.ResourceManagement.ResourceProviders;
using System;

namespace HighElixir.Unity.Addressable.SceneManagement.Helpers
{
    public static class SceneManageHelper
    {
        public sealed class Unloader : IAsyncDisposable
        {
            public SceneInstance? SceneInstance { get; private set; }
            public Scene? Scene { get; private set; }

            public bool ShouldUnload { get; set; } = true;
            public bool IsDisposed => !SceneInstance.HasValue && !Scene.HasValue;

            public Unloader(SceneInstance sceneInstance)
            {
                SceneInstance = sceneInstance;
            }
            public Unloader(Scene scene)
            {
                Scene = scene;
            }

            public async ValueTask DisposeAsync()
            {
                if (IsDisposed) return;
                if (ShouldUnload)
                {
                    if (SceneInstance.HasValue)
                        await UnloadSceneAsync(SceneInstance.Value);
                    else if (Scene.HasValue)
                        await UnloadSceneAsync(Scene.Value);
                }
                Cleanup();
            }

            private void Cleanup()
            {
                SceneInstance = null;
                Scene = null;
            }
        }

        public static Unloader GetCurrentSceneUnloader()
        {
            if (SceneStack.TryGetCurrentSceneInstance(out var inst))
                return new Unloader(inst);
            else
                return new Unloader(SceneManager.GetActiveScene());
        }

        public static bool TryGetCurrentSceneInstance(out SceneInstance sceneInstance)
        {
            return SceneStack.TryGetCurrentSceneInstance(out sceneInstance);
        }
        public static async Task UnloadSceneAsync(Scene scene)
        {
            if (SceneStack.TryGetScene(scene, out var inst))
                await UnloadSceneAsync(inst);
            else
                await SceneManager.UnloadSceneAsync(scene);
        }
        public static async Task UnloadSceneAsync(SceneInstance scene)
        {
            await Addressables.UnloadSceneAsync(scene).Task;
            SceneStack.UnregisterScene(scene);
        }

        public static async Task UnloadCurrentSceneAsync()
        {
            if (SceneStack.TryGetCurrentSceneInstance(out var inst))
            {
                await Addressables.UnloadSceneAsync(inst, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects).Task;
                SceneStack.UnregisterScene(inst);
            }
            else
            {
                await SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
            }
        }

        public static bool TryCreateReference(string guid, out AssetReference assetReference)
        {
            assetReference = null;
            if (string.IsNullOrEmpty(guid)) return false;
            try
            {
                assetReference = new(guid);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}