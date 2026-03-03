using WhiteTowerGames.DataFixerSharper.Schemas;

namespace WhiteTowerGames.DataFixerSharper.Datafixers;

public sealed class SchemaDrivenFix<TFormat> : IDataFix<TFormat>
{
    public Version Since { get; init; }
    private readonly ISchemaType _schema;
    private readonly Func<Dynamic<TFormat>, DynamicResult<TFormat>> _transformer;

    public SchemaDrivenFix(
        Version since,
        ISchemaType schema,
        Func<Dynamic<TFormat>, DynamicResult<TFormat>> transformer
    )
    {
        Since = since;
        _schema = schema;
        _transformer = transformer;
    }

    public DynamicResult<TFormat> Apply(DynamicResult<TFormat> input) =>
        _schema.Rewrite(input, _transformer);
}
