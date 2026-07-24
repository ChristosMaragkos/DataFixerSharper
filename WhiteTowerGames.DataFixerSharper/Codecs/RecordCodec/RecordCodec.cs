using WhiteTowerGames.DataFixerSharper.Abstractions;

namespace WhiteTowerGames.DataFixerSharper.Codecs.RecordCodec;

public readonly struct RecordCodec1<T, TF> : ICodec<T>
{
    private readonly IFieldCodec<T, TF> _f0;
    private readonly Func<TF, T> _factory;

    public RecordCodec1(IFieldCodec<T, TF> f0, Func<TF, T> factory)
    {
        _f0 = f0;
        _factory = factory;
    }

    public DataResult<(T, TFormat)> Decode<TOps, TFormat>(TFormat input)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        var map = new FieldMap<TFormat>();
        var readResult = TOps.ReadMap(input, ref map, new FieldMapConsumer<TOps, TFormat>());
        if (readResult.IsError)
            return DataResult<(T, TFormat)>.Fail(readResult.ErrorMessage);

        var d0 = _f0.DecodeFromMap<TOps, TFormat>(ref map);
        if (d0.IsError)
            return DataResult<(T, TFormat)>.Fail(d0.ErrorMessage);

        return DataResult<(T, TFormat)>.Success((_factory(d0.GetOrThrow()), input));
    }

    public DataResult<TFormat> Encode<TOps, TFormat>(T input, TFormat prefix)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        TOps.WriteMapStart(prefix);
        var e0 = _f0.Encode<TOps, TFormat>(input, prefix);
        if (e0.IsError)
            return e0;
        TOps.WriteMapEnd(prefix);
        return DataResult<TFormat>.Success(prefix);
    }

    public DataResult<T> Parse<TOps, TFormat>(TFormat input)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        var parsed = Decode<TOps, TFormat>(input);
        if (parsed.IsError)
            return DataResult<T>.Fail(parsed.ErrorMessage);
        return DataResult<T>.Success(parsed.GetOrThrow().Item1);
    }
}

public readonly struct RecordCodec2<T, TF, TF1> : ICodec<T>
{
    private readonly IFieldCodec<T, TF> _f0;
    private readonly IFieldCodec<T, TF1> _f1;
    private readonly Func<TF, TF1, T> _factory;

    public RecordCodec2(IFieldCodec<T, TF> f0, IFieldCodec<T, TF1> f1, Func<TF, TF1, T> factory)
    {
        _f0 = f0;
        _f1 = f1;
        _factory = factory;
    }

    public DataResult<(T, TFormat)> Decode<TOps, TFormat>(TFormat input)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        var map = new FieldMap<TFormat>();
        var readResult = TOps.ReadMap(input, ref map, new FieldMapConsumer<TOps, TFormat>());
        if (readResult.IsError)
            return DataResult<(T, TFormat)>.Fail(readResult.ErrorMessage);

        var d0 = _f0.DecodeFromMap<TOps, TFormat>(ref map);
        if (d0.IsError)
            return DataResult<(T, TFormat)>.Fail(d0.ErrorMessage);
        var d1 = _f1.DecodeFromMap<TOps, TFormat>(ref map);
        if (d1.IsError)
            return DataResult<(T, TFormat)>.Fail(d1.ErrorMessage);

        return DataResult<(T, TFormat)>.Success(
            (_factory(d0.GetOrThrow(), d1.GetOrThrow()), input)
        );
    }

    public DataResult<TFormat> Encode<TOps, TFormat>(T input, TFormat prefix)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        TOps.WriteMapStart(prefix);
        var e0 = _f0.Encode<TOps, TFormat>(input, prefix);
        if (e0.IsError)
            return e0;
        var e1 = _f1.Encode<TOps, TFormat>(input, prefix);
        if (e1.IsError)
            return e1;
        TOps.WriteMapEnd(prefix);
        return DataResult<TFormat>.Success(prefix);
    }

    public DataResult<T> Parse<TOps, TFormat>(TFormat input)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        var parsed = Decode<TOps, TFormat>(input);
        if (parsed.IsError)
            return DataResult<T>.Fail(parsed.ErrorMessage);
        return DataResult<T>.Success(parsed.GetOrThrow().Item1);
    }
}

