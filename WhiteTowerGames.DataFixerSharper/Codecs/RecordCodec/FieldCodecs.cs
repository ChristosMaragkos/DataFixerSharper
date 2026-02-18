using WhiteTowerGames.DataFixerSharper.Abstractions;

namespace WhiteTowerGames.DataFixerSharper.Codecs.RecordCodec;

public static class CodecFieldExtensions
{
    public static FieldCodec<T, TField> Field<T, TField>(
        this ICodec<TField> codec,
        Func<T, TField> getter,
        string name
    ) => new FieldCodec<T, TField>(codec, name, getter);

    public static OptionalFieldCodec<T, TField> OptionalField<T, TField>(
        this ICodec<TField> codec,
        Func<T, TField> getter,
        string name,
        TField defaultValue
    ) => new OptionalFieldCodec<T, TField>(codec, name, getter, defaultValue);
}

public interface IFieldCodec<T, TField>
{
    DataResult<TFormat> Encode<TOps, TFormat>(T input, TOps ops, TFormat accumulator)
        where TOps : IDynamicOps<TFormat>;
    DataResult<(TField, TFormat)> Decode<TOps, TFormat>(TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat>;
}

public class FieldCodec<T, TField> : IFieldCodec<T, TField>
{
    private readonly ICodec<TField> _codec;
    private readonly string _name;
    private readonly Func<T, TField> _getter;
    private readonly Dictionary<Type, object> _keyCache = new();

    public FieldCodec(ICodec<TField> codec, string name, Func<T, TField> getter)
    {
        _codec = codec;
        _name = name;
        _getter = getter;
    }

    public DataResult<(TField, TFormat)> Decode<TOps, TFormat>(TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat>
    {
        var fetchedValue = ops.GetValue(input, _name);
        if (fetchedValue.IsError)
            return DataResult<(TField, TFormat)>.Fail(fetchedValue.ErrorMessage);

        var value = _codec.Parse(ops, fetchedValue.GetOrThrow());
        if (value.IsError)
            return DataResult<(TField, TFormat)>.Fail(value.ErrorMessage);

        input = ops.RemoveFromInput(input, _name);
        return DataResult<(TField, TFormat)>.Success((value.GetOrThrow(), input));
    }

    public DataResult<TFormat> Encode<TOps, TFormat>(T input, TOps ops, TFormat accumulator)
        where TOps : IDynamicOps<TFormat>
    {
        var value = _getter(input);
        var encodedValue = _codec.EncodeStart<TOps, TFormat>(ops, value);

        if (encodedValue.IsError)
            return encodedValue;

        var key = _keyCache.TryGetValue(typeof(TFormat), out var cached)
            ? (TFormat)cached
            : CacheKey<TOps, TFormat>(ops);
        return ops.AddToMap(accumulator, key, encodedValue.GetOrThrow());
    }

    private TFormat CacheKey<TOps, TFormat>(TOps ops)
        where TOps : IDynamicOps<TFormat>
    {
        var converted = ops.CreateString(_name)!;
        _keyCache[typeof(TFormat)] = converted;
        return converted;
    }
}

public class OptionalFieldCodec<T, TField> : IFieldCodec<T, TField>
{
    private readonly ICodec<TField> _codec;
    private readonly string _name;
    private readonly Func<T, TField> _getter;
    private readonly TField _defaultValue;
    private readonly Dictionary<Type, object> _keyCache = new();

    public OptionalFieldCodec(
        ICodec<TField> codec,
        string name,
        Func<T, TField> getter,
        TField defaultValue
    )
    {
        _codec = codec;
        _name = name;
        _getter = getter;
        _defaultValue = defaultValue;
    }

    public DataResult<(TField, TFormat)> Decode<TOps, TFormat>(TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat>
    {
        var fetchedValue = ops.GetValue(input, _name);
        if (fetchedValue.IsError) // if the value was not present
            return DataResult<(TField, TFormat)>.Success((_defaultValue, input));

        var value = _codec.Parse(ops, fetchedValue.GetOrThrow());
        if (value.IsError) // if the value was found, but malformed
            return DataResult<(TField, TFormat)>.Fail(value.ErrorMessage);

        input = ops.RemoveFromInput(input, _name);
        return DataResult<(TField, TFormat)>.Success((value.GetOrThrow(), input));
    }

    public DataResult<TFormat> Encode<TOps, TFormat>(T input, TOps ops, TFormat accumulator)
        where TOps : IDynamicOps<TFormat>
    {
        var value = _getter(input);

        if (EqualityComparer<TField>.Default.Equals(value, _defaultValue))
            return DataResult<TFormat>.Success(accumulator); // don't encode the default value

        var encodedValue = _codec.EncodeStart<TOps, TFormat>(ops, value);
        if (encodedValue.IsError)
            return encodedValue;

        var key = _keyCache.TryGetValue(typeof(TFormat), out var cached)
            ? (TFormat)cached
            : CacheKey<TOps, TFormat>(ops);
        return ops.AddToMap(accumulator, key, encodedValue.GetOrThrow());
    }

    private TFormat CacheKey<TOps, TFormat>(TOps ops)
        where TOps : IDynamicOps<TFormat>
    {
        var converted = ops.CreateString(_name)!;
        _keyCache[typeof(TFormat)] = converted;
        return converted;
    }
}
