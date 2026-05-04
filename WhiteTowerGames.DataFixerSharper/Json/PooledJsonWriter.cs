using System.Buffers;

namespace WhiteTowerGames.DataFixerSharper.Json;

internal sealed class PooledJsonWriter
{
    private byte[] _buffer;
    private int _written;

    private const int InitialCapacity = 128;

    public PooledJsonWriter()
    {
        _buffer = ArrayPool<byte>.Shared.Rent(InitialCapacity);
        _written = 0;
    }

    public ReadOnlySpan<byte> WrittenSpan => _buffer.AsSpan(0, _written);

    public PooledJsonWriter Reset()
    {
        _written = 0;
        return this;
    }

    public void Write(ReadOnlySpan<byte> data)
    {
        EnsureCapacity(data.Length);
        data.CopyTo(_buffer.AsSpan(_written));
        _written += data.Length;
    }

    private void EnsureCapacity(int needed)
    {
        if (_written + needed <= _buffer.Length)
            return;

        var newSize = Math.Max(_buffer.Length * 2, _written + needed);
        var newBuffer = ArrayPool<byte>.Shared.Rent(newSize);
        _buffer.AsSpan(0, _written).CopyTo(newBuffer);
        ArrayPool<byte>.Shared.Return(_buffer);
        _buffer = newBuffer;
    }

    public void ReturnToPool() => ArrayPool<byte>.Shared.Return(_buffer);
}
