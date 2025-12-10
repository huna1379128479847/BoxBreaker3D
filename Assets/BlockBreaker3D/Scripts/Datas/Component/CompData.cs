using System;
using UnityEngine;

namespace BlockBreaker3D.Datas.Component
{
    public abstract class CompData : ScriptableObject
    {
        // 生成されるCompのクラス名を返す
        // CompInfoのあるなし、大文字小文字の違いは吸収される
        // 名前空間を書く必要がある
        // 必ず、static Comp Create(CompData data, GameDataHolder holder, IObject parent) メソッドを持つこと
        public abstract string ClassName { get; }

    }
}
