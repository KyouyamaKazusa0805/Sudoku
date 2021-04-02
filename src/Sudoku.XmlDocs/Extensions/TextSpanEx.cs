using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis.Text;
using Sudoku.DocComments;

namespace Sudoku.XmlDocs.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="TextSpan"/>.
	/// </summary>
	/// <seealso cref="TextSpan"/>
	public static class TextSpanEx
	{
		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="this">The current instance.</param>
		/// <param name="start">The range start value.</param>
		/// <param name="end">The range end value.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Deconstruct(this in TextSpan @this, out int start, out int end)
		{
			start = @this.Start;
			end = @this.End;
		}

		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="this">The current instance.</param>
		/// <param name="start">The range start value.</param>
		/// <param name="end">The range end value.</param>
		/// <param name="length">The length.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Deconstruct(this in TextSpan @this, out int start, out int end, out int length)
		{
			start = @this.Start;
			end = @this.End;
			length = @this.Length;
		}
	}
}
