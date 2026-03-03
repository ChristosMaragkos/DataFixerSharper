using WhiteTowerGames.DataFixerSharper.Datafixers;

namespace WhiteTowerGames.DataFixerSharper.Schemas;

public sealed class ListSchema : ISchemaType
{
    private readonly ISchemaType _elementSchema;

    public ListSchema(ISchemaType elementSchema)
    {
        _elementSchema = elementSchema;
    }

    public DynamicResult<TFormat> Rewrite<TFormat>(
        DynamicResult<TFormat> data,
        Func<Dynamic<TFormat>, DynamicResult<TFormat>> transformer
    )
    {
        var walkedList = data.Map(dyn =>
            dyn.UpdateList(itemDyn => _elementSchema.Rewrite(itemDyn, transformer))
        );

        return walkedList.Map(transformer);
    }
}
