using WhiteTowerGames.DataFixerSharper.Abstractions;

namespace WhiteTowerGames.DataFixerSharper.Codecs.PrimitiveCodec;

internal class Int32Codec : ICodec<int>
{
    public DataResult<(int, TFormat)> Decode<TOps, TFormat>(TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat>
    {
        var i32 = ops.GetInt32(input);
        if (i32.IsError)
            return DataResult<(int, TFormat)>.Fail(i32.ErrorMessage);

        return DataResult<(int, TFormat)>.Success((i32.GetOrThrow(), input));
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
        var i64 = ops.GetInt64(input);
        if (i64.IsError)
            return DataResult<(long, TFormat)>.Fail(i64.ErrorMessage);

        return DataResult<(long, TFormat)>.Success((i64.GetOrThrow(), input));
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
        var fl = ops.GetFloat(input);
        if (fl.IsError)
            return DataResult<(float, TFormat)>.Fail(fl.ErrorMessage);

        return DataResult<(float, TFormat)>.Success((fl.GetOrThrow(), input));
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
        var dbl = ops.GetDouble(input);
        if (dbl.IsError)
            return DataResult<(double, TFormat)>.Fail(dbl.ErrorMessage);

        return DataResult<(double, TFormat)>.Success((dbl.GetOrThrow(), input));
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
        var bl = ops.GetBool(input);
        if (bl.IsError)
            return DataResult<(bool, TFormat)>.Fail(bl.ErrorMessage);

        return DataResult<(bool, TFormat)>.Success((bl.GetOrThrow(), input));
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
        var str = ops.GetString(input);
        if (str.IsError)
            return DataResult<(string, TFormat)>.Fail(str.ErrorMessage);

        return DataResult<(string, TFormat)>.Success((str.GetOrThrow(), input));
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
        var i8 = ops.GetInt8(input);
        if (i8.IsError)
            return DataResult<(sbyte, TFormat)>.Fail(i8.ErrorMessage);

        return DataResult<(sbyte, TFormat)>.Success((i8.GetOrThrow(), input));
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
        var i16 = ops.GetInt16(input);
        if (i16.IsError)
            return DataResult<(short, TFormat)>.Fail(i16.ErrorMessage);

        return DataResult<(short, TFormat)>.Success((i16.GetOrThrow(), input));
    }

    public DataResult<TFormat> Encode<TOps, TFormat>(short input, TOps ops, TFormat prefix)
        where TOps : IDynamicOps<TFormat>
    {
        return DataResult<TFormat>.Success(ops.CreateInt16<TOps, TFormat>(input));
    }
}
