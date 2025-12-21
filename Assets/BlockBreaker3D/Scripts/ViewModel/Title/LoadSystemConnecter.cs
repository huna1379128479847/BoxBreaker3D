using BlockBreaker3D.Models.Resource;
using BlockBreaker3D.Utils;
using Cysharp.Threading.Tasks;
using HighElixir.Unity.Addressable.SceneManagement.Helpers;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;
namespace BlockBreaker3D.ViewModel
{
    public class LoadSystemConnector
    {
        [Inject]
        public LoadSystemConnector(SceneLoader loader, [Inject(Id = "GoMenu")] Button goMenu)
        {
            goMenu.onClick.AddListener(() => LoadAsync(loader, SceneLoader.Scene.Level1).Forget());
        }

        private async UniTask LoadAsync(SceneLoader loader, SceneLoader.Scene scene, Action<float> action = null)
        {
            BDebug.Log("Go to Main Menu", BDebug.BColor.blue, nameof(LoadSystemConnector));

            var inst = await loader.LoadSceneAsync(SceneLoader.Scene.Level1, false, progress: action != null ? Progress.Create<float>(action) : null); // 仮で１固定
            // 遷移演出を挟む

            // 挟む
            var current = SceneManager.GetActiveScene();
            inst.ActivateAsync();
            await SceneManageHelper.UnloadSceneAsync(current);

        }
    }
}