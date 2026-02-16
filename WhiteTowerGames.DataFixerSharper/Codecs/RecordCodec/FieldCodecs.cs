using WhiteTowerGames.DataFixerSharper.Abstractions;

namespace WhiteTowerGames.DataFixerSharper.Codecs.RecordCodec;

public static class CodecFieldExtensions
{
    public static FieldCodec<T, TField> Field<T, TField>(
        this Codec<TField> codec,
        Func<T, TField> getter,
        string name
    ) => new FieldCodec<T, TField>(codec, name, getter);

    public static OptionalFieldCodec<T, TField> OptionalField<T, TField>(
        this Codec<TField> codec,
        Func<T, TField> getter,
        string name,
        TField defaultValue
    ) => new OptionalFieldCodec<T, TField>(codec, name, getter, defaultValue);
}

public interface IFieldCodec<T, TField>
{
    DataResult<TFormat> Encode<TFormat>(T input, IDynamicOps<TFormat> ops, TFormat prefix);
    DataResult<(TField, TFormat)> Decode<TFormat>(IDynamicOps<TFormat> ops, TFormat input);
}

public class FieldCodec<T, TField> : IFieldCodec<T, TField>
{
    private readonly Codec<TField> _codec;
    private readonly string _name;
    private readonly Func<T, TField> _getter;
    private Dictionary<Type, object> _cachedFieldNames = new(); // we cache the representation of the field name per format to avoid having to encode it every time

    public FieldCodec(Codec<TField> codec, string name, Func<T, TField> getter)
    {
        _codec = codec;
        _name = name;
        _getter = getter;
    }

    public DataResult<(TField, TFormat)> Decode<TFormat>(IDynamicOps<TFormat> ops, TFormat input)
    {
        var key = _cachedFieldNames.TryGetValue(typeof(TFormat), out var formatted)
            ? (TFormat)formatted
            : CacheName(ops);

        var fieldResult = ops.GetValue(input, _name);
        if (fieldResult.IsError)
            return DataResult<(TField, TFormat)>.Fail(fieldResult.ErrorMessage);

        var parsed = _codec.Parse(ops, fieldResult.GetOrThrow());
        if (parsed.IsError)
            return DataResult<(TField, TFormat)>.Fail(parsed.ErrorMessage);

        return parsed.Map(decoded => (decoded, ops.RemoveFromInput(input, key)));
    }

    public DataResult<TFormat> Encode<TFormat>(T input, IDynamicOps<TFormat> ops, TFormat prefix)
    {
        var key = _cachedFieldNames.TryGetValue(typeof(TFormat), out var formatted)
            ? (TFormat)formatted
            : CacheName(ops);

        var fieldValue = _getter(input);
        var encoded = _codec.EncodeStart(ops, fieldValue);
        return builder.Add(key, encoded);
    }

    private TFormat CacheName<TFormat>(IDynamicOps<TFormat> ops)
    {
        var value = ops.CreateString(_name)!;
        _cachedFieldNames[typeof(TFormat)] = value;
        return value;
    }
}

public class OptionalFieldCodec<T, TField> : IFieldCodec<T, TField>
{
    private readonly Codec<TField> _codec;
    private readonly string _name;
    private readonly Func<T, TField> _getter;
    private readonly TField _defaultValue;
    private Dictionary<Type, object> _cachedFieldNames = new();

    public OptionalFieldCodec(
        Codec<TField> codec,
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

    public DataResult<(TField, TFormat)> Decode<TFormat>(IDynamicOps<TFormat> ops, TFormat input)
    {
        var key = _cachedFieldNames.TryGetValue(typeof(TFormat), out var formatted)
            ? (TFormat)formatted
            : CacheName(ops);

        var fieldResult = ops.GetValue(input, _name);
        if (fieldResult.IsError)
            return DataResult<(TField, TFormat)>.Success((_defaultValue, input));

        var parsed = _codec.Parse(ops, fieldResult.GetOrThrow());
        if (parsed.IsError)
            return DataResult<(TField, TFormat)>.Fail(parsed.ErrorMessage);

        return parsed.Map(decoded => (decoded, ops.RemoveFromInput(input, key)));
    }

    public DataResult<TFormat> Encode<TFormat>(T input, IDynamicOps<TFormat> ops, TFormat prefix)
    {
        var key = _cachedFieldNames.TryGetValue(typeof(TFormat), out var formatted)
            ? (TFormat)formatted
            : CacheName(ops);

        var fieldValue = _getter(input);

        var valueResult = _codec.EncodeStart(ops, fieldValue);

        return builder.Add(key, valueResult);
    }

    private TFormat CacheName<TFormat>(IDynamicOps<TFormat> ops)
    {
        var value = ops.CreateString(_name)!;
        _cachedFieldNames[typeof(TFormat)] = value;
        return value;
    }
}
