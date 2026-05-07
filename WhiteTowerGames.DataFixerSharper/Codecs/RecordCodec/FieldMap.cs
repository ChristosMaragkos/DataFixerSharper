using System.Diagnostics.CodeAnalysis;
using WhiteTowerGames.DataFixerSharper.Abstractions;

namespace WhiteTowerGames.DataFixerSharper.Codecs.RecordCodec;

internal readonly struct FieldMapEntry<TFormat>
{
    public readonly TFormat Key;
    public readonly TFormat Value;

    public FieldMapEntry(TFormat key, TFormat value)
    {
        Key = key;
        Value = value;
    }
}

public ref struct FieldMap<TFormat>
{
    private FieldMapEntry<TFormat> _e0,
        _e1,
        _e2,
        _e3,
        _e4,
        _e5,
        _e6,
        _e7,
        _e8,
        _e9,
        _e10,
        _e11,
        _e12,
        _e13,
        _e14,
        _e15;
    private int _count;

    public void Add(TFormat key, TFormat value)
    {
        switch (_count)
        {
            case 0:
                _e0 = new(key, value);
                break;
            case 1:
                _e1 = new(key, value);
                break;
            case 2:
                _e2 = new(key, value);
                break;
            case 3:
                _e3 = new(key, value);
                break;
            case 4:
                _e4 = new(key, value);
                break;
            case 5:
                _e5 = new(key, value);
                break;
            case 6:
                _e6 = new(key, value);
                break;
            case 7:
                _e7 = new(key, value);
                break;
            case 8:
                _e8 = new(key, value);
                break;
            case 9:
                _e9 = new(key, value);
                break;
            case 10:
                _e10 = new(key, value);
                break;
            case 11:
                _e11 = new(key, value);
                break;
            case 12:
                _e12 = new(key, value);
                break;
            case 13:
                _e13 = new(key, value);
                break;
            case 14:
                _e14 = new(key, value);
                break;
            case 15:
                _e15 = new(key, value);
                break;
        }
        _count++;
    }

    public bool TryGet<TOps>(TOps ops, string targetKey, out TFormat value)
        where TOps : IDynamicOps<TFormat>
    {
        for (var i = 0; i < _count; i++)
        {
            ref readonly var entry = ref GetEntry(i);

            if (ops.StringsMatch(entry.Key, targetKey))
            {
                value = entry.Value;
                return true;
            }
        }

        value = default!; // default is fine here, the decoder will propagate a nice error
        return false;
    }

    [UnscopedRef]
    private ref FieldMapEntry<TFormat> GetEntry(int index)
    {
        switch (index)
        {
            case 0:
                return ref _e0;
            case 1:
                return ref _e1;
            case 2:
                return ref _e2;
            case 3:
                return ref _e3;
            case 4:
                return ref _e4;
            case 5:
                return ref _e5;
            case 6:
                return ref _e6;
            case 7:
                return ref _e7;
            case 8:
                return ref _e8;
            case 9:
                return ref _e9;
            case 10:
                return ref _e10;
            case 11:
                return ref _e11;
            case 12:
                return ref _e12;
            case 13:
                return ref _e13;
            case 14:
                return ref _e14;
            default:
                return ref _e15;
        }
    }
}

internal readonly struct FieldMapConsumer<TOps, TFormat> : IMapConsumer<FieldMap<TFormat>, TFormat>
    where TOps : IDynamicOps<TFormat>
{
    public void Accept(ref FieldMap<TFormat> state, TFormat key, TFormat value)
    {
        state.Add(key, value);
    }
}
