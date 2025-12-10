using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace BlockBreaker3D.Core
{
    public static class Slow
    {
        private static float _saveTimeScale = 1f;
        private static bool _isPlaying = false;
        private static UniTask _current;
        private static CancellationTokenSource _cts;
        public static bool IsPlaying => _isPlaying;

        public static void Play(float target, float duration)
        {
            if (_current.Status != UniTaskStatus.Succeeded && !_cts.IsCancellationRequested)
            {
                _cts?.Cancel();
            }
            _cts = new CancellationTokenSource();
            _current = PlayAsync(target, duration, _cts.Token);
        }
        private static async UniTask PlayAsync(float target, float duration, CancellationToken token)
        {
            if (!_isPlaying) _saveTimeScale = Time.timeScale;
            _isPlaying = true;
            GameTimeScale.SetTimeScale(target);
            _current = UniTask.Delay(TimeSpan.FromSeconds(duration),ignoreTimeScale: true, cancellationToken: token);
            await _current;
            if (!token.IsCancellationRequested)
            {
                GameTimeScale.SetTimeScale(_saveTimeScale);
                _isPlaying = false;
            }
        }

        public static void Stop()
        {
            if (_current.Status != UniTaskStatus.Succeeded && !_cts.IsCancellationRequested)
            {
                _cts?.Cancel();
            }
            GameTimeScale.SetTimeScale(_saveTimeScale);
            _isPlaying = false;
        }
    }
}