namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines a source generator that generates the source code for the searcher options
/// on a step searcher.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class StepSearcherOptionsGenerator : ISourceGenerator
{
	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		if (
			context is not
			{
				Compilation.Assembly: { Name: "Sudoku.Solving" } assemblySymbol
			}
		)
		{
			return;
		}

		if (assemblySymbol.GetAttributes() is not { IsDefaultOrEmpty: false } attributesData)
		{
			return;
		}

		const string comma = ", ";
		const string attributeTypeName = "Sudoku.Solving.Manual.SearcherInitializerOptionAttribute<>";
		foreach (var attributeData in attributesData)
		{
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
					ConstructorArguments: [{ Value: { } priority }, { Value: { } displayingLevel }],
					NamedArguments: var namedArguments
				}
#pragma warning restore IDE0055
			)
			{
				continue;
			}

			var unboundAttributeTypeSymbol = attributeClassSymbol.ConstructUnboundGenericType();
			if (unboundAttributeTypeSymbol.ToDisplayString(TypeFormats.FullName) != attributeTypeName)
			{
				continue;
			}

			object? enabledAreas = null, disabledReason = null;
			if (namedArguments is { IsDefaultOrEmpty: false, Length: not 0 })
			{
				foreach (var (name, value) in namedArguments)
				{
					(name == nameof(EnabledAreas) ? ref enabledAreas : ref disabledReason) = value.Value;
				}
			}

			StringBuilder? sb = null;
			if (enabledAreas is not null || disabledReason is not null)
			{
				sb = new StringBuilder().Append(comma);
				if (enabledAreas is not null)
				{
					sb.Append($"{nameof(EnabledAreas)}: (global::{typeof(EnabledAreas).FullName}){enabledAreas}{comma}");
				}
				if (disabledReason is not null)
				{
					sb.Append($"{nameof(DisabledReason)}: (global::{typeof(DisabledReason).FullName}){disabledReason}{comma}");
				}

				sb.Remove(sb.Length - comma.Length, comma.Length);
			}

			context.AddSource(
				stepSearcherTypeSymbol.ToFileName(),
				"sso",
				$$"""
				namespace {{containingNamespace.ToDisplayString(TypeFormats.FullName)}};
				
				partial class {{stepSearcherName}}
				{
					/// <inheritdoc/>
					[global::{{typeof(GeneratedCodeAttribute).FullName}}("{{typeof(StepSearcherOptionsGenerator).FullName}}", "{{VersionValue}}")]
					[global::{{typeof(CompilerGeneratedAttribute).FullName}}]
					public global::{{typeof(SearchingOptions).FullName}} Options { get; set; } =
						new({{priority}}, (global::{{typeof(DisplayingLevel).FullName}}){{displayingLevel}}{{sb}});
				}
				"""
			);
		}
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context)
	{
	}
}
