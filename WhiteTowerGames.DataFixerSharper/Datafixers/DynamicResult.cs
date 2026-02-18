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

    public Dynamic<TFormat> GetOrElse(Dynamic<TFormat> defaultValue) =>
        !IsError ? Data : defaultValue;

    public Dynamic<TFormat> GetOrThrow() =>
        !IsError ? Data : throw new InvalidOperationException(ErrorMessage);

    public DynamicResult<TFormat> Map(Func<Dynamic<TFormat>, Dynamic<TFormat>> mapper) =>
        _result.Map<Dynamic<TFormat>>(mapper);

    public static implicit operator DynamicResult<TFormat>(DataResult<Dynamic<TFormat>> result) =>
        new DynamicResult<TFormat>(result);

    public static implicit operator DynamicResult<TFormat>(Dynamic<TFormat> data) => Success(data);

    public bool IsError => _result.IsError;
    public string ErrorMessage => _result.ErrorMessage;
    private Dynamic<TFormat> Data => _result.GetOrThrow();

    private static DynamicResult<TFormat> Fail(string errorMessage) =>
        new DynamicResult<TFormat>(DataResult<Dynamic<TFormat>>.Fail(errorMessage));

    private static DynamicResult<TFormat> Success(Dynamic<TFormat> result) =>
        new DynamicResult<TFormat>(DataResult<Dynamic<TFormat>>.Success(result));
}
