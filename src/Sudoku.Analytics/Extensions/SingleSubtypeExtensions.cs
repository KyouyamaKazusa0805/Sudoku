namespace Sudoku.Analytics.Categorization;

/// <summary>
/// Provides with extension methods on <see cref="SingleSubtype"/>.
/// </summary>
/// <seealso cref="SingleSubtype"/>
public static class SingleSubtypeExtensions
{
	/// <summary>
	/// Indicates whether the specified subtype is unnecessary in practice.
	/// For example, a naked single with 8 digits in a block will form a full house, which is unnecessary.
	/// </summary>
	/// <param name="this">The subtype.</param>
	/// <returns>A <see cref="bool"/> result indicating whether the subtype is unnecessary.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsUnnecessary(this SingleSubtype @this)
		=> @this is SingleSubtype.BlockHiddenSingle000 or SingleSubtype.RowHiddenSingle000 or SingleSubtype.ColumnHiddenSingle000
		or SingleSubtype.NakedSingle8
		or SingleSubtype.RowHiddenSingle200 or SingleSubtype.RowHiddenSingle201 or SingleSubtype.RowHiddenSingle202
		or SingleSubtype.ColumnHiddenSingle200 or SingleSubtype.ColumnHiddenSingle210 or SingleSubtype.ColumnHiddenSingle220;

	/// <summary>
	/// Try to get the number of excluders that the current single subtype will use.
	/// This method will return a non-zero value if and only if it is a hidden single.
	/// </summary>
	/// <param name="this">Indicates the subtype.</param>
	/// <returns>The number of excluders.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetExcludersCount(this SingleSubtype @this)
		=> @this.GetExcludersCount(HouseType.Block) + @this.GetExcludersCount(HouseType.Row) + @this.GetExcludersCount(HouseType.Column);

	/// <summary>
	/// Try to get the number of excluders that the current single subtype will use in the specified house type.
	/// </summary>
	/// <param name="this">Indicates the subtype.</param>
	/// <param name="houseType">The house type.</param>
	/// <returns>The number of excluders.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetExcludersCount(this SingleSubtype @this, HouseType houseType)
		=> @this.ToString()[^(3 - (int)houseType)] - '0';

	/// <summary>
	/// Try to get the direct difficulty level of the curreent subtype.
	/// </summary>
	/// <param name="this">The subtype.</param>
	/// <returns>The direct difficulty level.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetDirectDifficultyLevel(this SingleSubtype @this) => (int)@this / 100;

	/// <summary>
	/// Gets the abbreviation of the subtype.
	/// </summary>
	/// <param name="this">Indicates the subtype.</param>
	/// <returns>The abbreviation.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string? GetAbbreviation(this SingleSubtype @this) => @this.GetAttribute().Abbreviation;

	/// <summary>
	/// Try to get the related technique of the current subtype.
	/// </summary>
	/// <param name="this">The subtype.</param>
	/// <returns>The related technique instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Technique GetRelatedTechnique(this SingleSubtype @this) => @this.GetAttribute().RelatedTechnique;

	/// <summary>
	/// Try to get related <see cref="SingleTechniqueFlag"/> field.
	/// </summary>
	/// <param name="this">The subtype.</param>
	/// <param name="subtleValue">
	/// A <see cref="bool"/> indicating whether the return value should subtle the handling on return field.
	/// </param>
	/// <returns>The single technique returned.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the argument is out of range.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static SingleTechniqueFlag GetSingleTechnique(this SingleSubtype @this, bool subtleValue = false)
	{
		const string block = "Block", row = "Row", column = "Column";
		return Enum.IsDefined(@this) && @this != SingleSubtype.None
			? @this.ToString() is var s && s.StartsWith(nameof(Technique.FullHouse))
				? SingleTechniqueFlag.FullHouse
				: s == nameof(Technique.LastDigit)
					? subtleValue ? SingleTechniqueFlag.LastDigit : SingleTechniqueFlag.HiddenSingle
					: s.StartsWith(block) || s.StartsWith(row) || s.StartsWith(column)
						? subtleValue
							? s.StartsWith(block)
								? SingleTechniqueFlag.HiddenSingleBlock
								: s.StartsWith(row)
									? SingleTechniqueFlag.HiddenSingleRow
									: SingleTechniqueFlag.HiddenSingleColumn
							: SingleTechniqueFlag.HiddenSingle
						: SingleTechniqueFlag.NakedSingle
			: throw new ArgumentOutOfRangeException(nameof(@this));
	}

	/// <summary>
	/// Gets the attribute.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static TechniqueMetadataAttribute GetAttribute(this SingleSubtype @this)
		=> typeof(SingleSubtype).GetField(@this.ToString())!.GetCustomAttribute<TechniqueMetadataAttribute>()!;
}
