using Tuple = System.ValueTuple<
	Microsoft.CodeAnalysis.INamedTypeSymbol,
	Microsoft.CodeAnalysis.INamespaceSymbol,
	int,
	Sudoku.Solving.Manual.DisplayingLevel,
	string,
	System.Collections.Immutable.ImmutableArray<
		System.Collections.Generic.KeyValuePair<
			string,
			Microsoft.CodeAnalysis.TypedConstant>>>;

namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines a source generator that generates the source code for the searcher options
/// on a step searcher.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class StepSearcherOptionsGenerator : ISourceGenerator
{
	private static readonly DiagnosticDescriptor SCA0101 = new(
		nameof(SCA0101),
		"The source generator can only applied to the assembly 'Sudoku.Solving'",
		"The source generator can only applied to the assembly 'Sudoku.Solving'",
		"Usage",
		DiagnosticSeverity.Error,
		true,
		helpLinkUri: "https://github.com/SunnieShine/Sudoku/tree/main/docs/code-analysis/sca0101"
	);

	private static readonly DiagnosticDescriptor SCA0102 = new(
		nameof(SCA0102),
		"Duplicated priority value found",
		"Duplicated priority value found",
		"Design",
		DiagnosticSeverity.Error,
		true,
		helpLinkUri: "https://github.com/SunnieShine/Sudoku/tree/main/docs/code-analysis/sca0102"
	);


	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		// Checks whether the reference project is valid.
		if (
			context is not
			{
				Compilation.Assembly: { Name: "Sudoku.Solving" } assemblySymbol
			}
		)
		{
			context.ReportDiagnostic(Diagnostic.Create(SCA0101, null, messageArgs: null));
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
		const string attributeTypeName = "Sudoku.Solving.Manual.SearcherInitializerOptionAttribute<>";
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
				(
					stepSearcherTypeSymbol,
					containingNamespace,
					priority,
					(DisplayingLevel)dl,
					stepSearcherName,
					namedArguments
				)
			);
		}

		// Checks whether the collection has duplicated priority values.
		for (int i = 0; i < foundAttributesData.Count - 1; i++)
		{
			int a = foundAttributesData[i].Item3;
			for (int j = i + 1; j < foundAttributesData.Count; j++)
			{
				int b = foundAttributesData[j].Item3;
				if (a == b)
				{
					context.ReportDiagnostic(Diagnostic.Create(SCA0102, null, messageArgs: null));
				}
			}
		}

		// Iterate on each valid attribute data, and checks the inner value to be used
		// by the source generator to output.
		foreach (var (type, @namespace, priority, level, name, namedArguments) in foundAttributesData)
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
		static string f<TEnum>(TEnum field) where TEnum : Enum =>
			string.Join(
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
