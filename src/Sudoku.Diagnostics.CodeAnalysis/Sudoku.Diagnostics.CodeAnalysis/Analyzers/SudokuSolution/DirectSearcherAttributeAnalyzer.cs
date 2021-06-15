using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;
using Sudoku.CodeGen;
using Sudoku.Diagnostics.CodeAnalysis.Extensions;
using Comparer = Microsoft.CodeAnalysis.SymbolEqualityComparer;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	[CodeAnalyzer("SD0403")]
	public sealed partial class DirectSearcherAttributeAnalyzer : DiagnosticAnalyzer
	{
		/// <summary>
		/// Indicates the type name of the <c>StepSearcher</c>.
		/// </summary>
		private const string StepSearcherTypeName = "Sudoku.Solving.Manual.StepSearcher";

		/// <summary>
		/// Indicates the type name of the <c>DirectSearcherAttribute</c>.
		/// </summary>
		private const string AttributeTypeName = "Sudoku.Solving.Manual.DirectSearcherAttribute";


		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(
				AnalyzeSyntaxNode,
				new[] { SyntaxKind.ClassDeclaration, SyntaxKind.RecordDeclaration }
			);
		}


		private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
		{
			var (semanticModel, compilation, originalNode) = context;

			var attributeSymbol = compilation.GetTypeByMetadataName(AttributeTypeName);
			switch (originalNode)
			{
				/*length-pattern*/
				case RecordDeclarationSyntax { Identifier: { ValueText: var recordName } identifier }:
				{
					var attributeList = semanticModel.GetDeclaredSymbol(originalNode)!.GetAttributes();
					if (attributeList.Length == 0)
					{
						break;
					}

					if (attributeList.All(a => !Comparer.Default.Equals(a.AttributeClass, attributeSymbol)))
					{
						break;
					}

					context.ReportDiagnostic(
						Diagnostic.Create(
							descriptor: SD0403,
							location: identifier.GetLocation(),
							messageArgs: new[] { recordName }
						)
					);

					break;
				}

				/*length-pattern*/
				case ClassDeclarationSyntax { Identifier: { ValueText: var className } identifier }:
				{
					var attributeList = semanticModel.GetDeclaredSymbol(originalNode)!.GetAttributes();
					if (attributeList.Length == 0)
					{
						break;
					}

					if (attributeList.All(a => !Comparer.Default.Equals(a.AttributeClass, attributeSymbol)))
					{
						break;
					}

					var currentSymbol = (INamedTypeSymbol)semanticModel.GetDeclaredSymbol(originalNode)!;
					var stepSearcherSymbol = compilation.GetTypeByMetadataName(StepSearcherTypeName);
					if (currentSymbol.DerivedFrom(stepSearcherSymbol))
					{
						break;
					}

					context.ReportDiagnostic(
						Diagnostic.Create(
							descriptor: SD0403,
							location: identifier.GetLocation(),
							messageArgs: new[] { className }
						)
					);

					break;
				}
			}
		}
	}
}
