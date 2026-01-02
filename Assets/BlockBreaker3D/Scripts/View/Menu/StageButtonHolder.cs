using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace BlockBreaker3D.View.Menu
{
    public sealed class StageButtonHolder : IDisposable
    {
        private StageButton.ButtonPool _pool;
        private GridLayoutGroup _parent;
        private List<StageButton> _activeButtons = new();

        [Inject]
        public StageButtonHolder(
            StageButton.ButtonPool pool,
            [Inject(Id = "StageButtonCont")] GridLayoutGroup parent)
        {
            _pool = pool;
            _parent = parent;
        }

        public StageButton ActiveButton(int id, Action onClick)
        {
            var button = _pool.Spawn();
            button.transform.SetParent(_parent.transform, false);
            button.Button.onClick.AddListener(() => onClick?.Invoke());
            _activeButtons.Add(button);

            CalcHeight(); 
            return button;
        }

        private void CalcHeight()
        {
            var content = (RectTransform)_parent.transform;
            int count = _activeButtons.Count;

            if (count <= 0)
            {
                // 空ならパディング分だけにするか0にするかは好み
                content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
                    _parent.padding.top + _parent.padding.bottom);
                return;
            }

            int columns = GetColumnCount(content);
            columns = Mathf.Max(1, columns);

            int rows = Mathf.CeilToInt(count / (float)columns);

            float height =
                _parent.padding.top + _parent.padding.bottom +
                rows * _parent.cellSize.y +
                Mathf.Max(0, rows - 1) * _parent.spacing.y;

            content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

            // 即反映したいとき（ScrollRectの慣性やスクロール範囲更新も安定）
            LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        }

        private int GetColumnCount(RectTransform content)
        {
            // 1) 固定列数が設定されているならそれを使う
            if (_parent.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
                return _parent.constraintCount;

            // 2) 固定行数なら「列数は要素数/行数」で決まる（ただし要素数依存）
            if (_parent.constraint == GridLayoutGroup.Constraint.FixedRowCount)
            {
                int rows = Mathf.Max(1, _parent.constraintCount);
                return Mathf.CeilToInt(_activeButtons.Count / (float)rows);
            }

            // 3) どちらでもないなら Viewport/Content幅から列数を推定する
            //    ※通常、ContentはViewportと同幅運用が多い（Stretchしてる前提）
            float availableWidth = content.rect.width - _parent.padding.left - _parent.padding.right;
            float unit = _parent.cellSize.x + _parent.spacing.x;

            if (unit <= 0.0001f) return 1;

            // 端数は切り捨てでOK（入らない列は作れない）
            return Mathf.Max(1, Mathf.FloorToInt((availableWidth + _parent.spacing.x) / unit));
        }

        public void Dispose()
        {
            foreach (var cont in _activeButtons)
            {
                cont.Button.onClick.RemoveAllListeners();
                _pool.Despawn(cont);
            }
            _activeButtons.Clear();
        }
    }
}
