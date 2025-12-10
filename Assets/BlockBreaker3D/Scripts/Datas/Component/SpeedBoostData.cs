using UnityEngine;

namespace BlockBreaker3D.Datas.Component
{
    /// <summary>
    /// 速度を上げるコンポーネント。
    /// </summary>
    [CreateAssetMenu(fileName = "SpeedBoostData", menuName = "BlockBreaker3D/Component/SpeedBoostData")]
    public class SpeedBoostData : CompData
    {
        public int AddSpeed = 5;
        public float Duration = 3f;
        public bool EnableStackingFromSameSource = true;
        public override string ClassName => "BlockBreaker3D.Models.InGame.Component.SpeedBoostCompInfo";
    }
}