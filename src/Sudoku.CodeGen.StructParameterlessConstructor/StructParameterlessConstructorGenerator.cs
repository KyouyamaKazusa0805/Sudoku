using Microsoft.CodeAnalysis;

namespace Sudoku.CodeGen.StructParameterlessConstructor
{
	/// <summary>
	/// Defines a generator that controls generating parameterless constructor
	/// in <see langword="struct"/>s automatically.
	/// </summary>
	/// <remarks>
	/// C# 10 or later supports the feature "parameterless constructor in <see langword="struct"/>s",
	/// which allows us customize a parameterless constructor in a <see langword="struct"/>
	/// that don't effect on <see langword="default"/> expression
	/// (e.g. <see langword="default"/>(<see langword="int"/>)).
	/// </remarks>
	[Generator]
	public sealed class StructParameterlessConstructorGenerator : ISourceGenerator
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
