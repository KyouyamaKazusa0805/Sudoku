namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines a source generator that generates the well-known diagnostic descriptor list and last code analyzer members.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class WellKnownDiagnosticDescriptorsAndCodeAnalyzerMembersGenerator : IIncrementalGenerator
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

	private static readonly DiagnosticDescriptor SCA0005 = new(
		nameof(SCA0005),
		"The specified operation action kind is not supported",
		"The specified operation action kind '{0}' is not supported",
		"CodeGen",
		DiagnosticSeverity.Error,
		true,
		$"The specified operation action kind is not supported; Supported values are '{nameof(SyntaxKind)}', '{nameof(SymbolKind)}' and '{nameof(OperationKind)}'.",
		"https://sunnieshine.github.io/Sudoku/code-analysis/sca0005"
	);

	private static readonly DiagnosticDescriptor SCA0006 = new(
		nameof(SCA0006),
		"Code analyzer name does not satisfy the pattern",
		"Code analyzer name '{0}' does not satisfy the pattern",
		"CodeGen",
		DiagnosticSeverity.Warning,
		true,
		"Code analyzer name does not satisfy the pattern; it should name as 'SCA[0]_[1]Analyzer', where [0] must be a four-digit number, and [1] must be the analysis rule name of the analyzer.",
		"https://sunnieshine.github.io/Sudoku/code-analysis/sca0005"
	);

	/// <summary>
	/// The found descriptors.
	/// </summary>
	private DiagnosticDescriptor[]? _foundDescriptors;


	/// <inheritdoc/>
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		context.RegisterSourceOutput(
			context.AdditionalTextsProvider
				.Where(static a => a.Path.EndsWith("CodeAnalysisDetailList.csv"))
				.Collect(),
			OutputAction_Step1
		);

		context.RegisterSourceOutput(
			context.SyntaxProvider
				.CreateSyntaxProvider(
					static (node, _) => node is ClassDeclarationSyntax,
					Transform_Step2
				)
				.Where(static e => e is not null)
				.Collect(),
			OutputAction_Step2
		);
	}

	private void OutputAction_Step1(SourceProductionContext spc, ImmutableArray<AdditionalText> additionalTextFiles)
	{
		if (additionalTextFiles is not [{ Path: var path }])
		{
			return;
		}

		_foundDescriptors = DiagnosticDescriptorSerializer.GetDiagnosticDescriptorsFromFile(path);
		var text = string.Join(
			"\r\n\r\n\t",
			from descriptor in _foundDescriptors
			let helpLinkUrl = $"https://sunnieshine.github.io/Sudoku/code-analysis/{descriptor.Id.ToLower()}"
			select
				$$"""
				/// <summary>
					/// Indicates the diagnostic result <see href="{{helpLinkUrl}}">{{descriptor.Id}}</see>:
					/// <list type="bullet">
					/// <item><b>Title</b>: {{descriptor.Title}}</item>
					/// <item><b>Description</b>: {{descriptor.Description}}</item>
					/// <item><b>Category</b>: {{descriptor.Category}}</item>
					/// <item><b>Severity</b>: <see cref="global::Microsoft.CodeAnalysis.DiagnosticSeverity.{{descriptor.DefaultSeverity}}"/></item>
					/// </list>
					/// </summary>
					public static readonly global::Microsoft.CodeAnalysis.DiagnosticDescriptor {{descriptor.Id}} =
						new(
							nameof({{descriptor.Id}}),
							"{{descriptor.Title}}",
							"{{descriptor.MessageFormat}}",
							"{{descriptor.Category}}",
							global::Microsoft.CodeAnalysis.DiagnosticSeverity.{{descriptor.DefaultSeverity}},
							true,
							"{{descriptor.Description}}",
							"{{helpLinkUrl}}"
						);
				"""
		);

		spc.AddSource(
			$"WellKnownDiagnosticDescriptors.g.{Shortcuts.DiagnosticDescriptorList}.cs",
			$$"""
			namespace Sudoku.Diagnostics.CodeAnalysis;

			/// <summary>
			/// Represents with the well-known diagnostic descriptors.
			/// </summary>
			[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{GetType().FullName}}", "{{VersionValue}}")]
			[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
			internal static class WellKnownDiagnosticDescriptors
			{
				{{text}}
			}
			"""
		);
	}

	private static TypeSymbolData? Transform_Step2(GeneratorSyntaxContext gsc, CancellationToken ct)
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

	private void OutputAction_Step2(SourceProductionContext spc, ImmutableArray<TypeSymbolData?> foundTypeSymbols)
	{
		if (_foundDescriptors is null)
		{
			return;
		}

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
						{ Value: INamedTypeSymbol { Name: var operationKindTypeName, ContainingNamespace: var operationKindTypeNamespace } },
						{ Values: var kindsStr }
					]
				})
