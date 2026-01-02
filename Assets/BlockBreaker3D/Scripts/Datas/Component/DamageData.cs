using BlockBreaker3D.Utils;
using UnityEngine;

namespace BlockBreaker3D.Datas.Component
{
    [CreateAssetMenu(fileName = "DamageData", menuName = ScriptablePref.CompMenuNamePref + "DamageData")]
    public class DamageData : CompData
    {
        public int Value = 1;

        public bool enableDuplicate = false;

        public override string ClassName => "BlockBreaker3D.Models.InGame.Component.DamageCompInfo";
    }
}
