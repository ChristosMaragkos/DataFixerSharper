using WhiteTowerGames.DataFixerSharper.Abstractions;

namespace WhiteTowerGames.DataFixerSharper.Codecs;

internal class DictionaryCodec<TKey, TValue> : Codec<Dictionary<TKey, TValue>>
    where TKey : notnull
{
    private readonly Codec<TKey> _keyCodec;
    private readonly Codec<TValue> _valueCodec;

    public DictionaryCodec(Codec<TKey> keyCodec, Codec<TValue> valueCodec)
    {
        _keyCodec = keyCodec;
        _valueCodec = valueCodec;
    }

    public override DataResult<(Dictionary<TKey, TValue>, TFormat)> Decode<TOps, TFormat>(
        TOps ops,
        TFormat input
    )
    {
        var mapResult = ops.ReadMap(input);

        if (mapResult.IsError)
            return DataResult<(Dictionary<TKey, TValue>, TFormat)>.Fail(
                "Input was not a valid dictionary/map."
            );

        var map = mapResult.ResultOrPartial().ToArray();

        var dict = new Dictionary<TKey, TValue>();

        foreach (var kvp in map)
        {
            var decodedKey = _keyCodec.Decode(ops, kvp.Key);
            if (decodedKey.IsError)
                return DataResult<(Dictionary<TKey, TValue>, TFormat)>.Fail(
                    $"Failed to decode key [{decodedKey.ErrorMessage}]"
                );

            var decodedVal = _valueCodec.Decode(ops, kvp.Value);
            if (decodedVal.IsError)
                return DataResult<(Dictionary<TKey, TValue>, TFormat)>.Fail(
                    $"Failed to decode value [{decodedVal.ErrorMessage}]"
                );

            var keyResult = decodedKey.GetOrThrow().Item1;
            var valResult = decodedVal.GetOrThrow().Item1;
            dict[keyResult] = valResult;
            ops.RemoveFromInput(input, kvp.Key);
        }
        return DataResult<(Dictionary<TKey, TValue>, TFormat)>.Success((dict, input));
    }

    public override DataResult<TFormat> Encode<TOps, TFormat>(
        Dictionary<TKey, TValue> input,
        TOps ops,
        TFormat prefix
    )
    {
        var encodedMap = new List<KeyValuePair<TFormat, TFormat>>();

        foreach (var kvp in input)
        {
            var encodedKey = _keyCodec.EncodeStart<TOps, TFormat>(ops, kvp.Key);
            if (encodedKey.IsError)
                return DataResult<TFormat>.Fail(
                    $"Failed to encode key [{encodedKey.ErrorMessage}]"
                );

            var encodedValue = _valueCodec.EncodeStart<TOps, TFormat>(ops, kvp.Value);
            if (encodedValue.IsError)
                return DataResult<TFormat>.Fail(
                    $"Failed to encode value [{encodedValue.ErrorMessage}]"
                );

            encodedMap.Add(new(encodedKey.GetOrThrow(), encodedValue.GetOrThrow()));
        }

        return DataResult<TFormat>.Success(
            ops.AppendToPrefix(prefix, ops.CreateEmptyMap(encodedMap))
        );
    }
}
