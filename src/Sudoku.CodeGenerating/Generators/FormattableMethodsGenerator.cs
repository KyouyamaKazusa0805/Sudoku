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
			var receiver = (SyntaxReceiver)context.SyntaxReceiver!;
			var nameDic = new Dictionary<string, int>();
			var compilation = context.Compilation;
			foreach (var classSymbol in
				from candidate in receiver.Candidates
				let model = compilation.GetSemanticModel(candidate.SyntaxTree)
				select (INamedTypeSymbol)model.GetDeclaredSymbol(candidate)! into symbol
				where symbol.Marks<AutoFormattableAttribute>()
				select symbol)
			{
				_ = nameDic.TryGetValue(classSymbol.Name, out int i);
				string name = i == 0 ? classSymbol.Name : $"{classSymbol.Name}{(i + 1).ToString()}";
				nameDic[classSymbol.Name] = i + 1;

				if (getToStringMethodsCode(context, classSymbol) is { } c)
				{
					context.AddSource($"{name}.ToString.g.cs", c);
				}
			}


			static string? getToStringMethodsCode(in GeneratorExecutionContext context, INamedTypeSymbol symbol)
			{
				string namespaceName = symbol.ContainingNamespace.ToDisplayString();
				string fullTypeName = symbol.ToDisplayString(FormatOptions.TypeFormat);
				int i = fullTypeName.IndexOf('<');
				string genericParametersList = i == -1 ? string.Empty : fullTypeName.Substring(i);
				int j = fullTypeName.IndexOf('>');
				string genericParametersListWithoutConstraint = i == -1 ? string.Empty : fullTypeName.Substring(i, j - i + 1);
				string typeKind = symbol.GetTypeKindString();
				string readonlyKeyword = symbol.MemberShouldAppendReadOnly() ? "readonly " : string.Empty;

				return $@"#pragma warning disable 1591

using System.Runtime.CompilerServices;

#nullable enable

namespace {namespaceName}
{{
	partial {typeKind}{symbol.Name}{genericParametersList}
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
