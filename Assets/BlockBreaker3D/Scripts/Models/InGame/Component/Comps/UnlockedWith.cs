using System;
using BlockBreaker3D.Datas.Component;
using UnityEngine;

namespace BlockBreaker3D.Models.InGame.Component
{
    /// <summary>
    /// UnlockWithData で定義された条件を評価し、
    /// 同一 IObject 上の UnlockedWith とロック数を共有して、
    /// 全条件が満たされたタイミングでアクションを実行するコンポーネント。
    /// </summary>
    public class UnlockedWith : Comp
    {
        /// <summary>
        /// 同じ IObject 上の UnlockedWith 間で共有される状態
        /// </summary>
        public class Shared
        {
            /// <summary>まだ満たされていないロック条件の数</summary>
            public int RemainingLocks;

            /// <summary>すべてのロックが解除されていれば true</summary>
            public bool AllowUnlock => RemainingLocks <= 0;
        }

        private readonly Func<bool> _predicate;
        private readonly UnlockWithData _config;
        private readonly GameDataHolder _holder;
        private readonly IObject _parent;

        private Shared _shared;
        private bool _isUnlocked;
        private bool _actionExecuted;

        /// <summary>このグループ全体の共有状態</summary>
        public Shared SharedCondition => _shared;

        /// <summary>このインスタンスの条件が満たされているかどうか</summary>
        public bool IsUnlocked => _isUnlocked;

        public UnlockedWith(
            Func<bool> predicate,
            UnlockWithData config,
            GameDataHolder holder,
            IObject parent
        ) : base(true) // 初期コンポーネント扱い
        {
            _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _holder = holder ?? throw new ArgumentNullException(nameof(holder));
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        public override void OnStart()
        {
            // 同じ IObject 上の UnlockedWith から Shared を共有する
            foreach (var c in _parent.GetComps<UnlockedWith>())
            {
                if (c == this) continue;
                if (c._shared != null)
                {
                    _shared = c._shared;
                    break;
                }
            }

            _shared ??= new Shared();

            // このインスタンス分のロックを追加
            _shared.RemainingLocks++;
        }

        public override void OnUpdate(float deltaTime)
        {
            if (_shared == null || _isUnlocked)
                return;

            // 自分の条件がまだ満たされていない → 評価する
            if (!_predicate())
                return;

            // 条件達成
            _isUnlocked = true;
            _shared.RemainingLocks--;

            if (_shared.RemainingLocks < 0)
                _shared.RemainingLocks = 0;

            // 全部のロックが解除されたタイミングで、一度だけアクション実行
            if (_shared.AllowUnlock && !_actionExecuted)
            {
                _actionExecuted = true;
                ExecuteUnlockAction();
            }
        }

        public override void OnRemove()
        {
            // 削除時にまだアンロックされていなければロック数を補正
            if (_shared != null && !_isUnlocked)
            {
                _shared.RemainingLocks--;
                if (_shared.RemainingLocks < 0)
                    _shared.RemainingLocks = 0;
            }
        }

        /// <summary>
        /// Unlock 完了時のアクションを実行
        /// </summary>
        private void ExecuteUnlockAction()
        {
            switch (_config.ActionOnUnlock)
            {
                case UnlockWithData.OnUnlockAction.None:
                    // 何もしない
                    break;

                case UnlockWithData.OnUnlockAction.DisableParent:
                    DisableParentObject();
                    break;

                case UnlockWithData.OnUnlockAction.FireSignal:
                    FireSignalOnUnlock();
                    break;

                case UnlockWithData.OnUnlockAction.AddCompforParent:
                    AddCompToParent();
                    break;
            }
        }

        private void DisableParentObject()
        {
            // IObject の実装は MonoBehaviour を継承している前提
            if (_parent is ObjectBase b)
                b.gameObject.SetActive(false);
        }

        private void FireSignalOnUnlock()
        {
            if (string.IsNullOrEmpty(_config.SignalNameOnUnlock))
            {
                Debug.LogWarning("[UnlockedWith] FireSignal: SignalNameOnUnlock is null or empty.");
                return;
            }

            // ここは君の SignalBus / イベントシステムに接続してね
            // ひとまずログだけ出しておく
            Debug.Log($"[UnlockedWith] FireSignal: {_config.SignalNameOnUnlock} (parent: {_parent})");

            // 例）もし GameDataHolder に SignalBus があるなら、こんな感じに繋げる想定：
            // _holder.SignalBus.Fire(new NamedUnlockSignal(_config.SignalNameOnUnlock, _parent));
        }

        private void AddCompToParent()
        {
            if (_config.CompToAdd == null)
            {
                Debug.LogWarning("[UnlockedWith] AddCompToParent: CompToAdd is null.");
                return;
            }

            try
            {
                // CompCreator を使って Comp を生成し、親に追加
                var comp = CompCreator.Create(_config.CompToAdd, _holder, _parent);
                _parent.AddComp(comp);
            }
            catch (Exception ex)
            {
                Debug.LogError(
                    $"[UnlockedWith] Failed to add Comp to parent. CompData={_config.CompToAdd.GetType().Name}\n{ex}");
            }
        }

        /// <summary>
        /// CompCreator から呼ばれるファクトリメソッド
        /// </summary>
        public static Comp Create(CompData data, GameDataHolder holder, IObject parent)
        {
            if (data is not UnlockWithData d)
            {
                throw new ArgumentException(
                    $"Invalid CompData type for {nameof(UnlockedWith)}. " +
                    $"Expected {nameof(UnlockWithData)}, but got {data?.GetType().Name ?? "null"}.");
            }

            if (holder == null) throw new ArgumentNullException(nameof(holder));
            if (parent == null) throw new ArgumentNullException(nameof(parent));

            // --- スコア条件 ---
            Func<bool> scoreCond;
            if (d.NeedScore)
            {
                scoreCond = () =>
                {
                    
                    var scoreHolder = holder.ScoreHolder;
                    var score = scoreHolder.Score.Value;
                    Debug.Log($"[UnlockedWith] Checking score condition: NeedScore={d.NeedScore}, RequiredScore={d.RequiredScore}, IsGreatThanOrEqual={d.IsGreatThanOrEqual}, CurrentScore={score}");
                    return d.IsGreatThanOrEqual
                        ? score >= d.RequiredScore
                        : score < d.RequiredScore;
                };
            }
            else
            {
                scoreCond = () => true;
            }

            // --- ブロック数条件 ---
            Func<bool> blockCond;
            if (d.NeedBlocks)
            {
                blockCond = () =>
                {
                    var box = holder.BoxBehaviour.Value;
                    if (box == null)
                    {
                        // Box が存在しない場合は条件未達成扱い
                        return false;
                    }
                    var blocks = box.GetTotalBlockCount();
                    Debug.Log($"[UnlockedWith] Checking block condition: NeedBlocks={d.NeedBlocks}, RequiredBlocks={d.RequiredBlocks}, IsGreatThanOrEqualBlocks={d.IsGreatThanOrEqualBlocks}, CurrentBlocks={blocks}");
                    return d.IsGreatThanOrEqualBlocks
                        ? blocks >= d.RequiredBlocks
                        : blocks < d.RequiredBlocks;
                };
            }
            else
            {
                blockCond = () => true;
            }

            // 両方の条件を AND で結合
            Func<bool> predicate = () => scoreCond() && blockCond();

            return new UnlockedWith(predicate, d, holder, parent);
        }
    }
}
