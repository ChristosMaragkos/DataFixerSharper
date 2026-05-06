using WhiteTowerGames.DataFixerSharper.Abstractions;
using WhiteTowerGames.DataFixerSharper.Datafixers;
using WhiteTowerGames.DataFixerSharper.Schemas;

namespace WhiteTowerGames.DataFixerSharper.Versioning;

public class VersionStepBuilder<TFormat>
{
    private readonly Version _sinceVersion;
    private readonly Dictionary<string, ISchemaType> _fields;
    private readonly List<Func<Dynamic<TFormat>, DynamicResult<TFormat>>> _rules = [];
    private readonly TimelineBuilder<TFormat> _outerBuilder;
    private readonly RecordSchema _inputSchema;

    internal VersionStepBuilder(
        Version sinceVersion,
        Dictionary<string, ISchemaType> currentFields,
        TimelineBuilder<TFormat> outerBuilder,
        RecordSchema inputSchema
    )
    {
        _sinceVersion = sinceVersion;
        _fields = currentFields;
        _outerBuilder = outerBuilder;
        _inputSchema = inputSchema;
    }

    public VersionStepBuilder<TFormat> FieldAdded(
        string fieldName,
        ISchemaType fieldSchema,
        string defaultValue
    )
    {
        _fields[fieldName] = fieldSchema;
        _rules.Add(dyn => dyn.Set(fieldName, dyn.Ops.CreateString(defaultValue)));
        return this;
    }

    public VersionStepBuilder<TFormat> FieldAdded(
        string fieldName,
        ISchemaType fieldSchema,
        decimal defaultValue
    )
    {
        _fields[fieldName] = fieldSchema;
        _rules.Add(dyn => dyn.Set(fieldName, dyn.Ops.CreateNumeric(defaultValue)));
        return this;
    }

    public VersionStepBuilder<TFormat> FieldAdded(
        string fieldName,
        ISchemaType fieldSchema,
        bool defaultValue
    )
    {
        _fields[fieldName] = fieldSchema;
        _rules.Add(dyn => dyn.Set(fieldName, dyn.Ops.CreateBool(defaultValue)));
        return this;
    }

    public VersionStepBuilder<TFormat> FieldAdded(
        string fieldName,
        ISchemaType fieldSchema,
        Func<IDynamicOps<TFormat>, TFormat> defaultValueFactory
    )
    {
        _fields[fieldName] = fieldSchema;
        _rules.Add(dyn =>
        {
            var newValue = defaultValueFactory(dyn.Ops);
            return dyn.Set(fieldName, newValue);
        });
        return this;
    }

    public VersionStepBuilder<TFormat> FieldRemoved(string fieldName)
    {
        _fields.Remove(fieldName);
        _rules.Add(dyn => dyn.Remove(fieldName));
        return this;
    }

    public VersionStepBuilder<TFormat> FieldRenamed(string oldName, string newName)
    {
        _fields[newName] = _fields[oldName];
        _fields.Remove(oldName);
        _rules.Add(dyn => dyn.Rename(oldName, newName));
        return this;
    }

    public VersionStepBuilder<TFormat> CustomRule(
        Func<Dynamic<TFormat>, DynamicResult<TFormat>> rule
    )
    {
        _rules.Add(rule);
        return this;
    }

    public TimelineBuilder<TFormat> EndVersion()
    {
        Func<Dynamic<TFormat>, DynamicResult<TFormat>> pipeline = input =>
        {
            DynamicResult<TFormat> currentData = DataResult<Dynamic<TFormat>>.Success(input);

            foreach (var rule in _rules)
            {
                currentData = currentData.UnsafeMap(rule);
                if (currentData.IsError)
                    return currentData;
            }

            return currentData;
        };

        return _outerBuilder.AddVersion(
            new SchemaDrivenFix<TFormat>(_sinceVersion, _inputSchema, pipeline),
            _fields
        );
    }
}
