using System;
using System.IO;
using System.Threading.Tasks;
using Sudoku.Data.Meta;

namespace Sudoku.IO
{
	/// <summary>
	/// Provides a grid text reader.
	/// </summary>
	public sealed class GridReader : IDisposable
	{
		/// <summary>
		/// Indicates the inner text reader instance.
		/// </summary>
		private readonly TextReader _reader;


		/// <summary>
		/// Initializes an instance with a path.
		/// </summary>
		/// <param name="path">The path.</param>
		public GridReader(string path) => _reader = new StreamReader(path);


		/// <inheritdoc/>
		public void Dispose() => _reader.Dispose();

		/// <summary>
		/// Read a line of all text, and parse it to grid.
		/// </summary>
		/// <returns>
		/// The result after parsed. If failed to parse, the value will be
		/// <c>null</c>.
		/// </returns>
		public Grid? ReadLine()
		{
			string? result = _reader.ReadLine();
			return result is null ? null : Grid.Parse(result);
		}

		/// <summary>
		/// Read all text in a file, and parse it to grid.
		/// </summary>
		/// <returns>
		/// The result after parsed. If failed to parsem the value will be
		/// <c>null</c>.
		/// </returns>
		public Grid? ReadToEnd()
		{
			string? result = _reader.ReadToEnd();
			return result is null ? null : Grid.Parse(result);
		}

		/// <summary>
		/// Read a line of all text, and parse it to grid in an asynchronized way.
		/// </summary>
		/// <param name="continueOnCapturedContext">
		/// <c>true</c> to attempt to marshal the continuation back to the
		/// original context captured; otherwise, <c>false</c>.
		/// </param>
		/// <returns>The task.</returns>
		public async Task<Grid?> ReadLineAsync(bool continueOnCapturedContext = false) =>
			await Task.Run(() => ReadLine()).ConfigureAwait(continueOnCapturedContext);

		/// <summary>
		/// Read all text in a file, and parse it to grid in an asynchronized way.
		/// </summary>
		/// <param name="continueOnCapturedContext">
		/// <c>true</c> to attempt to marshal the continuation back to the
		/// original context captured; otherwise, <c>false</c>.
		/// </param>
		/// <returns>The task.</returns>
		public async Task<Grid?> ReadToEndAsync(bool continueOnCapturedContext = false) =>
			await Task.Run(() => ReadLine()).ConfigureAwait(continueOnCapturedContext);
	}
}
