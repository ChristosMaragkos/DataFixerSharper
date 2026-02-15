using WhiteTowerGames.DataFixerSharper.Abstractions;

namespace WhiteTowerGames.DataFixerSharper.Codecs;

internal class EnumerableCodec<T> : Codec<IEnumerable<T>>
{
    private readonly Codec<T> _underlying;

    public EnumerableCodec(Codec<T> underlying)
    {
        _underlying = underlying;
    }

    public override DataResult<(IEnumerable<T>, TFormat)> Decode<TFormat>(
        IDynamicOps<TFormat> ops,
        TFormat input
    )
    {
        var streamResult = ops.ReadAsStream(input);
        if (streamResult.IsError)
            return DataResult<(IEnumerable<T>, TFormat)>.Fail(
                $"Not a valid array: {streamResult.ErrorMessage}"
            );

        var stream = streamResult.ResultOrPartial();
        var result = new List<T>();

        foreach (var item in stream)
        {
            var decoded = _underlying.Parse(ops, item);
            if (decoded.IsError)
                return DataResult<(IEnumerable<T>, TFormat)>.Fail(
                    $"Failed to decode element: {decoded.ErrorMessage}"
                );

            result.Add(
                decoded.IsError
                    ? _underlying.Parse(ops, ops.Empty()).GetOrThrow()
                    : decoded.ResultOrPartial()
            );
            ops.RemoveFromInput(input, item);
        }

        return DataResult<(IEnumerable<T>, TFormat)>.Success((result, input));
    }

    public override DataResult<TFormat> Encode<TFormat>(
        IEnumerable<T> input,
        IDynamicOps<TFormat> ops,
        TFormat prefix
    )
    {
        var encodedElements = new List<TFormat>();
        foreach (var item in input)
        {
            var encoded = _underlying.EncodeStart(ops, item);
            if (encoded.IsError)
                return DataResult<TFormat>.Fail(
                    $"Failed to encode element: {encoded.ErrorMessage}"
                );
            var result = encoded.GetOrThrow();

            encodedElements.Add(result);
            ops.AppendToPrefix(prefix, result);
        }

        return DataResult<TFormat>.Success(ops.CreateList(encodedElements));
    }
}
