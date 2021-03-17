using System.Runtime.CompilerServices;
using Sudoku.DocComments;

namespace System.Extensions
{
	partial class BitOperationsEx
	{
		/// <inheritdoc cref="Integer.GetEnumerator(Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[CLSCompliant(false)]
		public static ReadOnlySpan<int>.Enumerator GetEnumerator(this sbyte @this) =>
			@this.GetAllSets().GetEnumerator();

		/// <inheritdoc cref="Integer.GetEnumerator(Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ReadOnlySpan<int>.Enumerator GetEnumerator(this byte @this) =>
			@this.GetAllSets().GetEnumerator();

		/// <inheritdoc cref="Integer.GetEnumerator(Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ReadOnlySpan<int>.Enumerator GetEnumerator(this short @this) =>
			@this.GetAllSets().GetEnumerator();

		/// <inheritdoc cref="Integer.GetEnumerator(Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[CLSCompliant(false)]
		public static ReadOnlySpan<int>.Enumerator GetEnumerator(this ushort @this) =>
			@this.GetAllSets().GetEnumerator();

		/// <inheritdoc cref="Integer.GetEnumerator(Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ReadOnlySpan<int>.Enumerator GetEnumerator(this int @this) =>
			@this.GetAllSets().GetEnumerator();

		/// <inheritdoc cref="Integer.GetEnumerator(Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[CLSCompliant(false)]
		public static ReadOnlySpan<int>.Enumerator GetEnumerator(this uint @this) =>
			@this.GetAllSets().GetEnumerator();

		/// <inheritdoc cref="Integer.GetEnumerator(Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ReadOnlySpan<int>.Enumerator GetEnumerator(this long @this) =>
			@this.GetAllSets().GetEnumerator();

		/// <inheritdoc cref="Integer.GetEnumerator(Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[CLSCompliant(false)]
		public static ReadOnlySpan<int>.Enumerator GetEnumerator(this ulong @this) =>
			@this.GetAllSets().GetEnumerator();
	}
}
