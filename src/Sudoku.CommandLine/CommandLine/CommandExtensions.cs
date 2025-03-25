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
	public static void AddRange(this Command @this, params SymbolList<Option> options)
	{
		foreach (var option in options)
		{
			@this.Add(option);
		}
	}

	/// <summary>
	/// Adds a list of <see cref="Argument"/> instances into the command.
	/// </summary>
	/// <param name="this">The command.</param>
	/// <param name="arguments">The arguments.</param>
	public static void AddRange(this Command @this, params SymbolList<Argument> arguments)
	{
		foreach (var argument in arguments)
		{
			@this.Add(argument);
		}
	}

	/// <summary>
	/// Adds a list of <see cref="Command"/> instances into the command.
	/// </summary>
	/// <param name="this">The command.</param>
	/// <param name="subcommands">The subcommands.</param>
	public static void AddRange(this Command @this, params ReadOnlySpan<Command> subcommands)
	{
		foreach (var subcommand in subcommands)
		{
			@this.Add(subcommand);
		}
	}
}
