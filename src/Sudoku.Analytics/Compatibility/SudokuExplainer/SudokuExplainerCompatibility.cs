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
				? fieldInfo.GetCustomAttribute<SudokuExplainerAliasedNamesAttribute>() is { Aliases: var aliases } ? aliases : null
				: null
			: throw new ArgumentOutOfRangeException(nameof(@this));

	/// <summary>
	/// Try to get difficulty rating of the specified technique.
	/// </summary>
	/// <param name="this">The technique.</param>
	/// <returns>
	/// <para>
	/// A <see cref="SudokuExplainderDifficultyResult"/> value defined by the project Sudoku Explainer.
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
	public static SudokuExplainderDifficultyResult? GetDifficultyRatingRange(Technique @this)
		=> @this == Technique.None || !Enum.IsDefined(@this)
			? throw new ArgumentOutOfRangeException(nameof(@this))
			: (SudokuExplainerDifficultyRatingAttribute[])typeof(Technique).GetField(@this.ToString())!.GetCustomAttributes<SudokuExplainerDifficultyRatingAttribute>() switch
			{
			[] => null,
			[(var min, var max, false)] => new(new(min, max ?? min), null),
			[(var min, var max, true)] => new(null, new(min, max ?? min)),
			[(var min1, var max1, false), (var min2, var max2, true)] => new(new(min1, max1 ?? min1), new(min2, max2 ?? min2)),
			[(var min1, var max1, true), (var min2, var max2, false)] => new(new(min2, max2 ?? min2), new(min1, max1 ?? min1)),
				_ => throw new InvalidOperationException("The field has marked too much attributes.")
			};
}
