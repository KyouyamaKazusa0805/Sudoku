using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;
using Microsoft.CodeAnalysis.Operations;
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

			context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, new[] { SyntaxKind.EnumDeclaration });
		}


		private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
		{
			var (semanticModel, _, n) = context;

			/*length-pattern*/
			if (n is not EnumDeclarationSyntax { AttributeLists: { Count: not 0 } attributeLists } node)
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
					case null:
					{
						context.ReportDiagnostic(
							Diagnostic.Create(
								descriptor: SS0403,
								location: fieldDeclaration.GetLocation(),
								messageArgs: null
							)
						);

						break;
					}
					case { Value: var expression }
					when semanticModel.GetOperation(expression) is ILiteralOperation
					{
						ConstantValue: { HasValue: true, Value: var value and (int or long) }
					}:
					{
						switch (value)
						{
							case int i when (i & i - 1) == 0:
							case long l when (l & l - 1) == 0:
							{
								continue;
							}
						}

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
