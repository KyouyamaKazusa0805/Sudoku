using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Sudoku.CodeGenerating.Extensions;
using static Sudoku.CodeGenerating.Constants;

namespace Sudoku.CodeGenerating.Generators
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
			Func<ISymbol?, ISymbol?, bool> f = SymbolEqualityComparer.Default.Equals;
			var compilation = context.Compilation;
			var attributeSymbol = compilation.GetTypeByMetadataName<AutoFormattableAttribute>();
			foreach (var type in
				from candidate in receiver.Candidates
				let model = compilation.GetSemanticModel(candidate.SyntaxTree)
				select (INamedTypeSymbol)model.GetDeclaredSymbol(candidate)! into type
				where type.GetAttributes().Any(a => f(a.AttributeClass, attributeSymbol))
				select type)
			{
				type.DeconstructInfo(
					false, out string fullTypeName, out string namespaceName, out string genericParametersList,
					out string genericParametersListWithoutConstraint, out string typeKind,
					out string readonlyKeyword, out _
				);

				context.AddSource(
					type.ToFileName(),
					"ToString",
					$@"#nullable enable

namespace {namespaceName}
{{
	partial {typeKind}{type.Name}{genericParametersList}
	{{
		/// <inheritdoc cref=""object.ToString""/>
		[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
		[global::System.Runtime.CompilerServices.CompilerGenerated]
		[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		public override {readonlyKeyword}partial string ToString() => ToString(null, null);

		/// <summary>
		/// Returns a string that represents the current object with the specified format string.
		/// </summary>
		/// <param name=""format"">
		/// The format. If available, the parameter can be <see langword=""null""/>.
		/// </param>
		/// <returns>The string result.</returns>
		[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
		[global::System.Runtime.CompilerServices.CompilerGenerated]
		[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		public {readonlyKeyword}partial string ToString(string? format) => ToString(format, null);
	}}
}}"
				);
			}
		}

		/// <inheritdoc/>
		public void Initialize(GeneratorInitializationContext context) => context.FastRegister<SyntaxReceiver>();
	}
}
