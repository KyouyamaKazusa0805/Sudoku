using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;
using Microsoft.CodeAnalysis.Operations;
using Sudoku.CodeGenerating;

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
					OperatorKind: var kind and (BinaryOperatorKind.Equals or BinaryOperatorKind.NotEquals),
					LeftOperand: { Type: var lType, Syntax: var lSyntax },
					RightOperand: { Type: var rType, Syntax: var rSyntax },
					Syntax: var node
				}
			)
			{
				return;
			}

			var objectType = compilation.GetSpecialType(SpecialType.System_Object);
			var d = SymbolEqualityComparer.Default;
			((Action)(d.Equals(lType, rType) && d.Equals(lType, objectType) ? f : static () => { }))();

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			void f() => context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: SS0706,
					location: node.GetLocation(),
					properties: ImmutableDictionary.CreateRange(
						new KeyValuePair<string, string?>[]
						{
							new("OperationKind", kind == BinaryOperatorKind.Equals ? "==" : "!=")
						}
					),
					messageArgs: null,
					additionalLocations: new[] { lSyntax.GetLocation(), rSyntax.GetLocation() }
				)
			);
		}
	}
}
