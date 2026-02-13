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

public abstract class Codec<T> : IEncoder<T>, IDecoder<T>
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

    public DataResult<TFormat> EncodeSingle<TFormat>(IDynamicOps<TFormat> ops, T input) =>
        Encode(input, ops, ops.Empty());

    public DataResult<T> Parse<TFormat>(IDynamicOps<TFormat> ops, TFormat input) =>
        Decode(ops, input).Map(pair => pair.Item1);

    /// <summary>
    /// Creates a Codec for IEnumerable<T> by recursively applying the existing Codec<T>.
    /// </summary>
    /// <remarks>
    /// Encoding and decoding of all enumerable codec types is best-effort. The process stops on the first error,
    /// and you get the partial result on failure.
    /// </remarks>
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
    public Codec<T> Constant(T value) =>
        new PrimitiveCodec<T>(
            (_, ops) => DataResult<object>.Success(((IDynamicOps<object>)ops).Empty()),
            (ops, input) => DataResult<(T, object)>.Success((value, input))
        );
}
