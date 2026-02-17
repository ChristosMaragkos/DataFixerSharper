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

    public DataResult<(TBase, TFormat)> Decode<TOps, TFormat>(TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat>
    {
        var typeResult = ops.GetValue(input, _discriminatorKeyName);
        if (typeResult.IsError)
            return DataResult<(TBase, TFormat)>.Fail(
                $"Input was missing polymorphic type discriminator named {_discriminatorKeyName} - [{typeResult.ErrorMessage}]"
            );

        var discrResult = _discriminatorCodec.Parse<TOps, TFormat>(ops, typeResult.GetOrThrow());
        if (discrResult.IsError)
            return DataResult<(TBase, TFormat)>.Fail(
                $"Failed to decode type discriminator: [{discrResult.ErrorMessage}]"
            );

        var discriminator = discrResult.GetOrThrow();
        var typeKey = ops.CreateString(_discriminatorKeyName);
        var innerCodec = _codecGetter(discriminator);

        var inputWithoutType = ops.RemoveFromInput(input, typeKey);

        return innerCodec.Decode(ops, inputWithoutType);
    }

    public DataResult<TFormat> Encode<TOps, TFormat>(TBase input, TOps ops, TFormat prefix)
        where TOps : IDynamicOps<TFormat>
    {
        var discriminator = _discriminatorGetter(input);
        var discrEncoded = _discriminatorCodec.EncodeStart<TOps, TFormat>(ops, discriminator);
        if (discrEncoded.IsError)
            return discrEncoded;

        var map = ops.CreateEmptyMap();
        var typeKey = ops.CreateString(_discriminatorKeyName);

        var typedMap = ops.AddToMap(map, typeKey, discrEncoded.GetOrThrow());
        if (typedMap.IsError)
            return typedMap;

        var combinedPrefix = ops.AppendToPrefix(prefix, typedMap.GetOrThrow());

        var innerCodec = _codecGetter(discriminator);
        return innerCodec.Encode(input, ops, combinedPrefix);
    }
}
