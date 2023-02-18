namespace SudokuStudio;

using static Technique;

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
				new() { IsEnabled = false, Technique = FullHouse, RoutingPageTypeName = typeof(FullHousePage).Name },
				new() { IsEnabled = false, Technique = LastDigit },
				new() { IsEnabled = false, Technique = HiddenSingleBlock },
				new() { IsEnabled = false, Technique = HiddenSingleRow },
				new() { IsEnabled = false, Technique = HiddenSingleColumn },
				new() { IsEnabled = false, Technique = NakedSingle }
			}
		}
	};
}
