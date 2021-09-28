namespace Sudoku.CodeGenerating.Generators;

/// <summary>
/// Indicates the generator that generates the code that overrides <see cref="object.GetHashCode"/>.
/// </summary>
/// <seealso cref="object.GetHashCode"/>
[Generator]
public sealed partial class AutoGetHashCodeGenerator : ISourceGenerator
{
	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		var receiver = (SyntaxReceiver)context.SyntaxReceiver!;
		var compilation = context.Compilation;
		var attributeSymbol = compilation.GetTypeByMetadataName<AutoHashCodeAttribute>();

		foreach (var (typeSymbol, attributeData) in
			from type in receiver.Candidates
			let semanticModel = compilation.GetSemanticModel(type.SyntaxTree)
			select semanticModel.GetDeclaredSymbol(type, context.CancellationToken)! into typeSymbol
			let attributesData = typeSymbol.GetAttributes()
			let attributeData = attributesData.FirstOrDefault(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeSymbol))
			where attributeData is { ConstructorArguments.IsDefaultOrEmpty: false }
			select (typeSymbol, attributeData))
		{
			typeSymbol.DeconstructInfo(
				true, out string fullTypeName, out string namespaceName, out string genericParametersList,
				out _, out string typeKind, out string readonlyKeyword, out _
			);

			string hashCodeStr = string.Join(
				" ^ ",
				from member in attributeData.ConstructorArguments[0].Values
				let memberValue = ((string)member.Value!).Trim()
				select $"{memberValue}.GetHashCode()"
			);

			context.AddSource(
				typeSymbol.ToFileName(),
				GeneratedFileShortcuts.AutoGetHashCode,
				$@"#nullable enable

namespace {namespaceName};

partial {typeKind}{typeSymbol.Name}{genericParametersList}
{{
	/// <inheritdoc cref=""object.GetHashCode""/>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public override {readonlyKeyword}int GetHashCode() => {hashCodeStr};
}}
"
			);
		}
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context) => context.FastRegister<SyntaxReceiver>();
}
