namespace BlockBreaker3D.Datas.Signals
{
    public sealed class GameSignal
    {
        public enum Type
        {
            // ゲーム状態変更系
            GameStarted,
            Pause,
            Resume,
            GameOver,
            LevelCompleted,
            Restart,
            GameClear,
            Exit,

            // 特定のシステムへの通知系
            RequestRespawn, // To Box, リスポーン要求
            DespawnBall,   // To Ball, ボールの消滅要求
            SpawnBall,

            // 主にViewModelへの通知系
            SurfaceBlockCleared, //  サーフェス上のブロックが全て消えた

            // マネージャーへの通知系
            BlocksAllCleared,
        }

        public Type SignalType { get; }

        public GameSignal(Type signalType)
        {
            SignalType = signalType;
        }

        public bool Has(Type type)
        {
            return SignalType == type;
        }

        public bool HasAny(params Type[] types)
        {
            foreach (var type in types)
            {
                if (SignalType == type)
                    return true;
            }
            return false;
        }
    }
}
