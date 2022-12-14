namespace Sudoku.Platforms.QQ.Configurations;

/// <summary>
/// Defines a type that stores a user-defined configurations.
/// </summary>
public sealed class UserDefinedConfigurations
{
	/// <summary>
	/// Indicates how many members will be displayed onto the ranking page. The more members to display, the longer the output text will be.
	/// </summary>
	[JsonPropertyName("rankingDisplayUsersCount")]
	public int RankingDisplayUsersCount { get; set; } = 10;
}
