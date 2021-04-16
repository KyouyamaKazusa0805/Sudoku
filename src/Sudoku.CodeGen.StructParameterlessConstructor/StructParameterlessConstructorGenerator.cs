#pragma warning disable IDE0057

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Sudoku.CodeGen.StructParameterlessConstructor.Annotations;
using Sudoku.CodeGen.StructParameterlessConstructor.Extensions;
using GenericsOptions = Microsoft.CodeAnalysis.SymbolDisplayGenericsOptions;
using GlobalNamespaceStyle = Microsoft.CodeAnalysis.SymbolDisplayGlobalNamespaceStyle;
using MiscellaneousOptions = Microsoft.CodeAnalysis.SymbolDisplayMiscellaneousOptions;
using TypeQualificationStyle = Microsoft.CodeAnalysis.SymbolDisplayTypeQualificationStyle;

namespace Sudoku.CodeGen.StructParameterlessConstructor
{
	/// <summary>
	/// Defines a generator that controls generating parameterless constructor
	/// in <see langword="struct"/>s automatically.
	/// </summary>
	/// <remarks>
	/// C# 10 or later supports the feature "parameterless constructor in <see langword="struct"/>s",
	/// which allows us customize a parameterless constructor in a <see langword="struct"/>
	/// that don't effect on <see langword="default"/> expression
	/// (e.g. <see langword="default"/>(<see langword="int"/>)).
	/// </remarks>
	[Generator]
	public sealed partial class StructParameterlessConstructorGenerator : ISourceGenerator
	{
		/// <summary>
		/// Indicates whether the project uses tabs <c>'\t'</c> as indenting characters.
		/// </summary>
		private static readonly bool UsingTabsAsIndentingCharacters = true;

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

			var structNameDic = new Dictionary<string, int>();
			foreach (var structSymbol in g(context, receiver))
			{
				_ = structNameDic.TryGetValue(structSymbol.Name, out int i);
				var name = i == 0 ? structSymbol.Name : $"{structSymbol.Name}{i + 1}";
				structNameDic[structSymbol.Name] = i + 1;

				context.AddSource(
					$"{name}.ParameterlessConstructor.g.cs",
					getParameterlessCtorCode(structSymbol)
				);
			}


			static IEnumerable<INamedTypeSymbol> g(GeneratorExecutionContext context, SyntaxReceiver receiver)
			{
				var compilation = context.Compilation;

				return from candidateStruct in receiver.CandidateStructs
					   let model = compilation.GetSemanticModel(candidateStruct.SyntaxTree)
					   select model.GetDeclaredSymbol(candidateStruct)! into structSymbol
					   where structSymbol.Marks<DisallowParameterlessConstructorAttribute>()
					   select (INamedTypeSymbol)structSymbol;
			}

			static string getParameterlessCtorCode(INamedTypeSymbol structSymbol)
			{
				string namespaceName = structSymbol.ContainingNamespace.ToDisplayString();
				string fullTypeName = structSymbol.ToDisplayString(TypeFormat);
				int i = fullTypeName.IndexOf('<');
				string genericParametersList = i == -1 ? string.Empty : fullTypeName.Substring(i);

				var source = new StringBuilder()
					.AppendLine(PrintHeader())
					.AppendLine(PrintUsingDirectives())
					.AppendLine()
					.Append(PrintNamespaceKeywordToken())
					.AppendLine(namespaceName)
					.AppendLine(PrintOpenBracketToken())
					.Append(PrintIndenting(1))
					.Append(PrintPartialKeywordToken())
					.Append(PrintStructKeywordToken())
					.Append(structSymbol.Name)
					.AppendLine(genericParametersList)
					.AppendLine(PrintOpenBracketToken(1))
					.AppendLine(PrintCompilerGenerated(2))
					.Append(PrintIndenting(2))
					.Append(PrintPrivateKeywordToken())
					.Append(structSymbol.Name)
					.Append(PrintOpenBraceToken())
					.AppendLine(PrintClosedBraceToken())
					.Append(PrintOpenBracketToken(2))
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
		private static partial string PrintStructKeywordToken();
		private static partial string PrintPrivateKeywordToken();
		private static partial string PrintOpenBracketToken(int indentingCount = 0);
		private static partial string PrintClosedBracketToken(int indentingCount = 0);
		private static partial string PrintUsingDirectives();
		private static partial string PrintIndenting(int indentingCount = 0);
		private static partial string PrintCompilerGenerated(int indentingCount = 0);
	}
}
