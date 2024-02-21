namespace Sudoku.Strategying.Constraints;

/// <summary>
/// Represents a constraint that checks for the name of the technique.
/// </summary>
[GetHashCode]
[ToString]
public sealed partial class CollectorTechniqueNameConstraint : Constraint
{
	/// <summary>
	/// Indicates the desired name pattern.
	/// </summary>
	[HashCodeMember]
	[StringMember]
	[StringSyntax(StringSyntax.Regex)]
	public required string Pattern { get; set; }

	/// <inheritdoc/>
	public override ConstraintCheckingProperty CheckingProperties => ConstraintCheckingProperty.CollectorResult;

	/// <summary>
	/// Indicates the current culture.
	/// </summary>
	[HashCodeMember]
	public required CultureInfo CurrentCulture { get; set; }

	[StringMember]
	private string CurrentCultureString => CurrentCulture.ToString();


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Constraint? other)
		=> other is CollectorTechniqueNameConstraint comparer && Pattern == comparer.Pattern;

	/// <inheritdoc/>
	protected internal override bool CheckCore(scoped ConstraintCheckingContext context)
	{
		if (!context.RequiresCollector)
		{
			return false;
		}

		foreach (var stepArray in context.CollectorResult)
		{
			foreach (var step in stepArray)
			{
				var name = step.GetName(CurrentCulture);
				if (name.IsMatch(Pattern))
				{
					return true;
				}
			}
		}

		return false;
	}

	/// <inheritdoc/>
	protected internal override ValidationResult ValidateCore()
		=> Pattern.IsRegexPattern()
			? ValidationResult.Successful
			: ValidationResult.Failed(nameof(Pattern), ValidationReason.MalformedPattern, ValidationSeverity.Error);
}
