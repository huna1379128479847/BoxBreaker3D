using UnityEngine;

namespace HighElixir
{
    /// <summary>
    ///　小さなUnityのデバッグクラスのラッパー
    ///　全てのメソッドはUNITY_EDITORでのみ動作します
    /// </summary>
    public static class HEDebug
    {
        public static bool Assert(bool condition, string message = "Assertion Failed")
        {
#if UNITY_EDITOR
            if (!condition)
            {
                Debug.Assert(false, message);
                UnityEngine.Debug.LogError(message);
            }
            return !condition;
#else
            return condition;
#endif
        }
    }
}