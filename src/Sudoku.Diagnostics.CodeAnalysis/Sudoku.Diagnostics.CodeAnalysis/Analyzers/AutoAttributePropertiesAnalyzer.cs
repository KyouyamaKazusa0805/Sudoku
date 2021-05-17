using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	/// <summary>
	/// Indicates an analyzer that analyzes the code for closed <see langword="enum"/> types.
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public sealed partial class AutoAttributePropertiesAnalyzer : DiagnosticAnalyzer
	{
		/// <summary>
		/// Indicates the full type names.
		/// </summary>
		private const string
			AutoDeconstructAttributeFullTypeName = "AutoDeconstructAttribute",
			AutoDeconstructExtensionAttributeFullTypeName = "AutoDeconstructExtensionAttribute",
			AutoEqualityAttributeFullTypeName = "AutoEqualityAttribute",
			AutoGetEnumeratorAttributeFullTypeName = "AutoGetEnumeratorAttribute",
			AutoHashCodeAttributeFullTypeName = "AutoHashCodeAttribute";

		/// <summary>
		/// Indicates the type format.
		/// </summary>
		public static readonly SymbolDisplayFormat TypeFormat = new(
			SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining,
			SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
			SymbolDisplayGenericsOptions.IncludeTypeParameters
			| SymbolDisplayGenericsOptions.IncludeTypeConstraints,
			miscellaneousOptions:
				SymbolDisplayMiscellaneousOptions.UseSpecialTypes
				| SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers
				| SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier
		);


		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(
				static context => CheckSudoku023(context),
				new[] { SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration, SyntaxKind.RecordDeclaration }
			);
		}


		private static void CheckSudoku023(SyntaxNodeAnalysisContext context)
		{
			var (semanticModel, _, node) = context;
			if (node is not MemberDeclarationSyntax { AttributeLists: { Count: not 0 } attributeLists })
			{
				return;
			}

			foreach (var attributeList in attributeLists)
			{
				if (attributeList.Attributes is not { Count: not 0 } attributes)
				{
					continue;
				}

				foreach (var attribute in attributes)
				{
					if (
						attribute is not
						{
							Name: IdentifierNameSyntax { Identifier: { ValueText: var text } } identifierName,
							ArgumentList: { Arguments: var arguments }
						}
					)
					{
						continue;
					}

					switch (text)
					{
						case AutoDeconstructAttributeFullTypeName:
						{
							if (arguments.Count < 2)
							{
								context.ReportDiagnostic(
									Diagnostic.Create(
										descriptor: new(
											id: DiagnosticIds.Sudoku024,
											title: Titles.Sudoku024,
											messageFormat: Messages.Sudoku024,
											category: Categories.Usage,
											defaultSeverity: DiagnosticSeverity.Warning,
											isEnabledByDefault: true,
											helpLinkUri: HelpLinks.Sudoku024
										),
										location: node.GetLocation(),
										messageArgs: new object[]
										{
											"Sudoku.CodeGen.AutoDeconstructAttributeFullTypeName",
											2,
											" at least"
										}
									)
								);

								continue;
							}

							goto ArgumentsChecking_Case1;
						}
						case var name and (
							AutoEqualityAttributeFullTypeName or AutoHashCodeAttributeFullTypeName
						):
						{
							if (arguments.Count < 1)
							{
								context.ReportDiagnostic(
									Diagnostic.Create(
										descriptor: new(
											id: DiagnosticIds.Sudoku024,
											title: Titles.Sudoku024,
											messageFormat: Messages.Sudoku024,
											category: Categories.Usage,
											defaultSeverity: DiagnosticSeverity.Warning,
											isEnabledByDefault: true,
											helpLinkUri: HelpLinks.Sudoku024
										),
										location: node.GetLocation(),
										messageArgs: new object[]
										{
											$"Sudoku.CodeGen.{name}",
											1,
											" at least"
										}
									)
								);

								continue;
							}

							goto ArgumentsChecking_Case1;
						}
						case AutoDeconstructExtensionAttributeFullTypeName:
						{
							if (arguments.Count < 3)
							{
								context.ReportDiagnostic(
									Diagnostic.Create(
										descriptor: new(
											id: DiagnosticIds.Sudoku024,
											title: Titles.Sudoku024,
											messageFormat: Messages.Sudoku024,
											category: Categories.Usage,
											defaultSeverity: DiagnosticSeverity.Warning,
											isEnabledByDefault: true,
											helpLinkUri: HelpLinks.Sudoku024
										),
										location: node.GetLocation(),
										messageArgs: new object[]
										{
											"Sudoku.CodeGen.AutoDeconstructExtensionAttributeFullTypeName",
											3,
											" at least"
										}
									)
								);

								continue;
							}

							goto ArgumentsChecking_Case2;
						}
						case AutoGetEnumeratorAttributeFullTypeName:
						{
							/*slice-pattern*/
							if (
								arguments[0] is { Expression: var expr } argument
								&& semanticModel.GetOperation(expr) is not { Kind: OperationKind.NameOf })
							{
								context.ReportDiagnostic(
									Diagnostic.Create(
										descriptor: new(
											id: DiagnosticIds.Sudoku023,
											title: Titles.Sudoku023,
											messageFormat: Messages.Sudoku023,
											category: Categories.Usage,
											defaultSeverity: DiagnosticSeverity.Warning,
											isEnabledByDefault: true,
											helpLinkUri: HelpLinks.Sudoku023
										),
										location: argument.GetLocation(),
										messageArgs: null
									)
								);
							}

							break;
						}

					ArgumentsChecking_Case1:
						{
							foreach (var argument in arguments)
							{
								if (
									semanticModel.GetOperation(argument.Expression) is not
									{
										Kind: OperationKind.NameOf
									}
								)
								{
									context.ReportDiagnostic(
										Diagnostic.Create(
											descriptor: new(
												id: DiagnosticIds.Sudoku023,
												title: Titles.Sudoku023,
												messageFormat: Messages.Sudoku023,
												category: Categories.Usage,
												defaultSeverity: DiagnosticSeverity.Warning,
												isEnabledByDefault: true,
												helpLinkUri: HelpLinks.Sudoku023
											),
											location: argument.GetLocation(),
											messageArgs: null
										)
									);
								}
							}

							break;
						}

					ArgumentsChecking_Case2:
						{
							for (int i = 1; i < arguments.Count; i++)
							{
								var argument = arguments[i];
								if (
									semanticModel.GetOperation(argument.Expression) is not
									{
										Kind: OperationKind.NameOf
									}
								)
								{
									context.ReportDiagnostic(
										Diagnostic.Create(
											descriptor: new(
												id: DiagnosticIds.Sudoku023,
												title: Titles.Sudoku023,
												messageFormat: Messages.Sudoku023,
												category: Categories.Usage,
												defaultSeverity: DiagnosticSeverity.Warning,
												isEnabledByDefault: true,
												helpLinkUri: HelpLinks.Sudoku023
											),
											location: argument.GetLocation(),
											messageArgs: null
										)
									);
								}
							}

							break;
						}
					}
				}
			}
		}
	}
}
