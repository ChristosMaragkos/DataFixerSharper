using WhiteTowerGames.DataFixerSharper.Abstractions;

namespace WhiteTowerGames.DataFixerSharper.Codecs;

// A simple primitive codec. This can (hopefully) serialize all primitive types, but I had to use boxing to get it to work.
// I hate it. But the alternative was a single codec for every primitive type, which I hated even more.
internal class PrimitiveCodec<T> : Codec<T>
{
    private readonly Func<T, IDynamicOps, DataResult<object>> _encoder;
    private readonly Func<IDynamicOps, object, DataResult<(T, object)>> _decoder;

    public PrimitiveCodec(
        Func<T, IDynamicOps, DataResult<object>> encoder,
        Func<IDynamicOps, object, DataResult<(T, object)>> decoder
    )
    {
        _encoder = encoder;
        _decoder = decoder;
    }

    public override DataResult<(T, TFormat)> Decode<TOps, TFormat>(TOps ops, TFormat input)
    {
        var decoded = _decoder(ops, input!);
        if (decoded.IsError)
            return DataResult<(T, TFormat)>.Fail(
                $"Failed to decode primitive value: {decoded.ErrorMessage}"
            );

        return decoded.Map<(T, TFormat)>(result => (result.Item1, (TFormat)result.Item2));
    }

    public override DataResult<TFormat> Encode<TOps, TFormat>(T input, TOps ops, TFormat prefix)
    {
        var encoded = _encoder(input, ops);
        if (encoded.IsError)
            return DataResult<TFormat>.Fail(
                $"Failed to encode primitive value: {encoded.ErrorMessage}"
            );

        return encoded.Map<TFormat>(obj => (TFormat)obj);
    }
}
