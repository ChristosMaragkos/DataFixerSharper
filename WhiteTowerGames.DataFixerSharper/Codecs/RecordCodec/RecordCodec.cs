using WhiteTowerGames.DataFixerSharper.Abstractions;

namespace WhiteTowerGames.DataFixerSharper.Codecs.RecordCodec;

public static class RecordCodecBuilder
{
    public static Codec<T> Create<T>(Func<Instance<T>, IFieldMapper<T>> builder)
    {
        var instance = new Instance<T>();
        var ctor = builder(instance);
        return new RecordCodec<T>(ctor);
    }

    private class RecordCodec<T> : Codec<T>
    {
        private readonly IFieldMapper<T> _ctor;

        public RecordCodec(IFieldMapper<T> ctor)
        {
            _ctor = ctor;
        }

        public override DataResult<(T, TFormat)> Decode<TOps, TFormat>(TOps ops, TFormat input)
        {
            return _ctor.Decode(ops, input);
        }

        public override DataResult<TFormat> Encode<TOps, TFormat>(T input, TOps ops, TFormat prefix)
        {
            return _ctor.Encode(input, ops, prefix);
        }
    }
}
