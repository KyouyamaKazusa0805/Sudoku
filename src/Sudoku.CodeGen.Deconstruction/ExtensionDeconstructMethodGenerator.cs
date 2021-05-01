#pragma warning disable IDE0057

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sudoku.CodeGen.Deconstruction.Extensions;
using ListOfArgLists = System.Linq.IGrouping<string, Microsoft.CodeAnalysis.CSharp.Syntax.AttributeArgumentListSyntax>;

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
			var groupedResult = g(syntaxReceiver, compilation).ToArray();

			var nameDic = new Dictionary<string, int>();
			foreach (var (typeSymbol, p) in groupedResult)
			{
				_ = nameDic.TryGetValue(typeSymbol.Name, out int i);
				string name = i == 0 ? typeSymbol.Name : $"{typeSymbol.Name}{(i + 1).ToString()}";
				nameDic[typeSymbol.Name] = i + 1;
				context.AddSource($"{name}Ex.DeconstructionMethods.g.cs", getDeconstructionCode(typeSymbol, p));
			}


			static IEnumerable<(INamedTypeSymbol, ListOfArgLists)> g(SyntaxReceiver syntaxReceiver, Compilation compilation) =>
				from attribute in syntaxReceiver.ModuleAttributes
				let argList = attribute.ArgumentList
				where argList is { Arguments: { Count: >= 2 } }
				let firstArg = argList.Arguments[0].Expression as TypeOfExpressionSyntax
				where firstArg is not null
				let type = firstArg.Type
				group argList by type.ToString() into attributeGroups
				let key = attributeGroups.Key
				let typeSymbol = compilation.GetTypeByMetadataName(key)
				where typeSymbol is not null
				select (typeSymbol, attributeGroups);

			static string getDeconstructionCode(INamedTypeSymbol symbol, ListOfArgLists argListLists)
			{
				string namespaceName = symbol.ContainingNamespace.ToString();
				string typeName = symbol.Name;
				string deconstructMethods = string.Join(
					"\r\n\r\n\t\t",
					from argList in argListLists
					let members =
						from arg in argList.Arguments
						let expr = arg.Expression as InvocationExpressionSyntax
						where expr.IsNameOfExpression() && expr!.ArgumentList.Arguments.Count > 1
						let p = expr.ArgumentList.Arguments[0].ToString()
						let lastDotTokenPos = p.LastIndexOf('.')
						where lastDotTokenPos != -1
						select p.Substring(lastDotTokenPos + 1)
					let assignments = string.Join("\r\n\t\t\t", from member in members select $"{toCamelCase(member)} = @this.{member};")
					let parameterList = string.Join(", ", from member in members select "") // TODO: Type checking.
					let perMethod = $@"[CompilerGenerated]
		public static void Deconstruct({parameterList})
		{{
			{assignments}
		}}"
					select $@""
				);

				return $@"#pragma warning disable 1591

using System.Runtime.CompilerServices;

#nullable enable

namespace {namespaceName}
{{
	[CompilerGenerated]
	public static class {typeName}Ex
	{{
		{deconstructMethods}
	}}
}}";

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


		/// <summary>
		/// Try to get all possible fields or properties in the specified type symbol.
		/// </summary>
		/// <param name="symbol">The specified type symbol.</param>
		/// <param name="handleRecursively">
		/// A <see cref="bool"/> value indicating whether the method will handle the type recursively.
		/// </param>
		/// <returns>The result list that contains all member symbols.</returns>
		private static IReadOnlyList<(ITypeSymbol Type, string ParameterName, string Name)> GetMembers(INamedTypeSymbol symbol, bool handleRecursively)
		{
			var result = new List<(ITypeSymbol, string, string)>(
				(
					from x in symbol.GetMembers().OfType<IFieldSymbol>()
					select (x.Type, toCamelCase(x.Name), x.Name)
				).Concat(
					from x in symbol.GetMembers().OfType<IPropertySymbol>()
					select (x.Type, toCamelCase(x.Name), x.Name)
				)
			);

			if (handleRecursively
				&& symbol.BaseType is { } baseType && baseType.Marks<AutoDeconstructAttribute>())
			{
				result.AddRange(GetMembers(baseType, true));
			}

			return result;


			static string toCamelCase(string name)
			{
				name = name.TrimStart('_');
				return name.Substring(0, 1).ToLowerInvariant() + name.Substring(1);
			}
		}
	}
}
