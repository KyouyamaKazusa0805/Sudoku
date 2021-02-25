using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Sudoku.CodeAnalysis.Analyzers
{
	partial class PropertiesInStepSearcherAnalyzer
	{
		partial void CheckSudoku001(GeneratorExecutionContext context, ClassDeclarationSyntax classNode) =>
			context.ReportDiagnostic(
				Diagnostic.Create(
					id: DiagnosticIds.Sudoku001,
					category: Categories.Usage,
					message: Messages.Sudoku001,
					severity: DiagnosticSeverity.Error,
					defaultSeverity: DiagnosticSeverity.Error,
					isEnabledByDefault: true,
					warningLevel: 0,
					title: Titles.Sudoku001,
					helpLink: HelpLinks.Sudoku001,
					location: classNode.GetLocation()));

		partial void CheckSudoku002(GeneratorExecutionContext context, IPropertySymbol? propertySymbol)
		{
			if (propertySymbol is not { DeclaredAccessibility: not Accessibility.Public })
			{
				return;
			}

			// The property must be public (expose to outside).
			context.ReportDiagnostic(
				Diagnostic.Create(
					id: DiagnosticIds.Sudoku002,
					category: Categories.Usage,
					message: Messages.Sudoku002,
					severity: DiagnosticSeverity.Error,
					defaultSeverity: DiagnosticSeverity.Error,
					isEnabledByDefault: true,
					warningLevel: 0,
					title: Titles.Sudoku002,
					helpLink: HelpLinks.Sudoku002,
					location: propertySymbol.Locations[0]));
		}

		partial void CheckSudoku003(GeneratorExecutionContext context, IPropertySymbol? propertySymbol)
		{
			if (propertySymbol is not { IsStatic: false })
			{
				return;
			}

			// The property must be static.
			context.ReportDiagnostic(
				Diagnostic.Create(
					id: DiagnosticIds.Sudoku003,
					category: Categories.Usage,
					message: Messages.Sudoku003,
					severity: DiagnosticSeverity.Error,
					defaultSeverity: DiagnosticSeverity.Error,
					isEnabledByDefault: true,
					warningLevel: 0,
					title: Titles.Sudoku003,
					helpLink: HelpLinks.Sudoku003,
					location: propertySymbol.Locations[0]));
		}

		partial void CheckSudoku004(GeneratorExecutionContext context, IPropertySymbol? propertySymbol)
		{
			if (propertySymbol is not { IsReadOnly: false })
			{
				return;
			}

			// The property shouldn't settable.
			context.ReportDiagnostic(
				Diagnostic.Create(
					id: DiagnosticIds.Sudoku004,
					category: Categories.Usage,
					message: Messages.Sudoku004,
					severity: DiagnosticSeverity.Error,
					defaultSeverity: DiagnosticSeverity.Error,
					isEnabledByDefault: true,
					warningLevel: 0,
					title: Titles.Sudoku004,
					helpLink: HelpLinks.Sudoku004,
					location: (
						propertySymbol.SetMethod is { } setMethodSymbol
						? (ISymbol)setMethodSymbol
						: propertySymbol
					).Locations[0]));
		}

		partial void CheckSudoku005(GeneratorExecutionContext context, Compilation compilation, IPropertySymbol propertySymbol)
		{
			if (
				SymbolEqualityComparer.Default.Equals(
					propertySymbol.Type,
					compilation.GetTypeByMetadataName(TechniquePropertiesTypeFullName)
				)
			)
			{
				return;
			}

			context.ReportDiagnostic(
				Diagnostic.Create(
					id: DiagnosticIds.Sudoku005,
					category: Categories.Usage,
					message: Messages.Sudoku005,
					severity: DiagnosticSeverity.Error,
					defaultSeverity: DiagnosticSeverity.Error,
					isEnabledByDefault: true,
					warningLevel: 0,
					title: Titles.Sudoku005,
					helpLink: HelpLinks.Sudoku005,
					location: propertySymbol.Locations[0]));
		}

		partial void CheckSudoku006(GeneratorExecutionContext context, IPropertySymbol? propertySymbol)
		{
			if (propertySymbol is not { NullableAnnotation: NullableAnnotation.Annotated })
			{
				return;
			}

			context.ReportDiagnostic(
				Diagnostic.Create(
					id: DiagnosticIds.Sudoku006,
					category: Categories.Usage,
					message: Messages.Sudoku006,
					severity: DiagnosticSeverity.Error,
					defaultSeverity: DiagnosticSeverity.Error,
					isEnabledByDefault: true,
					warningLevel: 0,
					title: Titles.Sudoku006,
					helpLink: HelpLinks.Sudoku006,
					location: propertySymbol.Locations[0]));
		}

		partial void CheckSudoku007(GeneratorExecutionContext context, PropertyDeclarationSyntax? propertyNode)
		{
			if (propertyNode is not { Initializer: null })
			{
				return;
			}

			context.ReportDiagnostic(
				Diagnostic.Create(
					id: DiagnosticIds.Sudoku007,
					category: Categories.Usage,
					message: Messages.Sudoku007,
					severity: DiagnosticSeverity.Error,
					defaultSeverity: DiagnosticSeverity.Error,
					isEnabledByDefault: true,
					warningLevel: 0,
					title: Titles.Sudoku007,
					helpLink: HelpLinks.Sudoku007,
					location: propertyNode.GetLocation()));
		}

		partial void CheckSudoku008(GeneratorExecutionContext context, PropertyDeclarationSyntax? propertyNode)
		{
			if (
				propertyNode is not
				{
					Initializer:
					{
						Value:
						{
							RawKind: not (
								(int)SyntaxKind.ObjectCreationExpression
								or (int)SyntaxKind.ImplicitObjectCreationExpression
							)
						}
					}
				}
			)
			{
				return;
			}

			context.ReportDiagnostic(
				Diagnostic.Create(
					id: DiagnosticIds.Sudoku008,
					category: Categories.Usage,
					message: Messages.Sudoku008,
					severity: DiagnosticSeverity.Error,
					defaultSeverity: DiagnosticSeverity.Error,
					isEnabledByDefault: true,
					warningLevel: 0,
					title: Titles.Sudoku008,
					helpLink: HelpLinks.Sudoku008,
					location: propertyNode.GetLocation()));
		}
	}
}
