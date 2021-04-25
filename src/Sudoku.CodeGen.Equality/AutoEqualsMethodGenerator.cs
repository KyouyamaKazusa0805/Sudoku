#pragma warning disable IDE0057

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Sudoku.CodeGen.Equality.Annotations;
using Sudoku.CodeGen.Equality.Extensions;
using GenericsOptions = Microsoft.CodeAnalysis.SymbolDisplayGenericsOptions;
using GlobalNamespaceStyle = Microsoft.CodeAnalysis.SymbolDisplayGlobalNamespaceStyle;
using MiscellaneousOptions = Microsoft.CodeAnalysis.SymbolDisplayMiscellaneousOptions;
using TypeQualificationStyle = Microsoft.CodeAnalysis.SymbolDisplayTypeQualificationStyle;

namespace Sudoku.CodeGen.Equality
{
	/// <summary>
	/// Indicates the generator that generates the override of <see cref="object.Equals(object)"/>.
	/// </summary>
	/// <seealso cref="object.Equals(object)"/>
	[Generator]
	public sealed partial class AutoEqualsMethodGenerator : ISourceGenerator
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
		/// Indicates whetehr the project outputs <c>[MethodImpl(MethodImplOptions.AggressiveInlining)]</c>
		/// as the inlining mark.
		/// </summary>
		private static readonly bool OutputAggressiveInliningMark = true;

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

			var nameDic = new Dictionary<string, int>();
			foreach (var classSymbol in g(context, receiver))
			{
				_ = nameDic.TryGetValue(classSymbol.Name, out int i);
				string name = i == 0 ? classSymbol.Name : $"{classSymbol.Name}{(i + 1).ToString()}";
				nameDic[classSymbol.Name] = i + 1;

				if (getEqualityMethodsCode(classSymbol) is { } c)
				{
					context.AddSource($"{name}.Equality.g.cs", c);
				}
			}


			static IEnumerable<INamedTypeSymbol> g(in GeneratorExecutionContext context, SyntaxReceiver receiver)
			{
				var compilation = context.Compilation;

				return
					from candidateClass in receiver.Candidates
					let model = compilation.GetSemanticModel(candidateClass.SyntaxTree)
					select model.GetDeclaredSymbol(candidateClass)! into symbol
					where symbol.Marks<AutoEqualityAttribute>()
					select (INamedTypeSymbol)symbol;
			}

