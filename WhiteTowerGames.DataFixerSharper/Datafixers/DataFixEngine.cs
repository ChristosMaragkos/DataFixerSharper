using WhiteTowerGames.DataFixerSharper.Abstractions;
using WhiteTowerGames.DataFixerSharper.Versioning;

namespace WhiteTowerGames.DataFixerSharper.Datafixers;

public sealed class DataFixEngine<TFormat>
{
    private readonly IDynamicOps<TFormat> _ops;
    private readonly Dictionary<
        Type,
        SortedDictionary<Version, List<IDataFix<TFormat>>>
    > _timelines = new();

    public DataFixEngine(IDynamicOps<TFormat> ops)
    {
        _ops = ops;
    }

    public void RegisterTimeline<TObj>(Timeline<TObj, TFormat> timeline)
    {
        var objectType = typeof(TObj);

        if (!_timelines.ContainsKey(objectType))
            _timelines[objectType] = new();

        foreach (var fix in timeline.Fixes)
        {
            if (!_timelines[objectType].ContainsKey(fix.Since))
                _timelines[objectType][fix.Since] = new();

            _timelines[objectType][fix.Since].Add(fix);
        }
    }

    public DataResult<TFormat> Migrate<TObj>(Version fromVersion, Version toVersion, TFormat data)
    {
        var objectType = typeof(TObj);
        if (!_timelines.TryGetValue(objectType, out var typeFixes))
            return DataResult<TFormat>.Success(data);

        var migrating = new Dynamic<TFormat>(_ops, data);
        foreach (var (version, fixes) in typeFixes)
        {
            if (version <= fromVersion)
                continue;

            if (version > toVersion)
                break;

            foreach (var fix in fixes)
            {
                var applied = fix.Apply(migrating);
                if (applied.IsError)
                    return DataResult<TFormat>.Fail(
                        $"Could not migrate data to version {version}: {applied.ErrorMessage}"
                    );

                migrating = applied.GetOrThrow();
            }
        }

        return DataResult<TFormat>.Success(migrating.Value);
    }
}
