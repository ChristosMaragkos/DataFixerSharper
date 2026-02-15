using WhiteTowerGames.DataFixerSharper.Abstractions;

namespace WhiteTowerGames.DataFixerSharper.Codecs;

internal class EitherCodec<T> : Codec<T>
{
    public readonly Codec<T> _first;
    public readonly Codec<T> _second;

    public EitherCodec(Codec<T> first, Codec<T> second)
    {
        _first = first;
        _second = second;
    }

    public override DataResult<(T, TFormat)> Decode<TFormat>(
        Abstractions.IDynamicOps<TFormat> ops,
        TFormat input
    )
    {
        var firstTry = _first.Decode(ops, input);
        if (!firstTry.IsError)
            return firstTry;

        var secondTry = _second.Decode(ops, input);
        if (!secondTry.IsError)
            return secondTry;

        return DataResult<(T, TFormat)>.Fail(secondTry.ErrorMessage);
    }

    public override DataResult<TFormat> Encode<TFormat>(
        T input,
        Abstractions.IDynamicOps<TFormat> ops,
        TFormat prefix
    )
    {
        var firstTry = _first.Encode(input, ops, prefix);
        if (!firstTry.IsError)
            return firstTry;

        var secondTry = _second.Encode(input, ops, prefix);
        if (!secondTry.IsError)
            return secondTry;

        return DataResult<TFormat>.Fail(secondTry.ErrorMessage);
    }
}
