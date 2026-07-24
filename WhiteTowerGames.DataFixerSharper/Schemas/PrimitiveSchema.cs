using WhiteTowerGames.DataFixerSharper.Abstractions;
using WhiteTowerGames.DataFixerSharper.Datafixers;

namespace WhiteTowerGames.DataFixerSharper.Schemas;

public sealed class PrimitiveSchema : ISchemaType
{
    public DynamicResult<TOps, TFormat> Rewrite<TOps, TFormat>(
        DynamicResult<TOps, TFormat> data,
        Func<Dynamic<TOps, TFormat>, DynamicResult<TOps, TFormat>> transformer
    )
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
        => data.Map(transformer);

    internal PrimitiveSchema() { }
}
