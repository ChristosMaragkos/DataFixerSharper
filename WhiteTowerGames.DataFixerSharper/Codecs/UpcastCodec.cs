using WhiteTowerGames.DataFixerSharper.Abstractions;

namespace WhiteTowerGames.DataFixerSharper.Codecs;

internal class UpcastCodec<TBase, TDer> : Codec<TBase>
    where TDer : TBase
{
    private readonly Codec<TDer> _underlying;

    public UpcastCodec(Codec<TDer> underlying)
    {
        _underlying = underlying;
    }

    public override DataResult<(TBase, TFormat)> Decode<TFormat>(
        IDynamicOps<TFormat> ops,
        TFormat input
    ) => _underlying.Decode(ops, input).Map(result => ((TBase)result.Item1, result.Item2));

    public override DataResult<TFormat> Encode<TFormat>(
        TBase input,
        IDynamicOps<TFormat> ops,
        TFormat prefix
    )
    {
        if (input is not TDer derived)
            return DataResult<TFormat>.Fail(
                $"Cannot encode polymorphically: expected object of type {typeof(TDer).FullName}, got {input!.GetType().FullName} instead"
            );

        return _underlying.Encode(derived, ops, prefix);
    }
}
