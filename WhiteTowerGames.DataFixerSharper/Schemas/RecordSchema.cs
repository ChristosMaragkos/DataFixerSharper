using WhiteTowerGames.DataFixerSharper.Abstractions;
using WhiteTowerGames.DataFixerSharper.Datafixers;

namespace WhiteTowerGames.DataFixerSharper.Schemas;

public sealed class RecordSchema : ISchemaType
{
    public IReadOnlyDictionary<string, ISchemaType> Fields { get; }

    public RecordSchema(IReadOnlyDictionary<string, ISchemaType> fields)
    {
        Fields = fields;
    }

    public DynamicResult<TOps, TFormat> Rewrite<TOps, TFormat>(
        DynamicResult<TOps, TFormat> data,
        Func<Dynamic<TOps, TFormat>, DynamicResult<TOps, TFormat>> transformer
    )
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
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
