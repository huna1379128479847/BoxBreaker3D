using UnityEngine;

namespace BlockBreaker3D.Datas.Component
{
    /// <summary>
    /// スコア / ブロック数を条件としたアンロック定義
    /// </summary>
    [CreateAssetMenu(
        fileName = "UnlockWithData",
        menuName = "BlockBreaker3D/Component/UnlockWithData")]
    public class UnlockWithData : CompData
    {
        public enum OnUnlockAction
        {
            None,
            DisableParent,
            FireSignal,
            AddCompforParent,
        }

        [Header("Score Condition")]
        [Tooltip("スコアでアンロック条件を使うかどうか")]
        public bool NeedScore;

        [Tooltip("必要なスコア量")]
        public int RequiredScore;

        [Tooltip("true: スコアが以上でアンロック / false: スコアが未満でアンロック")]
        public bool IsGreatThanOrEqual;

        [Header("Block Condition")]
        [Tooltip("残りブロック数でアンロック条件を使うかどうか")]
        public bool NeedBlocks;

        [Tooltip("条件となるブロック数")]
        public int RequiredBlocks;

        [Tooltip("true: ブロック数が以上でアンロック / false: ブロック数が未満でアンロック")]
        public bool IsGreatThanOrEqualBlocks;

        [Tooltip("アンロック時のアクション")]
        public OnUnlockAction ActionOnUnlock;

        [Tooltip("AcrionOnUnkock が AddCompforParent の場合に追加する CompData")]
        [SerializeReference] public CompData CompToAdd;

        [Tooltip("アンロック時に発火するシグナル名")]
        public string SignalNameOnUnlock;

        /// <summary>
        /// 対応する Comp クラスのフルネーム
        /// </summary>
        public override string ClassName =>
            "BlockBreaker3D.Models.InGame.Component.UnlockedWith";
    }
}