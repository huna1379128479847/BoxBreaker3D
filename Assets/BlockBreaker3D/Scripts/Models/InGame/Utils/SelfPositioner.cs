using BlockBreaker3D.Datas;
using BlockBreaker3D.Datas.Scriptable;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace BlockBreaker3D.Models.InGame.Utils
{
    public class SelfPositioner : MonoBehaviour
    {
#if UNITY_EDITOR
        // 対象オブジェクト自身の参照
        [SerializeField] private ObjectBase _targetObject;
        [SerializeField] private SurfaceBehaviour _surface;
        [SerializeField, ReadOnly] private Vector2 _position;

        [BoxGroup("Grid")]
        [SerializeField] private Vector2Int _positionGrid;
        [BoxGroup("Grid")]
        [SerializeField, MinValue(1)] private Vector3Int _sizeGrid;
        [BoxGroup("Grid")]
        [SerializeField] private bool _autoPerformOnValidate = true;

        private static StepData _cache;

        public Vector2 Position { get => _position; set => _position = value; }
        public Vector2Int PositionGrid { get => _positionGrid; set => _positionGrid = value; }
        public Vector3Int SizeGrid { get => _sizeGrid; set => _sizeGrid = value; }

        private void OnValidate()
        {
            if (_autoPerformOnValidate)
            {
                PerformPositioning();
            }
        }

        private void Reset()
        {
            _targetObject = GetComponent<ObjectBase>();
            _surface = GetComponentInParent<SurfaceBehaviour>();
            _position = Vector2.one;
            _positionGrid = Vector2Int.one;
            _sizeGrid = Vector3Int.one;
            _autoPerformOnValidate = true;
        }

        [Button("Perform Positioning")]
        private void PerformPositioning()
        {
            if (_targetObject == null || _surface == null)
            {
                Debug.LogWarning("TargetObject or Surface is not assigned.");
                return;
            }
            ReadCache();
            // グリッドの位置とサイズから実際の位置とスケールを計算して適用
            float x = _positionGrid.x * _cache.QuarterStep;
            float y = _positionGrid.y * _cache.QuarterStep;
            _position = new Vector2(x, y);

            float scaleX = (_sizeGrid.x >= 1 ? _sizeGrid.x : 1) * _cache.Step;
            float scaleY = (_sizeGrid.y >= 1 ? _sizeGrid.y : 1) * _cache.Step;
            float scaleZ = (_sizeGrid.z >= 1 ? _sizeGrid.z : 1) * _cache.Step;
            transform.localScale = new Vector3(scaleX, scaleY, scaleZ);

            // Surface 上の位置をワールド座標に変換して適用
            var origin = _surface.SurfaceOrigin;
            var (up, right) = Surface.DefaultMove(_surface.SurfaceType);
            var worldPos = origin + right * _position.x + up * _position.y;
            _targetObject.transform.position = worldPos;
        }

        [UnityEditor.MenuItem("Tools/BlockBreaker3D/StepReset")]
        public static void StepReset()
        {
            // Stepを変更した場合に呼び出して、全てのSelfPositionerを更新
            var positioners = FindObjectsByType<SelfPositioner>(FindObjectsSortMode.InstanceID);
            if (positioners.Length == 0)
            {
                Debug.Log("[<color=green>BoxBreker3D</color>] 対象のSelfPositionerは存在しません。");
                return;
            }

            ReadCache();
            Undo.RecordObjects(positioners, "Step Reset"); // Undo対応
            foreach (var p in positioners)
            {
                if (p == null || p._surface == null)
                {
                    Debug.LogWarning("[BoxBreker3D] Surface が未設定の SelfPositioner があるためスキップしました。");
                    continue;
                }

                // SurfaceType(文字列)からSurfaceを作って、面ローカル2Dへ逆変換する
                // 位置の参照は _targetObject があればそちらを優先（PerformPositioning が動かしているのは基本こっち）
                var surface = new Surface(p._surface.SurfaceType);
                var origin = p._surface.SurfaceOrigin;

                var targetTransform = (p._targetObject != null) ? p._targetObject.transform : p.transform;
                var local2D = surface.WorldToLocal(origin, targetTransform.position);

                // local2D は「HalfStep単位の実距離」なので、HalfStepで割ってグリッドへ
                p.PositionGrid = new Vector2Int(
                    Mathf.RoundToInt(local2D.x / _cache.QuarterStep),
                    Mathf.RoundToInt(local2D.y / _cache.QuarterStep)
                );

                // サイズは Step単位
                // ※PerformPositioning は SelfPositioner の transform.localScale を設定しているので、ここも同じ参照に合わせる
                p.SizeGrid = new Vector3Int(
                    Mathf.Max(1, Mathf.RoundToInt(p.transform.localScale.x / _cache.Step)),
                    Mathf.Max(1, Mathf.RoundToInt(p.transform.localScale.y / _cache.Step)),
                    Mathf.Max(1, Mathf.RoundToInt(p.transform.localScale.z / _cache.Step))
                );

                p.PerformPositioning();
            }

            Debug.Log("[<color=green>BoxBreker3D</color>] Stepの変更成功 ずれがないか確認してください。");
        }


        //[UnityEditor.MenuItem("Tools/BlockBreaker3D/ConvertPos")]
        //public static void ConvertPos()
        //{
        //    // Stepを変更した場合に呼び出して、全てのSelfPositionerを更新
        //    var positioners = FindObjectsByType<SelfPositioner>(FindObjectsSortMode.InstanceID);
        //    if (positioners.Length == 0)
        //    {
        //        Debug.Log("[<color=green>BoxBreker3D</color>] 対象のSelfPositionerは存在しません。");
        //        return;
        //    }
        //    ReadCache();
        //    foreach (var p in positioners)
        //    {
        //        p.PositionGrid /= 2; // Stepが半分になったので位置を2倍に
        //    }
        //    Debug.Log("[<color=green>BoxBreker3D</color>] Stepの変更成功 ずれがないか確認してください。");
        //}

        private static void ReadCache()
        {
            _cache ??= UnityEditor.AssetDatabase.LoadAssetAtPath<StepData>(StepData.PATH);
            if (_cache == null)
            {
                _cache = ScriptableObject.CreateInstance<StepData>();
                AssetDatabase.CreateAsset(_cache, StepData.PATH);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
#endif
        // 実行時にはオブジェクトを破棄
        private void Awake()
        {
            Destroy(this);
        }
    }
}