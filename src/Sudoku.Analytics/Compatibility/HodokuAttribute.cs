namespace Sudoku.Compatibility;

/// <summary>
/// Represents compatibility rules for Hodoku.
/// </summary>
/// <param name="rating">Indicates the rating value.</param>
/// <param name="difficultyLevel">Indicates the difficulty level.</param>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed partial class HodokuAttribute(
	[PrimaryConstructorParameter(Accessibility = "public override")] int rating,
	[PrimaryConstructorParameter(Accessibility = "public override")] HodokuDifficultyLevel difficultyLevel
) : ProgramMetadataAttribute<int, HodokuDifficultyLevel>
{
	/// <summary>
	/// Initializes a <see cref="HodokuAttribute"/> instance.
	/// </summary>
	public HodokuAttribute() : this(int.MinValue, (HodokuDifficultyLevel)int.MinValue)
	{
	}


	/// <summary>
	/// Indicates the technique prefix defined in Hodoku program.
	/// See <see href="https://hodoku.sourceforge.net/en/libs.php">this link</see> to learn more information about the library format.
	/// </summary>
	/// <seealso href="https://hodoku.sourceforge.net/en/libs.php">Hodoku: libraries</seealso>
	public string? Prefix { get; set; }
}
