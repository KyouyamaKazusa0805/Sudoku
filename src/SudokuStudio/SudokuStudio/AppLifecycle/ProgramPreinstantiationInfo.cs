namespace SudokuStudio.AppLifecycle;

/// <summary>
/// Represents the pre-instantiation information.
/// </summary>
public sealed class ProgramPreinstantiationInfo
{
	/// <summary>
	/// The opened sudoku file content.
	/// </summary>
	public GridSerializationData[]? OpenedSudoku { get; internal set; }

	/// <summary>
	/// The program preference.
	/// </summary>
	public ProgramPreference? OpenedProgramPreference { get; internal set; }
}
