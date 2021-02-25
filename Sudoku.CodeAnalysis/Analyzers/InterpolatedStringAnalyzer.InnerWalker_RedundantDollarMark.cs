using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Sudoku.CodeAnalysis.Analyzers
{
	partial class InterpolatedStringAnalyzer
	{
		/// <summary>
		/// Verify the diagnostic item <c>SUDOKU020</c>.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="root">The syntax root.</param>
		partial void VerifySudoku020(GeneratorExecutionContext context, SyntaxNode root)
		{
			var collector = new InnerWalker_RedundantDollarMark();
			collector.Visit(root);

			// If the syntax tree doesn't contain any dynamically called clause,
			// just skip it.
			if (collector.Collection is not null)
			{
				// Iterate on each location.
				foreach (var interpolation in collector.Collection)
				{
					// No calling conversion.
					context.ReportDiagnostic(
						Diagnostic.Create(
							descriptor: new(
								id: DiagnosticIds.Sudoku020,
								title: Titles.Sudoku020,
								messageFormat: Messages.Sudoku020,
								category: Categories.Usage,
								defaultSeverity: DiagnosticSeverity.Warning,
								isEnabledByDefault: true,
								helpLinkUri: HelpLinks.Sudoku020
							),
							location: interpolation.GetLocation(),
							messageArgs: null
						)
					);
				}
			}
		}


		/// <summary>
		/// Bound by the method <see cref="VerifySudoku020"/>.
		/// </summary>
		/// <seealso cref="VerifySudoku020"/>
		private sealed class InnerWalker_RedundantDollarMark : CSharpSyntaxWalker
		{
			/// <summary>
			/// Indicates the result list.
			/// </summary>
			public IList<InterpolatedStringExpressionSyntax>? Collection { get; private set; }


			/// <inheritdoc/>
			public override void VisitInterpolatedStringExpression(InterpolatedStringExpressionSyntax node)
			{
				if (node.DescendantNodes().OfType<InterpolationSyntax>().Any())
				{
					return;
				}

				Collection ??= new List<InterpolatedStringExpressionSyntax>();

				Collection.Add(node);
			}
		}
	}
}
