//using UnityEngine;

//namespace BlockBreaker3D.ViewModel
//{
//    public class FollowBallCam :  
//    {
//        public FollowBallCam()
//        {
//        }
//        public override void Tick()
//        {
//            if (_holder == null || _holder.BallBehaviour == null || _view == null) return;
//            var ballPos = _holder.BallBehaviour.transform.position;
//            var camTransform = _view.transform;
//            // カメラの位置をボールの位置に合わせて更新
//            camTransform.position = new UnityEngine.Vector3(ballPos.x, camTransform.position.y, ballPos.z);
//        }
//    }
//}