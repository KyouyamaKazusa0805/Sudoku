namespace Sudoku.Algorithms.Generating;

/// <summary>
/// Represents a base type for puzzle generated, need creating a data structure to store the details for the generated puzzle.
/// </summary>
[TypeImpl(
	TypeImplFlag.Object_Equals | TypeImplFlag.Object_GetHashCode | TypeImplFlag.EqualityOperators,
	OtherModifiersOnEquals = "sealed",
	GetHashCodeBehavior = GetHashCodeBehavior.MakeAbstract)]
public abstract partial class PuzzleBase : IEquatable<PuzzleBase>, IEqualityOperators<PuzzleBase, PuzzleBase, bool>
{
	/// <summary>
	/// Indicates whether the data represents "success" message and values are valid in use.
	/// </summary>
	public virtual bool Success => FailedReason == GeneratingFailedReason.None;

	/// <summary>
	/// Indicates the failed reason why causes the failure.
	/// </summary>
	[HashCodeMember]
	[StringMember]
	public GeneratingFailedReason FailedReason { get; init; }

	/// <summary>
	/// Indicates the puzzle just created. If the value <see cref="FailedReason"/> returns a value
	/// not <see cref="GeneratingFailedReason.None"/>, the value will always be <see cref="Grid.Undefined"/>.
	/// </summary>
	/// <seealso cref="FailedReason"/>
	/// <seealso cref="GeneratingFailedReason.None"/>
	/// <seealso cref="Grid.Undefined"/>
	[HashCodeMember]
	[StringMember]
	public Grid Puzzle { get; init; }


	/// <inheritdoc/>
	public abstract bool Equals([NotNullWhen(true)] PuzzleBase? other);

	/// <inheritdoc/>
	public override string ToString() => Puzzle.ToString();
}
