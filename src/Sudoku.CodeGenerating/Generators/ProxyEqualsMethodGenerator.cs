using Sudoku.CodeGenerating.Extensions;

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
		Func<ISymbol?, ISymbol?, bool> f = SymbolEqualityComparer.Default.Equals;
		var processedList = new List<INamedTypeSymbol>();
		var compilation = context.Compilation;
		var attributeSymbol = compilation.GetTypeByMetadataName<ProxyEqualityAttribute>();
		foreach (var type in
			from candidate in receiver.Candidates
			let model = compilation.GetSemanticModel(candidate.SyntaxTree)
			select model.GetDeclaredSymbol(candidate)! into type
			from member in type.GetMembers().OfType<IMethodSymbol>()
			where member.GetAttributes().Any(a => f(a.AttributeClass, attributeSymbol))
			let boolSymbol = compilation.GetSpecialType(SpecialType.System_Boolean)
			let returnTypeSymbol = member.ReturnType
			where f(returnTypeSymbol, boolSymbol)
			let parameters = member.Parameters
			where parameters.Length == 2 && parameters.All(p => f(p.Type, type))
			select type)
		{
			if (processedList.Contains(type, SymbolEqualityComparer.Default))
			{
				continue;
			}

			var methods = (
				from member in type.GetMembers().OfType<IMethodSymbol>()
				from attribute in member.GetAttributes()
				where f(attribute.AttributeClass, attributeSymbol)
				select member
			).First();

			if (
				type.IsReferenceType
				&& !methods.Parameters.NullableMatches(NullableAnnotation.Annotated, NullableAnnotation.Annotated)
			)
			{
				continue;
			}

			type.DeconstructInfo(
				false, out string fullTypeName, out string namespaceName, out string genericParametersList,
				out string genericParametersListWithoutConstraint, out string typeKind,
				out string readonlyKeyword, out _
			);
			string methodName = methods.Name;
			string inModifier = type.MemberShouldAppendIn() ? "in " : string.Empty;
			string nullableMark = type.TypeKind == TypeKind.Class || type.IsRecord ? "?" : string.Empty;
			string objectEqualityMethod = type.IsRefLikeType
				? "// This type is a ref struct, so 'bool Equals(object?) is useless."
				: $@"[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public override {readonlyKeyword}bool Equals(object? obj) => obj is {type.Name}{genericParametersListWithoutConstraint} comparer && {methodName}(this, comparer);";

			context.AddSource(
				type.ToFileName(),
				"ProxyEquality",
				$@"#pragma warning disable 1591

#nullable enable

namespace {namespaceName};

partial {typeKind}{type.Name}{genericParametersList}
{{
	{objectEqualityMethod}

	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public bool Equals({inModifier}{type.Name}{genericParametersListWithoutConstraint}{nullableMark} other) => {methodName}(this, other);


	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public static bool operator ==({inModifier}{type.Name}{genericParametersListWithoutConstraint} left, {inModifier}{type.Name}{genericParametersListWithoutConstraint} right) => {methodName}(left, right);

	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public static bool operator !=({inModifier}{type.Name}{genericParametersListWithoutConstraint} left, {inModifier}{type.Name}{genericParametersListWithoutConstraint} right) => !(left == right);
}}
");

			processedList.Add(type);
		}
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context) => context.FastRegister<SyntaxReceiver>();
}
