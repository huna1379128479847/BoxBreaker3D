using BlockBreaker3D.Datas.Scriptable;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace BlockBreaker3D.View.InGame
{
    // 演出のパドルを入力に応じて左右の端へ移動し、その後中央の戻る
    // ボールには干渉しない
    public class PaddleView : MonoBehaviour
    {
        [BoxGroup("References")]
        [SerializeField] private RectTransform _paddleTransform;
        [BoxGroup("References")]
        [BoxGroup("Left")]
        [SerializeField] private TMP_Text _left;
        [BoxGroup("Left")]
        [SerializeField] private ColorData _leftColor;
        [BoxGroup("References")]
        [BoxGroup("Right")]
        [SerializeField] private TMP_Text _right;
        [BoxGroup("Right")]
        [SerializeField] private ColorData _rightColor;

        [BoxGroup("Paddle Move Settings")]
        [SerializeField] private float _paddleSideMoveLength = 80f;
        [BoxGroup("Paddle Move Settings")]
        [SerializeField] private float _paddleMoveDuration = 0.50f;
        [BoxGroup("Paddle Move Settings")]
        [SerializeField] private float _paddleReturnDuration = 1.8f;
        private Vector3 _initPos;
        private Sequence _sq;

        public void MoveToSide(bool isRight)
        {
            _sq?.Kill();
            Vector2 targetPos = isRight ?
                new Vector2(_initPos.x + _paddleSideMoveLength, _initPos.y) :
                new Vector2(_initPos.x - _paddleSideMoveLength, _initPos.y);
            _sq = DOTween.Sequence();
            _sq.Append(_paddleTransform.DOAnchorPos(targetPos, _paddleMoveDuration).SetEase(Ease.Linear));
            _sq.Append(_paddleTransform.DOAnchorPos(_initPos, _paddleReturnDuration).SetEase(Ease.Linear));
            _sq.Play();
        }

        // Note: signature matches new IGameView Enable(bool) pattern when this is used as a view
        public void Enable(bool enable)
        {
            _left.gameObject.SetActive(enable);
            _right.gameObject.SetActive(enable);
            _paddleTransform.gameObject.SetActive(enable);
        }
        private void Awake()
        {
            _initPos = _paddleTransform.anchoredPosition;
            _left.color = _leftColor.color;
            _right.color = _rightColor.color;
        }
        private void OnDisable()
        {
            _sq?.Kill();
            _paddleTransform.anchoredPosition = _initPos;
        }
    }
}