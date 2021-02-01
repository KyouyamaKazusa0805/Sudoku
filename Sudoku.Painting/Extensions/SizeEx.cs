using System.Drawing;
using System.Runtime.CompilerServices;
using Sudoku.DocComments;

namespace Sudoku.Painting.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="SizeF"/>
	/// </summary>
	/// <seealso cref="SizeF"/>
	public static class SizeEx
	{
		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="this">(<see langword="this in"/> parameter) The size instance.</param>
		/// <param name="width">(<see langword="out"/> parameter) The width.</param>
		/// <param name="height">(<see langword="out"/> parameter) The height.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Deconstruct(this in SizeF @this, out float width, out float height)
		{
			width = @this.Width;
			height = @this.Height;
		}
	}
}
