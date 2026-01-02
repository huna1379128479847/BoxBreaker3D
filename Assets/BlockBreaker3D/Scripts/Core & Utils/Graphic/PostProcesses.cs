using BlockBreaker3D.Utils;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace BlockBreaker3D.Utils.Graphic
{
    // Full Screen Pass Renderer Feature reference に Materialが設定されている前提
    public static class PostProcesses
    {
        public enum ProcessType
        {
            MonoChrome,
            Noise,
            Glitch
        }

        private static readonly int StrengthHash = Shader.PropertyToID("_strength");

        private static readonly Dictionary<ProcessType, string> TypeToAddress = new()
        {
            { ProcessType.MonoChrome, "Shader/MonoChrome/Mat" },
            { ProcessType.Noise,     "Shader/Noise/NoiseMat" },
            { ProcessType.Glitch,    "Shader/Glitch/Glitch_mat" },
        };

        private static readonly Dictionary<ProcessType, Material> Cache = new();
        private static readonly Dictionary<ProcessType, float> Current = new();
        private static readonly Dictionary<ProcessType, Tween> Tweens = new();

        /// <summary>最低でも1つロードされているか</summary>
        public static bool IsAnyLoaded => Cache.Count > 0;

        public static bool IsLoadedMaterial(ProcessType type)
        {
            return Cache.TryGetValue(type, out var mat) && mat != null;
        }

        public static float GetCurrent(ProcessType type)
        {
            return Current.TryGetValue(type, out var v) ? v : 0f;
        }

        /// <summary>
        /// strength: 1に近いほど効果が強い（※シェーダ側の命名に合わせるため反転して渡す）
        /// </summary>
        public static void SetStrength(ProcessType type, float strength = 1f)
        {
            if (!LogPainter.Assert(IsLoadedMaterial(type), $"{type} material is not loaded"))
                return;

            var v = ProcessStrength(type, strength);
            // 命名に合わせるため反転（既存 Monochrome と同じ挙動）
            Cache[type].SetFloat(StrengthHash, Current[type] = v);
        }

        /// <summary>
        /// 指定typeの強さをフェードさせる（TimeScale非依存）
        /// </summary>
        public static async UniTask Fade(ProcessType type, float from, float to, float duration, Ease ease, CancellationToken token = default)
        {
            if (!LogPainter.Assert(IsLoadedMaterial(type), $"{type} material is not loaded"))
                return;

            // 既存Tweenがあれば潰す（重複制御）
            KillTween(type);

            // 即時反映（“呼んだ瞬間の見た目”が確定する）
            SetStrength(type, from);

            Tween tween = DOTween
                .To(() => Current[type], x =>
                {
                    SetStrength(type, x);
                }, to, duration)
                .SetUpdate(true)
                .SetEase(ease);

            Tweens[type] = tween;

            try
            {
                await tween.WithCancellation(token);
            }
            finally
            {
                // 完了/キャンセルのどちらでも参照を掃除
                if (Tweens.TryGetValue(type, out var t) && t == tween)
                    Tweens.Remove(type);
            }
        }

        /// <summary>
        /// Addressablesからロード（ロード済みなら何もしない）
        /// </summary>
        public static async UniTask LoadAsync(ProcessType type)
        {
            if (IsLoadedMaterial(type))
                return;

            if (!LogPainter.Assert(TypeToAddress.ContainsKey(type), $"Address not found for {type}"))
                return;

            var mat = await Addressables.LoadAssetAsync<Material>(TypeToAddress[type]);

            // 念のため null ガード
            if (!LogPainter.Assert(mat != null, $"Failed to load material for {type}"))
                return;

            Cache[type] = mat;

            // 初期値を用意（GetCurrentが安定する）
            if (!Current.ContainsKey(type))
                Current[type] = 0f;

            if (type == ProcessType.Noise)
            {
                // Noiseは初期シードを設定しておく
                mat.SetInt("_Seed", DateTime.Now.Second);
                var go = new GameObject("NoiseSeedUpdater");
                go.AddComponent<NoiseSeedUpdater>().SetMat(mat);
            }
        }

        /// <summary>
        /// 個別解放
        /// </summary>
        public static void Dispose(ProcessType type)
        {
            KillTween(type);

            if (Cache.TryGetValue(type, out var mat) && mat != null)
            {
                Addressables.Release(mat);
            }
            Cache.Remove(type);
            Current.Remove(type);
        }

        /// <summary>
        /// 全解放
        /// </summary>
        public static void DisposeAll()
        {
            foreach (var type in new List<ProcessType>(Cache.Keys))
            {
                Dispose(type);
            }
        }

        private static void KillTween(ProcessType type)
        {
            if (Tweens.TryGetValue(type, out var tween) && tween != null && tween.IsActive())
            {
                tween.Kill();
            }
            Tweens.Remove(type);
        }

        /// <summary>
        /// API上のstrength（1=強い）を返す。内部は反転保持なので再反転する。
        /// </summary>
        private static float ProcessStrength(ProcessType type, float newValue)
        {
            if (type == ProcessType.Noise ||
                type == ProcessType.Glitch)
            {
                // NoiseはAPI表現とシェーダ表現が同じなのでそのまま返す
                return newValue;
            }
            return Mathf.Clamp01(1f - newValue);
        }
    }
}
