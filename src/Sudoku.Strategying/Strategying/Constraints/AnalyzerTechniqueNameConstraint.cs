namespace Sudoku.Strategying.Constraints;

/// <summary>
/// Represents a constraint that checks for the name of the technique.
/// </summary>
[GetHashCode]
[ToString]
public sealed partial class AnalyzerTechniqueNameConstraint : Constraint
{
	/// <summary>
	/// Indicates the desired name pattern.
	/// </summary>
	[HashCodeMember]
	[StringMember]
	[StringSyntax(StringSyntax.Regex)]
	public required string Pattern { get; set; }

	/// <summary>
	/// Indicates the current culture.
	/// </summary>
	[HashCodeMember]
	public required CultureInfo CurrentCulture { get; set; }

	[StringMember]
	private string CurrentCultureString => CurrentCulture.ToString();


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Constraint? other)
		=> other is AnalyzerTechniqueNameConstraint comparer && Pattern == comparer.Pattern;

	/// <inheritdoc/>
	protected internal override bool CheckCore(scoped ConstraintCheckingContext context)
	{
		if (!context.RequiresAnalyzer)
		{
			return false;
		}

		foreach (var step in context.AnalyzerResult)
		{
			var name = step.GetName(CurrentCulture);
			if (name.IsMatch(Pattern))
			{
				return true;
			}
		}

		return false;
	}

	/// <inheritdoc/>
	protected internal override ValidationResult ValidateCore()
		=> Pattern.IsRegexPattern()
			? ValidationResult.Successful
			: ValidationResult.Failed(nameof(Pattern), ValidationReason.MalformedPattern, Severity.Error);
}
