using BlockBreaker3D.Datas.Component;
using System.Collections.Generic;
using UnityEngine;

namespace BlockBreaker3D.Models.InGame.Component.GameObjectComp
{
    /// <summary>
    /// ValidateViewData で定義された条件を評価し、全条件が満たされたタイミングでアクションを実行するコンポーネント。
    /// </summary>
    public class ValidateView : GameObjectComp
    {
        [SerializeField] private UnlockWithData _validateViewData;
        [SerializeField] private List<ValidateViewStruct> _validateViews = new();
        public override Comp Create()
        {
            throw new System.NotImplementedException();
        }
    }
}