using System.Collections.Generic;
using UnityEngine;

namespace BlockBreaker3D.Datas
{
    public readonly struct Surface
    {
        public enum ExitSide
        {
            None,
            Top,
            Bottom,
            Left,
            Right
        }
        // Use factory pattern so surface-specific logic can be provided per surface
        // Factories create a Surface instance without arguments
        private static readonly Dictionary<string, System.Func<Surface>> _vector = new()
        {
            { "Top",    () => new Surface("Top", Vector3.forward, Vector3.right) },
            { "Bottom", () => new Surface("Bottom", Vector3.back, Vector3.left) },
            { "Left",   () => new Surface("Left", Vector3.up, Vector3.back) },
            { "Right",  () => new Surface("Right", Vector3.up, Vector3.forward) },
            { "Front",  () => new Surface("Front", Vector3.up, Vector3.right) },
            { "Back",   () => new Surface("Back", Vector3.up, Vector3.left) },
        };

        public static readonly Surface Top = Create("Top");
        public static readonly Surface Bottom = Create("Bottom");
        public static readonly Surface Left = Create("Left");
        public static readonly Surface Right = Create("Right");
        public static readonly Surface Front = Create("Front");
        public static readonly Surface Back = Create("Back");

        // 一旦保留
        // 必要そうなもの：
        // ある面から別の面に移動するときにY軸を反転させる必要があるかどうか
        // X軸版も
        // あとは回転させるかどうか？
        // SurfaceBehaviour 側でやるべきかもしれない
        private static readonly Dictionary<string, List<string>> _shouldReverseY = new()
        {
            // To, Froms
            // Y軸を反転させる必要がある遷移(キーが移動先)
            { "Top", new(){"Back" }  },
            { "Bottom", new()},
            { "Left", new() },
            { "Right", new() },
            { "Front", new() },
            { "Back", new() },
        };

        public string Name { get; }
        public Vector3 UpVector { get; }
        public Vector3 RightVector { get; }

        public Surface(string name, Vector3 up, Vector3 right)
        {
            Name = name;
            UpVector = up;
            RightVector = right;
        }
        // Convenience ctor: create surface by name and lookup default vectors
        public Surface(string name)
        {
            Name = name;
            (UpVector, RightVector) = DefaultMove(name);
        }
        public override string ToString() => Name;

        public static bool TryCreateSurface(string name, out Surface surface)
        {
            if (_vector.TryGetValue(name, out var func))
            {
                surface = func();
                return true;
            }
            surface = default;
            return false;
        }

        private static Surface Create(string name)
        {
            if (_vector.TryGetValue(name, out var func))
            {
                return func();
            }
            return new Surface(name);
        }
        /// <summary>
        /// その面のデフォルトの上方向と右方向を取得します。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static (Vector3 up, Vector3 right) DefaultMove(string name)
        {
            if (_vector.TryGetValue(name, out var func))
            {
                var s = func();
                return (s.UpVector, s.RightVector);
            }
            return (Vector3.zero, Vector3.zero);
        }

        public static (Vector3 up, Vector3 right) DefaultMove(Surface surface)
        {
            return (surface.UpVector, surface.RightVector);
        }

        public static void RegisterCustomSurface(string name, Vector3 up, Vector3 right)
        {
            if (!_vector.ContainsKey(name))
            {
                _vector.Add(name, () => new Surface(name, up, right));
            }
        }

        /// <summary>
        /// Surface ローカル2D座標（X=right, Y=up）をワールド座標に変換します。
        /// </summary>
        public Vector3 LocalToWorld(Vector3 origin, Vector2 localPos)
        {
            var (up, right) = DefaultMove(this);
            return origin + right * localPos.x + up * localPos.y;
        }

        /// <summary>
        /// ワールド座標を Surface のローカル2D座標（X=right, Y=up）に変換します。
        /// </summary>
        public Vector2 WorldToLocal(Vector3 origin, Vector3 worldPos)
        {
            var (up, right) = DefaultMove(this);
            var local = worldPos - origin;
            float x = Vector3.Dot(local, right);
            float y = Vector3.Dot(local, up);
            return new Vector2(x, y);
        }

        /// <summary>
        /// UV (0..1, 0..1) を Surface のローカル領域（size）にマップしてワールド座標に変換します。
        /// UV (0,0) は左下、(1,1) は右上として扱います。
        /// </summary>
        public Vector3 UVToWorld(Vector3 origin, Vector2 uv, Vector2 size)
        {
            var localX = (uv.x - 0.5f) * size.x;
            var localY = (uv.y - 0.5f) * size.y;
            return LocalToWorld(origin, new Vector2(localX, localY));
        }

        /// <summary>
        /// 指定された Surface ローカル座標が与えられたサイズ領域(0..size.x, 0..size.y)の外かどうかを判定します。
        /// </summary>
        public bool IsOutside(Vector2 localPos, Vector2 size)
        {
            return localPos.x < 0f || localPos.x > size.x || localPos.y < 0f || localPos.y > size.y;
        }

        /// <summary>
        /// 領域外に出ている場合、どの辺を越えたかを返します。斜めに出ている場合は、正規化した超過量が大きい方向を返します>
        /// </summary>
        public ExitSide GetExitSide(Vector2 localPos, Vector2 size)
        {
            var leftDiff = -localPos.x; // positive when left outside
            var rightDiff = localPos.x - size.x; // positive when right outside
            var bottomDiff = -localPos.y; // positive when bottom outside
            var topDiff = localPos.y - size.y; // positive when top outside

            leftDiff = Mathf.Max(0f, leftDiff);
            rightDiff = Mathf.Max(0f, rightDiff);
            bottomDiff = Mathf.Max(0f, bottomDiff);
            topDiff = Mathf.Max(0f, topDiff);

            var max = Mathf.Max(Mathf.Max(leftDiff, rightDiff), Mathf.Max(bottomDiff, topDiff));
            if (max == 0f) return ExitSide.None;
            if (max == leftDiff) return ExitSide.Left;
            if (max == rightDiff) return ExitSide.Right;
            if (max == bottomDiff) return ExitSide.Bottom;
            return ExitSide.Top;
        }
    }
}