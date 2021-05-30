using System.Collections.Generic;
using System.Linq;
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
	/// Indicates an analyzer that analyzes the code for unncessary discard pattern.
	/// </summary>
	/// <remarks>
	/// All possible patterns will be analyzed:
	/// <list type="bullet">
	/// <item>Discard in positional pattern: <c>(DeconstructionMember: <see langword="_"/>)</c>.</item>
	/// <item>Discard in property pattern: <c>{ Property: <see langword="_"/> }</c>.</item>
	/// </list>
	/// </remarks>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public sealed partial class UnncessaryDiscardPatternAnalyzer : DiagnosticAnalyzer
	{
		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(
				AnalyzeSyntaxNode,
				new[] { SyntaxKind.PropertyPatternClause, SyntaxKind.PositionalPatternClause }
			);
		}


		private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
		{
			CheckSS0607(context);
			CheckSS0613(context);
		}

		private static void CheckSS0607(SyntaxNodeAnalysisContext context)
		{
			var (semanticModel, _, originalNode) = context;

			/*length-pattern*/
			if (
				originalNode is not PositionalPatternClauseSyntax
				{
					Parent: RecursivePatternSyntax parentNode,
					Subpatterns: { Count: >= 2 } subpatterns
				}
			)
			{
				return;
			}

			if (
				semanticModel.GetOperation(parentNode) is not IRecursivePatternOperation
				{
					DeconstructSymbol: IMethodSymbol { Parameters: var boundMethodParameters },
					MatchedType: { } type
				} operation
			)
			{
				return;
			}

			var nameLookup = new List<string>();
			var discards = new List<(SubpatternSyntax Pattern, string BoundParameterName)>();
			for (int i = 0; i < subpatterns.Count; i++)
			{
				switch (subpatterns[i])
				{
					case { NameColon: null, Pattern: var valuePattern } subpattern:
					{
						string boundParameterName = boundMethodParameters[i].Name;
						switch (valuePattern)
						{
							case DiscardPatternSyntax:
							//case DeclarationPatternSyntax { Designation: DiscardDesignationSyntax }:
							{
								discards.Add((subpattern, boundParameterName));
								break;
							}
							default:
							{
								// If the parameter is unnamed, we should check the operation,
								// and get the bound deconstruction method, and get the name.
								nameLookup.Add(boundParameterName);
								break;
							}
						}

						break;
					}
					case
					{
						NameColon: { Name: { Identifier: { ValueText: var parameterName } } } nameColonNode,
						Pattern: var valuePattern
					} subpattern:
					{
						switch (valuePattern)
						{
							case DiscardPatternSyntax:
							//case DeclarationPatternSyntax { Designation: DiscardDesignationSyntax }:
							{
								discards.Add((subpattern, parameterName));
								break;
							}
							default:
							{
								nameLookup.Add(parameterName);
								break;
							}
						}

						break;
					}
				}
			}

			foreach (var deconstructionMethod in
				from deconstructionMethod in type.GetAllDeconstructionMethods()
				where deconstructionMethod.Parameters.Length < subpatterns.Count
				select deconstructionMethod)
			{
				var parameters = deconstructionMethod.Parameters;
				if (nameLookup.Any(name => parameters.All(parameter => parameter.Name != name)))
				{
					continue;
				}

				foreach (var (discardNodeToReport, boundParameterName) in discards)
				{
					context.ReportDiagnostic(
						Diagnostic.Create(
							descriptor: SS0607,
							location: discardNodeToReport.GetLocation(),
							messageArgs: new[] { boundParameterName }
						)
					);
				}

				// Only reports once is okay.
				return;
			}
		}

		private static void CheckSS0613(SyntaxNodeAnalysisContext context)
		{
			/*length-pattern*/
			if (context.Node is not PropertyPatternClauseSyntax { Subpatterns: { Count: >= 1 } subpatterns })
			{
				return;
			}

			checkSS0613Recursively(context, subpatterns);


			static void checkSS0613Recursively(
				in SyntaxNodeAnalysisContext context, in SeparatedSyntaxList<SubpatternSyntax> subpatterns)
			{
				foreach (var subpattern in subpatterns)
				{
					if (subpattern is not { NameColon: { }, Pattern: var pattern })
					{
						continue;
					}

					switch (pattern)
					{
						case DiscardPatternSyntax:
						{
							context.ReportDiagnostic(
								Diagnostic.Create(
									descriptor: SS0613,
									location: subpattern.GetLocation(),
									messageArgs: new[] { subpattern.ToString() }
								)
							);

							break;
						}
						case RecursivePatternSyntax
						{
							PropertyPatternClause: { Subpatterns: { Count: >= 1 } nestedSubpatterns }
						}:
						{
							checkSS0613Recursively(context, nestedSubpatterns);

							break;
						}
					}
				}
			}
		}
	}
}
