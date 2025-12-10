namespace BlockBreaker3D.Datas.Scriptable
{
    using UnityEngine;
    [CreateAssetMenu(fileName = "AnimationData", menuName = "BlockBreaker3D/AnimationData", order = 0)]
    public sealed class AnimationData : ScriptableObject
    {
        [Header("Ball Animations")]
        public ParticleSystem OnSpawn;
        public ParticleSystem OnDespawn;
    }
}