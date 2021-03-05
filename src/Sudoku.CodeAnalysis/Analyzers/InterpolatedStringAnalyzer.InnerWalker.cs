using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace Sudoku.CodeAnalysis.Analyzers
{
	partial class InterpolatedStringAnalyzer
	{
		/// <summary>
		/// Verify the diagnostic item <c>SUDOKU016</c>.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="root">The syntax root.</param>
		/// <param name="model">The semantic model.</param>
		partial void VerifySudoku016(GeneratorExecutionContext context, SyntaxNode root, SemanticModel model)
		{
			var collector = new InnerWalker_ValueTypeInterpolation(model);
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
								id: DiagnosticIds.Sudoku016,
								title: Titles.Sudoku016,
								messageFormat: Messages.Sudoku016,
								category: Categories.Performance,
								defaultSeverity: DiagnosticSeverity.Warning,
								isEnabledByDefault: true,
								helpLinkUri: HelpLinks.Sudoku016
							),
							location: interpolation.GetLocation(),
							messageArgs: null
						)
					);
				}
			}
		}

		/// <summary>
		/// Bound by the method <see cref="VerifySudoku016"/>.
		/// </summary>
		/// <seealso cref="VerifySudoku016"/>
		private sealed class InnerWalker_ValueTypeInterpolation : CSharpSyntaxWalker
		{
			/// <summary>
			/// Indicates the semantic model.
			/// </summary>
			private readonly SemanticModel _semanticModel;


			/// <summary>
			/// Initializes an instance with the specified semantic model.
			/// </summary>
			/// <param name="semanticModel">The semantic model.</param>
			public InnerWalker_ValueTypeInterpolation(SemanticModel semanticModel) =>
				_semanticModel = semanticModel;


			/// <summary>
			/// Indicates the result list.
			/// </summary>
			public IList<InterpolationSyntax>? Collection { get; private set; }


			/// <inheritdoc/>
			public override void VisitInterpolation(InterpolationSyntax node)
			{
				if
				(
					_semanticModel.GetOperation(node) is not IInterpolationOperation
					{
						Kind: OperationKind.Interpolation,
						Expression: { Type: { IsValueType: true } }
					}
				)
				{
					return;
				}

				Collection ??= new List<InterpolationSyntax>();

				Collection.Add(node);
			}
		}
	}
}
