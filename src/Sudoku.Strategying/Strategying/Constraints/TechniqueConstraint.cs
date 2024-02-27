namespace Sudoku.Strategying.Constraints;

/// <summary>
/// Represents a rule that checks whether the specified analyzer result after analyzed by a grid
/// contains the specified techniques.
/// </summary>
[GetHashCode]
[ToString]
public sealed partial class TechniqueConstraint : Constraint
{
	/// <summary>
	/// Indicates the techniques used.
	/// </summary>
	public required Technique[] Techniques { get; set; }

	[HashCodeMember]
	private int TechniquesHashCode
	{
		get
		{
			var hashCode = new HashCode();
			foreach (var element in Techniques)
			{
				hashCode.Add(element);
			}

			return hashCode.ToHashCode();
		}
	}

	[StringMember]
	private string TechniquesString => Techniques.AsTechniqueSet().ToString();


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Constraint? other)
		=> other is TechniqueConstraint comparer && Techniques.AsTechniqueSet() == comparer.Techniques.AsTechniqueSet();

	/// <inheritdoc/>
	public override ConflictionResult VerifyConfliction(Constraint other)
	{
		if (other is not AnalyzerTechniqueCountConstraint { UniversalQuantifier: UniversalQuantifier.All, TechniqueAppearing.Keys: var techniques })
		{
			return ConflictionResult.Successful;
		}

		var set = Techniques.AsTechniqueSet();
		var otherSet = (TechniqueSet)([.. techniques]);
		if ((otherSet & set) == set)
		{
			// The other set fully covers the current set, so the current set won't work.
			return ConflictionResult.Failed(other, ConflictionType.ValueFullyCovered, Severity.Info);
		}

		return ConflictionResult.Failed(other, ConflictionType.ValueDiffers, Severity.Warning);
	}

	/// <inheritdoc/>
	protected internal override bool CheckCore(scoped ConstraintCheckingContext context)
	{
		if (!context.RequiresAnalyzer)
		{
			return false;
		}

		foreach (var step in context.AnalyzerResult)
		{
			if (Array.IndexOf(Techniques, step.Code) != -1)
			{
				return true;
			}
		}

		return false;
	}

	/// <inheritdoc/>
	protected internal override ValidationResult ValidateCore()
		=> Array.TrueForAll(Techniques, Enum.IsDefined)
			? ValidationResult.Successful
			: ValidationResult.Failed(nameof(Techniques), ValidationReason.EnumerationFieldNotDefined, Severity.Warning);
}
