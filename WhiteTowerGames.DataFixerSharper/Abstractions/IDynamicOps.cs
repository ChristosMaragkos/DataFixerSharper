namespace WhiteTowerGames.DataFixerSharper.Abstractions;

/// <summary>
/// Defines contracts used across this library for encoding and decoding.
/// Unless explicitly stated, all implemented methods that return a DataResult
/// must return a <see cref="DataResult{T}.Fail(string)"/> instead of throwing exceptions willy-nilly.
/// </summary>
/// <remarks>
/// It is generally recommended (and good practice) for <typeparamref name="TFormat"/> to be a value type (readonly struct),
/// in order to avoid allocating the same objects multiple times.
/// This is because most of the mutation methods here assume immutability and simply
/// return the newly mutated object wrapped in a <see cref="DataResult{TFormat}"/>.
/// </remarks>
public interface IDynamicOps<TFormat>
{
    #region Value Creation
    /// <summary>
    /// Creates the minimal viable accumulator you can get an empty object back from.
    /// In most formats, this would be represented by an empty map/dictionary (e.g. "{}" in JSON.)
    /// </summary>
    TFormat Empty();
    TFormat CreateNumeric(decimal number);
    TFormat CreateString(string value);
    TFormat CreateBool(bool value);
    #endregion

    #region Value Reading
    /// <summary>
    /// Formats the given <paramref name="input"/> into a <see langword="decimal"/>.
    /// </summary>
    DataResult<decimal> GetNumber(TFormat input);

    /// <summary>
    /// Formats the given <paramref name="input"/> into a <see langword="string"/>.
    /// </summary>
    DataResult<string> GetString(TFormat input);

    /// <summary>
    /// Formats the given <paramref name="input"/> into a <see langword="bool"/>.
    /// </summary>
    DataResult<bool> GetBool(TFormat input);

    /// <summary>
    /// Attempts to extract a child element named <paramref name="name"/> from the given <paramref name="input"/>.
    /// </summary>
    /// <returns>
    /// A success result containing the child element if found.
    /// Must return a failure result if the <paramref name="input"/> is not a map or the key does not exist.
    /// </returns>
    DataResult<TFormat> GetValue(TFormat input, string name);
    #endregion

    #region Enumerables
    /// <summary>
    /// Creates an empty list. This is necessary to call in order for any other list operations to be successful.
    /// </summary>
    TFormat CreateEmptyList();

    /// <summary>
    /// Adds the given <paramref name="element"/> to the given <paramref name="list"/>.
    /// </summary>
    /// <returns>
    /// A DataResult.Success with the <paramref name="list"/> containing the newly added element,
    /// or a DataResult.Fail containing any error messages.
    /// </returns>
    DataResult<TFormat> AddToList(TFormat list, TFormat element);

    /// <summary>
    /// Decodes the given <paramref name="input"/> by trying to parse it as a list.
    /// </summary>
    /// <returns>
    /// Nothing of use. The resulting list will be within the <paramref name="state"/> passed in by reference.
    /// However, should any errors occur, they will be within the returned DataResult.
    /// </returns>
    /// <remarks>
    /// Be careful operating on the <paramref name="input"/> after this method; if <typeparamref name="TFormat"/> is
    /// a reference type (class), it will be mutated as well, meaning both it and the returned DataResult will contain the new element.
    /// However, if <typeparamref name="TFormat"/> is a value type (struct), the old <paramref name="input"/> will not contain the new element.
    /// </remarks>
    DataResult<Unit> ReadList<TState, TCon>(TFormat input, ref TState state, TCon consumer)
        where TState : allows ref struct
        where TCon : ICollectionConsumer<TState, TFormat>;

    /// <summary>
    /// Closes out an open list. You must ABSOLUTELY call this unless you want malformed arrays.
    /// </summary>
    TFormat FinalizeList(TFormat list);
    #endregion

    #region Maps
    /// <summary>
    /// Creates an empty map. This is necessary to call in order for any other map operations to be successful.
    /// </summary>
    TFormat CreateEmptyMap();

    /// <summary>
    /// Adds the given <paramref name="key"/> and <paramref name="value"/> to the given <paramref name="map"/>.
    /// </summary>
    /// <returns>
    /// A DataResult.Success with the <paramref name="map"/> containing the newly added key-value pair,
    /// or a DataResult.Fail containing any error messages.
    /// </returns>
    /// <remarks>
    /// Be careful operating on the <paramref name="map"/> after this method; if <typeparamref name="TFormat"/> is
    /// a reference type (class), it will be mutated as well, meaning both it and the returned DataResult will contain the new element.
    /// However, if <typeparamref name="TFormat"/> is a value type (struct), the old <paramref name="map"/> will not contain the new element.
    /// </remarks>
    DataResult<TFormat> AddToMap(TFormat map, TFormat key, TFormat value);

    /// <summary>
    /// Decodes the given <paramref name="input"/> by trying to parse it as a map.
    /// </summary>
    /// <returns>
    /// Nothing of use. The resulting map will be within the <paramref name="state"/> passed in by reference.
    /// However, should any errors occur, they will be within the returned DataResult.
    /// </returns>
    DataResult<Unit> ReadMap<TState, TCon>(TFormat input, ref TState state, TCon consumer)
        where TState : allows ref struct
        where TCon : IMapConsumer<TState, TFormat>;

