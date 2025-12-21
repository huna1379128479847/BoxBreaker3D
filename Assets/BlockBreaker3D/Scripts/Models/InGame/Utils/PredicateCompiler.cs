using BlockBreaker3D.Models.InGame;
using BlockBreaker3D.Models.InGame.Box;
using System;
using System.Collections.Generic;

namespace BlockBreaker3D.Models.Utils
{
    public static class PredicateCompiler
    {
        // トークン種別
        private enum TokenKind
        {
            OperandVar,    // Score, GetScore, Block, GetBlock など
            OperandNum,    // 数値 123 など
            CompareOp,     // >, <, >=, <=, ==, !=
            LogicOp,       // AND, OR
            LeftParen,     // (
            RightParen     // )
        }

        private sealed class Token
        {
            public TokenKind Kind;
            public string Raw;    // 元の文字列
        }

        /// <summary>
        /// GameDataHolder にぶら下がっているデータを元に、
        /// PredicateString を評価する Func<bool> を生成する。
        /// GetScore/GetBlock は「この関数を生成した瞬間」からの差分。
        /// </summary>
        public static Func<GameDataHolder, IObject, (int baseScore, int baseBlocks), bool> CompilePredicate(string pre)
        {
            if (string.IsNullOrWhiteSpace(pre))
                return (_, __, ___) => true;

            // 1. トークナイズ
            var tokens = Tokenize(pre);

            // 2. Shunting-yard で RPN に変換
            var rpn = ToRpn(tokens);

            // 3. RPN を評価する Func<bool> を返す（ベースラインを閉じ込める）
            return (holder, obj, b) => EvaluateRpn(rpn, holder, obj, b.baseScore, b.baseBlocks);
        }

        //=====================
        // 1. Tokenize
        //=====================
        private static List<Token> Tokenize(string pre)
        {
            var result = new List<Token>();
            var parts = pre.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach (var part in parts)
            {
                var s = part.Trim();
                if (s.Length == 0) continue;

                if (s == "(")
                    result.Add(new Token { Kind = TokenKind.LeftParen, Raw = s });
                else if (s == ")")
                    result.Add(new Token { Kind = TokenKind.RightParen, Raw = s });
                else if (IsLogicOperator(s))
                    result.Add(new Token { Kind = TokenKind.LogicOp, Raw = s });
                else if (IsCompareOperator(s))
                    result.Add(new Token { Kind = TokenKind.CompareOp, Raw = s });
                else if (int.TryParse(s, out _))
                    result.Add(new Token { Kind = TokenKind.OperandNum, Raw = s });
                else
                    result.Add(new Token { Kind = TokenKind.OperandVar, Raw = s }); // それ以外は変数名
            }

            return result;
        }

        //=====================
        // 2. Shunting-yard (中置 -> RPN)
        //=====================
        private static List<Token> ToRpn(List<Token> tokens)
        {
            var output = new List<Token>();
            var ops = new Stack<Token>();

            foreach (var token in tokens)
            {
                switch (token.Kind)
                {
                    case TokenKind.OperandVar:
                    case TokenKind.OperandNum:
                        output.Add(token);
                        break;

                    case TokenKind.CompareOp:
                    case TokenKind.LogicOp:
                        while (ops.Count > 0 &&
                               IsOperator(ops.Peek()) &&
                               Precedence(ops.Peek()) >= Precedence(token))
                        {
                            output.Add(ops.Pop());
                        }
                        ops.Push(token);
                        break;

                    case TokenKind.LeftParen:
                        ops.Push(token);
                        break;

                    case TokenKind.RightParen:
                        while (ops.Count > 0 && ops.Peek().Kind != TokenKind.LeftParen)
                            output.Add(ops.Pop());

                        if (ops.Count == 0 || ops.Peek().Kind != TokenKind.LeftParen)
                            throw new Exception("括弧の対応が不正です（右括弧の対応する左括弧がありません）");

                        ops.Pop(); // 左括弧を捨てる
                        break;
                }
            }

            while (ops.Count > 0)
            {
                var op = ops.Pop();
                if (op.Kind == TokenKind.LeftParen || op.Kind == TokenKind.RightParen)
                    throw new Exception("括弧の対応が不正です（余った括弧があります）");

                output.Add(op);
            }

            return output;
        }

