using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;
using Microsoft.CodeAnalysis.Operations;
using Sudoku.CodeGenerating;
using Sudoku.Diagnostics.CodeAnalysis.Extensions;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	[CodeAnalyzer("SS0607F", "SS0613F")]
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

			// Get basic information.
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

			// Get the type information, to get all possible deconstruction methods.
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

			// Try to record the information about the sub-patterns.
			var nameLookup = new List<string>();
			var discards = new HashSet<(SubpatternSyntax Pattern, string BoundParameterName, int Index)>(
				new TripletComparer()
			);

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
								discards.Add((subpattern, boundParameterName, i));
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
								discards.Add((subpattern, parameterName, i));
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

			// Iterate on each deconstruction method, and get the minimum method that holds
			// the least number of parameters that can pass the values without the discard
			// in the current pattern.
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

				foreach (var (discardNodeToReport, boundParameterName, index) in discards)
				{
					context.ReportDiagnostic(
						Diagnostic.Create(
							descriptor: SS0607,
							location: discardNodeToReport.GetLocation(),
							messageArgs: new[] { boundParameterName },
							properties: ImmutableDictionary.CreateRange(
								new KeyValuePair<string, string?>[]
								{
									new("BoundParameterIndex", index.ToString())
								}
							)
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

			var discardPatterns = new List<SubpatternSyntax>();
			checkSS0613Recursively(context, subpatterns, discardPatterns);

			var additionalLocations = (from p in discardPatterns select p.GetLocation()).ToArray();
			foreach (var discardPattern in discardPatterns)
			{
				context.ReportDiagnostic(
					Diagnostic.Create(
						descriptor: SS0613,
						location: discardPattern.GetLocation(),
						messageArgs: new[] { discardPattern.ToString() },
						additionalLocations: additionalLocations
					)
				);
			}


			static void checkSS0613Recursively(
				in SyntaxNodeAnalysisContext context, in SeparatedSyntaxList<SubpatternSyntax> subpatterns,
				IList<SubpatternSyntax> discardPatterns)
			{
				foreach (var subpattern in subpatterns)
				{
					if (subpattern is not { NameColon: not null, Pattern: var pattern })
					{
						continue;
					}

					switch (pattern)
					{
						case DiscardPatternSyntax { Parent: SubpatternSyntax subpatternPart }:
						{
							discardPatterns.Add(subpatternPart);

							break;
						}
						case RecursivePatternSyntax
						{
							PropertyPatternClause: { Subpatterns: { Count: >= 1 } nestedSubpatterns }
						}:
						{
							checkSS0613Recursively(context, nestedSubpatterns, discardPatterns);

							break;
						}
					}
				}
			}
		}
	}
}
