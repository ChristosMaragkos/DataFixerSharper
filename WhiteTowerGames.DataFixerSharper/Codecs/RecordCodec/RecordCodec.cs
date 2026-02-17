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
}
