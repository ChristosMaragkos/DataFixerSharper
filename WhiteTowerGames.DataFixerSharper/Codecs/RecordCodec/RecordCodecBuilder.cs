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

    public MapCodec3<T, TF, TF1, TF2> WithFields<TF, TF1, TF2>(
        IFieldCodec<T, TF> f0,
        IFieldCodec<T, TF1> f1,
        IFieldCodec<T, TF2> f2
    ) => new MapCodec3<T, TF, TF1, TF2>(f0, f1, f2);

    public MapCodec4<T, TF, TF1, TF2, TF3> WithFields<TF, TF1, TF2, TF3>(
        IFieldCodec<T, TF> f0,
        IFieldCodec<T, TF1> f1,
        IFieldCodec<T, TF2> f2,
        IFieldCodec<T, TF3> f3
    ) => new MapCodec4<T, TF, TF1, TF2, TF3>(f0, f1, f2, f3);

    public MapCodec5<T, TF, TF1, TF2, TF3, TF4> WithFields<TF, TF1, TF2, TF3, TF4>(
        IFieldCodec<T, TF> f0,
        IFieldCodec<T, TF1> f1,
        IFieldCodec<T, TF2> f2,
        IFieldCodec<T, TF3> f3,
        IFieldCodec<T, TF4> f4
    ) => new MapCodec5<T, TF, TF1, TF2, TF3, TF4>(f0, f1, f2, f3, f4);

    public MapCodec6<T, TF, TF1, TF2, TF3, TF4, TF5> WithFields<TF, TF1, TF2, TF3, TF4, TF5>(
        IFieldCodec<T, TF> f0,
        IFieldCodec<T, TF1> f1,
        IFieldCodec<T, TF2> f2,
        IFieldCodec<T, TF3> f3,
        IFieldCodec<T, TF4> f4,
        IFieldCodec<T, TF5> f5
    ) => new MapCodec6<T, TF, TF1, TF2, TF3, TF4, TF5>(f0, f1, f2, f3, f4, f5);

    public MapCodec7<T, TF, TF1, TF2, TF3, TF4, TF5, TF6> WithFields<
        TF,
        TF1,
        TF2,
        TF3,
        TF4,
        TF5,
        TF6
    >(
        IFieldCodec<T, TF> f0,
        IFieldCodec<T, TF1> f1,
        IFieldCodec<T, TF2> f2,
        IFieldCodec<T, TF3> f3,
        IFieldCodec<T, TF4> f4,
        IFieldCodec<T, TF5> f5,
        IFieldCodec<T, TF6> f6
    ) => new MapCodec7<T, TF, TF1, TF2, TF3, TF4, TF5, TF6>(f0, f1, f2, f3, f4, f5, f6);

    public MapCodec8<T, TF, TF1, TF2, TF3, TF4, TF5, TF6, TF7> WithFields<
        TF,
        TF1,
        TF2,
        TF3,
        TF4,
        TF5,
        TF6,
        TF7
    >(
        IFieldCodec<T, TF> f0,
        IFieldCodec<T, TF1> f1,
        IFieldCodec<T, TF2> f2,
        IFieldCodec<T, TF3> f3,
        IFieldCodec<T, TF4> f4,
        IFieldCodec<T, TF5> f5,
        IFieldCodec<T, TF6> f6,
        IFieldCodec<T, TF7> f7
    ) => new MapCodec8<T, TF, TF1, TF2, TF3, TF4, TF5, TF6, TF7>(f0, f1, f2, f3, f4, f5, f6, f7);

    public MapCodec9<T, TF, TF1, TF2, TF3, TF4, TF5, TF6, TF7, TF8> WithFields<
        TF,
        TF1,
        TF2,
        TF3,
        TF4,
        TF5,
        TF6,
        TF7,
        TF8
    >(
        IFieldCodec<T, TF> f0,
        IFieldCodec<T, TF1> f1,
        IFieldCodec<T, TF2> f2,
        IFieldCodec<T, TF3> f3,
        IFieldCodec<T, TF4> f4,
        IFieldCodec<T, TF5> f5,
        IFieldCodec<T, TF6> f6,
        IFieldCodec<T, TF7> f7,
        IFieldCodec<T, TF8> f8
    ) =>
        new MapCodec9<T, TF, TF1, TF2, TF3, TF4, TF5, TF6, TF7, TF8>(
            f0,
            f1,
            f2,
            f3,
            f4,
            f5,
            f6,
            f7,
            f8
        );

    public MapCodec10<T, TF, TF1, TF2, TF3, TF4, TF5, TF6, TF7, TF8, TF9> WithFields<
        TF,
        TF1,
        TF2,
        TF3,
        TF4,
        TF5,
        TF6,
        TF7,
        TF8,
        TF9
    >(
        IFieldCodec<T, TF> f0,
        IFieldCodec<T, TF1> f1,
        IFieldCodec<T, TF2> f2,
        IFieldCodec<T, TF3> f3,
        IFieldCodec<T, TF4> f4,
        IFieldCodec<T, TF5> f5,
        IFieldCodec<T, TF6> f6,
        IFieldCodec<T, TF7> f7,
        IFieldCodec<T, TF8> f8,
        IFieldCodec<T, TF9> f9
    ) =>
        new MapCodec10<T, TF, TF1, TF2, TF3, TF4, TF5, TF6, TF7, TF8, TF9>(
            f0,
            f1,
            f2,
            f3,
            f4,
            f5,
            f6,
            f7,
            f8,
            f9
        );
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

