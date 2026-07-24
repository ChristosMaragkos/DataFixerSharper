using WhiteTowerGames.DataFixerSharper.Abstractions;
using WhiteTowerGames.DataFixerSharper.Datafixers;
using WhiteTowerGames.DataFixerSharper.Schemas;

namespace WhiteTowerGames.DataFixerSharper.Versioning;

public class Timeline<T, TOps, TFormat>
    where TOps : IDynamicOps<TFormat>
    where TFormat : struct
{
    public IReadOnlyList<IDataFix<TOps, TFormat>> Fixes { get; }
    public ISchemaType LatestSchema { get; }

    internal Timeline(IReadOnlyList<IDataFix<TOps, TFormat>> fixes, ISchemaType latestSchema)
    {
        Fixes = fixes;
        LatestSchema = latestSchema;
    }
}

public class TimelineBuilder<TOps, TFormat>
    where TOps : IDynamicOps<TFormat>
    where TFormat : struct
{
    private readonly List<IDataFix<TOps, TFormat>> _completedFixes = new();
    private Dictionary<string, ISchemaType> _currentFields = new();

    private TimelineBuilder() { }

    public static TimelineBuilder<TOps, TFormat> Create() => new TimelineBuilder<TOps, TFormat>();

    public TimelineBuilder<TOps, TFormat> BaseSchema(Dictionary<string, ISchemaType> initialFields)
    {
        _currentFields = new(initialFields);
        return this;
    }

    internal TimelineBuilder<TOps, TFormat> AddVersion(
        IDataFix<TOps, TFormat> fix,
        Dictionary<string, ISchemaType> resultingFields
    )
    {
        _completedFixes.Add(fix);
        _currentFields = resultingFields;
        return this;
    }

    public VersionStepBuilder<TOps, TFormat> SinceVersion(Version version)
    {
        var schemaSnapshot = new RecordSchema(new Dictionary<string, ISchemaType>(_currentFields));
        var mutableFields = new Dictionary<string, ISchemaType>(_currentFields);
        return new VersionStepBuilder<TOps, TFormat>(version, mutableFields, this, schemaSnapshot);
    }

    public Timeline<T, TOps, TFormat> Build<T>() =>
        new Timeline<T, TOps, TFormat>(_completedFixes, new RecordSchema(_currentFields));
}
