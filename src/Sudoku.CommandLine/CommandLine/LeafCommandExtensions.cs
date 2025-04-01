namespace Sudoku.CommandLine;

/// <summary>
/// Provides with extension methods on <see cref="ILeafCommand"/>.
/// </summary>
/// <seealso cref="ILeafCommand"/>
public static class LeafCommandExtensions
{
	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	public static void Deconstruct<TCommand>(this TCommand @this, out SymbolList<Option> options, out SymbolList<Argument> arguments)
		where TCommand : Command, ILeafCommand
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
		where TCommand : Command, ILeafCommand
		=> ((options, arguments), globalOptions) = (@this, @this.Parent?.GlobalOptionsCore ?? []);
}
