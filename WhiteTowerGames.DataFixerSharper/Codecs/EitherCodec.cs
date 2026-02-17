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

    public DataResult<(T, TFormat)> Decode<TOps, TFormat>(TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat>
    {
        var firstTry = _first.Decode(ops, input);
        if (!firstTry.IsError)
            return firstTry;

        return _second.Decode(ops, input);
    }

    public DataResult<TFormat> Encode<TOps, TFormat>(T input, TOps ops, TFormat prefix)
        where TOps : IDynamicOps<TFormat>
    {
        var firstTry = _first.Encode(input, ops, prefix);
        if (!firstTry.IsError)
            return firstTry;

        return _second.Encode(input, ops, prefix);
    }
}
