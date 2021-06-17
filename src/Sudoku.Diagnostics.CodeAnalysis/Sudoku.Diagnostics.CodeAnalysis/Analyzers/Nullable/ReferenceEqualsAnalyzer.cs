using System;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;
using Microsoft.CodeAnalysis.Operations;
using Sudoku.CodeGen;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	[CodeAnalyzer("SS0706")]
	public sealed partial class ReferenceEqualsAnalyzer : DiagnosticAnalyzer
	{
		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterOperationAction(AnalyzeOperation, new[] { OperationKind.Binary });
		}


		private static void AnalyzeOperation(OperationAnalysisContext context)
		{
			var (compilation, operation) = context;

			if (
				operation is not IBinaryOperation
				{
					OperatorKind: BinaryOperatorKind.Equals or BinaryOperatorKind.NotEquals,
					LeftOperand: { Type: var lType, Syntax: var lSyntax },
					RightOperand: { Type: var rType, Syntax: var rSyntax },
					Syntax: var node
				}
			)
			{
				return;
			}

			const int n = (int)SyntaxKind.NullLiteralExpression;
			var o = compilation.GetSpecialType(SpecialType.System_Object);
			var d = SymbolEqualityComparer.Default;
			Action? a = (lSyntax, rSyntax) switch
			{
				(LiteralExpressionSyntax { RawKind: n }, _) when d.Equals(rType, o) => f,
				(_, LiteralExpressionSyntax { RawKind: n }) when d.Equals(lType, o) => f,
				_ when d.Equals(lType, rType) && d.Equals(lType, o) => f,
				_ => null
			};

			a?.Invoke();

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			void f() => context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: SS0706,
					location: node.GetLocation(),
					messageArgs: null
				)
			);
		}
	}
}
