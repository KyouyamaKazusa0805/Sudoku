namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines a source generator that generates the source code for the searcher options
/// on a step searcher.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed partial class StepSearcherOptionsGenerator : ISourceGenerator
{
	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		// Checks whether the reference project is valid.
		if (context is not { Compilation.Assembly: { Name: "Sudoku.Solving.Manual" } assemblySymbol })
		{
			return;
		}

		// Checks whether the assembly has marked any attributes.
		if (assemblySymbol.GetAttributes() is not { IsDefaultOrEmpty: false } attributesData)
		{
			return;
		}

		// Gather the valid attributes data.
		var foundAttributesData = new List<Tuple>();
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
					NamedArguments: var namedArguments,
					ApplicationSyntaxReference: { SyntaxTree: var syntaxTree, Span: var span }
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

			var location = Location.Create(syntaxTree, span);

			// Adds the necessary info into the collection.
			foundAttributesData.Add(
				new(
					stepSearcherTypeSymbol, containingNamespace, priority, (DisplayingLevel)dl,
					stepSearcherName, namedArguments, location));
		}

		// Checks whether the collection has duplicated priority values.
		for (int i = 0; i < foundAttributesData.Count - 1; i++)
		{
			int a = foundAttributesData[i].Priority;
			for (int j = i + 1; j < foundAttributesData.Count; j++)
			{
				int b = foundAttributesData[j].Priority;
				if (a == b)
				{
					throw new InvalidOperationException("Cannot operate because two found step searchers has a same priority value.");
				}
			}
		}

		// Iterate on each valid attribute data, and checks the inner value to be used
		// by the source generator to output.
		foreach (var (type, @namespace, priority, level, name, namedArguments, _) in foundAttributesData)
		{
			// Checks whether the attribute has configured any extra options.
			EnabledAreas? enabledAreas = null;
			DisabledReason? disabledReason = null;
			if (namedArguments is { IsDefaultOrEmpty: false, Length: not 0 })
			{
				foreach (var (argName, value) in namedArguments)
				{
					if (argName == nameof(EnabledAreas) && value.Value is byte ea)
					{
						enabledAreas = (EnabledAreas)ea;
					}
					if (argName == nameof(DisabledReason) && value.Value is short dr)
					{
						disabledReason = (DisabledReason)dr;
					}
				}
			}

			// Gather the extra options on step searcher.
			StringBuilder? sb = null;
			if (enabledAreas is not null || disabledReason is not null)
			{
				sb = new StringBuilder().Append(comma);
				if (enabledAreas is { } ea)
				{
					string targetStr = f(ea);
					sb.Append($"{nameof(EnabledAreas)}: {targetStr}{comma}");
				}
				if (disabledReason is { } dr)
				{
					string targetStr = f(dr);
					sb.Append($"{nameof(DisabledReason)}: {targetStr}{comma}");
				}

				sb.Remove(sb.Length - comma.Length, comma.Length);
			}

			// Output the generated code.
			context.AddSource(
				type.ToFileName(),
				"sso",
				$$"""
				namespace {{@namespace.ToDisplayString(TypeFormats.FullName)}};
				
				partial class {{name}}
				{
					/// <inheritdoc/>
					[global::{{typeof(GeneratedCodeAttribute).FullName}}("{{typeof(StepSearcherOptionsGenerator).FullName}}", "{{VersionValue}}")]
					[global::{{typeof(CompilerGeneratedAttribute).FullName}}]
					public global::{{typeof(SearchingOptions).FullName}} Options { get; set; } =
						new({{priority}}, global::{{typeof(DisplayingLevel).FullName}}.{{level}}{{sb}});
				}
				"""
			);
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static string f<TEnum>(TEnum field) where TEnum : Enum
			=> string.Join(
				" | ",
				from e in field.ToString().Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
				select $"global::{typeof(TEnum).FullName}.{e}"
			);
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context)
	{
	}
}
