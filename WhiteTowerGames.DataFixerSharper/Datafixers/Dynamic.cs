using WhiteTowerGames.DataFixerSharper.Abstractions;

namespace WhiteTowerGames.DataFixerSharper.Datafixers;

public readonly struct Dynamic<TOps, TFormat>
    where TOps : IDynamicOps<TFormat>
    where TFormat : struct
{
    public readonly TFormat Value;

    private readonly Func<TFormat, DynamicResult<TOps, TFormat>>? _rebuilder;

    public Dynamic(TFormat value)
    {
        Value = value;
        _rebuilder = null;
    }

    internal Dynamic(
        TFormat value,
        Func<TFormat, DynamicResult<TOps, TFormat>> rebuilder
    )
    {
        Value = value;
        _rebuilder = rebuilder;
    }

    public DynamicResult<TOps, TFormat> Get(string key)
    {
        var result = TOps.GetValue(Value, key);
        if (result.IsError)
            return DataResult<Dynamic<TOps, TFormat>>.Fail(result.ErrorMessage);

        var self = this;

        return DataResult<Dynamic<TOps, TFormat>>.Success(
            new Dynamic<TOps, TFormat>(result.GetOrThrow(), child => self.Set(key, child))
        );
    }

    public DynamicResult<TOps, TFormat> Set(string targetKey, TFormat newValue)
    {
        var state = new MapTransformState
        {
            Map = TOps.CreateEmptyMap(),
            KeyFound = false,
            ErrorState = DataResult<Unit>.Success(default),
        };

        var consumer = new MapKeyAdder(targetKey, newValue);
        var readResult = TOps.ReadMap(Value, ref state, consumer);

        if (readResult.IsError)
            return DataResult<Dynamic<TOps, TFormat>>.Success(this);
        if (state.IsError)
            return DataResult<Dynamic<TOps, TFormat>>.Fail(state.ErrorMessage);

        if (!state.KeyFound)
        {
            var keyFormat = TOps.CreateString(targetKey);
            var addResult = TOps.AddToMap(state.Map, keyFormat, newValue);

            if (addResult.IsError)
                return DataResult<Dynamic<TOps, TFormat>>.Fail(addResult.ErrorMessage);

            state.Map = addResult.GetOrThrow();
        }

        state.Map = TOps.FinalizeMap(state.Map);

        var newFormat = state.Map;
        if (_rebuilder is not null)
        {
            return _rebuilder(newFormat);
        }

        return DataResult<Dynamic<TOps, TFormat>>.Success(new Dynamic<TOps, TFormat>(state.Map));
    }

    public DynamicResult<TOps, TFormat> Remove(string targetKey)
    {
        var state = new MapTransformState
        {
            Map = TOps.CreateEmptyMap(),
            KeyFound = false,
            ErrorState = DataResult<Unit>.Success(default),
        };

        var consumer = new MapKeyRemover(targetKey);
        var readResult = TOps.ReadMap(Value, ref state, consumer);

        if (readResult.IsError)
            return DataResult<Dynamic<TOps, TFormat>>.Success(this);
        if (state.IsError)
            return DataResult<Dynamic<TOps, TFormat>>.Fail(state.ErrorMessage);
        if (!state.KeyFound)
            return DataResult<Dynamic<TOps, TFormat>>.Success(this);

        state.Map = TOps.FinalizeMap(state.Map);

        var newFormat = state.Map;
        if (_rebuilder is not null)
        {
            return _rebuilder(newFormat);
        }

        return DataResult<Dynamic<TOps, TFormat>>.Success(new Dynamic<TOps, TFormat>(state.Map));
    }

    public DynamicResult<TOps, TFormat> Rename(string oldKey, string newKey)
    {
        var state = new MapTransformState
        {
            Map = TOps.CreateEmptyMap(),
            KeyFound = false,
            ErrorState = DataResult<Unit>.Success(default),
        };

        var consumer = new MapKeyRenamer(oldKey, newKey);
        var readResult = TOps.ReadMap(Value, ref state, consumer);

        if (readResult.IsError)
            return DataResult<Dynamic<TOps, TFormat>>.Success(this);
        if (state.IsError)
            return DataResult<Dynamic<TOps, TFormat>>.Fail(state.ErrorMessage);
        if (!state.KeyFound)
            return DataResult<Dynamic<TOps, TFormat>>.Success(this);

        state.Map = TOps.FinalizeMap(state.Map);

        var newFormat = state.Map;
        if (_rebuilder is not null)
        {
            return _rebuilder(newFormat);
        }

        return DataResult<Dynamic<TOps, TFormat>>.Success(new Dynamic<TOps, TFormat>(state.Map));
    }

    public DynamicResult<TOps, TFormat> UpdateList(
        Func<Dynamic<TOps, TFormat>, DynamicResult<TOps, TFormat>> itemUpdater
    )
    {
        var state = new ListTransformState
        {
            List = TOps.CreateEmptyList(),
            ErrorState = DataResult<Unit>.Success(default),
        };

        var consumer = new ListUpdater(itemUpdater);
        var readResult = TOps.ReadList(Value, ref state, consumer);

        if (readResult.IsError)
            return DataResult<Dynamic<TOps, TFormat>>.Success(this);
        if (state.IsError)
            return DataResult<Dynamic<TOps, TFormat>>.Fail(state.ErrorMessage);

        state.List = TOps.FinalizeList(state.List);
        return DataResult<Dynamic<TOps, TFormat>>.Success(new Dynamic<TOps, TFormat>(state.List));
    }

    public DynamicResult<TOps, TFormat> UpdateMap(
        Func<string, Dynamic<TOps, TFormat>, DynamicResult<TOps, TFormat>> fieldUpdater
    )
    {
        var state = new MapTransformState
        {
            Map = TOps.CreateEmptyMap(),
            KeyFound = false,
            ErrorState = DataResult<Unit>.Success(default),
        };

        var consumer = new MapUpdater(fieldUpdater);
        var readResult = TOps.ReadMap(Value, ref state, consumer);

        if (readResult.IsError)
            return DataResult<Dynamic<TOps, TFormat>>.Success(this);
        if (state.IsError)
            return DataResult<Dynamic<TOps, TFormat>>.Fail(state.ErrorMessage);

        state.Map = TOps.FinalizeMap(state.Map);
        return DataResult<Dynamic<TOps, TFormat>>.Success(new Dynamic<TOps, TFormat>(state.Map));
    }

    #region Utility Structs
    private readonly struct MapKeyAdder : IMapConsumer<MapTransformState, TFormat>
    {
        public readonly string TargetKey;
        public readonly TFormat NewValue;

        public MapKeyAdder(string targetKey, TFormat newValue)
        {
            TargetKey = targetKey;
            NewValue = newValue;
        }

        public void Accept(ref MapTransformState map, TFormat key, TFormat value)
        {
            if (map.IsError)
                return;

            var keyStrResult = TOps.GetString(key);
            if (keyStrResult.IsError)
            {
                map.ErrorState = DataResult<Unit>.Fail(keyStrResult.ErrorMessage);
                return;
            }

            var isTarget = keyStrResult.GetOrThrow() == TargetKey;
            var valueToWrite = isTarget ? NewValue : value;

            if (isTarget)
                map.KeyFound = true;

            var addResult = TOps.AddToMap(map.Map, key, valueToWrite);
            if (addResult.IsError)
                map.ErrorState = DataResult<Unit>.Fail(addResult.ErrorMessage);
            else
                map.Map = addResult.GetOrThrow();
        }
    }

    private readonly struct MapKeyRemover : IMapConsumer<MapTransformState, TFormat>
    {
        public readonly string TargetKey;

        public MapKeyRemover(string targetKey)
        {
            TargetKey = targetKey;
        }

        public void Accept(ref MapTransformState map, TFormat key, TFormat value)
        {
            if (map.IsError)
                return;

            var keyStrResult = TOps.GetString(key);
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

            var addResult = TOps.AddToMap(map.Map, key, value);
            if (addResult.IsError)
                map.ErrorState = DataResult<Unit>.Fail(addResult.ErrorMessage);
            else
                map.Map = addResult.GetOrThrow();
        }
    }

    private readonly struct MapKeyRenamer : IMapConsumer<MapTransformState, TFormat>
    {
        public readonly string OldKey;
        public readonly string NewKey;

        public MapKeyRenamer(string oldKey, string newKey)
        {
            OldKey = oldKey;
            NewKey = newKey;
        }

        public void Accept(ref MapTransformState map, TFormat key, TFormat value)
        {
            if (map.IsError)
                return;

            var keyResult = TOps.GetString(key);
            if (keyResult.IsError)
            {
                map.ErrorState = DataResult<Unit>.Fail(keyResult.ErrorMessage);
                return;
            }

            var keyToAdd =
                (keyResult.GetOrThrow() == OldKey && !map.KeyFound)
                    ? CreateNewKey(ref map, NewKey)
                    : key;
            var addResult = TOps.AddToMap(map.Map, keyToAdd, value);
            if (addResult.IsError)
                map.ErrorState = DataResult<Unit>.Fail(addResult.ErrorMessage);
            else
                map.Map = addResult.GetOrThrow();

            return;

            static TFormat CreateNewKey(
                ref MapTransformState map,
                string newKey
            )
            {
                map.KeyFound = true;
                return TOps.CreateString(newKey);
            }
        }
    }

    private readonly struct ListUpdater : ICollectionConsumer<ListTransformState, TFormat>
    {
        public readonly Func<Dynamic<TOps, TFormat>, DynamicResult<TOps, TFormat>> Updater;

        public ListUpdater(
            Func<Dynamic<TOps, TFormat>, DynamicResult<TOps, TFormat>> updater
        )
        {
            Updater = updater;
        }

        public void Accept(ref ListTransformState list, TFormat item)
        {
            if (list.IsError)
                return;

            var currentDyn = new Dynamic<TOps, TFormat>(item);
            var updatedResult = Updater(currentDyn);

            if (updatedResult.IsError)
            {
                list.ErrorState = DataResult<Unit>.Fail(updatedResult.ErrorMessage);
                return;
            }

            var addResult = TOps.AddToList(list.List, updatedResult.GetOrThrow().Value);
            if (addResult.IsError)
                list.ErrorState = DataResult<Unit>.Fail(addResult.ErrorMessage);
            else
                list.List = addResult.GetOrThrow();
        }
    }

    private readonly struct MapUpdater : IMapConsumer<MapTransformState, TFormat>
    {
        public readonly Func<string, Dynamic<TOps, TFormat>, DynamicResult<TOps, TFormat>> Updater;

        public MapUpdater(
            Func<string, Dynamic<TOps, TFormat>, DynamicResult<TOps, TFormat>> updater
        )
        {
            Updater = updater;
        }

        public void Accept(ref MapTransformState map, TFormat key, TFormat value)
        {
            if (map.IsError)
                return;

            var keyStrResult = TOps.GetString(key);
            if (keyStrResult.IsError)
            {
                map.ErrorState = DataResult<Unit>.Fail(keyStrResult.ErrorMessage);
                return;
            }

            var keyString = keyStrResult.GetOrThrow();
            var currentDyn = new Dynamic<TOps, TFormat>(value);

            var updatedResult = Updater(keyString, currentDyn);

            if (updatedResult.IsError)
            {
                map.ErrorState = DataResult<Unit>.Fail(updatedResult.ErrorMessage);
                return;
            }

            var addResult = TOps.AddToMap(map.Map, key, updatedResult.GetOrThrow().Value);
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

        public readonly bool IsError => ErrorState.IsError;
        public readonly string ErrorMessage => ErrorState.ErrorMessage;
    }

    private ref struct ListTransformState
    {
        public TFormat List;
        public DataResult<Unit> ErrorState;

        public readonly bool IsError => ErrorState.IsError;
        public readonly string ErrorMessage => ErrorState.ErrorMessage;
    }
    #endregion
}
