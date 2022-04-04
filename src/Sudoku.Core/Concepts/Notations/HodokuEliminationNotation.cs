namespace Sudoku.Concepts.Notations;

/// <summary>
/// Encapsulates a set of methods that handles a variety of instances, using Hodoku elimination notation
/// to output the <see cref="string"/> representation, or parse a <see cref="string"/> value to convert
/// it to the suitable-typed instance.
/// </summary>
public sealed class HodokuEliminationNotation : INotationHandler
{
	[Obsolete("Please don't call this constructor.", true)]
	private HodokuEliminationNotation() => throw new NotSupportedException();


	/// <inheritdoc/>
	public Notation Notation => Notation.HodokuElimination;
}
