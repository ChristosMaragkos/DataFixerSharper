using WhiteTowerGames.DataFixerSharper.Abstractions;
using WhiteTowerGames.DataFixerSharper.Codecs;

namespace WhiteTowerGames.DataFixerSharper;

public static class BuiltinCodecs
{
    public static readonly Codec<int> Int32 = Codec<int>.Primitive(
        (input, ops) => DataResult<object>.Success(((IDynamicOps<object>)ops).CreateInt32(input)),
        (ops, input) =>
            DataResult<(int, object)>.Success(
                ((((IDynamicOps<object>)ops).GetInt32(input)).GetOrThrow(), input) // My GOD I hate boxing. How do people write in Java?
            )
    );

    public static readonly Codec<long> Int64 = Codec<long>.Primitive(
        (input, ops) => DataResult<object>.Success(((IDynamicOps<object>)ops).CreateInt64(input)),
        (ops, input) =>
            DataResult<(long, object)>.Success(
                ((((IDynamicOps<object>)ops).GetInt64(input)).GetOrThrow(), input)
            )
    );

    public static readonly Codec<float> Float = Codec<float>.Primitive(
        (input, ops) => DataResult<object>.Success(((IDynamicOps<object>)ops).CreateFloat(input)),
        (ops, input) =>
            DataResult<(float, object)>.Success(
                ((((IDynamicOps<object>)ops).GetFloat(input)).GetOrThrow(), input)
            )
    );

    public static readonly Codec<double> Double = Codec<double>.Primitive(
        (input, ops) => DataResult<object>.Success(((IDynamicOps<object>)ops).CreateDouble(input)),
        (ops, input) =>
            DataResult<(double, object)>.Success(
                ((((IDynamicOps<object>)ops).GetDouble(input)).GetOrThrow(), input)
            )
    );

    public static readonly Codec<string> String = Codec<string>.Primitive(
        (input, ops) => DataResult<object>.Success(((IDynamicOps<object>)ops).CreateString(input)),
        (ops, input) =>
            DataResult<(string, object)>.Success(
                ((((IDynamicOps<object>)ops).GetString(input)).GetOrThrow(), input)
            )
    );

    public static readonly Codec<bool> Bool = Codec<bool>.Primitive(
        (input, ops) => DataResult<object>.Success(((IDynamicOps<object>)ops).CreateBool(input)),
        (ops, input) =>
            DataResult<(bool, object)>.Success(
                ((((IDynamicOps<object>)ops).GetBool(input)).GetOrThrow(), input)
            )
    );

    public static Codec<TEnum> EnumByValue<TEnum>()
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

    public static Codec<TEnum> EnumByName<TEnum>()
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

    public static Codec<TEnum> FlagsByName<TEnum>()
        where TEnum : struct, Enum
    {
        return String
            .ForArray()
            .Unsafe2SafeMap<TEnum>(
                value => FlagsToStringArray(value),
                strArray => StringArrayToFlags<TEnum>(strArray)
            );
    }

    private static string[] FlagsToStringArray<TEnum>(TEnum value)
        where TEnum : struct, Enum
    {
        var result = new List<string>();
        var underlying = Convert.ToUInt64(value);

        foreach (var enumValue in Enum.GetValues<TEnum>())
        {
            var bits = Convert.ToUInt64(enumValue);

            if (bits == 0 || (bits & (bits - 1)) != 0) // Evil bit hack
                continue;

            if ((underlying & bits) == bits)
            {
                result.Add(enumValue.ToString());
            }
        }

        return result.ToArray();
    }

    private static DataResult<TEnum> StringArrayToFlags<TEnum>(string[] strArray)
        where TEnum : struct, Enum
    {
        if (!typeof(TEnum).IsDefined(typeof(FlagsAttribute), false))
            return DataResult<TEnum>.Fail(
                $"{typeof(TEnum).FullName} must be an enum annotated with [Flags]"
            );

        ulong flags = 0;
        foreach (var str in strArray)
        {
            if (!Enum.TryParse(typeof(TEnum), str, false, out var value))
                return DataResult<TEnum>.Fail(
                    $"Parsed string value '{str}' does not match and members of enum {typeof(TEnum).FullName}",
                    (TEnum)(object)flags
                );

            var enumValue = Convert.ToUInt64(value);
            flags |= enumValue;
        }

        return DataResult<TEnum>.Success((TEnum)(object)flags);
    }
}
