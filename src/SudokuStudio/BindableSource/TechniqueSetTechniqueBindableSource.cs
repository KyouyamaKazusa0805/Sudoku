using Sudoku.Analytics.Categorization;

namespace SudokuStudio.BindableSource;

/// <summary>
/// Represents a bindable source for a <see cref="Technique"/>.
/// </summary>
/// <seealso cref="Technique"/>
public sealed class TechniqueSetTechniqueBindableSource
{
	/// <summary>
	/// Indicates the name of the technique.
	/// </summary>
	public string TechniqueName => TechniqueField.GetName();

	/// <summary>
	/// Indicates the technique used.
	/// </summary>
	public Technique TechniqueField { get; init; }

	/// <summary>
	/// Indicates the difficulty level of the technique.
	/// </summary>
	public DifficultyLevel DifficultyLevel => TechniqueField.GetDifficultyLevel();
}
