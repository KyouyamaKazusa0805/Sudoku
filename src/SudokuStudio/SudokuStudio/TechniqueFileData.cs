namespace SudokuStudio;

/// <summary>
/// Provides with a field that stores the techniques data used by page <see cref="TechniqueFilePage"/>.
/// </summary>
/// <seealso cref="TechniqueFilePage"/>
internal static class TechniqueFileData
{
	/// <summary>
	/// The routing groups.
	/// </summary>
	public static readonly ObservableCollection<TechniquePageRoutingDataGroup> RoutingGroups = new()
	{
		new()
		{
			Title = GetString("TechniqueFilePage_Singles"),
			Collection = new TechniquePageRoutingData[]
			{
				new() { Technique = Technique.FullHouse, RoutingPageTypeName = typeof(FullHousePage).Name },
				new() { Technique = Technique.LastDigit, RoutingPageTypeName = typeof(FullHousePage).Name },
				new() { Technique = Technique.HiddenSingleBlock, RoutingPageTypeName = typeof(FullHousePage).Name },
				new() { Technique = Technique.HiddenSingleRow, RoutingPageTypeName = typeof(FullHousePage).Name },
				new() { Technique = Technique.HiddenSingleColumn, RoutingPageTypeName = typeof(FullHousePage).Name },
				new() { Technique = Technique.NakedSingle, RoutingPageTypeName = typeof(FullHousePage).Name }
			}
		}
	};
}
