namespace WhiteTowerGames.DataFixerSharper.Codecs.RecordCodec;

public static class RecordCodecBuilder
{
    public static ICodec<T> Create<T>(Func<Instance<T>, ICodec<T>> builder) =>
        builder(new Instance<T>());
}

public readonly struct Instance<T>
{
    public MapCodec1<T, TF> WithFields<TF>(IFieldCodec<T, TF> f1) => new MapCodec1<T, TF>(f1);

    public MapCodec2<T, TF, TF1> WithFields<TF, TF1>(
        IFieldCodec<T, TF> f0,
        IFieldCodec<T, TF1> f1
    ) => new MapCodec2<T, TF, TF1>(f0, f1);
}

public readonly struct MapCodec1<T, TF>
{
    private readonly IFieldCodec<T, TF> _f0;

    public MapCodec1(IFieldCodec<T, TF> f1)
    {
        _f0 = f1;
    }

    public RecordCodec1<T, TF> WithCtor(Func<TF, T> ctor) => new(_f0, ctor);
}

public readonly struct MapCodec2<T, TF, TF1>
{
    private readonly IFieldCodec<T, TF> _f0;
    private readonly IFieldCodec<T, TF1> _f1;

    public MapCodec2(IFieldCodec<T, TF> f0, IFieldCodec<T, TF1> f1)
    {
        _f0 = f0;
        _f1 = f1;
    }

    public RecordCodec2<T, TF, TF1> WithCtor(Func<TF, TF1, T> ctor) => new(_f0, _f1, ctor);
}

public readonly struct MapCodec3<T, TF, TF1, TF2>
{
    private readonly IFieldCodec<T, TF> _f0;
    private readonly IFieldCodec<T, TF1> _f1;
    private readonly IFieldCodec<T, TF2> _f2;

    public MapCodec3(IFieldCodec<T, TF> f0, IFieldCodec<T, TF1> f1, IFieldCodec<T, TF2> f2)
    {
        _f0 = f0;
        _f1 = f1;
        _f2 = f2;
    }

    public RecordCodec3<T, TF, TF1, TF2> WithCtor(Func<TF, TF1, TF2, T> ctor) =>
        new(_f0, _f1, _f2, ctor);
}
