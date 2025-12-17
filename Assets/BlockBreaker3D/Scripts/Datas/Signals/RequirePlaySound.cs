using DG.Tweening;
using UnityEngine;

namespace BlockBreaker3D.Datas.Signals
{
    public sealed class RequirePlaySound
    {
        public struct FadeOption // フェードイン・フェードアウトの設定
        {
            public readonly float FirstVolume;
            public readonly float Duration;
            public readonly float LastVolume;
            public readonly Ease Ease;

            public FadeOption(float firstVolume, float duration, float lastVolume, Ease ease)
            {
                FirstVolume = firstVolume;
                Duration = duration;
                LastVolume = lastVolume;
                Ease = ease;
            }
        }

        public readonly bool IsBGM;
        public readonly AudioClip Clip;
        public readonly Vector3 Point;
        public FadeOption? FadeIn = null;
        public bool Played { get; set; } = false; // 再生済みかどうか

        public RequirePlaySound(AudioClip clip, bool isBGM)
        {
            Clip = clip;
            IsBGM = isBGM;
        }

        public RequirePlaySound(AudioClip clip, Vector3 point) : this(clip, false)
        {
            Point = point;
        }
        public void TakeRequier()
        {
            Played = true;
        }

        public RequirePlaySound WithFadeIn(float firstVolume, float duration, float lastVolume, Ease ease)
        {
            FadeIn = new FadeOption(firstVolume, duration, lastVolume, ease);
            return this;
        }
    }
}