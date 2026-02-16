namespace WhiteTowerGames.DataFixerSharper.Abstractions;

public interface IListBuilder<TFormat>
{
    IListBuilder<TFormat> Add(TFormat value);
    IListBuilder<TFormat> Add(DataResult<TFormat> value);
    DataResult<TFormat> Build(TFormat prefix);
}
