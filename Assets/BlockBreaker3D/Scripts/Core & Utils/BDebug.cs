using System.Runtime.CompilerServices;

namespace BlockBreaker3D.Utils
{
    public static class BDebug
    {
        public enum BColor
        {
            red,
            green,
            blue,
            yellow,
            cyan,
            magenta,
            white,
        }

        public static void Log(string message, BColor col = BColor.white, [CallerMemberName] string caller = "")
        {
            UnityEngine.Debug.Log($"[{caller.ColorText(col)}]{message}");
        }

        public static void Warn(string message, BColor col = BColor.white, [CallerMemberName] string caller = "")
        {
            UnityEngine.Debug.LogWarning($"[{caller.ColorText(col)}]{message}");
        }

        public static void Error(string message, BColor col = BColor.white, [CallerMemberName] string caller = "")
        {
            UnityEngine.Debug.LogError($"[{caller.ColorText(col)}]{message}");
        }

        // コンディションがfalseの場合にエラーログを出力し、falseを返す
        public static bool Assert(bool condition, string message, BColor col = BColor.white, [CallerMemberName] string caller = "")
        {
            if (!condition)
            {
                UnityEngine.Debug.LogError($"[{caller.ColorText(col)}]{message}");
                return false;
            }
            return true;
        }

        public static string ColorText(this string message, BColor col)
        {
            return $"<color={col}>{message}</color>";
        }
    }
}