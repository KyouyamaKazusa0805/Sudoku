namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines a source generator that generates the source code for code analyzer.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class CodeAnalyzerGenerator : IIncrementalGenerator
{
	private static readonly DiagnosticDescriptor SCA0002 = new(
		nameof(SCA0002),
		"Attribute type missing on analyzer type",
		"Attribute type '{0}' is missing on analyzer type",
		"CodeGen",
		DiagnosticSeverity.Error,
		true,
		"Attribute type is missing on analyzer type.",
		"https://sunnieshine.github.io/Sudoku/code-analysis/sca0002"
	);

	private static readonly DiagnosticDescriptor SCA0003 = new(
		nameof(SCA0003),
		"The second argument requires at least one element",
		"The second argument requires at least one element",
		"CodeGen",
		DiagnosticSeverity.Warning,
		true,
		"The second argument of attribute type 'RegisteredPropertyNamesAttribute' requires at least one element.",
		"https://sunnieshine.github.io/Sudoku/code-analysis/sca0003"
	);

	private static readonly DiagnosticDescriptor SCA0004 = new(
		nameof(SCA0004),
		"Code analyzer type should be sealed, and not abstract",
		"Code analyzer type should be sealed, and not abstract",
		"CodeGen",
		DiagnosticSeverity.Warning,
		true,
		"Code analyzer type should be sealed, and not abstract.",
		"https://sunnieshine.github.io/Sudoku/code-analysis/sca0005"
	);


	/// <inheritdoc/>
	public void Initialize(IncrementalGeneratorInitializationContext context)
		=> context.RegisterSourceOutput(
			context.SyntaxProvider
				.CreateSyntaxProvider(
					static (node, _) => node is ClassDeclarationSyntax,
					static TypeSymbolData? (gsc, ct) =>
					{
						if (gsc is not
							{
								Node: ClassDeclarationSyntax { Modifiers: var m and not [], Identifier: var identifier } node,
								SemanticModel: { Compilation: var compilation } semanticModel
							})
						{
							return null;
						}

						var baseType = f("Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer");
						var type = semanticModel.GetDeclaredSymbol(node, ct)!;
						if (!type.IsDerivedFrom(baseType) || !m.Any(SyntaxKind.PartialKeyword))
						{
							return null;
						}

						var idsAttributeType = f("Sudoku.Diagnostics.CodeGen.Annotations.SupportedDiagnosticsAttribute");
						var actionAttributeType = f("Sudoku.Diagnostics.CodeGen.Annotations.RegisterOperationActionAttribute");
						var propertyNamesAttributeType = f("Sudoku.Diagnostics.CodeGen.Annotations.RegisteredPropertyNamesAttribute");
						return new(type, identifier, idsAttributeType, actionAttributeType, propertyNamesAttributeType);


						INamedTypeSymbol f(string n)
							=> compilation.GetTypeByMetadataName(n) switch
							{
								{ } b => b,
								_ => throw new InvalidOperationException($"Type '{n[(n.LastIndexOf('.') + 1)..]}' is missing.")
							};
					}
				)
				.Where(static e => e is not null)
				.Collect(),
			(spc, foundTypeSymbols) =>
			{
				foreach (var data in foundTypeSymbols)
				{
#pragma warning disable format
					if (data is not (
						{ Name: var typeName, IsAbstract: var isAbstract, IsSealed: var isSealed } type,
							var identifier,
							var supportedDiagnostics,
							var registerOperationAction,
							var registeredPropertyNames
						))
					{
						continue;
					}
#pragma warning restore format

					if (isAbstract || !isSealed)
					{
						rr(SCA0004, spc, identifier);
					}

					var attributesData = type.GetAttributes();
					var supportedDiagnosticsData = f(attributesData, supportedDiagnostics);
					var registerOperationActionData = f(attributesData, registerOperationAction);
					var registeredPropertyNamesData = f(attributesData, registeredPropertyNames);
					if (supportedDiagnosticsData is not { ConstructorArguments: [{ Values: var ids }] })
					{
						r(spc, identifier, "SupportedDiagnosticsAttribute");
						return;
					}
#pragma warning disable format
					if (registerOperationActionData is not
						{
							ConstructorArguments:
							[
								{ Value: string operationName },
								{
									Value: INamedTypeSymbol
									{
										Name: var operationKindTypeName,
										ContainingNamespace: var operationKindTypeNamespace
									}
								},
								{ Value: string kindStr }
							]
						})
					{
						r(spc, identifier, "RegisterOperationActionAttribute");
						return;
					}
#pragma warning restore format
					if (registeredPropertyNamesData is { ConstructorArguments: [{ Values: [] }] })
					{
						rr(SCA0003, spc, identifier);
					}

					var extraPropertyNameFields = registeredPropertyNamesData switch
					{
						null => "// No registered property name fields.",
						{ ConstructorArguments: [{ Value: int rawAccessibility }, { Values: var values }] }
							=> (CSharpAccessibility)rawAccessibility switch
							{
								var accessibility => (from element in values select $"PropertyName_{element.Value} = \"{element.Value}\"") switch
								{
									var valueList => string.Join(",\r\n\t\t", valueList) switch
									{
										var content
											=> $"""
											/// <summary>
											/// The extra property fields. Such fields will be used while reporting diagnostics.
											/// </summary>
											{a(accessibility)}const string
													{content};
											"""
									}
								}
							}
					};

					var supportedIds = from value in ids select (string)value.Value!;
					var supportedIdsStr = string.Join(", ", supportedIds);
					var operationKindName = $"{operationKindTypeNamespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}.{operationKindTypeName}";
					var contextType = operationKindTypeName switch
					{
						nameof(OperationKind) => nameof(OperationAnalysisContext),
						nameof(SymbolKind) => nameof(SymbolAnalysisContext),
						nameof(SyntaxKind) => nameof(SyntaxNodeAnalysisContext),
						_ => throw new NotSupportedException($"The specified operation action kind '{operationKindTypeName}' is not supported.")
					};
					var docList = string.Join(
						"\r\n/// ",
						from id in supportedIds
						let idLower = id.ToLower()
						select $"<item><see href=\"https://sunnieshine.github.io/Sudoku/code-analysis/{idLower}\">{id}</see></item>"
					);
					spc.AddSource(
						$"{type.ToFileName()}.g.{Shortcuts.CodeAnalyzer}.cs",
						$$"""
						// <auto-generated/>
						
						#nullable enable

						namespace Sudoku.Diagnostics.CodeAnalysis;

						/// <summary>
						/// Indicates the analyzer that can provide the following diagnostic results:
						/// <list type="bullet">
						/// {{docList}}
						/// </list>
						/// </summary>
						[global::Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzerAttribute(global::Microsoft.CodeAnalysis.LanguageNames.CSharp)]
						partial class {{typeName}} : global::Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer
						{
							{{extraPropertyNameFields}}


							/// <inheritdoc/>
							[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{GetType().FullName}}", "{{VersionValue}}")]
							[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
							public override global::System.Collections.Immutable.ImmutableArray<global::Microsoft.CodeAnalysis.DiagnosticDescriptor> SupportedDiagnostics
								=> global::System.Collections.Immutable.ImmutableArray.Create({{supportedIdsStr}});


							/// <inheritdoc/>
							[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{GetType().FullName}}", "{{VersionValue}}")]
							[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
							public override void Initialize(global::Microsoft.CodeAnalysis.Diagnostics.AnalysisContext context)
							{
								context.ConfigureGeneratedCodeAnalysis(global::Microsoft.CodeAnalysis.Diagnostics.GeneratedCodeAnalysisFlags.None);
								context.EnableConcurrentExecution();

								context.{{operationName}}(AnalyzeCore, new[] { {{operationKindName}}.{{kindStr}} });
							}


							/// <summary>
							/// The core method to analyze the symbol, syntax node, operation or something else that can be analyzed.
							/// </summary>
							private static partial void AnalyzeCore({{contextType}} context);
						}
						"""
					);
				}


				static AttributeData? f(ImmutableArray<AttributeData> attributesData, INamedTypeSymbol attributeType)
					=> attributesData.FirstOrDefault(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeType));

				static void r(SourceProductionContext spc, SyntaxToken identifier, string typeName)
					=> spc.ReportDiagnostic(Diagnostic.Create(SCA0002, identifier.GetLocation(), messageArgs: new[] { typeName }));

				static void rr(DiagnosticDescriptor descriptor, SourceProductionContext spc, SyntaxToken identifier)
					=> spc.ReportDiagnostic(Diagnostic.Create(descriptor, identifier.GetLocation()));

				static string a(CSharpAccessibility accessibility)
					=> accessibility switch
					{
						CSharpAccessibility.Private => "private ",
						CSharpAccessibility.Protected => "protected ",
						CSharpAccessibility.PrivateProtected => "private protected ",
						CSharpAccessibility.ProtectedInternal => "protected internal ",
						CSharpAccessibility.Internal => "internal ",
						CSharpAccessibility.Public => "public ",
						CSharpAccessibility.File => throw new NotSupportedException("The specified accessibilty is not supported."),
						_ => throw new ArgumentOutOfRangeException(nameof(accessibility))
					};
			}
		);
}

file readonly record struct TypeSymbolData(
	INamedTypeSymbol FoundType,
	SyntaxToken Identifier,
	INamedTypeSymbol SupportedDiagnostics,
	INamedTypeSymbol RegisterOperationAction,
	INamedTypeSymbol RegisteredPropertyNames
);