public readonly struct RecordCodec3<T, TF, TF1, TF2> : ICodec<T>
{
    private readonly IFieldCodec<T, TF> _f0;
    private readonly IFieldCodec<T, TF1> _f1;
    private readonly IFieldCodec<T, TF2> _f2;
    private readonly Func<TF, TF1, TF2, T> _factory;

    public RecordCodec3(
        IFieldCodec<T, TF> f0,
        IFieldCodec<T, TF1> f1,
        IFieldCodec<T, TF2> f2,
        Func<TF, TF1, TF2, T> factory
    )
    {
        _f0 = f0;
        _f1 = f1;
        _f2 = f2;
        _factory = factory;
    }

    public DataResult<(T, TFormat)> Decode<TOps, TFormat>(TFormat input)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        var map = new FieldMap<TFormat>();
        var readResult = TOps.ReadMap(input, ref map, new FieldMapConsumer<TOps, TFormat>());
        if (readResult.IsError)
            return DataResult<(T, TFormat)>.Fail(readResult.ErrorMessage);

        var d0 = _f0.DecodeFromMap<TOps, TFormat>(ref map);
        if (d0.IsError)
            return DataResult<(T, TFormat)>.Fail(d0.ErrorMessage);
        var d1 = _f1.DecodeFromMap<TOps, TFormat>(ref map);
        if (d1.IsError)
            return DataResult<(T, TFormat)>.Fail(d1.ErrorMessage);
        var d2 = _f2.DecodeFromMap<TOps, TFormat>(ref map);
        if (d2.IsError)
            return DataResult<(T, TFormat)>.Fail(d2.ErrorMessage);

        return DataResult<(T, TFormat)>.Success(
            (_factory(d0.GetOrThrow(), d1.GetOrThrow(), d2.GetOrThrow()), input)
        );
    }

    public DataResult<TFormat> Encode<TOps, TFormat>(T input, TFormat prefix)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        TOps.WriteMapStart(prefix);
        var e0 = _f0.Encode<TOps, TFormat>(input, prefix);
        if (e0.IsError)
            return e0;
        var e1 = _f1.Encode<TOps, TFormat>(input, prefix);
        if (e1.IsError)
            return e1;
        var e2 = _f2.Encode<TOps, TFormat>(input, prefix);
        if (e2.IsError)
            return e2;
        TOps.WriteMapEnd(prefix);
        return DataResult<TFormat>.Success(prefix);
    }

    public DataResult<T> Parse<TOps, TFormat>(TFormat input)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        var parsed = Decode<TOps, TFormat>(input);
        if (parsed.IsError)
            return DataResult<T>.Fail(parsed.ErrorMessage);
        return DataResult<T>.Success(parsed.GetOrThrow().Item1);
    }
}

public readonly struct RecordCodec4<T, TF, TF1, TF2, TF3> : ICodec<T>
{
    private readonly IFieldCodec<T, TF> _f0;
    private readonly IFieldCodec<T, TF1> _f1;
    private readonly IFieldCodec<T, TF2> _f2;
    private readonly IFieldCodec<T, TF3> _f3;
    private readonly Func<TF, TF1, TF2, TF3, T> _factory;

    public RecordCodec4(
        IFieldCodec<T, TF> f0,
        IFieldCodec<T, TF1> f1,
        IFieldCodec<T, TF2> f2,
        IFieldCodec<T, TF3> f3,
        Func<TF, TF1, TF2, TF3, T> factory
    )
    {
        _f0 = f0;
        _f1 = f1;
        _f2 = f2;
        _f3 = f3;
        _factory = factory;
    }

    public DataResult<(T, TFormat)> Decode<TOps, TFormat>(TFormat input)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        var map = new FieldMap<TFormat>();
        var readResult = TOps.ReadMap(input, ref map, new FieldMapConsumer<TOps, TFormat>());
        if (readResult.IsError)
            return DataResult<(T, TFormat)>.Fail(readResult.ErrorMessage);

        var d0 = _f0.DecodeFromMap<TOps, TFormat>(ref map);
        if (d0.IsError)
            return DataResult<(T, TFormat)>.Fail(d0.ErrorMessage);
        var d1 = _f1.DecodeFromMap<TOps, TFormat>(ref map);
        if (d1.IsError)
            return DataResult<(T, TFormat)>.Fail(d1.ErrorMessage);
        var d2 = _f2.DecodeFromMap<TOps, TFormat>(ref map);
        if (d2.IsError)
            return DataResult<(T, TFormat)>.Fail(d2.ErrorMessage);
        var d3 = _f3.DecodeFromMap<TOps, TFormat>(ref map);
        if (d3.IsError)
            return DataResult<(T, TFormat)>.Fail(d3.ErrorMessage);

        return DataResult<(T, TFormat)>.Success(
            (_factory(d0.GetOrThrow(), d1.GetOrThrow(), d2.GetOrThrow(), d3.GetOrThrow()), input)
        );
    }

    public DataResult<TFormat> Encode<TOps, TFormat>(T input, TFormat prefix)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        TOps.WriteMapStart(prefix);
        var e0 = _f0.Encode<TOps, TFormat>(input, prefix);
        if (e0.IsError)
            return e0;
        var e1 = _f1.Encode<TOps, TFormat>(input, prefix);
        if (e1.IsError)
            return e1;
        var e2 = _f2.Encode<TOps, TFormat>(input, prefix);
        if (e2.IsError)
            return e2;
        var e3 = _f3.Encode<TOps, TFormat>(input, prefix);
        if (e3.IsError)
            return e3;
        TOps.WriteMapEnd(prefix);
        return DataResult<TFormat>.Success(prefix);
    }

    public DataResult<T> Parse<TOps, TFormat>(TFormat input)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        var parsed = Decode<TOps, TFormat>(input);
        if (parsed.IsError)
            return DataResult<T>.Fail(parsed.ErrorMessage);
        return DataResult<T>.Success(parsed.GetOrThrow().Item1);
    }
}

