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
					ConstructorArguments: [
						{
							Type.SpecialType: SpecialType.System_Int32,
							Value: int priority
						},
						{
							Type.TypeKind: TypeKind.Enum,
							Value: byte dl
						}
					],
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

			var displayingLevel = (DisplayingLevel)dl;
			EnabledAreas? enabledAreas = null;
			DisabledReason? disabledReason = null;
			if (namedArguments is { IsDefaultOrEmpty: false, Length: not 0 })
			{
				foreach (var (name, value) in namedArguments)
				{
					if (name == nameof(EnabledAreas) && value.Value is byte ea)
					{
						enabledAreas = (EnabledAreas)ea;
					}
					if (name == nameof(DisabledReason) && value.Value is short dr)
					{
						disabledReason = (DisabledReason)dr;
					}
				}
			}

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
						new({{priority}}, global::{{typeof(DisplayingLevel).FullName}}.{{displayingLevel}}{{sb}});
				}
				"""
			);


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static string f<TEnum>(TEnum field) where TEnum : Enum =>
				string.Join(
					" | ",
					from e in field.ToString().Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
					select $"global::{typeof(TEnum).FullName}.{e}"
				);
		}
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context)
	{
	}
}
