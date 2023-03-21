namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Represents a source generator that generates the step searcher importing code.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class StepSearcherImportGenerator : IIncrementalGenerator
{
	/// <summary>
	/// Indicates the property name of <c>Areas</c>.
	/// </summary>
	private const string AreasPropertyName = "Areas";


	/// <inheritdoc/>
	public void Initialize(IncrementalGeneratorInitializationContext context)
		=> context.RegisterSourceOutput(context.CompilationProvider, Output);

	private void Output(SourceProductionContext spc, Compilation compilation)
	{
		var assemblySymbol = compilation.Assembly;

		// Checks whether the assembly has marked any attributes.
		if (assemblySymbol.GetAttributes() is not { IsDefaultOrEmpty: false } attributesData)
		{
			return;
		}

		var stepSearcherBaseType = compilation.GetTypeByMetadataName("Sudoku.Analytics.StepSearcher");
		if (stepSearcherBaseType is null)
		{
			return;
		}

		var runningAreaTypeSymbol = compilation.GetTypeByMetadataName("Sudoku.Analytics.Metadata.StepSearcherRunningArea")!;
		var levelTypeSymbol = compilation.GetTypeByMetadataName("Sudoku.Analytics.Metadata.StepSearcherLevel")!;
		var runningAreasFields = new Dictionary<byte, string>();
		var levelFields = new Dictionary<byte, string>();
		foreach (var fieldSymbol in runningAreaTypeSymbol.GetMembers().OfType<IFieldSymbol>())
		{
			if (fieldSymbol is { ConstantValue: byte value, Name: var fieldName })
			{
				runningAreasFields.Add(value, fieldName);
			}
		}
		foreach (var fieldSymbol in levelTypeSymbol.GetMembers().OfType<IFieldSymbol>())
		{
			if (fieldSymbol is { ConstantValue: byte value, Name: var fieldName })
			{
				levelFields.Add(value, fieldName);
			}
		}

		// Gather the valid attributes data.
		var foundAttributesData = new List<Data>();
		const string comma = ", ";
		const string attributeTypeName = "global::Sudoku.Analytics.Metadata.StepSearcherImportAttribute<>";
		var priorityValue = 0;
		foreach (var attributeData in attributesData)
		{
			// Check validity.
#pragma warning disable format
			if (attributeData is not
				{
					AttributeClass:
					{
						IsGenericType: true,
						TypeArguments:
						[
							INamedTypeSymbol
							{
								IsRecord: false,
								ContainingNamespace: var containingNamespace,
								Name: var stepSearcherName,
								BaseType: { } baseType
							} stepSearcherType
						]
					} attributeClassSymbol,
					ConstructorArguments: [{ Type.TypeKind: Kind.Enum, Value: byte dl }],
					NamedArguments: var namedArguments
				})
#pragma warning restore format
			{
				continue;
			}

			// Checks whether the type is valid.
			var unboundAttributeTypeSymbol = attributeClassSymbol.ConstructUnboundGenericType();
			if (unboundAttributeTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) != attributeTypeName)
			{
				continue;
			}

			// Check whether the step searcher can be used for deriving.
			const string polymorphismAttributeName = "Sudoku.Analytics.Metadata.PolymorphismAttribute";
			var polymorphismAttributeType = compilation.GetTypeByMetadataName(polymorphismAttributeName)!;
			var isPolymorphism = stepSearcherType.GetAttributes().Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, polymorphismAttributeType));

			// Adds the necessary info into the collection.
			foundAttributesData.Add(new(containingNamespace, baseType, priorityValue++, dl, stepSearcherName, namedArguments, isPolymorphism));
		}

		// Iterate on each valid attribute data, and checks the inner value to be used by the source generator to output.
		var generatedCodeSnippets = new List<string>();
		var namespaceUsed = foundAttributesData[0].Namespace;
		foreach (var (_, baseType, priority, level, name, namedArguments, isPolymorphism) in foundAttributesData)
		{
			// Checks whether the attribute has configured any extra options.
			var nullableRunningArea = default(byte?);
			if (namedArguments is not [])
			{
				foreach (var (k, v) in namedArguments)
				{
					if (k == AreasPropertyName && v is { Value: byte value })
					{
						nullableRunningArea = value;
					}
				}
			}

			// Gather the extra options on step searcher.
			var levelStr = createLevelExpression(level, levelFields);
			var runningAreaStr = nullableRunningArea switch
			{
				{ } runningArea => createRunningAreasExpression(runningArea, runningAreasFields),
				_ => null
			};

			var sb = new StringBuilder().Append(levelStr);
			_ = runningAreaStr is not null ? sb.Append(comma).Append(runningAreaStr) : default;

			// Output the generated code.
			var baseTypeFullName = baseType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
			generatedCodeSnippets.Add(
				isPolymorphism
					? $$"""
					partial class {{name}} : {{baseTypeFullName}}
					{
						[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
						[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{GetType().Name}}", "{{VersionValue}}")]
						public {{name}}() : base({{priority}}, {{levelStr}}{{(runningAreaStr is not null ? $", {runningAreaStr}" : string.Empty)}})
						{
						}

						/// <param name="priority">
						/// <inheritdoc
						///     cref="global::Sudoku.Analytics.StepSearcher(int, global::Sudoku.Analytics.Metadata.StepSearcherLevel, global::Sudoku.Analytics.Metadata.StepSearcherRunningArea)"
						///     path="/param[@name='priority']"/>
						/// </param>
						/// <param name="level">
						/// <inheritdoc
						///     cref="global::Sudoku.Analytics.StepSearcher(int, global::Sudoku.Analytics.Metadata.StepSearcherLevel, global::Sudoku.Analytics.Metadata.StepSearcherRunningArea)"
						///     path="/param[@name='level']"/>
						/// </param>
						/// <param name="runningArea">
						/// <inheritdoc
						///     cref="global::Sudoku.Analytics.StepSearcher(int, global::Sudoku.Analytics.Metadata.StepSearcherLevel, global::Sudoku.Analytics.Metadata.StepSearcherRunningArea)"
						///     path="/param[@name='runningArea']"/>
						/// </param>
						[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
						[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{GetType().Name}}", "{{VersionValue}}")]
						public {{name}}(
							int priority,
							global::Sudoku.Analytics.Metadata.StepSearcherLevel level,
							global::Sudoku.Analytics.Metadata.StepSearcherRunningArea runningArea = global::Sudoku.Analytics.Metadata.StepSearcherRunningArea.Searching | global::Sudoku.Analytics.Metadata.StepSearcherRunningArea.Gathering
						) : base(priority, level, runningArea)
						{
						}
					}
					"""
					: $"partial class {name}() : {baseTypeFullName}({priority}, {sb});"
			);
		}

		spc.AddSource(
			$"StepSearcherImports.g.{Shortcuts.StepSearcherImports}.cs",
			$$"""
			// <auto-generated/>

			#pragma warning disable CS1591
			#nullable enable
			namespace {{namespaceUsed.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)["global::".Length..]}};
			
			{{string.Join($"{Environment.NewLine}{Environment.NewLine}", generatedCodeSnippets)}}
			"""
		);


		static unsafe string createRunningAreasExpression(byte field, IDictionary<byte, string> runningAreasFields)
		{
			var l = (int)field;
			if (l == 0)
			{
				return "0";
			}

			var targetList = new List<string>();
			for (var (temp, i) = (l, 0); temp != 0; temp >>= 1, i++)
			{
				if ((temp & 1) != 0)
				{
					targetList.Add($"global::Sudoku.Analytics.Metadata.StepSearcherRunningArea.{runningAreasFields[(byte)(1 << i)]}");
				}
			}

			return string.Join(" | ", targetList);
		}

		static unsafe string createLevelExpression(byte field, IDictionary<byte, string> levelFields)
		{
			if (field == 0)
			{
				return "0";
			}

			foreach (var (v, n) in levelFields)
			{
				if (v == field)
				{
					return $"global::Sudoku.Analytics.Metadata.StepSearcherLevel.{n}";
				}
			}

			return string.Empty;
		}
	}
}

/// <summary>
/// Simply encapsulates a data tuple describing the information of a found attribute.
/// </summary>
/// <param name="Namespace">Indicates the namespace symbol of that step searcher.</param>
/// <param name="BaseType">Indicates the base type of the step searcher.</param>
/// <param name="PriorityValue">The priority value of the step searcher.</param>
/// <param name="DifficultyLevel">The difficulty level of the step searcher.</param>
/// <param name="TypeName">The name of the step searcher type.</param>
/// <param name="NamedArguments">The named arguments of that attribute.</param>
/// <param name="IsPolymorphism">Indicates whether the step searcher can be also used for deriving.</param>
file sealed record Data(
	INamespaceSymbol Namespace,
	INamedTypeSymbol BaseType,
	int PriorityValue,
	byte DifficultyLevel,
	string TypeName,
	ImmutableArray<KeyValuePair<string, TypedConstant>> NamedArguments,
	bool IsPolymorphism
);
