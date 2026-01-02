using BlockBreaker3D.Utils;
using Cysharp.Threading.Tasks;
using HighElixir.Unity.Addressable.SceneManagement;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BlockBreaker3D.Models.Resource
{
    // Zenjectのインストールイベントで呼び出されるコンポーネント
    public class InstallEventBehavior : MonoBehaviour
    {
        // ブーストラップの読み込み
        public async void PreInstall()
        {
            if (SceneManager.GetSceneByName("BootstrapScene").isLoaded)
                return;
            await SceneLoaderAsync.LoadSceneAsync("Scene/Boostrap", true, false, false);
        }

        public async void PostInstall()
        {
            // このオブジェクトの所属シーンがアクティブなら何もしない
            if (gameObject.scene == SceneManager.GetActiveScene())
                return;
            if (SceneManager.GetActiveScene().name == "BootstrapScene")
            {
                // BootstrapSceneがアクティブならば、自身のシーンをアクティブにする
                await UniTask.WaitUntil(() => gameObject.scene.isLoaded);
                try
                {
                    SceneManager.SetActiveScene(gameObject.scene);
                }
                catch(Exception e)
                {
                    LogPainter.Error(e.Message, BColor.red);
                }
            }
        }
    }
}