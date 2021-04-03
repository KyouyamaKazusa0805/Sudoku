using System.Runtime.CompilerServices;

namespace System.Text.Markdown
{
	partial class Document
	{
		/// <summary>
		/// Append plain text.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <returns>The current instance.</returns>
		/// <exception cref="FormatException">
		/// Throws when the <paramref name="text"/> is <see langword="null"/>, empty or whitespaces.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Document AppendText(string text)
		{
			_innerBuilder.AppendText(text);
			return this;
		}

		/// <summary>
		/// Append a paragraph with a new line.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <returns>The current instance.</returns>
		/// <exception cref="FormatException">
		/// Throws when the <paramref name="text"/> is <see langword="null"/>, empty or whitespaces.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Document AppendParagraph(string text)
		{
			_innerBuilder.AppendParagraph(text);
			return this;
		}

		/// <summary>
		/// Append inline code block.
		/// </summary>
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
		public Document AppendInlineCodeBlock(string text, bool appendPaddingSpaces = false)
		{
			_innerBuilder.AppendInlineCodeBlock(text, appendPaddingSpaces);
			return this;
		}

		/// <summary>
		/// Append the code block.
		/// </summary>
		/// <param name="code">The inner code.</param>
		/// <param name="codelang">The code language. The default value is <c>"csharp"</c>.</param>
		/// <returns>The current instance.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Document AppendCodeBlock(string code, string? codelang = "csharp")
		{
			_innerBuilder.AppendCodeBlock(code, codelang);
			return this;
		}

		/// <summary>
		/// Append header text.
		/// </summary>
		/// <param name="text">The inner text.</param>
		/// <returns>The current instance.</returns>
		/// <exception cref="FormatException">
		/// Throws when the <paramref name="text"/> is <see langword="null"/>, empty
		/// or only contains whitespace characters.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Document AppendHeaderText(string text)
		{
			_innerBuilder.AppendHeaderText(text);
			return this;
		}

		/// <summary>
		/// Append header text, with the specified header level.
		/// </summary>
		/// <param name="level">The header level. The value should be between 1 and 6.</param>
		/// <param name="text">The inner text.</param>
		/// <returns>The current instance.</returns>
		/// <exception cref="FormatException">
		/// Throws when the <paramref name="text"/> is <see langword="null"/>, empty
		/// or only contains whitespace characters.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Document AppendHeaderText(int level, string text)
		{
			_innerBuilder.AppendHeaderText(level, text);
			return this;
		}

		/// <summary>
		/// Append bold block.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <returns>The current instance.</returns>
		/// <exception cref="FormatException">
		/// Throws when the <paramref name="text"/> is <see langword="null"/>, empty
		/// or only contains whitespace characters.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Document AppendBoldBlock(string text)
		{
			_innerBuilder.AppendText($"**{text}**");
			return this;
		}

		/// <summary>
		/// Append italic block.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <returns>The current instance.</returns>
		/// <exception cref="FormatException">
		/// Throws when the <paramref name="text"/> is <see langword="null"/>, empty
		/// or only contains whitespace characters.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Document AppendItalicBlock(string text)
		{
			_innerBuilder.AppendText($"*{text}*");
			return this;
		}

		/// <summary>
		/// Append delete block.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <returns>The current instance.</returns>
		/// <exception cref="FormatException">
		/// Throws when the <paramref name="text"/> is <see langword="null"/>, empty
		/// or only contains whitespace characters.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Document AppendDeleteBlock(string text)
		{
			_innerBuilder.AppendText($"~~{text}~~");
			return this;
		}

		/// <summary>
		/// Append a new line.
		/// </summary>
		/// <returns>The current instance.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Document AppendNewLine()
		{
			_innerBuilder.AppendNewLine();
			return this;
		}
	}
}
