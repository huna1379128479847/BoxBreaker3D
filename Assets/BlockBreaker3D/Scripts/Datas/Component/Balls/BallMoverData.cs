using UnityEngine;

namespace BlockBreaker3D.Datas.Component.Balls
{
    [CreateAssetMenu(fileName = "BallMoverData", menuName = "BlockBreaker3D/Component/Balls/BallMoverData")]
    public class BallMoverData : CompData
    {
        public override string ClassName => "BlockBreaker3D.Models.InGame.Balls.BallMover";
    }
}