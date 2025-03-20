namespace System.CommandLine;

/// <summary>
/// Provides with extension methods on <see cref="Command"/>.
/// </summary>
/// <seealso cref="Command"/>
public static class CommandExtensions
{
	/// <summary>
	/// Adds a list of <see cref="Option"/> instances into the command.
	/// </summary>
	/// <param name="this">The command.</param>
	/// <param name="options">The options.</param>
	public static void AddRange(this Command @this, params ReadOnlySpan<Option> options)
	{
		foreach (var option in options)
		{
			@this.Add(option);
		}
	}
}
