namespace WhiteTowerGames.DataFixerSharper.Codecs.RecordCodec;

public class Instance<T>
{
    public Fm1<T, TF> WithFields<TF>(IFieldCodec<T, TF> f1) => new Fm1<T, TF>(f1);

    public Fm2<T, TF, TF1> WithFields<TF, TF1>(IFieldCodec<T, TF> f1, IFieldCodec<T, TF1> f2) =>
        new Fm2<T, TF, TF1>(f1, f2);

    public Fm3<T, TF, TF1, TF2> WithFields<TF, TF1, TF2>(
        IFieldCodec<T, TF> f1,
        IFieldCodec<T, TF1> f2,
        IFieldCodec<T, TF2> f3
    ) => new Fm3<T, TF, TF1, TF2>(f1, f2, f3);

    public Fm4<T, TF, TF1, TF2, TF3> WithFields<TF, TF1, TF2, TF3>(
        IFieldCodec<T, TF> f1,
        IFieldCodec<T, TF1> f2,
        IFieldCodec<T, TF2> f3,
        IFieldCodec<T, TF3> f4
    ) => new Fm4<T, TF, TF1, TF2, TF3>(f1, f2, f3, f4);

    public Fm5<T, TF, TF1, TF2, TF3, TF4> WithFields<TF, TF1, TF2, TF3, TF4>(
        IFieldCodec<T, TF> f1,
        IFieldCodec<T, TF1> f2,
        IFieldCodec<T, TF2> f3,
        IFieldCodec<T, TF3> f4,
        IFieldCodec<T, TF4> f5
    ) => new Fm5<T, TF, TF1, TF2, TF3, TF4>(f1, f2, f3, f4, f5);
}
