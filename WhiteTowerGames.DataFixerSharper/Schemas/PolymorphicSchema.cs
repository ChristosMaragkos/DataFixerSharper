using WhiteTowerGames.DataFixerSharper.Abstractions;
using WhiteTowerGames.DataFixerSharper.Datafixers;

namespace WhiteTowerGames.DataFixerSharper.Schemas;

public sealed class PolymorphicSchema : ISchemaType
{
    public string IdField { get; }
    public IReadOnlyDictionary<string, ISchemaType> Choices { get; }

    public PolymorphicSchema(string idField, IReadOnlyDictionary<string, ISchemaType> choices)
    {
        IdField = idField;
        Choices = choices;
    }

    public DynamicResult<TOps, TFormat> Rewrite<TOps, TFormat>(
        DynamicResult<TOps, TFormat> data,
        Func<Dynamic<TOps, TFormat>, DynamicResult<TOps, TFormat>> transformer
    )
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        var idDataResult = data.Get(IdField);

        if (idDataResult.IsError)
            return data.Map(transformer);

        var idDyn = idDataResult.GetOrThrow();
        var typeStringResult = TOps.GetString(idDyn.Value);

        if (typeStringResult.IsError)
            return data.Map(transformer);

        var typeString = typeStringResult.GetOrThrow();

        if (Choices.TryGetValue(typeString, out var schema))
        {
            return schema.Rewrite(data, transformer);
        }

        return data.Map(transformer);
    }
}
