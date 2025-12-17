using BlockBreaker3D.Datas.Scriptable;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

namespace BlockBreaker3D.View.InGame
{
    // ターン予測線を表示するビュー
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
        [BoxGroup("Effect")]
        [SerializeField] private float _effectDuration = 1.0f;
        private ObjectPool<ParticleSystem> _effectPool;

        [SerializeField] private bool _enabled = true;
        public void UpdateAngle(Vector3 leftAngle, Vector3 rightAngle, Vector3 originalPos)
        {
            if (!_enabled) return;
            var leftDir = Quaternion.Euler(leftAngle) * Vector3.forward;
            var rightDir = Quaternion.Euler(rightAngle) * Vector3.forward;

            var leftEnd = originalPos + leftDir.normalized * _length;
            var rightEnd = originalPos + rightDir.normalized * _length;

            if (_leftLine != null && _leftLine.gameObject.activeInHierarchy)
            {
                _leftLine.positionCount = 2;
                _leftLine.colorGradient = _lineColor.gradient;
                _leftLine.SetPosition(0, originalPos);
                _leftLine.SetPosition(1, leftEnd);
            }

            if (_rightLine != null && _rightLine.gameObject.activeInHierarchy)
            {
                _rightLine.positionCount = 2;
                _rightLine.colorGradient = _rightLineColor.gradient;
                _rightLine.SetPosition(0, originalPos);
                _rightLine.SetPosition(1, rightEnd);
            }
        }

        // 左右のラインの表示・非表示を切り替え
        // 左右どちらか一方のみ表示する場合に使用
        // Enable(false) 時は無視される
        public void Enable_Left(bool enable)
        {
            if (_enabled)
                _leftLine.enabled = enable;
        }

        public void Enable_Right(bool enable)
        {
            if (_enabled)
                _rightLine.enabled = enable;
        }

        public void SpawnEffect(bool isRight)
        {
            // TODO : lineの向きに合わせてエフェクトを回転させる
            if (_effectPool == null) InitPool();
            var effect = _effectPool.Get();
            if (isRight)
            {
                if (_rightLine != null && _rightLine.positionCount > 0)
                {
                    effect.transform.position = _rightLine.GetPosition(0);
                    effect.transform.rotation = Quaternion.LookRotation(_rightLine.GetPosition(1) - _rightLine.GetPosition(0));
                }
                else return; // 表示されていない場合はエフェクトも出さない
            }
            else
            {
                if (_leftLine != null && _leftLine.positionCount > 0)
                {
                    effect.transform.position = _leftLine.GetPosition(0);
                    effect.transform.rotation = Quaternion.LookRotation(_leftLine.GetPosition(1) - _leftLine.GetPosition(0));
                }
                else return; // 表示されていない場合はエフェクトも出さない
            }
            UniTask.Create(async () =>
            {
                effect.Play();
                var d = effect.main.loop ? _effectDuration : effect.main.duration;
                await UniTask.Delay(System.TimeSpan.FromSeconds(d));
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

        // Implement IGameView style enable/disable
        public override void Enable(bool enable)
        {
            base.Enable(enable);
            _turnText?.gameObject.SetActive(enable);
            _enabled = enable;
            if (_leftLine != null) _leftLine.enabled = enable;
            if (_rightLine != null) _rightLine.enabled = enable;
        }

        private void Awake()
        {
            if (_effectPool == null) InitPool();
        }

        private void InitPool()
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
