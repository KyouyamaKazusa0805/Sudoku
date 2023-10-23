namespace System.Numerics;

/// <summary>
/// Provides extension methods on <see cref="BitOperations"/>.
/// </summary>
/// <seealso cref="BitOperations"/>
public static partial class BitOperationsExtensions
{
	public static partial ReadOnlySpan<int> GetAllSets(this sbyte @this);
	public static partial ReadOnlySpan<int> GetAllSets(this byte @this);
	public static partial ReadOnlySpan<int> GetAllSets(this short @this);
	public static partial ReadOnlySpan<int> GetAllSets(this ushort @this);
	public static partial ReadOnlySpan<int> GetAllSets(this int @this);
	public static partial ReadOnlySpan<int> GetAllSets(this uint @this);
	public static partial ReadOnlySpan<int> GetAllSets(this long @this);
	public static partial ReadOnlySpan<int> GetAllSets(this ulong @this);
	public static unsafe partial ReadOnlySpan<int> GetAllSets(this nint @this);
	public static unsafe partial ReadOnlySpan<int> GetAllSets(this nuint @this);

	public static partial ReadOnlySpan<int>.Enumerator GetEnumerator(this sbyte @this);
	public static partial ReadOnlySpan<int>.Enumerator GetEnumerator(this byte @this);
	public static partial ReadOnlySpan<int>.Enumerator GetEnumerator(this short @this);
	public static partial ReadOnlySpan<int>.Enumerator GetEnumerator(this ushort @this);
	public static partial ReadOnlySpan<int>.Enumerator GetEnumerator(this int @this);
	public static partial ReadOnlySpan<int>.Enumerator GetEnumerator(this uint @this);
	public static partial ReadOnlySpan<int>.Enumerator GetEnumerator(this long @this);
	public static partial ReadOnlySpan<int>.Enumerator GetEnumerator(this ulong @this);
	public static unsafe partial ReadOnlySpan<int>.Enumerator GetEnumerator(this nint @this);
	public static unsafe partial ReadOnlySpan<int>.Enumerator GetEnumerator(this nuint @this);

	public static partial int GetNextSet(this byte @this, int index);
	public static partial int GetNextSet(this short @this, int index);
	public static partial int GetNextSet(this int @this, int index);
	public static partial int GetNextSet(this long @this, int index);

	public static partial void ReverseBits(this scoped ref byte @this);
	public static partial void ReverseBits(this scoped ref short @this);
	public static partial void ReverseBits(this scoped ref int @this);
	public static partial void ReverseBits(this scoped ref long @this);

	public static partial int SetAt(this byte @this, int order);
	public static partial int SetAt(this short @this, int order);
	public static partial int SetAt(this int @this, int order);
	public static partial int SetAt(this long @this, int order);

	public static partial byte SkipSetBit(this byte @this, int setBitPosCount);
	public static partial short SkipSetBit(this short @this, int setBitPosCount);
	public static partial int SkipSetBit(this int @this, int setBitPosCount);
	public static partial long SkipSetBit(this long @this, int setBitPosCount);
}
