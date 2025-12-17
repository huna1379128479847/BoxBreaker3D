using BlockBreaker3D.Models.InGame;
using BlockBreaker3D.View.InGame;
using TMPro;
using UniRx;
using Zenject;

namespace BlockBreaker3D.ViewModel
{
    public class TutorEvtHandle
    {
        public TutorEvtHandle(TutorInput input, TutorEvent tutorEvent, [Inject(Id = "Space")] TMP_Text text)
        {
            tutorEvent.Enabled
                .Where(x => x)
                .Take(1)
                .Subscribe(_ =>
                {
                    input.OnSpaceAsObservable()
                        .Take(1)
                        .Subscribe(__ =>
                        {
                            tutorEvent.InputSpace();
                        })
                        .AddTo(input);
                })
                .AddTo(input);
            tutorEvent.Enabled
                .Subscribe(x =>
                {
                    if (x)
                    {
                        text.gameObject.SetActive(true);
                    }
                    else
                    {
                        text.gameObject.SetActive(false);
                    }
                });
        }
    }
}