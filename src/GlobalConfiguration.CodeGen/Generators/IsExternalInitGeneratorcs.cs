namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines a source generator that generates the source code for the type <c>IsExternalInit</c>.
/// The type is only used for <see langword="init"/> properties, to distinct with normal setters.
/// </summary>
[Generator]
public sealed class IsExternalInitGenerator : ISourceGenerator
{
	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context) =>
		context.AddSource(
			"IsExternalInit.g.cs",
			@"#nullable enable

namespace System.Runtime.CompilerServices;

#if !NET && (NETSTANDARD || NETFRAMEWORK)
/// <summary>
/// Declare this to get init properties.
/// </summary>
/// <remarks>
/// Please see
/// <see href=""https://github.com/dotnet/roslyn/issues/45510#issuecomment-694977239"">this link</see>
/// to learn more details about this type.
/// </remarks>
internal static class IsExternalInit
{
}
#endif
"
		);

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context)
	{
	}
}
