using Microsoft.CodeAnalysis;

namespace Sudoku.CodeAnalysis
{
	/// <summary>
	/// Indicates the analyzer that checks the dynamically invocation of the <see langword="dynamic"/>
	/// field <c>TextResources.Current</c>.
	/// </summary>
	[Generator]
	public sealed partial class DynamicallyInvocationOfCurrentAnalyzer : ISourceGenerator
	{
		/// <summary>
		/// Indicates the text resources class name.
		/// </summary>
		private const string TextResourcesClassName = "TextResources";

		/// <summary>
		/// Indicates that field dynamically bound.
		/// </summary>
		private const string TextResourcesStaticReadOnlyFieldName = "Current";


		/// <inheritdoc/>
		public void Execute(GeneratorExecutionContext context)
		{
			var compilation = context.Compilation;
			if (compilation.AssemblyName is "Sudoku.UI" or "Sudoku.Windows")
			{
				// We don't check on those two WPF projects, because those two projects has already used
				// their own resource dictionary (MergedDictionary).
				return;
			}

			foreach (var syntaxTree in compilation.SyntaxTrees)
			{
				// Check whether the syntax contains the root node.
				if (!syntaxTree.TryGetRoot(out var root))
				{
					continue;
				}

				// Create the semantic model and the property list.
				var semanticModel = compilation.GetSemanticModel(syntaxTree);
				var collector = new DynamicallyInvocationSearcher(semanticModel);
				collector.Visit(root);

				// If the syntax tree doesn't contain any dynamically called clause,
				// just skip it.
				if (collector.Collection is null)
				{
					continue;
				}

				// Iterate on each dynamically called location.
				foreach (var (node, methodName, argNodes) in collector.Collection)
				{

				}
			}
		}

		/// <inheritdoc/>
		public void Initialize(GeneratorInitializationContext context)
		{
		}
	}
}
