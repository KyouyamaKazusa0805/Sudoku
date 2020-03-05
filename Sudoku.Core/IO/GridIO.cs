using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Sudoku.Data;

namespace Sudoku.IO
{
	/// <summary>
	/// Provides with I/O operations for <see cref="Grid"/>s and <see cref="IReadOnlyGrid"/>s.
	/// </summary>
	/// <seealso cref="Grid"/>
	/// <seealso cref="IReadOnlyGrid"/>
	[DebuggerStepThrough]
	public static class GridIO
	{
		/// <summary>
		/// Write the grid to the file.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The grid.</param>
		/// <param name="path">The path.</param>
		/// <param name="format">The format.</param>
		public static void WriteToFile(this IReadOnlyGrid @this, string path, string format)
		{
			using var sw = new StreamWriter(path);
			sw.Write(@this.ToString(format));
			sw.Close();
		}

		/// <summary>
		/// Write the grid to the file.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The grid.</param>
		/// <param name="path">The path.</param>
		/// <param name="format">The format.</param>
		/// <returns>The task of this operation.</returns>
		public static async Task WriteToFileAsync(this IReadOnlyGrid @this, string path, string format)
		{
			using var sw = new StreamWriter(path);
			await sw.WriteAsync(@this.ToString(format));
			sw.Close();
		}
	}
}