public readonly struct RecordCodec5<T, TF, TF1, TF2, TF3, TF4> : ICodec<T>
{
    private readonly IFieldCodec<T, TF> _f0;
    private readonly IFieldCodec<T, TF1> _f1;
    private readonly IFieldCodec<T, TF2> _f2;
    private readonly IFieldCodec<T, TF3> _f3;
    private readonly IFieldCodec<T, TF4> _f4;
    private readonly Func<TF, TF1, TF2, TF3, TF4, T> _factory;

    public RecordCodec5(
        IFieldCodec<T, TF> f0,
        IFieldCodec<T, TF1> f1,
        IFieldCodec<T, TF2> f2,
        IFieldCodec<T, TF3> f3,
        IFieldCodec<T, TF4> f4,
        Func<TF, TF1, TF2, TF3, TF4, T> factory
    )
    {
        _f0 = f0;
        _f1 = f1;
        _f2 = f2;
        _f3 = f3;
        _f4 = f4;
        _factory = factory;
    }

    public DataResult<(T, TFormat)> Decode<TOps, TFormat>(TFormat input)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        var map = new FieldMap<TFormat>();
        var readResult = TOps.ReadMap(input, ref map, new FieldMapConsumer<TOps, TFormat>());
        if (readResult.IsError)
            return DataResult<(T, TFormat)>.Fail(readResult.ErrorMessage);

        var d0 = _f0.DecodeFromMap<TOps, TFormat>(ref map);
        if (d0.IsError)
            return DataResult<(T, TFormat)>.Fail(d0.ErrorMessage);
        var d1 = _f1.DecodeFromMap<TOps, TFormat>(ref map);
        if (d1.IsError)
            return DataResult<(T, TFormat)>.Fail(d1.ErrorMessage);
        var d2 = _f2.DecodeFromMap<TOps, TFormat>(ref map);
        if (d2.IsError)
            return DataResult<(T, TFormat)>.Fail(d2.ErrorMessage);
        var d3 = _f3.DecodeFromMap<TOps, TFormat>(ref map);
        if (d3.IsError)
            return DataResult<(T, TFormat)>.Fail(d3.ErrorMessage);
        var d4 = _f4.DecodeFromMap<TOps, TFormat>(ref map);
        if (d4.IsError)
            return DataResult<(T, TFormat)>.Fail(d4.ErrorMessage);

        return DataResult<(T, TFormat)>.Success(
            (
                _factory(
                    d0.GetOrThrow(),
                    d1.GetOrThrow(),
                    d2.GetOrThrow(),
                    d3.GetOrThrow(),
                    d4.GetOrThrow()
                ),
                input
            )
        );
    }

    public DataResult<TFormat> Encode<TOps, TFormat>(T input, TFormat prefix)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        TOps.WriteMapStart(prefix);
        var e0 = _f0.Encode<TOps, TFormat>(input, prefix);
        if (e0.IsError)
            return e0;
        var e1 = _f1.Encode<TOps, TFormat>(input, prefix);
        if (e1.IsError)
            return e1;
        var e2 = _f2.Encode<TOps, TFormat>(input, prefix);
        if (e2.IsError)
            return e2;
        var e3 = _f3.Encode<TOps, TFormat>(input, prefix);
        if (e3.IsError)
            return e3;
        var e4 = _f4.Encode<TOps, TFormat>(input, prefix);
        if (e4.IsError)
            return e4;
        TOps.WriteMapEnd(prefix);
        return DataResult<TFormat>.Success(prefix);
    }

    public DataResult<T> Parse<TOps, TFormat>(TFormat input)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        var parsed = Decode<TOps, TFormat>(input);
        if (parsed.IsError)
            return DataResult<T>.Fail(parsed.ErrorMessage);
        return DataResult<T>.Success(parsed.GetOrThrow().Item1);
    }
}

