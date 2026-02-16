using WhiteTowerGames.DataFixerSharper.Abstractions;

namespace WhiteTowerGames.DataFixerSharper.Codecs;

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

public abstract class Codec<T> : Codec
{
    public abstract DataResult<(T, TFormat)> Decode<TOps, TFormat>(TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat>;

    public abstract DataResult<TFormat> Encode<TOps, TFormat>(T input, TOps ops, TFormat prefix)
        where TOps : IDynamicOps<TFormat>;

    public DataResult<TFormat> EncodeStart<TOps, TFormat>(TOps ops, T input)
        where TOps : IDynamicOps<TFormat> => Encode(input, ops, ops.Empty());

    public DataResult<T> Parse<TOps, TFormat>(TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat> => Decode(ops, input).Map(pair => pair.Item1);

    public Codec<List<T>> ForList() => new ListCodec<T>(this);

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
