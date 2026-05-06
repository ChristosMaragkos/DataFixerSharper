using System.Text;
using WhiteTowerGames.DataFixerSharper.Codecs;

namespace WhiteTowerGames.DataFixerSharper.Extensions.ExtraCodecs;

public static class CipherCodecs
{
    /// <summary>
    /// Returns an <see cref="ICodec{int}"/> which XORs its input when encoding and its output when decoding with <paramref name="seed"/>.
    /// Useful for preventing people from fiddling with sensitive data like game save data.
    /// </summary>
    public static ICodec<int> Int32(int seed)
    {
        return BuiltinCodecs.Int32.SafeMap(i32 => i32 ^ seed, i32 => i32 ^ seed);
    }

    /// <summary>
    /// Returns an <see cref = "ICodec{string}"/> which XORs every byte in its input with the given <paramref name="key"/>
    /// and converting to a base 64 string when encoding, and reverses that process when decoding.
    /// Relatively simple; if you prefer something more secure, use <see cref="StringMultiByte"/>.
    /// </summary>
    public static ICodec<string> String(byte key)
    {
        return BuiltinCodecs.String.SafeMap(
            plaintext =>
            {
                var bytes = Encoding.UTF8.GetBytes(plaintext);
                for (int i = 0; i < bytes.Length; i++)
                {
                    bytes[i] ^= key;
                }
                return Convert.ToBase64String(bytes);
            },
            base64 =>
            {
                var bytes = Convert.FromBase64String(base64);
                for (int i = 0; i < bytes.Length; i++)
                {
                    bytes[i] ^= key;
                }
                return Encoding.UTF8.GetString(bytes);
            }
        );
    }

    /// <summary>
    /// Returns an <see cref = "ICodec{string}"/> which loops through its <paramref name="key"/> to XOR each byte in its input
    /// with a byte from it, and converting to a base 64 string, and reverses that process when decoding.
    /// Good luck getting this to stop spitting garbgage.
    /// </summary>
    public static ICodec<string> StringMultiByte(params byte[] key)
    {
        return BuiltinCodecs.String.SafeMap(
            plaintext =>
            {
                var bytes = Encoding.UTF8.GetBytes(plaintext);
                for (int i = 0; i < bytes.Length; i++)
                {
                    bytes[i] ^= key[i % key.Length];
                }
                return Convert.ToBase64String(bytes);
            },
            base64 =>
            {
                var bytes = Convert.FromBase64String(base64);
                for (int i = 0; i < bytes.Length; i++)
                {
                    bytes[i] ^= key[i % key.Length];
                }
                return Encoding.UTF8.GetString(bytes);
            }
        );
    }
}
