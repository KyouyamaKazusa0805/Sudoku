using System.IO;
using System.Threading.Tasks;

namespace Sudoku.Solving.Extensions
{
	/// <summary>
	/// Provides with operations to output to files for <see cref="AnalysisResult"/>s.
	/// </summary>
	/// <seealso cref="AnalysisResult"/>
	public static class AnalysisResultIO
	{
		/// <summary>
		/// Output the text to file.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The analysis result.</param>
		/// <param name="path">The path.</param>
		/// <param name="format">The format.</param>
		public static void WriteToFile(this AnalysisResult @this, string path, string format)
		{
			using var sw = new StreamWriter(path);
			sw.Write(@this.ToString(format));
			sw.Close();
		}

		/// <summary>
		/// Output the text to file.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The analysis result.</param>
		/// <param name="path">The path.</param>
		/// <param name="format">The format.</param>
		/// <returns>The task of this operation.</returns>
		public static async Task WriteToFileAsync(this AnalysisResult @this, string path, string format)
		{
			using var sw = new StreamWriter(path);
			await sw.WriteAsync(@this.ToString(format));
			sw.Close();
		}
	}
}
