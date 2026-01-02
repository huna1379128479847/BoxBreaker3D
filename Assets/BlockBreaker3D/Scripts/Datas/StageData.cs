using UnityEngine;
using UnityEngine.AddressableAssets;

namespace BlockBreaker3D.Datas
{
    [CreateAssetMenu(fileName = "StageData", menuName = "BlockBreaker3D/Datas/StageData", order = 0)]
    public sealed class StageData : ScriptableObject
    {
        [SerializeField] private string _sceneName = "Stage_01";
        [SerializeField] private string _stageTitle = "Fallen";
        [SerializeField] private AssetReferenceT<Texture2D> _stageIcon;

        public string SceneName => _sceneName;
        public string StageTitle => _stageTitle;
        public AssetReferenceT<Texture2D> StageIcon => _stageIcon;
    }
}