using BlockBreaker3D.Datas;
using BlockBreaker3D.Models;
using UnityEditor;
using UnityEngine;

namespace BlockBreaker3D.Editor
{
    internal static class SurfaceGizmoDrawer
    {
        [DrawGizmo(GizmoType.NonSelected | GizmoType.Pickable)]
        private static void DrawNonSelected(SurfaceBehaviour surface, GizmoType gizmoType)
        {
            if (!SurfaceGizmoSettings.ShowNonSelectedFrame) return;

            // SurfaceEditor と同じロジックで up/right と origin/size を取る
            var so = new SerializedObject(surface);

            var surfaceType = so.FindProperty("_surfaceType")?.stringValue;
            var origin = so.FindProperty("_surfaceOriginPos")?.vector3Value ?? surface.transform.position;
            var size = so.FindProperty("_size")?.vector2Value ?? Vector2.one;

            var (up, right) = Surface.DefaultMove(surfaceType);
            if (up == Vector3.zero || right == Vector3.zero)
            {
                Handles.Label(origin, "Surface Up/Right Vectors are invalid!");
                return;
            }

            // 枠線描画
            var p0 = origin;
            var p1 = origin + right * size.x;
            var p2 = origin + right * size.x + up * size.y;
            var p3 = origin + up * size.y;

            // 非選択は控えめに
            Handles.color = new Color(1f, 1f, 0f, 0.35f);
            Handles.DrawLine(p0, p1);
            Handles.DrawLine(p1, p2);
            Handles.DrawLine(p2, p3);
            Handles.DrawLine(p3, p0);
        }
    }
}
