#pragma warning disable IDE0057

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Sudoku.CodeGen.Deconstruction.Annotations;
using Sudoku.CodeGen.Deconstruction.Extensions;
using GenericsOptions = Microsoft.CodeAnalysis.SymbolDisplayGenericsOptions;
using GlobalNamespaceStyle = Microsoft.CodeAnalysis.SymbolDisplayGlobalNamespaceStyle;
using MiscellaneousOptions = Microsoft.CodeAnalysis.SymbolDisplayMiscellaneousOptions;
using TypeQualificationStyle = Microsoft.CodeAnalysis.SymbolDisplayTypeQualificationStyle;

namespace Sudoku.CodeGen.Deconstruction
{
	/// <summary>
	/// Provides a generator that generates the deconstruction methods.
	/// </summary>
	[Generator]
	public sealed partial class DeconstructMethodGenerator : ISourceGenerator
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
			foreach (var typeSymbol in g(context, receiver))
			{
				_ = nameDic.TryGetValue(typeSymbol.Name, out int i);
				string name = i == 0 ? typeSymbol.Name : $"{typeSymbol.Name}{(i + 1).ToString()}";
				nameDic[typeSymbol.Name] = i + 1;
				context.AddSource($"{name}.DeconstructionMethods.g.cs", getDeconstructionCode(typeSymbol));
			}


			static IEnumerable<INamedTypeSymbol> g(in GeneratorExecutionContext context, SyntaxReceiver receiver)
			{
				var compilation = context.Compilation;

				return
					from candidateType in receiver.Candidates
					let model = compilation.GetSemanticModel(candidateType.SyntaxTree)
					select model.GetDeclaredSymbol(candidateType)! into typeSymbol
					where typeSymbol.Marks<AutoDeconstructAttribute>()
					select typeSymbol;
			}

			static string getDeconstructionCode(INamedTypeSymbol symbol)
			{
				string namespaceName = symbol.ContainingNamespace.ToDisplayString();
				var members = GetMembers(symbol, handleRecursively: false);
				var possibleArgs = from x in members select (Info: x, Param: $"out {x.Type} {x.ParameterName}");
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

				var attributeData = (
					from attribute in symbol.GetAttributes()
					where attribute.AttributeClass?.Name == nameof(AutoDeconstructAttribute)
					select attribute
				).ToArray();

				foreach (var attribute in attributeData)
				{
					var methodSymbol = attribute.AttributeConstructor;
					string attributeStr = attribute.ToString();
					int tokenStartIndex = attributeStr.IndexOf("({");
					if (tokenStartIndex == -1)
					{
						// Error.

						continue;
					}

					string[] memberValues = (
						from parameterValue in attributeStr.Substring(
							tokenStartIndex,
							attributeStr.Length - tokenStartIndex - 2
						).Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries)
						select parameterValue.Substring(1, parameterValue.Length - 2)
					).ToArray(); // Remove quote token '"'.

					memberValues[0] = memberValues[0].Substring(2); // Remove token '{"'.

					bool isError = false;
					foreach (string member in memberValues)
					{
						string name = member;
						if (possibleArgs.All(pair => pair.Info.Name != name))
						{
							// TODO: Raise an error to tell the user the name can't be referenced and parsed.

							isError = true;
							break;
						}
					}
					if (isError)
					{
						continue;
					}

					if (OutputAggressiveInliningMark)
					{
						source.AppendLine(PrintAggressiveInlining(2));
					}

					source
						.AppendLine(PrintCompilerGenerated(2))
						.Append(PrintIndenting(2))
						.Append(PrintPublicKeywordToken());

					if (symbol is { TypeKind: TypeKind.Struct, IsReadOnly: false })
					{
						source.Append(PrintReadOnlyKeywordToken());
					}

					source
						.Append(PrintVoidKeywordToken())
						.Append(PrintDeconstructName())
						.Append(PrintOpenBraceToken())
						.Append(
							string.Join(
								Separator,
								from member in memberValues
								select possibleArgs.First(p => p.Info.Name == member).Param
							)
						)
						.AppendLine(PrintClosedBraceToken())
						.Append(PrintOpenBracketToken(2));

					foreach (string member in memberValues)
					{
						string paramName = possibleArgs.First(p => p.Info.Name == member).Info.ParameterName;

						source
							.AppendLine()
							.Append(PrintIndenting(3))
							.Append(paramName)
							.Append(PrintEqualsToken());

						if (OutputThisReference)
						{
							source
								.Append(PrintThisKeywordToken())
								.Append(PrintDotToken());
						}

						source
							.Append(member)
							.Append(PrintSemicolonToken());
					}

					source.AppendLine().AppendLine(PrintClosedBracketToken(2)).AppendLine();
				}

				if (attributeData.Length != 0)
				{
					source.Remove(source.Length - 2, 2); // Remove the last '\r\n'.
				}

				return source
					.AppendLine(PrintClosedBracketToken(1))
					.AppendLine(PrintClosedBracketToken())
					.ToString();
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
		private static partial string PrintVoidKeywordToken();
		private static partial string PrintDeconstructName();
		private static partial string PrintReadOnlyKeywordToken();
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
		private static partial string PrintAggressiveInlining(int indentingCount = 0);
		private static partial string PrintCompilerGenerated(int indentingCount = 0);


		private static partial IReadOnlyList<SymbolInfo> GetMembers(INamedTypeSymbol symbol, bool handleRecursively);
	}
}
