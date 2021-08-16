namespace Sudoku.CodeGenerating.Generators;

/// <summary>
/// Provides a generator that generates the deconstruction methods that are extension methods.
/// </summary>
[Generator]
public sealed partial class ExtensionDeconstructMethodGenerator : ISourceGenerator
{
	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		var receiver = (SyntaxReceiver)context.SyntaxReceiver!;
		var compilation = context.Compilation;
		var nameDic = new Dictionary<string, int>();
		foreach (var groupResult in
			from attribute in receiver.Attributes
			select attribute.ArgumentList into argList
			where argList is { Arguments.Count: >= 2 }
			let firstArg = argList.Arguments[0].Expression as TypeOfExpressionSyntax
			where firstArg is not null
			let semanticModel = compilation.GetSemanticModel(firstArg.SyntaxTree)
			let operation = semanticModel.GetOperation(firstArg) as ITypeOfOperation
			where operation is not null
			let type = operation.TypeOperand
			group (argList, type) by type.ToDisplayString(FormatOptions.TypeFormat))
		{
			string key = groupResult.Key;
			foreach (var (p, typeSymbol) in groupResult)
			{
				_ = nameDic.TryGetValue(typeSymbol.Name, out int i);
				string name = i == 0 ? typeSymbol.Name : $"{typeSymbol.Name}{i + 1}";
				nameDic[typeSymbol.Name] = i + 1;
				context.AddSource($"{name}", "DeconstructionMethods", getDeconstructionCode(typeSymbol, p));
			}
		}


		string getDeconstructionCode(ITypeSymbol symbol, AttributeArgumentListSyntax argList)
		{
			string? tempNamespace = argList.Arguments.FirstOrDefault(
				static arg => arg is { NameEquals.Name.Identifier.ValueText: "Namespace" }
			)?.Expression.ToString();
			string namespaceName = tempNamespace?.Substring(1, tempNamespace.Length - 2)
				?? $"{symbol.ContainingNamespace}";
			string typeName = symbol.Name;
			symbol.DeconstructInfo(
				out string fullTypeName, out _, out string genericParametersList,
				out string genericParameterListWithoutConstraint, out string fullTypeNameWithoutConstraint,
				out string constraint, out _
			);
			var members =
				from arg in argList.Arguments
				let expr = arg.Expression as InvocationExpressionSyntax
				where expr.IsNameOfExpression() && expr!.ArgumentList.Arguments.Count == 1
				let p = expr.ArgumentList.Arguments[0].ToString()
				let lastDotTokenPos = p.LastIndexOf('.')
				where lastDotTokenPos != -1
				select p.Substring(lastDotTokenPos + 1);
			string inModifier = symbol.TypeKind == TypeKind.Struct ? "in " : string.Empty;
			string parameterList = string.Join(
				", ",
				from member in members
				let memberFound = symbol.GetAllMembers().FirstOrDefault(m => m.Name == member)
				where memberFound is not null
				let memberType = memberFound.GetMemberType()
				where memberType is not null
				select $@"out {memberType} {member.ToCamelCase()}"
			);
			string assignments = string.Join(
				"\r\n\t\t\t",
				from member in members select $"{member.ToCamelCase()} = @this.{member};"
			);
			string deconstructMethods = $@"[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public static void Deconstruct{genericParameterListWithoutConstraint}(this {inModifier}{fullTypeNameWithoutConstraint} @this, {parameterList}){constraint}
	{{
		{assignments}
	}}";

			return $@"#pragma warning disable 618, 1591

#nullable enable

namespace {namespaceName};

public static partial class {typeName}_DeconstructionMethods
{{
	{deconstructMethods}
}}
";
		}
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context) => context.FastRegister<SyntaxReceiver>();
}
