#pragma warning disable IDE0057

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Sudoku.CodeGen.PrimaryConstructor.Annotations;

namespace Sudoku.CodeGen.PrimaryConstructor
{
	/// <summary>
	/// Indicates a generator that generates primary constructors for <see langword="class"/>es
	/// when they're marked <see cref="AutoGeneratePrimaryConstructorAttribute"/>.
	/// </summary>
	/// <seealso cref="AutoGeneratePrimaryConstructorAttribute"/>
	[Generator]
	public sealed partial class PrimaryConstructorGenerator : ISourceGenerator
	{
		/// <summary>
		/// Indicates the separator used.
		/// </summary>
		private const string Separator = ", ";


		/// <summary>
		/// Indicates whether the project uses tabs <c>'\t'</c> as indenting characters.
		/// </summary>
		private static readonly bool UsingTabsAsIndentingCharacters = true;

		/// <summary>
		/// Indicates whether the project outputs "this." as the member access expression.
		/// </summary>
		private static readonly bool OutputThisReference = false;


		/// <inheritdoc/>
		public void Execute(GeneratorExecutionContext context)
		{
			if (context.SyntaxReceiver is not SyntaxReceiver receiver)
			{
				return;
			}

			var classNameDic = new Dictionary<string, int>();
			foreach (var classSymbol in g(context, receiver))
			{
				_ = classNameDic.TryGetValue(classSymbol.Name, out int i);
				var name = i == 0 ? classSymbol.Name : $"{classSymbol.Name}{i + 1}";
				classNameDic[classSymbol.Name] = i + 1;
				context.AddSource($"{name}.PrimaryConstructor.g.cs", getPrimaryConstructorCode(classSymbol));
			}


			static IEnumerable<INamedTypeSymbol> g(GeneratorExecutionContext context, SyntaxReceiver receiver)
			{
				var compilation = context.Compilation;

				return from candidateClass in receiver.CandidateClasses
					   let model = compilation.GetSemanticModel(candidateClass.SyntaxTree)
					   select model.GetDeclaredSymbol(candidateClass)! into classSymbol
					   where HasMarked(classSymbol, nameof(AutoGeneratePrimaryConstructorAttribute))
					   select (INamedTypeSymbol)classSymbol;
			}

			static string getPrimaryConstructorCode(INamedTypeSymbol classSymbol)
			{
				string namespaceName = classSymbol.ContainingNamespace.ToDisplayString();

				var baseClassCtorArgs =
					classSymbol.BaseType is { } baseType
					&& HasMarked(baseType, nameof(AutoGeneratePrimaryConstructorAttribute))
					? GetMembers(baseType, handleRecursively: true)
					: null;
				string? baseCtorInheritance = baseClassCtorArgs is { Count: not 0 }
					? $" : base({string.Join(", ", from x in baseClassCtorArgs select x.ParameterName)})"
					: null;

				var members = GetMembers(classSymbol, handleRecursively: false);
				var arguments =
					from x in baseClassCtorArgs is null ? members : members.Concat(baseClassCtorArgs)
					select $"{x.Type} {x.ParameterName}";
				string fullTypeName = classSymbol.ToDisplayString(TypeFormat);
				int i = fullTypeName.IndexOf('<');
				string genericParametersList = i < 0 ? string.Empty : fullTypeName.Substring(i);

#pragma warning disable IDE0055
				// Code generating.
				// Here the code may be ugly...
				var source = new StringBuilder()
					.AppendLine(PrintHeader())
					.AppendLine(PrintPragmaWarningDisableCS1591())
					.AppendLine()
					.AppendLine(PrintUsingDirectives())
					.AppendLine()
					.AppendLine(PrintNullableEnable())
					.AppendLine()
					.Append(PrintNamespaceKeywordToken())
					.AppendLine(namespaceName)
					.AppendLine(PrintOpenBracketToken())
					.Append(PrintIndenting(1))
					.Append(PrintPartialKeywordToken())
					.Append(PrintClassKeywordToken())
					.Append(classSymbol.Name)
					.AppendLine(genericParametersList)
					.AppendLine(PrintOpenBracketToken(1))
					.AppendLine(PrintCompilerGenerated(2))
					.Append(PrintIndenting(2))
					.Append(PrintPublicKeywordToken())
					.Append(classSymbol.Name)
					.Append(PrintOpenBraceToken())
					.Append(string.Join(Separator, arguments))
					.Append(PrintClosedBraceToken())
					.AppendLine(baseCtorInheritance)
					.Append(PrintOpenBracketToken(2));

				foreach (var item in members)
				{
					source
						.AppendLine()
						.Append(PrintIndenting(3));

					if (OutputThisReference)
					{
						source.Append(PrintThisKeywordToken());
						source.Append(PrintDotToken());
					}

					source
						.Append(item.Name)
						.Append(PrintEqualsToken())
						.Append(item.ParameterName)
						.Append(PrintSemicolonToken());
				}

				source
					.AppendLine()
					.AppendLine(PrintClosedBracketToken(2))
					.AppendLine(PrintClosedBracketToken(1))
					.AppendLine(PrintClosedBracketToken());

				return source.ToString();
			}
		}

		/// <inheritdoc/>
		public void Initialize(GeneratorInitializationContext context) =>
			context.RegisterForSyntaxNotifications(static () => new SyntaxReceiver());


		private static partial string PrintHeader();
		private static partial string PrintOpenBraceToken();
		private static partial string PrintClosedBraceToken();
		private static partial string PrintNamespaceKeywordToken();
		private static partial string PrintPartialKeywordToken();
		private static partial string PrintClassKeywordToken();
		private static partial string PrintPublicKeywordToken();
		private static partial string PrintSemicolonToken();
		private static partial string PrintEqualsToken();
		private static partial string PrintThisKeywordToken();
		private static partial string PrintDotToken();
		private static partial string PrintOpenBracketToken(int indentingCount = 0);
		private static partial string PrintClosedBracketToken(int indentingCount = 0);
		private static partial string PrintPragmaWarningDisableCS1591();
		private static partial string PrintUsingDirectives();
		private static partial string PrintNullableEnable();
		private static partial string PrintIndenting(int indentingCount = 0);
		private static partial string PrintCompilerGenerated(int indentingCount = 0);

		private static partial bool HasMarked(ISymbol symbol, string name);
		private static partial IReadOnlyList<SymbolInfo> GetMembers(INamedTypeSymbol classSymbol, bool handleRecursively);
	}
}
