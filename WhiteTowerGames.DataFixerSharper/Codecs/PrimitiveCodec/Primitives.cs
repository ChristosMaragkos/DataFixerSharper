using WhiteTowerGames.DataFixerSharper.Abstractions;

namespace WhiteTowerGames.DataFixerSharper.Codecs.PrimitiveCodec;

internal class Int32Codec : Codec<int>
{
    public override DataResult<(int, TFormat)> Decode<TOps, TFormat>(TOps ops, TFormat input)
    {
        return ops.GetInt32<TOps, TFormat>(input).Map(i32 => (i32, input));
    }

    public override DataResult<TFormat> Encode<TOps, TFormat>(int input, TOps ops, TFormat prefix)
    {
        return DataResult<TFormat>.Success(ops.CreateInt32<TOps, TFormat>(input));
    }
}

internal class Int64Codec : Codec<long>
{
    public override DataResult<(long, TFormat)> Decode<TOps, TFormat>(TOps ops, TFormat input)
    {
        return ops.GetInt64<TOps, TFormat>(input).Map(i64 => (i64, input));
    }

    public override DataResult<TFormat> Encode<TOps, TFormat>(long input, TOps ops, TFormat prefix)
    {
        return DataResult<TFormat>.Success(ops.CreateInt64<TOps, TFormat>(input));
    }
}

internal class FloatCodec : Codec<float>
{
    public override DataResult<(float, TFormat)> Decode<TOps, TFormat>(TOps ops, TFormat input)
    {
        return ops.GetFloat<TOps, TFormat>(input).Map(f => (f, input));
    }

    public override DataResult<TFormat> Encode<TOps, TFormat>(float input, TOps ops, TFormat prefix)
    {
        return DataResult<TFormat>.Success(ops.CreateFloat<TOps, TFormat>(input));
    }
}

internal class DoubleCodec : Codec<double>
{
    public override DataResult<(double, TFormat)> Decode<TOps, TFormat>(TOps ops, TFormat input)
    {
        return ops.GetDouble<TOps, TFormat>(input).Map(d => (d, input));
    }

    public override DataResult<TFormat> Encode<TOps, TFormat>(
        double input,
        TOps ops,
        TFormat prefix
    )
    {
        return DataResult<TFormat>.Success(ops.CreateDouble<TOps, TFormat>(input));
    }
}

internal class BoolCodec : Codec<bool>
{
    public override DataResult<(bool, TFormat)> Decode<TOps, TFormat>(TOps ops, TFormat input)
    {
        return ops.GetBool(input).Map(b => (b, input));
    }

    public override DataResult<TFormat> Encode<TOps, TFormat>(bool input, TOps ops, TFormat prefix)
    {
        return DataResult<TFormat>.Success(ops.CreateBool(input));
    }
}

internal class StringCodec : Codec<string>
{
    public override DataResult<(string, TFormat)> Decode<TOps, TFormat>(TOps ops, TFormat input)
    {
        return ops.GetString(input).Map(s => (s, input));
    }

    public override DataResult<TFormat> Encode<TOps, TFormat>(
        string input,
        TOps ops,
        TFormat prefix
    )
    {
        return DataResult<TFormat>.Success(ops.CreateString(input));
    }
}

internal class Int8Codec : Codec<sbyte>
{
    public override DataResult<(sbyte, TFormat)> Decode<TOps, TFormat>(TOps ops, TFormat input)
    {
        return ops.GetInt8<TOps, TFormat>(input).Map(i32 => (i32, input));
    }

    public override DataResult<TFormat> Encode<TOps, TFormat>(sbyte input, TOps ops, TFormat prefix)
    {
        return DataResult<TFormat>.Success(ops.CreateInt8<TOps, TFormat>(input));
    }
}

internal class Int16Codec : Codec<short>
{
    public override DataResult<(short, TFormat)> Decode<TOps, TFormat>(TOps ops, TFormat input)
    {
        return ops.GetInt16<TOps, TFormat>(input).Map(i64 => (i64, input));
    }

    public override DataResult<TFormat> Encode<TOps, TFormat>(short input, TOps ops, TFormat prefix)
    {
        return DataResult<TFormat>.Success(ops.CreateInt16<TOps, TFormat>(input));
    }
}
