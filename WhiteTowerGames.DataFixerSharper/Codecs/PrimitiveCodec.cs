using System.Runtime.CompilerServices;
using WhiteTowerGames.DataFixerSharper.Abstractions;

namespace WhiteTowerGames.DataFixerSharper.Codecs;

// A simple primitive codec. This can (hopefully) serialize all primitive types, but I had to use boxing to get it to work.
// I hate it. But the alternative was a single codec for every primitive type, which I hated even more.
internal class PrimitiveCodec<T> : Codec<T>
{
    private readonly object _encoder;
    private readonly object _decoder;

    public PrimitiveCodec(object encoder, object decoder)
    {
        _encoder = encoder;
        _decoder = decoder;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override DataResult<(T, TFormat)> Decode<TFormat>(
        IDynamicOps<TFormat> ops,
        TFormat input
    )
    {
        var decoder = (Func<IDynamicOps, TFormat, DataResult<(T, TFormat)>>)_decoder;
        var decoded = decoder(ops, input!);
        if (decoded.IsError)
            return DataResult<(T, TFormat)>.Fail(decoded.ErrorMessage);

        return decoded.Map<(T, TFormat)>(result => (result.Item1, (TFormat)(result.Item2)));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override DataResult<TFormat> Encode<TFormat>(
        T input,
        IDynamicOps<TFormat> ops,
        TFormat prefix
    )
    {
        var encoder = (Func<T, IDynamicOps, DataResult<TFormat>>)_encoder;
        var encoded = encoder(input, ops);
        if (encoded.IsError)
            return DataResult<TFormat>.Fail(encoded.ErrorMessage);

        return encoded.Map(obj => (TFormat)obj);
    }
}
