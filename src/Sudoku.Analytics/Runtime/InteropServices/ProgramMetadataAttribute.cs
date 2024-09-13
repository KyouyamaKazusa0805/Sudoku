namespace Sudoku.Runtime.InteropServices;

/// <summary>
/// Represents program compatibility attribute type.
/// </summary>
public abstract class ProgramMetadataAttribute : Attribute
{
	/// <summary>
	/// Represents the aliased names for the current technique defined in the current program.
	/// </summary>
	[DisallowNull]
	public string[]? Aliases { get; init; }
}
