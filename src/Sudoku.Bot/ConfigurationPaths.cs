namespace Sudoku.Bot;

/// <summary>
/// Provides with the paths that are defined in the local JSON configuration file.
/// </summary>
internal static class ConfigurationPaths
{
	/// <summary>
	/// Indicates the path <c>lastTimeCheckedIn</c>. This field is used for checking
	/// whether the user clocks-in in a same day.
	/// </summary>
	public const string LastTimeCheckedIn = "lastTimeCheckedIn";

	/// <summary>
	/// Indicates the path <c>experiencePoint</c>. This field records the value
	/// measuring the user's interaction frequency. The greater the value be, the more times the user interacts
	/// with the bot.
	/// </summary>
	public const string ExperiencePoint = "experiencePoint";
}
