using WhiteTowerGames.DataFixerSharper.Datafixers;

namespace WhiteTowerGames.DataFixerSharper.Schemas;

public sealed class PrimitiveSchema : ISchemaType
{
    public DynamicResult<TFormat> Rewrite<TFormat>(
        DynamicResult<TFormat> data,
        Func<Dynamic<TFormat>, DynamicResult<TFormat>> transformer
    ) => data.Map(transformer);

    internal PrimitiveSchema() { }
}
