using WhiteTowerGames.DataFixerSharper.Abstractions;
using WhiteTowerGames.DataFixerSharper.Datafixers;
using WhiteTowerGames.DataFixerSharper.Schemas;

namespace WhiteTowerGames.DataFixerSharper.Versioning;

public class VersionStepBuilder<TOps, TFormat>
    where TOps : IDynamicOps<TFormat>
    where TFormat : struct
{
    private readonly Version _sinceVersion;
    private readonly Dictionary<string, ISchemaType> _fields;
    private readonly List<Func<Dynamic<TOps, TFormat>, DynamicResult<TOps, TFormat>>> _rules = [];
    private readonly TimelineBuilder<TOps, TFormat> _outerBuilder;
    private readonly RecordSchema _inputSchema;

    internal VersionStepBuilder(
        Version sinceVersion,
        Dictionary<string, ISchemaType> currentFields,
        TimelineBuilder<TOps, TFormat> outerBuilder,
        RecordSchema inputSchema
    )
    {
        _sinceVersion = sinceVersion;
        _fields = currentFields;
        _outerBuilder = outerBuilder;
        _inputSchema = inputSchema;
    }

    public VersionStepBuilder<TOps, TFormat> FieldAdded(
        string fieldName,
        ISchemaType fieldSchema,
        string defaultValue
    )
    {
        _fields[fieldName] = fieldSchema;
        _rules.Add(dyn => dyn.Set(fieldName, TOps.CreateString(defaultValue)));
        return this;
    }

    public VersionStepBuilder<TOps, TFormat> FieldAdded(
        string fieldName,
        ISchemaType fieldSchema,
        decimal defaultValue
    )
    {
        _fields[fieldName] = fieldSchema;
        _rules.Add(dyn => dyn.Set(fieldName, TOps.CreateNumeric(defaultValue)));
        return this;
    }

    public VersionStepBuilder<TOps, TFormat> FieldAdded(
        string fieldName,
        ISchemaType fieldSchema,
        bool defaultValue
    )
    {
        _fields[fieldName] = fieldSchema;
        _rules.Add(dyn => dyn.Set(fieldName, TOps.CreateBool(defaultValue)));
        return this;
    }

    public VersionStepBuilder<TOps, TFormat> FieldAdded(
        string fieldName,
        ISchemaType fieldSchema,
        Func<TFormat> defaultValueFactory
    )
    {
        _fields[fieldName] = fieldSchema;
        _rules.Add(dyn =>
        {
            var newValue = defaultValueFactory();
            return dyn.Set(fieldName, newValue);
        });
        return this;
    }

    public VersionStepBuilder<TOps, TFormat> FieldRemoved(string fieldName)
    {
        _fields.Remove(fieldName);
        _rules.Add(dyn => dyn.Remove(fieldName));
        return this;
    }

    public VersionStepBuilder<TOps, TFormat> FieldRenamed(string oldName, string newName)
    {
        _fields[newName] = _fields[oldName];
        _fields.Remove(oldName);
        _rules.Add(dyn => dyn.Rename(oldName, newName));
        return this;
    }

    public VersionStepBuilder<TOps, TFormat> CustomRule(
        Func<Dynamic<TOps, TFormat>, DynamicResult<TOps, TFormat>> rule
    )
    {
        _rules.Add(rule);
        return this;
    }

    public TimelineBuilder<TOps, TFormat> EndVersion()
    {
        Func<Dynamic<TOps, TFormat>, DynamicResult<TOps, TFormat>> pipeline = input =>
        {
            DynamicResult<TOps, TFormat> currentData = DataResult<Dynamic<TOps, TFormat>>.Success(input);

            foreach (var rule in _rules)
            {
                currentData = currentData.UnsafeMap(rule);
                if (currentData.IsError)
                    return currentData;
            }

            return currentData;
        };

        return _outerBuilder.AddVersion(
            new SchemaDrivenFix<TOps, TFormat>(_sinceVersion, _inputSchema, pipeline),
            _fields
        );
    }
}
