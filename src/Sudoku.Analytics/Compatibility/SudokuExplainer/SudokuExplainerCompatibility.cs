namespace Sudoku.Compatibility.SudokuExplainer;

/// <summary>
/// Represents some methods that are used for get the details supported and defined
/// by another program called
/// <see href="http://diuf.unifr.ch/pai/people/juillera/Sudoku/Sudoku.html">Sudoku Explainer</see> (Broken link).
/// </summary>
public static class SudokuExplainerCompatibility
{
	/// <summary>
	/// Gets all possible aliased names that are defined by Sudoku Explainer.
	/// </summary>
	/// <param name="this">The technique.</param>
	/// <returns>
	/// The array of aliased names, or <see langword="null"/> if it is not defined by Sudoku Explainer.
	/// </returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the specified value is not defined by the type <see cref="Technique"/>,
	/// or the value is <see cref="Technique.None"/>.
	/// </exception>
	/// <seealso cref="Technique"/>
	public static string[]? GetAliases(Technique @this)
		=> (@this != Technique.None && Enum.IsDefined(@this))
			? typeof(Technique).GetField(@this.ToString()) is { } fieldInfo
				? fieldInfo.GetCustomAttribute<SudokuExplainerAttribute>() is { Aliases: var names } ? names : null
				: null
			: throw new ArgumentOutOfRangeException(nameof(@this));

	/// <summary>
	/// Try to get the corresponding technique defined by Sudoku Explainer.
	/// </summary>
	/// <param name="this">The technique.</param>
	/// <returns>
	/// The corresponding technique defined by Sudoku Explainer. If not found, <see langword="null"/> will be returned.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static SudokuExplainerTechnique? GetCorrespondingTechnique(Technique @this)
	{
		var found = typeof(Technique).GetField(@this.ToString())?.GetCustomAttribute<SudokuExplainerAttribute>();
		return found is { DifficultyLevel: var flag } ? flag : null;
	}

	/// <summary>
	/// Try to get difficulty rating of the specified technique.
	/// </summary>
	/// <param name="this">The technique.</param>
	/// <returns>
	/// <para>A <see cref="SudokuExplainerRating"/> value defined by the project Sudoku Explainer.</para>
	/// <para>If this technique is not supported by Sudoku Explainer, <see langword="null"/> will be returned.</para>
	/// </returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the specified value is not defined by the type <see cref="Technique"/>,
	/// or the value is <see cref="Technique.None"/>.
	/// </exception>
	/// <seealso cref="Technique"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#pragma warning disable format
	public static SudokuExplainerRating? GetDifficultyRatingRange(Technique @this)
		=> @this == Technique.None || !Enum.IsDefined(@this)
			? throw new ArgumentOutOfRangeException(nameof(@this))
			: (SudokuExplainerAttribute[])typeof(Technique).GetField(@this.ToString())!.GetCustomAttributes<SudokuExplainerAttribute>() switch
			{
				[] => null,
				[{ RatingValueOriginal: [var min], RatingValueAdvanced: null }] => new(new((half)min, (half)min), null),
				[{ RatingValueOriginal: [var min, var max], RatingValueAdvanced: null }] => new(new((half)min, (half)max), null),
				[{ RatingValueAdvanced: [var min], RatingValueOriginal: null }] => new(null, new((half)min, (half)min)),
				[{ RatingValueAdvanced: [var min, var max], RatingValueOriginal: null }] => new(null, new((half)min, (half)max)),
				[{ RatingValueOriginal: [var min1], RatingValueAdvanced: [var min2] }]
					=> new(new((half)min1, (half)min1), new((half)min2, (half)min2)),
				[{ RatingValueOriginal: [var min1, var max1], RatingValueAdvanced: [var min2] }]
					=> new(new((half)min1, (half)max1), new((half)min2, (half)min2)),
				[{ RatingValueOriginal: [var min1], RatingValueAdvanced: [var min2, var max2] }]
					=> new(new((half)min1, (half)min1), new((half)min2, (half)max2)),
				[{ RatingValueOriginal: [var min1, var max1], RatingValueAdvanced: [var min2, var max2] }]
					=> new(new((half)min1, (half)max1), new((half)min2, (half)max2)),
				[{ RatingValue: var r }] => new(new((half)r, (half)r), null),
				_ => throw new InvalidOperationException(ResourceDictionary.ExceptionMessage("TooMuchAttributes"))
			};
#pragma warning restore format
}
