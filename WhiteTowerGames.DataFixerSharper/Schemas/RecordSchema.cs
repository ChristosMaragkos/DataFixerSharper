using WhiteTowerGames.DataFixerSharper.Datafixers;

namespace WhiteTowerGames.DataFixerSharper.Schemas;

public sealed class RecordSchema : ISchemaType
{
    public IReadOnlyDictionary<string, ISchemaType> Fields { get; }

    public RecordSchema(IReadOnlyDictionary<string, ISchemaType> fields)
    {
        Fields = fields;
    }

    public DynamicResult<TFormat> Rewrite<TFormat>(
        DynamicResult<TFormat> data,
        Func<Dynamic<TFormat>, DynamicResult<TFormat>> transformer
    )
    {
        var walkedRecord = data.UpdateMap(
            (keyName, fieldData) =>
            {
                if (Fields.TryGetValue(keyName, out var schema))
                {
                    return schema.Rewrite(fieldData, transformer);
                }

                return fieldData;
            }
        );

        return walkedRecord.Map(transformer);
    }
}
