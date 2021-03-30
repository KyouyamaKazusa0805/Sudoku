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
		public Document AppendText(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
			{
				throw new FormatException("The text shouldn't be an empty string, whitespaces or null.");
			}

			_innerBuilder.Append(text);

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
		public Document AppendParagraph(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
			{
				throw new FormatException("The text shouldn't be an empty string, whitespaces or null.");
			}

			_innerBuilder.Append(text).AppendLine().AppendLine();

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
		public Document AppendInlineCodeBlock(string text)
		{
			if (text.Contains(MarkdownSymbols.InlineCodeBlockStartDouble))
			{
				throw new FormatException(
					$"The text can't contain double tilde mark \"{MarkdownSymbols.InlineCodeBlockStartDouble}\"."
				);
			}

			_innerBuilder.Append(
				text.Contains(MarkdownSymbols.InlineCodeBlockStartSingle)
				? _innerBuilder
				.Append(MarkdownSymbols.InlineCodeBlockStartDouble)
				.Append(text)
				.Append(MarkdownSymbols.InlineCodeBlockEndDouble)
				: _innerBuilder
				.Append(MarkdownSymbols.InlineCodeBlockStartSingle)
				.Append(text)
				.Append(MarkdownSymbols.InlineCodeBlockEndSingle)
			);

			return this;
		}

		/// <summary>
		/// Append the code block.
		/// </summary>
		/// <param name="code">The inner code.</param>
		/// <param name="codelang">The code language. The default value is <c>"csharp"</c>.</param>
		/// <returns>The current instance.</returns>
		public Document AppendCodeBlock(string code, string? codelang = "csharp")
		{
			_innerBuilder
				.Append(MarkdownSymbols.CodeBlockStart).Append(codelang ?? string.Empty).AppendLine()
				.Append(code).AppendLine()
				.Append(MarkdownSymbols.CodeBlockEnd).AppendLine().AppendLine();

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
		public Document AppendHeaderText(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
			{
				throw new FormatException("The text shouldn't be an empty string, whitespaces or null.");
			}

			_innerBuilder.Append(MarkdownSymbols.H1).Append(text).AppendLine().AppendLine();

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
		public Document AppendHeaderText(int level, string text)
		{
			if (string.IsNullOrWhiteSpace(text))
			{
				throw new FormatException("The text shouldn't be an empty string, whitespaces or null.");
			}

			_innerBuilder.Append(level switch
			{
				1 => MarkdownSymbols.H1,
				2 => MarkdownSymbols.H2,
				3 => MarkdownSymbols.H3,
				4 => MarkdownSymbols.H4,
				5 => MarkdownSymbols.H5,
				6 => MarkdownSymbols.H6
			}).Append(text).AppendLine().AppendLine();

			return this;
		}

		/// <summary>
		/// Append a new line.
		/// </summary>
		/// <returns>The current instance.</returns>
		public Document AppendNewLine()
		{
			_innerBuilder.AppendLine().AppendLine();

			return this;
		}
	}
}