#pragma warning restore format
			{
				r(spc, identifier, "RegisterOperationActionAttribute");
				return;
			}

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
										{a(accessibility)} const string
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
#if false
				nameof(Compilation) => nameof(CompilationAnalysisContext),
				nameof(SyntaxTree) => nameof(SyntaxTreeAnalysisContext),
				nameof(BlockSyntax) => nameof(CodeBlockAnalysisContext),
				nameof(SemanticModel) => nameof(SemanticModelAnalysisContext),
				nameof(AdditionalText) => nameof(AdditionalFileAnalysisContext),
#endif
				_ => null
			};
			if (contextType is null)
			{
				rrr(SCA0005, spc, identifier, operationKindTypeName);
				return;
			}

			var docList = string.Join(
				"\r\n/// ",
				from id in supportedIds
				let title = _foundDescriptors.FirstOrDefault(e => e.Id == id)?.Title
				where title is not null
				let idLower = id.ToLower()
				select
					// lang = xml
					$"""<item><see href="https://sunnieshine.github.io/Sudoku/code-analysis/{idLower}">{id}</see> ({title})</item>"""
			);
			var rawTypeNameAsFile = type.ToFileName();
			var finalTypeFileName = (rawTypeNameAsFile.IndexOf("SCA"), rawTypeNameAsFile.IndexOf('_')) switch
			{
				(var scaIndex and not -1, var underscoreIndex and not -1) when underscoreIndex > scaIndex
					=> rawTypeNameAsFile[scaIndex..underscoreIndex],
				_
					=> rawTypeNameAsFile
			};
			if (finalTypeFileName == rawTypeNameAsFile)
			{
				rrr(SCA0006, spc, identifier, finalTypeFileName);
			}

			var kindsArrayStr = string.Join(", ", from element in kindsStr select $"{operationKindName}.{(string)element.Value!}");
			spc.AddSource(
				$"{finalTypeFileName}.g.{Shortcuts.CodeAnalyzer}.cs",
				$$"""
				// <auto-generated/>
						
				#nullable enable

				namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

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

						context.{{operationName}}(AnalyzeCore, new[] { {{kindsArrayStr}} });
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

		static void rrr(DiagnosticDescriptor descriptor, SourceProductionContext spc, SyntaxToken identifier, params object?[]? extraData)
			=> spc.ReportDiagnostic(Diagnostic.Create(descriptor, identifier.GetLocation(), extraData));

		static string a(CSharpAccessibility accessibility)
			=> accessibility switch
			{
				CSharpAccessibility.Private => "private",
				CSharpAccessibility.Protected => "protected",
				CSharpAccessibility.PrivateProtected => "private protected",
				CSharpAccessibility.ProtectedInternal => "protected internal",
				CSharpAccessibility.Internal => "internal",
				CSharpAccessibility.Public => "public",
				CSharpAccessibility.File => throw new NotSupportedException("The specified accessibilty is not supported."),
				_ => throw new ArgumentOutOfRangeException(nameof(accessibility))
			};
	}


	private readonly record struct TypeSymbolData(
		INamedTypeSymbol FoundType,
		SyntaxToken Identifier,
		INamedTypeSymbol SupportedDiagnostics,
		INamedTypeSymbol RegisterOperationAction,
		INamedTypeSymbol RegisteredPropertyNames
	);
}
