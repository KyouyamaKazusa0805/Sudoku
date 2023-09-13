using System.Runtime.CompilerServices;

namespace System.Algorithm;

/// <summary>
/// Defines some sequences that has been recorded in the
/// <see href="https://oeis.org/">On-Line Encyclopedia of Integer Sequences</see>.
/// </summary>
/// <shared-comments>
/// <para>The index of the sequence. The index is 0-based.</para>
/// <para>The index of the sequence. The index is 1-based.</para>
/// </shared-comments>
public static class Sequences
{
	/// <summary>
	/// Gets the value at the specified index in the sequence <see href="https://oeis.org/A000217">A000217</see>
	/// (0, 1, 3, 6, 10, 15, 21, 28, 36, 45, 55, ..).
	/// </summary>
	/// <param name="index"><inheritdoc cref="Sequences" path="//shared-comments/para[1]"/></param>
	/// <returns>The result value at the specified index.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int A000217(int index) => index * (index - 1) / 2;

	/// <summary>
	/// Gets the value at the specified index in the sequence <see href="https://oeis.org/A002024">A002024</see>
	/// (1, 2, 2, 3, 3, 3, 4, 4, 4, 4, 5, ..).
	/// </summary>
	/// <param name="index"><inheritdoc cref="Sequences" path="//shared-comments/para[2]"/></param>
	/// <returns>The result value at the specified index.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int A002024(int index) => (int)(Math.Sqrt(index << 1) + .5);

	/// <summary>
	/// Gets the value at the specified index in the sequence <see href="https://oeis.org/A004526">A004526</see>
	/// (0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, ..).
	/// </summary>
	/// <param name="index"><inheritdoc cref="Sequences" path="//shared-comments/para[1]"/></param>
	/// <returns>The result value at the specified index.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int A004526(int index) => index >> 1;

	/// <summary>
	/// Gets the value at the specified index in the sequence <see href="https://oeis.org/A057353">A057353</see>
	/// (0, 1, 2, 3, 3, 4, 5, 6, 6, 7, 8, 9, ..).
	/// </summary>
	/// <param name="index"><inheritdoc cref="Sequences" path="//shared-comments/para[1]"/></param>
	/// <returns>The result value at the specified index.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int A057353(int index) => (int)Math.Floor(3F * index / 4);
}
