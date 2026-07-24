using WhiteTowerGames.DataFixerSharper.Abstractions;

namespace WhiteTowerGames.DataFixerSharper.Codecs;

internal readonly struct ListCodec<TElement> : ICodec<IList<TElement>>
{
    private readonly ICodec<TElement> _underlying;

    public ListCodec(ICodec<TElement> underlying)
    {
        _underlying = underlying;
    }

    public DataResult<(IList<TElement>, TFormat)> Decode<TOps, TFormat>(TFormat input)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        // make a list out of TFormat, accept every element in it, add it to our ref list.
        var consumer = new ListConsumer<TOps, TFormat>(_underlying);
        var state = new DecodeState();
        var listResult = TOps.ReadList(input, ref state, consumer);

        if (listResult.IsError) // was there any error parsing the encoded value?
            return DataResult<(IList<TElement>, TFormat)>.Fail(listResult.ErrorMessage);

        if (state.IsError)
            return DataResult<(IList<TElement>, TFormat)>.Fail(state.ErrorMessage);

        return DataResult<(IList<TElement>, TFormat)>.Success((state.Elements, input));
    }

    public DataResult<TFormat> Encode<TOps, TFormat>(IList<TElement> input, TFormat prefix)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        TOps.WriteListStart(prefix);
        var first = true;
        foreach (var item in input)
        {
            if (!first)
                TOps.WriteListSeparator(prefix);

            var result = _underlying.Encode<TOps, TFormat>(item, prefix);
            if (result.IsError)
                return result;
            first = false;
        }
        TOps.WriteListEnd(prefix);
        return DataResult<TFormat>.Success(prefix);
    }

    private readonly struct ListConsumer<TOps, TFormat> : ICollectionConsumer<DecodeState, TFormat>
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        private readonly ICodec<TElement> _underlyingCodec;

        public ListConsumer(ICodec<TElement> underlyingCodec)
        {
            _underlyingCodec = underlyingCodec;
        }

        public void Accept(ref DecodeState collection, TFormat item)
        {
            if (collection.IsError)
                return;

            var decoded = _underlyingCodec.Parse<TOps, TFormat>(item);
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
            Elements = [];
            ErrorStatus = DataResult<Unit>.Success(default);
        }

        public readonly void Add(TElement item) => Elements.Add(item);

        public readonly bool IsError => ErrorStatus.IsError;

        public readonly string ErrorMessage => ErrorStatus.ErrorMessage;
    }
}
