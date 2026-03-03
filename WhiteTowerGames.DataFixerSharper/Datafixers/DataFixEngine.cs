using WhiteTowerGames.DataFixerSharper.Abstractions;

namespace WhiteTowerGames.DataFixerSharper.Datafixers;

public sealed class DataFixEngine<TFormat>
{
    private readonly IDynamicOps<TFormat> _ops;
    private readonly SortedDictionary<Version, List<IDataFix<TFormat>>> _fixes = new();

    public DataFixEngine(IDynamicOps<TFormat> ops)
    {
        _ops = ops;
    }

    public void RegisterDatafix(IDataFix<TFormat> fix)
    {
        if (!_fixes.ContainsKey(fix.Since))
            _fixes[fix.Since] = new List<IDataFix<TFormat>>();

        _fixes[fix.Since].Add(fix);
    }

    public DataResult<TFormat> Migrate(Version fromVersion, Version toVersion, TFormat data)
    {
        var migrating = new Dynamic<TFormat>(_ops, data);
        foreach (var (version, fixes) in _fixes)
        {
            if (version < fromVersion)
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
