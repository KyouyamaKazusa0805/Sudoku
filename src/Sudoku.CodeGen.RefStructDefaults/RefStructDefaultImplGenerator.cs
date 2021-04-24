using Microsoft.CodeAnalysis;

namespace Sudoku.CodeGen.RefStructDefaults
{
	/// <summary>
	/// Indicates the generator that generates the default overriden methods in a <see langword="ref struct"/>.
	/// </summary>
	[Generator]
	public sealed class RefStructDefaultImplGenerator : ISourceGenerator
	{
		/// <inheritdoc/>
		public void Execute(GeneratorExecutionContext context)
		{
		}

		/// <inheritdoc/>
		public void Initialize(GeneratorInitializationContext context)
		{
		}
	}
}
