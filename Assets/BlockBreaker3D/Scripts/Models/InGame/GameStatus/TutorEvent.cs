using BlockBreaker3D.Datas.Signals;
using BlockBreaker3D.Utils.Graphic;
using BlockBreaker3D.Models.Sounds;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Threading;
using UniRx;
using UnityEngine;
using Zenject;
using ProcessType = BlockBreaker3D.Utils.Graphic.PostProcesses.ProcessType;

namespace BlockBreaker3D.Models.InGame
{
    // チュートリアル用の演出を差し込む
    public class TutorEvent
    {
        private readonly GameDataHolder _holder;
        private readonly AudioLoader _loader;
        private bool _isSpacePressed = false;
        private BoolReactiveProperty _enabled = new(false);

        public IReadOnlyReactiveProperty<bool> Enabled => _enabled;

        [Inject]
        public TutorEvent(GameStateManager manager, GameDataHolder holder, AudioLoader loader)
        {
            Debug.Log("TutorEvent: Constructor called");
            _holder = holder;
            _loader = loader;
            // ゲーム開始のSignal発火前にさしこまれる
            manager.RegisterEventPreStart(PreStart);
            manager.RegisterEventPostStart(tc =>
            {
                var s = _holder.SignalBus;
                // ゲーム開始後は入力を無効化
                s.Fire(SetInputEnable.SetTurn(false));
                // チュートリアル表示を消す
                s.Fire(new SetViewVisible(SetViewVisible.ViewType.None, false, true));
                return UniTask.CompletedTask;
                // 次の演出につなげる予定
            });
        }

        private async UniTask PreStart(CancellationToken tc)
        {
            await _loader.LoadBGMAsync("stage_1");
            // 今後移動する予定
            await UniTask.WhenAll(
                PostProcesses.LoadAsync(PostProcesses.ProcessType.MonoChrome),
                PostProcesses.LoadAsync(PostProcesses.ProcessType.Glitch));
            // 作りたい演出
            // 開始時はモノクロの画面
            // ボールが上から落ちてくる ← 実装中
            // 面に着地し、モノクロの画面が色付きになる演出
            // スペースの文字と、矢印(移動方向)が表示される
            // スペースキーを押すと、ボールが矢印の方向に移動し始める
            // (元々画面が暗いせいで見えずらさを感じたので、調整が必要かも)

            _enabled.Value = false;

            // ボールが生成されるまで待機
            PostProcesses.SetStrength(ProcessType.MonoChrome, 1f);
            await UniTask.WaitUntil(() =>
            _holder.BallBehaviour != null &&
            _holder.BoxBehaviour.Value != null,
            cancellationToken: tc);

            // ボールを上から落とす
            var transform = _holder.BallBehaviour.transform;
            var tr = transform.GetComponent<TrailRenderer>();
            if (tr != null)
                tr.enabled = false;
            var sb = _holder.BoxBehaviour.Value.CurrentSurface.Value;
            var endPos = sb.SurfaceLocalToWorld(sb.SpawnPos);
            transform.position = endPos + Vector3.up * 10.0f;

            if (tr != null)
                tr.enabled = true;
            await transform.DOMove(endPos, 1.4f).SetEase(Ease.OutSine).WithCancellation(tc);
            Debug.Log("TutorEvent: Ball landed");

            await PostProcesses.Fade(ProcessType.MonoChrome, 1f, 0f, 3.3f, Ease.OutSine, tc);
            _enabled.Value = true;
            await UniTask.WaitUntil(() => _isSpacePressed, cancellationToken: tc);
            _enabled.Value = false;
            _isSpacePressed = false;

            // BGM再生
            var bgm = await _loader.LoadBGMAsync("stage_1");
            _holder.SignalBus.Fire(new RequirePlaySound(bgm, true).WithFadeIn(0.1f, 2.3f, 0.7f, Ease.OutSine));

            // 自動でゲームが始まる
        }

        public void InputSpace()
        {
            _isSpacePressed = true;
        }
    }
}