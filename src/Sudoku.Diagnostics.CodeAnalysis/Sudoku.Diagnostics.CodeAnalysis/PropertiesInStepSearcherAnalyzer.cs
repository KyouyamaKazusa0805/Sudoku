#pragma warning disable RS1030

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Sudoku.Diagnostics.CodeAnalysis.Extensions;

namespace Sudoku.Diagnostics.CodeAnalysis
{
	/// <summary>
	/// Indicates the analyzer that analyzes the code for the <see langword="static"/> property
	/// named <c>Properties</c>.
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public sealed partial class PropertiesInStepSearcherAnalyzer : DiagnosticAnalyzer
	{
		/// <summary>
		/// Indicates the target property name to check (i.e. <c>Properties</c>).
		/// </summary>
		private const string TargetPropertyName = "Properties";

		/// <summary>
		/// Indicates the full name of the type.
		/// </summary>
		private const string StepSearcherTypeFullName = "Sudoku.Solving.Manual.StepSearcher";

		/// <summary>
		/// Indicates the full name of the type of the property technique properties.
		/// </summary>
		private const string TechniquePropertiesTypeFullName = "Sudoku.Solving.Manual.TechniqueProperties";


		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
		}

		/// <summary>
		/// To analyze this symbol.
		/// </summary>
		/// <param name="context">The context.</param>
		private static void AnalyzeSymbol(SymbolAnalysisContext context)
		{
			var namedTypeSymbol = (INamedTypeSymbol)context.Symbol;
			if (namedTypeSymbol is not { TypeKind: TypeKind.Class })
			{
				return;
			}

			if (namedTypeSymbol is not { IsAbstract: false, IsAnonymousType: false, IsStatic: false })
			{
				return;
			}

			if (!namedTypeSymbol.DerivedFrom(context.Compilation, StepSearcherTypeFullName))
			{
				return;
			}

			var propertySymbols = namedTypeSymbol.GetMembers().OfType<IPropertySymbol>();
			if (propertySymbols.Any())
			{
				foreach (var propertySymbol in propertySymbols)
				{
					if (propertySymbol is not { IsIndexer: false, Name: TargetPropertyName })
					{
						continue;
					}

					CheckSudoku002(context, propertySymbol);
					CheckSudoku003(context, propertySymbol);
					CheckSudoku004(context, propertySymbol);
					CheckSudoku005(context, propertySymbol);
					CheckSudoku006(context, propertySymbol);

					var matchingNode = propertySymbol.FindMatchingNode(
						propertySymbol.ContainingType.DeclaringSyntaxReferences[0].GetSyntax(),
						context.Compilation.GetSemanticModel(
							propertySymbol.DeclaringSyntaxReferences[0].SyntaxTree
						)
					);
					CheckSudoku007(context, matchingNode);
					CheckSudoku008(context, matchingNode);
				}
			}
			else
			{
				CheckSudoku001(context, namedTypeSymbol);
			}
		}

		private static void CheckSudoku001(SymbolAnalysisContext context, INamedTypeSymbol namedTypeSymbol) =>
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
					location: namedTypeSymbol.Locations[0]
				)
			);

		private static void CheckSudoku002(SymbolAnalysisContext context, IPropertySymbol? propertySymbol)
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
					location: propertySymbol.Locations[0]
				)
			);
		}

		private static void CheckSudoku003(SymbolAnalysisContext context, IPropertySymbol? propertySymbol)
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
					location: propertySymbol.Locations[0]
				)
			);
		}

		private static void CheckSudoku004(SymbolAnalysisContext context, IPropertySymbol? propertySymbol)
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
						propertySymbol.SetMethod is { } setMethodSymbol ? (ISymbol)setMethodSymbol : propertySymbol
					).Locations[0]
				)
			);
		}

		private static void CheckSudoku005(
			SymbolAnalysisContext context, IPropertySymbol propertySymbol)
		{
			if (
				SymbolEqualityComparer.Default.Equals(
					propertySymbol.Type,
					context.Compilation.GetTypeByMetadataName(TechniquePropertiesTypeFullName)
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
					location: propertySymbol.Locations[0]
				)
			);
		}

		private static void CheckSudoku006(SymbolAnalysisContext context, IPropertySymbol? propertySymbol)
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
					location: propertySymbol.Locations[0]
				)
			);
		}

		private static void CheckSudoku007(
			SymbolAnalysisContext context, PropertyDeclarationSyntax? propertyNode)
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
					location: propertyNode.GetLocation()
				)
			);
		}

		private static void CheckSudoku008(
			SymbolAnalysisContext context, PropertyDeclarationSyntax? propertyNode)
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
					location: propertyNode.GetLocation()
				)
			);
		}
	}
}
