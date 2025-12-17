using BlockBreaker3D.Datas;
using BlockBreaker3D.Models.InGame.Component;
using System.Collections.Generic;

namespace BlockBreaker3D.Models.InGame
{
    public interface IObject
    {
        ObjectType ObjectType { get; }
        IObject BoxObject { get; set; }
        IObject SurfaceObject { get; set; }
        void SetObjectType(ObjectType objectType);
        void ResetState();
        void AddComp(IComp comp);

        /// <remarks>
        /// Comp継承クラス内から呼ばないこと。
        /// もしComp内から削除したい場合は、CompのShouldbeRemovedプロパティをtrueに設定してください。<br />
        /// または、IObjectのMarkCompForRemovalメソッドを使用してください。
        /// </remarks>
        /// <param name="comp"></param>
        void RemoveComp(IComp comp);
        void RemoveComps<T>() where T : IComp;
        void MarkCompForRemoval(IComp comp);
        void MarkCompsForRemoval<T>() where T : IComp;
        IEnumerable<T> GetComps<T>() where T : IComp;
        T GetComp<T>() where T : IComp;
    }
}