namespace Sudoku.SourceGeneration.Handlers;

/// <summary>
/// The generator handler for step searcher default importing.
/// </summary>
internal static class StepSearcherDefaultImportingHandler
{
	private const string AreasPropertyName = "Areas";

	private const string StepSearcherTypeName = "Sudoku.Analytics.StepSearcher";

	private const string StepSearcherRunningAreaTypeName = "Sudoku.Analytics.Metadata.StepSearcherRunningArea";

	private const string BuiltInStepSearcherAttributeName = "global::Sudoku.Analytics.Metadata.BuiltInStepSearcherAttribute<>";

	private const string StepSearcherSourceGenerationAttributeName = "Sudoku.Analytics.Metadata.StepSearcherSourceGenerationAttribute";


	/// <inheritdoc/>
	public static void Output(SourceProductionContext spc, Compilation compilation)
	{
		// Checks whether the assembly has marked any attributes.
		if (compilation.Assembly.GetAttributes() is not { IsDefaultOrEmpty: false } attributesData)
		{
			return;
		}

		var stepSearcherBaseType = compilation.GetTypeByMetadataName(StepSearcherTypeName);
		if (stepSearcherBaseType is null)
		{
			return;
		}

		var runningAreaTypeSymbol = compilation.GetTypeByMetadataName(StepSearcherRunningAreaTypeName)!;
		var runningAreasFields = new Dictionary<int, string>();
		foreach (var fieldSymbol in runningAreaTypeSymbol.GetMembers().OfType<IFieldSymbol>())
		{
			if (fieldSymbol is { ConstantValue: int value, Name: var fieldName })
			{
				runningAreasFields.Add(value, fieldName);
			}
		}

		// Gather the valid attributes data.
		var foundAttributesData = new List<CollectedResult>();
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
						TypeArguments: [
							INamedTypeSymbol
							{
								IsRecord: false,
								ContainingNamespace: var containingNamespace,
								Name: var stepSearcherName,
								BaseType: { } baseType
							} stepSearcherType
						]
					} attributeClassSymbol,
					ConstructorArguments: [{ Type.TypeKind: TypeKind.Struct, Value: int dl }],
					NamedArguments: var namedArguments
				})
#pragma warning restore format
			{
				continue;
			}

			// Checks whether the type is valid.
			var unboundAttributeTypeSymbol = attributeClassSymbol.ConstructUnboundGenericType();
			if (unboundAttributeTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) != BuiltInStepSearcherAttributeName)
			{
				continue;
			}

			// Check whether the step searcher can be used for deriving.
			var sourceGenerationAttributeType = compilation.GetTypeByMetadataName(StepSearcherSourceGenerationAttributeName)!;
			var isPolymorphism = stepSearcherType.GetAttributes()
				.Any(
					a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, sourceGenerationAttributeType)
						&& a.GetNamedArgument<bool>("CanDeriveTypes")
				);

			// Adds the necessary info into the collection.
			foundAttributesData.Add(new(containingNamespace, baseType, priorityValue++, dl, stepSearcherName, namedArguments, isPolymorphism));
		}

		// Iterate on each valid attribute data, and checks the inner value to be used by the source generator to output.
		var (generatedCodeSnippets, namespaceUsed) = (new List<string>(), foundAttributesData[0].Namespace);
		foreach (var (_, baseType, priority, level, name, namedArguments, isPolymorphism) in foundAttributesData)
		{
			// Checks whether the attribute has configured any extra options.
			var nullableRunningArea = default(int?);
			if (namedArguments is not [])
			{
				foreach (var (k, v) in namedArguments)
				{
					if (k == AreasPropertyName && v is { Value: int value })
					{
						nullableRunningArea = value;
					}
				}
			}

			// Gather the extra options on step searcher.
			var runningAreaStr = nullableRunningArea switch
			{
				{ } runningArea => createRunningAreasExpression(runningArea, runningAreasFields),
				_ => null
			};

			var sb = new StringBuilder().Append(level);
			if (runningAreaStr is not null)
			{
				sb.Append(", ").Append(runningAreaStr);
			}

			// Output the generated code.
			var baseTypeFullName = baseType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
			generatedCodeSnippets.Add(
				isPolymorphism
					? $$"""
					partial class {{name}} : {{baseTypeFullName}}
					{
						[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
						[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{typeof(StepSearcherDefaultImportingHandler).Name}}", "{{Value}}")]
						public {{name}}() : base({{priority}}, {{level}}{{(runningAreaStr is not null ? $", {runningAreaStr}" : string.Empty)}})
						{
						}

						/// <param name="priority">
						/// <inheritdoc
						///     cref="global::Sudoku.Analytics.StepSearcher(int, int, global::Sudoku.Analytics.Metadata.StepSearcherRunningArea)"
						///     path="/param[@name='priority']"/>
						/// </param>
						/// <param name="level">
						/// <inheritdoc
						///     cref="global::Sudoku.Analytics.StepSearcher(int, int, global::Sudoku.Analytics.Metadata.StepSearcherRunningArea)"
						///     path="/param[@name='level']"/>
						/// </param>
						/// <param name="runningArea">
						/// <inheritdoc
						///     cref="global::Sudoku.Analytics.StepSearcher(int, int, global::Sudoku.Analytics.Metadata.StepSearcherRunningArea)"
						///     path="/param[@name='runningArea']"/>
						/// </param>
						[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
						[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{typeof(StepSearcherDefaultImportingHandler).Name}}", "{{Value}}")]
						public {{name}}(
							int priority,
							int level,
							global::Sudoku.Analytics.Metadata.StepSearcherRunningArea runningArea = global::Sudoku.Analytics.Metadata.StepSearcherRunningArea.Searching | global::Sudoku.Analytics.Metadata.StepSearcherRunningArea.Collecting
						) : base(priority, level, runningArea)
						{
						}
					}
					"""
					: $"partial class {name}() : {baseTypeFullName}({priority}, {sb});"
			);
		}

		spc.AddSource(
			"StepSearcherImports.g.cs",
			$$"""
			{{Banner.AutoGenerated}}

			#pragma warning disable CS1591
			#nullable enable

			namespace {{namespaceUsed.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)["global::".Length..]}};
			
			{{string.Join("\r\n\r\n", generatedCodeSnippets)}}
			"""
		);


		static string createRunningAreasExpression(int field, IDictionary<int, string> runningAreasFields)
		{
			var l = field;
			if (l == 0)
			{
				return "0";
			}

			var targetList = new List<string>();
			for (var (temp, i) = (l, 0); temp != 0; temp >>= 1, i++)
			{
				if ((temp & 1) != 0)
				{
					targetList.Add($"global::Sudoku.Analytics.Metadata.StepSearcherRunningArea.{runningAreasFields[1 << i]}");
				}
			}

			return string.Join(" | ", targetList);
		}
	}
}

/// <summary>
/// Indicates the data collected via <see cref="StepSearcherDefaultImportingHandler"/>.
/// </summary>
/// <seealso cref="StepSearcherDefaultImportingHandler"/>
file sealed record CollectedResult(
	INamespaceSymbol Namespace,
	INamedTypeSymbol BaseType,
	int PriorityValue,
	int StepSearcherLevel,
	string TypeName,
	NamedArgs NamedArguments,
	bool IsPolymorphism
);
