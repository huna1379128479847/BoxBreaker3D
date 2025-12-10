using BlockBreaker3D.Datas.Scriptable;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

namespace BlockBreaker3D.View.InGame
{
    public class TurnView : GameViewBase
    {
        [SerializeField] private TMP_Text _turnText;
        [SerializeField] private float _length = 2.0f;

        [BoxGroup("Left")]
        [SerializeField] private LineRenderer _leftLine;
        [BoxGroup("Left")]
        [SerializeField] private ColorData _lineColor;
        [BoxGroup("Right")]
        [SerializeField] private LineRenderer _rightLine;
        [BoxGroup("Right")]
        [SerializeField] private ColorData _rightLineColor;
        [BoxGroup("Effect")]
        [SerializeField] private ParticleSystem _lineEffect;
        private ObjectPool<ParticleSystem> _effectPool;

        public void UpdateAngle(Vector3 leftAngle, Vector3 rightAngle, Vector3 originalPos)
        {
            var leftDir = Quaternion.Euler(leftAngle) * Vector3.forward;
            var rightDir = Quaternion.Euler(rightAngle) * Vector3.forward;

            var leftEnd = originalPos + leftDir.normalized * _length;
            var rightEnd = originalPos + rightDir.normalized * _length;

            if (_leftLine != null)
            {
                _leftLine.positionCount = 2;
                _leftLine.colorGradient = _lineColor.gradient;
                _leftLine.SetPosition(0, originalPos);
                _leftLine.SetPosition(1, leftEnd);
            }

            if (_rightLine != null)
            {
                _rightLine.positionCount = 2;
                _rightLine.colorGradient = _rightLineColor.gradient;
                _rightLine.SetPosition(0, originalPos);
                _rightLine.SetPosition(1, rightEnd);
            }
        }

        public void SpawnEffect(bool isRight)
        {
            // TODO : lineの向きに合わせてエフェクトを回転させる
            var effect = _effectPool.Get();
            if (isRight)
            {
                if (_rightLine != null)
                {
                    effect.transform.position = _rightLine.GetPosition(0);
                    effect.transform.rotation = Quaternion.LookRotation(_rightLine.GetPosition(1) - _rightLine.GetPosition(0));
                }
            }
            else
            {
                if (_leftLine != null)
                {
                    effect.transform.position = _leftLine.GetPosition(0);
                    effect.transform.rotation = Quaternion.LookRotation(_leftLine.GetPosition(1) - _leftLine.GetPosition(0));
                }
            }
            UniTask.Create(async () =>
            {
                effect.Play();
                await UniTask.Delay(System.TimeSpan.FromSeconds(1.0f));
                _effectPool.Release(effect);
            }).Forget();
        }
        public void ClearLines()
        {
            if (_leftLine != null)
            {
                _leftLine.positionCount = 0;
            }
            if (_rightLine != null)
            {
                _rightLine.positionCount = 0;
            }
        }
        public void SetText(string text)
        {
            if (_turnText != null)
                _turnText.SetText(text);
        }

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
            _turnText?.gameObject.SetActive(isActive);
            _leftLine.enabled = isActive;
            _rightLine.enabled = isActive;
        }

        private void Awake()
        {
            _effectPool = new ObjectPool<ParticleSystem>(() =>
            {
                var effect = Instantiate(_lineEffect);
                effect.gameObject.SetActive(false);
                return effect;
            },
            effect =>
            {
                effect.gameObject.SetActive(true);
            },
            effect =>
            {
                effect.gameObject.SetActive(false);
            },
            effect =>
            {
                Destroy(effect.gameObject);
            },
            false, 10, 100);
        }

        private void OnDestroy()
        {
            if (_leftLine != null)
            {
                _leftLine.material = null; // Avoid memory leak
            }
            if (_rightLine != null)
            {
                _rightLine.material = null; // Avoid memory leak
            }
        }
    }
}
