namespace Sudoku.Compatibilities;

/// <summary>
/// Represents some methods that are used for get the details supported and defined
/// by another program called <see href="https://sourceforge.net/projects/hodoku/">Hodoku</see>.
/// </summary>
public static class HodokuLibraryCompatiblity
{
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
	public static string? GetHodokuPrefix(this Technique @this)
		=> (@this != Technique.None && Enum.IsDefined(@this)) switch
		{
			true => typeof(Technique).GetField(@this.ToString()) switch
			{
				{ } fieldInfo => fieldInfo.GetCustomAttribute<HodokuTechniquePrefixAttribute>() switch
				{
					{ Prefix: var prefix } => prefix,
					_ => null
				},
				_ => null
			},
			_ => throw new ArgumentOutOfRangeException(nameof(@this))
		};
}
