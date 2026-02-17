using WhiteTowerGames.DataFixerSharper.Abstractions;

namespace WhiteTowerGames.DataFixerSharper.Codecs;

internal readonly struct ConstantCodec<T> : ICodec<T>
{
    private readonly T _value;

    public ConstantCodec(T value)
    {
        _value = value;
    }

    public DataResult<(T, TFormat)> Decode<TOps, TFormat>(TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat> => DataResult<(T, TFormat)>.Success((_value, input));

    public DataResult<TFormat> Encode<TOps, TFormat>(T input, TOps ops, TFormat prefix)
        where TOps : IDynamicOps<TFormat> => DataResult<TFormat>.Success(ops.Empty());
}
