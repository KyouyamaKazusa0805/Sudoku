namespace Sudoku.Compatibility;

/// <summary>
/// Provides a way to provide compatibility items (concepts, solving ways, etc.) to a program that also analyzes sudoku puzzles.
/// </summary>
public interface ICompatibilityProvider
{
	/// <summary>
	/// Indicates the program name of the current provider to be compatible.
	/// </summary>
	static abstract string ProgramName { get; }
}
