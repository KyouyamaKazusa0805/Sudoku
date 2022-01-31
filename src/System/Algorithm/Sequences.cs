using static System.MathF;

namespace System.Algorithm;

/// <summary>
/// Defines some sequences that has been recorded in the
/// <see href="https://oeis.org/">On-Line Encyclopedia of Integer Sequences</see>.
/// </summary>
public static class Sequences
{
	/// <summary>
	/// Gets the value at the specified index in the sequence
	/// <see href="https://oeis.org/A057353">A057353</see>
	/// (0, 1, 2, 3, 3, 4, 5, 6, 6, 7, 8, 9, ..).
	/// </summary>
	/// <param name="index">The index of the sequence. The index is 0-based.</param>
	/// <returns>The result value at the specified index.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int A057353(int index) => (int)Floor(3F * index / 4);
}
