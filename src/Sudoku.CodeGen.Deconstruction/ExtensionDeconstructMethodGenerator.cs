#pragma warning disable IDE0057

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Sudoku.CodeGen.Deconstruction.Extensions;

namespace Sudoku.CodeGen.Deconstruction
{
	/// <summary>
	/// Provides a generator that generates the deconstruction methods that are extension methods.
	/// </summary>
	[Generator]
	public sealed partial class ExtensionDeconstructMethodGenerator : ISourceGenerator
	{
		/// <inheritdoc/>
		public void Execute(GeneratorExecutionContext context)
		{
			if (context.SyntaxReceiver is not SyntaxReceiver syntaxReceiver)
			{
				return;
			}

			var compilation = context.Compilation;
			var groupedResult = g(syntaxReceiver, compilation);

			var nameDic = new Dictionary<string, int>();
			foreach (var groupResult in groupedResult)
			{
				string key = groupResult.Key;
				foreach (var (p, typeSymbol) in groupResult)
				{
					_ = nameDic.TryGetValue(typeSymbol.Name, out int i);
					string name = i == 0 ? typeSymbol.Name : $"{typeSymbol.Name}{i + 1}";
					nameDic[typeSymbol.Name] = i + 1;
					context.AddSource($"{name}Ex.DeconstructionMethods.g.cs", getDeconstructionCode(typeSymbol, p));
				}
			}


			static IEnumerable<IGrouping<string, (AttributeArgumentListSyntax, ITypeSymbol)>> g(
				SyntaxReceiver syntaxReceiver, Compilation compilation) =>
				from attribute in syntaxReceiver.Attributes
				let argList = attribute.ArgumentList
				where argList is { Arguments: { Count: >= 2 } }
				let firstArg = argList.Arguments[0].Expression as TypeOfExpressionSyntax
				where firstArg is not null
				let semanticModel = compilation.GetSemanticModel(firstArg.SyntaxTree)
				let operation = semanticModel.GetOperation(firstArg) as ITypeOfOperation
				where operation is not null
				let type = operation.TypeOperand
				group (argList, type) by type.ToDisplayString();

			static string getDeconstructionCode(ITypeSymbol symbol, AttributeArgumentListSyntax argList)
			{
				string? tempNamespace = argList.Arguments.FirstOrDefault(
					/*extended-property-pattern*/
					static arg => arg is { NameEquals: { Name: { Identifier: { ValueText: "Namespace" } } } }
				)?.Expression.ToString();
				string namespaceName = tempNamespace?.Substring(1, tempNamespace.Length - 2) ?? $"{symbol.ContainingNamespace}.Extensions";
				string typeName = symbol.Name;
				string fullTypeName = symbol.ToDisplayString(FormatOptions.TypeFormat);
				int i = fullTypeName.IndexOf('<');
				string genericParametersList = i == -1 ? string.Empty : fullTypeName.Substring(i);
				int j = fullTypeName.IndexOf('>');
				string fullTypeNameWithoutConstraint = j + 1 == genericParametersList.Length ? fullTypeName : fullTypeName.Substring(0, j);
				string genericParameterListWithoutConstraint = i == -1 ? string.Empty : fullTypeName.Substring(i, j - i + 1);
				string constraint = j + 1 == genericParametersList.Length ? string.Empty : genericParametersList.Substring(j + 1);
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
					let memberType = getMemberType(memberFound)
					where memberType is not null
					select $@"out {memberType} {toCamelCase(member)}"
				);
				string assignments = string.Join(
					"\r\n\t\t\t",
					from member in members select $"{toCamelCase(member)} = @this.{member};"
				);
				string deconstructMethods = $@"[CompilerGenerated]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Deconstruct{genericParameterListWithoutConstraint}(this {inModifier}{fullTypeNameWithoutConstraint} @this, {parameterList}){constraint}
		{{
			{assignments}
		}}";

				return $@"#pragma warning disable 1591

using System.Runtime.CompilerServices;

#nullable enable

namespace {namespaceName}
{{
	public static partial class {typeName}Ex
	{{
		{deconstructMethods}
	}}
}}";

				static string? getMemberType(ISymbol symbol) => symbol switch
				{
					IFieldSymbol f => f.Type.ToDisplayString(FormatOptions.PropertyTypeFormat),
					IPropertySymbol p => p.Type.ToDisplayString(FormatOptions.PropertyTypeFormat),
					_ => null
				};

				static string toCamelCase(string input)
				{
					input = input.TrimStart('_');
					return input.Substring(0, 1).ToLowerInvariant() + input.Substring(1);
				}
			}
		}

		/// <inheritdoc/>
		public void Initialize(GeneratorInitializationContext context) =>
			context.RegisterForSyntaxNotifications(static () => new SyntaxReceiver());
	}
}
