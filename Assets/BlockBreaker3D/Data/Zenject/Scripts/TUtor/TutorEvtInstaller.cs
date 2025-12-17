using BlockBreaker3D.View.InGame;
using BlockBreaker3D.ViewModel;
using BlockBreaker3D.ViewModel.Tutor;
using TMPro;
using UnityEngine;
using Zenject;

namespace BlockBreaker3D.Zenject
{
    public class TutorEvtInstaller : MonoInstaller
    {
        [SerializeField] private TutorInput _tutorInput;
        [SerializeField] private TMP_Text _tutorText;
        public override void InstallBindings()
        {
            // View
            Container.Bind<TMP_Text>().WithId("Space").FromInstance(_tutorText).AsCached().NonLazy();
            Container.Bind<TutorInput>().FromInstance(_tutorInput).AsSingle().NonLazy();

            // Model
            Container.BindInterfacesAndSelfTo<Models.InGame.TutorEvent>().AsSingle().NonLazy();

            // ViewModel
            Container.Bind<TutorEvtHandle>().AsSingle().NonLazy();
            Container.Bind<ScoreViewer>().AsSingle().NonLazy();
        }
    }
}