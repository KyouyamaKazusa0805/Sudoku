using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
#if DEBUG && SOURCE_GENERATOR_DEBUG
using System.Diagnostics;
#endif

namespace Sudoku.CodeAnalysis
{
	/// <summary>
	/// Indicates the analyzer that check the property named '<c>Properties</c>' in a step searcher.
	/// </summary>
	[Generator]
	public sealed partial class PropertiesInStepSearcherAnalyzer : ISourceGenerator
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

				// Create the semantic model and the property list.
				var semanticModel = compilation.GetSemanticModel(syntaxTree);
				var collector = new InnerWalker(compilation, semanticModel);
				collector.Visit(root);

				// If none, skip it.
				if (collector.TargetPropertyInfo is not { } targetPropertyInfos)
				{
					continue;
				}

				// Iterate on each information quadruple.
				foreach (var (hasTargetProperty, classNode, propertyNode, propertySymbol) in targetPropertyInfos)
				{
					if (hasTargetProperty)
					{
						if (propertySymbol is { DeclaredAccessibility: not Accessibility.Public })
						{
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

						if (propertySymbol is { IsStatic: false })
						{
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

						if (propertySymbol is { IsReadOnly: false })
						{
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

						if
						(
							!SymbolEqualityComparer.Default.Equals(
								propertySymbol!.Type, // Not null if 'hasTargetProperty' is true.
								compilation.GetTypeByMetadataName(TechniquePropertiesTypeFullName)
							)
						)
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

						if (propertySymbol is { NullableAnnotation: NullableAnnotation.Annotated })
						{
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

						if (propertyNode is { Initializer: null })
						{
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

						if
						(
							propertyNode is
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
					else
					{
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
					}
				}
			}
		}

		/// <inheritdoc/>
		public void Initialize(GeneratorInitializationContext context)
		{
#if DEBUG && SOURCE_GENERATOR_DEBUG
			if (!Debugger.IsAttached)
			{
				Debugger.Launch();
			}
#endif
		}
	}
}
