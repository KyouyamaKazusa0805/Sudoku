using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;
using Microsoft.CodeAnalysis.Operations;
using Sudoku.CodeGen;
using Sudoku.Diagnostics.CodeAnalysis.Extensions;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	[CodeAnalyzer("SS0301")]
	public sealed partial class LinqAnalyzer : DiagnosticAnalyzer
	{
		/// <summary>
		/// Indicates the full type name of <see cref="Enumerable"/>.
		/// </summary>
		/// <seealso cref="Enumerable"/>
		private const string EnumerableClassFullName = "System.Linq.Enumerable";


		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(
				static context =>
				{
					CheckSS0301(context);
				},
				new[] { SyntaxKind.GreaterThanExpression, SyntaxKind.GreaterThanOrEqualExpression }
			);
		}


		private static void CheckSS0301(SyntaxNodeAnalysisContext context)
		{
			var (semanticModel, compilation, node) = context;

			// Check whether the node is valid:
			//
			//   1) expr.Count() > a
			//   2) expr.Count() >= a
			//
			// Note that 'Take()' method invocation can't exist here.
			if (
				/*length-pattern*/
				node is not BinaryExpressionSyntax
				{
					RawKind: var kind and (
						(int)SyntaxKind.GreaterThanOrEqualExpression or (int)SyntaxKind.GreaterThanExpression
					),
					Left: InvocationExpressionSyntax
					{
						Expression: MemberAccessExpressionSyntax
						{
							RawKind: (int)SyntaxKind.SimpleMemberAccessExpression,
							Expression: var potentialNotTakeMethodInvocationNode,
							Name: IdentifierNameSyntax { Identifier: { ValueText: "Count" } },
						},
						ArgumentList: { Arguments: { Count: 0 } }
					} invocationNode,
					Right: var rightNode
				}
			)
			{
				return;
			}

			// If the invocation is 'Take' method, check whether the method has been passed an argument
			// of type 'int'.
			var int32 = compilation.GetSpecialType(SpecialType.System_Int32);
			if (
				potentialNotTakeMethodInvocationNode is InvocationExpressionSyntax
				{
					Expression: MemberAccessExpressionSyntax
					{
						RawKind: (int)SyntaxKind.SimpleMemberAccessExpression,
						Name: { Identifier: { ValueText: "Take" } }
					},
					ArgumentList: { Arguments: { Count: 1 } arguments }
				}
				/*slice-pattern*/
				&& arguments[0] is { Expression: var expr }
				&& semanticModel.GetOperation(expr) is { Type: { } type }
				&& SymbolEqualityComparer.Default.Equals(type, int32)
			)
			{
				return;
			}

			// Check the method invocation is from type 'System.Linq.Enumerable'.
			if (
				semanticModel.GetOperation(invocationNode) is not IInvocationOperation
				{
					TargetMethod:
					{
						ContainingType: var containingTypeSymbol,
						IsExtensionMethod: true,
						IsGenericMethod: true
					}
				}
			)
			{
				return;
			}

			// Because of 'Take' method invocation, we can judge that the expression to invoke
			// must be of type 'IEnumerable<int>'. Therefore, we don't need to check it.
			// So we don't check whether the expression node is a normal expression.
			// A query expression with a bracket is also okay.
			if (
				!SymbolEqualityComparer.Default.Equals(
					containingTypeSymbol,
					compilation.GetTypeByMetadataName(EnumerableClassFullName)
				)
			)
			{
				return;
			}

			context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: SS0301,
					location: node.GetLocation(),
					messageArgs: new[]
					{
						rightNode.ToString(),
						kind == (int)SyntaxKind.GreaterThanExpression ? ">" : ">="
					},
					additionalLocations: new[]
					{
						potentialNotTakeMethodInvocationNode.GetLocation(),
						rightNode.GetLocation()
					}
				)
			);
		}
	}
}
