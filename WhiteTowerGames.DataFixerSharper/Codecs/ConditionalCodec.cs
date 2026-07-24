using WhiteTowerGames.DataFixerSharper.Abstractions;

namespace WhiteTowerGames.DataFixerSharper.Codecs;

internal readonly struct ConditionalCodec<T> : ICodec<T>
{
    private readonly ICodec<T> _underlying;
    private readonly Predicate<T> _condition;

    public ConditionalCodec(ICodec<T> underlying, Predicate<T> condition)
    {
        _underlying = underlying;
        _condition = condition;
    }

    public DataResult<(T, TFormat)> Decode<TOps, TFormat>(TFormat input)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        var decoded = _underlying.Decode<TOps, TFormat>(input);
        if (decoded.IsError)
            return decoded;

        if (!_condition(decoded.GetOrThrow().Item1))
            return DataResult<(T, TFormat)>.Fail("Input did not comply with the given condition.");

        return decoded;
    }

    public DataResult<TFormat> Encode<TOps, TFormat>(T input, TFormat prefix)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        if (!_condition(input))
            return DataResult<TFormat>.Fail("Input did not comply with the given condition.");
        var encoded = _underlying.Encode<TOps, TFormat>(input, prefix);
        return encoded;
    }
}
