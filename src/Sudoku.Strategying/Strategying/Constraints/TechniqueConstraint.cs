namespace Sudoku.Strategying.Constraints;

/// <summary>
/// Represents a technique constraint. Different with <see cref="TechniqueCountConstraint"/>,
/// this constraint only controls the appearing techniques, rather than the number of times appeared.
/// </summary>
/// <seealso cref="TechniqueCountConstraint"/>
[GetHashCode]
[ToString]
public sealed partial class TechniqueConstraint : Constraint
{
	/// <inheritdoc/>
	public override bool AllowDuplicate => false;

	/// <summary>
	/// Indicates the techniques must appear.
	/// </summary>
	[HashCodeMember]
	public TechniqueSet Techniques { get; set; } = [];

	[StringMember]
	private string TechniquesString => Techniques.ToString();


	/// <inheritdoc/>
	public override bool Check(scoped ConstraintCheckingContext context)
	{
		if (!context.RequiresAnalyzer)
		{
			return false;
		}

		// Special case: If a user doesn't select any technique, we should consider this case as "always true".
		if (Techniques.Count == 0)
		{
			return true;
		}

		foreach (var step in context.AnalyzerResult)
		{
			if (Techniques.Contains(step.Code))
			{
				return true;
			}
		}

		return false;
	}

	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Constraint? other)
		=> other is TechniqueConstraint comparer && Techniques == comparer.Techniques;

	/// <inheritdoc/>
	public override string ToString(CultureInfo? culture = null)
		=> string.Format(ResourceDictionary.Get("TechniqueConstraint", culture), Techniques.ToString(culture));
}
