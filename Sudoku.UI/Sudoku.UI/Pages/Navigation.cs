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
		/// <param name="tag">The tag name.</param>
		/// <returns>The type of that page. If the page doesn't found, return <see langword="null"/>.</returns>
		public static Type? GetPageType(string tag) => Type.GetType($"Sudoku.IO.{tag}Page");
	}
}
