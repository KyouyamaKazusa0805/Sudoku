namespace Sudoku.Compatibility.Hodoku;

/// <summary>
/// Represents some methods that are used for get the details supported and defined
/// by another program called <see href="https://sourceforge.net/projects/hodoku/">Hodoku</see>.
/// </summary>
public static class HodokuCompatibility
{
	/// <summary>
	/// Gets all possible aliased names that are defined by Hodoku.
	/// </summary>
	/// <param name="this">The technique.</param>
	/// <returns>
	/// The array of aliased names, or <see langword="null"/> if it is not defined by Hodoku.
	/// </returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the specified value is not defined by the type <see cref="Technique"/>,
	/// or the value is <see cref="Technique.None"/>.
	/// </exception>
	/// <seealso cref="Technique"/>
	public static string[]? GetAliases(Technique @this)
		=> (@this != Technique.None && Enum.IsDefined(@this))
			? typeof(Technique).GetField(@this.ToString()) is { } fieldInfo
				? fieldInfo.GetCustomAttribute<HodokuAttribute>() is { Aliases: var aliases } ? aliases : null
				: null
			: throw new ArgumentOutOfRangeException(nameof(@this));

	/// <summary>
	/// Try to get the prefix of the specified technique. The return value will be a four-digit value
	/// represented as a <see cref="string"/> value. For more information please visit
	/// <see href="https://sourceforge.net/p/hodoku/code/HEAD/tree/HoDoKu/trunk/reglib-1.4.txt">this link</see>.
	/// </summary>
	/// <param name="this">The technique.</param>
	/// <returns>
	/// <para>
	/// A <see cref="string"/> value using a four-digit value to describe a technique used
	/// in a sudoku grid, which is defined by the project Hodoku.
	/// </para>
	/// <para>
	/// If this technique is not supported by Hodoku, <see langword="null"/> will be returned.
	/// </para>
	/// </returns>
	/// <remarks>
	/// <para>
	/// Please note that some technique fields are separated into multiple sub-types, but they all
	/// belong to a same type of a technique supported by Hodoku. For example,
	/// <see cref="Technique.SueDeCoq"/> and <see cref="Technique.SueDeCoqIsolated"/> are supported by
	/// Hodoku as one technique "Sue de Coq", so if you get the prefix from the two fields, the return value
	/// are same (i.e. the <see cref="string"/> value <c>"1101"</c>).
	/// </para>
	/// <para>
	/// However, if a sub-type is not supported by Hodoku (e.g. the field
	/// <see cref="Technique.UniqueRectangle2B1"/>), this method will still return <see langword="null"/>
	/// indicating the "Not supported" state.
	/// </para>
	/// </remarks>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the specified value is not defined by the type <see cref="Technique"/>,
	/// or the value is <see cref="Technique.None"/>.
	/// </exception>
	/// <seealso cref="Technique"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string? GetHodokuLibraryPrefix(Technique @this)
		=> (@this != Technique.None && Enum.IsDefined(@this))
			? typeof(Technique).GetField(@this.ToString()) is { } fieldInfo
				? fieldInfo.GetCustomAttribute<HodokuAttribute>() is { Prefix: var prefix } ? prefix : null
				: null
			: throw new ArgumentOutOfRangeException(nameof(@this));

	/// <summary>
	/// Try to get difficulty score of the specified technique.
	/// </summary>
	/// <param name="this">The technique.</param>
	/// <param name="difficultyLevel">The difficulty level that is defined by Hodoku.</param>
	/// <returns>
	/// <para>An <see cref="int"/> value defined by the project Hodoku.</para>
	/// <para>If this technique is not supported by Hodoku, <see langword="null"/> will be returned.</para>
	/// </returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the specified value is not defined by the type <see cref="Technique"/>,
	/// or the value is <see cref="Technique.None"/>.
	/// </exception>
	/// <seealso cref="Technique"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[return: NotNullIfNotNull(nameof(difficultyLevel))]
	public static int? GetDifficultyScore(Technique @this, out HodokuDifficultyLevel? difficultyLevel)
	{
		if (@this == Technique.None || !Enum.IsDefined(@this))
		{
			throw new ArgumentOutOfRangeException(nameof(@this));
		}

		(var @return, difficultyLevel) = typeof(Technique).GetField(@this.ToString()) switch
		{
			{ } fieldInfo => fieldInfo.GetCustomAttribute<HodokuAttribute>() switch
			{
				{ RatingValue: var rating, DifficultyLevel: var level } => (rating, level),
				_ => (null, null)
			},
			_ => default((int?, HodokuDifficultyLevel?))
		};
		return @return;
	}
}
