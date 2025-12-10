using UniRx;
using UnityEngine;

namespace BlockBreaker3D.Core
{
    /// <summary>
    /// ゲーム全体の時間スケールを管理します <br />
    /// Time.timeScale に依存するシステムと独立して動作します <br />
    /// (GameStateManagerからの移行中)
    /// </summary>
    public static class GameTimeScale
    {
        private static readonly ReactiveProperty<float> _timeScale = new(1f);
        public static IReadOnlyReactiveProperty<float> TimeScale => _timeScale;

        // ポーズ中の時間スケールを保存するための変数
        private static float _pauseedGameTimeScale = 1f;
        private static float _pausedTimeScale = 0f;
        private static bool _isPaused = false;

        // ポーズ状態を監視して更新
        public static bool IsPaused => _isPaused;

        /// <summary>
        /// ポーズ・アンポーズを切り替えます
        /// </summary>
        /// <param name="pause"></param>
        public static void Pause(bool pause)
        {
            _isPaused = pause;
            if (pause)
            {
                _pauseedGameTimeScale = _timeScale.Value;
                _pausedTimeScale = Time.timeScale;
                _timeScale.Value = 0f;
                Time.timeScale = 0f;
            }
            else
            {
                _timeScale.Value = _pauseedGameTimeScale;
                Time.timeScale = _pausedTimeScale;
            }
        }

        /// <summary>
        /// 相対的な時間スケールを取得します
        /// </summary>
        public static float RelativeTimeScale()
        {
            return _timeScale.Value * Time.timeScale;
        }

        /// <summary>
        /// Unity の Time.timeScale を直接設定します <br />
        /// パーティクルやアニメーションなど、Time.timeScale に依存するシステムに影響を与えます
        /// </summary>
        public static void SetTimeScale(float scale)
        {
            if (_isPaused) return;
            Time.timeScale = scale;
        }

        /// <summary>
        /// 内部の時間スケールを設定します
        /// </summary>
        /// <param name="scale"></param>
        public static void SetGameTimeScale(float scale)
        {
            if (_isPaused) return;
            _timeScale.Value = scale;
        }

        /// <summary>
        /// 両方の時間スケールをリセットします。ポーズ状態も解除されます
        /// </summary>
        public static void ResetTimeScale()
        {
            Time.timeScale = 1f;
            _timeScale.Value = 1f;
            _isPaused = false;
        }
    }
}