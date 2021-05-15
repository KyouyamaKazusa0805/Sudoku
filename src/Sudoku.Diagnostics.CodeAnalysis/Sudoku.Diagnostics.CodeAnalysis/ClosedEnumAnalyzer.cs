using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Sudoku.Diagnostics.CodeAnalysis
{
	/// <summary>
	/// Indicates an analyzer that analyzes the code for closed <see langword="enum"/> types.
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
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
				static context => CheckSudoku022(context),
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


		private static void CheckSudoku022(SyntaxNodeAnalysisContext context)
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
					ReportSudoku022(context, kind, node);

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
					ReportSudoku022(context, kind, node);

					break;
				}
				case BinaryExpressionSyntax
				{
					Left:
					{
						RawKind: (int)SyntaxKind.SimpleMemberAccessExpression or (int)SyntaxKind.IdentifierName
					} operand,
					OperatorToken: { RawKind: var kind }
				} node
				when condition(semanticModel, operand):
				{
					ReportSudoku022(context, kind, node);

					break;
				}
			}

			static bool condition(SemanticModel semanticModel, ExpressionSyntax operand) =>
				semanticModel.GetOperation(operand) is IFieldReferenceOperation { Type: { } type }
				/*length-pattern*/
				&& type.GetAttributes() is { Length: not 0 } attributes
				&& attributes.Any(
					static attributeData => attributeData is
					{
						AttributeClass: { } attributeClass
					} && attributeClass.ToDisplayString(TypeFormat) == ClosedAttributeFullTypeName
				);
		}

		private static void ReportSudoku022(SyntaxNodeAnalysisContext context, int kind, SyntaxNode node)
		{
			context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: new(
						id: DiagnosticIds.Sudoku022,
						title: Titles.Sudoku022,
						messageFormat: Messages.Sudoku022,
						category: Categories.Usage,
						defaultSeverity: DiagnosticSeverity.Error,
						isEnabledByDefault: true,
						helpLinkUri: HelpLinks.Sudoku022
					),
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
