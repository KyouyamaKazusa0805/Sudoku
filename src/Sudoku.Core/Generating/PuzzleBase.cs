namespace Sudoku.Generating;

/// <summary>
/// Represents a base type for puzzle generated, need creating a data structure to store the details for the generated puzzle.
/// </summary>
[Equals(OtherModifiers = "sealed")]
[GetHashCode(GetHashCodeBehavior.MakeAbstract)]
[ToString(ToStringBehavior.MakeAbstract)]
[EqualityOperators]
public abstract partial class PuzzleBase : IEquatable<PuzzleBase>, IEqualityOperators<PuzzleBase, PuzzleBase, bool>
{
	/// <summary>
	/// Indicates the generation result.
	/// </summary>
	[HashCodeMember]
	[StringMember]
	public GeneratingFailedReason Result { get; init; }

	/// <summary>
	/// Indicates the puzzle just created. If the value <see cref="Result"/> returns a value
	/// not <see cref="GeneratingFailedReason.None"/>, the value will always be <see cref="Grid.Undefined"/>.
	/// </summary>
	/// <seealso cref="Result"/>
	/// <seealso cref="GeneratingFailedReason.None"/>
	/// <seealso cref="Grid.Undefined"/>
	[HashCodeMember]
	[StringMember]
	public Grid Puzzle { get; init; }


	/// <inheritdoc/>
	public abstract bool Equals([NotNullWhen(true)] PuzzleBase? other);
}
