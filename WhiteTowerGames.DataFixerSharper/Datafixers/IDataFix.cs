using WhiteTowerGames.DataFixerSharper.Abstractions;

namespace WhiteTowerGames.DataFixerSharper.Datafixers;

public interface IDataFix<TOps, TFormat>
    where TOps : IDynamicOps<TFormat>
    where TFormat : struct
{
    Version Since { get; init; }
    DynamicResult<TOps, TFormat> Apply(DynamicResult<TOps, TFormat> input);
}
