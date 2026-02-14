using WhiteTowerGames.DataFixerSharper.Abstractions;

namespace WhiteTowerGames.DataFixerSharper.Codecs;

public interface IEncoder<T>
{
    DataResult<TFormat> Encode<TFormat>(T input, IDynamicOps<TFormat> ops, TFormat prefix);
}

public interface IDecoder<T>
{
    DataResult<(T, TFormat)> Decode<TFormat>(IDynamicOps<TFormat> ops, TFormat input);
}

public abstract class Codec
{
    public static Codec<Dictionary<TKey, TValue>> Dictionary<TKey, TValue>(
        Codec<TKey> keyCodec,
        Codec<TValue> valueCodec
    )
        where TKey : notnull => new DictionaryCodec<TKey, TValue>(keyCodec, valueCodec);

    public static Codec<T> Dispatch<T, TDis>(
        Codec<TDis> discriminatorCodec,
        Func<T, TDis> discriminatorGetter,
        Func<TDis, Codec<T>> codecGetter,
        string discriminatorKeyName = "type"
    ) =>
        new DispatchCodec<T, TDis>(
            discriminatorGetter,
            discriminatorCodec,
            codecGetter,
            discriminatorKeyName
        );
}

public abstract class Codec<T> : Codec, IEncoder<T>, IDecoder<T>
{
    public abstract DataResult<(T, TFormat)> Decode<TFormat>(
        IDynamicOps<TFormat> ops,
        TFormat input
    );

    public abstract DataResult<TFormat> Encode<TFormat>(
        T input,
        IDynamicOps<TFormat> ops,
        TFormat prefix
    );

    public DataResult<TFormat> EncodeStart<TFormat>(IDynamicOps<TFormat> ops, T input) =>
        Encode(input, ops, ops.Empty());

    public DataResult<T> Parse<TFormat>(IDynamicOps<TFormat> ops, TFormat input) =>
        Decode(ops, input).Map(pair => pair.Item1);

    public Codec<IEnumerable<T>> ForEnumerable() => new EnumerableCodec<T>(this);

    public Codec<List<T>> ForList() =>
        ForEnumerable()
            .SafeMap<List<T>>(list => list.AsEnumerable(), enumerable => enumerable.ToList());

    public Codec<HashSet<T>> ForHashSet() =>
        ForEnumerable()
            .SafeMap<HashSet<T>>(set => set.AsEnumerable(), enumerable => enumerable.ToHashSet());

    public Codec<T[]> ForArray() =>
        ForEnumerable()
            .SafeMap<T[]>(array => array.AsEnumerable(), enumerable => enumerable.ToArray());

    /// Creates a Codec<TOther> by converting TOther to T and vice versa when T <-> TOther always valid.
    public Codec<TOther> SafeMap<TOther>(Func<TOther, T> from, Func<T, TOther> to) =>
        new SafeMapCodec<T, TOther>(this, to, from);

    /// Creates a Codec<TOther> by converting TOther to T and vice versa when T <-> TOther not always valid.
    public Codec<TOther> UnsafeMap<TOther>(
        Func<TOther, DataResult<T>> from,
        Func<T, DataResult<TOther>> to
    ) => new UnsafeMapCodec<T, TOther>(this, to, from);

    /// Creates a Codec<TOther> by converting TOther to T and vice versa when T -> TOther always valid.
    public Codec<TOther> Safe2UnsafeMap<TOther>(
        Func<TOther, DataResult<T>> from,
        Func<T, TOther> to
    ) => new Safe2UnsafeMapCodec<T, TOther>(this, to, from);

    /// Creates a Codec<TOther> by converting TOther to T and vice versa when TOther -> T always valid.
    public Codec<TOther> Unsafe2SafeMap<TOther>(
        Func<TOther, T> from,
        Func<T, DataResult<TOther>> to
    ) => new Unsafe2SafeMapCodec<T, TOther>(this, to, from);

    public static Codec<T> Either(Codec<T> first, Codec<T> second) =>
        new EitherCodec<T>(first, second);

    public static Codec<T> Primitive(
        Func<T, IDynamicOps, DataResult<object>> encoder,
        Func<IDynamicOps, object, DataResult<(T, object)>> decoder
    ) => new PrimitiveCodec<T>(encoder, decoder);

    /// <summary>
    /// Creates a codec that decodes to a constant value and does nothing when encoding.
    /// </summary>
    public Codec<T> Constant(T value) => new ConstantCodec<T>(value);
}

public static class CodecExtensions
{
    /// <summary>
    /// Creates a codec that upcasts the targeted type into a less specific one (for example Square -> Shape)
    /// </summary>
    public static Codec<TBase> Upcast<TDer, TBase>(this Codec<TDer> codec)
        where TDer : TBase
    {
        return new UpcastCodec<TBase, TDer>(codec);
    }
}
