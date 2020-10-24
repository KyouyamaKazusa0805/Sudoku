#nullable enable annotations

#pragma warning disable IDE0060

using System;

namespace Sudoku.DocComments
{
	/// <summary>
	/// Provides with doc comments for <c>ToString</c> methods.
	/// </summary>
	public sealed class Formattable
	{
		/// <summary>
		/// Returns a string that represents the current object with the specified format string.
		/// </summary>
		/// <param name="format">The format. If available, the parameter can be <see langword="null"/>.</param>
		/// <returns>The string result.</returns>
		public string ToString(string? format) => throw new NotImplementedException();
	}
}
