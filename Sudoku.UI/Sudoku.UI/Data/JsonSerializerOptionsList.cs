using System.Text.Json;

namespace Sudoku.UI.Data
{
	/// <summary>
	/// Provides a list of <see cref="JsonSerializerOptions"/> instance used.
	/// </summary>
	public static class JsonSerializerOptionsList
	{
		/// <summary>
		/// Indicates the options that writes indented.
		/// </summary>
		public static readonly JsonSerializerOptions WithIndenting = new() { WriteIndented = true };
	}
}
