using UnityEngine;

namespace BlockBreaker3D.Datas.Component.Balls
{
    [CreateAssetMenu(fileName = "BallCollisionHandler", menuName = "BlockBreaker3D/Component/Balls/BallCollisionHandler")]
    public class BallCollisionHandlerData : CompData
    {
        public override string ClassName => "BlockBreaker3D.Models.InGame.Balls.BallCollisionHandler";
    }
}