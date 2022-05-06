namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines a source generator that can generate the source code
/// that implements the interface type <c><![CDATA[IDefaultable<T>]]></c>.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed partial class AutoImplementsDefaultableGenerator : ISourceGenerator
{
	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		if (context is not { SyntaxContextReceiver: Receiver { Collection: var collection } })
		{
			return;
		}

		foreach (var (type, attributeData) in collection)
		{
			if (attributeData is not { ConstructorArguments: [{ Value: string defaultFieldName }] })
			{
				continue;
			}

			var (_, _, namespaceName, genericParameterList, _, _, readOnlyKeyword, _, _, _) =
				SymbolOutputInfo.FromSymbol(type);

			string fullName = type.ToDisplayString(TypeFormats.FullName);
			string? assignment = attributeData.GetNamedArgument<string>("Pattern") is { } patternValue
				? $" = {patternValue}"
				: string.Empty;
			string isDefaultExpression = attributeData.GetNamedArgument<string>("IsDefaultExpression")
				?? $"this == {defaultFieldName}";
			string defaultFieldDescription = attributeData.GetNamedArgument<string>("DefaultFieldDescription")
				?? """<inheritdoc cref="global::System.IDefaultable{T}.Default" />""";

			context.AddSource(
				type.ToFileName(),
				Shortcuts.AutoImplementsDefaultable,
				$$"""
				#nullable enable
				
				namespace {{namespaceName}};
				
				partial {{type.GetTypeKindModifier()}} {{type.Name}}{{genericParameterList}}
				{
					/// <summary>
					/// {{defaultFieldDescription}}
					/// </summary>
					public static readonly {{fullName}} {{defaultFieldName}}{{assignment}};
					
					
					/// <inheritdoc/>
					{{readOnlyKeyword}}bool IDefaultable<{{fullName}}>.IsDefault => {{isDefaultExpression}};
					
					/// <inheritdoc/>
					static {{fullName}} IDefaultable<{{fullName}}>.Default => {{defaultFieldName}};
				}
				"""
			);
		}
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context)
		=> context.RegisterForSyntaxNotifications(() => new Receiver(context.CancellationToken));
}
