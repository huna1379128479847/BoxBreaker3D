using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace BlockBreaker3D.View.InGame
{
    public class GameOverView : AbstractScoreView
    {
        [SerializeField] private GameObject _panel; // オンオフ切り替え用
        [SerializeField] private TMP_Text _score;
        private Sequence _animSq;

        public override void Enable(bool enable)
        {
            base.Enable(enable);
            if (_panel != null)
                _panel.gameObject.SetActive(enable);
        }

        public override async UniTask PlayGameOverAnim()
        {
            if (_score == null)
                return;

            // Stop previous animation
            _animSq?.Kill();

            // Initialize visual state
            _score.rectTransform.localScale = Vector3.zero;
            var col = _score.color;
            col.a = 0f;
            _score.color = col;

            // Build animation: pop-in scale with fade-in
            _animSq = DOTween.Sequence();
            _ = _animSq.Append(_score.rectTransform.DOScale(Vector3.one * 1.2f, 0.40f).SetEase(Ease.OutBack));
            _ = _animSq.Join(DOTween.To(() => _score.color, x => _score.color = x, new Color(col.r, col.g, col.b, 1f), 0.40f));
            _ = _animSq.Append(_score.rectTransform.DOScale(Vector3.one, 0.18f).SetEase(Ease.Linear));
            await _animSq.Play().AsyncWaitForCompletion().AsUniTask();
        }

        public override void UpdateScore(int newScore)
        {
            if (_score != null)
            {
                _score.text = $"Lumen:{newScore.ToString()} lu";
            }
        }
    }
}