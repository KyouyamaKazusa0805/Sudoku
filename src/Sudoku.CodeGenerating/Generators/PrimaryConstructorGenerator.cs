using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Sudoku.CodeGenerating.Extensions;

namespace Sudoku.CodeGenerating
{
	/// <summary>
	/// Indicates a generator that generates primary constructors for <see langword="class"/>es
	/// when they're marked <see cref="AutoGeneratePrimaryConstructorAttribute"/>.
	/// </summary>
	/// <remarks>
	/// This generator can <b>only</b> support non-nested <see langword="class"/>es.
	/// </remarks>
	/// <seealso cref="AutoGeneratePrimaryConstructorAttribute"/>
	[Generator]
	public sealed partial class PrimaryConstructorGenerator : ISourceGenerator
	{
		/// <inheritdoc/>
		public void Execute(GeneratorExecutionContext context)
		{
			var receiver = (SyntaxReceiver)context.SyntaxReceiver!;
			var nameDic = new Dictionary<string, int>();
			var compilation = context.Compilation;
			foreach (var symbol in
				from candidate in receiver.CandidateClasses
				let model = compilation.GetSemanticModel(candidate.SyntaxTree)
				select model.GetDeclaredSymbol(candidate)! into symbol
				where symbol.Marks<AutoGeneratePrimaryConstructorAttribute>()
				select (INamedTypeSymbol)symbol)
			{
				_ = nameDic.TryGetValue(symbol.Name, out int i);
				string name = i == 0 ? symbol.Name : $"{symbol.Name}{(i + 1).ToString()}";
				nameDic[symbol.Name] = i + 1;
				context.AddSource($"{name}.PrimaryConstructor.g.cs", getPrimaryConstructorCode(symbol));
			}


			static string getPrimaryConstructorCode(INamedTypeSymbol symbol)
			{
				string namespaceName = symbol.ContainingNamespace.ToDisplayString();
				string fullTypeName = symbol.ToDisplayString(FormatOptions.TypeFormat);
				int i = fullTypeName.IndexOf('<');
				string genericParametersList = i == -1 ? string.Empty : fullTypeName.Substring(i);

				var baseClassCtorArgs =
					symbol.BaseType is { } baseType && baseType.Marks<AutoGeneratePrimaryConstructorAttribute>()
					? GetMembers(baseType, handleRecursively: true)
					: null;
				/*length-pattern*/
				string? baseCtorInheritance = baseClassCtorArgs is not { Count: not 0 }
					? null
					: $" : base({string.Join(", ", from x in baseClassCtorArgs select x.ParameterName)})";

				var members = GetMembers(symbol, handleRecursively: false);
				var arguments =
					from x in baseClassCtorArgs is null ? members : members.Concat(baseClassCtorArgs)
					select $"{x.Type} {x.ParameterName}";
				string parameterList = string.Join(", ", arguments);
				string memberAssignments = string.Join("\r\n\t\t\t", from member in members select $"{member.Name} = {member.ParameterName};");

				return $@"#pragma warning disable 1591

using System.Runtime.CompilerServices;

#nullable enable

namespace {namespaceName}
{{
	partial class {symbol.Name}{genericParametersList}
	{{
		[CompilerGenerated]
		public {symbol.Name}({parameterList}){baseCtorInheritance}
		{{
			{memberAssignments}
		}}
	}}
}}";
			}
		}

		/// <inheritdoc/>
		public void Initialize(GeneratorInitializationContext context) =>
			context.RegisterForSyntaxNotifications(static () => new SyntaxReceiver());


		/// <summary>
		/// Try to get all possible fields or properties in the specified <see langword="class"/> type.
		/// </summary>
		/// <param name="classSymbol">The specified class symbol.</param>
		/// <param name="handleRecursively">
		/// A <see cref="bool"/> value indicating whether the method will handle the type recursively.</param>
		/// <returns>The result list that contains all member symbols.</returns>
		private static IReadOnlyList<SymbolInfo> GetMembers(INamedTypeSymbol classSymbol, bool handleRecursively)
		{
			var result = new List<SymbolInfo>(
				(
					from x in classSymbol.GetMembers().OfType<IFieldSymbol>()
					where x is { CanBeReferencedByName: true, IsStatic: false }
						&& (
							x.IsReadOnly
							&& !x.HasInitializer()
							|| x.Marks<PrimaryConstructorIncludedMemberAttribute>()
						)
						&& !x.Marks<PrimaryConstructorIgnoredMemberAttribute>()
					select new SymbolInfo(
						x.Type.ToDisplayString(FormatOptions.PropertyTypeFormat),
						toCamelCase(x.Name),
						x.Name,
						x.GetAttributes()
					)
				).Concat(
					from x in classSymbol.GetMembers().OfType<IPropertySymbol>()
					where x is { CanBeReferencedByName: true, IsStatic: false }
						&& (
							x.IsReadOnly
							&& !x.HasInitializer()
							|| x.Marks<PrimaryConstructorIncludedMemberAttribute>()
						)
						&& !x.Marks<PrimaryConstructorIgnoredMemberAttribute>()
					select new SymbolInfo(
						x.Type.ToDisplayString(FormatOptions.PropertyTypeFormat),
						toCamelCase(x.Name),
						x.Name,
						x.GetAttributes()
					)
				)
			);

			if (handleRecursively
				&& classSymbol.BaseType is { } baseType
				&& baseType.Marks<AutoGeneratePrimaryConstructorAttribute>())
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
