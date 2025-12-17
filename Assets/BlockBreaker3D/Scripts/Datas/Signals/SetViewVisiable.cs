using System;

namespace BlockBreaker3D.Datas.Signals
{
    public class SetViewVisible
    {
        [Flags]
        public enum ViewType
        {
            None = 0,
            ScoreView = 1 << 0,
            LivesView = 1 << 1,
            TurnHandView = 1 << 2,
        }
        public ViewType View;
        public bool IsVisible;
        public bool IsReverse; // true: Viewで指定したもの以外を表示/非表示にする
        public SetViewVisible(ViewType view, bool isVisible, bool isReverse)
        {
            View = view;
            IsReverse = isReverse;
            IsVisible = isVisible;
        }
        public bool HasAny(ViewType type)
        {
            if (View == ViewType.None && IsReverse) return true; // None指定でReverseなら全てにマッチ
            return (View & type) != 0;
        }
    }

    public class SetInputEnable
    {
        [Flags]
        public enum InputType
        {
            None = 0,
            TurnInput = 1 << 0,
        }
        public InputType Type;
        public bool IsEnable;
        public bool IsReverse; // true: Viewで指定したもの以外を表示/非表示にする
        public SetInputEnable(InputType type, bool isEnable, bool isReverse)
        {
            Type = type;
            IsReverse = isReverse;
            IsEnable = isEnable;
        }

        public bool HasAny(InputType type)
        {
            if (Type == InputType.None && IsReverse) return true; // None指定でReverseなら全てにマッチ
            return (Type & type) != 0;
        }

        public static SetInputEnable SetTurn(bool isEnable)
        {
            return new SetInputEnable(InputType.TurnInput, isEnable, false);
        }
    }
}