using BlockBreaker3D.Datas;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlockBreaker3D.Models.Menu
{
    [CreateAssetMenu(fileName = "StageDataHolder", menuName = "BlockBreaker3D/Datas/StageDataHolder", order = 0)]
    public sealed class StageDataHolder : ScriptableObject
    {
        [SerializeField] private List<StageCont> _stages;

        public List<StageCont> Stages => _stages;
    }

    [Serializable]
    public sealed class StageCont
    {
        [SerializeField] private int _stageIndex;
        [SerializeField] private StageData _data;

        public int StageIndex => _stageIndex;
        public StageData Data => _data;
    }
}