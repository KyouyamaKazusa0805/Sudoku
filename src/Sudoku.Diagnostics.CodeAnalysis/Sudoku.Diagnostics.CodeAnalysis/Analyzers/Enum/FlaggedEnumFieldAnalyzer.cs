using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;
using Sudoku.CodeGen;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	/// <summary>
	/// Indicates an analyzer that analyzes the code for <see langword="enum"/> types
	/// marked <see cref="FlagsAttribute"/>.
	/// </summary>
	/// <seealso cref="FlagsAttribute"/>
	[CodeAnalyzer("SS0402", "SS0403")]
	public sealed partial class FlaggedEnumFieldAnalyzer : DiagnosticAnalyzer
	{
		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(CheckSS0402, new[] { SyntaxKind.EnumDeclaration });
		}


		private static void CheckSS0402(SyntaxNodeAnalysisContext context)
		{
			var (semanticModel, _, n) = context;

			/*length-pattern*/
			if (
				n is not EnumDeclarationSyntax
				{
					AttributeLists: { Count: not 0 } attributeLists
				} node
			)
			{
				return;
			}

			/*slice-pattern*/
			if (
				!attributeLists.Any(
					static attributeList =>
						attributeList is { Attributes: { Count: not 0 } attributes }
						&& attributes.Any(
							static attribute => attribute.Name is IdentifierNameSyntax
							{
								Identifier: { ValueText: "Flags" or nameof(FlagsAttribute) }
							}
						)
				)
			)
			{
				return;
			}

			foreach (var fieldDeclaration in node.DescendantNodes().OfType<EnumMemberDeclarationSyntax>())
			{
				switch (fieldDeclaration.EqualsValue)
				{
					case null when semanticModel.GetDeclaredSymbol(fieldDeclaration) is
					{
						ConstantValue: { } v
					}:
					{
						long value = Unsafe.As<object, long>(ref v);
						if (value != 0 && (value & value - 1) != 0)
						{
							context.ReportDiagnostic(
								Diagnostic.Create(
									descriptor: SS0403,
									location: fieldDeclaration.GetLocation(),
									messageArgs: null
								)
							);
						}

						break;
					}
					case { Value: var expression } when expression.DescendantNodes().Any(static expr =>
						expr.RawKind is not (
							(int)SyntaxKind.BitwiseAndExpression or (int)SyntaxKind.BitwiseNotExpression
							or (int)SyntaxKind.BitwiseOrExpression or (int)SyntaxKind.ExclusiveOrExpression
							or (int)SyntaxKind.IdentifierName or (int)SyntaxKind.NumericLiteralExpression
						)
					):
					{
						context.ReportDiagnostic(
							Diagnostic.Create(
								descriptor: SS0402,
								location: expression.GetLocation(),
								messageArgs: null
							)
						);

						break;
					}
				}
			}
		}
	}
}
