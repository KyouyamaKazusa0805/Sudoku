namespace Sudoku.Solving.Logical.Implementations.Data;

/// <summary>
/// Represents a data structure that describes the eliminations that are created and proved
/// by the exocet technique.
/// </summary>
/// <param name="Conclusions">Indicates the conclusions.</param>
/// <param name="Reason">Indicates the reason why these candidates can be eliminated.</param>
public readonly record struct ExocetElimination(Conclusion[] Conclusions, ExocetEliminatedReason Reason) :
	IEquatable<ExocetElimination>,
	IEqualityOperators<ExocetElimination, ExocetElimination, bool>,
	IPhasedConclusionProvider<ExocetElimination, ExocetEliminatedReason>
{
	/// <inheritdoc cref="object.ToString"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString()
	{
		var header = R[$"Exocet{Reason}EliminationName"]!;
		var snippet = R["ExocetElimination"]!;
		var elim = ConclusionFormatter.Format(Conclusions, FormattingMode.Normal);
		return $"* {header}{snippet}{elim}";
	}
}
