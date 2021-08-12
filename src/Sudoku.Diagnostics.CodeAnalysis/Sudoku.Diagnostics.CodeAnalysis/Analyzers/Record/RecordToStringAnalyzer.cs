using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;
using Sudoku.CodeGenerating;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[CodeAnalyzer("SS9004")]
public sealed partial class RecordToStringAnalyzer : DiagnosticAnalyzer
{
	/// <inheritdoc/>
	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, new[] { SyntaxKind.InvocationExpression });
	}


	private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
	{
		var (semanticModel, _, originalNode) = context;

		if (
			originalNode is not InvocationExpressionSyntax
			{
				Expression: MemberAccessExpressionSyntax
				{
					RawKind: (int)SyntaxKind.SimpleMemberAccessExpression,
					Expression: var expr,
					Name: { Identifier: { ValueText: "ToString" } }
				},
				ArgumentList: { Arguments: { Count: 0 } }
			}
		)
		{
			return;
		}

		if (semanticModel.GetOperation(expr) is not { Type: { IsRecord: true } type })
		{
			return;
		}

		var members = type.GetMembers();
		if (
			!members.OfType<IMethodSymbol>().Any(
				static method => method is { IsImplicitlyDeclared: true, Name: "ToString" }
			)
		)
		{
			return;
		}

		if (
			members.Any(member => member switch
			{
				IFieldSymbol { CanBeReferencedByName: true, Type: var fieldType } =>
					SymbolEqualityComparer.Default.Equals(type, fieldType),
				IPropertySymbol { CanBeReferencedByName: true, Type: var propertyType } =>
					SymbolEqualityComparer.Default.Equals(type, propertyType),
				_ => false
			})
		)
		{
			return;
		}

		context.ReportDiagnostic(
			Diagnostic.Create(
				descriptor: SS9004,
				location: originalNode.GetLocation(),
				messageArgs: null
			)
		);
	}
}
