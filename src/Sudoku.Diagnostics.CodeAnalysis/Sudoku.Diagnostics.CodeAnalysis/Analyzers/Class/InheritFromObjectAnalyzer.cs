using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;
using Sudoku.CodeGen;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	[CodeAnalyzer("SS9006")]
	public sealed partial class InheritFromObjectAnalyzer : DiagnosticAnalyzer
	{
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

			switch (originalNode)
			{
				/*length-pattern*/
				case ClassDeclarationSyntax
				{
					BaseList: { Types: { Count: not 0 } types } baseList,
					Identifier: var identifier
				} node
				/*slice-pattern*/
				when types[0] is SimpleBaseTypeSyntax { Type: var innerType }
				&& SymbolEqualityComparer.Default.Equals(
					semanticModel.GetSymbolInfo(innerType, context.CancellationToken).Symbol,
					compilation.GetSpecialType(SpecialType.System_Object)
				):
				{
					// Check whether the first base type is 'object'.
					// Because C# requires you should place base class type at first,
					// so we only need to check the first type.
					context.ReportDiagnostic(
						Diagnostic.Create(
							descriptor: SS9006,
							location: identifier.GetLocation(),
							messageArgs: null,
							additionalLocations: new[] { baseList.GetLocation() }
						)
					);

					break;
				}

				/*length-pattern*/
				case RecordDeclarationSyntax
				{
					BaseList: { Types: { Count: not 0 } types } baseList,
					Identifier: var identifier
				} node
				/*slice-pattern*/
				when types[0] is SimpleBaseTypeSyntax { Type: var innerType }
				&& SymbolEqualityComparer.Default.Equals(
					semanticModel.GetSymbolInfo(innerType, context.CancellationToken).Symbol,
					compilation.GetSpecialType(SpecialType.System_Object)
				):
				{
					// Check whether the first base type is 'object'.
					// Because C# requires you should place base class type at first,
					// so we only need to check the first type.
					context.ReportDiagnostic(
						Diagnostic.Create(
							descriptor: SS9006,
							location: identifier.GetLocation(),
							messageArgs: null,
							additionalLocations: new[] { baseList.GetLocation() }
						)
					);

					break;
				}
			}
		}
	}
}
