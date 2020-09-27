using System.Windows;

namespace Sudoku.Windows.Constants
{
	/// <summary>
	/// Provides all <see langword="const"/> or <see langword="readonly"/> values
	/// for internal processing.
	/// </summary>
	/// <remarks>
	/// Some values should be used after the window initialized, so they cannot be fields
	/// (Properties can be used as a method called for specified uses).
	/// </remarks>
	internal static class Processings
	{
		/// <summary>
		/// The language source dictionary.
		/// </summary>
		public static ResourceDictionary LangSource => Application.Current.Resources;
	}
}
