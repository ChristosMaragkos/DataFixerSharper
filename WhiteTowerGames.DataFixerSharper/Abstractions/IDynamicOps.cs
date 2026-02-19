namespace WhiteTowerGames.DataFixerSharper.Abstractions;

public interface IDynamicOps<TFormat>
{
    #region Value Creation
    TFormat Empty();
    TFormat CreateNumeric(decimal number);
    TFormat CreateString(string value);
    TFormat CreateBool(bool value);
    #endregion

    #region Value Reading
    DataResult<decimal> GetNumber(TFormat input);
    DataResult<string> GetString(TFormat input);
    DataResult<bool> GetBool(TFormat input);
    DataResult<TFormat> GetValue(TFormat input, string name);
    #endregion

    #region Enumerables
    TFormat CreateEmptyList();
    DataResult<TFormat> AddToList(TFormat list, TFormat element);
    DataResult<Unit> ReadList<TState, TCon>(TFormat input, ref TState state, TCon consumer)
        where TState : allows ref struct
        where TCon : ICollectionConsumer<TState, TFormat>;
    TFormat FinalizeList(TFormat list);
    #endregion

    #region Maps
    TFormat CreateEmptyMap();
    DataResult<TFormat> AddToMap(TFormat map, TFormat key, TFormat value);
    DataResult<Unit> ReadMap<TState, TCon>(TFormat input, ref TState state, TCon consumer)
        where TState : allows ref struct
        where TCon : IMapConsumer<TState, TFormat>;
    TFormat FinalizeMap(TFormat map);
    #endregion

    #region Utils
    TFormat AppendToPrefix(TFormat prefix, TFormat value);
    TFormat RemoveFromInput(TFormat input, string valueKey);
    // DataResult<TFormat> Parse(ReadOnlySpan<byte> bytes);
    #endregion
}

public static class DynamicOpsExtensions
{
    public static TFormat CreateInt8<TOps, TFormat>(this TOps ops, sbyte value)
        where TOps : IDynamicOps<TFormat> => ops.CreateNumeric((decimal)value);

    public static TFormat CreateInt16<TOps, TFormat>(this TOps ops, short value)
        where TOps : IDynamicOps<TFormat> => ops.CreateNumeric((decimal)value);

    public static TFormat CreateInt32<TOps, TFormat>(this TOps ops, int value)
        where TOps : IDynamicOps<TFormat> => ops.CreateNumeric(value);

    public static TFormat CreateInt64<TOps, TFormat>(this TOps ops, long value)
        where TOps : IDynamicOps<TFormat> => ops.CreateNumeric(value);

    public static TFormat CreateFloat<TOps, TFormat>(this TOps ops, float value)
        where TOps : IDynamicOps<TFormat> => ops.CreateNumeric((decimal)value);

    public static TFormat CreateDouble<TOps, TFormat>(this TOps ops, double value)
        where TOps : IDynamicOps<TFormat> => ops.CreateNumeric((decimal)value);

    public static DataResult<sbyte> GetInt8<TOps, TFormat>(this TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat>
    {
        var num = ops.GetNumber(input);
        return num.IsError
            ? DataResult<sbyte>.Fail(num.ErrorMessage)
            : DataResult<sbyte>.Success((sbyte)num.GetOrThrow());
    }

    public static DataResult<short> GetInt16<TOps, TFormat>(this TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat>
    {
        var num = ops.GetNumber(input);
        return num.IsError
            ? DataResult<short>.Fail(num.ErrorMessage)
            : DataResult<short>.Success((short)num.GetOrThrow());
    }

    public static DataResult<int> GetInt32<TOps, TFormat>(this TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat>
    {
        var num = ops.GetNumber(input);
        return num.IsError
            ? DataResult<int>.Fail(num.ErrorMessage)
            : DataResult<int>.Success((int)num.GetOrThrow());
    }

    public static DataResult<long> GetInt64<TOps, TFormat>(this TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat>
    {
        var num = ops.GetNumber(input);
        return num.IsError
            ? DataResult<long>.Fail(num.ErrorMessage)
            : DataResult<long>.Success((long)num.GetOrThrow());
    }

    public static DataResult<float> GetFloat<TOps, TFormat>(this TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat>
    {
        var num = ops.GetNumber(input);
        return num.IsError
            ? DataResult<float>.Fail(num.ErrorMessage)
            : DataResult<float>.Success((float)num.GetOrThrow());
    }

    public static DataResult<double> GetDouble<TOps, TFormat>(this TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat>
    {
        var num = ops.GetNumber(input);
        return num.IsError
            ? DataResult<double>.Fail(num.ErrorMessage)
            : DataResult<double>.Success((double)num.GetOrThrow());
    }
}
