namespace SudokuStudio;

/// <summary>
/// Provides with a field that stores the techniques data used by page <see cref="TechniqueGalleryPage"/>.
/// </summary>
/// <seealso cref="TechniqueGalleryPage"/>
internal static class TechniqueGalleryData
{
	/// <summary>
	/// The routing groups.
	/// </summary>
	public static readonly ObservableCollection<TechniquePageRoutingDataGroup> RoutingGroups = new()
	{
		new()
		{
			Title = GetString("TechniqueGalleryPage_Singles"),
			Collection = new TechniquePageRoutingData[]
			{
				new() { IsEnabled = false, Color = Colors.Green, Technique = Technique.FullHouse, RoutingPageTypeName = typeof(FullHousePage).Name },
				new() { IsEnabled = false, Color = Colors.Green, Technique = Technique.LastDigit },
				new() { IsEnabled = false, Color = Colors.Green, Technique = Technique.HiddenSingleBlock },
				new() { IsEnabled = false, Color = Colors.Green, Technique = Technique.HiddenSingleRow },
				new() { IsEnabled = false, Color = Colors.Green, Technique = Technique.HiddenSingleColumn },
				new() { IsEnabled = false, Color = Colors.Green, Technique = Technique.NakedSingle }
			}
		},
		new()
		{
			Title = GetString("TechniqueGalleryPage_Intersections"),
			Collection = new TechniquePageRoutingData[]
			{
				new() { IsEnabled = false, Color = Colors.Aqua, Technique = Technique.Pointing },
				new() { IsEnabled = false, Color = Colors.Aqua, Technique = Technique.Claiming }
			}
		},
		new()
		{
			Title = GetString("TechniqueGalleryPage_Subsets"),
			Collection = new TechniquePageRoutingData[]
			{
				new() { IsEnabled = false, Color = Colors.Yellow, Technique = Technique.LockedPair },
				new() { IsEnabled = false, Color = Colors.Yellow, Technique = Technique.LockedTriple },
				new() { IsEnabled = false, Color = Colors.Yellow, Technique = Technique.NakedPair },
				new() { IsEnabled = false, Color = Colors.Yellow, Technique = Technique.NakedTriple },
				new() { IsEnabled = false, Color = Colors.Yellow, Technique = Technique.NakedTriplePlus },
				new() { IsEnabled = false, Color = Colors.Yellow, Technique = Technique.NakedQuadruple },
				new() { IsEnabled = false, Color = Colors.Yellow, Technique = Technique.NakedQuadruplePlus },
				new() { IsEnabled = false, Color = Colors.Yellow, Technique = Technique.HiddenPair },
				new() { IsEnabled = false, Color = Colors.Yellow, Technique = Technique.HiddenTriple },
				new() { IsEnabled = false, Color = Colors.Yellow, Technique = Technique.HiddenQuadruple }
			}
		},
		new()
		{
			Title = GetString("TechniqueGalleryPage_NormalFishes"),
			Collection = new TechniquePageRoutingData[]
			{
				new() { IsEnabled = false, Color = Colors.Orange, Technique = Technique.XWing },
				new() { IsEnabled = false, Color = Colors.Orange, Technique = Technique.FinnedXWing },
				new() { IsEnabled = false, Color = Colors.Orange, Technique = Technique.SashimiXWing },
				new() { IsEnabled = false, Color = Colors.Orange, Technique = Technique.Swordfish },
				new() { IsEnabled = false, Color = Colors.Orange, Technique = Technique.FinnedSwordfish },
				new() { IsEnabled = false, Color = Colors.Orange, Technique = Technique.SashimiSwordfish },
				new() { IsEnabled = false, Color = Colors.Orange, Technique = Technique.Jellyfish },
				new() { IsEnabled = false, Color = Colors.Orange, Technique = Technique.FinnedJellyfish },
				new() { IsEnabled = false, Color = Colors.Orange, Technique = Technique.SashimiJellyfish }
			}
		},
		new()
		{
			Title = GetString("TechniqueGalleryPage_RegularWings"),
			Collection = new TechniquePageRoutingData[]
			{
				new() { IsEnabled = false, Color = Colors.Pink, Technique = Technique.XyWing },
				new() { IsEnabled = false, Color = Colors.Pink, Technique = Technique.XyzWing },
				new() { IsEnabled = false, Color = Colors.Pink, Technique = Technique.WxyzWing },
				new() { IsEnabled = false, Color = Colors.Pink, Technique = Technique.VwxyzWing },
				new() { IsEnabled = false, Color = Colors.Pink, Technique = Technique.IncompleteWxyzWing },
				new() { IsEnabled = false, Color = Colors.Pink, Technique = Technique.IncompleteVwxyzWing }
			}
		},
		new()
		{
			Title = GetString("TechniqueGalleryPage_IrregularWings"),
			Collection = new TechniquePageRoutingData[]
			{
				new() { IsEnabled = false, Color = Colors.DeepPink, Technique = Technique.WWing },
				new() { IsEnabled = false, Color = Colors.DeepPink, Technique = Technique.WWing },
				new() { IsEnabled = false, Color = Colors.DeepPink, Technique = Technique.MWing },
				new() { IsEnabled = false, Color = Colors.DeepPink, Technique = Technique.SplitWing },
				new() { IsEnabled = false, Color = Colors.DeepPink, Technique = Technique.LocalWing },
				new() { IsEnabled = false, Color = Colors.DeepPink, Technique = Technique.HybridWing },
				new() { IsEnabled = false, Color = Colors.DeepPink, Technique = Technique.GroupedWWing },
				new() { IsEnabled = false, Color = Colors.DeepPink, Technique = Technique.GroupedMWing },
				new() { IsEnabled = false, Color = Colors.DeepPink, Technique = Technique.GroupedSplitWing },
				new() { IsEnabled = false, Color = Colors.DeepPink, Technique = Technique.GroupedLocalWing },
				new() { IsEnabled = false, Color = Colors.DeepPink, Technique = Technique.GroupedHybridWing },
				new() { IsEnabled = false, Color = Colors.DeepPink, Technique = Technique.MultiBranchWWing }
			}
		},
		new()
		{
			Title = GetString("TechniqueGalleryPage_UniqueRectangles"),
			Collection = new TechniquePageRoutingData[]
			{
				new() { IsEnabled = false, Color = Colors.Purple, Technique = Technique.UniqueRectangleType1 },
				new() { IsEnabled = false, Color = Colors.Purple, Technique = Technique.UniqueRectangleType2 },
				new() { IsEnabled = false, Color = Colors.Purple, Technique = Technique.UniqueRectangleType3 },
				new() { IsEnabled = false, Color = Colors.Purple, Technique = Technique.UniqueRectangleType4 },
				new() { IsEnabled = false, Color = Colors.Purple, Technique = Technique.UniqueRectangleType5 },
				new() { IsEnabled = false, Color = Colors.Purple, Technique = Technique.UniqueRectangleType6 },
				new() { IsEnabled = false, Color = Colors.Purple, Technique = Technique.HiddenUniqueRectangle }
			}
		},
		new()
		{
			Title = GetString("TechniqueGalleryPage_UniqueRectangles2"),
			Collection = new TechniquePageRoutingData[]
			{
				new() { IsEnabled = false, Color = Colors.Purple, Technique = Technique.UniqueRectangle2D },
				new() { IsEnabled = false, Color = Colors.Purple, Technique = Technique.UniqueRectangle2B1 },
				new() { IsEnabled = false, Color = Colors.Purple, Technique = Technique.UniqueRectangle2D1 },
				new() { IsEnabled = false, Color = Colors.Purple, Technique = Technique.UniqueRectangle3X },
				new() { IsEnabled = false, Color = Colors.Purple, Technique = Technique.UniqueRectangle3X1L },
				new() { IsEnabled = false, Color = Colors.Purple, Technique = Technique.UniqueRectangle3X1U },
				new() { IsEnabled = false, Color = Colors.Purple, Technique = Technique.UniqueRectangle3X2 },
				new() { IsEnabled = false, Color = Colors.Purple, Technique = Technique.UniqueRectangle3N2 },
				new() { IsEnabled = false, Color = Colors.Purple, Technique = Technique.UniqueRectangle3U2 },
				new() { IsEnabled = false, Color = Colors.Purple, Technique = Technique.UniqueRectangle3E2 },
				new() { IsEnabled = false, Color = Colors.Purple, Technique = Technique.UniqueRectangle4X1L },
				new() { IsEnabled = false, Color = Colors.Purple, Technique = Technique.UniqueRectangle4X1U },
				new() { IsEnabled = false, Color = Colors.Purple, Technique = Technique.UniqueRectangle4X2L },
				new() { IsEnabled = false, Color = Colors.Purple, Technique = Technique.UniqueRectangle4X2U },
				new() { IsEnabled = false, Color = Colors.Purple, Technique = Technique.UniqueRectangle4X3 },
				new() { IsEnabled = false, Color = Colors.Purple, Technique = Technique.UniqueRectangle4C3 }
			}
		},
		new()
		{
			Title = GetString("TechniqueGalleryPage_UniqueRectangles3"),
			Collection = new TechniquePageRoutingData[]
			{
				new() { IsEnabled = false, Color = Colors.Purple, Technique = Technique.UniqueRectangleXyWing },
				new() { IsEnabled = false, Color = Colors.Purple, Technique = Technique.UniqueRectangleXyzWing },
				new() { IsEnabled = false, Color = Colors.Purple, Technique = Technique.UniqueRectangleWxyzWing },
				new() { IsEnabled = false, Color = Colors.Purple, Technique = Technique.UniqueRectangleSueDeCoq },
				new() { IsEnabled = false, Color = Colors.Purple, Technique = Technique.UniqueRectangleBabaGrouping },
				new() { IsEnabled = false, Color = Colors.Purple, Technique = Technique.UniqueRectangleExternalType1 },
				new() { IsEnabled = false, Color = Colors.Purple, Technique = Technique.UniqueRectangleExternalType2 },
				new() { IsEnabled = false, Color = Colors.Purple, Technique = Technique.UniqueRectangleExternalType3 },
				new() { IsEnabled = false, Color = Colors.Purple, Technique = Technique.UniqueRectangleExternalType4 },
				new() { IsEnabled = false, Color = Colors.Purple, Technique = Technique.UniqueRectangleExternalSkyscraper },
				new() { IsEnabled = false, Color = Colors.Purple, Technique = Technique.UniqueRectangleExternalTwoStringKite },
				new() { IsEnabled = false, Color = Colors.Purple, Technique = Technique.UniqueRectangleExternalTurbotFish },
				new() { IsEnabled = false, Color = Colors.Purple, Technique = Technique.UniqueRectangleExternalXyWing },
				new() { IsEnabled = false, Color = Colors.Purple, Technique = Technique.UniqueRectangleExternalAlmostLockedSetsXz }
			}
		}
	};
}
