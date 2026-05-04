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

    public DataResult<(T, TFormat)> Decode<TOps, TFormat>(TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat>
    {
        var dec0 = _f0.Decode(ops, input);
        if (dec0.IsError)
            return DataResult<(T, TFormat)>.Fail(dec0.ErrorMessage);

        var field0 = dec0.GetOrThrow().Item1;
        var instance = _factory(field0);
        var remainder = dec0.GetOrThrow().Item2;

        return DataResult<(T, TFormat)>.Success((instance, remainder));
    }

    public DataResult<TFormat> Encode<TOps, TFormat>(T input, TOps ops, TFormat prefix)
        where TOps : IDynamicOps<TFormat>
    {
        var map = ops.CreateEmptyMap();
        var enc0 = _f0.Encode(input, ops, map);
        if (enc0.IsError)
            return enc0;

        var finalPrefix = ops.AppendToPrefix(prefix, enc0.GetOrThrow());
        return DataResult<TFormat>.Success(finalPrefix);
    }

    public DataResult<TFormat> EncodeStart<TOps, TFormat>(TOps ops, T input)
        where TOps : IDynamicOps<TFormat> => Encode(input, ops, ops.Empty());

    public DataResult<T> Parse<TOps, TFormat>(TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat>
    {
        var parsed = Decode(ops, input);
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

    public DataResult<(T, TFormat)> Decode<TOps, TFormat>(TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat>
    {
        var dec0 = _f0.Decode(ops, input);
        if (dec0.IsError)
            return DataResult<(T, TFormat)>.Fail(dec0.ErrorMessage);

        var field0 = dec0.GetOrThrow().Item1;

        var dec1 = _f1.Decode(ops, dec0.GetOrThrow().Item2);
        if (dec1.IsError)
            return DataResult<(T, TFormat)>.Fail(dec1.ErrorMessage);

        var field1 = dec1.GetOrThrow().Item1;

        var instance = _factory(field0, field1);
        var remainder = dec1.GetOrThrow().Item2;

        return DataResult<(T, TFormat)>.Success((instance, remainder));
    }

    public DataResult<TFormat> Encode<TOps, TFormat>(T input, TOps ops, TFormat prefix)
        where TOps : IDynamicOps<TFormat>
    {
        var map = ops.CreateEmptyMap();
        var enc0 = _f0.Encode(input, ops, map);
        if (enc0.IsError)
            return enc0;

        // use (prefix + encoded fields) to accumulate
        var enc1 = _f1.Encode(input, ops, enc0.GetOrThrow());
        if (enc1.IsError)
            return enc1;

        var finalPrefix = ops.AppendToPrefix(prefix, enc1.GetOrThrow());
        return DataResult<TFormat>.Success(finalPrefix);
    }

    public DataResult<TFormat> EncodeStart<TOps, TFormat>(TOps ops, T input)
        where TOps : IDynamicOps<TFormat> => Encode(input, ops, ops.Empty());

    public DataResult<T> Parse<TOps, TFormat>(TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat>
    {
        var parsed = Decode(ops, input);
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

    public DataResult<(T, TFormat)> Decode<TOps, TFormat>(TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat>
    {
        var dec0 = _f0.Decode(ops, input);
        if (dec0.IsError)
            return DataResult<(T, TFormat)>.Fail(dec0.ErrorMessage);

        var field0 = dec0.GetOrThrow().Item1;

        var dec1 = _f1.Decode(ops, dec0.GetOrThrow().Item2);
        if (dec1.IsError)
            return DataResult<(T, TFormat)>.Fail(dec1.ErrorMessage);

        var field1 = dec1.GetOrThrow().Item1;

        var dec2 = _f2.Decode(ops, dec1.GetOrThrow().Item2);
        if (dec2.IsError)
            return DataResult<(T, TFormat)>.Fail(dec2.ErrorMessage);

        var field2 = dec2.GetOrThrow().Item1;

        var instance = _factory(field0, field1, field2);
        var remainder = dec2.GetOrThrow().Item2;

        return DataResult<(T, TFormat)>.Success((instance, remainder));
    }

    public DataResult<TFormat> Encode<TOps, TFormat>(T input, TOps ops, TFormat prefix)
        where TOps : IDynamicOps<TFormat>
    {
        var map = ops.CreateEmptyMap();
        var enc0 = _f0.Encode(input, ops, map);
        if (enc0.IsError)
            return enc0;

        // use (prefix + encoded fields) to accumulate
        var enc1 = _f1.Encode(input, ops, enc0.GetOrThrow());
        if (enc1.IsError)
            return enc1;

        var enc2 = _f2.Encode(input, ops, enc1.GetOrThrow());
        if (enc2.IsError)
            return enc2;

        var finalPrefix = ops.AppendToPrefix(prefix, enc2.GetOrThrow());
        return DataResult<TFormat>.Success(finalPrefix);
    }

    public DataResult<TFormat> EncodeStart<TOps, TFormat>(TOps ops, T input)
        where TOps : IDynamicOps<TFormat> => Encode(input, ops, ops.Empty());

    public DataResult<T> Parse<TOps, TFormat>(TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat>
    {
        var parsed = Decode(ops, input);
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

    public DataResult<(T, TFormat)> Decode<TOps, TFormat>(TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat>
    {
        var dec0 = _f0.Decode(ops, input);
        if (dec0.IsError)
            return DataResult<(T, TFormat)>.Fail(dec0.ErrorMessage);

        var dec1 = _f1.Decode(ops, dec0.GetOrThrow().Item2);
        if (dec1.IsError)
            return DataResult<(T, TFormat)>.Fail(dec1.ErrorMessage);

        var dec2 = _f2.Decode(ops, dec1.GetOrThrow().Item2);
        if (dec2.IsError)
            return DataResult<(T, TFormat)>.Fail(dec2.ErrorMessage);

        var dec3 = _f3.Decode(ops, dec2.GetOrThrow().Item2);
        if (dec3.IsError)
            return DataResult<(T, TFormat)>.Fail(dec3.ErrorMessage);

        var instance = _factory(
            dec0.GetOrThrow().Item1,
            dec1.GetOrThrow().Item1,
            dec2.GetOrThrow().Item1,
            dec3.GetOrThrow().Item1
        );

        return DataResult<(T, TFormat)>.Success((instance, dec3.GetOrThrow().Item2));
    }

    public DataResult<TFormat> Encode<TOps, TFormat>(T input, TOps ops, TFormat prefix)
        where TOps : IDynamicOps<TFormat>
    {
        var map = ops.CreateEmptyMap();
        var enc0 = _f0.Encode(input, ops, map);
        if (enc0.IsError)
            return enc0;

        var enc1 = _f1.Encode(input, ops, enc0.GetOrThrow());
        if (enc1.IsError)
            return enc1;

        var enc2 = _f2.Encode(input, ops, enc1.GetOrThrow());
        if (enc2.IsError)
            return enc2;

        var enc3 = _f3.Encode(input, ops, enc2.GetOrThrow());
        if (enc3.IsError)
            return enc3;

        return DataResult<TFormat>.Success(ops.AppendToPrefix(prefix, enc3.GetOrThrow()));
    }

    public DataResult<TFormat> EncodeStart<TOps, TFormat>(TOps ops, T input)
        where TOps : IDynamicOps<TFormat> => Encode(input, ops, ops.Empty());

    public DataResult<T> Parse<TOps, TFormat>(TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat>
    {
        var parsed = Decode(ops, input);
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

    public DataResult<(T, TFormat)> Decode<TOps, TFormat>(TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat>
    {
        var d0 = _f0.Decode(ops, input);
        if (d0.IsError)
            return DataResult<(T, TFormat)>.Fail(d0.ErrorMessage);
        var d1 = _f1.Decode(ops, d0.GetOrThrow().Item2);
        if (d1.IsError)
            return DataResult<(T, TFormat)>.Fail(d1.ErrorMessage);
        var d2 = _f2.Decode(ops, d1.GetOrThrow().Item2);
        if (d2.IsError)
            return DataResult<(T, TFormat)>.Fail(d2.ErrorMessage);
        var d3 = _f3.Decode(ops, d2.GetOrThrow().Item2);
        if (d3.IsError)
            return DataResult<(T, TFormat)>.Fail(d3.ErrorMessage);
        var d4 = _f4.Decode(ops, d3.GetOrThrow().Item2);
        if (d4.IsError)
            return DataResult<(T, TFormat)>.Fail(d4.ErrorMessage);

        var instance = _factory(
            d0.GetOrThrow().Item1,
            d1.GetOrThrow().Item1,
            d2.GetOrThrow().Item1,
            d3.GetOrThrow().Item1,
            d4.GetOrThrow().Item1
        );
        return DataResult<(T, TFormat)>.Success((instance, d4.GetOrThrow().Item2));
    }

    public DataResult<TFormat> Encode<TOps, TFormat>(T input, TOps ops, TFormat prefix)
        where TOps : IDynamicOps<TFormat>
    {
        var e0 = _f0.Encode(input, ops, ops.CreateEmptyMap());
        if (e0.IsError)
            return e0;
        var e1 = _f1.Encode(input, ops, e0.GetOrThrow());
        if (e1.IsError)
            return e1;
        var e2 = _f2.Encode(input, ops, e1.GetOrThrow());
        if (e2.IsError)
            return e2;
        var e3 = _f3.Encode(input, ops, e2.GetOrThrow());
        if (e3.IsError)
            return e3;
        var e4 = _f4.Encode(input, ops, e3.GetOrThrow());
        if (e4.IsError)
            return e4;
        return DataResult<TFormat>.Success(ops.AppendToPrefix(prefix, e4.GetOrThrow()));
    }

    public DataResult<TFormat> EncodeStart<TOps, TFormat>(TOps ops, T input)
        where TOps : IDynamicOps<TFormat> => Encode(input, ops, ops.Empty());

    public DataResult<T> Parse<TOps, TFormat>(TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat>
    {
        var parsed = Decode(ops, input);
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

    public DataResult<(T, TFormat)> Decode<TOps, TFormat>(TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat>
    {
        var d0 = _f0.Decode(ops, input);
        if (d0.IsError)
            return DataResult<(T, TFormat)>.Fail(d0.ErrorMessage);
        var d1 = _f1.Decode(ops, d0.GetOrThrow().Item2);
        if (d1.IsError)
            return DataResult<(T, TFormat)>.Fail(d1.ErrorMessage);
        var d2 = _f2.Decode(ops, d1.GetOrThrow().Item2);
        if (d2.IsError)
            return DataResult<(T, TFormat)>.Fail(d2.ErrorMessage);
        var d3 = _f3.Decode(ops, d2.GetOrThrow().Item2);
        if (d3.IsError)
            return DataResult<(T, TFormat)>.Fail(d3.ErrorMessage);
        var d4 = _f4.Decode(ops, d3.GetOrThrow().Item2);
        if (d4.IsError)
            return DataResult<(T, TFormat)>.Fail(d4.ErrorMessage);
        var d5 = _f5.Decode(ops, d4.GetOrThrow().Item2);
        if (d5.IsError)
            return DataResult<(T, TFormat)>.Fail(d5.ErrorMessage);

        var instance = _factory(
            d0.GetOrThrow().Item1,
            d1.GetOrThrow().Item1,
            d2.GetOrThrow().Item1,
            d3.GetOrThrow().Item1,
            d4.GetOrThrow().Item1,
            d5.GetOrThrow().Item1
        );
        return DataResult<(T, TFormat)>.Success((instance, d5.GetOrThrow().Item2));
    }

    public DataResult<TFormat> Encode<TOps, TFormat>(T input, TOps ops, TFormat prefix)
        where TOps : IDynamicOps<TFormat>
    {
        var e0 = _f0.Encode(input, ops, ops.CreateEmptyMap());
        if (e0.IsError)
            return e0;
        var e1 = _f1.Encode(input, ops, e0.GetOrThrow());
        if (e1.IsError)
            return e1;
        var e2 = _f2.Encode(input, ops, e1.GetOrThrow());
        if (e2.IsError)
            return e2;
        var e3 = _f3.Encode(input, ops, e2.GetOrThrow());
        if (e3.IsError)
            return e3;
        var e4 = _f4.Encode(input, ops, e3.GetOrThrow());
        if (e4.IsError)
            return e4;
        var e5 = _f5.Encode(input, ops, e4.GetOrThrow());
        if (e5.IsError)
            return e5;
        return DataResult<TFormat>.Success(ops.AppendToPrefix(prefix, e5.GetOrThrow()));
    }

    public DataResult<TFormat> EncodeStart<TOps, TFormat>(TOps ops, T input)
        where TOps : IDynamicOps<TFormat> => Encode(input, ops, ops.Empty());

    public DataResult<T> Parse<TOps, TFormat>(TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat>
    {
        var parsed = Decode(ops, input);
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

    public DataResult<(T, TFormat)> Decode<TOps, TFormat>(TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat>
    {
        var d0 = _f0.Decode(ops, input);
        if (d0.IsError)
            return DataResult<(T, TFormat)>.Fail(d0.ErrorMessage);
        var d1 = _f1.Decode(ops, d0.GetOrThrow().Item2);
        if (d1.IsError)
            return DataResult<(T, TFormat)>.Fail(d1.ErrorMessage);
        var d2 = _f2.Decode(ops, d1.GetOrThrow().Item2);
        if (d2.IsError)
            return DataResult<(T, TFormat)>.Fail(d2.ErrorMessage);
        var d3 = _f3.Decode(ops, d2.GetOrThrow().Item2);
        if (d3.IsError)
            return DataResult<(T, TFormat)>.Fail(d3.ErrorMessage);
        var d4 = _f4.Decode(ops, d3.GetOrThrow().Item2);
        if (d4.IsError)
            return DataResult<(T, TFormat)>.Fail(d4.ErrorMessage);
        var d5 = _f5.Decode(ops, d4.GetOrThrow().Item2);
        if (d5.IsError)
            return DataResult<(T, TFormat)>.Fail(d5.ErrorMessage);
        var d6 = _f6.Decode(ops, d5.GetOrThrow().Item2);
        if (d6.IsError)
            return DataResult<(T, TFormat)>.Fail(d6.ErrorMessage);

        var instance = _factory(
            d0.GetOrThrow().Item1,
            d1.GetOrThrow().Item1,
            d2.GetOrThrow().Item1,
            d3.GetOrThrow().Item1,
            d4.GetOrThrow().Item1,
            d5.GetOrThrow().Item1,
            d6.GetOrThrow().Item1
        );
        return DataResult<(T, TFormat)>.Success((instance, d6.GetOrThrow().Item2));
    }

    public DataResult<TFormat> Encode<TOps, TFormat>(T input, TOps ops, TFormat prefix)
        where TOps : IDynamicOps<TFormat>
    {
        var e0 = _f0.Encode(input, ops, ops.CreateEmptyMap());
        if (e0.IsError)
            return e0;
        var e1 = _f1.Encode(input, ops, e0.GetOrThrow());
        if (e1.IsError)
            return e1;
        var e2 = _f2.Encode(input, ops, e1.GetOrThrow());
        if (e2.IsError)
            return e2;
        var e3 = _f3.Encode(input, ops, e2.GetOrThrow());
        if (e3.IsError)
            return e3;
        var e4 = _f4.Encode(input, ops, e3.GetOrThrow());
        if (e4.IsError)
            return e4;
        var e5 = _f5.Encode(input, ops, e4.GetOrThrow());
        if (e5.IsError)
            return e5;
        var e6 = _f6.Encode(input, ops, e5.GetOrThrow());
        if (e6.IsError)
            return e6;
        return DataResult<TFormat>.Success(ops.AppendToPrefix(prefix, e6.GetOrThrow()));
    }

    public DataResult<TFormat> EncodeStart<TOps, TFormat>(TOps ops, T input)
        where TOps : IDynamicOps<TFormat> => Encode(input, ops, ops.Empty());

    public DataResult<T> Parse<TOps, TFormat>(TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat>
    {
        var parsed = Decode(ops, input);
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

    public DataResult<(T, TFormat)> Decode<TOps, TFormat>(TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat>
    {
        var d0 = _f0.Decode(ops, input);
        if (d0.IsError)
            return DataResult<(T, TFormat)>.Fail(d0.ErrorMessage);
        var d1 = _f1.Decode(ops, d0.GetOrThrow().Item2);
        if (d1.IsError)
            return DataResult<(T, TFormat)>.Fail(d1.ErrorMessage);
        var d2 = _f2.Decode(ops, d1.GetOrThrow().Item2);
        if (d2.IsError)
            return DataResult<(T, TFormat)>.Fail(d2.ErrorMessage);
        var d3 = _f3.Decode(ops, d2.GetOrThrow().Item2);
        if (d3.IsError)
            return DataResult<(T, TFormat)>.Fail(d3.ErrorMessage);
        var d4 = _f4.Decode(ops, d3.GetOrThrow().Item2);
        if (d4.IsError)
            return DataResult<(T, TFormat)>.Fail(d4.ErrorMessage);
        var d5 = _f5.Decode(ops, d4.GetOrThrow().Item2);
        if (d5.IsError)
            return DataResult<(T, TFormat)>.Fail(d5.ErrorMessage);
        var d6 = _f6.Decode(ops, d5.GetOrThrow().Item2);
        if (d6.IsError)
            return DataResult<(T, TFormat)>.Fail(d6.ErrorMessage);
        var d7 = _f7.Decode(ops, d6.GetOrThrow().Item2);
        if (d7.IsError)
            return DataResult<(T, TFormat)>.Fail(d7.ErrorMessage);

        var instance = _factory(
            d0.GetOrThrow().Item1,
            d1.GetOrThrow().Item1,
            d2.GetOrThrow().Item1,
            d3.GetOrThrow().Item1,
            d4.GetOrThrow().Item1,
            d5.GetOrThrow().Item1,
            d6.GetOrThrow().Item1,
            d7.GetOrThrow().Item1
        );
        return DataResult<(T, TFormat)>.Success((instance, d7.GetOrThrow().Item2));
    }

    public DataResult<TFormat> Encode<TOps, TFormat>(T input, TOps ops, TFormat prefix)
        where TOps : IDynamicOps<TFormat>
    {
        var e0 = _f0.Encode(input, ops, ops.CreateEmptyMap());
        if (e0.IsError)
            return e0;
        var e1 = _f1.Encode(input, ops, e0.GetOrThrow());
        if (e1.IsError)
            return e1;
        var e2 = _f2.Encode(input, ops, e1.GetOrThrow());
        if (e2.IsError)
            return e2;
        var e3 = _f3.Encode(input, ops, e2.GetOrThrow());
        if (e3.IsError)
            return e3;
        var e4 = _f4.Encode(input, ops, e3.GetOrThrow());
        if (e4.IsError)
            return e4;
        var e5 = _f5.Encode(input, ops, e4.GetOrThrow());
        if (e5.IsError)
            return e5;
        var e6 = _f6.Encode(input, ops, e5.GetOrThrow());
        if (e6.IsError)
            return e6;
        var e7 = _f7.Encode(input, ops, e6.GetOrThrow());
        if (e7.IsError)
            return e7;
        return DataResult<TFormat>.Success(ops.AppendToPrefix(prefix, e7.GetOrThrow()));
    }

    public DataResult<TFormat> EncodeStart<TOps, TFormat>(TOps ops, T input)
        where TOps : IDynamicOps<TFormat> => Encode(input, ops, ops.Empty());

    public DataResult<T> Parse<TOps, TFormat>(TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat>
    {
        var parsed = Decode(ops, input);
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

    public DataResult<(T, TFormat)> Decode<TOps, TFormat>(TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat>
    {
        var d0 = _f0.Decode(ops, input);
        if (d0.IsError)
            return DataResult<(T, TFormat)>.Fail(d0.ErrorMessage);
        var d1 = _f1.Decode(ops, d0.GetOrThrow().Item2);
        if (d1.IsError)
            return DataResult<(T, TFormat)>.Fail(d1.ErrorMessage);
        var d2 = _f2.Decode(ops, d1.GetOrThrow().Item2);
        if (d2.IsError)
            return DataResult<(T, TFormat)>.Fail(d2.ErrorMessage);
        var d3 = _f3.Decode(ops, d2.GetOrThrow().Item2);
        if (d3.IsError)
            return DataResult<(T, TFormat)>.Fail(d3.ErrorMessage);
        var d4 = _f4.Decode(ops, d3.GetOrThrow().Item2);
        if (d4.IsError)
            return DataResult<(T, TFormat)>.Fail(d4.ErrorMessage);
        var d5 = _f5.Decode(ops, d4.GetOrThrow().Item2);
        if (d5.IsError)
            return DataResult<(T, TFormat)>.Fail(d5.ErrorMessage);
        var d6 = _f6.Decode(ops, d5.GetOrThrow().Item2);
        if (d6.IsError)
            return DataResult<(T, TFormat)>.Fail(d6.ErrorMessage);
        var d7 = _f7.Decode(ops, d6.GetOrThrow().Item2);
        if (d7.IsError)
            return DataResult<(T, TFormat)>.Fail(d7.ErrorMessage);
        var d8 = _f8.Decode(ops, d7.GetOrThrow().Item2);
        if (d8.IsError)
            return DataResult<(T, TFormat)>.Fail(d8.ErrorMessage);

        var instance = _factory(
            d0.GetOrThrow().Item1,
            d1.GetOrThrow().Item1,
            d2.GetOrThrow().Item1,
            d3.GetOrThrow().Item1,
            d4.GetOrThrow().Item1,
            d5.GetOrThrow().Item1,
            d6.GetOrThrow().Item1,
            d7.GetOrThrow().Item1,
            d8.GetOrThrow().Item1
        );
        return DataResult<(T, TFormat)>.Success((instance, d8.GetOrThrow().Item2));
    }

    public DataResult<TFormat> Encode<TOps, TFormat>(T input, TOps ops, TFormat prefix)
        where TOps : IDynamicOps<TFormat>
    {
        var e0 = _f0.Encode(input, ops, ops.CreateEmptyMap());
        if (e0.IsError)
            return e0;
        var e1 = _f1.Encode(input, ops, e0.GetOrThrow());
        if (e1.IsError)
            return e1;
        var e2 = _f2.Encode(input, ops, e1.GetOrThrow());
        if (e2.IsError)
            return e2;
        var e3 = _f3.Encode(input, ops, e2.GetOrThrow());
        if (e3.IsError)
            return e3;
        var e4 = _f4.Encode(input, ops, e3.GetOrThrow());
        if (e4.IsError)
            return e4;
        var e5 = _f5.Encode(input, ops, e4.GetOrThrow());
        if (e5.IsError)
            return e5;
        var e6 = _f6.Encode(input, ops, e5.GetOrThrow());
        if (e6.IsError)
            return e6;
        var e7 = _f7.Encode(input, ops, e6.GetOrThrow());
        if (e7.IsError)
            return e7;
        var e8 = _f8.Encode(input, ops, e7.GetOrThrow());
        if (e8.IsError)
            return e8;
        return DataResult<TFormat>.Success(ops.AppendToPrefix(prefix, e8.GetOrThrow()));
    }

    public DataResult<TFormat> EncodeStart<TOps, TFormat>(TOps ops, T input)
        where TOps : IDynamicOps<TFormat> => Encode(input, ops, ops.Empty());

    public DataResult<T> Parse<TOps, TFormat>(TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat>
    {
        var parsed = Decode(ops, input);
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

    public DataResult<(T, TFormat)> Decode<TOps, TFormat>(TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat>
    {
        var d0 = _f0.Decode(ops, input);
        if (d0.IsError)
            return DataResult<(T, TFormat)>.Fail(d0.ErrorMessage);
        var d1 = _f1.Decode(ops, d0.GetOrThrow().Item2);
        if (d1.IsError)
            return DataResult<(T, TFormat)>.Fail(d1.ErrorMessage);
        var d2 = _f2.Decode(ops, d1.GetOrThrow().Item2);
        if (d2.IsError)
            return DataResult<(T, TFormat)>.Fail(d2.ErrorMessage);
        var d3 = _f3.Decode(ops, d2.GetOrThrow().Item2);
        if (d3.IsError)
            return DataResult<(T, TFormat)>.Fail(d3.ErrorMessage);
        var d4 = _f4.Decode(ops, d3.GetOrThrow().Item2);
        if (d4.IsError)
            return DataResult<(T, TFormat)>.Fail(d4.ErrorMessage);
        var d5 = _f5.Decode(ops, d4.GetOrThrow().Item2);
        if (d5.IsError)
            return DataResult<(T, TFormat)>.Fail(d5.ErrorMessage);
        var d6 = _f6.Decode(ops, d5.GetOrThrow().Item2);
        if (d6.IsError)
            return DataResult<(T, TFormat)>.Fail(d6.ErrorMessage);
        var d7 = _f7.Decode(ops, d6.GetOrThrow().Item2);
        if (d7.IsError)
            return DataResult<(T, TFormat)>.Fail(d7.ErrorMessage);
        var d8 = _f8.Decode(ops, d7.GetOrThrow().Item2);
        if (d8.IsError)
            return DataResult<(T, TFormat)>.Fail(d8.ErrorMessage);
        var d9 = _f9.Decode(ops, d8.GetOrThrow().Item2);
        if (d9.IsError)
            return DataResult<(T, TFormat)>.Fail(d9.ErrorMessage);

        var instance = _factory(
            d0.GetOrThrow().Item1,
            d1.GetOrThrow().Item1,
            d2.GetOrThrow().Item1,
            d3.GetOrThrow().Item1,
            d4.GetOrThrow().Item1,
            d5.GetOrThrow().Item1,
            d6.GetOrThrow().Item1,
            d7.GetOrThrow().Item1,
            d8.GetOrThrow().Item1,
            d9.GetOrThrow().Item1
        );
        return DataResult<(T, TFormat)>.Success((instance, d9.GetOrThrow().Item2));
    }

    public DataResult<TFormat> Encode<TOps, TFormat>(T input, TOps ops, TFormat prefix)
        where TOps : IDynamicOps<TFormat>
    {
        var e0 = _f0.Encode(input, ops, ops.CreateEmptyMap());
        if (e0.IsError)
            return e0;
        var e1 = _f1.Encode(input, ops, e0.GetOrThrow());
        if (e1.IsError)
            return e1;
        var e2 = _f2.Encode(input, ops, e1.GetOrThrow());
        if (e2.IsError)
            return e2;
        var e3 = _f3.Encode(input, ops, e2.GetOrThrow());
        if (e3.IsError)
            return e3;
        var e4 = _f4.Encode(input, ops, e3.GetOrThrow());
        if (e4.IsError)
            return e4;
        var e5 = _f5.Encode(input, ops, e4.GetOrThrow());
        if (e5.IsError)
            return e5;
        var e6 = _f6.Encode(input, ops, e5.GetOrThrow());
        if (e6.IsError)
            return e6;
        var e7 = _f7.Encode(input, ops, e6.GetOrThrow());
        if (e7.IsError)
            return e7;
        var e8 = _f8.Encode(input, ops, e7.GetOrThrow());
        if (e8.IsError)
            return e8;
        var e9 = _f9.Encode(input, ops, e8.GetOrThrow());
        if (e9.IsError)
            return e9;
        return DataResult<TFormat>.Success(ops.AppendToPrefix(prefix, e9.GetOrThrow()));
    }

    public DataResult<TFormat> EncodeStart<TOps, TFormat>(TOps ops, T input)
        where TOps : IDynamicOps<TFormat> => Encode(input, ops, ops.Empty());

    public DataResult<T> Parse<TOps, TFormat>(TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat>
    {
        var parsed = Decode(ops, input);
        if (parsed.IsError)
            return DataResult<T>.Fail(parsed.ErrorMessage);
        return DataResult<T>.Success(parsed.GetOrThrow().Item1);
    }
}
