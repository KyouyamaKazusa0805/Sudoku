namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Provides a source generator that generates the source code for extension deconstruction methods
/// with expressions.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class AutoLambdaedExtensionDeconstruct : ISourceGenerator
{
	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		var compilation = context.Compilation;
		var attributeSymbol = compilation
			.GetTypeByMetadataName(typeof(AutoDeconstructExtensionLambdaAttribute<,>).FullName)!
			.ConstructUnboundGenericType();

		foreach (var groupedResult in
			from attributeData in compilation.Assembly.GetAttributes()
			let a = attributeData.AttributeClass
			where a.IsGenericType
			let unboundAttribute = a.ConstructUnboundGenericType()
			where SymbolEqualityComparer.Default.Equals(unboundAttribute, attributeSymbol)
			let typeArgs = a.TypeArguments
			where !typeArgs.IsDefaultOrEmpty
			let firstTypeArg = typeArgs[0]
			let firstTypeArgConverted = firstTypeArg as INamedTypeSymbol
			where firstTypeArgConverted is not null
			let secondTypeArg = typeArgs[1]
			let secondTypeArgConverted = secondTypeArg as INamedTypeSymbol
			where secondTypeArgConverted is not null
			let typeArgStr = firstTypeArgConverted.ToDisplayString(TypeFormats.FullName)
			let arguments = attributeData.ConstructorArguments
			where !arguments.IsDefaultOrEmpty
			let argStrs = from arg in arguments[0].Values select ((string)arg.Value!).Trim()
			let n = attributeData.TryGetNamedArgument(nameof(AutoDeconstructExtensionLambdaAttribute<object, object>.Namespace), out var na) ? ((string)na.Value!).Trim() : null
			group (Type: firstTypeArgConverted, ProviderType: secondTypeArgConverted, Members: argStrs, Namespace: n) by typeArgStr)
		{
			var (typeArg, _, _, n) = groupedResult.First();
			string namespaceResult = n ?? typeArg.ContainingNamespace.ToDisplayString();
			string typeResult = typeArg.Name;
			string deconstructionMethodsCode = string.Join("\r\n\r\n\t", q());
			context.AddSource(
				typeArg.ToFileName(),
				GeneratedFileShortcuts.LambdaedExtensionDeconstructionMethod,
				$@"#nullable enable

namespace {namespaceResult};

/// <summary>
/// Provides the extension methods on this type.
/// </summary>
public static class {typeResult}_LambdaedDeconstructionMethods
{{
	{deconstructionMethodsCode}
}}
"
			);


			IEnumerable<string> q()
			{
				foreach (var (type, providerType, arguments, namedArgumentNamespace) in groupedResult)
				{
					var (
						_, _, _, _, genericParameterListWithoutConstraint, _, _, inKeyword, _, _
					) = SymbolOutputInfo.FromSymbol(type);
					string fullTypeNameWithoutConstraint = type.ToDisplayString(TypeFormats.FullNameWithConstraints);
					string constraint = fullTypeNameWithoutConstraint.IndexOf("where") is var index and not -1
						? fullTypeNameWithoutConstraint.Substring(index)
						: string.Empty;
					string parameterList = string.Join(
						", ",
						from member in arguments
						let dotIndex = member.IndexOf('.')
						let realMember = dotIndex != -1 ? member.Substring(dotIndex + 1) : member
						let memberType = (
							dotIndex != -1
								? providerType.GetAllMembers().OfType<IMethodSymbol>()
								: type.GetAllMembers()
						).FirstOrDefault(m => m.Name == realMember)?.GetMemberType()
						where memberType is not null
						select $@"out {memberType} {realMember.ToCamelCase()}"
					);
					string assignments = string.Join(
						"\r\n\t\t",
						from member in arguments
						let dotIndex = member.IndexOf('.')
						let providerMethodName = dotIndex != -1 ? member.Substring(dotIndex + 1) : null
						let realMember = dotIndex != -1 ? member.Substring(dotIndex + 1) : member
						let realArgument = (dotIndex != -1 ? member.Substring(dotIndex + 1) : member).ToCamelCase()
						let methodSymbol = providerMethodName is null ? null : (
							from methodSymbol in providerType.GetMembers().OfType<IMethodSymbol>()
							where methodSymbol.Name == providerMethodName
							select methodSymbol
						).FirstOrDefault()
						let firstArgName = methodSymbol?.Parameters[0].Name
						let rawExpression = methodSymbol is null
							? null
							: getExpression(methodSymbol, context.CancellationToken)
						let expression = rawExpression is null || firstArgName is null
							? null
							: firstArgName == "this"
								? rawExpression
								: rawExpression?.Replace(firstArgName, "@this")
						select $"{realArgument} = {expression ?? $"@this.{realMember}"};"
					);
					yield return $@"/// <summary>
	/// Deconstruct the instance to multiple elements.
	/// </summary>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public static void Deconstruct{genericParameterListWithoutConstraint}(this {inKeyword}{fullTypeNameWithoutConstraint} @this, {parameterList}){constraint}
	{{
		{assignments}
	}}";
				}
			}
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static string? getExpression(IMethodSymbol methodSymbol, CancellationToken ct) =>
			methodSymbol.Locations[0] switch
			{
				{ SourceTree: { } st } l =>
					(MethodDeclarationSyntax?)st.GetRoot(ct)?.FindNode(l.SourceSpan) switch
					{
						// public int Method() => value;
						{ ExpressionBody.Expression: var expressionNode } => expressionNode.ToString(),

						// public int Method() { return value; }
						{ Body.Statements: [ReturnStatementSyntax { Expression: { } expressionNode }] } =>
							expressionNode.ToString(),

						_ => null
					},

				_ => null
			};
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context)
	{
	}
}
