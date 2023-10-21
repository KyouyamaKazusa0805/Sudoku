namespace System.Numerics;

/// <summary>
/// Provides extension methods on <see cref="BitOperations"/>.
/// </summary>
/// <seealso cref="BitOperations"/>
public static partial class BitOperationsExtensions
{
	public static partial ReadOnlySpan<Offset> GetAllSets(this sbyte @this);
	public static partial ReadOnlySpan<Offset> GetAllSets(this byte @this);
	public static partial ReadOnlySpan<Offset> GetAllSets(this short @this);
	public static partial ReadOnlySpan<Offset> GetAllSets(this ushort @this);
	public static partial ReadOnlySpan<Offset> GetAllSets(this int @this);
	public static partial ReadOnlySpan<Offset> GetAllSets(this uint @this);
	public static partial ReadOnlySpan<Offset> GetAllSets(this long @this);
	public static partial ReadOnlySpan<Offset> GetAllSets(this ulong @this);
	public static unsafe partial ReadOnlySpan<Offset> GetAllSets(this nint @this);
	public static unsafe partial ReadOnlySpan<Offset> GetAllSets(this nuint @this);

	public static partial ReadOnlySpan<Offset>.Enumerator GetEnumerator(this sbyte @this);
	public static partial ReadOnlySpan<Offset>.Enumerator GetEnumerator(this byte @this);
	public static partial ReadOnlySpan<Offset>.Enumerator GetEnumerator(this short @this);
	public static partial ReadOnlySpan<Offset>.Enumerator GetEnumerator(this ushort @this);
	public static partial ReadOnlySpan<Offset>.Enumerator GetEnumerator(this int @this);
	public static partial ReadOnlySpan<Offset>.Enumerator GetEnumerator(this uint @this);
	public static partial ReadOnlySpan<Offset>.Enumerator GetEnumerator(this long @this);
	public static partial ReadOnlySpan<Offset>.Enumerator GetEnumerator(this ulong @this);
	public static unsafe partial ReadOnlySpan<Offset>.Enumerator GetEnumerator(this nint @this);
	public static unsafe partial ReadOnlySpan<Offset>.Enumerator GetEnumerator(this nuint @this);

	public static partial Offset GetNextSet(this byte @this, Offset index);
	public static partial Offset GetNextSet(this short @this, Offset index);
	public static partial Offset GetNextSet(this int @this, Offset index);
	public static partial Offset GetNextSet(this long @this, Offset index);

	public static partial void ReverseBits(this scoped ref byte @this);
	public static partial void ReverseBits(this scoped ref short @this);
	public static partial void ReverseBits(this scoped ref int @this);
	public static partial void ReverseBits(this scoped ref long @this);

	public static partial Offset SetAt(this byte @this, Offset order);
	public static partial Offset SetAt(this short @this, Offset order);
	public static partial Offset SetAt(this int @this, Offset order);
	public static partial Offset SetAt(this long @this, Offset order);

	public static partial byte SkipSetBit(this byte @this, Count setBitPosCount);
	public static partial short SkipSetBit(this short @this, Count setBitPosCount);
	public static partial int SkipSetBit(this int @this, Count setBitPosCount);
	public static partial long SkipSetBit(this long @this, Count setBitPosCount);
}
