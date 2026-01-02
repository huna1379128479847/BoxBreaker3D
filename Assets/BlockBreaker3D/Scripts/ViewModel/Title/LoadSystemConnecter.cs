using BlockBreaker3D.Models;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using Zenject;
namespace BlockBreaker3D.ViewModel
{
    public class TitleButtons
    {
        [Inject]
        public TitleButtons(LoadSceneMono loader, [Inject(Id = "GoMenu")] Button goMenu)
        {
            // メニューシーンはまだ作成していないため、仮でStage1シーンへ遷移
            goMenu.onClick.AddListener(() => loader.LoadSceneAsync("Scene/Menu/StageSelect").Forget());
        }
    }
}