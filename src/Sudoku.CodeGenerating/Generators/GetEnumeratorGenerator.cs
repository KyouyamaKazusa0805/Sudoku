namespace Sudoku.CodeGenerating.Generators;

/// <summary>
/// Indicates a source generator that generates the code for the method <c>GetEnumerator</c>.
/// </summary>
[Generator]
public sealed partial class GetEnumeratorGenerator : ISourceGenerator
{
	/// <summary>
	/// All possible replacements.
	/// </summary>
	private static readonly (string, Func<string?, string>)[] Replacements = new (string, Func<string?, string>)[]
	{
		("@", static memberName => memberName is null or "@" ? "this" : memberName),
		("*", static _ => "GetEnumerator()")
	};


	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		var receiver = (SyntaxReceiver)context.SyntaxReceiver!;
		var compilation = context.Compilation;
		var attributeSymbol = compilation.GetTypeByMetadataName(typeof(AutoGetEnumeratorAttribute).FullName);
		var ienumerableTypeSymbol = compilation.GetTypeByMetadataName(typeof(IEnumerable).FullName);
		var ienumerableGenericTypeSymbol = compilation.GetTypeByMetadataName(typeof(IEnumerable<>).FullName)!.ConstructUnboundGenericType();
		foreach (var (typeSymbol, memberName, memberConversion, extraNamespaces, returnType) in
			from type in receiver.Candidates
			let model = compilation.GetSemanticModel(type.SyntaxTree)
			select model.GetDeclaredSymbol(type, context.CancellationToken)! into typeSymbol
			let attributesData = typeSymbol.GetAttributes()
			let attributeData = attributesData.FirstOrDefault(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeSymbol))
			where attributeData is not null
			let mn = (string)attributeData.ConstructorArguments[0].Value!
			let mc = attributeData.TryGetNamedArgument(nameof(AutoGetEnumeratorAttribute.MemberConversion), out var result) ? (string)result.Value! : "@"
			let en = attributeData.TryGetNamedArgument(nameof(AutoGetEnumeratorAttribute.ExtraNamespaces), out var result) ? from arg in result.Values select (string?)arg.Value : null
			let rt = attributeData.TryGetNamedArgument(nameof(AutoGetEnumeratorAttribute.ReturnType), out var result) ? (INamedTypeSymbol)result.Value! : null
			select (typeSymbol, mn, mc, en, rt))
		{
			typeSymbol.DeconstructInfo(
				false, out string fullTypeName, out string namespaceName, out string genericParameterList,
				out _, out string typeKind, out string readonlyKeyword, out _
			);

			var ienumerableGeneric = (
				from @interface in typeSymbol.AllInterfaces
				where @interface.IsGenericType
				let unboundType = @interface.ConstructUnboundGenericType()
				where SymbolEqualityComparer.Default.Equals(unboundType, ienumerableGenericTypeSymbol)
				select @interface
			).FirstOrDefault()?.TypeArguments[0];
			string returnTypeStr = ienumerableGeneric is not null
				? $"System.Collections.Generic.IEnumerator<{ienumerableGeneric}>"
				: returnType.ToString();
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
	{readonlyKeyword}global::System.Collections.IEnumerator global::System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();";

			context.AddSource(
				typeSymbol.ToFileName(),
				GeneratedFileShortcuts.GetEnumeratorMethod,
				$@"{extraNamespacesStr}#nullable enable

namespace {namespaceName};

partial {typeKind}{typeSymbol.Name}{genericParameterList}
{{
	/// <inheritdoc cref=""object.GetHashCode"" />
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public {readonlyKeyword}{returnTypeStr} GetEnumerator() => {memberConversionStr};{interfaceExplicitlyImplementation}
}}
"
			);
		}
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context) => context.FastRegister<SyntaxReceiver>();
}
