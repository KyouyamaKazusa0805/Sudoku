namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines a source generator that generates for the code that is for the overriden of a type.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class AutoOverridesToStringGenerator : IIncrementalGenerator
{
	private const string AttributeFullName = "System.Diagnostics.CodeGen.AutoOverridesToStringAttribute";


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
		return attributeData is null ? null : (typeSymbol, attributeData);
	}

	private static void OutputSource(SourceProductionContext spc, ImmutableArray<(INamedTypeSymbol, AttributeData)?> list)
	{
		var recordedList = new List<INamedTypeSymbol>();
		foreach (var v in list)
		{
			if (v is not (var type, { ConstructorArguments: [{ Values: var typedConstants }] } attributeData))
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
						Name: nameof(ToString),
						Parameters: [],
						ReturnType.SpecialType: SpecialType.System_String,
						IsImplicitlyDeclared: false
					}
				)
			)
			{
				continue;
			}

			var (_, _, namespaceName, genericParameterList, _, _, readOnlyKeyword, _, _, _) =
				SymbolOutputInfo.FromSymbol(type);

			// Iterates on each member in member name list having been defined in the attribute data
			// whose corresponding attribute is needed.
			// Here, 'targetSymbolsRawString' is a list that stores all valid member assignments.
			// For example, expression '{{{nameof(Hello)}}} = {{{Hello}}}',
			// while 'symbolsRawValue' is a list that stores all valid member names.
			var targetSymbolsRawString = new List<string>();
			var symbolsRawValue = new List<string>();
			foreach (var typedConstant in typedConstants)
			{
				// Gets the member name via the constructor arguments.
				string memberName = (string)typedConstant.Value!;

				// Checks whether the specified member is in the target type.
				// Please note that we call the method 'GetAllMembers', which means all members
				// in base types or interfaces may also be to be checked.
				var selectedMembers = (from member in members where member.Name == memberName select member).ToArray();
				if (selectedMembers is not [var memberSymbol, ..])
				{
					continue;
				}

				// Gets the valid names and the raw assignment string value.
				switch (memberSymbol)
				{
					case IFieldSymbol { Name: var fieldName }:
					{
						targetSymbolsRawString.Add($$$"""{{nameof({{{fieldName}}})}} = {{{{{fieldName}}}}}""");
						symbolsRawValue.Add(fieldName);
						break;
					}
					case IPropertySymbol { GetMethod: not null, Name: var propertyName }:
					{
						targetSymbolsRawString.Add($$$"""{{nameof({{{propertyName}}})}} = {{{{{propertyName}}}}}""");
						symbolsRawValue.Add(propertyName);
						break;
					}
					case IMethodSymbol { Name: var methodName, Parameters: [], ReturnsVoid: false }:
					{
						targetSymbolsRawString.Add($$$"""{{nameof({{{methodName}}})}} = {{{{{methodName}}}()}}""");
						symbolsRawValue.Add($"{methodName}()");
						break;
					}
				}
			}

			// Gets the final output expression via the user-defined pattern syntax and the normal one.
			// If the property 'Pattern' is null, the source generator will use the return value
			// of record-like 'ToString' method invocation to emit the string as the result,
			// like the following expression:
			// 
			//     Type { Property1 = value1, Property2 = value2 }
			// 
			// Which put the type name at the first side, and list all property names and their own values,
			// separated by commas.
			string outputStringExpression = attributeData.GetNamedArgument<string>("Pattern") switch
			{
				{ } pattern => convert(pattern) switch
				{
					var convertedPattern => isSimpleInterpolatedPattern(pattern) switch
					{
						true => convertedPattern[1..^1],
						_ => $"""
						{interpolatedStringSuffix(pattern)}"{convertedPattern}"
						"""
					}
				},
				_ => $$""""
				$$"""{{type.Name}} { {{string.Join(", ", targetSymbolsRawString)}} }"""
				""""
			};

			// Emits the source code.
			spc.AddSource(
				$"{type.ToFileName()}.g.{Shortcuts.AutoOverridesToString}.cs",
				$$"""
				#nullable enable
				
				namespace {{namespaceName}};
				
				partial {{type.GetTypeKindModifier()}} {{type.Name}}{{genericParameterList}}
				{
					/// <inheritdoc cref="object.ToString"/>
					[global::System.Runtime.CompilerServices.CompilerGenerated]
					[global::System.CodeDom.Compiler.GeneratedCode("{{typeof(AutoOverridesToStringGenerator).FullName}}", "{{VersionValue}}")]
					[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
					public override {{readOnlyKeyword}}string ToString()
						=> {{outputStringExpression}};
				}
				"""
			);

			recordedList.Add(type);


			static bool isSimpleInterpolatedPattern(string pattern)
				=> Regex.Matches(pattern, """(?<={).+?(?=})""").Count == 1 && pattern is ['{', .., '}'];

			string convert(string pattern)
				=> Regex
					.Replace(pattern, """(\[0\]|\[[1-9]\d*\])""", m => symbolsRawValue[int.Parse(m.Value[1..^1])])
					.Replace("*", $"{nameof(ToString)}()");

			static string interpolatedStringSuffix(string pattern)
				=> Regex.IsMatch(pattern, """(?<={).+?(?=})""") ? "$" : string.Empty;
		}
	}
}
