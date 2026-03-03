using WhiteTowerGames.DataFixerSharper.Datafixers;

namespace WhiteTowerGames.DataFixerSharper.Schemas;

public interface ISchemaType
{
    DynamicResult<TFormat> Rewrite<TFormat>(
        DynamicResult<TFormat> data,
        Func<Dynamic<TFormat>, DynamicResult<TFormat>> transformer
    );
}
