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

		/// <summary>
		/// Try to get the path in the additional file list that satisify the specified condition.
		/// </summary>
		/// <param name="this">The list of additional files.</param>
		/// <param name="predicate">The condition</param>
		/// <returns>
		/// The file name found. If none of all files satisfies the condition, <see langword="null"/>.
		/// </returns>
		internal static string? GetPath(this in ImmutableArray<AdditionalText> @this, Predicate<string> predicate)
		{
			foreach (var file in @this)
			{
				if (predicate(file.Path))
				{
					return file.Path;
				}
			}

			return null;
		}
	}
}
