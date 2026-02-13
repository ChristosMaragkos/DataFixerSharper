using System.ComponentModel;

namespace WhiteTowerGames.DataFixerSharper.Abstractions;

[EditorBrowsable(EditorBrowsableState.Never)]
public interface IDynamicOps;

public interface IDynamicOps<T> : IDynamicOps
{
    #region Value Creation
    T Empty();
    T CreateNumeric(decimal number);
    T CreateString(string value);
    T CreateBool(bool value);
    #endregion

    #region Value Reading
    DataResult<decimal> GetNumber(T input);
    DataResult<string> GetString(T input);
    DataResult<bool> GetBool(T input);
    #endregion

    #region Enumerables
    T CreateList(IEnumerable<T> elements);
    DataResult<IReadOnlyList<T>> ReadAsStream(T input);
    #endregion

    #region Maps
    T CreateMap(IEnumerable<KeyValuePair<T, T>> map);

    DataResult<IEnumerable<KeyValuePair<T, T>>> ReadAsMap(T input);

    T Merge(T key, T value);

    /// <summary>
    /// Merges two values into a key-value pair and appends to an existing map
    /// </summary>
    T MergeAndAppend(T map, T key, T value);
    #endregion

    #region Utils
    T AppendToPrefix(T prefix, T value);
    T RemoveFromInput(T input, T value);
    #endregion
}

public static class DynamicOpsExtensions
{
    public static T CreateInt32<T>(this IDynamicOps<T> ops, int value) => ops.CreateNumeric(value);

    public static T CreateInt64<T>(this IDynamicOps<T> ops, long value) => ops.CreateNumeric(value);

    public static T CreateFloat<T>(this IDynamicOps<T> ops, float value) =>
        ops.CreateNumeric((decimal)value);

    public static T CreateDouble<T>(this IDynamicOps<T> ops, double value) =>
        ops.CreateNumeric((decimal)value);

    public static DataResult<int> GetInt32<T>(this IDynamicOps<T> ops, T input) =>
        ops.GetNumber(input).Map(d => (int)d);

    public static DataResult<long> GetInt64<T>(this IDynamicOps<T> ops, T input) =>
        ops.GetNumber(input).Map(d => (long)d);

    public static DataResult<float> GetFloat<T>(this IDynamicOps<T> ops, T input) =>
        ops.GetNumber(input).Map(d => (float)d);

    public static DataResult<double> GetDouble<T>(this IDynamicOps<T> ops, T input) =>
        ops.GetNumber(input).Map(d => (double)d);
}
