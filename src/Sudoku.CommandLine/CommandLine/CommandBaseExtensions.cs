namespace Sudoku.CommandLine;

/// <summary>
/// Provides with extension methods on <see cref="CommandBase"/>.
/// </summary>
/// <seealso cref="CommandBase"/>
public static class CommandBaseExtensions
{
	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	public static void Deconstruct<TCommand>(this TCommand @this, out SymbolList<Option> options, out SymbolList<Argument> arguments)
		where TCommand : CommandBase
	{
		options = @this.OptionsCore;
		arguments = @this.ArgumentsCore;
	}

	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	public static void Deconstruct<TCommand>(
		this TCommand @this,
		out SymbolList<Option> options,
		out SymbolList<Argument> arguments,
		out SymbolList<Option> globalOptions
	)
		where TCommand : CommandBase
		=> ((options, arguments), globalOptions) = (@this, @this.Parent?.GlobalOptionsCore ?? []);
}
