#pragma warning disable IDE0057

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Sudoku.CodeGenerating.Extensions;

namespace Sudoku.CodeGenerating
{
	/// <summary>
	/// Defines a source generator that generates the code for <c>ToString</c> methods. The methods below
	/// will be generated:
	/// <list type="bullet">
	/// <item><c>string ToString()</c></item>
	/// <item><c>string ToString(string? format)</c></item>
	/// </list>
	/// </summary>
	[Generator]
	public sealed partial class FormattableMethodsGenerator : ISourceGenerator
	{
		/// <inheritdoc/>
		public void Execute(GeneratorExecutionContext context)
		{
			if (context.SyntaxReceiver is not SyntaxReceiver receiver)
			{
				return;
			}

			var nameDic = new Dictionary<string, int>();
			foreach (var classSymbol in g(context, receiver))
			{
				_ = nameDic.TryGetValue(classSymbol.Name, out int i);
				string name = i == 0 ? classSymbol.Name : $"{classSymbol.Name}{(i + 1).ToString()}";
				nameDic[classSymbol.Name] = i + 1;

				if (getToStringMethodsCode(context, classSymbol) is { } c)
				{
					context.AddSource($"{name}.ToString.g.cs", c);
				}
			}


			static IEnumerable<INamedTypeSymbol> g(in GeneratorExecutionContext context, SyntaxReceiver receiver)
			{
				var compilation = context.Compilation;

				return
					from candidate in receiver.Candidates
					let model = compilation.GetSemanticModel(candidate.SyntaxTree)
					select model.GetDeclaredSymbol(candidate)! into symbol
					where symbol.Marks<AutoFormattableAttribute>()
					select (INamedTypeSymbol)symbol;
			}


			static string? getToStringMethodsCode(in GeneratorExecutionContext context, INamedTypeSymbol symbol)
			{
				string namespaceName = symbol.ContainingNamespace.ToDisplayString();
				string fullTypeName = symbol.ToDisplayString(FormatOptions.TypeFormat);
				int i = fullTypeName.IndexOf('<');
				string genericParametersList = i == -1 ? string.Empty : fullTypeName.Substring(i);
				int j = fullTypeName.IndexOf('>');
				string genericParametersListWithoutConstraint = i == -1 ? string.Empty : fullTypeName.Substring(i, j - i + 1);
				string typeKind = symbol switch
				{
					{ IsRecord: true } => "record",
					{ TypeKind: TypeKind.Class } => "class",
					{ TypeKind: TypeKind.Struct } => "struct"
				};

				string readonlyKeyword = symbol is { TypeKind: TypeKind.Struct, IsReadOnly: false } ? "readonly " : string.Empty;

				return $@"#pragma warning disable 1591

using System.Runtime.CompilerServices;

#nullable enable

namespace {namespaceName}
{{
	partial {typeKind} {symbol.Name}{genericParametersList}
	{{
		/// <inheritdoc cref=""object.ToString""/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[CompilerGenerated]
		public override {readonlyKeyword}partial string ToString() => ToString(null, null);

		/// <summary>
		/// Returns a string that represents the current object with the specified format string.
		/// </summary>
		/// <param name=""format"">
		/// The format. If available, the parameter can be <see langword=""null""/>.
		/// </param>
		/// <returns>The string result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[CompilerGenerated]
		public {readonlyKeyword}partial string ToString(string? format) => ToString(format, null);
	}}
}}";
			}
		}

		/// <inheritdoc/>
		public void Initialize(GeneratorInitializationContext context) =>
			context.RegisterForSyntaxNotifications(static () => new SyntaxReceiver());
	}
}
