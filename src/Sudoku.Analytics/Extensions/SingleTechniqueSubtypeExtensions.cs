namespace Sudoku.Analytics.Categorization;

/// <summary>
/// Provides with extension methods on <see cref="SingleTechniqueSubtype"/>.
/// </summary>
/// <seealso cref="SingleTechniqueSubtype"/>
public static class SingleTechniqueSubtypeExtensions
{
	/// <summary>
	/// Indicates whether the specified subtype is unnecessary in practice.
	/// For example, a naked single with 8 digits in a block will form a full house, which is unnecessary.
	/// </summary>
	/// <param name="this">The subtype.</param>
	/// <returns>A <see cref="bool"/> result indicating whether the subtype is unnecessary.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsUnnecessary(this SingleTechniqueSubtype @this)
		=> @this is SingleTechniqueSubtype.BlockHiddenSingle000 or SingleTechniqueSubtype.RowHiddenSingle000 or SingleTechniqueSubtype.ColumnHiddenSingle000
		or SingleTechniqueSubtype.NakedSingle8;

#pragma warning disable format
	/// <summary>
	/// Try to get the number of excluders that the current single subtype will use.
	/// This method will return a non-zero value if and only if it is a hidden single.
	/// </summary>
	/// <param name="this">Indicates the subtype.</param>
	/// <returns>The number of excluder value.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetExcludersCount(this SingleTechniqueSubtype @this)
		=> @this switch
		{
			>= SingleTechniqueSubtype.BlockHiddenSingle000 and < SingleTechniqueSubtype.NakedSingle0 => @this.GetAttribute().ExtraArguments switch
			{
				[int value] => value,
				_ => 0
			},
			_ => 0
		};
#pragma warning restore format

	/// <summary>
	/// Try to get the related technique of the current subtype.
	/// </summary>
	/// <param name="this">The subtype.</param>
	/// <returns>The related technique instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Technique GetRelatedTechnique(this SingleTechniqueSubtype @this) => @this.GetAttribute().RelatedTechnique;

	/// <summary>
	/// Gets the attribute.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static TechniqueMetadataAttribute GetAttribute(this SingleTechniqueSubtype @this)
		=> typeof(SingleTechniqueSubtype).GetField(@this.ToString())!.GetCustomAttribute<TechniqueMetadataAttribute>()!;
}