public readonly struct RecordCodec6<T, TF, TF1, TF2, TF3, TF4, TF5> : ICodec<T>
{
    private readonly IFieldCodec<T, TF> _f0;
    private readonly IFieldCodec<T, TF1> _f1;
    private readonly IFieldCodec<T, TF2> _f2;
    private readonly IFieldCodec<T, TF3> _f3;
    private readonly IFieldCodec<T, TF4> _f4;
    private readonly IFieldCodec<T, TF5> _f5;
    private readonly Func<TF, TF1, TF2, TF3, TF4, TF5, T> _factory;

    public RecordCodec6(
        IFieldCodec<T, TF> f0,
        IFieldCodec<T, TF1> f1,
        IFieldCodec<T, TF2> f2,
        IFieldCodec<T, TF3> f3,
        IFieldCodec<T, TF4> f4,
        IFieldCodec<T, TF5> f5,
        Func<TF, TF1, TF2, TF3, TF4, TF5, T> factory
    )
    {
        _f0 = f0;
        _f1 = f1;
        _f2 = f2;
        _f3 = f3;
        _f4 = f4;
        _f5 = f5;
        _factory = factory;
    }

    public DataResult<(T, TFormat)> Decode<TOps, TFormat>(TFormat input)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        var map = new FieldMap<TFormat>();
        var readResult = TOps.ReadMap(input, ref map, new FieldMapConsumer<TOps, TFormat>());
        if (readResult.IsError)
            return DataResult<(T, TFormat)>.Fail(readResult.ErrorMessage);

        var d0 = _f0.DecodeFromMap<TOps, TFormat>(ref map);
        if (d0.IsError)
            return DataResult<(T, TFormat)>.Fail(d0.ErrorMessage);
        var d1 = _f1.DecodeFromMap<TOps, TFormat>(ref map);
        if (d1.IsError)
            return DataResult<(T, TFormat)>.Fail(d1.ErrorMessage);
        var d2 = _f2.DecodeFromMap<TOps, TFormat>(ref map);
        if (d2.IsError)
            return DataResult<(T, TFormat)>.Fail(d2.ErrorMessage);
        var d3 = _f3.DecodeFromMap<TOps, TFormat>(ref map);
        if (d3.IsError)
            return DataResult<(T, TFormat)>.Fail(d3.ErrorMessage);
        var d4 = _f4.DecodeFromMap<TOps, TFormat>(ref map);
        if (d4.IsError)
            return DataResult<(T, TFormat)>.Fail(d4.ErrorMessage);
        var d5 = _f5.DecodeFromMap<TOps, TFormat>(ref map);
        if (d5.IsError)
            return DataResult<(T, TFormat)>.Fail(d5.ErrorMessage);

        return DataResult<(T, TFormat)>.Success(
            (
                _factory(
                    d0.GetOrThrow(),
                    d1.GetOrThrow(),
                    d2.GetOrThrow(),
                    d3.GetOrThrow(),
                    d4.GetOrThrow(),
                    d5.GetOrThrow()
                ),
                input
            )
        );
    }

    public DataResult<TFormat> Encode<TOps, TFormat>(T input, TFormat prefix)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        TOps.WriteMapStart(prefix);
        var e0 = _f0.Encode<TOps, TFormat>(input, prefix);
        if (e0.IsError)
            return e0;
        var e1 = _f1.Encode<TOps, TFormat>(input, prefix);
        if (e1.IsError)
            return e1;
        var e2 = _f2.Encode<TOps, TFormat>(input, prefix);
        if (e2.IsError)
            return e2;
        var e3 = _f3.Encode<TOps, TFormat>(input, prefix);
        if (e3.IsError)
            return e3;
        var e4 = _f4.Encode<TOps, TFormat>(input, prefix);
        if (e4.IsError)
            return e4;
        var e5 = _f5.Encode<TOps, TFormat>(input, prefix);
        if (e5.IsError)
            return e5;
        TOps.WriteMapEnd(prefix);
        return DataResult<TFormat>.Success(prefix);
    }

    public DataResult<T> Parse<TOps, TFormat>(TFormat input)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        var parsed = Decode<TOps, TFormat>(input);
        if (parsed.IsError)
            return DataResult<T>.Fail(parsed.ErrorMessage);
        return DataResult<T>.Success(parsed.GetOrThrow().Item1);
    }
}

