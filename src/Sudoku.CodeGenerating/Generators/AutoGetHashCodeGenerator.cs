using Sudoku.CodeGenerating.Extensions;

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
		Func<ISymbol?, ISymbol?, bool> f = SymbolEqualityComparer.Default.Equals;
		foreach (var type in
			from type in receiver.Candidates
			let model = compilation.GetSemanticModel(type.SyntaxTree)
			select model.GetDeclaredSymbol(type)! into symbol
			where symbol.GetAttributes().Any(a => f(a.AttributeClass, attributeSymbol))
			select symbol)
		{
			if (type.GetAttributeString(attributeSymbol) is not { } attributeStr)
			{
				continue;
			}

			int tokenStartIndex = attributeStr.IndexOf("({");
			if (tokenStartIndex == -1)
			{
				continue;
			}

			if (attributeStr.GetMemberValues(tokenStartIndex) is not { Length: not 0 } members)
			{
				continue;
			}

			type.DeconstructInfo(
				true, out string fullTypeName, out string namespaceName, out string genericParametersList,
				out _, out string typeKind, out string readonlyKeyword, out _
			);
			string hashCodeStr = string.Join(" ^ ", from member in members select $"{member}.GetHashCode()");

			context.AddSource(
				type.ToFileName(),
				"GetHashCode",
				$@"#nullable enable

namespace {namespaceName};

partial {typeKind}{type.Name}{genericParametersList}
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
