using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sudoku.CodeAnalysis.Extensions;

namespace Sudoku.CodeAnalysis
{
	/// <summary>
	/// Indicates the analyzer that checks the dynamically invocation of the <see langword="dynamic"/>
	/// field <c>TextResources.Current</c>.
	/// </summary>
	[Generator]
	public sealed partial class DynamicallyInvocationOfCurrentAnalyzer : ISourceGenerator
	{
		/// <summary>
		/// Indicates the text resources class name.
		/// </summary>
		private const string TextResourcesClassName = "TextResources";

		/// <summary>
		/// Indicates that field dynamically bound.
		/// </summary>
		private const string TextResourcesStaticReadOnlyFieldName = "Current";

		/// <summary>
		/// Indicates the method name that is called in order to change the language of that resource dictionary.
		/// </summary>
		private const string ChangeLanguageMethodName = "ChangeLanguage";

		/// <summary>
		/// Indicates the method name that is called in order to serialize the object.
		/// </summary>
		private const string SerializeMethodName = "Serialize";

		/// <summary>
		/// Indicates the method name that is called in order to deserialize the object.
		/// </summary>
		private const string DeserializeMethodName = "Deserialize";

		/// <summary>
		/// Indicates the country code type name.
		/// </summary>
		private const string CountryCodeTypeName = "Sudoku.Globalization.CountryCode";


		/// <inheritdoc/>
		public void Execute(GeneratorExecutionContext context)
		{
			var compilation = context.Compilation;
			if (compilation.AssemblyName is "Sudoku.UI" or "Sudoku.Windows")
			{
				// We don't check on those two WPF projects, because those two projects has already used
				// their own resource dictionary (MergedDictionary).
				return;
			}

			foreach (var syntaxTree in compilation.SyntaxTrees)
			{
				// Check whether the syntax contains the root node.
				if (!syntaxTree.TryGetRoot(out var root))
				{
					continue;
				}

				// Create the semantic model and the property list.
				var semanticModel = compilation.GetSemanticModel(syntaxTree);
				var collector = new InnerWalker(semanticModel);
				collector.Visit(root);

				// If the syntax tree doesn't contain any dynamically called clause,
				// just skip it.
				if (collector.Collection is null)
				{
					continue;
				}

				// Iterate on each dynamically called location.
				foreach (var (node, nameNode, methodName, argNodes) in collector.Collection)
				{
					if
					(
						methodName is not (
							ChangeLanguageMethodName or SerializeMethodName or DeserializeMethodName
						)
					)
					{
						context.ReportDiagnostic(
							Diagnostic.Create(
								descriptor: new(
									id: DiagnosticIds.Sudoku009,
									title: Titles.Sudoku009,
									messageFormat: Messages.Sudoku009,
									category: Categories.Usage,
									defaultSeverity: DiagnosticSeverity.Error,
									isEnabledByDefault: true,
									helpLinkUri: HelpLinks.Sudoku009
								),
								location: nameNode.GetLocation(),
								messageArgs: new[] { methodName }
							)
						);

						goto CheckSudoku012;
					}

					int actualParamsCount = argNodes.Arguments.Count;
					int requiredParamsCount = methodName switch
					{
						ChangeLanguageMethodName => 1,
						SerializeMethodName => 2,
						DeserializeMethodName => 2
					};
					if (actualParamsCount != requiredParamsCount)
					{
						context.ReportDiagnostic(
							Diagnostic.Create(
								descriptor: new(
									id: DiagnosticIds.Sudoku010,
									title: Titles.Sudoku010,
									messageFormat: Messages.Sudoku010,
									category: Categories.Usage,
									defaultSeverity: DiagnosticSeverity.Error,
									isEnabledByDefault: true,
									helpLinkUri: HelpLinks.Sudoku010
								),
								location: nameNode.GetLocation(),
								messageArgs: new object[] { methodName, requiredParamsCount, actualParamsCount }
							)
						);

						goto CheckSudoku012;
					}

					switch (methodName)
					{
						case ChangeLanguageMethodName:
						{
							var actualTypeSymbol = semanticModel.GetOperation(
								argNodes.Arguments[0].Expression
							)!.Type;
							var expectedTypeSymbol = compilation.GetTypeByMetadataName(CountryCodeTypeName)!;
							if (!SymbolEqualityComparer.Default.Equals(actualTypeSymbol, expectedTypeSymbol))
							{
								context.ReportDiagnostic(
									Diagnostic.Create(
										descriptor: new(
											id: DiagnosticIds.Sudoku011,
											title: Titles.Sudoku011,
											messageFormat: Messages.Sudoku011,
											category: Categories.Usage,
											defaultSeverity: DiagnosticSeverity.Error,
											isEnabledByDefault: true,
											helpLinkUri: HelpLinks.Sudoku011
										),
										location: nameNode.GetLocation(),
										messageArgs: new object?[]
										{
											methodName,
											CountryCodeTypeName,
											argNodes.Arguments[0].GetParamFullName(semanticModel)
										}
									)
								);
							}

							break;
						}
						case SerializeMethodName:
						case DeserializeMethodName:
						{
							var expectedTypeSymbols = new[]
							{
								compilation.GetSpecialType(SpecialType.System_String),
								compilation.GetSpecialType(SpecialType.System_String)
							};
							for (int i = 0; i < 2; i++)
							{
								var actualTypeSymbol = semanticModel.GetOperation(
									argNodes.Arguments[i].Expression
								)!.Type;
								if
								(
									!SymbolEqualityComparer.Default.Equals(
										actualTypeSymbol,
										expectedTypeSymbols[i]
									)
								)
								{
									context.ReportDiagnostic(
										Diagnostic.Create(
											descriptor: new(
												id: DiagnosticIds.Sudoku011,
												title: Titles.Sudoku011,
												messageFormat: Messages.Sudoku011,
												category: Categories.Usage,
												defaultSeverity: DiagnosticSeverity.Error,
												isEnabledByDefault: true,
												helpLinkUri: HelpLinks.Sudoku011
											),
											location: argNodes.Arguments[i].GetLocation(),
											messageArgs: new object?[]
											{
												methodName,
												"string",
												argNodes.Arguments[i].GetParamFullName(semanticModel)
											}
										)
									);
								}
							}

							break;
						}
					}

				CheckSudoku012:
					if (node.Parent is not ExpressionStatementSyntax)
					{
						context.ReportDiagnostic(
							Diagnostic.Create(
								descriptor: new(
									id: DiagnosticIds.Sudoku012,
									title: Titles.Sudoku012,
									messageFormat: Messages.Sudoku012,
									category: Categories.Usage,
									defaultSeverity: DiagnosticSeverity.Error,
									isEnabledByDefault: true,
									helpLinkUri: HelpLinks.Sudoku012
								),
								location: node.GetLocation(),
								messageArgs: new[] { methodName }
							)
						);
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
