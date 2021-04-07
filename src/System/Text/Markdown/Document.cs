using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Sudoku.DocComments;

namespace System.Text.Markdown
{
	/// <summary>
	/// Provides a markdown document.
	/// </summary>
	public sealed partial class Document : IEquatable<Document>
	{
		/// <summary>
		/// Indicates the inner builder.
		/// </summary>
		private readonly StringBuilder _innerBuilder = new();


		/// <inheritdoc cref="DefaultConstructor"/>
		private Document()
		{
		}


		/// <inheritdoc/>
		public override bool Equals(object? obj) => Equals(obj as Document);

		/// <inheritdoc/>
		public bool Equals(Document? other) => other is not null && _innerBuilder.Equals(other._innerBuilder);

		/// <inheritdoc/>
		public override int GetHashCode() => _innerBuilder.ToString().GetHashCode();

		/// <inheritdoc/>
		public override string ToString() => _innerBuilder.ToString();

		/// <summary>
		/// Format the document.
		/// </summary>
		/// <param name="cultureInfo">
		/// The culture information instance. If <see langword="null"/>, the parameter will be
		/// re-initialized by "en-us".
		/// </param>
		/// <returns>The current document.</returns>
		public Document Format(CultureInfo? cultureInfo)
		{
			cultureInfo ??= new("en-us");

			switch (cultureInfo.Name)
			{
				case "en-US":
				{
					for (int index = 0; index < _innerBuilder.Length - 1; index++)
					{
						char currentChar = _innerBuilder[index], nextChar = _innerBuilder[index + 1];
						if (char.IsPunctuation(currentChar) && !char.IsPunctuation(nextChar)
							&& currentChar is not (' ' or '(' or '[' or '{' or '<' or '*' or '_' or '`'))
						{
							_innerBuilder.Insert(index + 1, ' ');
							index++;
						}
					}

					goto default;
				}
				default:
				{
					// Do nothing.
					return this;
				}
			}
		}

		/// <summary>
		/// Save the file asynchronously.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="cancellationToken">The cancellation token that is used for cancelling the task.</param>
		/// <returns>The task of the operation.</returns>
		public async ValueTask SaveAsync(string path, CancellationToken cancellationToken = default)
		{
			string dir = Path.GetDirectoryName(path)!;
			if (!Directory.Exists(dir))
			{
				Directory.CreateDirectory(dir);
			}

			if (File.Exists(path))
			{
				int i = 1;
				string fileNameToCheck;
				while (File.Exists(fileNameToCheck = $"{path}_{i.ToString()}.md"))
				{
					i++;
				}

				await File.WriteAllTextAsync(fileNameToCheck, ToString(), cancellationToken);
			}
			else
			{
				await File.WriteAllTextAsync($"{path}.md", ToString(), cancellationToken);
			}
		}


		/// <summary>
		/// Creates an empty markdown document.
		/// </summary>
		/// <returns>The empty markdown document instance.</returns>
		public static Document Create() => new();


		/// <inheritdoc cref="Operators.operator =="/>
		public static bool operator ==(Document left, Document right) => left.Equals(right);

		/// <inheritdoc cref="Operators.operator !="/>
		public static bool operator !=(Document left, Document right) => !(left == right);
	}
}
