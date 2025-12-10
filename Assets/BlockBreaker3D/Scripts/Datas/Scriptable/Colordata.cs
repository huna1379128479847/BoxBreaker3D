namespace BlockBreaker3D.Datas.Scriptable
{
    using Codice.CM.Client.Differences.Graphic;
    using HighElixir;
    using UnityEngine;
    [CreateAssetMenu(fileName = "ColorData", menuName = "BlockBreaker3D/ColorData", order = 0)]
    public class ColorData : ScriptableObject
    {
        [LinkedFileName]
        public string colorName;
        public Color color;
        public Gradient gradient;
    }
}