public readonly struct RecordCodec7<T, TF, TF1, TF2, TF3, TF4, TF5, TF6> : ICodec<T>
{
    private readonly IFieldCodec<T, TF> _f0;
    private readonly IFieldCodec<T, TF1> _f1;
    private readonly IFieldCodec<T, TF2> _f2;
    private readonly IFieldCodec<T, TF3> _f3;
    private readonly IFieldCodec<T, TF4> _f4;
    private readonly IFieldCodec<T, TF5> _f5;
    private readonly IFieldCodec<T, TF6> _f6;
    private readonly Func<TF, TF1, TF2, TF3, TF4, TF5, TF6, T> _factory;

    public RecordCodec7(
        IFieldCodec<T, TF> f0,
        IFieldCodec<T, TF1> f1,
        IFieldCodec<T, TF2> f2,
        IFieldCodec<T, TF3> f3,
        IFieldCodec<T, TF4> f4,
        IFieldCodec<T, TF5> f5,
        IFieldCodec<T, TF6> f6,
        Func<TF, TF1, TF2, TF3, TF4, TF5, TF6, T> factory
    )
    {
        _f0 = f0;
        _f1 = f1;
        _f2 = f2;
        _f3 = f3;
        _f4 = f4;
        _f5 = f5;
        _f6 = f6;
        _factory = factory;
    }

    public DataResult<(T, TFormat)> Decode<TOps, TFormat>(TFormat input)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        var map = new FieldMap<TFormat>();
        var readResult = TOps.ReadMap(input, ref map, new FieldMapConsumer<TOps, TFormat>());
        if (readResult.IsError)
            return DataResult<(T, TFormat)>.Fail(readResult.ErrorMessage);

        var d0 = _f0.DecodeFromMap<TOps, TFormat>(ref map);
        if (d0.IsError)
            return DataResult<(T, TFormat)>.Fail(d0.ErrorMessage);
        var d1 = _f1.DecodeFromMap<TOps, TFormat>(ref map);
        if (d1.IsError)
            return DataResult<(T, TFormat)>.Fail(d1.ErrorMessage);
        var d2 = _f2.DecodeFromMap<TOps, TFormat>(ref map);
        if (d2.IsError)
            return DataResult<(T, TFormat)>.Fail(d2.ErrorMessage);
        var d3 = _f3.DecodeFromMap<TOps, TFormat>(ref map);
        if (d3.IsError)
            return DataResult<(T, TFormat)>.Fail(d3.ErrorMessage);
        var d4 = _f4.DecodeFromMap<TOps, TFormat>(ref map);
        if (d4.IsError)
            return DataResult<(T, TFormat)>.Fail(d4.ErrorMessage);
        var d5 = _f5.DecodeFromMap<TOps, TFormat>(ref map);
        if (d5.IsError)
            return DataResult<(T, TFormat)>.Fail(d5.ErrorMessage);
        var d6 = _f6.DecodeFromMap<TOps, TFormat>(ref map);
        if (d6.IsError)
            return DataResult<(T, TFormat)>.Fail(d6.ErrorMessage);

        return DataResult<(T, TFormat)>.Success(
            (
                _factory(
                    d0.GetOrThrow(),
                    d1.GetOrThrow(),
                    d2.GetOrThrow(),
                    d3.GetOrThrow(),
                    d4.GetOrThrow(),
                    d5.GetOrThrow(),
                    d6.GetOrThrow()
                ),
                input
            )
        );
    }

    public DataResult<TFormat> Encode<TOps, TFormat>(T input, TFormat prefix)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        TOps.WriteMapStart(prefix);
        var e0 = _f0.Encode<TOps, TFormat>(input, prefix);
        if (e0.IsError)
            return e0;
        var e1 = _f1.Encode<TOps, TFormat>(input, prefix);
        if (e1.IsError)
            return e1;
        var e2 = _f2.Encode<TOps, TFormat>(input, prefix);
        if (e2.IsError)
            return e2;
        var e3 = _f3.Encode<TOps, TFormat>(input, prefix);
        if (e3.IsError)
            return e3;
        var e4 = _f4.Encode<TOps, TFormat>(input, prefix);
        if (e4.IsError)
            return e4;
        var e5 = _f5.Encode<TOps, TFormat>(input, prefix);
        if (e5.IsError)
            return e5;
        var e6 = _f6.Encode<TOps, TFormat>(input, prefix);
        if (e6.IsError)
            return e6;
        TOps.WriteMapEnd(prefix);
        return DataResult<TFormat>.Success(prefix);
    }

    public DataResult<T> Parse<TOps, TFormat>(TFormat input)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        var parsed = Decode<TOps, TFormat>(input);
        if (parsed.IsError)
            return DataResult<T>.Fail(parsed.ErrorMessage);
        return DataResult<T>.Success(parsed.GetOrThrow().Item1);
    }
}

