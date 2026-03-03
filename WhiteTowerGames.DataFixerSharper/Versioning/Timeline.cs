using WhiteTowerGames.DataFixerSharper.Datafixers;
using WhiteTowerGames.DataFixerSharper.Schemas;

namespace WhiteTowerGames.DataFixerSharper.Versioning;

public class Timeline<T, TFormat>
{
    public IReadOnlyList<IDataFix<TFormat>> Fixes { get; }
    public ISchemaType LatestSchema { get; }

    internal Timeline(IReadOnlyList<IDataFix<TFormat>> fixes, ISchemaType latestSchema)
    {
        Fixes = fixes;
        LatestSchema = latestSchema;
    }
}

public class TimelineBuilder<TFormat>
{
    private readonly List<IDataFix<TFormat>> _completedFixes = new();
    private Dictionary<string, ISchemaType> _currentFields = new();

    private TimelineBuilder() { }

    public static TimelineBuilder<TFormat> Create() => new TimelineBuilder<TFormat>();

    public TimelineBuilder<TFormat> BaseSchema(Dictionary<string, ISchemaType> initialFields)
    {
        _currentFields = new(initialFields);
        return this;
    }

    internal TimelineBuilder<TFormat> AddVersion(
        IDataFix<TFormat> fix,
        Dictionary<string, ISchemaType> resultingFields
    )
    {
        _completedFixes.Add(fix);
        _currentFields = resultingFields;
        return this;
    }

    public VersionStepBuilder<TFormat> SinceVersion(Version version)
    {
        var schemaSnapshot = new RecordSchema(new Dictionary<string, ISchemaType>(_currentFields));
        var mutableFields = new Dictionary<string, ISchemaType>(_currentFields);
        return new VersionStepBuilder<TFormat>(version, mutableFields, this, schemaSnapshot);
    }

    public Timeline<T, TFormat> Build<T>() =>
        new Timeline<T, TFormat>(_completedFixes, new RecordSchema(_currentFields));
}
