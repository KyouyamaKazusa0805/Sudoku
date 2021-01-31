namespace Sudoku.UI.Data
{
	/// <summary>
	/// Stores a series of paths.
	/// </summary>
	public static class Paths
	{
		/// <summary>
		/// The appropraiate file path that can store configuration file.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Please note that the return value is a path, but this path may not exist in the hard drive.
		/// You can use the code like <c>Directory.Create(path)</c> to create the path.
		/// </para>
		/// <para>
		/// The extension name <c>ssss</c> is the abbreviation of
		/// <c><b>S</b>unnie's <b>s</b>udoku <b>s</b>olution <b>s</b>ettings</c>.
		/// </para>
		/// <para>The program is only running on Windows 10, so the path can be fixed.</para>
		/// </remarks>
		public const string ConfigurationFile = @"C:\ProgramData\Sunnie's Sudoku Solution\config.ssss";
	}
}
