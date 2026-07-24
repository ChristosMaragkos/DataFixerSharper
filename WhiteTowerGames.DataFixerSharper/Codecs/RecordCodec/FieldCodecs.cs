using System.Collections.Concurrent;
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
    DataResult<TFormat> Encode<TOps, TFormat>(T input, TFormat accumulator)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct;
    DataResult<(TField, TFormat)> Decode<TOps, TFormat>(TFormat input)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct;

    DataResult<TField> DecodeFromMap<TOps, TFormat>(ref FieldMap<TFormat> map)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct;

    internal static readonly ConcurrentDictionary<(Type, string), object> KeyCache = new();
}

public readonly struct FieldCodec<T, TField> : IFieldCodec<T, TField>
{
    private readonly ICodec<TField> _codec;
    private readonly string _name;
    private readonly Func<T, TField> _getter;

    public FieldCodec(ICodec<TField> codec, string name, Func<T, TField> getter)
    {
        _codec = codec;
        _name = name;
        _getter = getter;
    }

    public DataResult<(TField, TFormat)> Decode<TOps, TFormat>(TFormat input)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        var fetchedValue = TOps.GetValue(input, _name);
        if (fetchedValue.IsError)
            return DataResult<(TField, TFormat)>.Fail(fetchedValue.ErrorMessage);

        var value = _codec.Parse<TOps, TFormat>(fetchedValue.GetOrThrow());
        if (value.IsError)
            return DataResult<(TField, TFormat)>.Fail(value.ErrorMessage);

        input = TOps.RemoveFromInput(input, _name);
        return DataResult<(TField, TFormat)>.Success((value.GetOrThrow(), input));
    }

    public DataResult<TField> DecodeFromMap<TOps, TFormat>(ref FieldMap<TFormat> map)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        if (!map.TryGet<TOps>(_name, out var rawValue))
            return DataResult<TField>.Fail($"Missing required field: '{_name}'");

        return _codec.Parse<TOps, TFormat>(rawValue);
    }

    public DataResult<TFormat> Encode<TOps, TFormat>(T input, TFormat accumulator)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        var value = _getter(input);
        var key = IFieldCodec<T, TField>.KeyCache.TryGetValue(
            (typeof(TFormat), _name),
            out var cached
        )
            ? (TFormat)cached
            : CacheKey<TOps, TFormat>();

        TOps.WriteKey(accumulator, key);
        return _codec.Encode<TOps, TFormat>(value, accumulator);
    }

    private TFormat CacheKey<TOps, TFormat>()
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        var converted = TOps.CreateString(_name)!;
        IFieldCodec<T, TField>.KeyCache[(typeof(TFormat), _name)] = converted;
        return converted;
    }
}

public readonly struct OptionalFieldCodec<T, TField> : IFieldCodec<T, TField>
{
    private readonly ICodec<TField> _codec;
    private readonly string _name;
    private readonly Func<T, TField> _getter;
    private readonly TField _defaultValue;

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

    public DataResult<(TField, TFormat)> Decode<TOps, TFormat>(TFormat input)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        var fetchedValue = TOps.GetValue(input, _name);
        if (fetchedValue.IsError)
            return DataResult<(TField, TFormat)>.Success((_defaultValue, input));

        var value = _codec.Parse<TOps, TFormat>(fetchedValue.GetOrThrow());
        if (value.IsError)
            return DataResult<(TField, TFormat)>.Fail(value.ErrorMessage);

        input = TOps.RemoveFromInput(input, _name);
        return DataResult<(TField, TFormat)>.Success((value.GetOrThrow(), input));
    }

    public DataResult<TField> DecodeFromMap<TOps, TFormat>(ref FieldMap<TFormat> map)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        if (!map.TryGet<TOps>(_name, out var rawValue))
            return DataResult<TField>.Success(_defaultValue);

        return _codec.Parse<TOps, TFormat>(rawValue);
    }

    public DataResult<TFormat> Encode<TOps, TFormat>(T input, TFormat accumulator)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        var value = _getter(input);

        if (EqualityComparer<TField>.Default.Equals(value, _defaultValue))
            return DataResult<TFormat>.Success(accumulator);

        var key = IFieldCodec<T, TField>.KeyCache.TryGetValue(
            (typeof(TFormat), _name),
            out var cached
        )
            ? (TFormat)cached
            : CacheKey<TOps, TFormat>();

        TOps.WriteKey(accumulator, key);
        return _codec.Encode<TOps, TFormat>(value, accumulator);
    }

    private TFormat CacheKey<TOps, TFormat>()
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        var converted = TOps.CreateString(_name)!;
        IFieldCodec<T, TField>.KeyCache[(typeof(TFormat), _name)] = converted;
        return converted;
    }
}