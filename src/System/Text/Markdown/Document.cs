using System.Extensions;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Sudoku.CodeGen.Equality.Annotations;
using Sudoku.CodeGen.HashCode.Annotations;
using Sudoku.DocComments;

namespace System.Text.Markdown
{
	/// <summary>
	/// Provides a markdown document.
	/// </summary>
	[AutoHashCode(nameof(ResultStr))]
	[AutoEquality(nameof(ResultStr))]
	public sealed partial class Document : IEquatable<Document>
	{
		/// <summary>
		/// Indicates the default culture name.
		/// </summary>
		private const string DefaultCultureName = "en-US";


		/// <summary>
		/// Indicates the inner builder.
		/// </summary>
		private readonly StringBuilder _innerBuilder = new();


		/// <inheritdoc cref="DefaultConstructor"/>
		private Document()
		{
		}


		/// <summary>
		/// Indicates the result string.
		/// </summary>
		private string ResultStr => _innerBuilder.ToString();


		/// <inheritdoc/>
		public override string ToString() => ResultStr;

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
			cultureInfo ??= new(DefaultCultureName);

			switch (cultureInfo.Name)
			{
				case DefaultCultureName:
				{
					for (int index = 0; index < _innerBuilder.Length - 1; index++)
					{
						char currentChar = _innerBuilder[index], nextChar = _innerBuilder[index + 1];
						if (CharEx.IsAsciiPuncuation(currentChar) && !CharEx.IsAsciiPuncuation(nextChar))
						{
							_innerBuilder.Insert(++index, ' ');
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

			await File.WriteAllTextAsync(
				path: File.Exists(path) ? getAvailablePath(path) : $"{path}.md",
				contents: ToString(),
				cancellationToken
			);

			static string getAvailablePath(string path)
			{
				string fileNameToCheck;
				for (int i = 1; File.Exists(fileNameToCheck = $"{path}_{i.ToString()}.md"); i++) ;

				return fileNameToCheck;
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
