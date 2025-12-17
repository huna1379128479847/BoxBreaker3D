using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlockBreaker3D.View.InGame
{
    public class HPView : GameViewBase
    {
        [SerializeField] private TMP_Text _hp;
        [SerializeField] private Image _heart;

        public void SetHP(int hp)
        {
            _hp.text = hp.ToString();
        }

        public void Shake(float pow, float duration)
        {

        }

        public override void Enable(bool enable)
        {
            if (_hp != null) _hp.gameObject.SetActive(enable);
            if (_heart != null) _heart.gameObject.SetActive(enable);
        }
    }
}