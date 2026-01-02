using HighElixir.Unity;
using System.Runtime.CompilerServices;
using Unity.Logging;
using UnityEngine;
using ULog = Unity.Logging.Log;

namespace BlockBreaker3D.Utils
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

    /// <summary>
    /// Loggingを使ってパフォーマンス改善したデバッグログクラス <br/>
    /// 文字数が多いときに例外を返す仕様への改善方法がわからなかったため、文字数が多い場合は通常のUnityEngine.Debug.Logにフォールバックします。 <br/>
    /// </summary>
    [HideInStackTrace] // Loggingで使われるスタックトレースからこのクラスを隠す
    public static class LogPainter
    {
        [HideInCallstack] // このメソッドをコールスタックから隠す
        public static void Info(string message, BColor col = BColor.white, [CallerMemberName] string caller = "")
        {
            var str = GetRunes(caller, col, message);
            try
            {
                ULog.Info(str);
            }
            catch
            {
                // 文字数が多い場合などに例外が発生するため、通常のログにフォールバック
                UnityEngine.Debug.Log(str);
            }
        }
        // インターバル付き情報ログ(フレーム依存)
        [HideInCallstack]
        public static void Info(int interval, string message, BColor col = BColor.white, [CallerMemberName] string caller = "", [CallerFilePath] string path = "", [CallerLineNumber] int lineNumber = 0)
        {
            if (Interval.Check(interval, caller, path, lineNumber))
                Info(message, col, caller);
        }
        [HideInCallstack]
        public static void Debug(string message, BColor col = BColor.white, [CallerMemberName] string caller = "")
        {
            var str = GetRunes(caller, col, message);
            try
            {
                ULog.Debug(str);
            }
            catch
            {
                UnityEngine.Debug.Log(str);
            }
        }
        [HideInCallstack]
        public static void Warn(string message, BColor col = BColor.white, [CallerMemberName] string caller = "")
        {
            var str = GetRunes(caller, col, message);
            try
            {
                ULog.Warning(str);
            }
            catch
            {
                UnityEngine.Debug.LogWarning(str);
            }
        }
        [HideInCallstack]
        public static void Error(string message, BColor col = BColor.white, [CallerMemberName] string caller = "")
        {
            var str = GetRunes(caller, col, message);
            try
            {
                ULog.Error(str);
            }
            catch
            {
                UnityEngine.Debug.LogError(str);
            }
        }

        // コンディションがfalseの場合にエラーログを出力し、falseを返す
        [HideInCallstack]
        public static bool Assert(bool condition, string message, BColor col = BColor.white, bool loggingOnlyEditor = true, [CallerMemberName] string caller = "")
        {
#if !UNITY_EDITOR
            // エディター上でのみログを出力する場合
            if (loggingOnlyEditor)
            {
                return condition;
            }
#endif
            if (!condition) // コンディションを満たさなかった場合
            {
                Error(message, col, caller);
                return false;
            }
            return true;
        }

        [HideInCallstack]
        public static string Paint(this string message, BColor col)
        {
            return $"<color={col}>{message}</color>";
        }

        [HideInCallstack]

        private static string GetRunes(string caller, BColor col, string message)
        {
            return string.Format("[{0}]{1}", caller.Paint(col), message);
        }

        static LogPainter()
        {
            // ロガー設定の作成
            var conf = new Unity.Logging.LoggerConfig();
            conf.WriteTo.ResolveMinLevel(LogLevel.Debug);
            conf.OutputTemplate("{Level} | {Message} [{Timestamp}]{NewLine}{Stacktrace}");
            // セットアップ
            ULog.Logger = conf.CreateLogger();
        }
    }
}