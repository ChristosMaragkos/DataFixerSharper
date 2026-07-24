using System.Buffers;
using System.Runtime.InteropServices;
using System.Text;

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

    public void Reset()
    {
        _written = 0;
    }

    public void Write(ReadOnlySpan<byte> data)
    {
        EnsureCapacity(data.Length);
        data.CopyTo(_buffer.AsSpan(_written));
        _written += data.Length;
    }

    public void Write(byte b)
    {
        EnsureCapacity(1);
        _buffer[_written++] = b;
    }

    public void Write(ReadOnlySpan<char> text)
    {
        var byteCount = Encoding.UTF8.GetByteCount(text);
        EnsureCapacity(byteCount);
        var written = Encoding.UTF8.GetBytes(text, _buffer.AsSpan(_written));
        _written += written;
    }

    public void WriteEscapedJsonString(ReadOnlySpan<char> value)
    {
        Write((byte)'"');
        Span<byte> utf8Buf = stackalloc byte[4];
        foreach (var ch in value)
        {
            if (ch < 0x20)
            {
                switch (ch)
                {
                    case '\n': Write((byte)'\\'); Write((byte)'n'); break;
                    case '\r': Write((byte)'\\'); Write((byte)'r'); break;
                    case '\t': Write((byte)'\\'); Write((byte)'t'); break;
                    case '\b': Write((byte)'\\'); Write((byte)'b'); break;
                    case '\f': Write((byte)'\\'); Write((byte)'f'); break;
                    default:
                        Write((byte)'\\'); Write((byte)'u'); Write((byte)'0'); Write((byte)'0');
                        Write(HexDigit(ch >> 4));
                        Write(HexDigit(ch & 0xF));
                        break;
                }
            }
            else switch (ch)
            {
                case '"': Write((byte)'\\'); Write((byte)'"'); break;
                case '\\': Write((byte)'\\'); Write((byte)'\\'); break;
                default:
                    if (ch < 0x80)
                    {
                        Write((byte)ch);
                    }
                    else
                    {
                        var c = ch;
                        var charSpan = MemoryMarshal.CreateReadOnlySpan(ref c, 1);
                        var written = Encoding.UTF8.GetBytes(charSpan, utf8Buf);
                        Write(utf8Buf[..written]);
                    }
                    break;
            }
        }
        Write((byte)'"');
    }

    private static byte HexDigit(int val) => (byte)(val < 10 ? '0' + val : 'a' + val - 10);

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
