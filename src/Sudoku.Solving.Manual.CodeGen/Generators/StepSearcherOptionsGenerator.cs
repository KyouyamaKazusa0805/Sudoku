namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines a source generator that generates the source code for the searcher options
/// on a step searcher.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class StepSearcherOptionsGenerator : IIncrementalGenerator
{
	/// <summary>
	/// Defines a dictionary that stores the lookup table for fields in the enumeration type <c>EnabledArea</c>.
	/// </summary>
	private readonly IDictionary<byte, string> _enabledAreasFields = new Dictionary<byte, string>();

	/// <summary>
	/// Defines a dictionary that stores the lookup table for fields in the enumeration type <c>DisabledReason</c>.
	/// </summary>
	private readonly IDictionary<short, string> _disabledReasonFields = new Dictionary<short, string>();


	/// <inheritdoc/>
	public void Initialize(IncrementalGeneratorInitializationContext context)
		=> context.RegisterSourceOutput(context.CompilationProvider, CreateSourceGeneration);

	private void CreateSourceGeneration(SourceProductionContext spc, Compilation compilation)
	{
		if (compilation is not { Assembly: { Name: "Sudoku.Solving.Manual" } assemblySymbol })
		{
			return;
		}

		// Checks whether the assembly has marked any attributes.
		if (assemblySymbol.GetAttributes() is not { IsDefaultOrEmpty: false } attributesData)
		{
			return;
		}

		var enabledAreaTypeSymbol = compilation.GetTypeByMetadataName("Sudoku.Concepts.Solving.EnabledArea")!;
		var disabledReasonTypeSymbol = compilation.GetTypeByMetadataName("Sudoku.Concepts.Solving.DisabledReason")!;

		foreach (var fieldSymbol in enabledAreaTypeSymbol.GetMembers().OfType<IFieldSymbol>())
		{
			_enabledAreasFields.Add((byte)fieldSymbol.ConstantValue!, fieldSymbol.Name);
		}
		foreach (var fieldSymbol in disabledReasonTypeSymbol.GetMembers().OfType<IFieldSymbol>())
		{
			_disabledReasonFields.Add((short)fieldSymbol.ConstantValue!, fieldSymbol.Name);
		}

		// Gather the valid attributes data.
		var foundAttributesData = new List<(
			INamedTypeSymbol StepSearcherSymbol, INamespaceSymbol ContainingNamespace,
			int Priority, byte DisplayingLevel, string StepSearcherName,
			ImmutableArray<KeyValuePair<string, TypedConstant>> NamedArguments)>();
		const string comma = ", ";
		const string attributeTypeName = "Sudoku.Solving.Manual.SearcherConfigurationAttribute<>";
		foreach (var attributeData in attributesData)
		{
			// Check validity.
			if (
#pragma warning disable IDE0055
				attributeData is not
				{
					AttributeClass:
					{
						IsGenericType: true,
						TypeArguments: [
							INamedTypeSymbol
							{
								IsRecord: false,
								ContainingNamespace: var containingNamespace,
								Name: var stepSearcherName
							} stepSearcherTypeSymbol
						]
					} attributeClassSymbol,
					ConstructorArguments: [
						{ Type.SpecialType: SpecialType.System_Int32, Value: int priority },
						{ Type.TypeKind: TypeKind.Enum, Value: byte dl }
					],
					NamedArguments: var namedArguments
				}
#pragma warning restore IDE0055
			)
			{
				continue;
			}

			// Checks whether the type is valid.
			var unboundAttributeTypeSymbol = attributeClassSymbol.ConstructUnboundGenericType();
			if (unboundAttributeTypeSymbol.ToDisplayString(TypeFormats.FullName) != attributeTypeName)
			{
				continue;
			}

			// Adds the necessary info into the collection.
			foundAttributesData.Add(
				(stepSearcherTypeSymbol, containingNamespace, priority, dl, stepSearcherName, namedArguments));
		}

#if false
		// Checks whether the collection has duplicated priority values.
		for (int i = 0; i < foundAttributesData.Count - 1; i++)
		{
			int a = foundAttributesData[i].Priority;
			for (int j = i + 1; j < foundAttributesData.Count; j++)
			{
				int b = foundAttributesData[j].Priority;
				if (a == b)
				{
					throw new InvalidOperationException(
						"Cannot operate because two found step searchers has a same priority value.");
				}
			}
		}
#endif

		// Iterate on each valid attribute data, and checks the inner value to be used
		// by the source generator to output.
		foreach (var (type, @namespace, priority, level, name, namedArguments) in foundAttributesData)
		{
			// Checks whether the attribute has configured any extra options.
			byte? enabledArea = null;
			short? disabledReason = null;
			if (namedArguments is not [])
			{
				foreach (var kvp in namedArguments)
				{
					switch (kvp)
					{
						case ("EnabledArea", { Value: byte ea }):
						{
							enabledArea = ea;
							break;
						}
						case ("DisabledReason", { Value: short dr }):
						{
							disabledReason = dr;
							break;
						}
					}
				}
			}

			// Gather the extra options on step searcher.
			StringBuilder? sb = null;
			if (enabledArea is not null || disabledReason is not null)
			{
				sb = new StringBuilder().Append(comma);
				if (enabledArea is { } ea)
				{
					string targetStr = CreateExpression(ea, "EnabledArea");
					sb.Append($"EnabledArea: {targetStr}{comma}");
				}
				if (disabledReason is { } dr)
				{
					string targetStr = CreateExpression(dr, "DisabledReason");
					sb.Append($"DisabledReason: {targetStr}{comma}");
				}

				sb.Remove(sb.Length - comma.Length, comma.Length);
			}

			// Output the generated code.
			spc.AddSource(
				$"{name}.g.sso.cs",
				$$"""
				namespace {{@namespace.ToDisplayString(TypeFormats.FullName)}};
				
				partial class {{name}}
				{
					/// <inheritdoc/>
					[global::{{typeof(GeneratedCodeAttribute).FullName}}("{{typeof(StepSearcherOptionsGenerator).FullName}}", "{{VersionValue}}")]
					[global::{{typeof(CompilerGeneratedAttribute).FullName}}]
					public global::Sudoku.Concepts.Solving.SearchingOptions Options { get; set; } =
						new({{priority}}, global::Sudoku.Concepts.Solving.DisplayingLevel.{{(char)(level + 'A' - 1)}}{{sb}});
				}
				"""
			);
		}
	}

	private unsafe string CreateExpression<T>(T field, string typeName) where T : unmanaged
	{
		long l = sizeof(T) switch
		{
			1 or 2 or 4 => Unsafe.As<T, int>(ref field),
			8 => Unsafe.As<T, long>(ref field),
			_ => default
		};

		// Special case: If the value is zero, just get the default field in the enumeration field
		// or just get the expression '(T)0' as the emitted code.
		if (l == 0)
		{
			return $"(global::Sudoku.Concepts.Solving.{typeName})0";
		}

		var targetList = new List<string>();
		for (var (temp, i) = (l, 0); temp != 0; temp >>= 1, i++)
		{
			if ((temp & 1) == 0)
			{
				continue;
			}

			switch (typeName)
			{
				case "EnabledArea" when _enabledAreasFields[(byte)(1 << i)] is var fieldValue:
				{
					targetList.Add($"global::Sudoku.Concepts.Solving.EnabledArea.{fieldValue}");

					break;
				}
				case "DisabledReason" when _disabledReasonFields[(short)(1 << i)] is var fieldValue:
				{
					targetList.Add($"global::Sudoku.Concepts.Solving.DisabledReason.{fieldValue}");

					break;
				}
			}
		}

		return string.Join(" | ", targetList);
	}
}
