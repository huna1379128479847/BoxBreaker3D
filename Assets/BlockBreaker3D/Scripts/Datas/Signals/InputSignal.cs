using System;

namespace BlockBreaker3D.Datas.Signals
{
    [Flags]
    public enum InputType : uint
    {
        Invalid = 0,
        TurnRight = 1 << 0,
        TurnLeft = 1 << 1,
        Turn = TurnRight | TurnLeft,
    }

    public static class InputTypeExtensions
    {
        public static bool HasFlagAnyFast(this InputType inputType, InputType flag)
        {
            return (inputType & flag) != 0;
        }

        public static bool HasFlagAnyFast(this InputSignal signal, InputType flag)
        {
            return signal.Type.HasFlagAnyFast(flag);
        }
    }

    public sealed class InputSignal
    {
        public InputType Type { get; }
        public InputSignal(InputType type)
        {
            Type = type;
        }
    }
}