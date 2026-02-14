using WhiteTowerGames.DataFixerSharper.Abstractions;

namespace WhiteTowerGames.DataFixerSharper.Codecs.RecordCodec;

public interface IFieldMapper<T>
{
    DataResult<TFormat> Encode<TFormat>(T input, IDynamicOps<TFormat> ops, TFormat prefix);
    DataResult<(T, TFormat)> Decode<TFormat>(IDynamicOps<TFormat> ops, TFormat input);
}

public class Fm1<T, TF> : IFieldMapper<T>
{
    private readonly IFieldCodec<T, TF> _f1;

    public Fm1(IFieldCodec<T, TF> f1)
    {
        _f1 = f1;
    }

    private Func<TF, T>? _ctor;

    public DataResult<(T, TFormat)> Decode<TFormat>(IDynamicOps<TFormat> ops, TFormat input) =>
        _f1.Decode(ops, input).Map(result => (_ctor!(result.Item1), result.Item2));

    public DataResult<TFormat> Encode<TFormat>(T input, IDynamicOps<TFormat> ops, TFormat prefix) =>
        _f1.Encode(input, ops, prefix);

    public IFieldMapper<T> WithCtor(Func<TF, T> ctor)
    {
        _ctor = ctor;
        return this;
    }
}

public class Fm2<T, TF, TF1> : IFieldMapper<T>
{
    private readonly IFieldCodec<T, TF> _f1;
    private readonly IFieldCodec<T, TF1> _f2;

    public Fm2(IFieldCodec<T, TF> f1, IFieldCodec<T, TF1> f2)
    {
        _f1 = f1;
        _f2 = f2;
    }

    private Func<TF, TF1, T>? _ctor;

    public DataResult<(T, TFormat)> Decode<TFormat>(IDynamicOps<TFormat> ops, TFormat input)
    {
        var r1 = _f1.Decode(ops, input);
        if (r1.IsError)
            return DataResult<(T, TFormat)>.Fail(r1.ErrorMessage);

        var r2 = _f2.Decode(ops, input);
        if (r2.IsError)
            return DataResult<(T, TFormat)>.Fail(r2.ErrorMessage);

        return DataResult<(T, TFormat)>.Success(
            (_ctor!(r1.GetOrThrow().Item1, r2.GetOrThrow().Item1), input)
        );
    }

    public DataResult<TFormat> Encode<TFormat>(T input, IDynamicOps<TFormat> ops, TFormat prefix)
    {
        var r1 = _f1.Encode(input, ops, prefix);
        if (r1.IsError)
            return DataResult<TFormat>.Fail(r1.ErrorMessage);

        var r2 = _f2.Encode(input, ops, r1.GetOrThrow());
        if (r2.IsError)
            return DataResult<TFormat>.Fail(r2.ErrorMessage);

        return r2;
    }

    public IFieldMapper<T> WithCtor(Func<TF, TF1, T> ctor)
    {
        _ctor = ctor;
        return this;
    }
}

public class Fm3<T, TF, TF1, TF2> : IFieldMapper<T>
{
    private readonly IFieldCodec<T, TF> _f1;
    private readonly IFieldCodec<T, TF1> _f2;
    private readonly IFieldCodec<T, TF2> _f3;

    public Fm3(IFieldCodec<T, TF> f1, IFieldCodec<T, TF1> f2, IFieldCodec<T, TF2> f3)
    {
        _f1 = f1;
        _f2 = f2;
        _f3 = f3;
    }

    private Func<TF, TF1, TF2, T>? _ctor;

    public DataResult<(T, TFormat)> Decode<TFormat>(IDynamicOps<TFormat> ops, TFormat input)
    {
        var r1 = _f1.Decode(ops, input);
        if (r1.IsError)
            return DataResult<(T, TFormat)>.Fail(r1.ErrorMessage);

        var r2 = _f2.Decode(ops, input);
        if (r2.IsError)
            return DataResult<(T, TFormat)>.Fail(r2.ErrorMessage);

        var r3 = _f3.Decode(ops, input);
        if (r3.IsError)
            return DataResult<(T, TFormat)>.Fail(r3.ErrorMessage);

        return DataResult<(T, TFormat)>.Success(
            (_ctor!(r1.GetOrThrow().Item1, r2.GetOrThrow().Item1, r3.GetOrThrow().Item1), input)
        );
    }

    public DataResult<TFormat> Encode<TFormat>(T input, IDynamicOps<TFormat> ops, TFormat prefix)
    {
        var r1 = _f1.Encode(input, ops, prefix);
        if (r1.IsError)
            return DataResult<TFormat>.Fail(r1.ErrorMessage);

        var r2 = _f2.Encode(input, ops, r1.GetOrThrow());
        if (r2.IsError)
            return DataResult<TFormat>.Fail(r2.ErrorMessage);

        var r3 = _f3.Encode(input, ops, r2.GetOrThrow());
        if (r3.IsError)
            return DataResult<TFormat>.Fail(r3.ErrorMessage);

        return r3;
    }

    public IFieldMapper<T> WithCtor(Func<TF, TF1, TF2, T> ctor)
    {
        _ctor = ctor;
        return this;
    }
}

public class Fm4<T, TF, TF1, TF2, TF3> : IFieldMapper<T>
{
    private readonly IFieldCodec<T, TF> _f1;
    private readonly IFieldCodec<T, TF1> _f2;
    private readonly IFieldCodec<T, TF2> _f3;
    private readonly IFieldCodec<T, TF3> _f4;

    public Fm4(
        IFieldCodec<T, TF> f1,
        IFieldCodec<T, TF1> f2,
        IFieldCodec<T, TF2> f3,
        IFieldCodec<T, TF3> f4
    )
    {
        _f1 = f1;
        _f2 = f2;
        _f3 = f3;
        _f4 = f4;
    }

