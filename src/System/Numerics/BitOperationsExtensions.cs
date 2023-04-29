namespace System.Numerics;

/// <summary>
/// Provides extension methods on <see cref="BitOperations"/>.
/// </summary>
/// <seealso cref="BitOperations"/>
public static partial class BitOperationsExtensions
{
	public static partial Bits GetAllSets(this sbyte @this);
	public static partial Bits GetAllSets(this byte @this);
	public static partial Bits GetAllSets(this short @this);
	public static partial Bits GetAllSets(this ushort @this);
	public static partial Bits GetAllSets(this int @this);
	public static partial Bits GetAllSets(this uint @this);
	public static partial Bits GetAllSets(this long @this);
	public static partial Bits GetAllSets(this ulong @this);
	public static unsafe partial Bits GetAllSets(this nint @this);
	public static unsafe partial Bits GetAllSets(this nuint @this);

	public static partial BitEnumerator GetEnumerator(this sbyte @this);
	public static partial BitEnumerator GetEnumerator(this byte @this);
	public static partial BitEnumerator GetEnumerator(this short @this);
	public static partial BitEnumerator GetEnumerator(this ushort @this);
	public static partial BitEnumerator GetEnumerator(this int @this);
	public static partial BitEnumerator GetEnumerator(this uint @this);
	public static partial BitEnumerator GetEnumerator(this long @this);
	public static partial BitEnumerator GetEnumerator(this ulong @this);
	public static unsafe partial BitEnumerator GetEnumerator(this nint @this);
	public static unsafe partial BitEnumerator GetEnumerator(this nuint @this);

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
