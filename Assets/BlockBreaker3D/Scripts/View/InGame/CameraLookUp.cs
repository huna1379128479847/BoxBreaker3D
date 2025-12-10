using UnityEngine;
using Unity.Cinemachine;
using Cysharp.Threading.Tasks;

namespace BlockBreaker3D.View.InGame
{
    public class CameraLookUp : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera _camera;
        [SerializeField] private float _lookUpDistance = 7f;
        [SerializeField] private float _duration = 0.5f;
        private float _defaultDistance;
        private bool _isLookingUp = false;

        public float LookUpDistance => _lookUpDistance;
        public float Duration => _duration;
        public void LookUp()
        {
            var body = _camera.GetCinemachineComponent(CinemachineCore.Stage.Body);
            if (body is CinemachinePositionComposer composer)
            {
                if (_isLookingUp) return;
                LookUpAnim(composer).Forget();
            }
        }

        private async UniTask LookUpAnim(CinemachinePositionComposer composer)
        {
            _isLookingUp = true;
            _defaultDistance = composer.CameraDistance;
            composer.CameraDistance = _lookUpDistance;
            await UniTask.Delay((int)(_duration * 1000), true);
            composer.CameraDistance = _defaultDistance;
            _isLookingUp = false;
        }
    }
}