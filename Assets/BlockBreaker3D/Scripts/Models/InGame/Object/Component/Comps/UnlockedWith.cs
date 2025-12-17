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

        private readonly Func<bool> _predicate;
        private readonly UnlockWithData _config;
        private readonly GameDataHolder _holder;
        private readonly IObject _parent;

        private bool _isUnlocked;
        private bool _actionExecuted;

        /// <summary>このインスタンスの条件が満たされているかどうか</summary>
        public bool IsUnlocked => _isUnlocked;

        public UnlockedWith(
            Func<bool> predicate,
            UnlockWithData config,
            GameDataHolder holder,
            IObject parent
        ) : base(true) // 複製可
        {
            _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _holder = holder ?? throw new ArgumentNullException(nameof(holder));
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }
        public override void OnUpdate(float deltaTime)
        {
            if (_isUnlocked) return;

            // 自分の条件がまだ満たされていない → 評価する
            if (!_predicate())
                return;

            // 条件達成
            _isUnlocked = true;

            // 全部のロックが解除されたタイミングで、一度だけアクション実行
            if (!_actionExecuted)
            {
                _actionExecuted = true;
                ExecuteUnlockAction();
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
            _holder.SignalBus.Fire(new Message(_config.SignalNameOnUnlock));
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
                    $"Expected {nameof(UnlockWithData)}, but got {data.GetType().Name ?? "null"}.");
            }

            if (holder == null) throw new ArgumentNullException(nameof(holder));
            if (parent == null) throw new ArgumentNullException(nameof(parent));

            Func<bool> predicate = null;

            if (!PredicateBuilder.IsValid(d.PredicateString))
            {
                Debug.LogError(
                    $"[UnlockedWith] Invalid predicate string: '{d.PredicateString}'. " +
                    "Ensure the syntax is correct.");
                predicate = () => true;
            }
            else
            {
                predicate = holder.CompilePredicate(parent, d.PredicateString);
            }

            return new UnlockedWith(predicate, d, holder, parent);
        }
    }
}
