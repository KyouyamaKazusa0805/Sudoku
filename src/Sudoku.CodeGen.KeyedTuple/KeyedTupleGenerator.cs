using System;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Sudoku.CodeGen.KeyedTuple
{
	/// <summary>
	/// Define a keyed tuple generator.
	/// </summary>
	[Generator]
	public sealed partial class KeyedTupleGenerator : ISourceGenerator
	{
		/// <summary>
		/// Indicates the separator that is used for separating multiple values.
		/// </summary>
		private const string CommaToken = ", ";


		/// <summary>
		/// Indicates whether the project uses tabs <c>'\t'</c> as indenting characters.
		/// </summary>
		private static readonly bool UsingTabsAsIndentingCharacters = true;

		/// <summary>
		/// Indicates the new line character in this current environment.
		/// </summary>
		private static readonly string NewLine = Environment.NewLine;


		/// <inheritdoc/>
		public void Execute(GeneratorExecutionContext context)
		{
			var sb = new StringBuilder();
			for (int length = 2; length <= 4; length++)
			{
				sb.Clear();
				sb.AppendLine(PrintHeader());
				sb.AppendLine(PrintPragmaWarningDisableCS8509());
				sb.AppendLine();
				sb.AppendLine(PrintNullableEnable());
				sb.AppendLine();
				sb.AppendLine(PrintUsingDirectives());
				sb.AppendLine();
				sb.AppendLine(PrintNamespace());
				sb.AppendLine(PrintOpenBracketToken());
				sb.AppendLine(PrintRecordDocComment(length));
				sb.AppendLine(PrintCompilerGenerated());
				sb.AppendLine(PrintRecordStatement(length));
				sb.AppendLine(PrintOpenBracketToken(1));
				sb.AppendLine(PrintUserDefinedConstructorDocComment(length));
				sb.AppendLine(PrintUserDefinedConstructor(length));
				sb.AppendLine(PrintOpenBracketToken(2));
				sb.AppendLine(PrintClosedBracketToken(2));
				sb.AppendLine();
				sb.AppendLine();
				sb.AppendLine(PrintInheritDoc());
				sb.AppendLine(PrintLength(length));
				sb.AppendLine();
				sb.AppendLine();
				sb.AppendLine(PrintInheritDoc());
				sb.AppendLine(PrintIndexerWithValue(length));
				sb.AppendLine();
				sb.AppendLine();
				sb.AppendLine(PrintInheritDoc());
				sb.AppendLine(PrintToStringWithValue());
				sb.AppendLine(PrintClosedBracketToken(1));
				sb.AppendLine(PrintClosedBracketToken());

				context.AddSource(
					hintName: $"KeyedTuple_{length.ToString()}.cs",
					sourceText: SourceText.From(
						text: sb.ToString(),
						encoding: Encoding.UTF8
					)
				);
			}
		}

		/// <inheritdoc/>
		public void Initialize(GeneratorInitializationContext context)
		{
		}


		private static partial string PrintOpenBracketToken(int indentingCount = 0);
		private static partial string PrintClosedBracketToken(int indentingCount = 0);
		private static partial string PrintHeader();
		private static partial string PrintPragmaWarningDisableCS8509();
		private static partial string PrintUsingDirectives();
		private static partial string PrintNullableEnable();
		private static partial string PrintNamespace();
		private static partial string PrintCompilerGenerated();
		private static partial string PrintRecordDocComment(int length);
		private static partial string PrintRecordStatement(int length);
		private static partial string PrintUserDefinedConstructorDocComment(int length);
		private static partial string PrintUserDefinedConstructor(int length);
		private static partial string PrintInheritDoc();
		private static partial string PrintLength(int length);
		private static partial string PrintIndexerWithValue(int length);
		private static partial string PrintIndexerValue(bool withIndent, int length);
		private static partial string PrintToStringWithValue();
	}
}
