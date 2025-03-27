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
	public static void AddRange(this Command @this, params SymbolList<Command> subcommands)
	{
		foreach (var subcommand in subcommands)
		{
			@this.Add(subcommand);
		}
	}

	/// <summary>
	/// Adds a list of <see cref="Option"/> instances into the command,
	/// as global ones applying to the current command and its sub-commands.
	/// </summary>
	/// <param name="this">The command.</param>
	/// <param name="options">The options.</param>
	public static void AddRangeGlobal(this Command @this, params SymbolList<Option> options)
	{
		foreach (var option in options)
		{
			@this.AddGlobalOption(option);
		}
	}

	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	internal static void Deconstruct<TCommand>(this TCommand @this, out SymbolList<Option> options, out SymbolList<Argument> arguments)
		where TCommand : Command, ICommand
	{
		options = @this.OptionsCore;
		arguments = @this.ArgumentsCore;
	}

	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	internal static void Deconstruct<TCommand>(
		this TCommand @this,
		out SymbolList<Option> options,
		out SymbolList<Argument> arguments,
		out Command? parent
	)
		where TCommand : Command, ICommand
		=> ((options, arguments), parent) = (@this, @this.Parent);

	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	internal static void Deconstruct<TNonLeafCommand>(this TNonLeafCommand @this, out SymbolList<Option> options)
		where TNonLeafCommand : INonLeafCommand
		=> options = @this.GlobalOptionsCore;
}
