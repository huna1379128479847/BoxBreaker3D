using BlockBreaker3D.Datas;
using BlockBreaker3D.Utils;
using Cysharp.Threading.Tasks;
using HighElixir.Unity.Addressable.SceneManagement;
using HighElixir.Unity.Addressable.SceneManagement.Helpers;
using System;
using System.Threading;
using UnityEngine;

namespace BlockBreaker3D.Models
{
    // Zenjectを用い、他シーンからも利用可能なMonoBehaviourシーンローダー
    public sealed class LoadSceneMono : MonoBehaviour
    {
        public async UniTask LoadSceneAsync(string sceneName, CancellationToken token = default)
            => await LoadSceneInternal(sceneName, token);
        public async UniTask LoadSceneAsync(StageData stageData, CancellationToken token = default)
            => await LoadSceneInternal(stageData.SceneName, token);

        private async UniTask LoadSceneInternal(string sceneName, CancellationToken token)
        {
            await using (var u = SceneManageHelper.GetCurrentSceneUnloader())
            {
                try // Memo: ロード演出もtryで囲むと重いかもしれない？
                {
                    var inst = await SceneLoaderAsync.LoadSceneAsync(sceneName, false, token: token);
                    // TODO : Load演出など
                    // 今は省略
                    await inst.ActivateAsync(); // シーンアクティベート
                    LogPainter.Info($"シーンロード完了: {sceneName}");
                }
                catch (OperationCanceledException)
                {
                    u.ShouldUnload = false;
                }
                catch (Exception e)
                {
                    u.ShouldUnload = false; 
                    LogPainter.Error($"シーンロードに失敗しました: {sceneName}\n{e}");
                }
            } // この時点で前のシーンアンロード
        }
    }
}