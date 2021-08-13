namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

partial class UnncessaryDiscardPatternAnalyzer
{
	/// <summary>
	/// Defines a comparer that allows a triplet of <see cref="SubpatternSyntax"/>, <see cref="string"/>
	/// and <see cref="int"/> participating in this comparison.
	/// </summary>
	private sealed class TripletComparer : IEqualityComparer<(SubpatternSyntax, string, int Index)>
	{
		/// <inheritdoc/>
		public bool Equals((SubpatternSyntax, string, int Index) x, (SubpatternSyntax, string, int Index) y) =>
			x.Index == y.Index;

		/// <inheritdoc/>
		public int GetHashCode((SubpatternSyntax, string, int Index) obj) => obj.Index;
	}
}
