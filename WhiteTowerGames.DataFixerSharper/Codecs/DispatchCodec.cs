using WhiteTowerGames.DataFixerSharper.Abstractions;

namespace WhiteTowerGames.DataFixerSharper.Codecs;

internal class DispatchCodec<TBase, TDis> : Codec<TBase>
{
    private readonly Func<TBase, TDis> _discriminatorGetter;
    private readonly string _discriminatorKeyName;
    private readonly Codec<TDis> _discriminatorCodec;
    private readonly Func<TDis, Codec<TBase>> _codecGetter;

    public DispatchCodec(
        Func<TBase, TDis> discriminatorGetter,
        Codec<TDis> discriminatorCodec,
        Func<TDis, Codec<TBase>> codecGetter,
        string discriminatorKeyName = "type"
    )
    {
        _discriminatorGetter = discriminatorGetter;
        _discriminatorKeyName = discriminatorKeyName;
        _discriminatorCodec = discriminatorCodec;
        _codecGetter = codecGetter;
    }

    public override DataResult<(TBase, TFormat)> Decode<TOps, TFormat>(TOps ops, TFormat input)
    {
        var discrField = ops.GetValue(input, _discriminatorKeyName);
        if (discrField.IsError)
            return DataResult<(TBase, TFormat)>.Fail(
                $"Input was missing polymorphic type discriminator named {_discriminatorKeyName} - [{discrField.ErrorMessage}]"
            );

        var decodedDiscr = _discriminatorCodec.Parse<TOps, TFormat>(ops, discrField.GetOrThrow());
        if (decodedDiscr.IsError)
            return DataResult<(TBase, TFormat)>.Fail(
                $"Failed to decode type discriminator: [{decodedDiscr.ErrorMessage}]"
            );

        var discriminator = decodedDiscr.GetOrThrow();
        var codec = _codecGetter(discriminator);

        ops.RemoveFromInput(
            input,
            ops.Merge(ops.CreateString(_discriminatorKeyName), discrField.GetOrThrow())
        );

        var valueResult = codec.Decode(ops, input);
        return valueResult;
    }

    public override DataResult<TFormat> Encode<TOps, TFormat>(TBase input, TOps ops, TFormat prefix)
    {
        var discr = _discriminatorGetter(input);
        var discrResult = _discriminatorCodec.EncodeStart<TOps, TFormat>(ops, discr);

        if (discrResult.IsError)
            return discrResult;

        // Add type discriminator to prefix
        var discriminated = ops.MergeAndAppend(
            prefix,
            ops.CreateString(_discriminatorKeyName)!,
            discrResult.GetOrThrow()
        );

        var codec = _codecGetter(discr);
        var valueResult = codec.Encode(input, ops, discriminated);

        return valueResult;
    }
}
