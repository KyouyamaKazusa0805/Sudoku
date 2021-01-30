using System.Runtime.CompilerServices;
using Windows.UI.Xaml;

namespace Sudoku.UI.Dictionaries
{
	/// <summary>
	/// The resource dictionary.
	/// </summary>
	public static class DictionaryResources
	{
		/// <summary>
		/// Indicates the language resource.
		/// </summary>
		public static ResourceDictionary LangSource
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Application.Current.Resources;
		}
	}
}
