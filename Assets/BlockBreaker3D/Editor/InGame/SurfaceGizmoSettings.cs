using UnityEditor;

namespace BlockBreaker3D.Editor
{
    internal static class SurfaceGizmoSettings
    {
        private const string Key = "BlockBreaker3D.Surface.ShowNonSelectedFrame";

        public static bool ShowNonSelectedFrame
        {
            get => EditorPrefs.GetBool(Key, true);
            set => EditorPrefs.SetBool(Key, value);
        }

        [MenuItem("Tools/BlockBreaker3D/Surface Gizmos/Show Non-Selected Surface Frames")]
        private static void Toggle()
        {
            ShowNonSelectedFrame = !ShowNonSelectedFrame;
            SceneView.RepaintAll();
        }

        [MenuItem("Tools/BlockBreaker3D/Surface Gizmos/Show Non-Selected Surface Frames", true)]
        private static bool ToggleValidate()
        {
            Menu.SetChecked("Tools/BlockBreaker3D/Surface Gizmos/Show Non-Selected Surface Frames", ShowNonSelectedFrame);
            return true;
        }
    }
}
