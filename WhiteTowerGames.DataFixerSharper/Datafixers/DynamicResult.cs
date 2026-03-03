using WhiteTowerGames.DataFixerSharper.Abstractions;

namespace WhiteTowerGames.DataFixerSharper.Datafixers;

public readonly struct DynamicResult<TFormat>
{
    private readonly DataResult<Dynamic<TFormat>> _result;

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
    public DynamicResult<TFormat> Set(string key, DynamicResult<TFormat> valueResult)
    {
        if (IsError)
            return this;
        if (valueResult.IsError)
            return valueResult;

        return Data.Set(key, valueResult.Data);
    }

    /// <summary>
    /// Sets the value under <c>key</c> in the given object to a value. Simply skips if the value is invalid.
    /// </summary>
    public DynamicResult<TFormat> SetOptional(string key, DynamicResult<TFormat> valueResult)
    {
        if (IsError || valueResult.IsError)
            return this;

        return Data.Set(key, valueResult.Data);
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

    public DataResult<Dynamic<TFormat>> GetOrElse(Dynamic<TFormat> defaultValue) =>
        !IsError
            ? DataResult<Dynamic<TFormat>>.Success(Data)
            : DataResult<Dynamic<TFormat>>.Success(defaultValue);

    public Dynamic<TFormat> GetOrThrow() =>
        !IsError ? Data : throw new InvalidOperationException(ErrorMessage);

    public static implicit operator DynamicResult<TFormat>(DataResult<Dynamic<TFormat>> result) =>
        new DynamicResult<TFormat>(result);

    public static implicit operator DynamicResult<TFormat>(Dynamic<TFormat> data) => Success(data);

    public bool IsError => _result.IsError;
    public string ErrorMessage => _result.ErrorMessage;
    private Dynamic<TFormat> Data => _result.GetOrThrow();

    private static DynamicResult<TFormat> Success(Dynamic<TFormat> result) =>
        new DynamicResult<TFormat>(DataResult<Dynamic<TFormat>>.Success(result));
}