			static string? getEqualityMethodsCode(INamedTypeSymbol symbol)
			{
				string namespaceName = symbol.ContainingNamespace.ToDisplayString();
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
					.AppendLine(PrintOpenBracketToken(1));

				if (symbol is { IsRefLikeType: false })
				{
					source.AppendLine(PrintCompilerGenerated(2));

					if (OutputAggressiveInliningMark)
					{
						source.AppendLine(PrintAggressiveInlining(2));
					}

					source
						.Append(PrintIndenting(2))
						.Append(PrintPublicKeywordToken())
						.Append(PrintOverrideKeywordToken());

					// Not 'ref struct' and not 'readonly struct'.
					if (symbol is { TypeKind: TypeKind.Struct, IsRefLikeType: false, IsReadOnly: false })
					{
						source.Append(PrintReadOnlyKeywordToken());
					}

					source
						.Append(PrintBoolKeywordToken())
						.Append(PrintEquals())
						.Append(PrintOpenBraceToken())
						.Append(PrintNullableObjectKeywordToken())
						.Append(PrintOther())
						.Append(PrintClosedBraceToken())
						.Append(PrintLambdaOperatorToken())
						.Append(PrintOther())
						.Append(PrintSpace())
						.Append(PrintIsKeywordToken())
						.Append(fullTypeName)
						.Append(PrintSpace())
						.Append(PrintComparer())
						.Append(PrintLogicalAndOperatorToken())
						.Append(PrintEquals())
						.Append(PrintOpenBraceToken())
						.Append(PrintComparer())
						.Append(PrintClosedBraceToken())
						.AppendLine(PrintSemicolonToken())
						.AppendLine();
				}

				source.AppendLine(PrintCompilerGenerated(2));

				if (OutputAggressiveInliningMark)
				{
					source.AppendLine(PrintAggressiveInlining(2));
				}

				source
					.Append(PrintIndenting(2))
					.Append(PrintPublicKeywordToken());

				// Not 'ref struct' and not 'readonly struct'.
				if (symbol is { TypeKind: TypeKind.Struct, IsRefLikeType: false, IsReadOnly: false })
				{
					source.Append(PrintReadOnlyKeywordToken());
				}

				source
					.Append(PrintBoolKeywordToken())
					.Append(PrintEquals())
					.Append(PrintOpenBraceToken());

				if (symbol.TypeKind == TypeKind.Struct)
				{
					source.Append(PrintInKeywordToken());
				}

				source.Append(fullTypeName);

				if (symbol.TypeKind == TypeKind.Class)
				{
					source.Append("?");
				}

				source
					.Append(PrintSpace())
					.Append(PrintOther())
					.Append(PrintClosedBraceToken())
					.Append(PrintLambdaOperatorToken());

				if (symbol.TypeKind == TypeKind.Class)
				{
					source
						.Append(PrintOther())
						.Append(PrintSpace())
						.Append(PrintIsKeywordToken())
						.Append(PrintNotKeywordToken())
						.Append(PrintNullKeywordToken())
						.Remove(source.Length - 1, 1) // Remove the last ' '.
						.Append(PrintLogicalAndOperatorToken());
				}

				string attributeStr = (
					from attribute in symbol.GetAttributes()
					where attribute.AttributeClass?.Name == nameof(AutoEqualityAttribute)
					select attribute
				).First().ToString();
				int tokenStartIndex = attributeStr.IndexOf("({");
				if (tokenStartIndex == -1)
				{
					return null;
				}

				string[] members = (
					from parameterValue in attributeStr.Substring(
						tokenStartIndex,
						attributeStr.Length - tokenStartIndex - 2
					).Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries)
					select parameterValue.Substring(1, parameterValue.Length - 2)
				).ToArray(); // Remove quote token '"'.
				if (members is not { Length: not 0 })
				{
					return null;
				}

				members[0] = members[0].Substring(2); // Remove token '{"'.

				foreach (string member in members)
				{
					if (OutputThisReference)
					{
						source
							.Append(PrintThisKeywordToken())
							.Append(PrintDotToken());
					}

					source
						.Append(member)
						.Append(PrintEqualsOperatorToken())
						.Append(PrintOther())
						.Append(PrintDotToken())
						.Append(member)
						.Append(PrintLogicalAndOperatorToken());
				}

				source
					.Remove(source.Length - 4, 4) // Remove last '&&'.
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
		private static partial string PrintBoolKeywordToken();
		private static partial string PrintNullableObjectKeywordToken();
		private static partial string PrintOverrideKeywordToken();
		private static partial string PrintEquals();
		private static partial string PrintReturnKeywordToken();
		private static partial string PrintOther();
		private static partial string PrintSpace();
		private static partial string PrintComparer();
		private static partial string PrintIsKeywordToken();
		private static partial string PrintNotKeywordToken();
		private static partial string PrintNullKeywordToken();
		private static partial string PrintInKeywordToken();
		private static partial string PrintSemicolonToken();
		private static partial string PrintLogicalAndOperatorToken();
		private static partial string PrintThisKeywordToken();
		private static partial string PrintDotToken();
		private static partial string PrintEqualsOperatorToken();
		private static partial string PrintLambdaOperatorToken();
		private static partial string PrintOpenBracketToken(int indentingCount = 0);
		private static partial string PrintClosedBracketToken(int indentingCount = 0);
		private static partial string PrintAggressiveInlining(int indentingCount = 0);
		private static partial string PrintPragmaWarningDisableCS1591();
		private static partial string PrintUsingDirectives();
		private static partial string PrintNullableEnable();
		private static partial string PrintIndenting(int indentingCount = 0);
		private static partial string PrintCompilerGenerated(int indentingCount = 0);

		private static partial IReadOnlyList<string> GetMembers(INamedTypeSymbol symbol, bool handleRecursively);
	}
}
