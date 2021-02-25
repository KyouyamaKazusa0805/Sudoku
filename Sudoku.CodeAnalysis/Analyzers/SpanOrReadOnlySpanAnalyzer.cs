using System;
using Microsoft.CodeAnalysis;

namespace Sudoku.CodeAnalysis.Analyzers
{
	/// <summary>
	/// Indicates the analyzer that analyzes the type <see cref="Span{T}"/> or <see cref="ReadOnlySpan{T}"/>.
	/// </summary>
	/// <remarks>
	/// All supported diagnostics:
	/// <list type="bullet">
	/// <item><a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3622127&amp;doc_id=633030">SUDOKU017</a> (The result of the expression '.ctor(<see langword="void"/>*, <see cref="int"/>)' can't be the return value as any methods)</item>
	/// </list>
	/// </remarks>
	/// <seealso cref="Span{T}"/>
	/// <seealso cref="ReadOnlySpan{T}"/>
	[Generator]
	public sealed partial class SpanOrReadOnlySpanAnalyzer : ISourceGenerator
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
				var semanticModel = compilation.GetSemanticModel(syntaxTree);
				var collector = new InnerWalker(semanticModel, compilation);
				collector.Visit(root);

				// If the syntax tree doesn't contain any dynamically called clause,
				// just skip it.
				if (collector.Collection is null)
				{
					continue;
				}

				// Iterate on each node.
				foreach (var (fromSpan, node) in collector.Collection)
				{
					context.ReportDiagnostic(
						Diagnostic.Create(
							descriptor: new(
								id: DiagnosticIds.Sudoku017,
								title: Titles.Sudoku017,
								messageFormat: Messages.Sudoku017,
								category: Categories.Usage,
								defaultSeverity: DiagnosticSeverity.Warning,
								isEnabledByDefault: true,
								helpLinkUri: HelpLinks.Sudoku017
							),
							location: node.GetLocation(),
							messageArgs: new[] { fromSpan ? "Span" : "ReadOnlySpan" }
						)
					);
				}
			}
		}

		/// <inheritdoc/>
		public void Initialize(GeneratorInitializationContext context)
		{
		}
	}
}
