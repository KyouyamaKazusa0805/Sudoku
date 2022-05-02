namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines a source generator that generates the code that overloads the <c><see langword="operator"/> ==</c>
/// or <c><see langword="operator"/> !=</c>.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed partial class AutoOverloadsEqualityOperatorsGenerator : ISourceGenerator
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
			var (_, _, namespaceName, genericParameterList, _, _, _, _, _, _) = SymbolOutputInfo.FromSymbol(type);

			var namedArgs = attributeData.NamedArguments;
			static bool inKeywordChecker(KeyValuePair<string, TypedConstant> n) => n.Key == "EmitInKeyword";
			static bool nullableAnnotationChecker(KeyValuePair<string, TypedConstant> n) => n.Key == "WithNullableAnnotation";
			string inKeyword = (bool?)namedArgs.FirstOrDefault(inKeywordChecker).Value.Value ?? false
				? "in "
				: string.Empty;
			string nullableAnnotation = (bool?)namedArgs.FirstOrDefault(nullableAnnotationChecker).Value.Value ?? false
				? "?"
				: string.Empty;
			string realComparisonExpression = (bool?)namedArgs.FirstOrDefault(nullableAnnotationChecker).Value.Value ?? false
				? "(left, right) switch { (null, null) => true, (not null, not null) => left.Equals(right), _ => false }"
				: "left.Equals(right)";

			string fullName = type.ToDisplayString(TypeFormats.FullName);
			context.AddSource(
				type.ToFileName(),
				"eq",
				$$"""
				#nullable enable
				
				namespace {{namespaceName}};
				
				partial {{type.GetTypeKindModifier()}} {{type.Name}}{{genericParameterList}}
				{
					/// <summary>
					/// Determines whether two instances hold a same value.
					/// </summary>
					/// <param name="left">The left-side instance to take part in the comparison operation.</param>
					/// <param name="right">The right-side instance to take part in the comparison operation.</param>
					/// <returns>A <see cref="bool"/> value indicating that.</returns>
					[global::System.Runtime.CompilerServices.CompilerGenerated]
					[global::System.CodeDom.Compiler.GeneratedCode("{{GetType().FullName}}", "{{VersionValue}}")]
					[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
					public static bool operator ==({{inKeyword}}{{fullName}}{{nullableAnnotation}} left, {{inKeyword}}{{fullName}}{{nullableAnnotation}} right)
						=> {{realComparisonExpression}};
					
					/// <summary>
					/// Determines whether two instances don't hold a same value.
					/// </summary>
					/// <param name="left">The left-side instance to take part in the comparison operation.</param>
					/// <param name="right">The right-side instance to take part in the comparison operation.</param>
					/// <returns>A <see cref="bool"/> value indicating that.</returns>
					[global::System.Runtime.CompilerServices.CompilerGenerated]
					[global::System.CodeDom.Compiler.GeneratedCode("{{GetType().FullName}}", "{{VersionValue}}")]
					[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
					public static bool operator !=({{inKeyword}}{{fullName}}{{nullableAnnotation}} left, {{inKeyword}}{{fullName}}{{nullableAnnotation}} right)
						=> !(left == right);
				}
				"""
			);
		}
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context)
		=> context.RegisterForSyntaxNotifications(() => new Receiver(context.CancellationToken));
}
