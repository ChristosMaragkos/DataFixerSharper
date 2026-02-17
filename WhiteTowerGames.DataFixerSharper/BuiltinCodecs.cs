using WhiteTowerGames.DataFixerSharper.Abstractions;
using WhiteTowerGames.DataFixerSharper.Codecs;
using WhiteTowerGames.DataFixerSharper.Codecs.PrimitiveCodec;

namespace WhiteTowerGames.DataFixerSharper;

public static class BuiltinCodecs
{
    public static readonly ICodec<int> Int32 = new Int32Codec();

    public static readonly ICodec<long> Int64 = new Int64Codec();

    public static readonly ICodec<float> Float = new FloatCodec();

    public static readonly ICodec<double> Double = new DoubleCodec();

    public static readonly ICodec<string> String = new StringCodec();

    public static readonly ICodec<bool> Bool = new BoolCodec();

    public static ICodec<TEnum> EnumByValue<TEnum>()
        where TEnum : struct, Enum
    {
        return Int32.Unsafe2SafeMap(
            value => (int)(object)value,
            integer =>
                Enum.IsDefined(typeof(TEnum), integer)
                    ? DataResult<TEnum>.Success((TEnum)(object)integer)
                    : DataResult<TEnum>.Fail(
                        $"Parsed value {integer} was not a valid value for enum {typeof(TEnum).FullName}"
                    )
        );
    }

    public static ICodec<TEnum> EnumByName<TEnum>()
        where TEnum : struct, Enum
    {
        return String.Unsafe2SafeMap(
            value => Enum.Format(typeof(TEnum), value, "F"),
            str =>
                Enum.TryParse(typeof(TEnum), str, false, out object? value)
                    ? DataResult<TEnum>.Success((TEnum)value)
                    : DataResult<TEnum>.Fail(
                        $"Parsed string value '{str}' does not match any members of enum {typeof(TEnum).FullName}"
                    )
        );
    }

    public static ICodec<TEnum> FlagsByName<TEnum>()
        where TEnum : struct, Enum
    {
        return String
            .ForList()
            .Unsafe2SafeMap<TEnum>(
                value => FlagsToStringArray(value),
                strArray => StringArrayToFlags<TEnum>(strArray)
            );
    }

    private static List<string> FlagsToStringArray<TEnum>(TEnum value)
        where TEnum : struct, Enum
    {
        var result = new List<string>();
        var underlying = Convert.ToUInt64(value);

        foreach (var enumValue in Enum.GetValues<TEnum>())
        {
            var bits = Convert.ToUInt64(enumValue);

            if (bits == 0 || (bits & (bits - 1)) != 0)
                continue;

            if ((underlying & bits) == bits)
            {
                result.Add(enumValue.ToString());
            }
        }

        return result;
    }

    private static DataResult<TEnum> StringArrayToFlags<TEnum>(List<string> strFlags)
        where TEnum : struct, Enum
    {
        if (!typeof(TEnum).IsDefined(typeof(FlagsAttribute), false))
            return DataResult<TEnum>.Fail(
                $"{typeof(TEnum).FullName} must be an enum annotated with [Flags]"
            );

        ulong flags = 0;
        foreach (var str in strFlags)
        {
            if (!Enum.TryParse(typeof(TEnum), str, false, out var value))
                return DataResult<TEnum>.Fail(
                    $"Parsed string value '{str}' does not match and members of enum {typeof(TEnum).FullName}"
                );

            var enumValue = Convert.ToUInt64(value);
            flags |= enumValue;
        }

        var underlyingType = Enum.GetUnderlyingType(typeof(TEnum));
        object castValue = underlyingType switch
        {
            Type t when t == typeof(byte) => (TEnum)(object)(byte)flags,
            Type t when t == typeof(sbyte) => (TEnum)(object)(sbyte)flags,
            Type t when t == typeof(short) => (TEnum)(object)(short)flags,
            Type t when t == typeof(ushort) => (TEnum)(object)(ushort)flags,
            Type t when t == typeof(int) => (TEnum)(object)(int)flags,
            Type t when t == typeof(uint) => (TEnum)(object)(uint)flags,
            Type t when t == typeof(long) => (TEnum)(object)(long)flags,
            Type t when t == typeof(ulong) => (TEnum)(object)flags,
            _ => throw new InvalidOperationException("Unsupported enum underlying type"),
        };

        return DataResult<TEnum>.Success((TEnum)castValue);
    }
}
