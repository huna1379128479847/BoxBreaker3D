using BlockBreaker3D.Datas.Component;
using System.Collections.Generic;
using UnityEngine;

namespace BlockBreaker3D.Datas.Mapping
{
    public interface IObjectData
    {
        List<Vector2Int> Occupancy { get; }
        List<CompData> Comps { get; }
    }
}