public readonly struct MapCodec4<T, TF, TF1, TF2, TF3>
{
    private readonly IFieldCodec<T, TF> _f0;
    private readonly IFieldCodec<T, TF1> _f1;
    private readonly IFieldCodec<T, TF2> _f2;
    private readonly IFieldCodec<T, TF3> _f3;

    public MapCodec4(
        IFieldCodec<T, TF> f0,
        IFieldCodec<T, TF1> f1,
        IFieldCodec<T, TF2> f2,
        IFieldCodec<T, TF3> f3
    )
    {
        _f0 = f0;
        _f1 = f1;
        _f2 = f2;
        _f3 = f3;
    }

    public RecordCodec4<T, TF, TF1, TF2, TF3> WithCtor(Func<TF, TF1, TF2, TF3, T> ctor) =>
        new(_f0, _f1, _f2, _f3, ctor);
}

public readonly struct MapCodec5<T, TF, TF1, TF2, TF3, TF4>
{
    private readonly IFieldCodec<T, TF> _f0;
    private readonly IFieldCodec<T, TF1> _f1;
    private readonly IFieldCodec<T, TF2> _f2;
    private readonly IFieldCodec<T, TF3> _f3;
    private readonly IFieldCodec<T, TF4> _f4;

    public MapCodec5(
        IFieldCodec<T, TF> f0,
        IFieldCodec<T, TF1> f1,
        IFieldCodec<T, TF2> f2,
        IFieldCodec<T, TF3> f3,
        IFieldCodec<T, TF4> f4
    )
    {
        _f0 = f0;
        _f1 = f1;
        _f2 = f2;
        _f3 = f3;
        _f4 = f4;
    }

    public RecordCodec5<T, TF, TF1, TF2, TF3, TF4> WithCtor(Func<TF, TF1, TF2, TF3, TF4, T> ctor) =>
        new(_f0, _f1, _f2, _f3, _f4, ctor);
}

public readonly struct MapCodec6<T, TF, TF1, TF2, TF3, TF4, TF5>
{
    private readonly IFieldCodec<T, TF> _f0;
    private readonly IFieldCodec<T, TF1> _f1;
    private readonly IFieldCodec<T, TF2> _f2;
    private readonly IFieldCodec<T, TF3> _f3;
    private readonly IFieldCodec<T, TF4> _f4;
    private readonly IFieldCodec<T, TF5> _f5;

    public MapCodec6(
        IFieldCodec<T, TF> f0,
        IFieldCodec<T, TF1> f1,
        IFieldCodec<T, TF2> f2,
        IFieldCodec<T, TF3> f3,
        IFieldCodec<T, TF4> f4,
        IFieldCodec<T, TF5> f5
    )
    {
        _f0 = f0;
        _f1 = f1;
        _f2 = f2;
        _f3 = f3;
        _f4 = f4;
        _f5 = f5;
    }

    public RecordCodec6<T, TF, TF1, TF2, TF3, TF4, TF5> WithCtor(
        Func<TF, TF1, TF2, TF3, TF4, TF5, T> ctor
    ) => new(_f0, _f1, _f2, _f3, _f4, _f5, ctor);
}

