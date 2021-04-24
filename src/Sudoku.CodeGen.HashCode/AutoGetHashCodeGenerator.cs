using Microsoft.CodeAnalysis;

namespace Sudoku.CodeGen.HashCode
{
	/// <summary>
	/// Indicates the generator that generates the code that overrides <see cref="object.GetHashCode"/>.
	/// </summary>
	/// <seealso cref="object.GetHashCode"/>
#if false
	[Generator]
#endif
	public sealed partial class AutoGetHashCodeGenerator : ISourceGenerator
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
