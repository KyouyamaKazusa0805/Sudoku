using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

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
				var collector = new TargetPropertySearcher(compilation, semanticModel);
				collector.Visit(root);

				switch (collector.HasTargetProperty)
				{
					case true when collector.TargetPropertyInfo is { } targetPropertyInfos:
					{
						foreach (var targetPropertyInfo in targetPropertyInfos)
						{
							switch (targetPropertyInfo)
							{
								case (_, { DeclaredAccessibility: not Accessibility.Public } symbol):
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
											location: symbol.Locations[0]));

									break;
								}
								case (_, { IsReadOnly: false }):
								case (_, { IsWriteOnly: true }):
								{
									var symbol = targetPropertyInfo.Symbol;

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
												symbol.SetMethod is { } setMethodSymbol
												? (ISymbol)setMethodSymbol
												: symbol
											).Locations[0]));

									break;
								}
								case (_, { NullableAnnotation: NullableAnnotation.Annotated } symbol):
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
											location: symbol.Locations[0]));

									break;
								}
								case ({ Initializer: null } node, _):
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
											location: node.GetLocation()));

									break;
								}
								case (
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
								} node, _):
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
											location: node.GetLocation()));

									break;
								}
								case var (_, symbol)
								when !SymbolEqualityComparer.Default.Equals(
									symbol.Type,
									compilation.GetTypeByMetadataName(TechniquePropertiesTypeFullName)
								):
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
											location: symbol.Locations[0]));

									break;
								}
							}
						}

						break;
					}
					case false:
					{
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
								location: collector.ClassDeclaration!.GetLocation()));

						break;
					}
				}
			}
		}

		/// <inheritdoc/>
		public void Initialize(GeneratorInitializationContext context)
		{
		}
	}
}
