namespace Sudoku.CodeGenerating.Generators;

/// <summary>
/// Indicates a generator that generates the code about the equality method.
/// </summary>
[Generator]
public sealed partial class ProxyEqualsMethodGenerator : ISourceGenerator
{
	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		var receiver = (SyntaxReceiver)context.SyntaxReceiver!;
		var processedList = new List<INamedTypeSymbol>();
		var compilation = context.Compilation;
		var attributeSymbol = compilation.GetTypeByMetadataName<ProxyEqualityAttribute>();
		var boolSymbol = compilation.GetSpecialType(SpecialType.System_Boolean);
		foreach (var (typeSymbol, method) in
			from candidate in receiver.Candidates
			let model = compilation.GetSemanticModel(candidate.SyntaxTree)
			select model.GetDeclaredSymbol(candidate)! into typeSymbol
			from member in typeSymbol.GetMembers().OfType<IMethodSymbol>()
			let attributesData = member.GetAttributes()
			where attributesData.Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeSymbol))
				&& SymbolEqualityComparer.Default.Equals(member.ReturnType, boolSymbol)
			let parameters = member.Parameters
			where parameters.Length == 2
				&& parameters.All(p => SymbolEqualityComparer.Default.Equals(p.Type, typeSymbol))
				&& !processedList.Contains(typeSymbol, SymbolEqualityComparer.Default)
			let method = (
				from member in typeSymbol.GetMembers().OfType<IMethodSymbol>()
				from attribute in member.GetAttributes()
				where SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, attributeSymbol)
				select member
			).First()
			let methodParams = method.Parameters
			where !typeSymbol.IsReferenceType || methodParams.NullableMatches(NullableAnnotation.Annotated)
			select (typeSymbol, method))
		{
			typeSymbol.DeconstructInfo(
				false, out string fullTypeName, out string namespaceName, out string genericParametersList,
				out string genericParametersListWithoutConstraint, out string typeKind,
				out string readonlyKeyword, out _
			);
			string methodName = method.Name;
			string inModifier = typeSymbol.MemberShouldAppendIn() ? "in " : string.Empty;
			string nullableMark = typeSymbol.TypeKind == TypeKind.Class || typeSymbol.IsRecord ? "?" : string.Empty;
			string objectEqualityMethod = typeSymbol.IsRefLikeType
				? "// This type is a ref struct, so 'bool Equals(object?) is useless."
				: $@"/// <inheritdoc/>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public override {readonlyKeyword}bool Equals(object? obj) => obj is {typeSymbol.Name}{genericParametersListWithoutConstraint} comparer && {methodName}(this, comparer);";

			context.AddSource(
				typeSymbol.ToFileName(),
				GeneratedFileShortcuts.ProxyEqualsMethod,
				$@"#nullable enable

namespace {namespaceName};

partial {typeKind}{typeSymbol.Name}{genericParametersList}
{{
	{objectEqualityMethod}

	/// <inheritdoc cref=""IEquatable{{T}}.Equals(T)"" />
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public bool Equals({inModifier}{typeSymbol.Name}{genericParametersListWithoutConstraint}{nullableMark} other) => {methodName}(this, other);


	/// <summary>
	/// Determine whether two instances hold a same value.
	/// </summary>
	/// <param name=""left"">The left-side instance to compare.</param>
	/// <param name=""right"">The right-side instance to compare.</param>
	/// <returns>A <see cref=""bool""/> result.</returns>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public static bool operator ==({inModifier}{typeSymbol.Name}{genericParametersListWithoutConstraint} left, {inModifier}{typeSymbol.Name}{genericParametersListWithoutConstraint} right) => {methodName}(left, right);

	/// <summary>
	/// Determine whether two instances don't hold a same value.
	/// </summary>
	/// <param name=""left"">The left-side instance to compare.</param>
	/// <param name=""right"">The right-side instance to compare.</param>
	/// <returns>A <see cref=""bool""/> result.</returns>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public static bool operator !=({inModifier}{typeSymbol.Name}{genericParametersListWithoutConstraint} left, {inModifier}{typeSymbol.Name}{genericParametersListWithoutConstraint} right) => !(left == right);
}}
"
			);

			processedList.Add(typeSymbol);
		}
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context) => context.FastRegister<SyntaxReceiver>();
}
