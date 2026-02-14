using System.ComponentModel;

namespace WhiteTowerGames.DataFixerSharper.Abstractions;

[EditorBrowsable(EditorBrowsableState.Never)]
public interface IDynamicOps
{
    object Empty();
    object CreateNumeric(decimal number);
    object CreateString(string value);
    object CreateBool(bool value);

    DataResult<decimal> GetNumber(object input);
    DataResult<string> GetString(object input);
    DataResult<bool> GetBool(object input);
    DataResult<object> GetValue(object input, string name);

    object CreateList(IEnumerable<object> elements);
    DataResult<IEnumerable<object>> ReadAsStream(object input);

    object CreateMap(IEnumerable<KeyValuePair<object, object>> map);
    DataResult<IEnumerable<KeyValuePair<object, object>>> ReadAsMap(object input);
    object Merge(object key, object value);
    object MergeAndAppend(object map, object key, object value);

    object AppendToPrefix(object prefix, object value);
    object RemoveFromInput(object input, object value);
}

public interface IDynamicOps<T> : IDynamicOps
{
    #region Value Creation
    new T Empty();
    new T CreateNumeric(decimal number);
    new T CreateString(string value);
    new T CreateBool(bool value);
    #endregion

    #region Value Reading
    DataResult<decimal> GetNumber(T input);
    DataResult<string> GetString(T input);
    DataResult<bool> GetBool(T input);
    DataResult<T> GetValue(T input, string name);
    #endregion

    #region Enumerables
    T CreateList(IEnumerable<T> elements);
    DataResult<IEnumerable<T>> ReadAsStream(T input);
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

    #region Default Implementations
    object IDynamicOps.Empty() => Empty()!;
    object IDynamicOps.CreateNumeric(decimal number) => CreateNumeric(number)!;
    object IDynamicOps.CreateString(string value) => CreateString(value)!;
    object IDynamicOps.CreateBool(bool value) => CreateBool(value)!;

    DataResult<decimal> IDynamicOps.GetNumber(object input) => GetNumber((T)input);
    DataResult<string> IDynamicOps.GetString(object input) => GetString((T)input);
    DataResult<bool> IDynamicOps.GetBool(object input) => GetBool((T)input);
    DataResult<object> IDynamicOps.GetValue(object input, string name) =>
        GetValue((T)input, name).Map(result => (object)result!);

    object IDynamicOps.CreateList(IEnumerable<object> elements) =>
        CreateList((IEnumerable<T>)elements)!;

    DataResult<IEnumerable<object>> IDynamicOps.ReadAsStream(object input) =>
        ReadAsStream((T)input).Map(stream => stream.Select(item => (object)item!));

    object IDynamicOps.CreateMap(IEnumerable<KeyValuePair<object, object>> map) =>
        CreateMap((IEnumerable<KeyValuePair<T, T>>)map)!;

    DataResult<IEnumerable<KeyValuePair<object, object>>> IDynamicOps.ReadAsMap(object input) =>
        ReadAsMap((T)input)
            .Map(map =>
                map.Select(kvp => new KeyValuePair<object, object>(
                    (object)kvp.Key!,
                    (object)kvp.Value!
                ))
            );

    object IDynamicOps.Merge(object key, object value) => Merge((T)key, (T)value)!;
    object IDynamicOps.MergeAndAppend(object map, object key, object value) =>
        MergeAndAppend((T)map, (T)key, (T)value)!;

    object IDynamicOps.AppendToPrefix(object prefix, object value) =>
        AppendToPrefix((T)prefix, (T)value)!;
    object IDynamicOps.RemoveFromInput(object input, object value) =>
        RemoveFromInput((T)input, (T)value)!;
    #endregion
}

public static class DynamicOpsExtensions
{
    public static object CreateInt32(this IDynamicOps ops, int value) => ops.CreateNumeric(value);

    public static object CreateInt64(this IDynamicOps ops, long value) => ops.CreateNumeric(value);

    public static object CreateFloat(this IDynamicOps ops, float value) =>
        ops.CreateNumeric((decimal)value);

    public static object CreateDouble(this IDynamicOps ops, double value) =>
        ops.CreateNumeric((decimal)value);

    public static DataResult<int> GetInt32(this IDynamicOps ops, object input) =>
        ops.GetNumber(input).Map(d => (int)d);

    public static DataResult<long> GetInt64(this IDynamicOps ops, object input) =>
        ops.GetNumber(input).Map(d => (long)d);

    public static DataResult<float> GetFloat(this IDynamicOps ops, object input) =>
        ops.GetNumber(input).Map(d => (float)d);

    public static DataResult<double> GetDouble(this IDynamicOps ops, object input) =>
        ops.GetNumber(input).Map(d => (double)d);
}
