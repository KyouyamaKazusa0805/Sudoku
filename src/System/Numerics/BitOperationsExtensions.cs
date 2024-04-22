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
	public static partial ReadOnlySpan<int> GetAllSets(this nint @this);
	public static partial ReadOnlySpan<int> GetAllSets(this nuint @this);

	public static partial Int32Enumerator GetEnumerator(this sbyte @this);
	public static partial Int32Enumerator GetEnumerator(this byte @this);
	public static partial Int32Enumerator GetEnumerator(this short @this);
	public static partial Int32Enumerator GetEnumerator(this ushort @this);
	public static partial Int32Enumerator GetEnumerator(this int @this);
	public static partial Int32Enumerator GetEnumerator(this uint @this);
	public static partial Int64Enumerator GetEnumerator(this long @this);
	public static partial Int64Enumerator GetEnumerator(this ulong @this);
	public static partial NativeIntegerEnumerator GetEnumerator(this nint @this);
	public static partial NativeIntegerEnumerator GetEnumerator(this nuint @this);

	public static partial int GetNextSet(this byte @this, int index);
	public static partial int GetNextSet(this short @this, int index);
	public static partial int GetNextSet(this int @this, int index);
	public static partial int GetNextSet(this long @this, int index);

	public static partial void ReverseBits(this ref byte @this);
	public static partial void ReverseBits(this ref short @this);
	public static partial void ReverseBits(this ref int @this);
	public static partial void ReverseBits(this ref long @this);

	public static partial int SetAt(this sbyte @this, int order);
	public static partial int SetAt(this byte @this, int order);
	public static partial int SetAt(this short @this, int order);
	public static partial int SetAt(this ushort @this, int order);
	public static partial int SetAt(this int @this, int order);
	public static partial int SetAt(this uint @this, int order);
	public static partial int SetAt(this long @this, int order);
	public static partial int SetAt(this ulong @this, int order);
	public static partial int SetAt(this nint @this, int order);
	public static partial int SetAt(this nuint @this, int order);

	public static partial byte SkipSetBit(this byte @this, int setBitPosCount);
	public static partial short SkipSetBit(this short @this, int setBitPosCount);
	public static partial int SkipSetBit(this int @this, int setBitPosCount);
	public static partial long SkipSetBit(this long @this, int setBitPosCount);
}
