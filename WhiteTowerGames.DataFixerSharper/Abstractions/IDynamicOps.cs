namespace WhiteTowerGames.DataFixerSharper.Abstractions;

/// <summary>
/// Defines contracts used across this library for encoding and decoding.
/// Unless explicitly stated, all implemented methods that return a DataResult
/// must return a <see cref="DataResult{T}.Fail(string)"/> instead of throwing exceptions willy-nilly.
/// </summary>
public interface IDynamicOps<TFormat> where TFormat : struct
{
    #region Value Creation
    static abstract TFormat Empty();
    static abstract TFormat CreateNumeric(decimal number);
    static abstract TFormat CreateString(string value);
    static abstract TFormat CreateBool(bool value);
    #endregion

    #region Value Reading
    static abstract DataResult<decimal> GetNumber(TFormat input);
    static abstract DataResult<string> GetString(TFormat input);
    static abstract DataResult<bool> GetBool(TFormat input);
    static abstract DataResult<TFormat> GetValue(TFormat input, string name);
    #endregion

    #region Enumerables
    static abstract TFormat CreateEmptyList();
    static abstract DataResult<TFormat> AddToList(TFormat list, TFormat element);

    static abstract DataResult<Unit> ReadList<TState, TCon>(TFormat input, ref TState state, TCon consumer)
        where TState : allows ref struct
        where TCon : ICollectionConsumer<TState, TFormat>;

    static abstract TFormat FinalizeList(TFormat list);
    #endregion

    #region Maps
    static abstract TFormat CreateEmptyMap();
    static abstract DataResult<TFormat> AddToMap(TFormat map, TFormat key, TFormat value);

    static abstract DataResult<Unit> ReadMap<TState, TCon>(TFormat input, ref TState state, TCon consumer)
        where TState : allows ref struct
        where TCon : IMapConsumer<TState, TFormat>;

    static abstract TFormat FinalizeMap(TFormat map);
    #endregion

    #region Utils
    static abstract TFormat AppendToPrefix(TFormat prefix, TFormat value);
    static abstract TFormat RemoveFromInput(TFormat input, string valueKey);
    static abstract bool StringsMatch(TFormat key, string targetKey);
    #endregion
}

public static class DynamicOpsExtensions
{
    public static TFormat CreateInt8<TOps, TFormat>(sbyte value)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct => TOps.CreateNumeric(value);

    public static TFormat CreateUInt8<TOps, TFormat>(byte value)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct => TOps.CreateNumeric(value);

    public static TFormat CreateInt16<TOps, TFormat>(short value)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct => TOps.CreateNumeric(value);

    public static TFormat CreateUInt16<TOps, TFormat>(ushort value)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct => TOps.CreateNumeric(value);

    public static TFormat CreateInt32<TOps, TFormat>(int value)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct => TOps.CreateNumeric(value);

    public static TFormat CreateUInt32<TOps, TFormat>(uint value)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct => TOps.CreateNumeric(value);

    public static TFormat CreateInt64<TOps, TFormat>(long value)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct => TOps.CreateNumeric(value);

    public static TFormat CreateUInt64<TOps, TFormat>(ulong value)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct => TOps.CreateNumeric(value);

    public static TFormat CreateFloat<TOps, TFormat>(float value)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct => TOps.CreateNumeric((decimal)value);

    public static TFormat CreateDouble<TOps, TFormat>(double value)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct => TOps.CreateNumeric((decimal)value);

    public static DataResult<sbyte> GetInt8<TOps, TFormat>(TFormat input)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        var num = TOps.GetNumber(input);
        return num.IsError
            ? DataResult<sbyte>.Fail(num.ErrorMessage)
            : DataResult<sbyte>.Success((sbyte)num.GetOrThrow());
    }

    public static DataResult<byte> GetUInt8<TOps, TFormat>(TFormat input)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        var num = TOps.GetNumber(input);
        return num.IsError
            ? DataResult<byte>.Fail(num.ErrorMessage)
            : DataResult<byte>.Success((byte)num.GetOrThrow());
    }

    public static DataResult<short> GetInt16<TOps, TFormat>(TFormat input)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        var num = TOps.GetNumber(input);
        return num.IsError
            ? DataResult<short>.Fail(num.ErrorMessage)
            : DataResult<short>.Success((short)num.GetOrThrow());
    }

    public static DataResult<ushort> GetUInt16<TOps, TFormat>(TFormat input)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        var num = TOps.GetNumber(input);
        return num.IsError
            ? DataResult<ushort>.Fail(num.ErrorMessage)
            : DataResult<ushort>.Success((ushort)num.GetOrThrow());
    }

    public static DataResult<int> GetInt32<TOps, TFormat>(TFormat input)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        var num = TOps.GetNumber(input);
        return num.IsError
            ? DataResult<int>.Fail(num.ErrorMessage)
            : DataResult<int>.Success((int)num.GetOrThrow());
    }

    public static DataResult<uint> GetUInt32<TOps, TFormat>(TFormat input)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        var num = TOps.GetNumber(input);
        return num.IsError
            ? DataResult<uint>.Fail(num.ErrorMessage)
            : DataResult<uint>.Success((uint)num.GetOrThrow());
    }

    public static DataResult<long> GetInt64<TOps, TFormat>(TFormat input)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        var num = TOps.GetNumber(input);
        return num.IsError
            ? DataResult<long>.Fail(num.ErrorMessage)
            : DataResult<long>.Success((long)num.GetOrThrow());
    }

    public static DataResult<ulong> GetUInt64<TOps, TFormat>(TFormat input)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        var num = TOps.GetNumber(input);
        return num.IsError
            ? DataResult<ulong>.Fail(num.ErrorMessage)
            : DataResult<ulong>.Success((ulong)num.GetOrThrow());
    }

    public static DataResult<float> GetFloat<TOps, TFormat>(TFormat input)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        var num = TOps.GetNumber(input);
        return num.IsError
            ? DataResult<float>.Fail(num.ErrorMessage)
            : DataResult<float>.Success((float)num.GetOrThrow());
    }

    public static DataResult<double> GetDouble<TOps, TFormat>(TFormat input)
        where TOps : IDynamicOps<TFormat>
        where TFormat : struct
    {
        var num = TOps.GetNumber(input);
        return num.IsError
            ? DataResult<double>.Fail(num.ErrorMessage)
            : DataResult<double>.Success((double)num.GetOrThrow());
    }
}
