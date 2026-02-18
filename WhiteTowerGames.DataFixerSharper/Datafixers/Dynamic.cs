using WhiteTowerGames.DataFixerSharper.Abstractions;

namespace WhiteTowerGames.DataFixerSharper.Datafixers;

public readonly struct Dynamic<TFormat>
{
    public readonly IDynamicOps<TFormat> Ops;
    public readonly TFormat Value;

    public Dynamic(IDynamicOps<TFormat> ops, TFormat value)
    {
        Ops = ops;
        Value = value;
    }

    public DataResult<Dynamic<TFormat>> Get(string key)
    {
        var result = Ops.GetValue(Value, key);
        if (result.IsError)
            return DataResult<Dynamic<TFormat>>.Fail(result.ErrorMessage);

        return DataResult<Dynamic<TFormat>>.Success(new Dynamic<TFormat>(Ops, result.GetOrThrow()));
    }

    public DataResult<Dynamic<TFormat>> Set(string targetKey, Dynamic<TFormat> newValue)
    {
        var state = new MapTransformState
        {
            Map = Ops.CreateEmptyMap(),
            KeyFound = false,
            ErrorState = DataResult<Unit>.Success(default),
        };

        var consumer = new MapTransformer(Ops, targetKey, newValue.Value);
        var readResult = Ops.ReadMap(Value, ref state, consumer);

        if (readResult.IsError)
            return DataResult<Dynamic<TFormat>>.Fail(readResult.ErrorMessage);
        if (state.IsError)
            return DataResult<Dynamic<TFormat>>.Fail(state.ErrorMessage);

        if (!state.KeyFound)
        {
            var keyFormat = Ops.CreateString(targetKey);
            var addResult = Ops.AddToMap(state.Map, keyFormat, newValue.Value);

            if (addResult.IsError)
                return DataResult<Dynamic<TFormat>>.Fail(addResult.ErrorMessage);

            state.Map = addResult.GetOrThrow();
        }

        return DataResult<Dynamic<TFormat>>.Success(new Dynamic<TFormat>(Ops, state.Map));
    }

    private readonly struct MapTransformer : IMapConsumer<MapTransformState, TFormat>
    {
        public readonly IDynamicOps<TFormat> Ops;
        public readonly string TargetKey;
        public readonly TFormat NewValue;

        public MapTransformer(IDynamicOps<TFormat> ops, string targetKey, TFormat newValue)
        {
            Ops = ops;
            TargetKey = targetKey;
            NewValue = newValue;
        }

        public void Accept(ref MapTransformState map, TFormat key, TFormat value)
        {
            if (map.IsError)
                return;

            var keyStrResult = Ops.GetString(key);
            if (keyStrResult.IsError)
            {
                map.ErrorState = DataResult<Unit>.Fail(keyStrResult.ErrorMessage);
                return;
            }

            var isTarget = keyStrResult.GetOrThrow() == TargetKey;
            var valueToWrite = isTarget ? NewValue : value;

            if (isTarget)
                map.KeyFound = true;

            var addResult = Ops.AddToMap(map.Map, key, valueToWrite);
            if (addResult.IsError)
                map.ErrorState = DataResult<Unit>.Fail(addResult.ErrorMessage);
            else
                map.Map = addResult.GetOrThrow();
        }
    }

    private ref struct MapTransformState
    {
        public TFormat Map;
        public bool KeyFound;
        public DataResult<Unit> ErrorState;

        public bool IsError => ErrorState.IsError;
        public string ErrorMessage => ErrorState.ErrorMessage;
    }
}
