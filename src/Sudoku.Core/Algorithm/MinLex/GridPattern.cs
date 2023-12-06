#pragma warning disable

using System.Runtime.CompilerServices;

namespace Sudoku.Algorithm.MinLex;

/// <summary>
/// Represents for a pattern for a grid.
/// </summary>
/// <remarks>
/// <inheritdoc cref="BestTriplet" path="/remarks"/>
/// </remarks>
public unsafe struct GridPattern
{
	/// <summary>
	/// Indicates the rows.
	/// </summary>
	public fixed int Rows[9];

	/// <summary>
	/// Indicates the digits.
	/// </summary>
	public fixed int Digits[81];


	/// <summary>
	/// Indicates the best top-row score.
	/// </summary>
	public readonly extern int BestTopRowScore { get; }


	/// <inheritdoc cref="FromStringUnsafe(string, GridPattern*, GridPattern*)"/>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the argument <paramref name="text"/> does not have 81 characters.</exception>
	public static extern void FromString(string text, out GridPattern normal, out GridPattern transposed);

	/// <summary>
	/// Loads a string text, parsing the data and returns two <see cref="GridPattern"/> results
	/// indicating the data equivalent to the grid.
	/// </summary>
	/// <param name="text">The text.</param>
	/// <param name="pair">The pair of pointers indicating the normal and transposed cases. <i><b>The length must be 2.</b></i></param>
	public static extern void FromStringUnsafe(string text, GridPattern* pair);

	/// <summary>
	/// Loads a string text, parsing the data and returns two <see cref="GridPattern"/> results
	/// indicating the data equivalent to the grid.
	/// </summary>
	/// <param name="text">The text.</param>
	/// <param name="normal">The normal converted data.</param>
	/// <param name="transposed">The transposed converted data.</param>
	public static extern void FromStringUnsafe(string text, GridPattern* normal, GridPattern* transposed);
}
