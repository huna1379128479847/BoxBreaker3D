using UnityEngine;

namespace BlockBreaker3D.Utils
{
    public static class PredicateBuilder
    {
        //=============================
        // トークン種別定義
        //=============================
        public enum TokenKind
        {
            None,       // 先頭 / まだ何もない
            Operand,    // Score, Block, GetScore, 数値など
            CompareOp,  // >, <, >=, <=
            LogicOp,    // AND, OR
            LeftParen,  // (
            RightParen  // )
        }

        //=============================
        // メイン: トークン追加処理
        //=============================
        public static void Build(ref string t, string s)
        {
            // 余計な空白を削る
            s = s?.Trim();
            if (string.IsNullOrEmpty(s))
                return;

            var newKind = GetTokenKind(s);

            // 直前トークン種別
            TokenKind prevKind = TokenKind.None;
            if (!string.IsNullOrEmpty(t))
            {
                var sp = t
                    .Split(' ', System.StringSplitOptions.RemoveEmptyEntries);

                if (sp.Length > 0)
                {
                    string lastToken = sp[^1];
                    prevKind = GetTokenKind(lastToken);
                }
            }

            // 現在の括弧の深さを計算
            int parenDepth = ComputeParenDepth(t);

            // 閉じ括弧のチェック：対応する '(' がないのに ')' はNG
            if (newKind == TokenKind.RightParen && parenDepth <= 0)
            {
                Debug.LogError("対応する開き括弧 '(' が存在しません");
                return;
            }

            // 並びが不正ならエラー
            if (!CanFollow(prevKind, newKind))
            {
                Debug.LogError($"トークンの並びが不正です: {prevKind} → {newKind} (\"{s}\")");
                return;

            }

            // 文字列として連結
            t = string.IsNullOrEmpty(t)
                ? s
                : $"{t} {s}";
        }

        //=============================
        // トークン種別判定
        //=============================
        public static TokenKind GetTokenKind(string token)
        {
            if (token == "(")
                return TokenKind.LeftParen;
            if (token == ")")
                return TokenKind.RightParen;
            if (IsLogicOperator(token))
                return TokenKind.LogicOp;
            if (IsCompareOperator(token))
                return TokenKind.CompareOp;

            // それ以外は全部オペランド扱い（Score, Block, GetScore, 数値など）
            return TokenKind.Operand;
        }

        public static void RemoveLastToken(ref string predicate)
        {
            if (string.IsNullOrWhiteSpace(predicate))
                return;
            var tokens = predicate.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 0)
                return;
            // 最後のトークンを除去して再結合
            predicate = string.Join(" ", tokens, 0, tokens.Length - 1);
        }

        //=============================
        // 並びの妥当性チェック
        //=============================
        public static bool CanFollow(TokenKind prev, TokenKind next)
        {
            return prev switch
            {
                TokenKind.None => next == TokenKind.Operand || next == TokenKind.LeftParen,// 先頭に来ていいのはオペランドか '('
                TokenKind.Operand => next == TokenKind.CompareOp
                                        || next == TokenKind.LogicOp
                                        || next == TokenKind.RightParen,// Operand のあと：
                                                                        //   Operand > ...
                                                                        //   Operand AND ...
                                                                        //   Operand ) ...
                TokenKind.CompareOp => next == TokenKind.Operand
                                        || next == TokenKind.LeftParen,// > のあと：
                                                                       //   > Operand
                                                                       //   > ( Operand ...
                TokenKind.LogicOp => next == TokenKind.Operand
                                        || next == TokenKind.LeftParen,// AND / OR のあと：
                                                                       //   AND Operand
                                                                       //   AND ( ...
                TokenKind.LeftParen => next == TokenKind.Operand
                                        || next == TokenKind.LeftParen,// '(' のあと：
                                                                       //   ( Operand
                                                                       //   ( (
                TokenKind.RightParen => next == TokenKind.LogicOp
                                        || next == TokenKind.RightParen,// ')' のあと：
                                                                        //   ) AND ...
                                                                        //   ) OR ...
                                                                        //   ) ) ...
                _ => false,
            };
        }

        public static bool IsValid(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return false;

            var tokens = s.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 0)
                return false;

            PredicateBuilder.TokenKind prevKind = TokenKind.None;
            int parenDepth = 0;

            foreach (var token in tokens)
            {
                var kind = GetTokenKind(token);

                // 並びの妥当性チェック
                if (!CanFollow(prevKind, kind))
                    return false;

                // 括弧の深さチェック
                if (kind == TokenKind.LeftParen)
                    parenDepth++;
                else if (kind == TokenKind.RightParen)
                {
                    parenDepth--;
                    if (parenDepth < 0)
                        return false;
                }

                prevKind = kind;
            }

            // 括弧が閉じているか
            if (parenDepth != 0)
                return false;

            // 最後のトークンが不正でないか
            if (prevKind == TokenKind.CompareOp || prevKind == TokenKind.LogicOp || prevKind == TokenKind.LeftParen)
                return false;

            return true;
        }
        //=============================
        // 括弧の深さ計算
        //=============================
        public static int ComputeParenDepth(string predicate)
        {
            if (string.IsNullOrEmpty(predicate))
                return 0;

            int depth = 0;
            var tokens = predicate.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);

            foreach (var tok in tokens)
            {
                if (tok == "(")
                {
                    depth++;
                }
                else if (tok == ")")
                {
                    depth--;
                }
            }
            return depth;
        }

        // 比較演算子判定
        public static bool IsCompareOperator(string s)
            => s == ">" || s == "<" || s == ">=" || s == "<=";

        // 論理演算子判定
        public static bool IsLogicOperator(string s)
            => s == "AND" || s == "OR";
    }
}