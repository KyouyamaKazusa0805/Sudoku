namespace SudokuStudio.Interaction;

/// <summary>
/// Defines a display item that will be displayed on tooltip to describe a data unit of a <see cref="Step"/>.
/// </summary>
/// <seealso cref="Step"/>
[Flags]
public enum StepTooltipDisplayItems
{
	/// <summary>
	/// Indicates none of all elements mentioned below will be displayed.
	/// </summary>
	None = 0,

	/// <summary>
	/// Indicates the technique name will be displayed.
	/// </summary>
	TechniqueName = 1,

	/// <summary>
	/// Indicates the position of the step in the whole solving path.
	/// </summary>
	TechniqueIndex = 1 << 1,

	/// <summary>
	/// Indicates the difficulty rating.
	/// </summary>
	DifficultyRating = 1 << 2,

	/// <summary>
	/// Indicates the simple description.
	/// </summary>
	SimpleDescription = 1 << 3,

	/// <summary>
	/// Indicates the extra difficulty cases.
	/// </summary>
	ExtraDifficultyCases = 1 << 4,

	/// <summary>
	/// Indicates the abbreviation.
	/// </summary>
	Abbreviation = 1 << 5,

	/// <summary>
	/// Indicates the aliases.
	/// </summary>
	Aliases = 1 << 6
}
