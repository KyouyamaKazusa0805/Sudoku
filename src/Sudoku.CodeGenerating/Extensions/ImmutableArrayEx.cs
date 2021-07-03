using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.CodeGenerating.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="ImmutableArray{T}"/>
	/// </summary>
	/// <seealso cref="ImmutableArray{T}"/>
	public static class ImmutableArrayEx
	{
		/// <summary>
		/// Matches the array of <see cref="IParameterSymbol"/> instances, to check whether the
		/// types matches the nullable annotation list.
		/// </summary>
		/// <param name="this">The array of parameter symbols.</param>
		/// <param name="nullableAnnotations">The nullable annotation list to check.</param>
		/// <returns>
		/// Returns <see langword="true"/> when all parameters matches the corresponding nullable annotation;
		/// otherwise, <see langword="false"/>.
		/// </returns>
		public static bool NullableMatches(
			this in ImmutableArray<IParameterSymbol> @this, params NullableAnnotation[] nullableAnnotations)
		{
			if (@this.Length != nullableAnnotations.Length)
			{
				throw new ArgumentException("The length doesn't match.", nameof(nullableAnnotations));
			}

			for (int i = 0, length = nullableAnnotations.Length; i < length; i++)
			{
				if (@this[i].NullableAnnotation != nullableAnnotations[i])
				{
					return false;
				}
			}

			return true;
		}
	}
}
