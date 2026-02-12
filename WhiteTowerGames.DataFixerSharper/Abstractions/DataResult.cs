namespace WhiteTowerGames.DataFixerSharper.Abstractions;

public abstract class DataResult<T>
{
    public static DataResult<T> Success(T value) => new SuccessResult(value);

    public static DataResult<T> Fail(string message, T? partialResult = default)
    {
        if (partialResult == null)
            return new ErrorResult(message);

        return new ErrorResult(message, partialResult);
    }

    /// <summary>
    /// Returns the result on success, throws an InvalidOperationException on fail.
    /// </summary>
    public abstract T GetOrThrow();

    /// <summary>
    /// Returns the result on success, or a default value on fail.
    /// </summary>
    public abstract T GetOrDefault(T defaultValue);

    public abstract T ResultOrPartial();

    /// <summary>
    /// Returns the result on success, consumes an error message and returns null on fail
    /// </summary>
    public abstract T? ResultOrConsume(Action<string> onError);

    /// <summary>
    /// Applies a function on the result on success or a function on this instance of DataResult on fail.
    /// </summary>
    public abstract DataResult<T> Evaluate<TResult>(
        Action<T> successFunc,
        Action<DataResult<T>> errorFunc
    );

    public abstract string ErrorMessage { get; }
    public abstract bool IsSuccess { get; }
    public abstract bool IsError { get; }

    /// <summary>
    /// Maps the value of this DataResult to another type
    /// </summary>
    public abstract DataResult<TOther> Map<TOther>(Func<T, TOther> converter);

    /// <summary>
    /// Maps the value of this DataResult to a DataResult of another type
    /// </summary>
    public abstract DataResult<TOther> UnsafeMap<TOther>(Func<T, DataResult<TOther>> converter);

    private class SuccessResult : DataResult<T>
    {
        private readonly T _value;

        public SuccessResult(T value)
        {
            _value = value;
        }

        public override bool IsSuccess => true;

        public override bool IsError => false;

        public override string ErrorMessage => "Success!";

        public override DataResult<T> Evaluate<TResult>(
            Action<T> successFunc,
            Action<DataResult<T>> errorFunc
        )
        {
            errorFunc(this);
            return this;
        }

        public override T GetOrDefault(T defaultValue) => _value;

        public override T GetOrThrow() => _value;

        public override T ResultOrPartial() => _value;

        public override DataResult<TOther> Map<TOther>(Func<T, TOther> converter) =>
            DataResult<TOther>.Success(converter(_value));

        public override T? ResultOrConsume(Action<string> onError) => _value;

        public override string ToString() => $"Success[{_value}]";

        public override DataResult<TOther> UnsafeMap<TOther>(Func<T, DataResult<TOther>> converter)
        {
            return converter(_value);
        }
    }

    private class ErrorResult : DataResult<T>
    {
        private readonly string _message;
        private readonly T? _partialResult;
        private readonly bool _hasPartial;

        public ErrorResult(string message)
        {
            _message = message;
            _hasPartial = false;
            _partialResult = default;
        }

        public ErrorResult(string message, T partialResult)
        {
            _message = message;
            _hasPartial = true;
            _partialResult = partialResult;
        }

        public override string ErrorMessage => _message;

        public override bool IsSuccess => false;

        public override bool IsError => true;

        public override DataResult<T> Evaluate<TResult>(
            Action<T> successFunc,
            Action<DataResult<T>> errorFunc
        )
        {
            errorFunc(this);
            return this;
        }

        public override T GetOrDefault(T defaultValue) => defaultValue;

        public override T ResultOrPartial() => _hasPartial ? _partialResult! : default!;

        public override T GetOrThrow() =>
            throw new InvalidOperationException($"DataResult error: {_message}");

        public override DataResult<TOther> Map<TOther>(Func<T, TOther> converter)
        {
            if (_hasPartial)
                return DataResult<TOther>.Fail(_message, converter(_partialResult!));

            return DataResult<TOther>.Fail(_message);
        }

        public override T? ResultOrConsume(Action<string> onError)
        {
            onError(_message);
            return default;
        }

        public override string ToString() => $"Error[{_message}]";

        public override DataResult<TOther> UnsafeMap<TOther>(
            Func<T, DataResult<TOther>> converter
        ) => DataResult<TOther>.Fail(_message);
    }
}
