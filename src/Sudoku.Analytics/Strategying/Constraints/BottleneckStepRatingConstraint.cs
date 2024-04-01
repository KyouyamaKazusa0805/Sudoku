namespace Sudoku.Strategying.Constraints;

/// <summary>
/// Represents a constraint that checks for difficulty rating of each step.
/// </summary>
[GetHashCode]
[ToString]
public sealed partial class BottleneckStepRatingConstraint : Constraint
{
	/// <inheritdoc/>
	public override bool AllowDuplicate => false;

	/// <summary>
	/// Indicates the minimal value.
	/// </summary>
	[HashCodeMember]
	[StringMember]
	public decimal Minimum { get; set; }

	/// <summary>
	/// Indicates the maximum value.
	/// </summary>
	[HashCodeMember]
	[StringMember]
	public decimal Maximum { get; set; }

	/// <summary>
	/// Indicates the between rule.
	/// </summary>
	[HashCodeMember]
	[StringMember]
	public BetweenRule BetweenRule { get; set; }


	/// <inheritdoc/>
	public override bool Check(ConstraintCheckingContext context)
	{
		return context.AnalyzerResult.BottleneckSteps is [{ Difficulty: var d }] && b(d, BetweenRule, Minimum, Maximum);


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static bool b(decimal v, BetweenRule rule, decimal min, decimal max)
			=> rule switch
			{
				BetweenRule.BothOpen => v > min && v < max,
				BetweenRule.LeftOpen => v > min && v <= max,
				BetweenRule.RightOpen => v >= min && v < max,
				BetweenRule.BothClosed => v >= min && v <= max
			};
	}

	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Constraint? other)
		=> other is BottleneckStepRatingConstraint comparer
		&& (Minimum, Maximum, BetweenRule) == (comparer.Minimum, comparer.Maximum, comparer.BetweenRule);

	/// <inheritdoc/>
	public override string ToString(CultureInfo? culture = null)
		=> string.Format(
			ResourceDictionary.Get("BottleneckStepRatingConstraint", culture),
			Minimum,
			Maximum,
			BetweenRule switch
			{
				BetweenRule.BothOpen => ResourceDictionary.Get("BothOpen", culture),
				BetweenRule.LeftOpen => ResourceDictionary.Get("LeftOpen", culture),
				BetweenRule.RightOpen => ResourceDictionary.Get("RightOpen", culture),
				BetweenRule.BothClosed => ResourceDictionary.Get("BothClosed", culture)
			}
		);
}
