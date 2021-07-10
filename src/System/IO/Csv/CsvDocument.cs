using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;

namespace System.IO.Csv
{
	/// <summary>
	/// Introduces a <c>*.csv</c> document.
	/// </summary>
	public sealed class CsvDocument : IDisposable
	{
		/// <summary>
		/// Indicates the reader of the document.
		/// </summary>
		private readonly TextFieldParser _parser;


		/// <summary>
		/// Indicates the value that suggests whether the document instance has been already disposed.
		/// </summary>
		private bool _hasBeenDisposed;


		/// <summary>
		/// Initializes the <see cref="CsvDocument"/> with the specified path.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="withHeader">Indicates whether the table contain the table header at the first line.</param>
		/// <param name="delimiter">The delimiter.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public CsvDocument(string path, bool withHeader = false, string? delimiter = null)
		{
			Path = path;
			WithHeader = withHeader;

			_parser = delimiter is null
				? new(Path) { HasFieldsEnclosedInQuotes = true }
				: new(Path)
				{
					TextFieldType = FieldType.Delimited,
					Delimiters = new[] { delimiter },
					HasFieldsEnclosedInQuotes = true
				};
		}


		/// <summary>
		/// Indicates whether the table contains the header.
		/// </summary>
		public bool WithHeader { get; }

		/// <summary>
		/// Indicates the current line number.
		/// </summary>
		public long LineNumber
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _parser.LineNumber;
		}

		/// <summary>
		/// Indicates the path.
		/// </summary>
		public string Path { get; }

		/// <summary>
		/// Indicates the delimiter.
		/// </summary>
		public string? Delimiter { get; }


		/// <summary>
		/// Closes the file, and releases the memory.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Close() => Dispose();

		/// <inheritdoc/>
		/// <exception cref="ObjectDisposedException">Throws when the object has been already disposed.</exception>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Dispose()
		{
			if (_hasBeenDisposed)
			{
				throw new ObjectDisposedException(
					"The object has been already disposed.",
					new InvalidOperationException(
						"Can't release an object whose memory has been already disposed."
					)
				);
			}

			_parser.Close();

			_hasBeenDisposed = true;
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Try to get the fields of the table if worth. The table may contain those fields or not
		/// displayed at the first line.
		/// </summary>
		/// <returns>The result value. The value can be:
		/// <list type="table">
		/// <item>
		/// <term><see langword="null"/></term>
		/// <description>
		/// The property <see cref="WithHeader"/> is <see langword="false"/>, or the table is empty.
		/// </description>
		/// </item>
		/// <item>
		/// <term>The string of the field names</term>
		/// <description>Otherwise.</description>
		/// </item>
		/// </list>
		/// </returns>
		/// <seealso cref="WithHeader"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string[]? GetFields() => WithHeader && _parser.ReadFields() is { } result ? result : null;

		/// <summary>
		/// Reads a line of values.
		/// </summary>
		/// <returns>The values.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string[]? ReadLine()
		{
			if (WithHeader && LineNumber == 0 && !_parser.EndOfData)
			{
				_parser.ReadFields();
			}

			return _parser.ReadFields();
		}

		/// <summary>
		/// Reads the whole document, and returns the separated result.
		/// </summary>
		/// <returns>The list of lines of fields.</returns>
		public IEnumerable<string[]?> ReadToEnd()
		{
			while (!_parser.EndOfData)
			{
				yield return _parser.ReadFields();
			}
		}

		/// <summary>
		/// Reads the whole document, and returns the separated result asynchronously.
		/// </summary>
		/// <returns>The list of lines of fields.</returns>
		/// <remarks>
		/// The same as <see cref="ReadToEnd"/>, this method can also provide iteration. However,
		/// due to the return type <see cref="IAsyncEnumerable{T}"/>, you should use the method to iterate
		/// asynchronously, such as:
		/// <code>
		/// await foreach (string[] fields in instance.ReadToEndAsync())
		/// {
		///     // Do whatever you like.
		/// }
		/// </code>
		/// </remarks>
		/// <seealso cref="ReadToEnd"/>
		public async IAsyncEnumerable<string[]?> ReadToEndAsync()
		{
			while (!_parser.EndOfData)
			{
				yield return await f(_parser);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static ValueTask<string[]?> f(TextFieldParser parser) => new(parser.ReadFields());
		}

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override string? ToString() => _parser.ReadToEnd();
	}
}
