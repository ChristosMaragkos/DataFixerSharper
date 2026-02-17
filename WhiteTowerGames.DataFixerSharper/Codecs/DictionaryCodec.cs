using WhiteTowerGames.DataFixerSharper.Abstractions;

namespace WhiteTowerGames.DataFixerSharper.Codecs;

internal readonly struct DictionaryCodec<TKey, TValue> : ICodec<Dictionary<TKey, TValue>>
    where TKey : notnull
{
    private readonly ICodec<TKey> _keyCodec;
    private readonly ICodec<TValue> _valueCodec;

    public DictionaryCodec(ICodec<TKey> keyCodec, ICodec<TValue> valueCodec)
    {
        _keyCodec = keyCodec;
        _valueCodec = valueCodec;
    }

    public DataResult<(Dictionary<TKey, TValue>, TFormat)> Decode<TOps, TFormat>(
        TOps ops,
        TFormat input
    )
        where TOps : IDynamicOps<TFormat>
    {
        var consumer = new DictConsumer<TOps, TFormat>(_keyCodec, _valueCodec, ops);
        var state = new DecodeState();
        var mapResult = ops.ReadMap(input, ref state, consumer);

        if (mapResult.IsError)
            return Fail(mapResult.ErrorMessage);

        if (state.IsError)
            return Fail(state.ErrorMessage);

        return Success(state.Dictionary, input);

        DataResult<(Dictionary<TKey, TValue>, TFormat)> Fail(string errorMessage) =>
            DataResult<(Dictionary<TKey, TValue>, TFormat)>.Fail(errorMessage);

        DataResult<(Dictionary<TKey, TValue>, TFormat)> Success(
            Dictionary<TKey, TValue> result,
            TFormat input
        ) => DataResult<(Dictionary<TKey, TValue>, TFormat)>.Success((result, input));
    }

    public DataResult<TFormat> Encode<TOps, TFormat>(
        Dictionary<TKey, TValue> input,
        TOps ops,
        TFormat prefix
    )
        where TOps : IDynamicOps<TFormat>
    {
        var map = ops.CreateEmptyMap();
        foreach (var kvp in input)
        {
            var keyEnc = _keyCodec.EncodeStart<TOps, TFormat>(ops, kvp.Key);
            if (keyEnc.IsError)
                return keyEnc;

            var valEnc = _valueCodec.EncodeStart<TOps, TFormat>(ops, kvp.Value);
            if (valEnc.IsError)
                return valEnc;

            var appended = ops.AddToMap(map, keyEnc.GetOrThrow(), valEnc.GetOrThrow());
            if (appended.IsError) // just in case
                return appended;

            map = appended.GetOrThrow();
        }

        var finalPrefix = ops.AppendToPrefix(prefix, map);
        return DataResult<TFormat>.Success(finalPrefix);
    }

    private readonly struct DictConsumer<TOps, TFormat> : IMapConsumer<DecodeState, TFormat>
        where TOps : IDynamicOps<TFormat>
    {
        private readonly ICodec<TKey> _keyCodec;
        private readonly ICodec<TValue> _valueCodec;
        private readonly TOps _ops;

        public DictConsumer(ICodec<TKey> keyCodec, ICodec<TValue> valueCodec, TOps ops)
        {
            _keyCodec = keyCodec;
            _valueCodec = valueCodec;
            _ops = ops;
        }

        public void Accept(ref DecodeState map, TFormat key, TFormat value)
        {
            if (map.IsError)
                return;

            var keyDec = _keyCodec.Parse(_ops, key);
            if (keyDec.IsError)
            {
                map.ErrorState = DataResult<Unit>.Fail(keyDec.ErrorMessage);
                return;
            }

            var valDec = _valueCodec.Parse(_ops, value);
            if (valDec.IsError)
            {
                map.ErrorState = DataResult<Unit>.Fail(valDec.ErrorMessage);
                return;
            }

            map.Add(keyDec.GetOrThrow(), valDec.GetOrThrow());
        }
    }

    private ref struct DecodeState
    {
        public readonly Dictionary<TKey, TValue> Dictionary;
        public DataResult<Unit> ErrorState;

        public DecodeState()
        {
            Dictionary = new();
            ErrorState = DataResult<Unit>.Success(default!);
        }

        public void Add(TKey key, TValue value) => Dictionary[key] = value;

        public bool IsError => ErrorState.IsError;
        public string ErrorMessage => ErrorState.ErrorMessage;
    }
}
