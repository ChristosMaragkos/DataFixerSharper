using WhiteTowerGames.DataFixerSharper.Codecs.RecordCodec;

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
    DataResult<TFormat> AddToList(TFormat list, TFormat element); // returns data result because the list might not be a list at all.
    DataResult<Unit> ReadList<TState, TCon>(TFormat input, ref TState state, TCon consumer)
        where TState : allows ref struct
        where TCon : ICollectionConsumer<TState, TFormat>;
    #endregion

    #region Maps
    TFormat CreateMap(IEnumerable<KeyValuePair<TFormat, TFormat>> map);

    DataResult<Unit> ReadMap<TState, TCon>(TFormat input, ref TState state, TCon consumer)
        where TState : allows ref struct
        where TCon : IMapConsumer<TState, TFormat>;

    TFormat Merge(TFormat key, TFormat value);

    /// <summary>
    /// Merges two values into a key-value pair and appends to an existing map
    /// </summary>
    TFormat MergeAndAppend(TFormat map, TFormat key, TFormat value);
    #endregion

    #region Utils
    TFormat AppendToPrefix(TFormat prefix, TFormat value);
    TFormat RemoveFromInput(TFormat input, TFormat value);
    IRecordBuilder<TFormat> MapBuilder();
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
        where TOps : IDynamicOps<TFormat> => ops.GetNumber(input).Map(d => (sbyte)d);

    public static DataResult<short> GetInt16<TOps, TFormat>(this TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat> => ops.GetNumber(input).Map(d => (short)d);

    public static DataResult<int> GetInt32<TOps, TFormat>(this TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat> => ops.GetNumber(input).Map(d => (int)d);

    public static DataResult<long> GetInt64<TOps, TFormat>(this TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat> => ops.GetNumber(input).Map(d => (long)d);

    public static DataResult<float> GetFloat<TOps, TFormat>(this TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat> => ops.GetNumber(input).Map(d => (float)d);

    public static DataResult<double> GetDouble<TOps, TFormat>(this TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat> => ops.GetNumber(input).Map(d => (double)d);
}
