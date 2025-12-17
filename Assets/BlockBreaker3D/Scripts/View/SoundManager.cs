using BlockBreaker3D.Datas.Signals;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using Zenject;

namespace BlockBreaker3D.View
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private AudioSource _bgmAudioSource;

        [Inject]
        public void Construct(SignalBus signal)
        {
            signal.GetStream<RequirePlaySound>()
                .Subscribe(x =>
                {
                    if (x.Played) return;
                    x.TakeRequier();
                    if (x.IsBGM)
                    {
                        if (x.FadeIn.HasValue)
                            FadeVolumeToBGM(x.FadeIn.Value.FirstVolume, x.FadeIn.Value.LastVolume, x.FadeIn.Value.Duration, x.FadeIn.Value.Ease);
                        PlayBGM(x.Clip);
                    }
                    else
                    {
                        AudioSource.PlayClipAtPoint(x.Clip, x.Point);
                    }
                })
                .AddTo(this);
        }

        public void PlayBGM(AudioClip clip)
        {
            _bgmAudioSource.clip = clip;
            _bgmAudioSource.Play();
        }

        public void StopBGM()
        {
            _bgmAudioSource.Stop();
        }

        public void FadeVolumeToBGM(float start, float targetVolume, float duration, Ease ease)
        {
            _bgmAudioSource.volume = start;
            _bgmAudioSource.DOFade(targetVolume, duration).SetEase(ease);
        }
    }
}