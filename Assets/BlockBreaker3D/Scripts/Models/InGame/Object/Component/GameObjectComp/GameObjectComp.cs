using UnityEngine;

namespace BlockBreaker3D.Models.InGame.Component.GameObjectComp
{
    /// <summary>
    /// ScriptableObjectを用意せずにMonoBehaviourから直接Compを生成するための基底クラス
    /// </summary>
    public abstract class GameObjectComp : MonoBehaviour, ICompdata
    {
        public void Construct()
        {
            if (gameObject.TryGetComponent<ObjectBase>(out var obj))
                obj.AddCompAsStartMember(Create());
            else
                Debug.LogError($"GameObjectComp must be attached to a GameObject with ObjectBase. GameObject name: {gameObject.name}");
            Destroy(this); // 初期化が完了したらこのコンポーネントを破棄する
        }

        public abstract Comp Create();
    }
}