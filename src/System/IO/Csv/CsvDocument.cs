using System.Runtime.CompilerServices;
using Microsoft.VisualBasic.FileIO;

namespace System.IO.Csv
{
	/// <summary>
	/// Introduces a <c>*.csv</c> document.
	/// </summary>
	public sealed partial class CsvDocument : IDisposable
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
		/// Initializes the <see cref="CsvDocument"/> with the specified path, with two optional parameters.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="withHeader">Indicates whether the table contain the table header at the first line.</param>
		/// <param name="delimiter">
		/// The delimiter. This delimiter will be processed in splitting the line and
		/// separating to the multiple values. The default value is <see langword="default"/>(<see cref="char"/>)
		/// (i.e. <c>'\0'</c>) as the undefined character.
		/// </param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public CsvDocument(string path, bool withHeader = false, char delimiter = '\0')
		{
			Path = path;
			WithHeader = withHeader;

			_parser = delimiter == '\0'
				? new(Path) { HasFieldsEnclosedInQuotes = true }
				: new(Path)
				{
					TextFieldType = FieldType.Delimited,
					Delimiters = new[] { delimiter.ToString() },
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
		public void Close() => ((IDisposable)this).Dispose();

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
		public string[]? ReadLine() => _parser.ReadFields();

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>An enumerator that iterates through the collection.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Enumerator GetEnumerator() => new(_parser);

		/// <summary>
		/// Returns an enumerator that iterates asynchronously through the collection.
		/// </summary>
		/// <returns>An enumerator that iterates asynchronously through the collection.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public AsyncEnumerator GetAsyncEnumerator() => new(_parser);

		/// <inheritdoc/>
		/// <exception cref="ObjectDisposedException">Throws when the object has been already disposed.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void IDisposable.Dispose()
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
	}
}
