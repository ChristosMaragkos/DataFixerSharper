using WhiteTowerGames.DataFixerSharper.Abstractions;

namespace WhiteTowerGames.DataFixerSharper.Datafixers;

public static class DataFixer
{
    private static readonly SortedDictionary<Version, List<IDataFix>> DataFixes = new();

    public static void RegisterDatafix(IDataFix fix)
    {
        DataFixes.TryAdd(fix.Since, new());
        DataFixes[fix.Since].Add(fix);
    }

    public static DataResult<TFormat> Migrate<TOps, TFormat>(
        Version fromVersion,
        Version toVersion,
        TOps ops,
        TFormat data
    )
        where TOps : IDynamicOps<TFormat>
    {
        var migrating = new Dynamic<TFormat>(ops, data);
        foreach (var (version, fixes) in DataFixes)
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
