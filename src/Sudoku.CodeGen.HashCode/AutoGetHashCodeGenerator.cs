#pragma warning disable IDE0057

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Sudoku.CodeGen.HashCode.Annotations;
using Sudoku.CodeGen.HashCode.Extensions;
using GenericsOptions = Microsoft.CodeAnalysis.SymbolDisplayGenericsOptions;
using GlobalNamespaceStyle = Microsoft.CodeAnalysis.SymbolDisplayGlobalNamespaceStyle;
using MiscellaneousOptions = Microsoft.CodeAnalysis.SymbolDisplayMiscellaneousOptions;
using TypeQualificationStyle = Microsoft.CodeAnalysis.SymbolDisplayTypeQualificationStyle;

namespace Sudoku.CodeGen.HashCode
{
	/// <summary>
	/// Indicates the generator that generates the code that overrides <see cref="object.GetHashCode"/>.
	/// </summary>
	/// <seealso cref="object.GetHashCode"/>
	[Generator]
	public sealed partial class AutoGetHashCodeGenerator : ISourceGenerator
	{
		/// <summary>
		/// Indicates whether the project uses tabs <c>'\t'</c> as indenting characters.
		/// </summary>
		private static readonly bool UsingTabsAsIndentingCharacters = true;

		/// <summary>
		/// Indicates whether the project outputs "<c><see langword="this"/>.</c>"
		/// as the member access expression.
		/// </summary>
		private static readonly bool OutputThisReference = false;

		/// <summary>
		/// Indicates the type format, and the property type format.
		/// </summary>
		private static readonly SymbolDisplayFormat
			TypeFormat = new(
				globalNamespaceStyle: GlobalNamespaceStyle.OmittedAsContaining,
				typeQualificationStyle: TypeQualificationStyle.NameAndContainingTypesAndNamespaces,
				genericsOptions: GenericsOptions.IncludeTypeParameters | GenericsOptions.IncludeTypeConstraints,
				miscellaneousOptions:
					MiscellaneousOptions.UseSpecialTypes
					| MiscellaneousOptions.EscapeKeywordIdentifiers
					| MiscellaneousOptions.IncludeNullableReferenceTypeModifier
			),
			PropertyTypeFormat = new(
				globalNamespaceStyle: GlobalNamespaceStyle.OmittedAsContaining,
				typeQualificationStyle: TypeQualificationStyle.NameAndContainingTypesAndNamespaces,
				genericsOptions: GenericsOptions.IncludeTypeParameters,
				miscellaneousOptions:
					MiscellaneousOptions.UseSpecialTypes
					| MiscellaneousOptions.EscapeKeywordIdentifiers
					| MiscellaneousOptions.IncludeNullableReferenceTypeModifier
			);


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
				string name = i == 0 ? classSymbol.Name : $"{classSymbol.Name}{(i + 1).ToString()}";
				classNameDic[classSymbol.Name] = i + 1;
				context.AddSource($"{name}.GetHashCode.g.cs", getGetHashCodeCode(classSymbol));
			}

			static IEnumerable<INamedTypeSymbol> g(in GeneratorExecutionContext context, SyntaxReceiver receiver)
			{
				var compilation = context.Compilation;

				return
					from candidateClass in receiver.Candidates
					let model = compilation.GetSemanticModel(candidateClass.SyntaxTree)
					select model.GetDeclaredSymbol(candidateClass)! into symbol
					where symbol.Marks<AutoHashCodeAttribute>()
					select (INamedTypeSymbol)symbol;
			}

			static string getGetHashCodeCode(INamedTypeSymbol symbol)
			{
				string namespaceName = symbol.ContainingNamespace.ToDisplayString();
				var members = GetMembers(symbol, handleRecursively: false);
				string fullTypeName = symbol.ToDisplayString(TypeFormat);
				int i = fullTypeName.IndexOf('<');
				string genericParametersList = i == -1 ? string.Empty : fullTypeName.Substring(i);

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
					.Append(PrintTypeKeywordToken(symbol.IsRecord ? true : null, symbol.TypeKind))
					.Append(symbol.Name)
					.AppendLine(genericParametersList)
					.AppendLine(PrintOpenBracketToken(1))
					.AppendLine(PrintCompilerGenerated(2))
					.Append(PrintIndenting(2))
					.Append(PrintPublicKeywordToken())
					.Append(PrintOverrideKeywordToken());

				// Not 'ref struct' and not 'readonly struct'.
				if (symbol is { TypeKind: TypeKind.Struct, IsRefLikeType: false, IsReadOnly: false })
				{
					source.Append(PrintReadOnlyKeywordToken());
				}

				source
					.Append(PrintIntKeywordToken())
					.Append(PrintGetHashCode())
					.Append(PrintOpenBraceToken())
					.Append(PrintClosedBraceToken())
					.Append(PrintLambdaOperatorToken());

				foreach (string item in members)
				{
					if (OutputThisReference)
					{
						source
							.Append(PrintThisKeywordToken())
							.Append(PrintDotToken());
					}

					source
						.Append(item)
						.Append(PrintDotToken())
						.Append(PrintGetHashCode())
						.Append(PrintOpenBraceToken())
						.Append(PrintClosedBraceToken())
						.Append(PrintExclusiveOrOperatorToken());
				}

				source
					.Remove(source.Length - 3, 3) // Remove last '^'.
					.AppendLine(PrintSemicolonToken())
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
		private static partial string PrintTypeKeywordToken(bool? isRecord, TypeKind typeKind);
		private static partial string PrintPublicKeywordToken();
		private static partial string PrintReadOnlyKeywordToken();
		private static partial string PrintIntKeywordToken();
		private static partial string PrintOverrideKeywordToken();
		private static partial string PrintGetHashCode();
		private static partial string PrintReturnKeywordToken();
		private static partial string PrintSemicolonToken();
		private static partial string PrintExclusiveOrOperatorToken();
		private static partial string PrintThisKeywordToken();
		private static partial string PrintDotToken();
		private static partial string PrintLambdaOperatorToken();
		private static partial string PrintOpenBracketToken(int indentingCount = 0);
		private static partial string PrintClosedBracketToken(int indentingCount = 0);
		private static partial string PrintPragmaWarningDisableCS1591();
		private static partial string PrintUsingDirectives();
		private static partial string PrintNullableEnable();
		private static partial string PrintIndenting(int indentingCount = 0);
		private static partial string PrintCompilerGenerated(int indentingCount = 0);

		private static partial IReadOnlyList<string> GetMembers(INamedTypeSymbol symbol, bool handleRecursively);
	}
}
