namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines a source generator that generates the source code for overloading on comparison operators
/// <c><![CDATA[>]]></c>, <c><![CDATA[<]]></c>, <c><![CDATA[>=]]></c> and <c><![CDATA[<=]]></c>.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed partial class AutoOverloadsComparisonOperatorsGenerator : ISourceGenerator
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

			string inKeyword = attributeData.GetNamedArgument<bool>("EmitsInKeyword") ? "in " : string.Empty;
			string fullName = type.ToDisplayString(TypeFormats.FullName);
			context.AddSource(
				type.ToFileName(),
				Shortcuts.AutoOverloadsComparisonOperators,
				$$"""
				#nullable enable
				
				namespace {{namespaceName}};
				
				partial {{type.GetTypeKindModifier()}} {{type.Name}}{{genericParameterList}}
				{
					/// <summary>
					/// Determines whether the <paramref name="left"/>-side instance is less than
					/// the <paramref name="right"/>-side one.
					/// </summary>
					/// <param name="left">The left-side instance to be compared.</param>
					/// <param name="right">The right-side instance to be compared.</param>
					/// <returns>A <see cref="bool"/> result indicating that.</returns>
					[global::System.Runtime.CompilerServices.CompilerGenerated]
					[global::System.CodeDom.Compiler.GeneratedCode("{{GetType().FullName}}", "{{VersionValue}}")]
					[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
					public static bool operator <({{inKeyword}}{{fullName}} left, {{inKeyword}}{{fullName}} right)
						=> left.CompareTo(right) < 0;
					
					/// <summary>
					/// Determines whether the <paramref name="left"/>-side instance is less than
					/// the <paramref name="right"/>-side one, or they are considered equal.
					/// </summary>
					/// <param name="left">The left-side instance to be compared.</param>
					/// <param name="right">The right-side instance to be compared.</param>
					/// <returns>A <see cref="bool"/> result indicating that.</returns>
					[global::System.Runtime.CompilerServices.CompilerGenerated]
					[global::System.CodeDom.Compiler.GeneratedCode("{{GetType().FullName}}", "{{VersionValue}}")]
					[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
					public static bool operator <=({{inKeyword}}{{fullName}} left, {{inKeyword}}{{fullName}} right)
						=> left.CompareTo(right) <= 0;
					
					/// <summary>
					/// Determines whether the <paramref name="left"/>-side instance is greater than
					/// the <paramref name="right"/>-side one.
					/// </summary>
					/// <param name="left">The left-side instance to be compared.</param>
					/// <param name="right">The right-side instance to be compared.</param>
					/// <returns>A <see cref="bool"/> result indicating that.</returns>
					[global::System.Runtime.CompilerServices.CompilerGenerated]
					[global::System.CodeDom.Compiler.GeneratedCode("{{GetType().FullName}}", "{{VersionValue}}")]
					[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
					public static bool operator >({{inKeyword}}{{fullName}} left, {{inKeyword}}{{fullName}} right)
						=> left.CompareTo(right) > 0;
					
					/// <summary>
					/// Determines whether the <paramref name="left"/>-side instance is greater than
					/// the <paramref name="right"/>-side one, or they are considered equal.
					/// </summary>
					/// <param name="left">The left-side instance to be compared.</param>
					/// <param name="right">The right-side instance to be compared.</param>
					/// <returns>A <see cref="bool"/> result indicating that.</returns>
					[global::System.Runtime.CompilerServices.CompilerGenerated]
					[global::System.CodeDom.Compiler.GeneratedCode("{{GetType().FullName}}", "{{VersionValue}}")]
					[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
					public static bool operator >=({{inKeyword}}{{fullName}} left, {{inKeyword}}{{fullName}} right)
						=> left.CompareTo(right) >= 0;
				}
				"""
			);
		}
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context)
		=> context.RegisterForSyntaxNotifications(() => new Receiver(context.CancellationToken));
}
