using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace BlockBreaker3D.View.InGame
{
    public interface IGameView : IInitializable
    {
        void Enable(bool enable);
        UniTask PlayGameClearAnim();
        UniTask PlayGameOverAnim();
        void UpdateView();
        void InitState();
    }

    public abstract class GameViewBase : MonoBehaviour, IGameView
    {
        public virtual void Enable(bool enable)
        {
            // Default behavior: toggle GameObject active state
            gameObject.SetActive(enable);
        }
        public virtual async UniTask PlayGameClearAnim()
        {
            await UniTask.CompletedTask;
        }
        public virtual async UniTask PlayGameOverAnim()
        {
            await UniTask.CompletedTask;
        }

        // 初期化処理
        public virtual void Initialize()
        {
        }
        public virtual void UpdateView()
        {
        }

        // リセット時などの状態初期化
        public virtual void InitState()
        {
        }
    }
}