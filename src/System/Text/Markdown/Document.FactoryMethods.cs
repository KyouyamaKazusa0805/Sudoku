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
			_innerBuilder.AppendMarkdownPlainText(text);
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
			_innerBuilder.AppendMarkdownParagraph(text);
			return this;
		}

		/// <summary>
		/// Append inline code block.
		/// </summary>
		/// <param name="text">The inner text.</param>
		/// <returns>The current instance.</returns>
		/// <exception cref="FormatException">
		/// Throws when the <paramref name="text"/> contains double tilde mark <c>"``"</c>.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Document AppendInlineCodeBlock(string text)
		{
			_innerBuilder.AppendMarkdownInlineCodeBlock(text);
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
			_innerBuilder.AppendMarkdownCodeBlock(code, codelang);
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
		public Document AppendHeader(string text)
		{
			_innerBuilder.AppendMarkdownHeader(text);
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
		public Document AppendHeader(int level, string text)
		{
			_innerBuilder.AppendMarkdownHeader(level, text);
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
			_innerBuilder.AppendMarkdownBoldBlock(text);
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
			_innerBuilder.AppendMarkdownItalicBlock(text);
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
			_innerBuilder.AppendMarkdownDeleteBlock(text);
			return this;
		}

		/// <summary>
		/// Append image block.
		/// </summary>
		/// <param name="description">The description.</param>
		/// <param name="uri">The URI.</param>
		/// <returns>The current instance.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Document AppendImageBlock(string? description, string uri)
		{
			_innerBuilder.AppendMarkdownImageBlock(description, uri);
			return this;
		}

		/// <summary>
		/// Append hyperlink.
		/// </summary>
		/// <param name="description">The description.</param>
		/// <param name="uri">The URI.</param>
		/// <param name="appendNewLine">
		/// A <see cref="bool"/> value indicates whether the method appends a new line
		/// at the tail of the document. The default value is <see langword="false"/>.
		/// </param>
		/// <returns>The current instance.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Document AppendHyperlink(string? description, string uri, bool appendNewLine = false)
		{
			_innerBuilder.AppendMarkdownHyperlink(description, uri, appendNewLine);
			return this;
		}

		/// <summary>
		/// Append inline LaTeX block.
		/// </summary>
		/// <param name="latex">The LaTeX expression.</param>
		/// <returns>The current instance.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Document AppendInlineLatexBlock(string latex)
		{
			_innerBuilder.AppendMarkdownInlineLatexBlock(latex);
			return this;
		}

		/// <summary>
		/// Append LaTeX block.
		/// </summary>
		/// <param name="latex">The LaTeX expression.</param>
		/// <returns>The current instance.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Document AppendLatexBlock(string latex)
		{
			_innerBuilder.AppendMarkdownLatexBlock(latex);
			return this;
		}

		/// <summary>
		/// Append a new line.
		/// </summary>
		/// <returns>The current instance.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Document AppendNewLine()
		{
			_innerBuilder.AppendMarkdownNewLine();
			return this;
		}
	}
}
