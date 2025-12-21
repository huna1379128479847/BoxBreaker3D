namespace BlockBreaker3D.Datas.Scriptable
{
    using Sirenix.OdinInspector;
    using UnityEngine;
    [CreateAssetMenu(fileName = "StepData", menuName = "BlockBreaker3D/Datas/StepData", order = 1)]
    public class StepData : ScriptableObject
    {
        public const string PATH = "Assets/BlockBreaker3D/Data/StepData.asset";
        [SerializeField] private float _step = 0.5f;

        public float Step => _step;
        public float HalfStep => _step / 2f;
        public float QuarterStep => HalfStep / 2f;

    }
}