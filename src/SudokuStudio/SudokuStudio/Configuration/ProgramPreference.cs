namespace SudokuStudio.Configuration;

/// <summary>
/// Defines a user-defined program preference.
/// </summary>
public sealed class ProgramPreference
{
	/// <inheritdoc cref="LogicalAnalysisPreference"/>
	public LogicalAnalysisPreference LogicalAnalysisPreferences { get; set; } = new();
}
