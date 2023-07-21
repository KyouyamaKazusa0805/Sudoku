namespace SudokuStudio.BindableSource;

/// <summary>
/// Represents a bindable source for step searcher supported techniques.
/// </summary>
public sealed class StepSearcherSupportedTechniqueBindableSource
{
	/// <summary>
	/// Indicates the supported techniques.
	/// </summary>
	public required Technique[] Techniques { get; set; }

	/// <summary>
	/// Indicates the supported difficulty levels.
	/// </summary>
	public required DifficultyLevel[] DifficultyLevels { get; set; }
}
