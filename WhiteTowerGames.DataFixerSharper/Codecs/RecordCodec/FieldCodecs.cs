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

    public FieldCodec(Codec<TField> codec, string name, Func<T, TField> getter)
    {
        _codec = codec;
        _name = name;
        _getter = getter;
    }

    public DataResult<(TField, TFormat)> Decode<TFormat>(IDynamicOps<TFormat> ops, TFormat input)
    {
        var fieldResult = ops.GetValue(input, _name);
        if (fieldResult.IsError)
            return DataResult<(TField, TFormat)>.Fail(fieldResult.ErrorMessage);

        var parsed = _codec.Parse(ops, fieldResult.GetOrThrow());
        if (parsed.IsError)
            return DataResult<(TField, TFormat)>.Fail(parsed.ErrorMessage);

        return parsed.Map(decoded =>
            (decoded, ops.RemoveFromInput(input, ops.CreateString(_name)))
        );
    }

    public DataResult<TFormat> Encode<TFormat>(T input, IDynamicOps<TFormat> ops, TFormat prefix)
    {
        var fieldValue = _getter(input);
        var encoded = _codec.EncodeStart(ops, fieldValue);
        return encoded.Map(e => ops.MergeAndAppend(prefix, ops.CreateString(_name), e));
    }
}

public class OptionalFieldCodec<T, TField> : IFieldCodec<T, TField>
{
    private readonly Codec<TField> _codec;
    private readonly string _name;
    private readonly Func<T, TField> _getter;
    private readonly TField _defaultValue;

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
        var fieldResult = ops.GetValue(input, _name);
        if (fieldResult.IsError)
            return DataResult<(TField, TFormat)>.Success((_defaultValue, input));

        var parsed = _codec.Parse(ops, fieldResult.GetOrThrow());
        if (parsed.IsError)
            return DataResult<(TField, TFormat)>.Fail(parsed.ErrorMessage);

        return parsed.Map(decoded =>
            (decoded, ops.RemoveFromInput(input, ops.CreateString(_name)))
        );
    }

    public DataResult<TFormat> Encode<TFormat>(T input, IDynamicOps<TFormat> ops, TFormat prefix)
    {
        var fieldValue = _getter(input);

        var key = ops.CreateString(_name);
        var valueResult = _codec.EncodeStart(ops, fieldValue);

        if (valueResult.IsError)
            return valueResult;

        ops.MergeAndAppend(prefix, key, valueResult.GetOrThrow());

        return DataResult<TFormat>.Success(prefix);
    }
}