        private static bool IsOperator(Token t)
            => t.Kind == TokenKind.CompareOp || t.Kind == TokenKind.LogicOp;

        private static int Precedence(Token t) => t.Kind switch
        {
            TokenKind.CompareOp => 2,
            TokenKind.LogicOp => 1,
            _ => 0
        };

        //=====================
        // 3. RPN 評価
        //=====================
        private static bool EvaluateRpn(
            List<Token> rpn,
            GameDataHolder holder,
            IObject parent,
            int baseScore,
            int baseBlock)
        {
            var stack = new Stack<object>();

            foreach (var token in rpn)
            {
                switch (token.Kind)
                {
                    case TokenKind.OperandNum:
                        stack.Push(int.Parse(token.Raw));
                        break;

                    case TokenKind.OperandVar:
                        stack.Push(ResolveVariable(token.Raw, holder, parent, baseScore, baseBlock));
                        break;

                    case TokenKind.CompareOp:
                        if (stack.Count < 2) throw new Exception("比較演算子のオペランドが不足しています");
                        {
                            var right = (int)stack.Pop();
                            var left = (int)stack.Pop();
                            stack.Push(EvalCompare(token.Raw, left, right));
                        }
                        break;

                    case TokenKind.LogicOp:
                        if (stack.Count < 2) throw new Exception("論理演算子のオペランドが不足しています");
                        {
                            var right = (bool)stack.Pop();
                            var left = (bool)stack.Pop();
                            stack.Push(token.Raw switch
                            {
                                "AND" => left && right,
                                "OR" => left || right,
                                _ => throw new Exception($"未対応の論理演算子: {token.Raw}")
                            });
                        }
                        break;
                }
            }

            if (stack.Count != 1 || stack.Peek() is not bool)
                throw new Exception("式の評価結果が不正です（bool 1つになりませんでした）");

            return (bool)stack.Pop();
        }

        private static bool EvalCompare(string op, int left, int right) => op switch
        {
            ">" => left > right,
            "<" => left < right,
            ">=" => left >= right,
            "<=" => left <= right,
            "==" => left == right,
            "!=" => left != right,
            _ => throw new Exception($"未対応の比較演算子: {op}")
        };

        //=====================
        // 変数名 → 実際の値
        //=====================
        private static int ResolveVariable(
            string name,
            GameDataHolder holder,
            IObject parent,
            int baseScore,
            int baseBlock)
        {
            var curScore = holder.ScoreHolder.Score?.Value ?? 0;
            var curBlock = holder.ScoreHolder.DestroyedBlock?.Value ?? 0;

            return name switch
            {
                "Score" => curScore,
                "GetScore" => curScore - baseScore,

                "Block" => curBlock,
                "GetBlock" => curBlock - baseBlock,

                "BlockRemain.Box" =>
                    parent.BoxObject is BoxBehaviour boxObj
                        ? boxObj.GetTotalBlockCount()
                        : throw new Exception("Parent が BoxObject を持っていません"),

                "BlockRemain.Surface" =>
                    parent.SurfaceObject is SurfaceBehaviour s
                        ? s.GetBlockCount()
                        : throw new Exception("Parent が SurfaceObject を持っていません"),

                _ => throw new Exception($"未対応の変数名: {name}"),
            };
        }

        private static bool IsCompareOperator(string s)
            => s == ">" || s == "<" || s == ">=" || s == "<=" || s == "==" || s == "!=";

        private static bool IsLogicOperator(string s)
            => s == "AND" || s == "OR";
    }
}
