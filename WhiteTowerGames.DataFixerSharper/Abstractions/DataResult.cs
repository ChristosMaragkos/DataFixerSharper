namespace WhiteTowerGames.DataFixerSharper.Abstractions;

public readonly struct DataResult<T>
{
    private readonly T _value;
    private readonly string _errorMessage;
    private readonly bool _isError;

    public bool IsError => _isError;

    public T GetOrThrow() => !_isError ? _value : throw new InvalidOperationException(ErrorMessage);

    public T ResultOrPartial() => GetOrThrow();

    public readonly string ErrorMessage =>
        _isError
            ? $"DataResult[{typeof(T)}] Fail: {_errorMessage}"
            : $"DataResult[{typeof(T)}] Success";

    public static DataResult<T> Success(T value) => new(value, null, false);

    public static DataResult<T> Fail(string errorMessage) => new(default!, errorMessage, true);

    public DataResult(T value, string? errorMessage, bool isError)
    {
        _value = value;
        _errorMessage = errorMessage ?? "";
        _isError = isError;
    }

    public DataResult<TOther> Map<TOther>(Func<T, TOther> mapper)
    {
        if (IsError)
            return DataResult<TOther>.Fail(_errorMessage);

        return DataResult<TOther>.Success(mapper(_value));
    }

    public DataResult<TOther> UnsafeMap<TOther>(Func<T, DataResult<TOther>> mapper)
    {
        var mapped = mapper(_value);
        if (IsError)
            return DataResult<TOther>.Fail(_errorMessage);
        else if (mapped.IsError)
            return DataResult<TOther>.Fail(mapped.ErrorMessage);

        return DataResult<TOther>.Success(mapped.GetOrThrow());
    }
}
