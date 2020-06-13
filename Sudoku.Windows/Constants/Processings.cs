using System;
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
		/// The splitter in using <see cref="string.Split(char[]?)"/> or other split methods
		/// where the method contain <see cref="char"/>[]? type parameter.
		/// </summary>
		/// <seealso cref="string.Split(char[]?)"/>
		public static readonly char[] Splitter = new[] { '\r', '\n' };


		/// <summary>
		/// Gets a new-line string defined for this environment.
		/// </summary>
		public static string NewLine => Environment.NewLine;
		
		/// <summary>
		/// The language source dictionary.
		/// </summary>
		public static ResourceDictionary LangSource => Application.Current.Resources;
	}
}
