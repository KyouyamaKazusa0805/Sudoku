using Microsoft.CodeAnalysis;

namespace Sudoku.CodeGen.Deconstruction
{
	/// <summary>
	/// Provides a generator that generates the deconstruction methods that are extension methods.
	/// </summary>
	[Generator]
	public sealed partial class ExtensionDeconstructMethodGenerator : ISourceGenerator
	{
		/// <inheritdoc/>
		public void Execute(GeneratorExecutionContext context)
		{
		}

		/// <inheritdoc/>
		public void Initialize(GeneratorInitializationContext context) =>
			context.RegisterForSyntaxNotifications(static () => new SyntaxReceiver());
	}
}
