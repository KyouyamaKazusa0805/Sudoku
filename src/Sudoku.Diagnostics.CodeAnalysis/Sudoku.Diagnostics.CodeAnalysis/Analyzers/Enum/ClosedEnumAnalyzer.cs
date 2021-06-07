using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using Sudoku.CodeGen;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	[CodeAnalyzer("SS0401")]
	public sealed partial class ClosedEnumAnalyzer : DiagnosticAnalyzer
	{
		/// <summary>
		/// Indicates the full type name of the closed attribute.
		/// </summary>
		private const string ClosedAttributeFullTypeName = "System.Diagnostics.CodeAnalysis.ClosedAttribute";


		/// <summary>
		/// Indicates the type format.
		/// </summary>
		public static readonly SymbolDisplayFormat TypeFormat = new(
			SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining,
			SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
			SymbolDisplayGenericsOptions.IncludeTypeParameters
			| SymbolDisplayGenericsOptions.IncludeTypeConstraints,
			miscellaneousOptions:
				SymbolDisplayMiscellaneousOptions.UseSpecialTypes
				| SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers
				| SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier
		);



		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(
				static context =>
				{
					CheckSS0401(context);
				},
				new[]
				{
					SyntaxKind.UnaryPlusExpression,
					SyntaxKind.UnaryMinusExpression,
					SyntaxKind.AddExpression,
					SyntaxKind.AddAssignmentExpression,
					SyntaxKind.SubtractExpression,
					SyntaxKind.SubtractAssignmentExpression,
					SyntaxKind.MultiplyExpression,
					SyntaxKind.MultiplyAssignmentExpression,
					SyntaxKind.DivideExpression,
					SyntaxKind.DivideAssignmentExpression,
					SyntaxKind.ModuloExpression,
					SyntaxKind.ModuloAssignmentExpression,
					SyntaxKind.ExclusiveOrExpression,
					SyntaxKind.ExclusiveOrAssignmentExpression,
					SyntaxKind.BitwiseAndExpression,
					SyntaxKind.AndAssignmentExpression,
					SyntaxKind.BitwiseOrExpression,
					SyntaxKind.OrAssignmentExpression,
					SyntaxKind.BitwiseNotExpression,
					SyntaxKind.PreIncrementExpression,
					SyntaxKind.PreDecrementExpression,
					SyntaxKind.PostIncrementExpression,
					SyntaxKind.PostDecrementExpression
				}
			);
		}


		private static void CheckSS0401(SyntaxNodeAnalysisContext context)
		{
			var semanticModel = context.SemanticModel;
			switch (context.Node)
			{
				case PrefixUnaryExpressionSyntax
				{
					RawKind: var kind,
					Operand:
					{
						RawKind: (int)SyntaxKind.SimpleMemberAccessExpression or (int)SyntaxKind.IdentifierName
					} operand
				} node
				when condition(semanticModel, operand):
				{
					ReportSS0401(context, kind, node);

					break;
				}
				case PostfixUnaryExpressionSyntax
				{
					RawKind: var kind,
					Operand:
					{
						RawKind: (int)SyntaxKind.SimpleMemberAccessExpression or (int)SyntaxKind.IdentifierName
					} operand
				} node
				when condition(semanticModel, operand):
				{
					ReportSS0401(context, kind, node);

					break;
				}
				case BinaryExpressionSyntax
				{
					RawKind: var kind,
					Left:
					{
						RawKind: (int)SyntaxKind.SimpleMemberAccessExpression or (int)SyntaxKind.IdentifierName
					} operand
				} node
				when condition(semanticModel, operand):
				{
					ReportSS0401(context, kind, node);

					break;
				}
			}

			static bool condition(SemanticModel semanticModel, ExpressionSyntax operand) =>
				semanticModel.GetOperation(operand) switch
				{
					IFieldReferenceOperation { Type: { } type } => innerCondition(type),
					ILocalReferenceOperation { Local: { Type: var type } } => innerCondition(type),
					_ => false
				};

			static bool innerCondition(ITypeSymbol type) =>
				/*length-pattern*/
				type.GetAttributes() is { Length: not 0 } attributes &&
				attributes.Any(
					static attributeData => attributeData is
					{
						AttributeClass: { } attributeClass
					} && attributeClass.ToDisplayString(TypeFormat) == ClosedAttributeFullTypeName
				);
		}

		private static void ReportSS0401(SyntaxNodeAnalysisContext context, int kind, SyntaxNode node)
		{
			context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: SS0401,
					location: node.GetLocation(),
					messageArgs: new[] { p((SyntaxKind)kind) }
				)
			);

			static string p(SyntaxKind kind) => kind switch
			{
				SyntaxKind.UnaryPlusExpression => "+ (unary)",
				SyntaxKind.UnaryMinusExpression => "- (unary)",
				SyntaxKind.AddExpression => "+ (binary)",
				SyntaxKind.AddAssignmentExpression => "+=",
				SyntaxKind.SubtractExpression => "- (binary)",
				SyntaxKind.SubtractAssignmentExpression => "-=",
				SyntaxKind.MultiplyExpression => "*",
				SyntaxKind.MultiplyAssignmentExpression => "*=",
				SyntaxKind.DivideExpression => "/",
				SyntaxKind.DivideAssignmentExpression => "/=",
				SyntaxKind.ModuloExpression => "%",
				SyntaxKind.ModuloAssignmentExpression => "%=",
				SyntaxKind.ExclusiveOrExpression => "^",
				SyntaxKind.ExclusiveOrAssignmentExpression => "^=",
				SyntaxKind.BitwiseAndExpression => "&",
				SyntaxKind.AndAssignmentExpression => "&=",
				SyntaxKind.BitwiseOrExpression => "|",
				SyntaxKind.OrAssignmentExpression => "|=",
				SyntaxKind.BitwiseNotExpression => "~",
				SyntaxKind.PreIncrementExpression => "++ (prefix)",
				SyntaxKind.PreDecrementExpression => "-- (prefix)",
				SyntaxKind.PostIncrementExpression => "++ (postfix)",
				SyntaxKind.PostDecrementExpression => "-- (postfix)"
			};
		}
	}
}
