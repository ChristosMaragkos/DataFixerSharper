using WhiteTowerGames.DataFixerSharper.Abstractions;

namespace WhiteTowerGames.DataFixerSharper.Codecs;

internal class ConstantCodec<T> : Codec<T>
{
    private readonly T _value;

    public ConstantCodec(T value)
    {
        _value = value;
    }

    public override DataResult<(T, TFormat)> Decode<TFormat>(
        IDynamicOps<TFormat> ops,
        TFormat input
    ) => DataResult<(T, TFormat)>.Success((_value, input));

    public override DataResult<TFormat> Encode<TFormat>(
        T input,
        Abstractions.IDynamicOps<TFormat> ops,
        TFormat prefix
    ) => DataResult<TFormat>.Success(ops.Empty());
}
