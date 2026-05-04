using System.Runtime.CompilerServices;

namespace WhiteTowerGames.DataFixerSharper.Extensions.Enums;

internal static class EnumCache<TEnum>
    where TEnum : struct, Enum
{
    public static readonly TEnum[] Values = Enum.GetValues<TEnum>();

    public static readonly bool IsFlags = typeof(TEnum).IsDefined(typeof(FlagsAttribute), false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong ToUInt64(TEnum value)
    {
        if (Unsafe.SizeOf<TEnum>() == 1)
            return Unsafe.As<TEnum, byte>(ref value);
        if (Unsafe.SizeOf<TEnum>() == 2)
            return Unsafe.As<TEnum, ushort>(ref value);
        if (Unsafe.SizeOf<TEnum>() == 4)
            return Unsafe.As<TEnum, uint>(ref value);
        if (Unsafe.SizeOf<TEnum>() == 8)
            return Unsafe.As<TEnum, ulong>(ref value);
        return 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum FromUInt64(ulong value)
    {
        TEnum result = default;
        if (Unsafe.SizeOf<TEnum>() == 1)
        {
            byte v = (byte)value;
            result = Unsafe.As<byte, TEnum>(ref v);
        }
        else if (Unsafe.SizeOf<TEnum>() == 2)
        {
            ushort v = (ushort)value;
            result = Unsafe.As<ushort, TEnum>(ref v);
        }
        else if (Unsafe.SizeOf<TEnum>() == 4)
        {
            uint v = (uint)value;
            result = Unsafe.As<uint, TEnum>(ref v);
        }
        else if (Unsafe.SizeOf<TEnum>() == 8)
        {
            result = Unsafe.As<ulong, TEnum>(ref value);
        }
        return result;
    }
}
