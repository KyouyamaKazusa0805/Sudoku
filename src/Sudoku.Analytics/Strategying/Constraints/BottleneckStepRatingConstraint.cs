namespace Sudoku.Strategying.Constraints;

/// <summary>
/// Represents a constraint that checks for difficulty rating of each step.
/// </summary>
[TypeImpl(TypeImplFlag.Object_GetHashCode | TypeImplFlag.Object_ToString)]
public sealed partial class BottleneckStepRatingConstraint : Constraint, IBetweenRuleConstraint
{
	/// <summary>
	/// Indicates the minimal value.
	/// </summary>
	[HashCodeMember]
	[StringMember]
	public int Minimum { get; set; }

	/// <summary>
	/// Indicates the maximum value.
	/// </summary>
	[HashCodeMember]
	[StringMember]
	public int Maximum { get; set; }

	/// <inheritdoc/>
	[HashCodeMember]
	[StringMember]
	public BetweenRule BetweenRule { get; set; }


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Constraint? other)
		=> other is BottleneckStepRatingConstraint comparer
		&& (Minimum, Maximum, BetweenRule) == (comparer.Minimum, comparer.Maximum, comparer.BetweenRule);

	/// <inheritdoc/>
	public override string ToString(IFormatProvider? formatProvider)
	{
		var culture = formatProvider as CultureInfo;
		return string.Format(
			SR.Get("BottleneckStepRatingConstraint", culture),
#if NET9_0_OR_GREATER
			[
#endif
			Minimum,
			Maximum,
			BetweenRule switch
			{
				BetweenRule.BothOpen => SR.Get("BothOpen", culture),
				BetweenRule.LeftOpen => SR.Get("LeftOpen", culture),
				BetweenRule.RightOpen => SR.Get("RightOpen", culture),
				BetweenRule.BothClosed => SR.Get("BothClosed", culture)
			}
#if NET9_0_OR_GREATER
			]
#endif
		);
	}

	/// <inheritdoc/>
	public override BottleneckStepRatingConstraint Clone()
		=> new() { BetweenRule = BetweenRule, IsNegated = IsNegated, Minimum = Minimum, Maximum = Maximum };

	/// <inheritdoc/>
	protected override bool CheckCore(ConstraintCheckingContext context)
	{
		return context.AnalyzerResult.BottleneckSteps is [{ Difficulty: var d }] && b(d, BetweenRule, Minimum, Maximum);


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static bool b(int v, BetweenRule rule, int min, int max)
			=> rule switch
			{
				BetweenRule.BothOpen => v > min && v < max,
				BetweenRule.LeftOpen => v > min && v <= max,
				BetweenRule.RightOpen => v >= min && v < max,
				BetweenRule.BothClosed => v >= min && v <= max
			};
	}
}
