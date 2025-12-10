using BlockBreaker3D.Datas.Signals;
using BlockBreaker3D.Models.InGame.Box;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace BlockBreaker3D.ViewModel
{
    /// <summary>
    /// ブロックの残り数を管理するViewModel
    /// </summary>
    public class BlockRemainingViewModel : IFixedTickable
    {
        private readonly SignalBus _signalBus;
        private readonly BoxBehaviour _box;
        private readonly TMP_Text _scoreText;
        private readonly ReactiveProperty<int> _displayedScore = new(0);

        public IReadOnlyReactiveProperty<int> CurrentScore => _displayedScore;
        public int ChangeAmountPerFrame { get; set; } = 15; // ベース変化量
        public float ScalePerDigit { get; set; } = 10.0f;    // 使うなら後で

        public BlockRemainingViewModel(SignalBus signalBus, BoxBehaviour holder, [Inject(Id = "BlockText")] TMP_Text scoreText)
        {
            _signalBus = signalBus;
            _box = holder;
            _displayedScore.Value = _box.GetTotalBlockCount();
            _scoreText = scoreText;

            int _lastDigits = 1;

            _displayedScore
                .Subscribe(score =>
                {
                    _scoreText.text = $"Blocks: {score}";

                    var digits = Mathf.FloorToInt(Mathf.Log10(Mathf.Max(1, score))) + 1;
                    if (digits != _lastDigits)
                    {
                        // ここでスケール演出をかける
                        var s = 1f + (digits - _lastDigits) * (ScalePerDigit * 0.1f);
                        _lastDigits = digits;
                    }
                })
                .AddTo(_scoreText);
            _signalBus.GetStream<GameSignal>()
                .Subscribe(s =>
                {
                    if (s.HasAny(GameSignal.Type.GameClear, GameSignal.Type.GameOver))
                        _scoreText.gameObject.SetActive(false);
                    if (s.HasAny(GameSignal.Type.GameStarted, GameSignal.Type.Restart))
                        _scoreText.gameObject.SetActive(true);
                }).AddTo(_scoreText);
        }

        public void FixedTick()
        {
            var current = _displayedScore.Value;
            var target = _box.GetTotalBlockCount();
            var diff = target - current;

            if (diff == 0) return;

            // 残り差分の絶対値に対する桁数
            var absDiff = Mathf.Abs(diff);
            var digits = Mathf.FloorToInt(Mathf.Log10(Mathf.Max(1, absDiff))) + 1;

            // 桁数に応じてステップを増やす
            var step = Mathf.Max(1, ChangeAmountPerFrame * digits);

            if (absDiff <= step)
            {
                _displayedScore.Value = target;
            }
            else
            {
                _displayedScore.Value += diff > 0 ? step : -step;
            }
        }
    }
}