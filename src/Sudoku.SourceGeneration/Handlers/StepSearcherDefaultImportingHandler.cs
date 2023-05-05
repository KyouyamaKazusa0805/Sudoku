namespace Sudoku.Diagnostics.CodeGen;

/// <summary>
/// The generator handler for step searcher default importing.
/// </summary>
internal sealed class StepSearcherDefaultImportingHandler : IIncrementalGeneratorCompilationHandler
{
	private const string AreasPropertyName = "Areas";

	private const string StepSearcherTypeName = "Sudoku.Analytics.StepSearcher";

	private const string StepSearcherRunningAreaTypeName = "Sudoku.Analytics.Metadata.StepSearcherRunningArea";

	private const string StepSearcherLevelTypeName = "Sudoku.Analytics.Metadata.StepSearcherLevel";

	private const string StepSearcherImportAttributeName = "global::Sudoku.Analytics.Metadata.StepSearcherImportAttribute<>";

	private const string PolymorphismAttributeName = "Sudoku.Analytics.Metadata.PolymorphismAttribute";


	/// <inheritdoc/>
	public void Output(SourceProductionContext spc, Compilation compilation)
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
		var levelTypeSymbol = compilation.GetTypeByMetadataName(StepSearcherLevelTypeName)!;
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
		var foundAttributesData = new List<StepSearcherDefaultImportingCollectedResult>();
		const string comma = ", ";
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
					ConstructorArguments: [{ Type.TypeKind: TypeKind.Enum, Value: byte dl }],
					NamedArguments: var namedArguments
				})
#pragma warning restore format
			{
				continue;
			}

			// Checks whether the type is valid.
			var unboundAttributeTypeSymbol = attributeClassSymbol.ConstructUnboundGenericType();
			if (unboundAttributeTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) != StepSearcherImportAttributeName)
			{
				continue;
			}

			// Check whether the step searcher can be used for deriving.
			var polymorphismAttributeType = compilation.GetTypeByMetadataName(PolymorphismAttributeName)!;
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


		static string createRunningAreasExpression(byte field, IDictionary<byte, string> runningAreasFields)
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

		static string createLevelExpression(byte field, IDictionary<byte, string> levelFields)
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
