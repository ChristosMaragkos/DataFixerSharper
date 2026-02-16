using WhiteTowerGames.DataFixerSharper.Abstractions;

namespace WhiteTowerGames.DataFixerSharper.Codecs;

// Internal types for mapping objects in order to create codecs for them,
// while allowing either or both of the conversions to return a DataResult.
// For example, let's say we want to create a codec for integers from a Codec<string>.
// All integers are valid strings, but not all strings are valid integers, so we want to
// fail early and avoid exceptions.

/// A <-> B always valid (for example, converting between an integer and a float)
internal class SafeMapCodec<TFrom, TTo> : Codec<TTo>
{
    private readonly Codec<TFrom> _underlying;
    private readonly Func<TFrom, TTo> _to;
    private readonly Func<TTo, TFrom> _from;

    public SafeMapCodec(Codec<TFrom> underlying, Func<TFrom, TTo> to, Func<TTo, TFrom> from)
    {
        _underlying = underlying;
        _to = to;
        _from = from;
    }

    public override DataResult<TFormat> Encode<TOps, TFormat>(TTo input, TOps ops, TFormat prefix)
    {
        return _underlying.Encode(_from(input), ops, prefix);
    }

    public override DataResult<(TTo, TFormat)> Decode<TOps, TFormat>(TOps ops, TFormat input)
    {
        return _underlying.Decode(ops, input).Map(pair => (_to(pair.Item1), pair.Item2));
    }
}

// A <-> B not always valid (rare, but good to have)
internal class UnsafeMapCodec<TFrom, TTo> : Codec<TTo>
{
    private readonly Codec<TFrom> _underlying;
    private readonly Func<TFrom, DataResult<TTo>> _to;
    private readonly Func<TTo, DataResult<TFrom>> _from;

    public UnsafeMapCodec(
        Codec<TFrom> underlying,
        Func<TFrom, DataResult<TTo>> to,
        Func<TTo, DataResult<TFrom>> from
    )
    {
        _underlying = underlying;
        _to = to;
        _from = from;
    }

    public override DataResult<TFormat> Encode<TOps, TFormat>(TTo input, TOps ops, TFormat prefix)
    {
        var transformed = _from(input);
        if (transformed.IsError)
            return DataResult<TFormat>.Fail(transformed.ErrorMessage);

        var value = transformed.GetOrThrow();

        return _underlying.Encode(value, ops, prefix);
    }

    public override DataResult<(TTo, TFormat)> Decode<TOps, TFormat>(TOps ops, TFormat input)
    {
        return _underlying
            .Decode(ops, input)
            .UnsafeMap(pair => _to(pair.Item1).Map(mapped => (mapped, pair.Item2)));
    }
}

// A -> B always valid, B -> A not always valid (for example, string to integer)
internal class Safe2UnsafeMapCodec<TFrom, TTo> : Codec<TTo>
{
    private readonly Codec<TFrom> _underlying;
    private readonly Func<TFrom, TTo> _to;
    private readonly Func<TTo, DataResult<TFrom>> _from;

    public Safe2UnsafeMapCodec(
        Codec<TFrom> underlying,
        Func<TFrom, TTo> to,
        Func<TTo, DataResult<TFrom>> from
    )
    {
        _underlying = underlying;
        _to = to;
        _from = from;
    }

    public override DataResult<TFormat> Encode<TOps, TFormat>(TTo input, TOps ops, TFormat prefix)
    {
        var transformed = _from(input);
        if (transformed.IsError)
            return DataResult<TFormat>.Fail(transformed.ErrorMessage);

        var value = transformed.GetOrThrow();

        return _underlying.Encode(value, ops, prefix);
    }

    public override DataResult<(TTo, TFormat)> Decode<TOps, TFormat>(TOps ops, TFormat input)
    {
        return _underlying.Decode(ops, input).Map(pair => (_to(pair.Item1), pair.Item2));
    }
}

// A -> B not always valid, but B -> A always valid (for example, float array to 3D vector)
internal class Unsafe2SafeMapCodec<TFrom, TTo> : Codec<TTo>
{
    private readonly Codec<TFrom> _underlying;
    private readonly Func<TFrom, DataResult<TTo>> _to;
    private readonly Func<TTo, TFrom> _from;

    public Unsafe2SafeMapCodec(
        Codec<TFrom> underlying,
        Func<TFrom, DataResult<TTo>> to,
        Func<TTo, TFrom> from
    )
    {
        _underlying = underlying;
        _to = to;
        _from = from;
    }

    public override DataResult<TFormat> Encode<TOps, TFormat>(TTo input, TOps ops, TFormat prefix)
    {
        return _underlying.Encode(_from(input), ops, prefix);
    }

    public override DataResult<(TTo, TFormat)> Decode<TOps, TFormat>(TOps ops, TFormat input)
    {
        return _underlying
            .Decode(ops, input)
            .UnsafeMap(pair => _to(pair.Item1).Map(mapped => (mapped, pair.Item2)));
    }
}
