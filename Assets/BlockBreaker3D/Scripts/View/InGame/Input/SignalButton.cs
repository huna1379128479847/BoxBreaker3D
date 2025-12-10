using BlockBreaker3D.Datas.Signals;
using UnityEngine;
using Zenject;

namespace BlockBreaker3D.View.InGame
{
    public class SignalButton : MonoBehaviour
    {
        [Inject] private SignalBus _signalBus;
        [EnumButtons]
        [SerializeField] private GameSignal.Type _fireOnClick;

        public void Fire()
        {
            _signalBus.Fire(new GameSignal(_fireOnClick));
        }
    }
}