public readonly struct RecordCodec8<T, TF, TF1, TF2, TF3, TF4, TF5, TF6, TF7> : ICodec<T>
{
    private readonly IFieldCodec<T, TF> _f0;
    private readonly IFieldCodec<T, TF1> _f1;
    private readonly IFieldCodec<T, TF2> _f2;
    private readonly IFieldCodec<T, TF3> _f3;
    private readonly IFieldCodec<T, TF4> _f4;
    private readonly IFieldCodec<T, TF5> _f5;
    private readonly IFieldCodec<T, TF6> _f6;
    private readonly IFieldCodec<T, TF7> _f7;
    private readonly Func<TF, TF1, TF2, TF3, TF4, TF5, TF6, TF7, T> _factory;

    public RecordCodec8(
        IFieldCodec<T, TF> f0,
        IFieldCodec<T, TF1> f1,
        IFieldCodec<T, TF2> f2,
        IFieldCodec<T, TF3> f3,
        IFieldCodec<T, TF4> f4,
        IFieldCodec<T, TF5> f5,
        IFieldCodec<T, TF6> f6,
        IFieldCodec<T, TF7> f7,
        Func<TF, TF1, TF2, TF3, TF4, TF5, TF6, TF7, T> factory
    )
    {
        _f0 = f0;
        _f1 = f1;
        _f2 = f2;
        _f3 = f3;
        _f4 = f4;
        _f5 = f5;
        _f6 = f6;
        _f7 = f7;
        _factory = factory;
    }

    public DataResult<(T, TFormat)> Decode<TOps, TFormat>(TFormat input)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        var map = new FieldMap<TFormat>();
        var readResult = TOps.ReadMap(input, ref map, new FieldMapConsumer<TOps, TFormat>());
        if (readResult.IsError)
            return DataResult<(T, TFormat)>.Fail(readResult.ErrorMessage);

        var d0 = _f0.DecodeFromMap<TOps, TFormat>(ref map);
        if (d0.IsError)
            return DataResult<(T, TFormat)>.Fail(d0.ErrorMessage);
        var d1 = _f1.DecodeFromMap<TOps, TFormat>(ref map);
        if (d1.IsError)
            return DataResult<(T, TFormat)>.Fail(d1.ErrorMessage);
        var d2 = _f2.DecodeFromMap<TOps, TFormat>(ref map);
        if (d2.IsError)
            return DataResult<(T, TFormat)>.Fail(d2.ErrorMessage);
        var d3 = _f3.DecodeFromMap<TOps, TFormat>(ref map);
        if (d3.IsError)
            return DataResult<(T, TFormat)>.Fail(d3.ErrorMessage);
        var d4 = _f4.DecodeFromMap<TOps, TFormat>(ref map);
        if (d4.IsError)
            return DataResult<(T, TFormat)>.Fail(d4.ErrorMessage);
        var d5 = _f5.DecodeFromMap<TOps, TFormat>(ref map);
        if (d5.IsError)
            return DataResult<(T, TFormat)>.Fail(d5.ErrorMessage);
        var d6 = _f6.DecodeFromMap<TOps, TFormat>(ref map);
        if (d6.IsError)
            return DataResult<(T, TFormat)>.Fail(d6.ErrorMessage);
        var d7 = _f7.DecodeFromMap<TOps, TFormat>(ref map);
        if (d7.IsError)
            return DataResult<(T, TFormat)>.Fail(d7.ErrorMessage);

        return DataResult<(T, TFormat)>.Success(
            (
                _factory(
                    d0.GetOrThrow(),
                    d1.GetOrThrow(),
                    d2.GetOrThrow(),
                    d3.GetOrThrow(),
                    d4.GetOrThrow(),
                    d5.GetOrThrow(),
                    d6.GetOrThrow(),
                    d7.GetOrThrow()
                ),
                input
            )
        );
    }

    public DataResult<TFormat> Encode<TOps, TFormat>(T input, TFormat prefix)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        TOps.WriteMapStart(prefix);
        var e0 = _f0.Encode<TOps, TFormat>(input, prefix);
        if (e0.IsError)
            return e0;
        var e1 = _f1.Encode<TOps, TFormat>(input, prefix);
        if (e1.IsError)
            return e1;
        var e2 = _f2.Encode<TOps, TFormat>(input, prefix);
        if (e2.IsError)
            return e2;
        var e3 = _f3.Encode<TOps, TFormat>(input, prefix);
        if (e3.IsError)
            return e3;
        var e4 = _f4.Encode<TOps, TFormat>(input, prefix);
        if (e4.IsError)
            return e4;
        var e5 = _f5.Encode<TOps, TFormat>(input, prefix);
        if (e5.IsError)
            return e5;
        var e6 = _f6.Encode<TOps, TFormat>(input, prefix);
        if (e6.IsError)
            return e6;
        var e7 = _f7.Encode<TOps, TFormat>(input, prefix);
        if (e7.IsError)
            return e7;
        TOps.WriteMapEnd(prefix);
        return DataResult<TFormat>.Success(prefix);
    }

    public DataResult<T> Parse<TOps, TFormat>(TFormat input)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        var parsed = Decode<TOps, TFormat>(input);
        if (parsed.IsError)
            return DataResult<T>.Fail(parsed.ErrorMessage);
        return DataResult<T>.Success(parsed.GetOrThrow().Item1);
    }
}

