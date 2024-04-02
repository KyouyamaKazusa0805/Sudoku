namespace Sudoku.Strategying.Constraints;

/// <summary>
/// Represents a count between constraint.
/// </summary>
[GetHashCode]
[ToString]
public sealed partial class CountBetweenConstraint : Constraint
{
	/// <summary>
	/// Indicates the range of the numbers set.
	/// </summary>
	[HashCodeMember]
	[StringMember]
	[JsonConverter(typeof(RangeConverter))]
	public Range Range { get; set; }

	/// <summary>
	/// Indicates the cell state to be checked. The desired values should be
	/// <see cref="CellState.Given"/> or <see cref="CellState.Empty"/>.
	/// </summary>
	/// <seealso cref="CellState.Given"/>
	/// <seealso cref="CellState.Empty"/>
	[HashCodeMember]
	[StringMember]
	public CellState CellState { get; set; }

	/// <summary>
	/// Indicates the between rule.
	/// </summary>
	[HashCodeMember]
	[StringMember]
	public BetweenRule BetweenRule { get; set; }


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Constraint? other)
		=> other is CountBetweenConstraint comparer
		&& Range.Equals(comparer.Range) && (CellState, BetweenRule) == (comparer.CellState, comparer.BetweenRule);

	/// <inheritdoc/>
	public override string ToString(CultureInfo? culture = null)
		=> string.Format(
			ResourceDictionary.Get("CountBetweenConstraint", culture),
			ResourceDictionary.Get(CellState switch { CellState.Given => "GivenCell", _ => "EmptyCell" }, culture),
			Range.Start.Value,
			Range.End.Value,
			BetweenRule switch
			{
				BetweenRule.BothOpen => ResourceDictionary.Get("BothOpen", culture),
				BetweenRule.LeftOpen => ResourceDictionary.Get("LeftOpen", culture),
				BetweenRule.RightOpen => ResourceDictionary.Get("RightOpen", culture),
				BetweenRule.BothClosed => ResourceDictionary.Get("BothClosed", culture)
			}
		);

	/// <inheritdoc/>
	protected override bool CheckCore(ConstraintCheckingContext context)
		=> context.Grid is var grid
		&& CellState switch { CellState.Empty => grid.EmptiesCount, _ => grid.GivensCount } is var factCount
		&& Range is { Start.Value: var min, End.Value: var max }
		&& BetweenRule switch
		{
			BetweenRule.BothOpen => factCount > min && factCount < max,
			BetweenRule.LeftOpen => factCount >= min && factCount <= max,
			BetweenRule.RightOpen => factCount >= min && factCount < max,
			BetweenRule.BothClosed => factCount >= min && factCount <= max
		};
}
