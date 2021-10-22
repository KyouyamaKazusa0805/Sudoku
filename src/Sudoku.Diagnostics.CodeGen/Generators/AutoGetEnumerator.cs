namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines the source generator that generates the source code for <c>GetEnumerator</c> methods.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class AutoGetEnumerator : ISourceGenerator
{
	/// <summary>
	/// All possible replacements.
	/// </summary>
	private static readonly (string, Func<string?, string>)[] Replacements = new (string, Func<string?, string>)[]
	{
		("@", static memberName => memberName is null or "@" ? "this" : memberName),
		("*", static _ => "GetEnumerator()")
	};


	/// <summary>
	/// The result collection.
	/// </summary>
	private readonly ICollection<(INamedTypeSymbol Symbol, AttributeData AttributeData)> _resultCollection =
		new List<(INamedTypeSymbol, AttributeData)>();


	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		var compilation = context.Compilation;
		var attributeSymbol = compilation.GetTypeByMetadataName(typeof(AutoGetEnumeratorAttribute).FullName);
		var ienumerableTypeSymbol = compilation.GetTypeByMetadataName(typeof(IEnumerable).FullName);
		var ienumerableGenericTypeSymbol = compilation.GetTypeByMetadataName(typeof(IEnumerable<>).FullName)!.ConstructUnboundGenericType();
		foreach (var (typeSymbol, attributeData) in _resultCollection)
		{
			var (
				typeName, fullTypeName, namespaceName, genericParameterList, genericParameterListWithoutConstraint,
				typeKind, readOnlyKeyword, inKeyword, nullableAnnotation, _
			) = SymbolOutputInfo.FromSymbol(typeSymbol);

			string memberName = (string)attributeData.ConstructorArguments[0].Value!;
			string memberConversion = attributeData.TryGetNamedArgument(nameof(AutoGetEnumeratorAttribute.MemberConversion), out var mnResult) ? (string)mnResult.Value! : "@";
			var extraNamespaces = attributeData.TryGetNamedArgument(nameof(AutoGetEnumeratorAttribute.ExtraNamespaces), out var enResult) ? from arg in enResult.Values select (string?)arg.Value : null;
			var returnType = attributeData.TryGetNamedArgument(nameof(AutoGetEnumeratorAttribute.ReturnType), out var rtResult) ? (INamedTypeSymbol)rtResult.Value! : null;

			var ienumerableGeneric = (
				from @interface in typeSymbol.AllInterfaces
				where @interface.IsGenericType
				let unboundType = @interface.ConstructUnboundGenericType()
				where SymbolEqualityComparer.Default.Equals(unboundType, ienumerableGenericTypeSymbol)
				select @interface
			).FirstOrDefault()?.TypeArguments[0];
			string? returnTypeStr = ienumerableGeneric is not null
				? $"System.Collections.Generic.IEnumerator<{ienumerableGeneric}>"
				: returnType?.ToString();
			string memberConversionStr = memberConversion;
			foreach (var (key, func) in Replacements)
			{
				memberConversionStr = memberConversionStr.Replace(key, func(memberName));
			}

			string extraNamespacesStr = (extraNamespaces?.Any() ?? false) && string.Join("\r\n", from extraNamespace in extraNamespaces select $"using {extraNamespace};") is var code
				? $"{code}\r\n\r\n"
				: string.Empty;
			bool implementsIEnumerableNongeneric = typeSymbol.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, ienumerableTypeSymbol));
			string interfaceExplicitlyImplementation = typeSymbol.IsRefLikeType || !implementsIEnumerableNongeneric ? string.Empty : $@"

	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	{readOnlyKeyword}global::System.Collections.IEnumerator global::System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();";

			context.AddSource(
				typeSymbol.ToFileName(),
				GeneratedFileShortcuts.GetEnumeratorMethod,
				$@"{extraNamespacesStr}#nullable enable

namespace {namespaceName};

partial {typeKind}{typeSymbol.Name}{genericParameterList}
{{
	/// <summary>
	/// Returns an enumerator that iterates through the collection.
	/// </summary>
	/// <returns>An enumerator that can be used to iterate through the collection.</returns>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public {readOnlyKeyword}{returnTypeStr} GetEnumerator() => {memberConversionStr};{interfaceExplicitlyImplementation}
}}
"
			);
		}
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context) =>
		context.RegisterForSyntaxNotifications(
			() => SyntaxContextReceiverCreator.Create(
				(syntaxNode, semanticModel) =>
				{
					if (
						(
							SyntaxNode: syntaxNode,
							SemanticModel: semanticModel,
							Context: context
						) is not (
							SyntaxNode: TypeDeclarationSyntax { AttributeLists.Count: not 0 } n,
							SemanticModel: { Compilation: { } compilation },
							Context: { CancellationToken: var cancellationToken }
						)
					)
					{
						return;
					}

					if (semanticModel.GetDeclaredSymbol(n, cancellationToken) is not { } typeSymbol)
					{
						return;
					}

					var attribute = compilation.GetTypeByMetadataName(typeof(AutoGetEnumeratorAttribute).FullName)!;
					var attributesData = typeSymbol.GetAttributes();
					var attributeData = attributesData.FirstOrDefault(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attribute));
					if (attributeData is not { ConstructorArguments.IsDefaultOrEmpty: false })
					{
						return;
					}

					_resultCollection.Add((typeSymbol, attributeData));
				}
			)
		);
}
