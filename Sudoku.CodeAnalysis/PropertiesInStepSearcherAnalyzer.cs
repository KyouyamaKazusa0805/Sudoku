using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Sudoku.CodeAnalysis
{
	/// <summary>
	/// Indicates the analyzer that check the property named '<c>Properties</c>' in a step searcher.
	/// </summary>
	[Generator]
	public sealed class PropertiesInStepSearcherAnalyzer : ISourceGenerator
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
		public void Execute(GeneratorExecutionContext context)
		{
			var compilation = context.Compilation;
			foreach (var syntaxTree in compilation.SyntaxTrees)
			{
				if (!syntaxTree.TryGetRoot(out var root))
				{
					continue;
				}

				var semanticModel = compilation.GetSemanticModel(syntaxTree);

				// The file contains syntax tree.
				var classDeclarations = root.ChildNodes().OfType<ClassDeclarationSyntax>();
				foreach (var classDeclaration in classDeclarations)
				{
					if (!(semanticModel.GetDeclaredSymbol(classDeclaration) is INamedTypeSymbol classInfo))
					{
						continue;
					}

					if (classInfo.IsAbstract || classInfo.IsAnonymousType || classInfo.IsStatic)
					{
						continue;
					}

					// Check whether the type is derived from StepSearcher.
					if (!classInfo.InheritsFrom(compilation.GetTypeByMetadataName(StepSearcherTypeFullName)))
					{
						continue;
					}

					// Instance class that isn't abstract.
					var propertySymbols = classInfo.GetMembers().OfType<IPropertySymbol>();
					if (!propertySymbols.Any())
					{
						// The class doesn't contain any properties.
						goto ReportSudoku004;
					}

					bool hasAPropertyNamedProperties = false;
					foreach (var propertySymbol in propertySymbols)
					{
						if (propertySymbol.IsIndexer // this[]
							|| !propertySymbol.IsStatic) // int Property { get; }
						{
							continue;
						}

						// Static properties.
						if (propertySymbol.Name != TargetPropertyName)
						{
							continue;
						}

						hasAPropertyNamedProperties = true;
						if (propertySymbol.DeclaredAccessibility != Accessibility.Public)
						{
							// The property doesn't expose outside.
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
									location: propertySymbol.Locations[0]));
						}

						if (propertySymbol.IsWriteOnly || !propertySymbol.IsReadOnly)
						{
							// The property shouldn't settable.
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
									location: (
										propertySymbol.SetMethod is var setMethodSymbol
										&& !(setMethodSymbol is null)
										? (ISymbol)setMethodSymbol
										: propertySymbol
									).Locations[0]));
						}

						if
						(
							!SymbolEqualityComparer.Default.Equals(
								propertySymbol.Type,
								compilation.GetTypeByMetadataName(TechniquePropertiesTypeFullName)
							)
						)
						{
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
						else if (propertySymbol.NullableAnnotation == NullableAnnotation.Annotated)
						{
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

						// Now check whether the property has a pre-defined value.

					}

					if (hasAPropertyNamedProperties)
					{
						continue;
					}

				ReportSudoku004:
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
							location: classInfo.Locations[0]));
				}
			}
		}

		/// <inheritdoc/>
		public void Initialize(GeneratorInitializationContext context)
		{
		}
	}
}
