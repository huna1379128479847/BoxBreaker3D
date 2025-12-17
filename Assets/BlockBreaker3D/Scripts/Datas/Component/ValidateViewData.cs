using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlockBreaker3D.Datas.Component
{
    [Serializable]
    public struct ValidateViewStruct
    {
        public Vector2Int pos;
        public float up; // 面からどれだけ上にあるか
        public string text;
        public Sprite icon;
        public int priority; // 優先度。数値が大きいほど優先される
    }
    /// <summary>
    /// 特定のUI要素をオブジェクトに表示するためのデータクラス。
    /// </summary>
    public class ValidateViewData : UnlockWithData
    {
        [SerializeField] private List<ValidateViewStruct> _validateViews = new();
        public List<ValidateViewStruct> ValidateViews => _validateViews;
    }
}