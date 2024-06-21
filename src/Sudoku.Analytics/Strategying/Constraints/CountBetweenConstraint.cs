namespace Sudoku.Strategying.Constraints;

/// <summary>
/// Represents a count between constraint.
/// </summary>
[TypeImpl(TypeImplFlag.Object_GetHashCode | TypeImplFlag.Object_ToString)]
public sealed partial class CountBetweenConstraint : Constraint, IBetweenRuleConstraint
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

	/// <inheritdoc/>
	[HashCodeMember]
	[StringMember]
	public BetweenRule BetweenRule { get; set; }


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Constraint? other)
		=> other is CountBetweenConstraint comparer
		&& Range.Equals(comparer.Range) && (CellState, BetweenRule) == (comparer.CellState, comparer.BetweenRule);

	/// <inheritdoc/>
	public override string ToString(IFormatProvider? formatProvider)
	{
		var culture = formatProvider as CultureInfo;
		return string.Format(
			SR.Get("CountBetweenConstraint", culture),
			SR.Get(CellState switch { CellState.Given => "GivenCell", _ => "EmptyCell" }, culture),
			Range.Start.Value,
			Range.End.Value,
			BetweenRule switch
			{
				BetweenRule.BothOpen => SR.Get("BothOpen", culture),
				BetweenRule.LeftOpen => SR.Get("LeftOpen", culture),
				BetweenRule.RightOpen => SR.Get("RightOpen", culture),
				BetweenRule.BothClosed => SR.Get("BothClosed", culture)
			}
		);
	}

	/// <inheritdoc/>
	public override CountBetweenConstraint Clone()
		=> new() { IsNegated = IsNegated, BetweenRule = BetweenRule, CellState = CellState, Range = Range };

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
