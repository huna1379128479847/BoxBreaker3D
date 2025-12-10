using BlockBreaker3D.Models;
using BlockBreaker3D.Models.InGame;
using DG.Tweening;
using UniRx;
using Unity.Cinemachine;
using Zenject;

namespace BlockBreaker3D.ViewModel
{
    public class CamViewModel : IInitializable
    {
        private GameDataHolder _dataHolder;
        private CinemachineCamera _camera;

        private DG.Tweening.Sequence _sq;
        [Inject]
        public CamViewModel(GameDataHolder dataHolder, [Inject(Id = "BallView")]CinemachineCamera camera)
        {
            _dataHolder = dataHolder;
            _camera = camera;
        }

        public void Initialize()
        {
            _dataHolder.BoxBehaviour
                .Where(b => b != null)
                .Subscribe(box => box.CurrentSurface.Subscribe(BindToSurface).AddTo(_camera))
                .AddTo(_camera);
        }

        public void BindToSurface(SurfaceBehaviour surface)
        {
            if (_sq.IsActive())
                _sq.Kill();
            _sq = DOTween.Sequence();
            _sq.Append(_camera.transform.DOLocalRotate(surface.CamRotate, 0.4f));
        }
    }
}