namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines a source generator that generates the source code on implementation for the type <see cref="IComparable{T}"/>.
/// </summary>
/// <seealso cref="IComparable{T}"/>
[Generator(LanguageNames.CSharp)]
public sealed partial class AutoImplementsComparableGenerator : ISourceGenerator
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
			var (_, _, namespaceName, genericParameterList, _, _, readOnlyKeyword, _, _, _) =
				SymbolOutputInfo.FromSymbol(type);

			string fullName = type.ToDisplayString(TypeFormats.FullName);
			var namedArgs = attributeData.NamedArguments;
			static bool isExplicitly(KeyValuePair<string, TypedConstant> n) => n.Key == "UseExplicitImplementation";
			string? memberName = (string?)attributeData.ConstructorArguments[0].Value;
			bool @explicit = (bool?)namedArgs.FirstOrDefault(isExplicitly).Value.Value ?? false;
			bool isStruct = type.TypeKind == TypeKind.Struct;
			string method = (@explicit, isStruct, memberName) switch
			{
				(false, true, not null)
					=> $"""
						public {readOnlyKeyword}int CompareTo({fullName} other) => {memberName}.CompareTo({memberName});
					""",
				(false, true, null)
					=> $"""
						public {readOnlyKeyword}int CompareTo({fullName} other) => CompareTo(other);
					""",
				(false, false, not null)
					=> $$"""
						public {{readOnlyKeyword}}int CompareTo([global::System.Diagnostic.CodeAnalysis.DisallowNull] {{fullName}}? other)
						{
							global::System.ArgumentNullException.ThrowIfNull(other);
						
							return {{memberName}}.CompareTo({{memberName}});
						}
					""",
				(false, false, null)
					=> $$"""
						public {{readOnlyKeyword}}int CompareTo([global::System.Diagnostic.CodeAnalysis.DisallowNull] {{fullName}}? other)
						{
							global::System.ArgumentNullException.ThrowIfNull(other);
						
							return CompareTo(other);
						}
					""",
				(true, true, not null)
					=> $$"""
						{{readOnlyKeyword}}int global::System.IComparable<{{fullName}}>.CompareTo({{fullName}} other)
							=> {{memberName}}.CompareTo({{memberName}});
					""",
				(true, true, null)
					=> $$"""
						{{readOnlyKeyword}}int global::System.IComparable<{{fullName}}>.CompareTo({{fullName}} other)
							=> CompareTo(other);
					""",
				(true, false, not null)
					=> $$"""
						{{readOnlyKeyword}}int global::System.IComparable<{{fullName}}>.CompareTo({{fullName}}? other)
						{
							global::System.ArgumentNullException.ThrowIfNull(other);
						
							return {{memberName}}.CompareTo({{memberName}});
						}
					""",
				(true, false, null)
					=> $$"""
						{{readOnlyKeyword}}int global::System.IComparable<{{fullName}}>.CompareTo({{fullName}}? other)
						{
							global::System.ArgumentNullException.ThrowIfNull(other);
						
							return CompareTo(other);
						}
					"""
			};

			context.AddSource(
				type.ToFileName(),
				"eq",
				$$"""
				#nullable enable
				
				namespace {{namespaceName}};
				
				partial {{type.GetTypeKindModifier()}} {{type.Name}}{{genericParameterList}}
				{
					/// <summary>
					/// Compares the current instance with another object of the same type and returns an integer
					/// that indicates whether the current instance precedes, follows, or occurs in the same position
					/// in the sort order as the other object.
					/// </summary>
					/// <param name="other">An object to compare with this instance.</param>
					/// <returns>
					/// A value that indicates the relative order of the objects being compared.
					/// The return value has these meanings:
					/// <list type="table">
					/// <listheader>
					/// <term>Value</term>
					/// <description>Meaning</description>
					/// </listheader>
					/// <item>
					/// <term>Less than zero</term>
					/// <description>This instance precedes <paramref name="other"/> in the sort order.</description>
					/// </item>
					/// <item>
					/// <term>Zero</term>
					/// <description>
					/// The instance occurs in the same position in the sort order as <paramref name="other"/>.
					/// </description>
					/// </item>
					/// <item>
					/// <term>Greater than zero</term>
					/// <description>The instance follows <paramref name="other"/> in the sort order.</description>
					/// </item>
					/// </list>
					/// </returns>
					[global::System.Runtime.CompilerServices.CompilerGenerated]
					[global::System.CodeDom.Compiler.GeneratedCode("{{GetType().FullName}}", "{{VersionValue}}")]
					[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
				{{method}}
				}
				"""
			);
		}
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context)
		=> context.RegisterForSyntaxNotifications(() => new Receiver(context.CancellationToken));
}
