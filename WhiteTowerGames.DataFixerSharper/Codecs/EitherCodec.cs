using WhiteTowerGames.DataFixerSharper.Abstractions;

namespace WhiteTowerGames.DataFixerSharper.Codecs;

internal readonly struct EitherCodec<T> : ICodec<T>
{
    public readonly ICodec<T> _first;
    public readonly ICodec<T> _second;

    public EitherCodec(ICodec<T> first, ICodec<T> second)
    {
        _first = first;
        _second = second;
    }

    public DataResult<(T, TFormat)> Decode<TOps, TFormat>(TFormat input)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        var firstTry = _first.Decode<TOps, TFormat>(input);
        if (!firstTry.IsError)
            return firstTry;

        return _second.Decode<TOps, TFormat>(input);
    }

    public DataResult<TFormat> Encode<TOps, TFormat>(T input, TFormat prefix)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        var firstTry = _first.Encode<TOps, TFormat>(input, prefix);
        if (!firstTry.IsError)
            return firstTry;

        return _second.Encode<TOps, TFormat>(input, prefix);
    }
}
