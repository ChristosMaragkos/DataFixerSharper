using WhiteTowerGames.DataFixerSharper.Abstractions;

namespace WhiteTowerGames.DataFixerSharper.Codecs;

internal readonly struct DispatchCodec<TBase, TDis> : ICodec<TBase>
{
    private readonly Func<TBase, TDis> _discriminatorGetter;
    private readonly string _discriminatorKeyName;
    private readonly ICodec<TDis> _discriminatorCodec;
    private readonly Func<TDis, ICodec<TBase>> _codecGetter;

    public DispatchCodec(
        Func<TBase, TDis> discriminatorGetter,
        ICodec<TDis> discriminatorCodec,
        Func<TDis, ICodec<TBase>> codecGetter,
        string discriminatorKeyName = "type"
    )
    {
        _discriminatorGetter = discriminatorGetter;
        _discriminatorKeyName = discriminatorKeyName;
        _discriminatorCodec = discriminatorCodec;
        _codecGetter = codecGetter;
    }

    public DataResult<(TBase, TFormat)> Decode<TOps, TFormat>(TFormat input)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        var typeResult = TOps.GetValue(input, _discriminatorKeyName);
        if (typeResult.IsError)
            return DataResult<(TBase, TFormat)>.Fail(
                $"Input was missing polymorphic type discriminator named {_discriminatorKeyName} - [{typeResult.ErrorMessage}]"
            );

        var discrResult = _discriminatorCodec.Parse<TOps, TFormat>(typeResult.GetOrThrow());
        if (discrResult.IsError)
            return DataResult<(TBase, TFormat)>.Fail(
                $"Failed to decode type discriminator: [{discrResult.ErrorMessage}]"
            );

        var discriminator = discrResult.GetOrThrow();
        var innerCodec = _codecGetter(discriminator);

        var inputWithoutType = TOps.RemoveFromInput(input, _discriminatorKeyName);

        return innerCodec.Decode<TOps, TFormat>(inputWithoutType);
    }

    public DataResult<TFormat> Encode<TOps, TFormat>(TBase input, TFormat prefix)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        var discriminator = _discriminatorGetter(input);
        var typeKey = TOps.CreateString(_discriminatorKeyName);

        var discBuf = TOps.CreateEmptyBuffer();
        TOps.WriteMapStart(discBuf);
        TOps.WriteKey(discBuf, typeKey);
        var discrResult = _discriminatorCodec.Encode<TOps, TFormat>(discriminator, discBuf);
        if (discrResult.IsError)
            return discrResult;
        TOps.WriteMapEnd(discBuf);
        var discMap = TOps.FinalizeBuffer(discBuf);

        var innerCodec = _codecGetter(discriminator);
        var innerResult = innerCodec.EncodeStart<TOps, TFormat>(input);
        if (innerResult.IsError)
            return innerResult;

        var merged = TOps.AppendToPrefix(discMap, innerResult.GetOrThrow());
        return DataResult<TFormat>.Success(TOps.AppendToPrefix(prefix, merged));
    }
}
