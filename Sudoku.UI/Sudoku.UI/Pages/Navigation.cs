using System;

namespace Sudoku.UI.Pages
{
	/// <summary>
	/// Provides methods on navigation view.
	/// </summary>
	internal static class Navigation
	{
		/// <summary>
		/// Try to get the page type using a tag name.
		/// </summary>
		/// <param name="tag">
		/// The tag name. The tag name is used to embed into the type name. For example, if the tag is
		/// <c>Info</c>, then the type name to navigate is <c>Sudoku.IO.InfoPage</c>. Note the prefix
		/// <c>Sudoku.IO</c> and the suffix <c>Page</c> is fixed and can't be modifed.
		/// </param>
		/// <returns>The type of that page. If the page doesn't found, return <see langword="null"/>.</returns>
		public static Type? GetPageType(string tag) => Type.GetType($"Sudoku.IO.{tag}Page");
	}
}
