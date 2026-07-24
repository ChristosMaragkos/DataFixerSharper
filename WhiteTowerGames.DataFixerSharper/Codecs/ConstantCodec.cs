using WhiteTowerGames.DataFixerSharper.Abstractions;

namespace WhiteTowerGames.DataFixerSharper.Codecs;

internal readonly struct ConstantCodec<T> : ICodec<T>
{
    private readonly T _value;

    public ConstantCodec(T value)
    {
        _value = value;
    }

    public DataResult<(T, TFormat)> Decode<TOps, TFormat>(TFormat input)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct => DataResult<(T, TFormat)>.Success((_value, input));

    public DataResult<TFormat> Encode<TOps, TFormat>(T input, TFormat prefix)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct => DataResult<TFormat>.Success(TOps.Empty());
}
