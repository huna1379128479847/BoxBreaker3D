using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace BlockBreaker3D.View.InGame
{
    public interface IGameView : IInitializable
    {
        void Enable();
        void Disable();
        UniTask PlayGameClearAnim();
        UniTask PlayGameOverAnim();
        void UpdateView();
        void InitState();
    }

    public abstract class GameViewBase : MonoBehaviour, IGameView
    {
        public virtual void Enable()
        {
        }
        public virtual void Disable()
        {
        }
        public virtual async UniTask PlayGameClearAnim()
        {
            await UniTask.CompletedTask;
        }
        public virtual async UniTask PlayGameOverAnim()
        {
            await UniTask.CompletedTask;
        }
        public virtual void Initialize()
        {
        }
        public virtual void UpdateView()
        {
        }

        public virtual void InitState()
        {
        }
    }
}