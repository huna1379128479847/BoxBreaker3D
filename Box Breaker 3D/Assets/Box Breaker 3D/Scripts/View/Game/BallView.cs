using BoxBreaker3D.ViewModel;
using UniRx;
using UnityEngine;
using Zenject;

namespace BoxBreaker3D.View
{
    public class BallView : MonoBehaviour
    {
        private BallViewModel _viewModel;

        // 外から Model を渡して初期化する想定
        [Inject]
        public void Bind(BallViewModel model)
        {
            Debug.Log("3");
            _viewModel = model;
            _viewModel.OnTick.Subscribe(dt =>
            {
                // ② Modelの位置をViewに反映
                transform.position = _viewModel.CurrentPosition;
            }).AddTo(this);
            // 初期位置同期
            _viewModel.SetPosition(transform.position);
        }
    }
}