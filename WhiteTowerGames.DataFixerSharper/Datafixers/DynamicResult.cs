using WhiteTowerGames.DataFixerSharper.Abstractions;

namespace WhiteTowerGames.DataFixerSharper.Datafixers;

public readonly struct DynamicResult<TOps, TFormat>
    where TOps : IDynamicOps<TFormat>
    where TFormat : struct
{
    private readonly DataResult<Dynamic<TOps, TFormat>> _result;
    public bool IsError => _result.IsError;
    public string ErrorMessage => _result.ErrorMessage;
    private Dynamic<TOps, TFormat> Data => _result.GetOrThrow();

    internal DynamicResult(DataResult<Dynamic<TOps, TFormat>> result)
    {
        _result = result;
    }

    /// <summary>
    /// Gets the value under <c>key</c> in the given object
    /// </summary>
    public DynamicResult<TOps, TFormat> Get(string key)
    {
        if (IsError)
            return this;

        return Data.Get(key);
    }

    /// <summary>
    /// Sets the value under <c>key</c> in the given object to a value. Fails entirely if the value is invalid.
    /// </summary>
    public DynamicResult<TOps, TFormat> Set(string key, TFormat value)
    {
        if (IsError)
            return this;
        return Data.Set(key, value);
    }

    /// <summary>
    /// Sets the value under <c>key</c> in the given object to a value. Simply skips if the value is invalid.
    /// </summary>
    [Obsolete("Use Set instead", true)]
    public DynamicResult<TOps, TFormat> SetOptional(string key, TFormat value)
    {
        return Set(key, value);
    }

    public DynamicResult<TOps, TFormat> Rename(string oldKey, string newKey)
    {
        if (IsError)
            return this;

        return Data.Rename(oldKey, newKey);
    }

    /// <summary>
    /// Iterates over a list applying the updater function to each element. Then, returns a new list.
    /// Fails if the current value is not a list or if any element fails to update.
    /// </summary>
    public DynamicResult<TOps, TFormat> UpdateList(
        Func<Dynamic<TOps, TFormat>, DynamicResult<TOps, TFormat>> itemUpdater
    )
    {
        if (IsError)
            return this;

        return Data.UpdateList(itemUpdater);
    }

    /// <summary>
    /// Iterates over a map applying the updater to each key-value pair. Then, returns a new map.
    /// Fails if the current value is not a valid map or any key-value pair fails to update properly.
    /// </summary>
    public DynamicResult<TOps, TFormat> UpdateMap(
        Func<string, Dynamic<TOps, TFormat>, DynamicResult<TOps, TFormat>> fieldUpdater
    )
    {
        if (IsError)
            return this;
        return Data.UpdateMap(fieldUpdater);
    }

    public DynamicResult<TOps, TFormat> Map(Func<Dynamic<TOps, TFormat>, DynamicResult<TOps, TFormat>> mapper) =>
        _result.Map(mapper).GetOrElse(this);

    public DynamicResult<TOps, TFormat> UnsafeMap(Func<Dynamic<TOps, TFormat>, DynamicResult<TOps, TFormat>> mapper)
    {
        if (IsError)
            return this;
        return mapper(Data);
    }

    public Dynamic<TOps, TFormat> GetOrElse(Dynamic<TOps, TFormat> defaultValue) =>
        !IsError ? Data : defaultValue;

    public Dynamic<TOps, TFormat> GetOrThrow() =>
        !IsError ? Data : throw new InvalidOperationException(ErrorMessage);

    public static implicit operator DynamicResult<TOps, TFormat>(DataResult<Dynamic<TOps, TFormat>> result) =>
        new(result);

    public static implicit operator DynamicResult<TOps, TFormat>(Dynamic<TOps, TFormat> data) => Success(data);

    public static implicit operator Dynamic<TOps, TFormat>(DynamicResult<TOps, TFormat> result) =>
        result._result.GetOrThrow();

    private static DynamicResult<TOps, TFormat> Success(Dynamic<TOps, TFormat> result) =>
        new(DataResult<Dynamic<TOps, TFormat>>.Success(result));
}
