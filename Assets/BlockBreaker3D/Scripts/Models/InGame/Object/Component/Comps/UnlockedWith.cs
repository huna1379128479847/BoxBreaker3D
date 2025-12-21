using System;
using BlockBreaker3D.Datas.Component;
using BlockBreaker3D.Datas.Signals;
using BlockBreaker3D.Models.Utils;
using BlockBreaker3D.Utils;
using UnityEngine;

namespace BlockBreaker3D.Models.InGame.Component
{
    /// <summary>
    /// UnlockWithData で定義された条件を評価し、全条件が満たされたタイミングでアクションを実行するコンポーネント。
    /// </summary>
    public class UnlockedWith : Comp
    {

        private readonly Func<GameDataHolder, IObject, (int baseScore, int baseBlocks), bool> _predicate;
        private readonly UnlockWithData _config;
        private int _baseScore;
        private int _baseBlocks;

        private bool _isUnlocked;
        private bool _actionExecuted;

        /// <summary>このインスタンスの条件が満たされているかどうか</summary>
        public bool IsUnlocked => _isUnlocked;

        public UnlockedWith(Func<GameDataHolder, IObject, (int baseScore, int baseBlocks), bool> func, UnlockWithData config) : base(true) // 複製可
        {
            _config = config;
            _predicate = func;
        }

        public override void OnStart(IObject parent, GameDataHolder dataHolder)
        {
            // ベース値の取得
            _baseScore = dataHolder.ScoreHolder.Score.Value;
            _baseBlocks = dataHolder.BoxBehaviour.Value.GetTotalBlockCount();
        }
        public override void OnUpdate(IObject parent, GameDataHolder holder, float deltaTime)
        {
            if (_isUnlocked) return;

            // 自分の条件がまだ満たされていない → 評価する
            if (!_predicate(holder, parent, (_baseScore, _baseBlocks)))
                return;

            // 条件達成
            _isUnlocked = true;

            // 全部のロックが解除されたタイミングで、一度だけアクション実行
            if (!_actionExecuted)
            {
                _actionExecuted = true;
                ExecuteUnlockAction(parent, holder);
            }
        }

        /// <summary>
        /// Unlock 完了時のアクションを実行
        /// </summary>
        private void ExecuteUnlockAction(IObject parent, GameDataHolder holder)
        {
            switch (_config.ActionOnUnlock)
            {
                case UnlockWithData.OnUnlockAction.None:
                    // 何もしない
                    break;

                case UnlockWithData.OnUnlockAction.DisableParent:
                    DisableParentObject(parent);
                    break;

                case UnlockWithData.OnUnlockAction.FireSignal:
                    FireSignalOnUnlock(holder);
                    break;

                case UnlockWithData.OnUnlockAction.AddCompforParent:
                    AddCompToParent(parent);
                    break;
            }
        }

        private void DisableParentObject(IObject p)
        {
            if (p is ObjectBase b)
                b.gameObject.SetActive(false);
        }

        private void FireSignalOnUnlock(GameDataHolder g)
        {
            if (string.IsNullOrEmpty(_config.SignalNameOnUnlock))
            {
                Debug.LogWarning("[UnlockedWith] FireSignal: SignalNameOnUnlock is null or empty.");
                return;
            }
            g.SignalBus.Fire(new Message(_config.SignalNameOnUnlock));
        }

        private void AddCompToParent(IObject p)
        {
            if (_config.CompToAdd == null)
            {
                Debug.LogWarning("[UnlockedWith] AddCompToParent: CompToAdd is null.");
                return;
            }

            try
            {
                // CompCreator を使って Comp を生成し、親に追加
                var comp = CompCreator.Create(_config.CompToAdd);
                p.AddComp(comp);
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
        public static Comp Create(CompData data)
        {
            if (data is not UnlockWithData d)
            {
                throw new ArgumentException(
                    $"Invalid CompData type for {nameof(UnlockedWith)}. " +
                    $"Expected {nameof(UnlockWithData)}, but got {data.GetType().Name ?? "null"}.");
            }

            Func<GameDataHolder, IObject, (int baseScore, int baseBlocks), bool> predicate = null;

            if (!PredicateBuilder.IsValid(d.PredicateString))
            {
                Debug.LogError(
                    $"[UnlockedWith] Invalid predicate string: '{d.PredicateString}'. " +
                    "Ensure the syntax is correct.");
                predicate = (_, __, ___) => true;
            }
            else
            {
                predicate = PredicateCompiler.CompilePredicate(d.PredicateString);
            }

            return new UnlockedWith(predicate, d);
        }
    }
}
