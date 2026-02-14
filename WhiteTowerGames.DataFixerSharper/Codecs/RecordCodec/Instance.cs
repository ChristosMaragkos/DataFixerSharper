namespace WhiteTowerGames.DataFixerSharper.Codecs.RecordCodec;

public class Instance<T>
{
    public Fm1<T, TF> WithFields<TF>(IFieldCodec<T, TF> f1) => new Fm1<T, TF>(f1);

    public Fm2<T, TF, TF1> WithFields<TF, TF1>(IFieldCodec<T, TF> f1, IFieldCodec<T, TF1> f2) =>
        new Fm2<T, TF, TF1>(f1, f2);
}
