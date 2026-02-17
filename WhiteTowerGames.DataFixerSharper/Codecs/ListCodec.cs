using WhiteTowerGames.DataFixerSharper.Abstractions;

namespace WhiteTowerGames.DataFixerSharper.Codecs;

internal readonly struct ListCodec<TElement> : ICodec<List<TElement>>
{
    private readonly ICodec<TElement> _underlying;

    public ListCodec(ICodec<TElement> underlying)
    {
        _underlying = underlying;
    }

    public DataResult<(List<TElement>, TFormat)> Decode<TOps, TFormat>(TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat>
    {
        // make a list out of TFormat, accept every element in it, add it to our ref list.
        var consumer = new ListConsumer<TOps, TFormat>(_underlying, ops);
        var state = new DecodeState();
        var listResult = ops.ReadList(input, ref state, consumer);

        if (listResult.IsError) // was there any error parsing the encoded value?
            return DataResult<(List<TElement>, TFormat)>.Fail(listResult.ErrorMessage);

        if (state.IsError)
            return DataResult<(List<TElement>, TFormat)>.Fail(state.ErrorMessage);

        return DataResult<(List<TElement>, TFormat)>.Success((state.Elements, input));
    }

    public DataResult<TFormat> Encode<TOps, TFormat>(List<TElement> input, TOps ops, TFormat prefix)
        where TOps : IDynamicOps<TFormat>
    {
        var list = DataResult<TFormat>.Success(ops.CreateEmptyList());
        foreach (var item in input)
        {
            var encoded = _underlying.EncodeStart<TOps, TFormat>(ops, item);
            if (encoded.IsError)
                return encoded;

            var appendedValue = ops.AddToList(list.GetOrThrow(), encoded.GetOrThrow());
            if (list.IsError)
                return appendedValue;

            list = appendedValue;
        }
        var finalValue = ops.AppendToPrefix(prefix, list.GetOrThrow());
        return DataResult<TFormat>.Success(prefix);
    }

    private readonly struct ListConsumer<TOps, TFormat> : ICollectionConsumer<DecodeState, TFormat>
        where TOps : IDynamicOps<TFormat>
    {
        private readonly ICodec<TElement> _underlyingCodec;
        private readonly TOps _ops;

        public ListConsumer(ICodec<TElement> underlyingCodec, TOps ops)
        {
            _underlyingCodec = underlyingCodec;
            _ops = ops;
        }

        public void Accept(ref DecodeState collection, TFormat item)
        {
            if (collection.IsError)
                return;

            var decoded = _underlyingCodec.Parse(_ops, item);
            if (decoded.IsError)
                collection.ErrorStatus = DataResult<Unit>.Fail(decoded.ErrorMessage);
            else
                collection.Add(decoded.GetOrThrow());
        }
    }

    private ref struct DecodeState
    {
        public readonly List<TElement> Elements;
        public DataResult<Unit> ErrorStatus;

        public DecodeState()
        {
            Elements = new();
            ErrorStatus = DataResult<Unit>.Success(default);
        }

        public void Add(TElement item) => Elements.Add(item);

        public bool IsError => ErrorStatus.IsError;

        public string ErrorMessage => ErrorStatus.ErrorMessage;
    }
}
