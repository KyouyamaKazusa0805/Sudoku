namespace Sudoku.Strategying.Constraints;

/// <summary>
/// Represents a conclusion constraint.
/// </summary>
[ConstraintOptions(AllowsMultiple = true)]
[GetHashCode]
[ToString]
public sealed partial class ConclusionConstraint : Constraint
{
	/// <summary>
	/// Indicates whether the conclusion should be appeared.
	/// </summary>
	[HashCodeMember]
	[StringMember]
	public bool ShouldAppear { get; set; }

	/// <summary>
	/// Indicates the conclusion.
	/// </summary>
	[HashCodeMember]
	[StringMember]
	public Conclusion Conclusion { get; set; }


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Constraint? other)
		=> other is ConclusionConstraint comparer && (Conclusion, ShouldAppear) == (comparer.Conclusion, comparer.ShouldAppear);

	/// <inheritdoc/>
	public override string ToString(IFormatProvider? formatProvider)
	{
		var culture = formatProvider as CultureInfo;
		return string.Format(
			ResourceDictionary.Get("ConclusionConstraint", culture),
			Conclusion.ToString(culture),
			ShouldAppear ? string.Empty : ResourceDictionary.Get("NoString", culture)
		);
	}

	/// <inheritdoc/>
	public override ConclusionConstraint Clone()
		=> new() { IsNegated = IsNegated, Conclusion = Conclusion, ShouldAppear = ShouldAppear };

	/// <inheritdoc/>
	protected override bool CheckCore(ConstraintCheckingContext context)
	{
		var appeared = false;
		foreach (var step in context.AnalyzerResult)
		{
			foreach (var conclusion in step.Conclusions)
			{
				if (Conclusion == conclusion)
				{
					appeared = true;
					break;
				}
			}
		}

		return !(ShouldAppear ^ appeared);
	}
}
