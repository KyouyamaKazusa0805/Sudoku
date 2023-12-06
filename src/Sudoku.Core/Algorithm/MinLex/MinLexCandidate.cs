#pragma warning disable

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Sudoku.Runtime.MaskServices;

namespace Sudoku.Algorithm.MinLex;

/// <summary>
/// Represents for the basic data for candidates used by MinLex operations.
/// </summary>
public struct MinLexCandidate
{
	/// <summary>
	/// Skips for initialization for <see langword="this"/>
	/// </summary>
	public extern MinLexCandidate();


	/// <inheritdoc cref="object.ToString"/>
	public override readonly extern string ToString();


	/// <summary>
	/// Find minimum lexicographical-ordered string for the specified grid as a string value.
	/// </summary>
	/// <param name="source">The source grid.</param>
	/// <param name="result">The result grid.</param>
	/// <param name="patternOnly">Indicates whether the method only studies with pattern.</param>
	public static extern void PatCanon(string source, out string result, bool patternOnly = false);
}
