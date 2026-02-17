using WhiteTowerGames.DataFixerSharper.Abstractions;

namespace WhiteTowerGames.DataFixerSharper.Codecs;

// Internal types for mapping objects in order to create codecs for them,
// while allowing either or both of the conversions to return a DataResult.
// For example, let's say we want to create a codec for integers from a Codec<string>.
// All integers are valid strings, but not all strings are valid integers, so we want to
// fail early and avoid exceptions.

/// A <-> B always valid (for example, converting between an integer and a float)
internal readonly struct SafeMapCodec<TFrom, TTo> : ICodec<TTo>
{
    private readonly ICodec<TFrom> _underlying;
    private readonly Func<TFrom, TTo> _to;
    private readonly Func<TTo, TFrom> _from;

    public SafeMapCodec(ICodec<TFrom> underlying, Func<TFrom, TTo> to, Func<TTo, TFrom> from)
    {
        _underlying = underlying;
        _to = to;
        _from = from;
    }

    public DataResult<TFormat> Encode<TOps, TFormat>(TTo input, TOps ops, TFormat prefix)
        where TOps : IDynamicOps<TFormat>
    {
        return _underlying.Encode(_from(input), ops, prefix);
    }

    public DataResult<(TTo, TFormat)> Decode<TOps, TFormat>(TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat>
    {
        var decoded = _underlying.Decode(ops, input);
        if (decoded.IsError)
            return DataResult<(TTo, TFormat)>.Fail(decoded.ErrorMessage);

        return DataResult<(TTo, TFormat)>.Success(
            (_to(decoded.GetOrThrow().Item1), decoded.GetOrThrow().Item2)
        );
    }
}

// A <-> B not always valid (rare, but good to have)
internal readonly struct UnsafeMapCodec<TFrom, TTo> : ICodec<TTo>
{
    private readonly ICodec<TFrom> _underlying;
    private readonly Func<TFrom, DataResult<TTo>> _to;
    private readonly Func<TTo, DataResult<TFrom>> _from;

    public UnsafeMapCodec(
        ICodec<TFrom> underlying,
        Func<TFrom, DataResult<TTo>> to,
        Func<TTo, DataResult<TFrom>> from
    )
    {
        _underlying = underlying;
        _to = to;
        _from = from;
    }

    public DataResult<TFormat> Encode<TOps, TFormat>(TTo input, TOps ops, TFormat prefix)
        where TOps : IDynamicOps<TFormat>
    {
        var transformed = _from(input);
        if (transformed.IsError)
            return DataResult<TFormat>.Fail(transformed.ErrorMessage);

        var value = transformed.GetOrThrow();

        return _underlying.Encode(value, ops, prefix);
    }

    public DataResult<(TTo, TFormat)> Decode<TOps, TFormat>(TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat>
    {
        var decoded = _underlying.Decode(ops, input);
        if (decoded.IsError)
            return DataResult<(TTo, TFormat)>.Fail(decoded.ErrorMessage);

        var pair = decoded.GetOrThrow();
        var transformed = _to(pair.Item1);
        if (transformed.IsError)
            return DataResult<(TTo, TFormat)>.Fail(transformed.ErrorMessage);

        return DataResult<(TTo, TFormat)>.Success((transformed.GetOrThrow(), pair.Item2));
    }
}

// A -> B always valid, B -> A not always valid (for example, string to integer)
internal readonly struct Safe2UnsafeMapCodec<TFrom, TTo> : ICodec<TTo>
{
    private readonly ICodec<TFrom> _underlying;
    private readonly Func<TFrom, TTo> _to;
    private readonly Func<TTo, DataResult<TFrom>> _from;

    public Safe2UnsafeMapCodec(
        ICodec<TFrom> underlying,
        Func<TFrom, TTo> to,
        Func<TTo, DataResult<TFrom>> from
    )
    {
        _underlying = underlying;
        _to = to;
        _from = from;
    }

    public DataResult<TFormat> Encode<TOps, TFormat>(TTo input, TOps ops, TFormat prefix)
        where TOps : IDynamicOps<TFormat>
    {
        var transformed = _from(input);
        if (transformed.IsError)
            return DataResult<TFormat>.Fail(transformed.ErrorMessage);

        var value = transformed.GetOrThrow();

        return _underlying.Encode(value, ops, prefix);
    }

    public DataResult<(TTo, TFormat)> Decode<TOps, TFormat>(TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat>
    {
        var decoded = _underlying.Decode(ops, input);
        if (decoded.IsError)
            return DataResult<(TTo, TFormat)>.Fail(decoded.ErrorMessage);

        return DataResult<(TTo, TFormat)>.Success(
            (_to(decoded.GetOrThrow().Item1), decoded.GetOrThrow().Item2)
        );
    }
}

// A -> B not always valid, but B -> A always valid (for example, float array to 3D vector)
internal readonly struct Unsafe2SafeMapCodec<TFrom, TTo> : ICodec<TTo>
{
    private readonly ICodec<TFrom> _underlying;
    private readonly Func<TFrom, DataResult<TTo>> _to;
    private readonly Func<TTo, TFrom> _from;

    public Unsafe2SafeMapCodec(
        ICodec<TFrom> underlying,
        Func<TFrom, DataResult<TTo>> to,
        Func<TTo, TFrom> from
    )
    {
        _underlying = underlying;
        _to = to;
        _from = from;
    }

    public DataResult<TFormat> Encode<TOps, TFormat>(TTo input, TOps ops, TFormat prefix)
        where TOps : IDynamicOps<TFormat>
    {
        return _underlying.Encode(_from(input), ops, prefix);
    }

    public DataResult<(TTo, TFormat)> Decode<TOps, TFormat>(TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat>
    {
        var decoded = _underlying.Decode(ops, input);
        if (decoded.IsError)
            return DataResult<(TTo, TFormat)>.Fail(decoded.ErrorMessage);

        var pair = decoded.GetOrThrow();
        var transformed = _to(pair.Item1);
        if (transformed.IsError)
            return DataResult<(TTo, TFormat)>.Fail(transformed.ErrorMessage);

        return DataResult<(TTo, TFormat)>.Success((transformed.GetOrThrow(), pair.Item2));
    }
}
