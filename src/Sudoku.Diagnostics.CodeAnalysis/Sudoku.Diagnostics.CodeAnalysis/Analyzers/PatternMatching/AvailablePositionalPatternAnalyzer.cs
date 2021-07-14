using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;
using Microsoft.CodeAnalysis.Operations;
using Sudoku.CodeGenerating;
using Sudoku.Diagnostics.CodeAnalysis.Extensions;
using InfoTuple = System.ValueTuple<
	string, // Local
	string, // MemberName
	Microsoft.CodeAnalysis.ITypeSymbol, // Type
	string, // LeftExpr
	string, // RightExpr
	bool, // IsConstantExprLeft
	bool // IsNotEquals
>;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	[CodeAnalyzer("SS0606")]
	public sealed partial class AvailablePositionalPatternAnalyzer : DiagnosticAnalyzer
	{
		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, new[] { SyntaxKind.LogicalAndExpression });
		}


		private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
		{
			var (semanticModel, _, originalNode) = context;

			if (
				originalNode is not BinaryExpressionSyntax
				{
					Parent: not BinaryExpressionSyntax { RawKind: (int)SyntaxKind.LogicalAndExpression }
				} node
			)
			{
				return;
			}

			// Then checks its ancestors.
			foreach (var currentNode in originalNode.Ancestors())
			{
				if (
					semanticModel.GetOperation(currentNode) is IMethodBodyOperation
					{
						BlockBody: { Locals: { Length: not 0 } locals }
					}
				)
				{
					AnalyzeSyntaxNode(context, semanticModel, node, locals);

					return;
				}
			}
		}

		private static void AnalyzeSyntaxNode(
			SyntaxNodeAnalysisContext context, SemanticModel semanticModel,
			BinaryExpressionSyntax node, ImmutableArray<ILocalSymbol> locals)
		{
			// Suppose we have a varible named 'v':
			// The expression should be detected, as the goal:
			//
			//     v.A == 1 && v.B == 2 && v.C == 3
			//
			// As what you see, the expression is a logical and expression,
			// and all sub-expressions are the same condition to satisfy:
			//
			//   1) The left-side expression is always a simple access member expression (dot expression).
			//   2) The right-side expression is always a constant expression.
			//   3) The operator is always '==' or '!='.
			//
			// And all sub-expressions should be connected by logical and operator '&&'.
			// The analyzer should check and detect on this case, because this expression
			// can be simplified to a positional pattern:
			//
			//    v is (a: 1, b: 2, c: 3)
			//
			// Here the name 'a', 'b' and 'c' is the parameter name of the corresponding
			// deconstruction method.
			// Of course, some details may not be expand in this comment block,
			// such as the two expression in the in-equality expression
			// can exchange the position with each other.
			// In other words, we can also place the expression 'v.A' to the right side, and place
			// the constant expression '1' to the left side.

			// Firstly, get the whole expression, and stores the list of the sub-expressions.
			var currentNode = node;
			var recursiveSubexprs = new List<BinaryExpressionSyntax>();
			while (
				currentNode is
				{
					RawKind: (int)SyntaxKind.LogicalAndExpression,
					Left: BinaryExpressionSyntax leftSubexpr,
					Right: BinaryExpressionSyntax
					{
						RawKind: (int)SyntaxKind.EqualsExpression or (int)SyntaxKind.NotEqualsExpression
					}
				}
			)
			{
				recursiveSubexprs.Add(currentNode);

				if (
					leftSubexpr is
					{
						RawKind: (int)SyntaxKind.EqualsExpression or (int)SyntaxKind.NotEqualsExpression
					}
				)
				{
					recursiveSubexprs.Add(leftSubexpr);
					break;
				}

				currentNode = leftSubexpr;
			}

			// Then get all sub expressions.
			// If the whole is like this tree:
			//
			//                  Here:
			//         A          A: operator &&
			//        / \         B: operator &&
			//       /   \        C: r._c == 3
			//      B     C       D: r._a == 1
			//     / \            E: r._b == 2
			//    /   \
			//   D     E          The whole expression: r._a == 1 && r._b == 2 && r._c == 3

			// Gather expressions.
			var subexprs = new BinaryExpressionSyntax[recursiveSubexprs.Count];
			for (int i = 0, count = recursiveSubexprs.Count; i < count; i++)
			{
				var n = recursiveSubexprs[i];
				if (i != count - 1)
				{
					if (
						n is not
						{
							Left: BinaryExpressionSyntax leftExpr,
							Right: BinaryExpressionSyntax rightExpr
						}
					)
					{
						return;
					}

					subexprs[i] = rightExpr;
				}
				else
				{
					subexprs[i] = n;
				}
			}

			Array.Reverse(subexprs);

			if (
				subexprs.Any(static subexpr => subexpr is
				{
					RawKind: not ((int)SyntaxKind.EqualsExpression or (int)SyntaxKind.NotEqualsExpression)
				})
			)
			{
				return;
			}

			// Checks all sub-expression are simple member access expression,
			// and another-side expression is a constant one.
			bool phase1Flag = true;
			var info = new List<InfoTuple>();
			foreach (var subexpr in subexprs)
			{
				// Deconstruct the sub-expression to left and right those two expressions.
				if (subexpr is not { Left: var leftExpr, Right: var rightExpr })
				{
					phase1Flag = false;
					break;
				}

				// Here we should check one should be a simple member access expression,
				// and another is a constant expression.
				var pair = new[] { (leftExpr, rightExpr), (rightExpr, leftExpr) };
				bool success = false;
				for (int i = 0; i < 2; i++)
				{
					var (memberAccess, constant) = pair[i];
					if (
						memberAccess is not MemberAccessExpressionSyntax
						{
							RawKind: (int)SyntaxKind.SimpleMemberAccessExpression,
							Expression: IdentifierNameSyntax
							{
								Identifier: { ValueText: var identifierName }
							} identifierNameNode,
							Name: { Identifier: { ValueText: var fieldOrPropertyName } }
						}
					)
					{
						continue;
					}

					if (semanticModel.GetOperation(constant) is not { ConstantValue: { HasValue: true } })
					{
						continue;
					}

					var type = semanticModel.GetOperation(identifierNameNode)!.Type!;

					info.Add(
						(
							identifierName,
							fieldOrPropertyName,
							type,
							leftExpr.ToString(),
							rightExpr.ToString(),
							i == 1,
							subexpr.RawKind == (int)SyntaxKind.NotEqualsExpression
						)
					);

					success = true;
					break;
				}

				if (!success)
				{
					phase1Flag = false;
					break;
				}
			}

			if (!phase1Flag || info.Count == 0)
			{
				return;
			}

			// Check same variable, and same type.
			bool phase2flag = true;
			string? storedIdentifierName = null;
			ITypeSymbol? storedTypeSymbol = null;
			foreach (var (identifierName, _, type, _, _, _, _) in info)
			{
				if (storedIdentifierName is null)
				{
					storedIdentifierName = identifierName;
				}
				else if (storedIdentifierName != identifierName)
				{
					phase2flag = false;
					break;
				}

				if (storedTypeSymbol is null)
				{
					storedTypeSymbol = type;
				}
				else if (!SymbolEqualityComparer.IncludeNullability.Equals(storedTypeSymbol, type))
				{
					phase2flag = false;
					break;
				}
			}

			if (!phase2flag)
			{
				return;
			}

			// Now check whether the variable can be referenced to a local variable.
			var (identifier, _, localType, _, _, _, _) = info[0];
			if (locals.Any(local => local.Name != identifier))
			{
				return;
			}

			CheckNormalDeconstructionMethod(context, node, localType, info, identifier);
			CheckRecordPrimaryConstructor(context, node, localType, info, identifier);
		}

		private static void CheckRecordPrimaryConstructor(
			SyntaxNodeAnalysisContext context, BinaryExpressionSyntax node, ITypeSymbol localType,
			IReadOnlyList<InfoTuple> info, string identifier)
		{
			if (localType is not { IsRecord: true })
			{
				return;
			}

			// Check primary constructor in record.
			var ctors = localType.GetMembers().OfType<IMethodSymbol>();
			foreach (var ctor in ctors)
			{
				if (ctor.Name is not ("" or ".ctor"))
				{
					continue;
				}

				var parameters = ctor.Parameters;

				// Checks whether all member reference and be corresponded to the parameters.
				bool matchFlag = true;
				foreach (string memberName in from tuple in info select tuple.Item2)
				{
					if (parameters.All(parameter => parameter.Name != memberName))
					{
						matchFlag = false;
						break;
					}
				}
				if (!matchFlag)
				{
					continue;
				}

				// Only find one is okay.
				// Here we sort the whole method symbols by the number of parameters,
				// so the first found one holds the lowest number of parameters, which is the best case.
				if (ReportSS0606(context, node, info, identifier))
				{
					return;
				}
			}
		}

		private static void CheckNormalDeconstructionMethod(
			SyntaxNodeAnalysisContext context, SyntaxNode node, ITypeSymbol localType,
			IReadOnlyList<InfoTuple> info, string identifier)
		{
			// Same type, same variable, and satistied equals or not equals expression, and variable matched.
			// Now we should check the type symbol, and get all possible deconstruction methods
			// and determine whether the all referenced members can be referenced to one deconstruct method
			// parameters.
			var possibleDeconstructionMethods = localType.GetAllDeconstructionMethods();
			if (!possibleDeconstructionMethods.Any())
			{
				return;
			}

			// Iterate on each possible deconstruction method symbol.
			foreach (var possibleDeconstructionMethod in possibleDeconstructionMethods)
			{
				var parameters = possibleDeconstructionMethod.Parameters;

				// Checks whether all member reference and be corresponded to the parameters.
				bool matchFlag = true;
				foreach (string memberName in from tuple in info select tuple.Item2.ToCamelCase())
				{
					if (parameters.All(parameter => parameter.Name != memberName))
					{
						matchFlag = false;
						break;
					}
				}
				if (!matchFlag)
				{
					continue;
				}

				// Only find one is okay.
				// Here we sort the whole method symbols by the number of parameters,
				// so the first found one holds the lowest number of parameters, which is the best case.
				if (ReportSS0606(context, node, info, identifier))
				{
					return;
				}
			}
		}

		private static bool ReportSS0606(
			SyntaxNodeAnalysisContext context, SyntaxNode node,
			IReadOnlyList<InfoTuple> info, string identifier)
		{
			var positionalPatternExprSb = new StringBuilder()
				.Append(identifier)
				.Append(" is (")
				.Append(
					string.Join(
						", ",
						from tuple in info
						let parameterName = tuple.Item2.ToCamelCase()
						let constantExpr = tuple.Item6 ? tuple.Item4 : tuple.Item5
						let notKeyword = tuple.Item7 ? "not " : string.Empty
						select $"{parameterName}: {notKeyword}{constantExpr}"
					)
				)
				.Append(")");

			context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: SS0606,
					location: node.GetLocation(),
					messageArgs: new[] { positionalPatternExprSb.ToString() }
				)
			);

			return true;
		}
	}
}
