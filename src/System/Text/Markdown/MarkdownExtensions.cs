using System.Runtime.CompilerServices;

namespace System.Text.Markdown
{
	/// <summary>
	/// Provides extension methods for markdown utility on <see cref="StringBuilder"/> and
	/// <see cref="ValueStringBuilder"/>.
	/// </summary>
	/// <seealso cref="StringBuilder"/>
	/// <seealso cref="ValueStringBuilder"/>
	public static class MarkdownExtensions
	{
		/// <summary>
		/// Append plain text.
		/// </summary>
		/// <param name="this">The current instance.</param>
		/// <param name="text">The text.</param>
		/// <returns>The current instance.</returns>
		/// <exception cref="FormatException">
		/// Throws when the <paramref name="text"/> is <see langword="null"/>, empty or whitespaces.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static StringBuilder AppendText(this StringBuilder @this, string text) =>
			string.IsNullOrWhiteSpace(text)
			? throw new FormatException("The text shouldn't be an empty string, whitespaces or null.")
			: @this.Append(text);

		/// <summary>
		/// Append a paragraph with a new line.
		/// </summary>
		/// <param name="this">The current instance.</param>
		/// <param name="text">The text.</param>
		/// <returns>The current instance.</returns>
		/// <exception cref="FormatException">
		/// Throws when the <paramref name="text"/> is <see langword="null"/>, empty or whitespaces.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static StringBuilder AppendParagraph(this StringBuilder @this, string text) =>
			string.IsNullOrWhiteSpace(text)
			? throw new FormatException("The text shouldn't be an empty string, whitespaces or null.")
			: @this.Append(text).AppendLine().AppendLine();

		/// <summary>
		/// Append inline code block.
		/// </summary>
		/// <param name="this">The current instance.</param>
		/// <param name="text">The inner text.</param>
		/// <param name="appendPaddingSpaces">
		/// Indicates whether the block will append padding spaces surrounded
		/// the block. If <see langword="true"/>,
		/// the method will append a trailing space and a leading space to the block.
		/// The default value is <see langword="false"/>.
		/// </param>
		/// <returns>The current instance.</returns>
		/// <exception cref="FormatException">
		/// Throws when the <paramref name="text"/> contains double tilde mark <c>"``"</c>.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static StringBuilder AppendInlineCodeBlock(this StringBuilder @this, string text, bool appendPaddingSpaces) =>
			text.Contains(MarkdownSymbols.InlineCodeBlockStartDouble)
			? throw new FormatException(
				$"The text can't contain double tilde mark \"{MarkdownSymbols.InlineCodeBlockStartDouble}\"."
			)
			: (text.Contains(MarkdownSymbols.InlineCodeBlockStartSingle), appendPaddingSpaces) switch
			{
				(true, true) => @this
					.Append(' ')
					.Append(MarkdownSymbols.InlineCodeBlockStartDouble)
					.Append(text)
					.Append(MarkdownSymbols.InlineCodeBlockEndDouble)
					.Append(' '),
				(true, false) => @this
					.Append(MarkdownSymbols.InlineCodeBlockStartDouble)
					.Append(text)
					.Append(MarkdownSymbols.InlineCodeBlockEndDouble),
				(false, true) => @this
					.Append(' ')
					.Append(MarkdownSymbols.InlineCodeBlockStartSingle)
					.Append(text)
					.Append(MarkdownSymbols.InlineCodeBlockEndSingle)
					.Append(' '),
				_ => @this
					.Append(MarkdownSymbols.InlineCodeBlockStartSingle)
					.Append(text)
					.Append(MarkdownSymbols.InlineCodeBlockEndSingle)
			};

		/// <summary>
		/// Append the code block.
		/// </summary>
		/// <param name="this">The current instance.</param>
		/// <param name="code">The inner code.</param>
		/// <param name="codelang">The code language.</param>
		/// <returns>The current instance.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static StringBuilder AppendCodeBlock(this StringBuilder @this, string code, string? codelang) =>
			@this
				.Append(MarkdownSymbols.CodeBlockStart).Append(codelang ?? string.Empty).AppendLine()
				.Append(code).AppendLine()
				.Append(MarkdownSymbols.CodeBlockEnd).AppendLine().AppendLine();

		/// <summary>
		/// Append header text.
		/// </summary>
		/// <param name="this">The current instance.</param>
		/// <param name="text">The inner text.</param>
		/// <returns>The current instance.</returns>
		/// <exception cref="FormatException">
		/// Throws when the <paramref name="text"/> is <see langword="null"/>, empty
		/// or only contains whitespace characters.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static StringBuilder AppendHeaderText(this StringBuilder @this, string text) =>
			string.IsNullOrWhiteSpace(text)
			? throw new FormatException("The text shouldn't be an empty string, whitespaces or null.")
			: @this.Append(MarkdownSymbols.H1).Append(text).AppendLine().AppendLine();

		/// <summary>
		/// Append header text, with the specified header level.
		/// </summary>
		/// <param name="this">The current instance.</param>
		/// <param name="level">The header level. The value should be between 1 and 6.</param>
		/// <param name="text">The inner text.</param>
		/// <returns>The current instance.</returns>
		/// <exception cref="FormatException">
		/// Throws when the <paramref name="text"/> is <see langword="null"/>, empty
		/// or only contains whitespace characters.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static StringBuilder AppendHeaderText(this StringBuilder @this, int level, string text) =>
			string.IsNullOrWhiteSpace(text)
			? throw new FormatException("The text shouldn't be an empty string, whitespaces or null.")
			: @this.Append(level switch
			{
				1 => MarkdownSymbols.H1,
				2 => MarkdownSymbols.H2,
				3 => MarkdownSymbols.H3,
				4 => MarkdownSymbols.H4,
				5 => MarkdownSymbols.H5,
				6 => MarkdownSymbols.H6
			}).Append(text).AppendLine().AppendLine();

		/// <summary>
		/// Append a new line.
		/// </summary>
		/// <param name="this">The current instance.</param>
		/// <returns>The current instance.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static StringBuilder AppendNewLine(this StringBuilder @this) => @this.AppendLine().AppendLine();
	}
}
