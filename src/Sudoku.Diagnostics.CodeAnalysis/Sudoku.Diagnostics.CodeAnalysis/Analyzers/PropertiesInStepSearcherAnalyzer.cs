using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Sudoku.Diagnostics.CodeAnalysis.Extensions;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	/// <summary>
	/// Indicates the analyzer that check the property named '<c>Properties</c>' in a step searcher.
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

					CheckSD0102(context, propertySymbol);
					CheckSD0103(context, propertySymbol);
					CheckSD0104(context, propertySymbol);
					CheckSD0105(context, propertySymbol);
					CheckSD0106(context, propertySymbol);

					var node = (PropertyDeclarationSyntax)propertySymbol.DeclaringSyntaxReferences[0].GetSyntax();
					CheckSD0107(context, node);
					CheckSD0108(context, node);
				}
			}
			else
			{
				CheckSD0101(context, namedTypeSymbol);
			}
		}

		private static void CheckSD0101(SymbolAnalysisContext context, INamedTypeSymbol namedTypeSymbol) =>
			context.ReportDiagnostic(
				Diagnostic.Create(
					id: DiagnosticIds.SD0101,
					category: Categories.StaticTechniqueProperties,
					message: Messages.SD0101,
					severity: DiagnosticSeverity.Error,
					defaultSeverity: DiagnosticSeverity.Error,
					isEnabledByDefault: true,
					warningLevel: 0,
					title: Titles.SD0101,
					helpLink: HelpLinks.SD0101,
					location: namedTypeSymbol.Locations[0]
				)
			);

		private static void CheckSD0102(SymbolAnalysisContext context, IPropertySymbol? propertySymbol)
		{
			if (propertySymbol is not { DeclaredAccessibility: not Accessibility.Public })
			{
				return;
			}

			// The property must be public (expose to outside).
			context.ReportDiagnostic(
				Diagnostic.Create(
					id: DiagnosticIds.SD0102,
					category: Categories.StaticTechniqueProperties,
					message: Messages.SD0102,
					severity: DiagnosticSeverity.Error,
					defaultSeverity: DiagnosticSeverity.Error,
					isEnabledByDefault: true,
					warningLevel: 0,
					title: Titles.SD0102,
					helpLink: HelpLinks.SD0102,
					location: propertySymbol.Locations[0]
				)
			);
		}

		private static void CheckSD0103(SymbolAnalysisContext context, IPropertySymbol? propertySymbol)
		{
			if (propertySymbol is not { IsStatic: false })
			{
				return;
			}

			// The property must be static.
			context.ReportDiagnostic(
				Diagnostic.Create(
					id: DiagnosticIds.SD0103,
					category: Categories.StaticTechniqueProperties,
					message: Messages.SD0103,
					severity: DiagnosticSeverity.Error,
					defaultSeverity: DiagnosticSeverity.Error,
					isEnabledByDefault: true,
					warningLevel: 0,
					title: Titles.SD0103,
					helpLink: HelpLinks.SD0103,
					location: propertySymbol.Locations[0]
				)
			);
		}

		private static void CheckSD0104(SymbolAnalysisContext context, IPropertySymbol? propertySymbol)
		{
			if (propertySymbol is not { IsReadOnly: false })
			{
				return;
			}

			// The property shouldn't settable.
			context.ReportDiagnostic(
				Diagnostic.Create(
					id: DiagnosticIds.SD0104,
					category: Categories.StaticTechniqueProperties,
					message: Messages.SD0104,
					severity: DiagnosticSeverity.Error,
					defaultSeverity: DiagnosticSeverity.Error,
					isEnabledByDefault: true,
					warningLevel: 0,
					title: Titles.SD0104,
					helpLink: HelpLinks.SD0104,
					location: (
						propertySymbol.SetMethod is { } setMethodSymbol ? (ISymbol)setMethodSymbol : propertySymbol
					).Locations[0]
				)
			);
		}

		private static void CheckSD0105(SymbolAnalysisContext context, IPropertySymbol propertySymbol)
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
					id: DiagnosticIds.SD0105,
					category: Categories.StaticTechniqueProperties,
					message: Messages.SD0105,
					severity: DiagnosticSeverity.Error,
					defaultSeverity: DiagnosticSeverity.Error,
					isEnabledByDefault: true,
					warningLevel: 0,
					title: Titles.SD0105,
					helpLink: HelpLinks.SD0105,
					location: propertySymbol.Locations[0]
				)
			);
		}

		private static void CheckSD0106(SymbolAnalysisContext context, IPropertySymbol? propertySymbol)
		{
			if (propertySymbol is not { NullableAnnotation: NullableAnnotation.Annotated })
			{
				return;
			}

			context.ReportDiagnostic(
				Diagnostic.Create(
					id: DiagnosticIds.SD0106,
					category: Categories.StaticTechniqueProperties,
					message: Messages.SD0106,
					severity: DiagnosticSeverity.Error,
					defaultSeverity: DiagnosticSeverity.Error,
					isEnabledByDefault: true,
					warningLevel: 0,
					title: Titles.SD0106,
					helpLink: HelpLinks.SD0106,
					location: propertySymbol.Locations[0]
				)
			);
		}

		private static void CheckSD0107(SymbolAnalysisContext context, PropertyDeclarationSyntax? propertyNode)
		{
			if (propertyNode is not { Initializer: null })
			{
				return;
			}

			context.ReportDiagnostic(
				Diagnostic.Create(
					id: DiagnosticIds.SD0107,
					category: Categories.StaticTechniqueProperties,
					message: Messages.SD0107,
					severity: DiagnosticSeverity.Error,
					defaultSeverity: DiagnosticSeverity.Error,
					isEnabledByDefault: true,
					warningLevel: 0,
					title: Titles.SD0107,
					helpLink: HelpLinks.SD0107,
					location: propertyNode.GetLocation()
				)
			);
		}

		private static void CheckSD0108(SymbolAnalysisContext context, PropertyDeclarationSyntax? propertyNode)
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
					id: DiagnosticIds.SD0108,
					category: Categories.StaticTechniqueProperties,
					message: Messages.SD0108,
					severity: DiagnosticSeverity.Error,
					defaultSeverity: DiagnosticSeverity.Error,
					isEnabledByDefault: true,
					warningLevel: 0,
					title: Titles.SD0108,
					helpLink: HelpLinks.SD0108,
					location: propertyNode.GetLocation()
				)
			);
		}
	}
}
