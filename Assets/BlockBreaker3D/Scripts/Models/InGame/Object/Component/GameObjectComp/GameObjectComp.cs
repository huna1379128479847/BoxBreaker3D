using UnityEngine;
using Zenject;

namespace BlockBreaker3D.Models.InGame.Component.GameObjectComp
{
    /// <summary>
    /// ScriptableObjectを用意せずにMonoBehaviourから直接Compを生成するための基底クラス
    /// </summary>
    public abstract class GameObjectComp : MonoBehaviour
    {
        [Inject]
        // Zenjectとのタイミングを合わせるためにConstructメソッドによって初期化を行う
        public void Construct()
        {
            if (gameObject.TryGetComponent<ObjectBase>(out var obj))
            {
                var c = Create();
                obj.AddCompAsStartMember(c);
                Destroy(this); // 初期化が完了したらこのコンポーネントを破棄する
            }
            else
            {
                Debug.LogError($"GameObjectComp must be attached to a GameObject with ObjectBase. GameObject name: {gameObject.name}");
            }
        }

        public abstract Comp Create();
    }
}