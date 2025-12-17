using BlockBreaker3D.Datas.Signals;
using BlockBreaker3D.Models.InGame.GameStatus;
using BlockBreaker3D.View.InGame;
using BlockBreaker3D.Core;
using System;
using UniRx;
using UnityEngine;
using Zenject;
using Cysharp.Threading.Tasks;

namespace BlockBreaker3D.ViewModel
{
    /// <summary>
    /// ScoreHolderのスコアを徐々に変化させて表示するためのクラス
    /// </summary>
    public class ScoreLeaper : ViewModelBase, IFixedTickable
    {
        private readonly ScoreHolder _scoreHolder;
        private readonly AbstractScoreView _scoreText;
        private readonly ReactiveProperty<int> _displayedScore = new(0);
        public IReadOnlyReactiveProperty<int> CurrentScore => _displayedScore;
        public int ChangeAmountPerFrame { get; set; } = 15; // ベース変化量
        public float ScalePerDigit { get; set; } = 10.0f;    // 使うなら後で

        public override SetViewVisible.ViewType ViewType => SetViewVisible.ViewType.ScoreView;

        public ScoreLeaper(ScoreHolder holder, AbstractScoreView scoreText)
        {
            _scoreHolder = holder;
            _displayedScore.Value = _scoreHolder.Score.Value;
            _scoreText = scoreText;

            _displayedScore
                .Subscribe(score => _scoreText?.UpdateScore(score))
                .AddTo(Disposables);
        }

        public void FixedTick()
        {
            var current = _displayedScore.Value;
            var target = _scoreHolder.Score.Value;
            var diff = target - current;

            if (diff == 0) return;

            // 残り差分の絶対値に対する桁数
            var absDiff = Mathf.Abs(diff);
            var digits = Mathf.FloorToInt(Mathf.Log10(Mathf.Max(1, absDiff))) + 1;

            // 桁数に応じてステップを増やす。GameTimeScale の影響を受けるようにする。
            var scale = GameTimeScale.RelativeTimeScale();
            // If time scale is zero, pause score progression
            if (Mathf.Approximately(scale, 0f)) return;
            var scaledStep = ChangeAmountPerFrame * digits * scale;
            var step = Mathf.Max(1, Mathf.RoundToInt(scaledStep));

            if (absDiff <= step)
            {
                _displayedScore.Value = target;
            }
            else
            {
                _displayedScore.Value += diff > 0 ? step : -step;
            }
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public override void Initialize()
        {
            _scoreText?.InitState();
        }

        protected override void SetVisible(bool isActive)
        {
            _scoreText?.Enable(isActive);
        }

        public override void OnReceiveGameSignal(GameSignal.Type type)
        {
            if (type.Has(GameSignal.Type.GameClear))
            {
                _displayedScore.Value = _scoreHolder.Score.Value;
                if (_scoreText != null)
                    _scoreText.PlayGameClearAnim().Forget();
            }
            if (type.Has(GameSignal.Type.GameOver))
                _scoreText?.Enable(false);
            if (type.HasAny(GameSignal.Type.GameStarted, GameSignal.Type.Restart))
            {
                _scoreText?.InitState();
                _scoreText?.Enable(true);
            }
        }
    }
}