using WhiteTowerGames.DataFixerSharper.Abstractions;

namespace WhiteTowerGames.DataFixerSharper.Datafixers;

public readonly struct DynamicResult<TFormat>
{
    private readonly DataResult<Dynamic<TFormat>> _result;
    public bool IsError => _result.IsError;
    public string ErrorMessage => _result.ErrorMessage;
    private Dynamic<TFormat> Data => _result.GetOrThrow();

    internal DynamicResult(DataResult<Dynamic<TFormat>> result)
    {
        _result = result;
    }

    /// <summary>
    /// Gets the value under <c>key</c> in the given object
    /// </summary>
    public DynamicResult<TFormat> Get(string key)
    {
        if (IsError)
            return this;

        return Data.Get(key);
    }

    /// <summary>
    /// Sets the value under <c>key</c> in the given object to a value. Fails entirely if the value is invalid.
    /// </summary>
    public DynamicResult<TFormat> Set(string key, TFormat value)
    {
        if (IsError)
            return this;
        return Data.Set(key, value);
    }

    /// <summary>
    /// Sets the value under <c>key</c> in the given object to a value. Simply skips if the value is invalid.
    /// </summary>
    [Obsolete("Use Set instead", true)]
    public DynamicResult<TFormat> SetOptional(string key, TFormat value)
    {
        return Set(key, value);
    }

    public DynamicResult<TFormat> Rename(string oldKey, string newKey)
    {
        if (IsError)
            return this;

        return Data.Rename(oldKey, newKey);
    }

    /// <summary>
    /// Iterates over a list applying the updater function to each element. Then, returns a new list.
    /// Fails if the current value is not a list or if any element fails to update.
    /// </summary>
    public DynamicResult<TFormat> UpdateList(
        Func<Dynamic<TFormat>, DynamicResult<TFormat>> itemUpdater
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
    public DynamicResult<TFormat> UpdateMap(
        Func<string, Dynamic<TFormat>, DynamicResult<TFormat>> fieldUpdater
    )
    {
        if (IsError)
            return this;
        return Data.UpdateMap(fieldUpdater);
    }

    public DynamicResult<TFormat> Map(Func<Dynamic<TFormat>, DynamicResult<TFormat>> mapper) =>
        _result.Map(mapper).GetOrElse(this);

    public DynamicResult<TFormat> UnsafeMap(Func<Dynamic<TFormat>, DynamicResult<TFormat>> mapper)
    {
        if (IsError)
            return this;
        return mapper(Data);
    }

    public Dynamic<TFormat> GetOrElse(Dynamic<TFormat> defaultValue) =>
        !IsError ? Data : defaultValue;

    public Dynamic<TFormat> GetOrThrow() =>
        !IsError ? Data : throw new InvalidOperationException(ErrorMessage);

    public static implicit operator DynamicResult<TFormat>(DataResult<Dynamic<TFormat>> result) =>
        new(result);

    public static implicit operator DynamicResult<TFormat>(Dynamic<TFormat> data) => Success(data);

    public static implicit operator Dynamic<TFormat>(DynamicResult<TFormat> result) =>
        result._result.GetOrThrow();

    private static DynamicResult<TFormat> Success(Dynamic<TFormat> result) =>
        new(DataResult<Dynamic<TFormat>>.Success(result));
}
