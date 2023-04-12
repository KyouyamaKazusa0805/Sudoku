namespace Sudoku.Diagnostics.CodeGen.Generators;

using Data = (INamedTypeSymbol ContainingType, IMethodSymbol Method, ImmutableArray<IParameterSymbol> Parameters, SyntaxTokenList Modifiers, INamedTypeSymbol AttributeType, string AssemblyName);

/// <summary>
/// Defines a source generator that generates the source code for deconstruction methods.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class DeconstructionMethodGenerator : IIncrementalGenerator
{
	/// <inheritdoc/>
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		context.RegisterSourceOutput(
			context.SyntaxProvider
				.ForAttributeWithMetadataName("System.Diagnostics.CodeGen.DeconstructionMethodAttribute", nodePredicate, transformInstance)
				.Where(static data => data is not null)
				.Select(static (data, _) => data!.Value)
				.Collect(),
			outputInstance
		);


		static bool nodePredicate(SyntaxNode n, CancellationToken _) => n is MethodDeclarationSyntax { Modifiers: var m and not [] };

		static string toPascalCase(string name)
			=> name switch
			{
				['_', .. var slice] => toPascalCase(slice),
				[>= 'A' and <= 'Z', ..] => name,
				[var ch and >= 'a' and <= 'z', .. var slice] => $"{char.ToUpper(ch)}{slice}",
				_ => name
			};

		static Data? transformInstance(GeneratorAttributeSyntaxContext gasc, CancellationToken ct)
		{
			if (gasc is not
				{
					Attributes.Length: 1,
					TargetNode: MethodDeclarationSyntax { Modifiers: var modifiers } node,
					TargetSymbol: IMethodSymbol
					{
						Name: "Deconstruct",
						TypeParameters: [],
						Parameters: var parameters and not [],
						IsStatic: false,
						ReturnsVoid: true,
						ContainingType: { ContainingType: null, IsFileLocal: false } type
					} symbol,
					SemanticModel.Compilation: { AssemblyName: { } assemblyName } compilation
				})
			{
				return null;
			}

			if (!parameters.All(static p => p.RefKind == RefKind.Out))
			{
				return null;
			}

			var argumentAttributeType = compilation.GetTypeByMetadataName("System.Diagnostics.CodeGen.DeconstructionMethodArgumentAttribute");
			if (argumentAttributeType is null)
			{
				return null;
			}

			return (type, symbol, parameters, modifiers, argumentAttributeType, assemblyName);
		}

		void outputInstance(SourceProductionContext spc, ImmutableArray<Data> data)
		{
			_ = spc is { CancellationToken: var ct };

			foreach (var group in data.GroupBy<Data, INamedTypeSymbol>(static data => data.ContainingType, SymbolEqualityComparer.Default))
			{
				var containingType = group.Key;
				var typeName = containingType.Name;
				var @namespace = containingType.ContainingNamespace;
				var typeParameters = containingType.TypeParameters;

				var namespaceStr = @namespace?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) switch
				{
					{ } s => $"namespace {s["global::".Length..]};\r\n\r\n",
					_ => string.Empty
				};

				var typeParametersStr = typeParameters switch
				{
					[] => string.Empty,
					_ => $"<{string.Join(", ", from typeParameter in typeParameters select typeParameter.Name)}>"
				};

				var codeSnippets = new List<string>();
				foreach (var element in group)
				{
					if (element is not
						{
							Method: { DeclaredAccessibility: var methodAccessibility } method,
							Parameters: var parameters,
							Modifiers: var modifiers,
							AttributeType: var attributeType,
							AssemblyName: var assemblyName
						})
					{
						continue;
					}

					var parameterNameData = new List<(string Parameter, string Member)>();
					foreach (var parameter in parameters)
					{
						var name = parameter.Name;
						parameterNameData.Add(
							parameter.GetAttributes().FirstOrDefault(predicate) switch
							{
								{ ConstructorArguments: [{ Value: string s }] } => (name, s),
								_ => (name, toPascalCase(name))
							}
						);


						bool predicate(AttributeData a) => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeType);
					}

					var assignmentsCode = string.Join("\r\n\t\t", from t in parameterNameData select $"{t.Parameter} = {t.Member};");

					var argsStr = string.Join(
						", ",
						from parameter in parameters
						let parameterType = parameter.Type
						let name = parameterType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
						let annotation = parameterType is { NullableAnnotation: NullableAnnotation.Annotated, IsReferenceType: true }
							? "?"
							: string.Empty
						select $"out {name}{annotation} {parameter.Name}"
					);

					var includingReferenceLevel = assemblyName.StartsWith("SudokuStudio") ? "../../../" : "../../";

					codeSnippets.Add(
						$$"""
						/// <include file="{{includingReferenceLevel}}global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
							[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
							[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{GetType().FullName}}", "{{VersionValue}}")]
							[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
							{{modifiers}} void Deconstruct({{argsStr}})
							{
								{{assignmentsCode}}
							}
						"""
					);
				}

				spc.AddSource(
					$"{containingType.ToFileName()}.g.{Shortcuts.GeneratedDeconstruction}.cs",
					$$"""
					// <auto-generated/>
					
					#nullable enable
					
					{{namespaceStr}}partial {{containingType.GetTypeKindModifier()}} {{typeName}}{{typeParametersStr}}
					{
						{{string.Join("\r\n\r\n\t", codeSnippets)}}
					}
					"""
				);
			}
		}
	}
}