    /// <summary>
    /// Closes out an open map. You must ABSOLUTELY call this unless you want malformed dictionaries.
    /// </summary>
    TFormat FinalizeMap(TFormat map);
    #endregion

    #region Utils
    /// <summary>
    /// Merges the encoded <paramref name="value"/> into the accumulating <paramref name="prefix"/>.
    /// This is heavily used by <see cref="Codecs.RecordCodec" to build up a final structure field by field.
    /// </summary>
    /// <returns>
    /// The combined representation. If <typeparamref name="TFormat"/> is a value type, this returns a new struct.
    /// If it is a reference type, it may mutate and return the original <paramref name="prefix"/>.
    /// </returns>
    TFormat AppendToPrefix(TFormat prefix, TFormat value);

    /// <summary>
    /// Returns a new representation of the <paramref name="input"/> with the specified <paramref name="valueKey"/> removed.
    /// Used during decoding to calculate the "remainder" of a structure after a field has been parsed.
    /// </summary>
    /// <remarks>
    /// Not strictly necessary to be implemented unless your backing format requires removing fields as they are being processed.
    /// </remarks>
    TFormat RemoveFromInput(TFormat input, string valueKey);

    /// <summary>
    /// Checks if the given <paramref name="key"/> matches the <paramref name="targetKey"/>.
    /// </summary>
    /// <remarks>
    /// This method should generally not allocate on the heap unless necessary
    /// to prevent <see cref="StackOverflowException"/>.
    /// <see langword="stackalloc"/> is your friend.
    /// </remarks>
    bool StringsMatch(TFormat key, string targetKey);
    #endregion
}

public static class DynamicOpsExtensions
{
    public static TFormat CreateInt8<TOps, TFormat>(this TOps ops, sbyte value)
        where TOps : IDynamicOps<TFormat> => ops.CreateNumeric(value);

    public static TFormat CreateUInt8<TOps, TFormat>(this TOps ops, byte value)
        where TOps : IDynamicOps<TFormat> => ops.CreateNumeric(value);

    public static TFormat CreateInt16<TOps, TFormat>(this TOps ops, short value)
        where TOps : IDynamicOps<TFormat> => ops.CreateNumeric(value);

    public static TFormat CreateUInt16<TOps, TFormat>(this TOps ops, ushort value)
        where TOps : IDynamicOps<TFormat> => ops.CreateNumeric(value);

    public static TFormat CreateInt32<TOps, TFormat>(this TOps ops, int value)
        where TOps : IDynamicOps<TFormat> => ops.CreateNumeric(value);

    public static TFormat CreateUInt32<TOps, TFormat>(this TOps ops, uint value)
        where TOps : IDynamicOps<TFormat> => ops.CreateNumeric(value);

    public static TFormat CreateInt64<TOps, TFormat>(this TOps ops, long value)
        where TOps : IDynamicOps<TFormat> => ops.CreateNumeric(value);

    public static TFormat CreateUInt64<TOps, TFormat>(this TOps ops, ulong value)
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

    public static DataResult<byte> GetUInt8<TOps, TFormat>(this TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat>
    {
        var num = ops.GetNumber(input);
        return num.IsError
            ? DataResult<byte>.Fail(num.ErrorMessage)
            : DataResult<byte>.Success((byte)num.GetOrThrow());
    }

    public static DataResult<short> GetInt16<TOps, TFormat>(this TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat>
    {
        var num = ops.GetNumber(input);
        return num.IsError
            ? DataResult<short>.Fail(num.ErrorMessage)
            : DataResult<short>.Success((short)num.GetOrThrow());
    }

    public static DataResult<ushort> GetUInt16<TOps, TFormat>(this TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat>
    {
        var num = ops.GetNumber(input);
        return num.IsError
            ? DataResult<ushort>.Fail(num.ErrorMessage)
            : DataResult<ushort>.Success((ushort)num.GetOrThrow());
    }

    public static DataResult<int> GetInt32<TOps, TFormat>(this TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat>
    {
        var num = ops.GetNumber(input);
        return num.IsError
            ? DataResult<int>.Fail(num.ErrorMessage)
            : DataResult<int>.Success((int)num.GetOrThrow());
    }

    public static DataResult<uint> GetUInt32<TOps, TFormat>(this TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat>
    {
        var num = ops.GetNumber(input);
        return num.IsError
            ? DataResult<uint>.Fail(num.ErrorMessage)
            : DataResult<uint>.Success((uint)num.GetOrThrow());
    }

    public static DataResult<long> GetInt64<TOps, TFormat>(this TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat>
    {
        var num = ops.GetNumber(input);
        return num.IsError
            ? DataResult<long>.Fail(num.ErrorMessage)
            : DataResult<long>.Success((long)num.GetOrThrow());
    }

    public static DataResult<ulong> GetUInt64<TOps, TFormat>(this TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat>
    {
        var num = ops.GetNumber(input);
        return num.IsError
            ? DataResult<ulong>.Fail(num.ErrorMessage)
            : DataResult<ulong>.Success((ulong)num.GetOrThrow());
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
