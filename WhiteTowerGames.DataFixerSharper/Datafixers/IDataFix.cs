using WhiteTowerGames.DataFixerSharper.Abstractions;

namespace WhiteTowerGames.DataFixerSharper.Datafixers;

public interface IDataFix
{
    Version Since { get; init; }
    DataResult<Dynamic<TFormat>> Apply<TFormat>(Dynamic<TFormat> input);
}
