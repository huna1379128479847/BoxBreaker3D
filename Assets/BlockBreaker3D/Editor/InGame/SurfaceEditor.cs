using BlockBreaker3D.Datas;
using BlockBreaker3D.Models;
using UnityEditor;
using UnityEngine;

namespace BlockBreaker3D.Editor
{
    [CustomEditor(typeof(SurfaceBehaviour))]
    public class SurfaceEditor : UnityEditor.Editor
    {
        private SurfaceBehaviour _surface;
        private SerializedProperty _surfaceSize;
        private SerializedProperty _surfaceType;
        private SerializedProperty _surfaceOrigin;
        private SerializedProperty _spawnPos;
        private SerializedProperty _blockRemainTextPos;

        private void OnEnable()
        {
            _surface = (SurfaceBehaviour)target;
            _surfaceSize = serializedObject.FindProperty("_size");
            _surfaceType = serializedObject.FindProperty("_surfaceType");
            _surfaceOrigin = serializedObject.FindProperty("_surfaceOriginPos");
            _spawnPos = serializedObject.FindProperty("_spawnPos");
            _blockRemainTextPos = serializedObject.FindProperty("_blockRemainTextPos");
        }

        private void OnSceneGUI()
        {
            // シリアライズデータを最新状態に
            serializedObject.Update();

            // 面の「up / right」を取得
            var (up, right) = Surface.DefaultMove(_surfaceType.stringValue);
            // Use the configured surface origin instead of the transform position
            var origin = _surfaceOrigin.vector3Value;

            // Handle to move the surface origin
            EditorGUI.BeginChangeCheck();
            var newOrigin = Handles.PositionHandle(origin, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_surface, "Move Surface Origin");
                _surfaceOrigin.vector3Value = newOrigin;
                origin = newOrigin;
            }

            // 現在のスポーン位置（ローカル2D）をワールド座標に変換
            var spawn2D = _spawnPos.vector2Value;
            var spawnWorldPos = origin + right * spawn2D.x + up * spawn2D.y;

            // ハンドル描画・操作 (2D handle on surface plane)
            Handles.color = Color.green;
            EditorGUI.BeginChangeCheck();
            var newSpawnWorldPos = Handles.PositionHandle(spawnWorldPos, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_surface, "Move Spawn Position");
                // project into surface local coords
                var local = newSpawnWorldPos - origin;
                float x = Vector3.Dot(local, right);
                float y = Vector3.Dot(local, up);
                _spawnPos.vector2Value = new Vector2(x, y);
            }

            // 矢印はそのまま表示
            Handles.ArrowHandleCap(0, spawnWorldPos, Quaternion.LookRotation(up), 0.5f, EventType.Repaint);

            // サイズ調整用のハンドル（右上コーナー）。origin を基準に右方向と上方向に広がるように扱う（原点は左下）。
            var size = _surfaceSize.vector2Value;
            var topRight = origin + right * size.x + up * size.y;

            // 2D handle for resizing (drag top-right corner in plane)
            EditorGUI.BeginChangeCheck();
            var newTopRight = Handles.PositionHandle(topRight, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_surface, "Resize Surface");
                var local = newTopRight - origin;
                var newX = Vector3.Dot(local, right);
                var newY = Vector3.Dot(local, up);
                // Ensure sizes are positive
                newX = Mathf.Max(0f, newX);
                newY = Mathf.Max(0f, newY);
                _surfaceSize.vector2Value = new Vector2(newX, newY);
            }

            // Block remain text position handle (yellow circle)
            var text2D = _blockRemainTextPos.vector2Value;
            var textWorld = origin + right * text2D.x + up * text2D.y;
            Handles.color = Color.yellow;
            EditorGUI.BeginChangeCheck();
            // FreeMoveHandle returns new position after dragging; constrain to plane by projecting to basis
            var fmh_94_66_639010066713347153 = Quaternion.identity; var newTextWorld = Handles.FreeMoveHandle(textWorld, 0.25f, Vector3.zero, Handles.CircleHandleCap);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_surface, "Move Block Remain Text");
                var local = newTextWorld - origin;
                float x = Vector3.Dot(local, right);
                float y = Vector3.Dot(local, up);
                _blockRemainTextPos.vector2Value = new Vector2(x, y);
            }

            // ここまでの変更を反映
            serializedObject.ApplyModifiedProperties();

            // ついでに、面のサイズを枠線で描く例（任意）
            DrawSurfaceRect(origin, up, right);
        }

        private void DrawSurfaceRect(Vector3 origin, Vector3 up, Vector3 right)
        {
            var size = _surfaceSize.vector2Value;

            var p0 = origin;
            var p1 = origin + right * size.x;
            var p2 = origin + right * size.x + up * size.y;
            var p3 = origin + up * size.y;

            Handles.color = Color.yellow;
            Handles.DrawLine(p0, p1);
            Handles.DrawLine(p1, p2);
            Handles.DrawLine(p2, p3);
            Handles.DrawLine(p3, p0);
        }
    }
}