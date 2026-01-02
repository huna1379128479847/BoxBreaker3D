using BlockBreaker3D.Models;
using BlockBreaker3D.Models.Menu;
using BlockBreaker3D.View.Menu;
using Cysharp.Threading.Tasks;
using Zenject;

namespace BlockBreaker3D.ViewModel.Menu
{
    public sealed class  StageSelector : IInitializable
    {
        private StageDataHolder _datas;
        private LoadSceneMono _loader;
        private StageButtonHolder _buttonHolder;
        [Inject]
        public StageSelector(
            StageDataHolder stageDataHolder, 
            LoadSceneMono loader,
            StageButtonHolder buttonHolder)
        {
            _datas = stageDataHolder;
            _loader = loader;
            _buttonHolder = buttonHolder;
        }

        public void Initialize()
        {
            // 各ステージボタンの生成
            foreach (var data in _datas.Stages)
            {
                var b = _buttonHolder.ActiveButton(data.StageIndex, () =>
                {
                    // TODO : ロード中UIの表示
                    // 1回目のクリックでステージ概要の表示
                    // 2回目のクリックでシーン遷移
                    _loader.LoadSceneAsync(data.Data.SceneName).Forget();
                });
                b.Title.SetText(data.Data.StageTitle);
            }
        }
    }
}
