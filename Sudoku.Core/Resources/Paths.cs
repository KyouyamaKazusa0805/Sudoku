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
	}
}
