namespace Sudoku.Strategying.Constraints;

/// <summary>
/// Represents a technique constraint. Different with <see cref="TechniqueCountConstraint"/>,
/// this constraint only controls the appearing techniques, rather than the number of times appeared.
/// </summary>
/// <seealso cref="TechniqueCountConstraint"/>
[ConstraintOptions(AllowsMultiple = true, AllowsNegation = true)]
[TypeImpl(TypeImplFlag.Object_GetHashCode | TypeImplFlag.Object_ToString)]
public sealed partial class TechniqueConstraint : Constraint
{
	/// <summary>
	/// Indicates the techniques must appear.
	/// </summary>
	[HashCodeMember]
	public TechniqueSet Techniques { get; set; } = TechniqueSets.None;

	[StringMember]
	private string TechniquesString => Techniques.ToString();


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Constraint? other)
		=> other is TechniqueConstraint comparer && Techniques == comparer.Techniques;

	/// <inheritdoc/>
	public override string ToString(IFormatProvider? formatProvider)
	{
		var culture = formatProvider as CultureInfo;
		return string.Format(SR.Get("TechniqueConstraint", culture), Techniques.ToString(culture));
	}

	/// <inheritdoc/>
	public override TechniqueConstraint Clone() => new() { IsNegated = IsNegated, Techniques = Techniques[..] };

	/// <inheritdoc/>
	protected override bool CheckCore(ConstraintCheckingContext context)
	{
		// Special case: If a user doesn't select any technique, we should consider this case as "always true".
		if (!Techniques)
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
}
