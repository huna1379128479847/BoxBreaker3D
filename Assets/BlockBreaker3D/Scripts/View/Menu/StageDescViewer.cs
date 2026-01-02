using TMPro;
using UnityEngine;

namespace BlockBreaker3D.View.Menu
{
    public class StageDescViewer : MonoBehaviour
    {
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _description;
        [SerializeField] private Texture _icon;
        public void SetDescription(string desc)
        {
            // TODO : ステージ説明の表示更新
            _title.SetText(desc);
        }

        public void SetTitle(string title)
        {
            // TODO : ステージタイトルの表示更新
            _description.SetText(title);
        }

        public void SetIcon(Texture icon)
        {
            // TODO : ステージアイコンの表示更新
            _icon = icon;
        }

        public void Show()
        {
            // TODO : ステージ説明UIの表示
        }

        public void Hide()
        {
            // TODO : ステージ説明UIの非表示
        }
    }
}