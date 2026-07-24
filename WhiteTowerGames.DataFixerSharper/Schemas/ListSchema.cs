using WhiteTowerGames.DataFixerSharper.Abstractions;
using WhiteTowerGames.DataFixerSharper.Datafixers;

namespace WhiteTowerGames.DataFixerSharper.Schemas;

public sealed class ListSchema : ISchemaType
{
    private readonly ISchemaType _elementSchema;

    public ListSchema(ISchemaType elementSchema)
    {
        _elementSchema = elementSchema;
    }

    public DynamicResult<TOps, TFormat> Rewrite<TOps, TFormat>(
        DynamicResult<TOps, TFormat> data,
        Func<Dynamic<TOps, TFormat>, DynamicResult<TOps, TFormat>> transformer
    )
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        var walkedList = data.Map(dyn =>
            dyn.UpdateList(itemDyn => _elementSchema.Rewrite(itemDyn, transformer))
        );

        return walkedList.Map(transformer);
    }
}
