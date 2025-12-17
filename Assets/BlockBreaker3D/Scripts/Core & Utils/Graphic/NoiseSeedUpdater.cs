using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BlockBreaker3D.Utils.Graphic
{
    public class NoiseSeedUpdater : MonoBehaviour
    {
        private int _strengthHash = Shader.PropertyToID("_strength");
        private int _seedHash = Shader.PropertyToID("_Seed");
        Material m_mat;
        [Range(0, 1)]
        public float horizonValue;
        public void SetMat(Material mat)
        {
            m_mat = mat;
        }

        void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            if (m_mat == null)
                return;
            // ランダムシード値を更新することで乱数を動かす
            m_mat.SetInt(_seedHash, Time.frameCount);
            // 左右にずらす値をセット
            m_mat.SetFloat(_strengthHash, horizonValue);
            Graphics.Blit(src, dest, m_mat);
        }
    }
}