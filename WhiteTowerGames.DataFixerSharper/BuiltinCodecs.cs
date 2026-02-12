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

    // TODO: Enums (by value, by name, flags to string array)
}
