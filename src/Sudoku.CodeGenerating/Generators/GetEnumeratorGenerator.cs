#pragma warning disable IDE0057

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Sudoku.CodeGenerating.Extensions;

namespace Sudoku.CodeGenerating
{
	/// <summary>
	/// Indicates a source generator that generates the code for the method <c>GetEnumerator</c>.
	/// </summary>
	[Generator]
	public sealed partial class GetEnumeratorGenerator : ISourceGenerator
	{
		/// <inheritdoc/>
		public void Execute(GeneratorExecutionContext context)
		{
			if (context.SyntaxReceiver is not SyntaxReceiver receiver)
			{
				return;
			}

			var compilation = context.Compilation;
			var nameDic = new Dictionary<string, int>();
			foreach (var (symbol, attribute) in g(context, receiver))
			{
				_ = nameDic.TryGetValue(symbol.Name, out int i);
				string name = i == 0 ? symbol.Name : $"{symbol.Name}{(i + 1).ToString()}";
				nameDic[symbol.Name] = i + 1;

				var syntaxTree = attribute.SyntaxTree;
				var semanticModel = compilation.GetSemanticModel(syntaxTree);
				if (getGetEnumeratorCode(symbol, attribute, semanticModel) is { } c)
				{
					context.AddSource($"{name}.GetEnumerator.g.cs", c);
				}
			}

			static IEnumerable<(INamedTypeSymbol, AttributeSyntax)> g(
				in GeneratorExecutionContext context, SyntaxReceiver receiver)
			{
				var compilation = context.Compilation;

				return
					from candidateInfo in receiver.Candidates
					let model = compilation.GetSemanticModel(candidateInfo.Node.SyntaxTree)
					select (
						(INamedTypeSymbol)model.GetDeclaredSymbol(candidateInfo.Node)!,
						candidateInfo.Attribute
					);
			}

			static ITypeSymbol? getReturnType(
				AttributeArgumentListSyntax attributeArgumentList, SemanticModel semanticModel)
			{
				foreach (var attributeArg in attributeArgumentList.Arguments)
				{
					if (
						attributeArg is
						{
							NameEquals:
							{
								Name:
								{
									Identifier: { ValueText: nameof(AutoGetEnumeratorAttribute.ReturnType) }
								}
							},
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
					from attributeArgument in attributeArguments
					let nameEqualsNode = attributeArgument.NameEquals
					where nameEqualsNode is not null && nameEqualsNode.Name.Identifier.ValueText == nameof(AutoGetEnumeratorAttribute.ExtraNamespaces)
					let array = attributeArgument.Expression
					let op = semanticModel.GetOperation(array) as IArrayCreationOperation
					where op is not null
					from element in op.Initializer.ElementValues
					let literalOp = element as ILiteralOperation
					where literalOp is { ConstantValue: { HasValue: true } }
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
							NameEquals:
							{
								Name:
								{
									Identifier:
									{
										ValueText: nameof(AutoGetEnumeratorAttribute.MemberConversion)
									}
								}
							},
							Expression: LiteralExpressionSyntax
							{
								Token: { ValueText: var text }
							}
						}
					)
					{
						return text;
					}
				}

				return "@";
			}

			static string? getGetEnumeratorCode(
				INamedTypeSymbol symbol, AttributeSyntax attribute, SemanticModel semanticModel)
			{
				string namespaceName = symbol.ContainingNamespace.ToDisplayString();
				string typeName = symbol.Name;
				string fullTypeName = symbol.ToDisplayString(FormatOptions.TypeFormat);
				int i = fullTypeName.IndexOf('<');
				string genericParameterList = i == -1 ? string.Empty : fullTypeName.Substring(i);
				string readonlyKeyword = symbol.MemberShouldAppendReadOnly()  ? "readonly " : string.Empty;
				string? typeArguments = symbol.AllInterfaces.FirstOrDefault(static i => i.Name.StartsWith("IEnumerable"))?.TypeArguments[0].ToDisplayString(FormatOptions.TypeFormat);
				var typeSymbol = getReturnType(attribute.ArgumentList, semanticModel);
				string returnType = typeSymbol is null ? $"System.Collections.Generic.IEnumerator<{typeArguments}>" : typeSymbol.ToDisplayString(FormatOptions.TypeFormat);

				if (attribute.ArgumentList is null || typeSymbol is null && typeArguments is null)
				{
					return null;
				}

				string typeKind = symbol.GetTypeKindString();
				string memberNameStr = attribute.ArgumentList.Arguments[0].Expression.ToString();
				string memberName = memberNameStr == @"""@""" ? "this" : memberNameStr.Substring(7, memberNameStr.Length - 8);
				string exprStr = getConversion(attribute.ArgumentList.Arguments);
				string memberConversion = exprStr.Replace("@", memberName).Replace("*", "GetEnumerator()");
				string extraNamespaces = getExtraNamespaces(attribute.ArgumentList.Arguments, semanticModel);
				bool implementsIEnumerableNongeneric = symbol.AllInterfaces.Any(static i => i is { Name: nameof(IEnumerable), IsGenericType: false });
				string interfaceExplicitlyImplementation = symbol.IsRefLikeType || !implementsIEnumerableNongeneric ? string.Empty : $@"

		[CompilerGenerated]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		{readonlyKeyword}System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();";

				return $@"#pragma warning disable 1591

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;{extraNamespaces}

#nullable enable

namespace {namespaceName}
{{
	partial {typeKind}{typeName}{genericParameterList}
	{{
		[CompilerGenerated]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public {readonlyKeyword}{returnType} GetEnumerator() => {memberConversion};{interfaceExplicitlyImplementation}
	}}
}}";
			}
		}

		/// <inheritdoc/>
		public void Initialize(GeneratorInitializationContext context) =>
			context.RegisterForSyntaxNotifications(static () => new SyntaxReceiver());
	}
}
