namespace Sudoku.Runtime.MeasuringServices.Functions;

/// <summary>
/// Represents a list of methods that calculates for values in OEIS sequences.
/// </summary>
/// <shared-comments>
/// <para>The index of the sequence. The index is 0-based.</para>
/// <para>The index of the sequence. The index is 1-based.</para>
/// </shared-comments>
public sealed class OeisSequences : IFunctionProvider
{
	/// <summary>
	/// Gets the value at the specified index in the sequence <see href="https://oeis.org/A000217">A000217</see>
	/// (0, 1, 3, 6, 10, 15, 21, 28, 36, 45, 55, ..).
	/// </summary>
	/// <param name="index"><inheritdoc cref="OeisSequences" path="//shared-comments/para[1]"/></param>
	/// <returns>The result value at the specified index.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int A000217(int index) => index * (index - 1) >> 1;

	/// <summary>
	/// Gets the value at the specified index in the sequence <see href="https://oeis.org/A002024">A002024</see>
	/// (1, 2, 2, 3, 3, 3, 4, 4, 4, 4, 5, ..).
	/// </summary>
	/// <param name="index"><inheritdoc cref="OeisSequences" path="//shared-comments/para[2]"/></param>
	/// <returns>The result value at the specified index.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int A002024(int index) => (int)(Math.Sqrt(index << 1) + .5);

	/// <summary>
	/// Gets the value at the specified index in the sequence <see href="https://oeis.org/A004526">A004526</see>
	/// (0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, ..).
	/// </summary>
	/// <param name="index"><inheritdoc cref="OeisSequences" path="//shared-comments/para[1]"/></param>
	/// <returns>The result value at the specified index.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int A004526(int index) => index >> 1;
}