    private Func<TF, TF1, TF2, TF3, T>? _ctor;

    public DataResult<(T, TFormat)> Decode<TFormat>(IDynamicOps<TFormat> ops, TFormat input)
    {
        var r1 = _f1.Decode(ops, input);
        if (r1.IsError)
            return DataResult<(T, TFormat)>.Fail(r1.ErrorMessage);

        var r2 = _f2.Decode(ops, input);
        if (r2.IsError)
            return DataResult<(T, TFormat)>.Fail(r2.ErrorMessage);

        var r3 = _f3.Decode(ops, input);
        if (r3.IsError)
            return DataResult<(T, TFormat)>.Fail(r3.ErrorMessage);

        var r4 = _f4.Decode(ops, input);
        if (r4.IsError)
            return DataResult<(T, TFormat)>.Fail(r4.ErrorMessage);

        return DataResult<(T, TFormat)>.Success(
            (
                _ctor!(
                    r1.GetOrThrow().Item1,
                    r2.GetOrThrow().Item1,
                    r3.GetOrThrow().Item1,
                    r4.GetOrThrow().Item1
                ),
                input
            )
        );
    }

    public DataResult<TFormat> Encode<TFormat>(T input, IDynamicOps<TFormat> ops, TFormat prefix)
    {
        var r1 = _f1.Encode(input, ops, prefix);
        if (r1.IsError)
            return DataResult<TFormat>.Fail(r1.ErrorMessage);

        var r2 = _f2.Encode(input, ops, r1.GetOrThrow());
        if (r2.IsError)
            return DataResult<TFormat>.Fail(r2.ErrorMessage);

        var r3 = _f3.Encode(input, ops, r2.GetOrThrow());
        if (r3.IsError)
            return DataResult<TFormat>.Fail(r3.ErrorMessage);

        var r4 = _f4.Encode(input, ops, r3.GetOrThrow());
        if (r4.IsError)
            return DataResult<TFormat>.Fail(r4.ErrorMessage);

        return r4;
    }

    public IFieldMapper<T> WithCtor(Func<TF, TF1, TF2, TF3, T> ctor)
    {
        _ctor = ctor;
        return this;
    }
}

public class Fm5<T, TF, TF1, TF2, TF3, TF4> : IFieldMapper<T>
{
    private readonly IFieldCodec<T, TF> _f1;
    private readonly IFieldCodec<T, TF1> _f2;
    private readonly IFieldCodec<T, TF2> _f3;
    private readonly IFieldCodec<T, TF3> _f4;
    private readonly IFieldCodec<T, TF4> _f5;

    public Fm5(
        IFieldCodec<T, TF> f1,
        IFieldCodec<T, TF1> f2,
        IFieldCodec<T, TF2> f3,
        IFieldCodec<T, TF3> f4,
        IFieldCodec<T, TF4> f5
    )
    {
        _f1 = f1;
        _f2 = f2;
        _f3 = f3;
        _f4 = f4;
        _f5 = f5;
    }

    private Func<TF, TF1, TF2, TF3, TF4, T>? _ctor;

    public DataResult<(T, TFormat)> Decode<TFormat>(IDynamicOps<TFormat> ops, TFormat input)
    {
        var r1 = _f1.Decode(ops, input);
        if (r1.IsError)
            return DataResult<(T, TFormat)>.Fail(r1.ErrorMessage);

        var r2 = _f2.Decode(ops, input);
        if (r2.IsError)
            return DataResult<(T, TFormat)>.Fail(r2.ErrorMessage);

        var r3 = _f3.Decode(ops, input);
        if (r3.IsError)
            return DataResult<(T, TFormat)>.Fail(r3.ErrorMessage);

        var r4 = _f4.Decode(ops, input);
        if (r4.IsError)
            return DataResult<(T, TFormat)>.Fail(r4.ErrorMessage);

        var r5 = _f5.Decode(ops, input);
        if (r5.IsError)
            return DataResult<(T, TFormat)>.Fail(r5.ErrorMessage);

        return DataResult<(T, TFormat)>.Success(
            (
                _ctor!(
                    r1.GetOrThrow().Item1,
                    r2.GetOrThrow().Item1,
                    r3.GetOrThrow().Item1,
                    r4.GetOrThrow().Item1,
                    r5.GetOrThrow().Item1
                ),
                input
            )
        );
    }

    public DataResult<TFormat> Encode<TFormat>(T input, IDynamicOps<TFormat> ops, TFormat prefix)
    {
        var r1 = _f1.Encode(input, ops, prefix);
        if (r1.IsError)
            return DataResult<TFormat>.Fail(r1.ErrorMessage);

        var r2 = _f2.Encode(input, ops, r1.GetOrThrow());
        if (r2.IsError)
            return DataResult<TFormat>.Fail(r2.ErrorMessage);

        var r3 = _f3.Encode(input, ops, r2.GetOrThrow());
        if (r3.IsError)
            return DataResult<TFormat>.Fail(r3.ErrorMessage);

        var r4 = _f4.Encode(input, ops, r3.GetOrThrow());
        if (r4.IsError)
            return DataResult<TFormat>.Fail(r4.ErrorMessage);

        var r5 = _f5.Encode(input, ops, r4.GetOrThrow());
        if (r5.IsError)
            return DataResult<TFormat>.Fail(r5.ErrorMessage);

        return r5;
    }

    public IFieldMapper<T> WithCtor(Func<TF, TF1, TF2, TF3, TF4, T> ctor)
    {
        _ctor = ctor;
        return this;
    }
}
