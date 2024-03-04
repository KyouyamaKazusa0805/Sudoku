namespace Sudoku.Strategying.Constraints;

/// <summary>
/// Represents a conclusion constraint.
/// </summary>
[GetHashCode]
[ToString]
public sealed partial class ConclusionConstraint : Constraint
{
	/// <inheritdoc/>
	public override bool AllowDuplicate => true;

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
	public override bool Check(ConstraintCheckingContext context)
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

	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Constraint? other)
		=> other is ConclusionConstraint comparer && (Conclusion, ShouldAppear) == (comparer.Conclusion, comparer.ShouldAppear);

	/// <inheritdoc/>
	public override string ToString(CultureInfo? culture = null)
		=> string.Format(
			ResourceDictionary.Get("ConclusionConstraint", culture),
			Conclusion.ToString(culture),
			ShouldAppear ? string.Empty : ResourceDictionary.Get("NoString", culture)
		);
}
