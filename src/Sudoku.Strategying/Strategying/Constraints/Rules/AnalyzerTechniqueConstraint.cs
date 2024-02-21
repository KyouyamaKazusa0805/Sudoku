namespace Sudoku.Strategying.Constraints;

/// <summary>
/// Represents a rule that checks whether the specified analyzer result after analyzed by a grid
/// contains the specified techniques.
/// </summary>
[GetHashCode]
[ToString]
public sealed partial class AnalyzerTechniqueConstraint : Constraint
{
	/// <summary>
	/// Indicates the techniques used.
	/// </summary>
	public required Technique[] Techniques { get; set; }

	/// <inheritdoc/>
	public override ConstraintCheckingProperty CheckingProperties => ConstraintCheckingProperty.AnalyzerResult;

	/// <inheritdoc/>
	protected internal override ValidationResult ValidationResult
		=> Array.TrueForAll(Techniques, Enum.IsDefined)
			? ValidationResult.Successful
			: ValidationResult.Failed(
				nameof(Techniques),
				ValidationReason.EnumerationFieldNotDefined,
				ValidationSeverity.Warning
			);

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
		=> other is AnalyzerTechniqueConstraint comparer && Techniques.AsTechniqueSet() == comparer.Techniques.AsTechniqueSet();

	/// <inheritdoc/>
	protected internal override bool CheckCore(scoped ConstraintCheckingContext context)
	{
		if (!context.RequiresAnalyzer)
		{
			return false;
		}

		var result = context.AnalyzerResult;
		foreach (var step in result)
		{
			if (Array.IndexOf(Techniques, step.Code) != -1)
			{
				return true;
			}
		}

		return false;
	}
}
