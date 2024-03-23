namespace Sudoku.Compatibility.Hodoku;

/// <summary>
/// Represents compatibility rules for Hodoku.
/// </summary>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed class HodokuAttribute : ProgramMetadataAttribute<int, HodokuDifficultyLevel>
{
	/// <summary>
	/// Indicates the technique prefix defined in Hodoku program.
	/// See <see href="https://hodoku.sourceforge.net/en/libs.php">this link</see> to learn more information about the library format.
	/// </summary>
	/// <seealso href="https://hodoku.sourceforge.net/en/libs.php">Hodoku: libraries</seealso>
	public string? Prefix { get; init; }
}
