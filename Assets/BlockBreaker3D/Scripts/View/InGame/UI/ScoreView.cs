using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Zenject;

namespace BlockBreaker3D.View.InGame
{
    public class ScoreView : AbstractScoreView
    {
        [SerializeField] private Transform _clearedPosition; // ゲームクリア時のアニメーション用位置
        [SerializeField] private float _clearAnimationDuration = 1.0f;
        [SerializeField] private float _clearAnimationScale = 1.5f;
        [SerializeField] private Ease _clearAnimationEase = Ease.InOutQuad;

        private Vector3 _initialPosition;
        private TMP_Text _scoreText;
        private Sequence _sequence;
        public override void UpdateScore(int newScore)
        {
            if (_scoreText == null)
            {
                _scoreText = GetComponent<TMP_Text>();
            }
            _scoreText.text = $"Lumen: {newScore}lu";
        }

        public override async UniTask PlayGameClearAnim()
        {
            _ = _sequence.Join(gameObject.transform.DOMove(_clearedPosition.position, _clearAnimationDuration).SetEase(_clearAnimationEase));
            _ = _sequence.Join(gameObject.transform.DOScale(Vector3.one * _clearAnimationScale, _clearAnimationDuration).SetEase(_clearAnimationEase));
            await _sequence.Play();
        }

        [Inject]
        public override void Initialize()
        {
            _scoreText = GetComponent<TMP_Text>();
            if (_sequence.IsActive())
                _sequence.Kill();
            _sequence = DOTween.Sequence();
            _initialPosition = gameObject.transform.position;
        }

        public override void InitState()
        {
            gameObject.transform.position = _initialPosition;
        }

        public override void Enable(bool enable)
        {
            if (_scoreText != null)
                _scoreText.gameObject.SetActive(enable);
        }
    }
}