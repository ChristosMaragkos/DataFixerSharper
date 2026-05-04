using System.Runtime.CompilerServices;
using WhiteTowerGames.DataFixerSharper.Abstractions;
using WhiteTowerGames.DataFixerSharper.Codecs;

namespace WhiteTowerGames.DataFixerSharper.Extensions.Enums;

public static class EnumCodecs
{
    public static ICodec<TEnum> EnumByValue<TEnum>()
        where TEnum : struct, Enum
    {
        if (Unsafe.SizeOf<TEnum>() != 4)
            throw new InvalidOperationException(
                $"EnumByValue<TEnum> requires a 32-bit backing type. {typeof(TEnum).Name} is {Unsafe.SizeOf<TEnum>()} bytes."
            );

        return BuiltinCodecs.Int32.Unsafe2SafeMap(
            value => Unsafe.As<TEnum, int>(ref value),
            integer =>
            {
                var enumValue = Unsafe.As<int, TEnum>(ref integer);

                return Enum.IsDefined(enumValue)
                    ? DataResult<TEnum>.Success(enumValue)
                    : DataResult<TEnum>.Fail(
                        $"Parsed value {integer} is not a valid value for enum {typeof(TEnum).Name}"
                    );
            }
        );
    }

    public static ICodec<TEnum> EnumByName<TEnum>()
        where TEnum : struct, Enum
    {
        return BuiltinCodecs.String.Unsafe2SafeMap(
            value => value.ToString(),
            str =>
                Enum.TryParse<TEnum>(str, false, out var result)
                    ? DataResult<TEnum>.Success(result)
                    : DataResult<TEnum>.Fail(
                        $"Parsed string '{str}' does not match any members of enum {typeof(TEnum).Name}"
                    )
        );
    }

    public static ICodec<TEnum> FlagsByName<TEnum>()
        where TEnum : struct, Enum
    {
        if (!EnumCache<TEnum>.IsFlags)
            throw new InvalidOperationException(
                $"{typeof(TEnum).Name} must be an enum annotated with [Flags]"
            );

        return BuiltinCodecs
            .String.ForList()
            .Unsafe2SafeMap(
                value =>
                {
                    var result = new List<string>();
                    ulong underlying = EnumCache<TEnum>.ToUInt64(value);

                    foreach (var enumValue in EnumCache<TEnum>.Values)
                    {
                        ulong bits = EnumCache<TEnum>.ToUInt64(enumValue);

                        if (bits == 0 || (bits & (bits - 1)) != 0)
                            continue;

                        if ((underlying & bits) == bits)
                        {
                            result.Add(enumValue.ToString());
                        }
                    }

                    return result;
                },
                strFlags =>
                {
                    ulong flags = 0;
                    foreach (var str in strFlags)
                    {
                        if (!Enum.TryParse<TEnum>(str, false, out var value))
                            return DataResult<TEnum>.Fail(
                                $"Parsed string '{str}' is not a valid flag for {typeof(TEnum).Name}"
                            );

                        flags |= EnumCache<TEnum>.ToUInt64(value);
                    }

                    return DataResult<TEnum>.Success(EnumCache<TEnum>.FromUInt64(flags));
                }
            );
    }
}
