namespace Sudoku.Strategying.Constraints;

/// <summary>
/// Represents a constraint that checks whether a puzzle contains any bottleneck.
/// </summary>
[GetHashCode]
[ToString]
public sealed partial class BottleneckConstraint : Constraint
{
	/// <summary>
	/// Indicates the type of the bottleneck will be checked.
	/// </summary>
	[HashCodeMember]
	public required BottleneckType Type { get; set; }

	[StringMember]
	private string TypeString => Type.ToString();


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Constraint? other)
		=> other is BottleneckConstraint comparer && Type == comparer.Type;

	/// <inheritdoc/>
	protected internal override bool CheckCore(scoped ConstraintCheckingContext context)
	{
		return context switch
		{
			{
				RequiresAnalyzer: true,
				AnalyzerResult: { IsSolved: true, DifficultyLevel: var difficultyLevel, Steps: var steps }
			} => Type switch
			{
				BottleneckType.DirectBottleneck when difficultyLevel == DifficultyLevel.Easy
					=> Array.Exists(steps, directViewBottleneckMatcher),
				BottleneckType.PuzzleBottleneck
					=> Array.Exists(
						steps,
						difficultyLevel switch
						{
							DifficultyLevel.Easy => directViewBottleneckMatcher,
							_ => static step => step.IsAssignment is not true
						}
					)
			},
			_ => false
		};


		static bool directViewBottleneckMatcher(Step step) => step is HiddenSingleStep { House: >= 9 } or NakedSingleStep;
	}

	/// <inheritdoc/>
	protected internal override ValidationResult ValidateCore()
		=> Type is BottleneckType.PuzzleBottleneck or BottleneckType.DirectBottleneck
			? ValidationResult.Successful
			: ValidationResult.Failed(nameof(Type), ValidationReason.EnumerationFieldNotDefined, Severity.Error);
}
