using WhiteTowerGames.DataFixerSharper.Abstractions;

namespace WhiteTowerGames.DataFixerSharper.Codecs.PrimitiveCodec;

internal class Int32Codec : ICodec<int>
{
    public DataResult<(int, TFormat)> Decode<TOps, TFormat>(TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat>
    {
        return ops.GetInt32<TOps, TFormat>(input).Map(i32 => (i32, input));
    }

    public DataResult<TFormat> Encode<TOps, TFormat>(int input, TOps ops, TFormat prefix)
        where TOps : IDynamicOps<TFormat>
    {
        return DataResult<TFormat>.Success(ops.CreateInt32<TOps, TFormat>(input));
    }
}

internal class Int64Codec : ICodec<long>
{
    public DataResult<(long, TFormat)> Decode<TOps, TFormat>(TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat>
    {
        return ops.GetInt64<TOps, TFormat>(input).Map(i64 => (i64, input));
    }

    public DataResult<TFormat> Encode<TOps, TFormat>(long input, TOps ops, TFormat prefix)
        where TOps : IDynamicOps<TFormat>
    {
        return DataResult<TFormat>.Success(ops.CreateInt64<TOps, TFormat>(input));
    }
}

internal class FloatCodec : ICodec<float>
{
    public DataResult<(float, TFormat)> Decode<TOps, TFormat>(TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat>
    {
        return ops.GetFloat<TOps, TFormat>(input).Map(f => (f, input));
    }

    public DataResult<TFormat> Encode<TOps, TFormat>(float input, TOps ops, TFormat prefix)
        where TOps : IDynamicOps<TFormat>
    {
        return DataResult<TFormat>.Success(ops.CreateFloat<TOps, TFormat>(input));
    }
}

internal class DoubleCodec : ICodec<double>
{
    public DataResult<(double, TFormat)> Decode<TOps, TFormat>(TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat>
    {
        return ops.GetDouble<TOps, TFormat>(input).Map(d => (d, input));
    }

    public DataResult<TFormat> Encode<TOps, TFormat>(double input, TOps ops, TFormat prefix)
        where TOps : IDynamicOps<TFormat>
    {
        return DataResult<TFormat>.Success(ops.CreateDouble<TOps, TFormat>(input));
    }
}

internal class BoolCodec : ICodec<bool>
{
    public DataResult<(bool, TFormat)> Decode<TOps, TFormat>(TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat>
    {
        return ops.GetBool(input).Map(b => (b, input));
    }

    public DataResult<TFormat> Encode<TOps, TFormat>(bool input, TOps ops, TFormat prefix)
        where TOps : IDynamicOps<TFormat>
    {
        return DataResult<TFormat>.Success(ops.CreateBool(input));
    }
}

internal class StringCodec : ICodec<string>
{
    public DataResult<(string, TFormat)> Decode<TOps, TFormat>(TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat>
    {
        return ops.GetString(input).Map(s => (s, input));
    }

    public DataResult<TFormat> Encode<TOps, TFormat>(string input, TOps ops, TFormat prefix)
        where TOps : IDynamicOps<TFormat>
    {
        return DataResult<TFormat>.Success(ops.CreateString(input));
    }
}

internal class Int8Codec : ICodec<sbyte>
{
    public DataResult<(sbyte, TFormat)> Decode<TOps, TFormat>(TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat>
    {
        return ops.GetInt8<TOps, TFormat>(input).Map(i32 => (i32, input));
    }

    public DataResult<TFormat> Encode<TOps, TFormat>(sbyte input, TOps ops, TFormat prefix)
        where TOps : IDynamicOps<TFormat>
    {
        return DataResult<TFormat>.Success(ops.CreateInt8<TOps, TFormat>(input));
    }
}

internal class Int16Codec : ICodec<short>
{
    public DataResult<(short, TFormat)> Decode<TOps, TFormat>(TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat>
    {
        return ops.GetInt16<TOps, TFormat>(input).Map(i64 => (i64, input));
    }

    public DataResult<TFormat> Encode<TOps, TFormat>(short input, TOps ops, TFormat prefix)
        where TOps : IDynamicOps<TFormat>
    {
        return DataResult<TFormat>.Success(ops.CreateInt16<TOps, TFormat>(input));
    }
}
