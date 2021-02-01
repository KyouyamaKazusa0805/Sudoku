namespace Sudoku.Resources
{
	/// <summary>
	/// Here displays the default and basic paths in this project.
	/// </summary>
	internal static class Paths
	{
		/// <summary>
		/// Indicates the trained data file name.
		/// </summary>
		public const string TrainedDataFileName
#if WIN_UI
		= @"C:\ProgramData\Sunnie's Sudoku Solution\eng.traineddata";
#elif WPF_OR_CONSOLE
		= "eng.traineddata";
#endif

		/// <summary>
		/// Indicates the language source dictioanry (English).
		/// </summary>
		public const string LangSourceEnUs
#if WIN_UI
		= @"C:\ProgramData\Sunnie's Sudoku Solution\lang\Resources.en-us.dic";
#elif WPF_OR_CONSOLE
		= @"lang\Resources.en-us.dic";
#endif

		/// <summary>
		/// Indicates the language source dictionary (Chinese).
		/// </summary>
		public const string LangSourceZhCn
#if WIN_UI
		= @"C:\ProgramData\Sunnie's Sudoku Solution\lang\Resources.zh-cn.dic";
#elif WPF_OR_CONSOLE
		= @"lang\Resources.zh-cn.dic";
#endif

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
		public const string ConfigurationFile
#if WIN_UI
		= @"C:\ProgramData\Sunnie's Sudoku Solution\config.ssss";
#elif WPF_OR_CONSOLE
		= @"config.ssss";
#endif
	}
}
