using BlockBreaker3D.Models.Menu;
using BlockBreaker3D.View.Menu;
using BlockBreaker3D.ViewModel.Menu;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace BlockBreaker3D.Zenject
{
    public sealed partial class MenuInstaller : MonoInstaller
    {
        [SerializeField] private GridLayoutGroup _stageButtonCont; // ボタンを格納する親オブジェクト
        [SerializeField] private StageButton _stageButtonPref; // ステージ選択ボタンのプレハブ
        [SerializeField] private StageDataHolder _stageDataHolder; // ステージデータホルダー

        public override void InstallBindings()
        {
            // View
            Container.Bind<GridLayoutGroup>().WithId("StageButtonCont").FromInstance(_stageButtonCont).AsSingle();
            Container.BindMemoryPool<StageButton, StageButton.ButtonPool>()
                .WithInitialSize(10)
                .FromComponentInNewPrefab(_stageButtonPref)
                .UnderTransform(_stageButtonCont.transform);
            Container.BindInterfacesAndSelfTo<StageButtonHolder>().AsSingle();

            // Model
            Container.Bind<StageDataHolder>().FromInstance(_stageDataHolder).AsSingle();

            // ViewModel
            Container.BindInterfacesAndSelfTo<StageSelector>().AsSingle().NonLazy();
        }
    }
}