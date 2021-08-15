namespace Sudoku.CodeGenerating.Generators;

/// <summary>
/// Indicates a source generator that generates the code for the method <c>GetEnumerator</c>.
/// </summary>
[Generator]
public sealed partial class GetEnumeratorGenerator : ISourceGenerator
{
	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		var receiver = (SyntaxReceiver)context.SyntaxReceiver!;
		var compilation = context.Compilation;
		foreach (var (type, attribute) in
			from type in receiver.Candidates
			let model = compilation.GetSemanticModel(type.Node.SyntaxTree)
			select (model.GetDeclaredSymbol(type.Node)!, type.Attribute))
		{
			var semanticModel = compilation.GetSemanticModel(attribute.SyntaxTree);
			if (getGetEnumeratorCode(type, attribute, semanticModel) is { } c)
			{
				context.AddSource(type.ToFileName(), "GetEnumerator", c);
			}
		}


		static ITypeSymbol? getReturnType(
			AttributeArgumentListSyntax attributeArgumentList, SemanticModel semanticModel)
		{
			foreach (var attributeArg in attributeArgumentList.Arguments)
			{
				if (
					attributeArg is
					{
						NameEquals.Name.Identifier.ValueText: nameof(AutoGetEnumeratorAttribute.ReturnType),
						Expression: var expr
					}
					&& semanticModel.GetOperation(expr) is ITypeOfOperation { TypeOperand: var operand }
				)
				{
					return operand;
				}
			}

			return null;
		}

		static string getExtraNamespaces(
			in SeparatedSyntaxList<AttributeArgumentSyntax> attributeArguments, SemanticModel semanticModel)
		{
			string p = string.Join(
				"\r\n",
				from arg in attributeArguments
				where arg.NameEquals?.Name.Identifier.ValueText == nameof(AutoGetEnumeratorAttribute.ExtraNamespaces)
				select arg.Expression into array
				select semanticModel.GetOperation(array) as IArrayCreationOperation into op
				where op is not null
				from element in op.Initializer.ElementValues
				select element as ILiteralOperation into literalOp
				where literalOp is { ConstantValue.HasValue: true }
				select $"using {literalOp.ConstantValue.Value};"
			);

			return p == string.Empty ? string.Empty : $"\r\n{p}";
		}

		static string getConversion(in SeparatedSyntaxList<AttributeArgumentSyntax> attributeArguments)
		{
			foreach (var attributeArgument in attributeArguments)
			{
				if (
					attributeArgument is
					{
						NameEquals.Name.Identifier.ValueText: nameof(AutoGetEnumeratorAttribute.MemberConversion),
						Expression: LiteralExpressionSyntax { Token.ValueText: var text }
					}
				)
				{
					return text;
				}
			}

			return "@";
		}

		string? getGetEnumeratorCode(
			INamedTypeSymbol symbol, AttributeSyntax attribute, SemanticModel semanticModel)
		{
			symbol.DeconstructInfo(
				false, out string fullTypeName, out string namespaceName, out string genericParameterList,
				out _, out string typeKind, out string readonlyKeyword, out _
			);

			string? typeArguments = symbol.AllInterfaces.FirstOrDefault(static i => i.Name.StartsWith("IEnumerable"))?.TypeArguments[0].ToDisplayString(FormatOptions.TypeFormat);
			var typeSymbol = getReturnType(attribute.ArgumentList, semanticModel);
			string returnType = typeSymbol is null ? $"System.Collections.Generic.IEnumerator<{typeArguments}>" : typeSymbol.ToDisplayString(FormatOptions.TypeFormat);

			if (attribute.ArgumentList is null || typeSymbol is null && typeArguments is null)
			{
				return null;
			}

			string memberNameStr = attribute.ArgumentList.Arguments[0].Expression.ToString();
			string memberName;
			if (memberNameStr == @"""@""")
			{
				memberName = "this";
			}
			else if (memberNameStr.Length - 8 is var pos and >= 0)
			{
				memberName = memberNameStr.Substring(7, pos);
			}
			else
			{
				return null;
			}

			string exprStr = getConversion(attribute.ArgumentList.Arguments);
			string memberConversion = exprStr.Replace("@", memberName).Replace("*", "GetEnumerator()");
			string extraNamespaces = getExtraNamespaces(attribute.ArgumentList.Arguments, semanticModel);
			bool implementsIEnumerableNongeneric = symbol.AllInterfaces.Any(static i => i is { Name: nameof(IEnumerable), IsGenericType: false });
			string interfaceExplicitlyImplementation = symbol.IsRefLikeType || !implementsIEnumerableNongeneric ? string.Empty : $@"

	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	{readonlyKeyword}System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();";

			return $@"#pragma warning disable 1591

using System.Collections;
using System.Collections.Generic;{extraNamespaces}

#nullable enable

namespace {namespaceName};

partial {typeKind}{symbol.Name}{genericParameterList}
{{
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public {readonlyKeyword}{returnType} GetEnumerator() => {memberConversion};{interfaceExplicitlyImplementation}
}}
";
		}
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context) => context.FastRegister<SyntaxReceiver>();
}
