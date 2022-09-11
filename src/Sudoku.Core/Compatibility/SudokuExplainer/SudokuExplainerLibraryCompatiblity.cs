namespace Sudoku.Compatibility.SudokuExplainer;

using DifficultyRange = ValueTuple<
	/*Original*/ SudokuExplainerDifficultyRatingRange?,
	/*Advanced*/ SudokuExplainerDifficultyRatingRange?>;

/// <summary>
/// Represents some methods that are used for get the details supported and defined
/// by another program called
/// <see href="http://diuf.unifr.ch/pai/people/juillera/Sudoku/Sudoku.html">Sudoku Explainer</see> (Broken link).
/// </summary>
public static class SudokuExplainerLibraryCompatiblity// : ICompatibilityProvider
{
	/// <inheritdoc cref="ICompatibilityProvider.ProgramName"/>
	public static string ProgramName => "Sudoku Explainer";


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
	public static string[]? GetAliases(this Technique @this)
		=> (@this != Technique.None && Enum.IsDefined(@this)) switch
		{
			true => typeof(Technique).GetField(@this.ToString()) switch
			{
				{ } fieldInfo => fieldInfo.GetCustomAttribute<SudokuExplainerAliasedNamesAttribute>() switch
				{
					{ Aliases: var aliases } => aliases,
					_ => null
				},
				_ => null
			},
			_ => throw new ArgumentOutOfRangeException(nameof(@this))
		};

	/// <summary>
	/// Try to get difficulty rating of the specified technique.
	/// </summary>
	/// <param name="this">The technique.</param>
	/// <returns>
	/// <para>
	/// An <see cref="int"/> value defined by the project Sudoku Explainer.
	/// </para>
	/// <para>
	/// If this technique is not supported by Sudoku Explainer, <see langword="null"/> will be returned.
	/// </para>
	/// </returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the specified value is not defined by the type <see cref="Technique"/>,
	/// or the value is <see cref="Technique.None"/>.
	/// </exception>
	/// <seealso cref="Technique"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DifficultyRange? GetDifficultyRatingRange(this Technique @this)
	{
		if (@this == Technique.None || !Enum.IsDefined(@this))
		{
			throw new ArgumentOutOfRangeException(nameof(@this));
		}

		var attributes = typeof(Technique)
			.GetField(@this.ToString())!
			.GetCustomAttributes<SudokuExplainerDifficultyRatingAttribute>()
			.ToArray();
		return attributes switch
		{
			[] => null,
			[
				{
					DifficultyRating: var min,
					DifficultyRatingMaximumThreshold: var max,
					IsAdvancedDefined: false
				}
			] => (new(min, max ?? min), null),
#pragma warning disable IDE0055
			[
				{
					DifficultyRating: var min1,
					DifficultyRatingMaximumThreshold: var max1,
					IsAdvancedDefined: false
				},
				{
					DifficultyRating: var min2,
					DifficultyRatingMaximumThreshold: var max2,
					IsAdvancedDefined: true
				}
			] => (new(min1, max1 ?? min1), new(min2, max2 ?? min2)),
			[
				{
					DifficultyRating: var min1,
					DifficultyRatingMaximumThreshold: var max1,
					IsAdvancedDefined: true
				},
				{
					DifficultyRating: var min2,
					DifficultyRatingMaximumThreshold: var max2,
					IsAdvancedDefined: false
				}
			] => (new(min2, max2 ?? min2), new(min1, max1 ?? min1)),
#pragma warning restore IDE0055
			_ => throw new InvalidOperationException("The field has marked too much attributes.")
		};
	}
}
