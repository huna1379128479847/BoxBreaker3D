using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace BlockBreaker3D.View.Menu
{
    public sealed class StageButton : MonoBehaviour
    {
        public sealed class ButtonPool : MemoryPool<StageButton>
        {
            protected override void OnCreated(StageButton item)
            {
                item.gameObject.SetActive(false);
            }
            protected override void Reinitialize(StageButton item)
            {
                item.gameObject.SetActive(true);
            }
            protected override void OnDespawned(StageButton item)
            {
                if (item == null) return;
                item.Button.onClick.RemoveAllListeners();
                item.gameObject.SetActive(false);
            }
        }
        [SerializeField] private Image _icon;
        [SerializeField] private Image _backGround;
        [SerializeField] private Image _frame;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private Button _button;

        public Image Icon { get => _icon; set => _icon = value; }
        public Image BackGround { get => _backGround; set => _backGround = value; }
        public Image Frame { get => _frame; set => _frame = value; }
        public TMP_Text Title { get => _title; set => _title = value; }
        public Button Button { get => _button; set => _button = value; }
    }
}