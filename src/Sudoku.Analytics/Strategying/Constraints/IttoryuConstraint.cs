namespace Sudoku.Strategying.Constraints;

/// <summary>
/// Represents a constraint that checks whether a puzzle can be finished by ittoryu rules.
/// </summary>
[GetHashCode]
[ToString]
public sealed partial class IttoryuConstraint : Constraint, IComparisonOperatorConstraint, ILimitCountConstraint<int>
{
	/// <inheritdoc/>
	public override bool AllowDuplicate => false;

	/// <summary>
	/// Indicates the rounds used.
	/// </summary>
	[HashCodeMember]
	[StringMember]
	public int Rounds { get; set; }

	/// <inheritdoc/>
	[HashCodeMember]
	[StringMember]
	public ComparisonOperator Operator { get; set; }

	/// <inheritdoc/>
	int ILimitCountConstraint<int>.LimitCount { get => Rounds; set => Rounds = value; }


	/// <inheritdoc/>
	public static int Minimum => 1;

	/// <inheritdoc/>
	public static int Maximum => 10;


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Constraint? other)
		=> other is IttoryuConstraint comparer && (Rounds, Operator) == (comparer.Rounds, comparer.Operator);

	/// <inheritdoc/>
	public override bool Check(ConstraintCheckingContext context)
	{
		var steps = context.AnalyzerResult.Steps!;
		var roundsCount = 1;
		for (var i = 0; i < steps.Length - 1; i++)
		{
			var a = ((SingleStep)steps[i]).Digit;
			var b = ((SingleStep)steps[i + 1]).Digit;
			if ((a, b) is (8, 0))
			{
				roundsCount++;
				continue;
			}

			if (b - a is 0 or 1)
			{
				continue;
			}

			roundsCount = -1;
			break;
		}

		return Operator.GetOperator<int>()(roundsCount, Rounds);
	}

	/// <inheritdoc/>
	public override string ToString(CultureInfo? culture = null)
		=> string.Format(ResourceDictionary.Get("IttoryuConstraint", culture), Operator.GetOperatorString(), Rounds);
}
