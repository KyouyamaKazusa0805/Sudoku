namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines a source generator that generates for the code that is for the overriden of a type.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class AutoOverridesGetHashCodeGenerator : IIncrementalGenerator
{
	private const string AttributeFullName = "System.Diagnostics.CodeGen.AutoOverridesGetHashCodeAttribute";


	/// <inheritdoc/>
	public void Initialize(IncrementalGeneratorInitializationContext context)
		=> context.RegisterSourceOutput(
			context.SyntaxProvider
				.CreateSyntaxProvider(NodePredicate, GetValuesProvider)
				.Where(static element => element is not null)
				.Collect(),
			OutputSource
		);


	private static bool NodePredicate(SyntaxNode node, CancellationToken _)
		=> node is TypeDeclarationSyntax { Modifiers: var modifiers, AttributeLists.Count: > 0 }
			&& modifiers.Any(SyntaxKind.PartialKeyword);

	private static (INamedTypeSymbol, AttributeData)? GetValuesProvider(GeneratorSyntaxContext gsc, CancellationToken ct)
	{
		if (gsc is not { Node: TypeDeclarationSyntax n, SemanticModel: { Compilation: { } compilation } semanticModel })
		{
			return null;
		}

		if (semanticModel.GetDeclaredSymbol(n, ct) is not { ContainingType: null } typeSymbol)
		{
			return null;
		}

		var attributeTypeSymbol = compilation.GetTypeByMetadataName(AttributeFullName);
		var attributeData = (
			from a in typeSymbol.GetAttributes()
			where SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeTypeSymbol)
			select a
		).FirstOrDefault();
		return attributeData is not { ConstructorArguments: [{ Values: not [] }] } ? null : (typeSymbol, attributeData);
	}

	private static void OutputSource(SourceProductionContext spc, ImmutableArray<(INamedTypeSymbol, AttributeData)?> list)
	{
		var recordedList = new List<INamedTypeSymbol>();
		foreach (var v in list)
		{
			if (v is not var (type, attributeData))
			{
				continue;
			}

			if (recordedList.FindIndex(e => SymbolEqualityComparer.Default.Equals(e, type)) != -1)
			{
				continue;
			}

			var members = type.GetAllMembers();
			var methods = members.OfType<IMethodSymbol>().ToArray();
			if (
				!type.IsRecord && Array.Exists(
					methods,
					static symbol => symbol is
					{
						ContainingType.SpecialType: not (SpecialType.System_Object or SpecialType.System_ValueType),
						IsStatic: false,
						IsAbstract: false,
						Name: nameof(GetHashCode),
						Parameters: [],
						ReturnType.SpecialType: SpecialType.System_Int32,
						IsImplicitlyDeclared: false
					}
				)
			)
			{
				continue;
			}

			var (_, _, namespaceName, genericParameterList, _, _, readOnlyKeyword, _, _, _) =
				SymbolOutputInfo.FromSymbol(type);

			var targetSymbolsRawString = new List<string>();
			var symbolsRawValue = new List<string>();
			foreach (var typedConstant in attributeData.ConstructorArguments[0].Values)
			{
				string memberName = (string)typedConstant.Value!;

				// Checks whether the specified member is in the target type.
				var selectedMembers = (from member in members where member.Name == memberName select member).ToArray();
				if (selectedMembers is not [var memberSymbol, ..])
				{
					continue;
				}

				switch (memberSymbol)
				{
					case IFieldSymbol { Name: var fieldName }:
					{
						targetSymbolsRawString.Add(fieldName);
						symbolsRawValue.Add(fieldName);
						break;
					}
					case IPropertySymbol { GetMethod: not null, Name: var propertyName }:
					{
						targetSymbolsRawString.Add(propertyName);
						symbolsRawValue.Add(propertyName);
						break;
					}
					case IMethodSymbol { Name: var methodName, Parameters: [], ReturnsVoid: false }:
					{
						targetSymbolsRawString.Add($"{methodName}()");
						symbolsRawValue.Add($"{methodName}()");
						break;
					}
				}
			}

			bool isNotStruct = type.TypeKind != TypeKind.Struct;
			string? pattern = attributeData.GetNamedArgument<string>("Pattern");
			bool withSealedKeyword = attributeData.GetNamedArgument<bool>("EmitsSealedKeyword");
			string sealedKeyword = withSealedKeyword && isNotStruct ? "sealed " : string.Empty;
			string methodBody = targetSymbolsRawString.Count switch
			{
				<= 8 => pattern switch
				{
					null => $"\t\t=> global::System.HashCode.Combine({string.Join(", ", targetSymbolsRawString)});",
					_ => $"\t\t=> {convert(pattern)};",
				},
				_ => $$"""
					{
						var final = new global::System.HashCode();
						{{string.Join("\r\n\t\t", from e in targetSymbolsRawString select $"final.Add({e});")}}
						return final.ToHashCode();
					}
				"""
			};

			spc.AddSource(
				$"{type.ToFileName()}.g.{Shortcuts.AutoOverridesGetHashCode}.cs",
				$$"""
				#nullable enable
				
				namespace {{namespaceName}};
				
				partial {{type.GetTypeKindModifier()}} {{type.Name}}{{genericParameterList}}
				{
					/// <inheritdoc cref="object.GetHashCode"/>
					[global::System.Runtime.CompilerServices.CompilerGenerated]
					[global::System.CodeDom.Compiler.GeneratedCode("{{typeof(AutoOverridesGetHashCodeGenerator).FullName}}", "{{VersionValue}}")]
					[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
					public {{sealedKeyword}}override {{readOnlyKeyword}}int GetHashCode()
				{{methodBody}}
				}
				"""
			);

			recordedList.Add(type);


			string convert(string pattern)
				=> Regex
					.Replace(pattern, """(\[0\]|\[[1-9]\d*\])""", m => symbolsRawValue[int.Parse(m.Value[1..^1])])
					.Replace("*", $"{nameof(GetHashCode)}()");
		}
	}
}
