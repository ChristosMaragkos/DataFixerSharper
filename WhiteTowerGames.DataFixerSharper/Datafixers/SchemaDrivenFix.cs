using WhiteTowerGames.DataFixerSharper.Abstractions;
using WhiteTowerGames.DataFixerSharper.Schemas;

namespace WhiteTowerGames.DataFixerSharper.Datafixers;

public sealed class SchemaDrivenFix<TOps, TFormat> : IDataFix<TOps, TFormat>
    where TOps : IDynamicOps<TFormat>
    where TFormat : struct
{
    public Version Since { get; init; }
    private readonly ISchemaType _schema;
    private readonly Func<Dynamic<TOps, TFormat>, DynamicResult<TOps, TFormat>> _transformer;

    public SchemaDrivenFix(
        Version since,
        ISchemaType schema,
        Func<Dynamic<TOps, TFormat>, DynamicResult<TOps, TFormat>> transformer
    )
    {
        Since = since;
        _schema = schema;
        _transformer = transformer;
    }

    public DynamicResult<TOps, TFormat> Apply(DynamicResult<TOps, TFormat> input) =>
        _schema.Rewrite(input, _transformer);
}
