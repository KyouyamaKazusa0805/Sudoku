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
using Sudoku.Diagnostics.CodeAnalysis.Extensions;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	/// <summary>
	/// Indicates an analyzer that analyzes the code for the positional pattern matching.
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public sealed partial class PositionalPatternAnalyzer : DiagnosticAnalyzer
	{
		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(
				AnalyzeSyntaxNode,
				new[] { SyntaxKind.MethodDeclaration, SyntaxKind.CompilationUnit }
			);
		}


		private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
		{
			var (semanticModel, _, originalNode) = context;
			switch (originalNode)
			{
				case MethodDeclarationSyntax methodDeclaration
				when tryGetLocals(semanticModel, methodDeclaration, out var locals):
				{
					traverseDescendants(methodDeclaration, locals);

					break;
				}
				case CompilationUnitSyntax compilationUnit
				when tryGetLocals(semanticModel, compilationUnit, out var locals):
				{
					traverseDescendants(compilationUnit, locals);

					break;
				}
			}

			static bool tryGetLocals(
				SemanticModel semanticModel, SyntaxNode node, out ImmutableArray<ILocalSymbol> locals)
			{
				if (
					semanticModel.GetOperation(node) is IMethodBodyOperation
					{
						/*length-pattern*/
						BlockBody: { Locals: { Length: not 0 } l }
					}
				)
				{
					locals = l;
					return true;
				}
				else
				{
					locals = default;
					return false;
				}
			}

			void traverseDescendants(SyntaxNode node, ImmutableArray<ILocalSymbol> locals)
			{
				foreach (var descendant in
					from descendant in node.DescendantNodes().OfType<BinaryExpressionSyntax>()
					where descendant is
					{
						RawKind: (int)SyntaxKind.LogicalAndExpression,
						Parent: not BinaryExpressionSyntax { RawKind: (int)SyntaxKind.LogicalAndExpression }
					}
					select descendant)
				{
					AnalyzeSyntaxNode(context, semanticModel, descendant, locals);
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

			/*slice-pattern*/
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
			var info = new List<
				(
					string Local, string MemberName, ITypeSymbol Type,
					string LeftExpr, string RightExpr, bool IsConstantExprLeft, bool IsNotEquals
				)
			>();
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

			// Same type, same variable, and satistied equals or not equals expression, and variable matched.
			// Now we should check the type symbol, and get all possible deconstruction methods
			// and determine whether the all referenced members can be referenced to one deconstruct method
			// parameters.
			var possibleDeconstructionMethods =
				from methodSymbol in localType.GetMembers().OfType<IMethodSymbol>()
				where methodSymbol.IsDeconstructionMethod()
				let parameters = methodSymbol.Parameters
				orderby parameters.Length
				select methodSymbol;

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
				foreach (string memberName in from tuple in info select tuple.MemberName.ToCamelCase())
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

				var positionalPatternExprSb = new StringBuilder()
					.Append(identifier)
					.Append(" is (")
					.Append(
						string.Join(
							", ",
							from tuple in info
							let parameterName = tuple.MemberName.ToCamelCase()
							let constantExpr = tuple.IsConstantExprLeft ? tuple.LeftExpr : tuple.RightExpr
							let notKeyword = tuple.IsNotEquals ? "not " : string.Empty
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

				// Only find one is okay.
				return;
			}
		}
	}
}
