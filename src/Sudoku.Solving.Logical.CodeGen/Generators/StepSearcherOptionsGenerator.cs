﻿namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines a source generator that generates the source code for the searcher options
/// on a step searcher.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class StepSearcherOptionsGenerator : IIncrementalGenerator
{
	/// <summary>
	/// Indicates the property name of <c>EnabledArea</c>.
	/// </summary>
	private const string EnabledAreaPropertyName = "EnabledArea";

	/// <summary>
	/// Indicates the property name of <c>DisabledReason</c>.
	/// </summary>
	private const string DisabledReasonPropertyName = "DisabledReason";


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

		var stepSearcherInterfaceType = compilation.GetTypeByMetadataName("Sudoku.Solving.Logical.IStepSearcher");
		if (stepSearcherInterfaceType is null)
		{
			return;
		}

		var enabledAreaTypeSymbol = compilation.GetTypeByMetadataName("Sudoku.Runtime.AnalysisServices.SearcherEnabledArea")!;
		var disabledReasonTypeSymbol = compilation.GetTypeByMetadataName("Sudoku.Runtime.AnalysisServices.SearcherDisabledReason")!;

		var enabledAreasFields = new Dictionary<byte, string>();
		var disabledReasonFields = new Dictionary<short, string>();
		foreach (var fieldSymbol in enabledAreaTypeSymbol.GetMembers().OfType<IFieldSymbol>())
		{
			enabledAreasFields.Add((byte)fieldSymbol.ConstantValue!, fieldSymbol.Name);
		}
		foreach (var fieldSymbol in disabledReasonTypeSymbol.GetMembers().OfType<IFieldSymbol>())
		{
			disabledReasonFields.Add((short)fieldSymbol.ConstantValue!, fieldSymbol.Name);
		}

		// Gather the valid attributes data.
		var foundAttributesData = new List<Data>();
		const string comma = ", ";
		const string attributeTypeName = "global::Sudoku.Solving.Logical.Annotations.SearcherConfigurationAttribute<>";
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
								Interfaces: var stepSearcherImplementedInterfaces,
								BaseType: var baseType
							}
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

			// Adds the necessary info into the collection.
			foundAttributesData.Add(
				new(
					containingNamespace,
					priorityValue++,
					dl,
					stepSearcherName,
					stepSearcherImplementedInterfaces.First(namespaceChecker).Name,
					namedArguments,
					baseType switch { null or { SpecialType: SpecialType.System_Object } => false, _ => isNameConflict(baseType) }
				)
			);


			bool isNameConflict(INamedTypeSymbol baseType)
			{
				for (var tempType = baseType; tempType is not null; tempType = tempType.BaseType)
				{
					foreach (var tempTypeImpledInterface in tempType.AllInterfaces)
					{
						if (SymbolEqualityComparer.Default.Equals(stepSearcherInterfaceType, tempTypeImpledInterface))
						{
							return true;
						}
					}
				}

				return false;
			}
		}

		// Iterate on each valid attribute data, and checks the inner value to be used
		// by the source generator to output.
		var generatedCodeSnippets = new List<string>();
		var namespaceUsed = foundAttributesData[0].Namespace;
		foreach (var (_, priority, level, name, docCommentTypeName, namedArguments, nameConflict) in foundAttributesData)
		{
			// Checks whether the attribute has configured any extra options.
			var enabledArea = default(byte?);
			var disabledReason = default(short?);
			if (namedArguments is not [])
			{
				foreach (var kvp in namedArguments)
				{
					switch (kvp)
					{
						case (EnabledAreaPropertyName, { Value: byte ea }):
						{
							enabledArea = ea;
							break;
						}
						case (DisabledReasonPropertyName, { Value: short dr }):
						{
							disabledReason = dr;
							break;
						}
					}
				}
			}

			// Gather the extra options on step searcher.
			var sb = default(StringBuilder?);
			if (enabledArea is not null || disabledReason is not null)
			{
				sb = new StringBuilder().Append(comma);
				if (enabledArea is { } ea)
				{
					var targetStr = createExpression(ea, $"Searcher{EnabledAreaPropertyName}", enabledAreasFields, disabledReasonFields);
					sb.Append($"{EnabledAreaPropertyName}: {targetStr}{comma}");
				}
				if (disabledReason is { } dr)
				{
					var targetStr = createExpression(dr, $"Searcher{DisabledReasonPropertyName}", enabledAreasFields, disabledReasonFields);
					sb.Append($"{DisabledReasonPropertyName}: {targetStr}{comma}");
				}

				sb.Remove(sb.Length - comma.Length, comma.Length);
			}

			var newKeyword = nameConflict ? "new " : string.Empty;

			// Output the generated code.
			generatedCodeSnippets.Add(
				$$"""
				/// <inheritdoc cref="{{docCommentTypeName}}"/>
				partial class {{name}}
				{
					/// <inheritdoc/>
					[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{GetType().FullName}}", "{{VersionValue}}")]
					[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
					public {{newKeyword}}global::Sudoku.Runtime.AnalysisServices.SearcherInitializationOptions Options { get; set; } =
						new({{priority}}, global::Sudoku.Runtime.AnalysisServices.SearcherDisplayingLevel.{{(char)(level + 'A' - 1)}}{{sb}});
				}
				"""
			);
		}

		var selectedRange = "global::".Length..;
		spc.AddSource(
			$"StepSearcherConfig.g.{Shortcuts.StepSearcherOptions}.cs",
			$$"""
			// <auto-generated/>
			
			namespace {{namespaceUsed.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)[selectedRange]}};
			
			{{string.Join("\r\n\r\n", generatedCodeSnippets)}}
			"""
		);


		static unsafe string createExpression<T>(
			T field,
			string typeName,
			IDictionary<byte, string> enabledAreasFields,
			IDictionary<short, string> disabledReasonFields
		) where T : unmanaged
		{
			var l = sizeof(T) switch { 1 or 2 or 4 => As<T, int>(ref field), 8 => As<T, long>(ref field) };

			// Special case: If the value is zero, just get the default field in the enumeration field
			// or just get the expression '(T)0' as the emitted code.
			if (l == 0)
			{
				return $"(global::Sudoku.Runtime.AnalysisServices.{typeName})0";
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
					case $"Searcher{EnabledAreaPropertyName}"
					when enabledAreasFields[(byte)(1 << i)] is var fieldValue:
					{
						targetList.Add($"global::Sudoku.Runtime.AnalysisServices.Searcher{EnabledAreaPropertyName}.{fieldValue}");

						break;
					}
					case $"Searcher{DisabledReasonPropertyName}"
					when disabledReasonFields[(short)(1 << i)] is var fieldValue:
					{
						targetList.Add($"global::Sudoku.Runtime.AnalysisServices.Searcher{DisabledReasonPropertyName}.{fieldValue}");

						break;
					}
				}
			}

			return string.Join(" | ", targetList);
		}

		static bool namespaceChecker(INamedTypeSymbol i)
		{
			var namespaceName = i.ContainingNamespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
			return namespaceName[(namespaceName.LastIndexOf('.') + 1)..] != "Specialized";
		}
	}
}

/// <summary>
/// Simply encapsulates a data tuple describing the information of a found attribute.
/// </summary>
/// <param name="Namespace">Indicates the namespace symbol of that step searcher.</param>
/// <param name="PriorityValue">The priority value of the step searcher.</param>
/// <param name="DifficultyLevel">The difficulty level of the step searcher.</param>
/// <param name="TypeName">The name of the step searcher type.</param>
/// <param name="DocCommentInterfaceTypeName">
/// The name of the interface type that is used for displaying for the doc comment.
/// </param>
/// <param name="NamedArguments">The named arguments of that attribute.</param>
/// <param name="BaseTypeContainsSameNameProperty">Indicates whether the base types contains the same-name property.</param>
file readonly record struct Data(
	INamespaceSymbol Namespace,
	int PriorityValue,
	byte DifficultyLevel,
	string TypeName,
	string DocCommentInterfaceTypeName,
	ImmutableArray<KeyValuePair<string, TypedConstant>> NamedArguments,
	bool BaseTypeContainsSameNameProperty
);