public readonly struct RecordCodec9<T, TF, TF1, TF2, TF3, TF4, TF5, TF6, TF7, TF8> : ICodec<T>
{
    private readonly IFieldCodec<T, TF> _f0;
    private readonly IFieldCodec<T, TF1> _f1;
    private readonly IFieldCodec<T, TF2> _f2;
    private readonly IFieldCodec<T, TF3> _f3;
    private readonly IFieldCodec<T, TF4> _f4;
    private readonly IFieldCodec<T, TF5> _f5;
    private readonly IFieldCodec<T, TF6> _f6;
    private readonly IFieldCodec<T, TF7> _f7;
    private readonly IFieldCodec<T, TF8> _f8;
    private readonly Func<TF, TF1, TF2, TF3, TF4, TF5, TF6, TF7, TF8, T> _factory;

    public RecordCodec9(
        IFieldCodec<T, TF> f0,
        IFieldCodec<T, TF1> f1,
        IFieldCodec<T, TF2> f2,
        IFieldCodec<T, TF3> f3,
        IFieldCodec<T, TF4> f4,
        IFieldCodec<T, TF5> f5,
        IFieldCodec<T, TF6> f6,
        IFieldCodec<T, TF7> f7,
        IFieldCodec<T, TF8> f8,
        Func<TF, TF1, TF2, TF3, TF4, TF5, TF6, TF7, TF8, T> factory
    )
    {
        _f0 = f0;
        _f1 = f1;
        _f2 = f2;
        _f3 = f3;
        _f4 = f4;
        _f5 = f5;
        _f6 = f6;
        _f7 = f7;
        _f8 = f8;
        _factory = factory;
    }

    public DataResult<(T, TFormat)> Decode<TOps, TFormat>(TFormat input)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        var map = new FieldMap<TFormat>();
        var readResult = TOps.ReadMap(input, ref map, new FieldMapConsumer<TOps, TFormat>());
        if (readResult.IsError)
            return DataResult<(T, TFormat)>.Fail(readResult.ErrorMessage);

        var d0 = _f0.DecodeFromMap<TOps, TFormat>(ref map);
        if (d0.IsError)
            return DataResult<(T, TFormat)>.Fail(d0.ErrorMessage);
        var d1 = _f1.DecodeFromMap<TOps, TFormat>(ref map);
        if (d1.IsError)
            return DataResult<(T, TFormat)>.Fail(d1.ErrorMessage);
        var d2 = _f2.DecodeFromMap<TOps, TFormat>(ref map);
        if (d2.IsError)
            return DataResult<(T, TFormat)>.Fail(d2.ErrorMessage);
        var d3 = _f3.DecodeFromMap<TOps, TFormat>(ref map);
        if (d3.IsError)
            return DataResult<(T, TFormat)>.Fail(d3.ErrorMessage);
        var d4 = _f4.DecodeFromMap<TOps, TFormat>(ref map);
        if (d4.IsError)
            return DataResult<(T, TFormat)>.Fail(d4.ErrorMessage);
        var d5 = _f5.DecodeFromMap<TOps, TFormat>(ref map);
        if (d5.IsError)
            return DataResult<(T, TFormat)>.Fail(d5.ErrorMessage);
        var d6 = _f6.DecodeFromMap<TOps, TFormat>(ref map);
        if (d6.IsError)
            return DataResult<(T, TFormat)>.Fail(d6.ErrorMessage);
        var d7 = _f7.DecodeFromMap<TOps, TFormat>(ref map);
        if (d7.IsError)
            return DataResult<(T, TFormat)>.Fail(d7.ErrorMessage);
        var d8 = _f8.DecodeFromMap<TOps, TFormat>(ref map);
        if (d8.IsError)
            return DataResult<(T, TFormat)>.Fail(d8.ErrorMessage);

        return DataResult<(T, TFormat)>.Success(
            (
                _factory(
                    d0.GetOrThrow(),
                    d1.GetOrThrow(),
                    d2.GetOrThrow(),
                    d3.GetOrThrow(),
                    d4.GetOrThrow(),
                    d5.GetOrThrow(),
                    d6.GetOrThrow(),
                    d7.GetOrThrow(),
                    d8.GetOrThrow()
                ),
                input
            )
        );
    }

    public DataResult<TFormat> Encode<TOps, TFormat>(T input, TFormat prefix)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        TOps.WriteMapStart(prefix);
        var e0 = _f0.Encode<TOps, TFormat>(input, prefix);
        if (e0.IsError)
            return e0;
        var e1 = _f1.Encode<TOps, TFormat>(input, prefix);
        if (e1.IsError)
            return e1;
        var e2 = _f2.Encode<TOps, TFormat>(input, prefix);
        if (e2.IsError)
            return e2;
        var e3 = _f3.Encode<TOps, TFormat>(input, prefix);
        if (e3.IsError)
            return e3;
        var e4 = _f4.Encode<TOps, TFormat>(input, prefix);
        if (e4.IsError)
            return e4;
        var e5 = _f5.Encode<TOps, TFormat>(input, prefix);
        if (e5.IsError)
            return e5;
        var e6 = _f6.Encode<TOps, TFormat>(input, prefix);
        if (e6.IsError)
            return e6;
        var e7 = _f7.Encode<TOps, TFormat>(input, prefix);
        if (e7.IsError)
            return e7;
        var e8 = _f8.Encode<TOps, TFormat>(input, prefix);
        if (e8.IsError)
            return e8;
        TOps.WriteMapEnd(prefix);
        return DataResult<TFormat>.Success(prefix);
    }

    public DataResult<T> Parse<TOps, TFormat>(TFormat input)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        var parsed = Decode<TOps, TFormat>(input);
        if (parsed.IsError)
            return DataResult<T>.Fail(parsed.ErrorMessage);
        return DataResult<T>.Success(parsed.GetOrThrow().Item1);
    }
}

