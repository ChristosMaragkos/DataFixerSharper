using WhiteTowerGames.DataFixerSharper.Abstractions;

namespace WhiteTowerGames.DataFixerSharper.Codecs;

public interface ICodec
{
    public static ICodec<Dictionary<TKey, TValue>> Dictionary<TKey, TValue>(
        ICodec<TKey> keyCodec,
        ICodec<TValue> valueCodec
    )
        where TKey : notnull => new DictionaryCodec<TKey, TValue>(keyCodec, valueCodec);

    public static ICodec<T> Dispatch<T, TDis>(
        ICodec<TDis> discriminatorCodec,
        Func<T, TDis> discriminatorGetter,
        Func<TDis, ICodec<T>> codecGetter,
        string discriminatorKeyName = "type"
    ) =>
        new DispatchCodec<T, TDis>(
            discriminatorGetter,
            discriminatorCodec,
            codecGetter,
            discriminatorKeyName
        );

    public static ICodec<T> Either<T>(ICodec<T> first, ICodec<T> second) =>
        new EitherCodec<T>(first, second);
}

public interface ICodec<T> : ICodec
{
    DataResult<(T, TFormat)> Decode<TOps, TFormat>(TOps ops, TFormat input)
        where TOps : IDynamicOps<TFormat>;

    DataResult<TFormat> Encode<TOps, TFormat>(T input, TOps ops, TFormat prefix)
        where TOps : IDynamicOps<TFormat>;
}

public static class CodecExtensions
{
    /// <summary>
    /// Creates a codec that upcasts the targeted type into a less specific one (for example Square -> Shape)
    /// </summary>
    public static ICodec<TBase> Upcast<TDer, TBase>(this ICodec<TDer> codec)
        where TDer : TBase
    {
        return new UpcastCodec<TBase, TDer>(codec);
    }

    public static DataResult<TFormat> EncodeStart<T, TOps, TFormat>(
        this ICodec<T> codec,
        TOps ops,
        T input
    )
        where TOps : IDynamicOps<TFormat> => codec.Encode(input, ops, ops.Empty());

    public static DataResult<T> Parse<T, TOps, TFormat>(
        this ICodec<T> codec,
        TOps ops,
        TFormat input
    )
        where TOps : IDynamicOps<TFormat> => codec.Decode(ops, input).Map(pair => pair.Item1);

    public static ICodec<List<T>> ForList<T>(this ICodec<T> codec) => new ListCodec<T>(codec);

    /// Creates a Codec<TOther> by converting TOther to T and vice versa when T <-> TOther always valid.
    public static ICodec<TOther> SafeMap<T, TOther>(
        this ICodec<T> codec,
        Func<TOther, T> from,
        Func<T, TOther> to
    ) => new SafeMapCodec<T, TOther>(codec, to, from);

    /// Creates a Codec<TOther> by converting TOther to T and vice versa when T <-> TOther not always valid.
    public static ICodec<TOther> UnsafeMap<T, TOther>(
        this ICodec<T> codec,
        Func<TOther, DataResult<T>> from,
        Func<T, DataResult<TOther>> to
    ) => new UnsafeMapCodec<T, TOther>(codec, to, from);

    /// Creates a Codec<TOther> by converting TOther to T and vice versa when T -> TOther always valid.
    public static ICodec<TOther> Safe2UnsafeMap<T, TOther>(
        this ICodec<T> codec,
        Func<TOther, DataResult<T>> from,
        Func<T, TOther> to
    ) => new Safe2UnsafeMapCodec<T, TOther>(codec, to, from);

    /// Creates a Codec<TOther> by converting TOther to T and vice versa when TOther -> T always valid.
    public static ICodec<TOther> Unsafe2SafeMap<T, TOther>(
        this ICodec<T> codec,
        Func<TOther, T> from,
        Func<T, DataResult<TOther>> to
    ) => new Unsafe2SafeMapCodec<T, TOther>(codec, to, from);

    /// <summary>
    /// Creates a codec that decodes to a constant value and does nothing when encoding.
    /// </summary>
    public static ICodec<T> Constant<T>(this ICodec<T> codec, T value) =>
        new ConstantCodec<T>(value);
}
