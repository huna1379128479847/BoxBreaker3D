using BlockBreaker3D.Utils;
using HighElixir;
using Sirenix.OdinInspector;
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

        #region Predicator
        // 作成中
        [BoxGroup("Predicate String")]
        [Tooltip("条件式を文字列で指定する")]
        public bool UsePredicateString;

        [ShowIf("UsePredicateString")]
        [BoxGroup("Predicate String")]
        [InfoBox("条件式を組み立てると、Comp生成時に任意の条件をつかえる", Icon = SdfIconType.BatteryFull)]
        public string PredicateString;

        //=============================
        // 条件用トークン追加ボタン群
        //=============================
#if UNITY_EDITOR
        [ShowIf("UsePredicateString")]
        [BoxGroup("Predicate String")]
        [FoldoutGroup("Predicate String/ScoreConditions")]
        [Button("Add Score Condition")]
        [Tooltip("このゲーム中の累計スコア条件を条件式に追加する")]
        public void AddScore()
            => PredicateBuilder.Build(ref PredicateString, "Score");

        [ShowIf("UsePredicateString")]
        [BoxGroup("Predicate String")]
        [FoldoutGroup("Predicate String/ScoreConditions")]
        [Button("Add GetScore Condition")]
        [Tooltip("このCompが有効化されてから獲得したスコアを条件に追加する")]
        public void AddGetScore()
            => PredicateBuilder.Build(ref PredicateString, "GetScore");

        // ブロック系
        [ShowIf("UsePredicateString")]
        [BoxGroup("Predicate String")]
        [FoldoutGroup("Predicate String/BlockConditions")]
        [Button("Add Block Condition")]
        [Tooltip("このゲーム中の累計ブロック破壊数を条件に追加する")]
        public void AddBlock()
            => PredicateBuilder.Build(ref PredicateString, "Block");

        [ShowIf("UsePredicateString")]
        [BoxGroup("Predicate String")]
        [FoldoutGroup("Predicate String/BlockConditions")]
        [Button("Add GetBlock Condition")]
        [Tooltip("このCompが有効化されてから破壊したブロック数を条件に追加する")]
        public void AddGetBlock()
            => PredicateBuilder.Build(ref PredicateString, "GetBlock");

        [ShowIf("UsePredicateString")]
        [BoxGroup("Predicate String")]
        [FoldoutGroup("Predicate String/BlockConditions")]
        [Button("Add BlockRemain.Box Condition")]
        [Tooltip("このコンポーネントが属するBoxの残りブロック数を条件に追加する")]
        public void AddBlockRemain_Box()
            => PredicateBuilder.Build(ref PredicateString, "BlockRemain.Box");

        [ShowIf("UsePredicateString")]
        [BoxGroup("Predicate String")]
        [FoldoutGroup("Predicate String/BlockConditions")]
        [Button("Add BlockRemain.Surface Condition")]
        [Tooltip("このコンポーネントが属するSurfaceの残りブロック数を条件に追加する")]
        public void AddBlockRemain_Surface()
            => PredicateBuilder.Build(ref PredicateString, "BlockRemain.Surface");

        // 論理演算子 AND / OR
        [ShowIf("UsePredicateString")]
        [BoxGroup("Predicate String")]
        [FoldoutGroup("Predicate String/LogicalOperators")]
        [Button("Add AND Operator")]
        [Tooltip("条件式に AND 論理演算子を追加する")]
        public void AddAnd()
            => PredicateBuilder.Build(ref PredicateString, "AND");

        [ShowIf("UsePredicateString")]
        [BoxGroup("Predicate String")]
        [FoldoutGroup("Predicate String/LogicalOperators")]
        [Button("Add OR Operator")]
        [Tooltip("条件式に OR 論理演算子を追加する")]
        public void AddOr()
            => PredicateBuilder.Build(ref PredicateString, "OR");

        // 比較演算子 >, >=, <, <=
        [ShowIf("UsePredicateString")]
        [BoxGroup("Predicate String")]
        [FoldoutGroup("Predicate String/LogicalSymbols")]
        [Button("Add >")]
        [Tooltip("条件式に Greater 演算子（>）を追加する")]
        public void AddGreater()
            => PredicateBuilder.Build(ref PredicateString, ">");

        [ShowIf("UsePredicateString")]
        [BoxGroup("Predicate String")]
        [FoldoutGroup("Predicate String/LogicalSymbols")]
        [Button("Add >=")]
        [Tooltip("条件式に Greater Than 演算子（>=）を追加する")]
        public void AddGreaterEqual()
            => PredicateBuilder.Build(ref PredicateString, ">=");

        [ShowIf("UsePredicateString")]
        [BoxGroup("Predicate String")]
        [FoldoutGroup("Predicate String/LogicalSymbols")]
        [Button("Add <")]
        [Tooltip("条件式に Less 演算子（<）を追加する")]
        public void AddLess()
            => PredicateBuilder.Build(ref PredicateString, "<");

        [ShowIf("UsePredicateString")]
        [BoxGroup("Predicate String")]
        [FoldoutGroup("Predicate String/LogicalSymbols")]
        [Button("Add <=")]
        [Tooltip("条件式に Less Than 演算子（<=）を追加する")]
        public void AddLessEqual()
            => PredicateBuilder.Build(ref PredicateString, "<=");

        // 括弧
        [ShowIf("UsePredicateString")]
        [BoxGroup("Predicate String")]
        [FoldoutGroup("Predicate String/Parentheses")]
        [Button("Add ( ")]
        [Tooltip("条件式に開き括弧 '(' を追加する")]
        public void AddLeftParen()
            => PredicateBuilder.Build(ref PredicateString, "(");

        [ShowIf("UsePredicateString")]
        [BoxGroup("Predicate String")]
        [FoldoutGroup("Predicate String/Parentheses")]
        [Button("Add ) ")]
        [Tooltip("条件式に閉じ括弧 ')' を追加する")]
        public void AddRightParen()
            => PredicateBuilder.Build(ref PredicateString, ")");

        // 数値（リテラル）の追加
        [ShowIf("UsePredicateString")]
        [BoxGroup("Predicate String")]
        [FoldoutGroup("Predicate String/Value")]
        [LabelText("Value")]
        [Tooltip("条件式に追加する数値リテラル")]
        public int TempValue;

        [ShowIf("UsePredicateString")]
        [BoxGroup("Predicate String")]
        [FoldoutGroup("Predicate String/Value")]
        [Button("Add Value")]
        [Tooltip("TempValue を条件式に追加する")]
        public void AddValue()
            => PredicateBuilder.Build(ref PredicateString, TempValue.ToString());

        [ShowIf("UsePredicateString")]
        [BoxGroup("Predicate String")]
        [FoldoutGroup("Predicate String/Value")]
        [Button("Remove Last")]
        [Tooltip("最後のトークンを削除")]
        public void RemoveLast()
            => PredicateBuilder.RemoveLastToken(ref PredicateString);
#endif
        #endregion

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