public readonly struct MapCodec7<T, TF, TF1, TF2, TF3, TF4, TF5, TF6>
{
    private readonly IFieldCodec<T, TF> _f0;
    private readonly IFieldCodec<T, TF1> _f1;
    private readonly IFieldCodec<T, TF2> _f2;
    private readonly IFieldCodec<T, TF3> _f3;
    private readonly IFieldCodec<T, TF4> _f4;
    private readonly IFieldCodec<T, TF5> _f5;
    private readonly IFieldCodec<T, TF6> _f6;

    public MapCodec7(
        IFieldCodec<T, TF> f0,
        IFieldCodec<T, TF1> f1,
        IFieldCodec<T, TF2> f2,
        IFieldCodec<T, TF3> f3,
        IFieldCodec<T, TF4> f4,
        IFieldCodec<T, TF5> f5,
        IFieldCodec<T, TF6> f6
    )
    {
        _f0 = f0;
        _f1 = f1;
        _f2 = f2;
        _f3 = f3;
        _f4 = f4;
        _f5 = f5;
        _f6 = f6;
    }

    public RecordCodec7<T, TF, TF1, TF2, TF3, TF4, TF5, TF6> WithCtor(
        Func<TF, TF1, TF2, TF3, TF4, TF5, TF6, T> ctor
    ) => new(_f0, _f1, _f2, _f3, _f4, _f5, _f6, ctor);
}

public readonly struct MapCodec8<T, TF, TF1, TF2, TF3, TF4, TF5, TF6, TF7>
{
    private readonly IFieldCodec<T, TF> _f0;
    private readonly IFieldCodec<T, TF1> _f1;
    private readonly IFieldCodec<T, TF2> _f2;
    private readonly IFieldCodec<T, TF3> _f3;
    private readonly IFieldCodec<T, TF4> _f4;
    private readonly IFieldCodec<T, TF5> _f5;
    private readonly IFieldCodec<T, TF6> _f6;
    private readonly IFieldCodec<T, TF7> _f7;

    public MapCodec8(
        IFieldCodec<T, TF> f0,
        IFieldCodec<T, TF1> f1,
        IFieldCodec<T, TF2> f2,
        IFieldCodec<T, TF3> f3,
        IFieldCodec<T, TF4> f4,
        IFieldCodec<T, TF5> f5,
        IFieldCodec<T, TF6> f6,
        IFieldCodec<T, TF7> f7
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
    }

    public RecordCodec8<T, TF, TF1, TF2, TF3, TF4, TF5, TF6, TF7> WithCtor(
        Func<TF, TF1, TF2, TF3, TF4, TF5, TF6, TF7, T> ctor
    ) => new(_f0, _f1, _f2, _f3, _f4, _f5, _f6, _f7, ctor);
}

public readonly struct MapCodec9<T, TF, TF1, TF2, TF3, TF4, TF5, TF6, TF7, TF8>
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

    public MapCodec9(
        IFieldCodec<T, TF> f0,
        IFieldCodec<T, TF1> f1,
        IFieldCodec<T, TF2> f2,
        IFieldCodec<T, TF3> f3,
        IFieldCodec<T, TF4> f4,
        IFieldCodec<T, TF5> f5,
        IFieldCodec<T, TF6> f6,
        IFieldCodec<T, TF7> f7,
        IFieldCodec<T, TF8> f8
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
    }

    public RecordCodec9<T, TF, TF1, TF2, TF3, TF4, TF5, TF6, TF7, TF8> WithCtor(
        Func<TF, TF1, TF2, TF3, TF4, TF5, TF6, TF7, TF8, T> ctor
    ) => new(_f0, _f1, _f2, _f3, _f4, _f5, _f6, _f7, _f8, ctor);
}

public readonly struct MapCodec10<T, TF, TF1, TF2, TF3, TF4, TF5, TF6, TF7, TF8, TF9>
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

    public MapCodec10(
        IFieldCodec<T, TF> f0,
        IFieldCodec<T, TF1> f1,
        IFieldCodec<T, TF2> f2,
        IFieldCodec<T, TF3> f3,
        IFieldCodec<T, TF4> f4,
        IFieldCodec<T, TF5> f5,
        IFieldCodec<T, TF6> f6,
        IFieldCodec<T, TF7> f7,
        IFieldCodec<T, TF8> f8,
        IFieldCodec<T, TF9> f9
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
    }

    public RecordCodec10<T, TF, TF1, TF2, TF3, TF4, TF5, TF6, TF7, TF8, TF9> WithCtor(
        Func<TF, TF1, TF2, TF3, TF4, TF5, TF6, TF7, TF8, TF9, T> ctor
    ) => new(_f0, _f1, _f2, _f3, _f4, _f5, _f6, _f7, _f8, _f9, ctor);
}
