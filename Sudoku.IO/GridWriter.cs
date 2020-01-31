using System;
using System.IO;
using System.Threading.Tasks;
using Sudoku.Data.Meta;

namespace Sudoku.IO
{
	/// <summary>
	/// Provides a grid text writer.
	/// </summary>
	public sealed class GridWriter : IDisposable
	{
		/// <summary>
		/// Indicates the inner text writer.
		/// </summary>
		private readonly TextWriter _writer;


		/// <summary>
		/// Initializes an instance with the file path.
		/// </summary>
		/// <param name="path">The file path.</param>
		public GridWriter(string path) => _writer = new StreamWriter(path);


		/// <inheritdoc/>
		public void Dispose() => _writer.Dispose();

		/// <summary>
		/// Write the grid.
		/// </summary>
		/// <param name="grid">The grid to write.</param>
		public void Write(Grid grid) => Write(grid, ".");

		/// <summary>
		/// Write the grid with the specified format.
		/// </summary>
		/// <param name="grid">The grid to write.</param>
		/// <param name="format">The format.</param>
		public void Write(Grid grid, string format) =>
			_writer.Write(grid.ToString(format));

		/// <summary>
		/// Write the grid in an asynchronized way.
		/// </summary>
		/// <param name="grid">The grid to write.</param>
		/// <param name="continueOnCapturedContext">
		/// <see langword="true"/> to attempt to marshal the continuation back to the
		/// original context captured; otherwise, <see langword="false"/>.
		/// </param>
		/// <returns>The task.</returns>
		public async Task WriteAsync(Grid grid, bool continueOnCapturedContext = false)
			=> await WriteAsync(grid, ".", continueOnCapturedContext);

		/// <summary>
		/// Write the grid with the specified format in an asynchronized way.
		/// </summary>
		/// <param name="grid">The grid to write.</param>
		/// <param name="format">The format.</param>
		/// <param name="continueOnCapturedContext">
		/// <see langword="true"/> to attempt to marshal the continuation back to the
		/// original context captured; otherwise, <see langword="false"/>.
		/// </param>
		/// <returns>The task.</returns>
		public async Task WriteAsync(
			Grid grid, string format, bool continueOnCapturedContext = false) =>
			await Task.Run(() => Write(grid, format)).ConfigureAwait(continueOnCapturedContext);
	}
}
