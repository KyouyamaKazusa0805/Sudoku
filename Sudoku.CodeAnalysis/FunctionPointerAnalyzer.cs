using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Sudoku.CodeAnalysis
{
	/// <summary>
	/// Indicates the function pointer analyzer.
	/// </summary>
	[Generator]
	public sealed class FunctionPointerAnalyzer : ISourceGenerator
	{
		/// <inheritdoc/>
		public void Execute(GeneratorExecutionContext context)
		{
			var compilation = context.Compilation;
			foreach (var syntaxTree in compilation.SyntaxTrees)
			{
				// Check whether the syntax contains the root node.
				if (!syntaxTree.TryGetRoot(out var root))
				{
					continue;
				}

				// Create the semantic model and the property list.
				var collector = new FunctionPointerTypeSyntaxNodeSearcher();
				collector.Visit(root);

				// If the syntax tree doesn't contain any dynamically called clause,
				// just skip it.
				if (collector.Collection is null)
				{
					continue;
				}

				// Iterate on each location.
				foreach (var node in collector.Collection)
				{
					// No calling conversion.
					context.ReportDiagnostic(
						Diagnostic.Create(
							descriptor: new(
								id: DiagnosticIds.Sudoku015,
								title: Titles.Sudoku015,
								messageFormat: Messages.Sudoku015,
								category: Categories.Usage,
								defaultSeverity: DiagnosticSeverity.Warning,
								isEnabledByDefault: true,
								helpLinkUri: HelpLinks.Sudoku015
							),
							location: node.GetLocation(),
							messageArgs: null
						)
					);
				}
			}
		}

		/// <inheritdoc/>
		public void Initialize(GeneratorInitializationContext context)
		{
		}


		/// <summary>
		/// Indicates the searcher that searches for function pointer syntax node.
		/// </summary>
		private sealed class FunctionPointerTypeSyntaxNodeSearcher : CSharpSyntaxWalker
		{
			/// <summary>
			/// Indicates the result list.
			/// </summary>
			public IList<FunctionPointerTypeSyntax>? Collection { get; private set; }


			/// <inheritdoc/>
			public override void VisitFunctionPointerType(FunctionPointerTypeSyntax node)
			{
				if (node.CallingConvention is not null)
				{
					return;
				}

				Collection ??= new List<FunctionPointerTypeSyntax>();

				Collection.Add(node);
			}
		}
	}
}
