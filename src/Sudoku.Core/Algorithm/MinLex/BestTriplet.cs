#pragma warning disable

using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Sudoku.Algorithm.MinLex;

/// <summary>
/// Represents for a best triplet permutation.
/// </summary>
/// <remarks><i><b>
/// This type hasn't been implemented correctly, so I hide the internal logic to avoid users using this API.
/// </b></i></remarks>
public struct BestTriplet
{
	/// <summary>
	/// The best triplet permutations.
	/// </summary>
	public static readonly BestTriplet[][] BestTripletPermutations = null!;


	/// <summary>
	/// The total score of the pattern.
	/// </summary>
	public extern ref int BestResult { get; }

	/// <summary>
	/// The result mask.
	/// </summary>
	public extern ref int ResultMask { get; }

	/// <summary>
	/// The result number bits.
	/// </summary>
	public extern ref int ResultNumBits { get; }


	/// <summary>
	/// Creates a <see cref="BestTriplet"/> instance via collection expression.
	/// </summary>
	/// <param name="values">The values.</param>
	/// <returns>A valid <see cref="BestTriplet"/> result.</returns>
	public static extern BestTriplet Create(scoped ReadOnlySpan<int> values);
}
