using WhiteTowerGames.DataFixerSharper.Abstractions;

namespace WhiteTowerGames.DataFixerSharper.Codecs;

internal readonly struct UpcastCodec<TBase, TDer> : ICodec<TBase>
    where TDer : TBase
{
    private readonly ICodec<TDer> _underlying;

    public UpcastCodec(ICodec<TDer> underlying)
    {
        _underlying = underlying;
    }

    public DataResult<(TBase, TFormat)> Decode<TOps, TFormat>(TFormat input)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        var decodedResult = _underlying.Decode<TOps, TFormat>(input);
        if (decodedResult.IsError)
            return DataResult<(TBase, TFormat)>.Fail(decodedResult.ErrorMessage);

        var decoded = decodedResult.GetOrThrow();
        return DataResult<(TBase, TFormat)>.Success((decoded.Item1, decoded.Item2));
    }

    public DataResult<TFormat> Encode<TOps, TFormat>(TBase input, TFormat prefix)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        if (input is not TDer derived)
            return DataResult<TFormat>.Fail(
                $"Cannot encode polymorphically: expected object of type {typeof(TDer).Name}"
            );

        return _underlying.Encode<TOps, TFormat>(derived, prefix);
    }
}
