using System;
using System.IO;
using System.Threading.Tasks;
using Sudoku.Data.Meta;

namespace Sudoku.IO
{
	public sealed class GridWriter : IDisposable
	{
		private readonly TextWriter _writer;


		public GridWriter(string path) => _writer = new StreamWriter(path);


		public void Dispose() => _writer.Dispose();

		public void Write(Grid grid) => Write(grid, ".");

		public void Write(Grid grid, string format) =>
			_writer.Write(grid.ToString(format));

		public async Task WriteAsync(Grid grid, bool continueOnCapturedContext = false)
			=> await WriteAsync(grid, ".", continueOnCapturedContext);

		public async Task WriteAsync(
			Grid grid, string format, bool continueOnCapturedContext = false) =>
			await Task.Run(() => Write(grid, format)).ConfigureAwait(continueOnCapturedContext);
	}
}
