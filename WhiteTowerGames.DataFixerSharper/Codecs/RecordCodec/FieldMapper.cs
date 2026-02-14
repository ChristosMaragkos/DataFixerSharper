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