public readonly struct RecordCodec10<T, TF, TF1, TF2, TF3, TF4, TF5, TF6, TF7, TF8, TF9> : ICodec<T>
{
    private readonly IFieldCodec<T, TF> _f0;
    private readonly IFieldCodec<T, TF1> _f1;
    private readonly IFieldCodec<T, TF2> _f2;
    private readonly IFieldCodec<T, TF3> _f3;
    private readonly IFieldCodec<T, TF4> _f4;
    private readonly IFieldCodec<T, TF5> _f5;
    private readonly IFieldCodec<T, TF6> _f6;
    private readonly IFieldCodec<T, TF7> _f7;
    private readonly IFieldCodec<T, TF8> _f8;
    private readonly IFieldCodec<T, TF9> _f9;
    private readonly Func<TF, TF1, TF2, TF3, TF4, TF5, TF6, TF7, TF8, TF9, T> _factory;

    public RecordCodec10(
        IFieldCodec<T, TF> f0,
        IFieldCodec<T, TF1> f1,
        IFieldCodec<T, TF2> f2,
        IFieldCodec<T, TF3> f3,
        IFieldCodec<T, TF4> f4,
        IFieldCodec<T, TF5> f5,
        IFieldCodec<T, TF6> f6,
        IFieldCodec<T, TF7> f7,
        IFieldCodec<T, TF8> f8,
        IFieldCodec<T, TF9> f9,
        Func<TF, TF1, TF2, TF3, TF4, TF5, TF6, TF7, TF8, TF9, T> factory
    )
    {
        _f0 = f0;
        _f1 = f1;
        _f2 = f2;
        _f3 = f3;
        _f4 = f4;
        _f5 = f5;
        _f6 = f6;
        _f7 = f7;
        _f8 = f8;
        _f9 = f9;
        _factory = factory;
    }

    public DataResult<(T, TFormat)> Decode<TOps, TFormat>(TFormat input)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        var map = new FieldMap<TFormat>();
        var readResult = TOps.ReadMap(input, ref map, new FieldMapConsumer<TOps, TFormat>());
        if (readResult.IsError)
            return DataResult<(T, TFormat)>.Fail(readResult.ErrorMessage);

        var d0 = _f0.DecodeFromMap<TOps, TFormat>(ref map);
        if (d0.IsError)
            return DataResult<(T, TFormat)>.Fail(d0.ErrorMessage);
        var d1 = _f1.DecodeFromMap<TOps, TFormat>(ref map);
        if (d1.IsError)
            return DataResult<(T, TFormat)>.Fail(d1.ErrorMessage);
        var d2 = _f2.DecodeFromMap<TOps, TFormat>(ref map);
        if (d2.IsError)
            return DataResult<(T, TFormat)>.Fail(d2.ErrorMessage);
        var d3 = _f3.DecodeFromMap<TOps, TFormat>(ref map);
        if (d3.IsError)
            return DataResult<(T, TFormat)>.Fail(d3.ErrorMessage);
        var d4 = _f4.DecodeFromMap<TOps, TFormat>(ref map);
        if (d4.IsError)
            return DataResult<(T, TFormat)>.Fail(d4.ErrorMessage);
        var d5 = _f5.DecodeFromMap<TOps, TFormat>(ref map);
        if (d5.IsError)
            return DataResult<(T, TFormat)>.Fail(d5.ErrorMessage);
        var d6 = _f6.DecodeFromMap<TOps, TFormat>(ref map);
        if (d6.IsError)
            return DataResult<(T, TFormat)>.Fail(d6.ErrorMessage);
        var d7 = _f7.DecodeFromMap<TOps, TFormat>(ref map);
        if (d7.IsError)
            return DataResult<(T, TFormat)>.Fail(d7.ErrorMessage);
        var d8 = _f8.DecodeFromMap<TOps, TFormat>(ref map);
        if (d8.IsError)
            return DataResult<(T, TFormat)>.Fail(d8.ErrorMessage);
        var d9 = _f9.DecodeFromMap<TOps, TFormat>(ref map);
        if (d9.IsError)
            return DataResult<(T, TFormat)>.Fail(d9.ErrorMessage);

        return DataResult<(T, TFormat)>.Success(
            (
                _factory(
                    d0.GetOrThrow(),
                    d1.GetOrThrow(),
                    d2.GetOrThrow(),
                    d3.GetOrThrow(),
                    d4.GetOrThrow(),
                    d5.GetOrThrow(),
                    d6.GetOrThrow(),
                    d7.GetOrThrow(),
                    d8.GetOrThrow(),
                    d9.GetOrThrow()
                ),
                input
            )
        );
    }

    public DataResult<TFormat> Encode<TOps, TFormat>(T input, TFormat prefix)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        TOps.WriteMapStart(prefix);
        var e0 = _f0.Encode<TOps, TFormat>(input, prefix);
        if (e0.IsError)
            return e0;
        var e1 = _f1.Encode<TOps, TFormat>(input, prefix);
        if (e1.IsError)
            return e1;
        var e2 = _f2.Encode<TOps, TFormat>(input, prefix);
        if (e2.IsError)
            return e2;
        var e3 = _f3.Encode<TOps, TFormat>(input, prefix);
        if (e3.IsError)
            return e3;
        var e4 = _f4.Encode<TOps, TFormat>(input, prefix);
        if (e4.IsError)
            return e4;
        var e5 = _f5.Encode<TOps, TFormat>(input, prefix);
        if (e5.IsError)
            return e5;
        var e6 = _f6.Encode<TOps, TFormat>(input, prefix);
        if (e6.IsError)
            return e6;
        var e7 = _f7.Encode<TOps, TFormat>(input, prefix);
        if (e7.IsError)
            return e7;
        var e8 = _f8.Encode<TOps, TFormat>(input, prefix);
        if (e8.IsError)
            return e8;
        var e9 = _f9.Encode<TOps, TFormat>(input, prefix);
        if (e9.IsError)
            return e9;
        TOps.WriteMapEnd(prefix);
        return DataResult<TFormat>.Success(prefix);
    }

    public DataResult<T> Parse<TOps, TFormat>(TFormat input)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        var parsed = Decode<TOps, TFormat>(input);
        if (parsed.IsError)
            return DataResult<T>.Fail(parsed.ErrorMessage);
        return DataResult<T>.Success(parsed.GetOrThrow().Item1);
    }
}