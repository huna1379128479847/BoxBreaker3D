using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using BlockBreaker3D.Datas.Component;

namespace BlockBreaker3D.Models.InGame.Component
{
    public static class CompCreator
    {
        private readonly static ConcurrentDictionary<Type, Func<CompData, Comp>> _cache = new();

        // リフレクションを使って CompData から Comp インスタンスを生成する
        // 名前空間をまたいで CompInfo クラスを探すため、型名だけではなくフルネームでも検索を試みる
        public static Comp Create(CompData data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            var name = data.ClassName;
            var type = FindCompType(name) ??
                throw new InvalidOperationException($"CompInfo type not found for ClassName: {data.ClassName}");


            if (_cache.TryGetValue(type, out var func))
            {
                return func(data);
            }

            var method = type.GetMethod(
                "Create",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static) ??
                throw new MissingMethodException(
                    $"Create method not found on type {type.FullName}. Expected signature: static Comp Create(CompData, GameDataHolder, IObject)");

            var del = (Func<CompData, Comp>)
                Delegate.CreateDelegate(typeof(Func<CompData, Comp>), method);

            _cache[type] = del;

            return del(data);
        }


        private static Type FindCompType(string className)
        {
            if (string.IsNullOrWhiteSpace(className)) return null;

            // 1. そのまま型名として探す
            var type = Type.GetType(className, false, true);

            // 2. 見つからなければ "CompInfo" の付け外しを試す
            if (type == null)
            {
                var name = className;
                var idx = name.IndexOf("CompInfo", StringComparison.OrdinalIgnoreCase);
                if (idx == -1)
                {
                    name += "CompInfo";
                }
                else
                {
                    // "CompInfo" だけ取り除く
                    name = name.Remove(idx, "CompInfo".Length);
                }

                type = Type.GetType(name, false, true);
            }

            // 3. まだ見つからない場合、全アセンブリから探す
            if (type == null)
            {
                var simpleName = className.Contains(".")
                    ? className.Split('.').Last()
                    : className;

                type = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .Select(a => a.GetType(simpleName, false, true))
                    .FirstOrDefault(t => t != null);
            }

            return type;
        }
    }
}
