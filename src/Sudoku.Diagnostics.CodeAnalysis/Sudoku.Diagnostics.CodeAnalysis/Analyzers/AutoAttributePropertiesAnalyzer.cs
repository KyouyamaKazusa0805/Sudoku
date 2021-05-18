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


		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(
				CheckSD0402,
				new[]
				{
					SyntaxKind.ClassDeclaration,
					SyntaxKind.StructDeclaration,
					SyntaxKind.RecordDeclaration
				}
			);
		}


		private static void CheckSD0402(SyntaxNodeAnalysisContext context)
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
											id: DiagnosticIds.SD0402,
											title: Titles.SD0402,
											messageFormat: Messages.SD0402,
											category: Categories.Usage,
											defaultSeverity: DiagnosticSeverity.Warning,
											isEnabledByDefault: true,
											helpLinkUri: HelpLinks.SD0402
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
											id: DiagnosticIds.SD0402,
											title: Titles.SD0402,
											messageFormat: Messages.SD0402,
											category: Categories.Usage,
											defaultSeverity: DiagnosticSeverity.Warning,
											isEnabledByDefault: true,
											helpLinkUri: HelpLinks.SD0402
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
											id: DiagnosticIds.SD0402,
											title: Titles.SD0402,
											messageFormat: Messages.SD0402,
											category: Categories.Usage,
											defaultSeverity: DiagnosticSeverity.Warning,
											isEnabledByDefault: true,
											helpLinkUri: HelpLinks.SD0402
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
											id: DiagnosticIds.SD0401,
											title: Titles.SD0401,
											messageFormat: Messages.SD0401,
											category: Categories.Usage,
											defaultSeverity: DiagnosticSeverity.Warning,
											isEnabledByDefault: true,
											helpLinkUri: HelpLinks.SD0401
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
												id: DiagnosticIds.SD0401,
												title: Titles.SD0401,
												messageFormat: Messages.SD0401,
												category: Categories.Usage,
												defaultSeverity: DiagnosticSeverity.Warning,
												isEnabledByDefault: true,
												helpLinkUri: HelpLinks.SD0401
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
												id: DiagnosticIds.SD0401,
												title: Titles.SD0401,
												messageFormat: Messages.SD0401,
												category: Categories.Usage,
												defaultSeverity: DiagnosticSeverity.Warning,
												isEnabledByDefault: true,
												helpLinkUri: HelpLinks.SD0401
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
