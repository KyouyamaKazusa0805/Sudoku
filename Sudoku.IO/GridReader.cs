using System;
using System.IO;
using System.Threading.Tasks;
using Sudoku.Data.Meta;

namespace Sudoku.IO
{
	public sealed class GridReader : IDisposable
	{
		private readonly TextReader _reader;


		public GridReader(string path) => _reader = new StreamReader(path);


		public void Dispose() => _reader.Dispose();

		public Grid? ReadLine()
		{
			string? result = _reader.ReadLine();
			return result is null ? null : Grid.Parse(result);
		}

		public Grid? ReadToEnd()
		{
			string? result = _reader.ReadToEnd();
			return result is null ? null : Grid.Parse(result);
		}

		public async Task<Grid?> ReadLineAsync(bool continueOnCapturedContext = false) =>
			await Task.Run(() => ReadLine()).ConfigureAwait(continueOnCapturedContext);

		public async Task<Grid?> ReadToEndAsync(bool continueOnCapturedContext = false) =>
			await Task.Run(() => ReadLine()).ConfigureAwait(continueOnCapturedContext);
	}
}
