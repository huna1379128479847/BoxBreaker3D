using System;
using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace BlockBreaker3D.Datas.Mapping
{
    [Serializable]
    public class SurfaceLayout
    {
        private Dictionary<Vector2Int, IObjectData> _map = new();

        [JsonIgnore]
        public Dictionary<Vector2Int, IObjectData> Map => _map;

        public bool CanUse(Vector2Int position)
        {
            return _map.ContainsKey(position);
        }

        public void SetObjectData(Vector2Int position, IObjectData objectData)
        {
            _map[position] = objectData;
        }
    }
}