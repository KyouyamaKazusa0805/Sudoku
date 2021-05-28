using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;
using Sudoku.Diagnostics.CodeAnalysis.Extensions;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	/// <summary>
	/// Indicates the analyzer that checks the dynamically invocation of the <see langword="dynamic"/>
	/// field <c>TextResources.Current</c>.
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public sealed partial class DynamicallyInvocationOfCurrentAnalyzer : DiagnosticAnalyzer
	{
		/// <summary>
		/// Indicates the method name that is called in order to change the language of that resource dictionary.
		/// </summary>
		private const string ChangeLanguageMethodName = "ChangeLanguage";

		/// <summary>
		/// Indicates the method name that is called in order to serialize the object.
		/// </summary>
		private const string SerializeMethodName = "Serialize";

		/// <summary>
		/// Indicates the method name that is called in order to deserialize the object.
		/// </summary>
		private const string DeserializeMethodName = "Deserialize";

		/// <summary>
		/// Indicates the country code type name.
		/// </summary>
		private const string CountryCodeTypeName = "Sudoku.Globalization.CountryCode";

		/// <summary>
		/// Indicates the text resources class name.
		/// </summary>
		private const string TextResourcesClassName = "TextResources";

		/// <summary>
		/// Indicates that field dynamically bound.
		/// </summary>
		private const string TextResourcesStaticReadOnlyFieldName = "Current";


		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(
				AnalyzeDynamicInvocation,
				new[] { SyntaxKind.InvocationExpression }
			);
		}

		private static void AnalyzeDynamicInvocation(SyntaxNodeAnalysisContext context)
		{
			var (semanticModel, compilation, n) = context;
			if (compilation.AssemblyName is "Sudoku.UI" or "Sudoku.Windows")
			{
				// We don't check on those two WPF projects, because those two projects has already used
				// their own resource dictionary called 'MergedDictionary'.
				return;
			}

			if (
				n is not InvocationExpressionSyntax
				{
					Expression: MemberAccessExpressionSyntax
					{
						RawKind: (int)SyntaxKind.SimpleMemberAccessExpression,
						Expression: MemberAccessExpressionSyntax
						{
							RawKind: (int)SyntaxKind.SimpleMemberAccessExpression,
							Expression: IdentifierNameSyntax
							{
								Identifier: { ValueText: TextResourcesClassName }
							},
							Name: IdentifierNameSyntax
							{
								Identifier: { ValueText: TextResourcesStaticReadOnlyFieldName }
							}
						},
						Name: IdentifierNameSyntax
						{
							Identifier: { ValueText: var methodName }
						} identifierNameNode
					},
					ArgumentList: var argumentListNode
				} node
			)
			{
				return;
			}

			int actualParamsCount = argumentListNode.Arguments.Count;
			int requiredParamsCount = methodName switch
			{
				ChangeLanguageMethodName => 1,
				SerializeMethodName => 2,
				DeserializeMethodName => 2,
				_ => -1
			};
			if (requiredParamsCount != -1 && actualParamsCount != requiredParamsCount)
			{
				ReportSD0202(context, methodName, identifierNameNode, actualParamsCount, requiredParamsCount);

				goto CheckSudoku012;
			}

			switch (methodName)
			{
				case ChangeLanguageMethodName:
				{
					if (
						!SymbolEqualityComparer.Default.Equals(
							semanticModel.GetOperation(argumentListNode.Arguments[0].Expression)!.Type,
							compilation.GetTypeByMetadataName(CountryCodeTypeName)
						)
					)
					{
						ReportSD0203_Case1(context, semanticModel, identifierNameNode, methodName, argumentListNode);
					}

					break;
				}
				case SerializeMethodName:
				case DeserializeMethodName:
				{
					var expectedTypeSymbol = compilation.GetSpecialType(SpecialType.System_String);
					for (int i = 0; i < 2; i++)
					{
						if (
							!SymbolEqualityComparer.Default.Equals(
								semanticModel.GetOperation(argumentListNode.Arguments[i].Expression)!.Type,
								expectedTypeSymbol
							)
						)
						{
							ReportSD0203_Case2(context, semanticModel, methodName, argumentListNode, i);
						}
					}

					break;
				}
				default:
				{
					ReportSD0206(context, identifierNameNode, methodName);

					break;
				}
			}

		CheckSudoku012:
			if (node.Parent is not ExpressionStatementSyntax)
			{
				ReportSD0204(context, node, methodName);
			}
		}


		private static void ReportSD0206(
			SyntaxNodeAnalysisContext context, IdentifierNameSyntax identifierNameNode, string methodName) =>
			context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: SD0206,
					location: identifierNameNode.GetLocation(),
					messageArgs: new[] { methodName }
				)
			);

		private static void ReportSD0202(
			SyntaxNodeAnalysisContext context, string methodName, IdentifierNameSyntax nameNode,
			int actualParamsCount, int requiredParamsCount) =>
			context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: SD0202,
					location: nameNode.GetLocation(),
					messageArgs: new object[] { methodName, requiredParamsCount, actualParamsCount }
				)
			);

		private static void ReportSD0203_Case1(
			SyntaxNodeAnalysisContext context, SemanticModel semanticModel,
			IdentifierNameSyntax identifierNameNode, string? methodName,
			ArgumentListSyntax argListNode) => context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: SD0203,
					location: identifierNameNode.GetLocation(),
					messageArgs: new object?[]
					{
						methodName,
						CountryCodeTypeName,
						argListNode.Arguments[0].GetParamFullName(semanticModel)
					}
				)
			);

		private static void ReportSD0203_Case2(
			SyntaxNodeAnalysisContext context, SemanticModel semanticModel, string? methodName,
			ArgumentListSyntax argListNode, int i) =>
			context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: SD0203,
					location: argListNode.Arguments[i].GetLocation(),
					messageArgs: new object?[]
					{
						methodName,
						"string",
						argListNode.Arguments[i].GetParamFullName(semanticModel)
					}
				)
			);

		private static void ReportSD0204(
			SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocationNode, string methodName) =>
			context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: SD0204,
					location: invocationNode.GetLocation(),
					messageArgs: new[] { methodName }
				)
			);
	}
}
