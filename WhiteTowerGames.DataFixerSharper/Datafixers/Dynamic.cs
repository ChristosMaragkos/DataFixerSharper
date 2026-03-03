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

    public DynamicResult<TFormat> Get(string key)
    {
        var result = Ops.GetValue(Value, key);
        if (result.IsError)
            return DataResult<Dynamic<TFormat>>.Fail(result.ErrorMessage);

        return DataResult<Dynamic<TFormat>>.Success(new Dynamic<TFormat>(Ops, result.GetOrThrow()));
    }

    public DynamicResult<TFormat> Set(string targetKey, Dynamic<TFormat> newValue)
    {
        var state = new MapTransformState
        {
            Map = Ops.CreateEmptyMap(),
            KeyFound = false,
            ErrorState = DataResult<Unit>.Success(default),
        };

        var consumer = new MapKeyAdder(Ops, targetKey, newValue.Value);
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

        state.Map = Ops.FinalizeMap(state.Map);
        return DataResult<Dynamic<TFormat>>.Success(new Dynamic<TFormat>(Ops, state.Map));
    }

    public DynamicResult<TFormat> Remove(string targetKey)
    {
        var state = new MapTransformState
        {
            Map = Ops.CreateEmptyMap(),
            KeyFound = false,
            ErrorState = DataResult<Unit>.Success(default),
        };

        var consumer = new MapKeyRemover(Ops, targetKey);
        var readResult = Ops.ReadMap(Value, ref state, consumer);

        if (readResult.IsError)
            return DataResult<Dynamic<TFormat>>.Success(this);
        if (state.IsError)
            return DataResult<Dynamic<TFormat>>.Fail(state.ErrorMessage);
        if (!state.KeyFound)
            return DataResult<Dynamic<TFormat>>.Success(this);

        state.Map = Ops.FinalizeMap(state.Map);
        return DataResult<Dynamic<TFormat>>.Success(new Dynamic<TFormat>(Ops, state.Map));
    }

    public DynamicResult<TFormat> Rename(string oldKey, string newKey)
    {
        var state = new MapTransformState
        {
            Map = Ops.CreateEmptyMap(),
            KeyFound = false,
            ErrorState = DataResult<Unit>.Success(default),
        };

        var consumer = new MapKeyRenamer(Ops, oldKey, newKey);
        var readResult = Ops.ReadMap(Value, ref state, consumer);

        if (readResult.IsError)
            return DataResult<Dynamic<TFormat>>.Success(this);
        if (state.IsError)
            return DataResult<Dynamic<TFormat>>.Fail(state.ErrorMessage);
        if (!state.KeyFound)
            return DataResult<Dynamic<TFormat>>.Success(this); // no key found, no problem

        state.Map = Ops.FinalizeMap(state.Map);
        return DataResult<Dynamic<TFormat>>.Success(new Dynamic<TFormat>(Ops, state.Map));
    }

    #region Utility Structs
    private readonly struct MapKeyAdder : IMapConsumer<MapTransformState, TFormat>
    {
        public readonly IDynamicOps<TFormat> Ops;
        public readonly string TargetKey;
        public readonly TFormat NewValue;

        public MapKeyAdder(IDynamicOps<TFormat> ops, string targetKey, TFormat newValue)
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

    private readonly struct MapKeyRemover : IMapConsumer<MapTransformState, TFormat>
    {
        public readonly IDynamicOps<TFormat> Ops;
        public readonly string TargetKey;

        public MapKeyRemover(IDynamicOps<TFormat> ops, string targetKey)
        {
            Ops = ops;
            TargetKey = targetKey;
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

            if (keyStrResult.GetOrThrow() == TargetKey)
            {
                map.KeyFound = true;
                return;
            }

            var addResult = Ops.AddToMap(map.Map, key, value);
            if (addResult.IsError)
                map.ErrorState = DataResult<Unit>.Fail(addResult.ErrorMessage);
            else
                map.Map = addResult.GetOrThrow();
        }
    }

    private readonly struct MapKeyRenamer : IMapConsumer<MapTransformState, TFormat>
    {
        public readonly IDynamicOps<TFormat> Ops;
        public readonly string OldKey;
        public readonly string NewKey;

        public MapKeyRenamer(IDynamicOps<TFormat> ops, string oldKey, string newKey)
        {
            Ops = ops;
            OldKey = oldKey;
            NewKey = newKey;
        }

        public void Accept(ref MapTransformState map, TFormat key, TFormat value)
        {
            if (map.IsError)
                return;

            var keyResult = Ops.GetString(key);
            if (keyResult.IsError)
            {
                map.ErrorState = DataResult<Unit>.Fail(keyResult.ErrorMessage);
                return;
            }

            var keyToAdd =
                (keyResult.GetOrThrow() == OldKey && !map.KeyFound)
                    ? CreateNewKey(ref map, Ops, NewKey)
                    : key;
            var addResult = Ops.AddToMap(map.Map, keyToAdd, value);
            if (addResult.IsError)
                map.ErrorState = DataResult<Unit>.Fail(addResult.ErrorMessage);
            else
                map.Map = addResult.GetOrThrow();

            return;

            TFormat CreateNewKey(ref MapTransformState map, IDynamicOps<TFormat> ops, string newKey)
            {
                map.KeyFound = true;
                return ops.CreateString(newKey);
            }
        }
    }

    private readonly struct ListUpdater : ICollectionConsumer<ListTransformState, TFormat>
    {
        public readonly IDynamicOps<TFormat> Ops;
        public readonly Func<Dynamic<TFormat>, DynamicResult<TFormat>> Updater;

        public ListUpdater(
            IDynamicOps<TFormat> ops,
            Func<Dynamic<TFormat>, DynamicResult<TFormat>> updater
        )
        {
            Ops = ops;
            Updater = updater;
        }

        public void Accept(ref ListTransformState list, TFormat item)
        {
            if (list.IsError)
                return;

            var currentDyn = new Dynamic<TFormat>(Ops, item);
            var updatedResult = Updater(currentDyn);

            if (updatedResult.IsError)
            {
                list.ErrorState = DataResult<Unit>.Fail(updatedResult.ErrorMessage);
                return;
            }

            var addResult = Ops.AddToList(list.List, updatedResult.GetOrThrow().Value);
            if (addResult.IsError)
                list.ErrorState = DataResult<Unit>.Fail(addResult.ErrorMessage);
            else
                list.List = addResult.GetOrThrow();
        }
    }

    private readonly struct MapUpdater : IMapConsumer<MapTransformState, TFormat>
    {
        public readonly IDynamicOps<TFormat> Ops;
        public readonly Func<string, Dynamic<TFormat>, DynamicResult<TFormat>> Updater;

        public MapUpdater(
            IDynamicOps<TFormat> ops,
            Func<string, Dynamic<TFormat>, DynamicResult<TFormat>> updater
        )
        {
            Ops = ops;
            Updater = updater;
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

            var keyString = keyStrResult.GetOrThrow();
            var currentDyn = new Dynamic<TFormat>(Ops, value);

            var updatedResult = Updater(keyString, currentDyn);

            if (updatedResult.IsError)
            {
                map.ErrorState = DataResult<Unit>.Fail(updatedResult.ErrorMessage);
                return;
            }

            var addResult = Ops.AddToMap(map.Map, key, updatedResult.GetOrThrow().Value);
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

    private ref struct ListTransformState
    {
        public TFormat List;
        public DataResult<Unit> ErrorState;

        public bool IsError => ErrorState.IsError;
        public string ErrorMessage => ErrorState.ErrorMessage;
    }
    #endregion
}
