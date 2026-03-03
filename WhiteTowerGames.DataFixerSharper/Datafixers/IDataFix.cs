namespace WhiteTowerGames.DataFixerSharper.Datafixers;

public interface IDataFix<TFormat>
{
    Version Since { get; init; }
    DynamicResult<TFormat> Apply(DynamicResult<TFormat> input);
}
