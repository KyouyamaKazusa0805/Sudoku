namespace Sudoku.Analytics.Chaining;

/// <summary>
/// Provides with extension methods on <see cref="ChainPattern"/> instances.
/// </summary>
/// <seealso cref="ChainPattern"/>
public static class ChainPatternExtensions
{
	/// <summary>
	/// Indicates the sort key dictionary.
	/// </summary>
	private static readonly Dictionary<Technique, byte> SortKeyDictionary = new()
	{
		// 3-chain
		{ Technique.Skyscraper, 1 },
		{ Technique.TwoStringKite, 2 },
		{ Technique.TurbotFish, 3 },
		{ Technique.GroupedSkyscraper, 4 },
		{ Technique.GroupedTwoStringKite, 5 },
		{ Technique.GroupedTurbotFish, 6 },

		// 5-chain
		{ Technique.WWing, 101 },
		{ Technique.MWing, 102 },
		{ Technique.SWing, 103 },
		{ Technique.LWing, 104 },
		{ Technique.HWing, 105 },
		{ Technique.PurpleCow, 106 },
		{ Technique.GroupedWWing, 107 },
		{ Technique.GroupedMWing, 108 },
		{ Technique.GroupedSWing, 109 },
		{ Technique.GroupedLWing, 110 },
		{ Technique.GroupedHWing, 111 },
		{ Technique.GroupedPurpleCow, 112 },

		// Others
		{ Technique.FishyCycle, 201 },
		{ Technique.XyCycle, 202 },
		{ Technique.ContinuousNiceLoop, 203 },
		{ Technique.XChain, 204 },
		{ Technique.XyChain, 205 },
		{ Technique.XyChain, 206 },
		{ Technique.SelfConstraint, 207 },
		{ Technique.AlternatingInferenceChain, 209 },
		{ Technique.DiscontinuousNiceLoop, 210 },
		{ Technique.GroupedFishyCycle, 211 },
		{ Technique.GroupedXyCycle, 212 },
		{ Technique.GroupedContinuousNiceLoop, 213 },
		{ Technique.GroupedXChain, 214 },
		{ Technique.GroupedXyChain, 215 },
		{ Technique.GroupedXyChain, 216 },
		{ Technique.GroupedSelfConstraint, 217 },
		{ Technique.NodeCollision, 218 },
		{ Technique.GroupedAlternatingInferenceChain, 219 },
		{ Technique.GroupedDiscontinuousNiceLoop, 220 },
	};


	/// <summary>
	/// Try to categorize the pattern and return an equivalent <see cref="Technique"/> field representing such patterns.
	/// </summary>
	/// <param name="this">The pattern to be checked.</param>
	/// <param name="grid">The grid to calculate on conclusions for the pattern.</param>
	/// <returns>The <see cref="Technique"/> field categorized.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Technique GetTechnique(this ChainPattern @this, scoped ref readonly Grid grid)
		=> @this.GetTechnique(@this.GetConclusions(in grid));

	/// <summary>
	/// Try to categorize the pattern and return an equivalent <see cref="Technique"/> field representing such patterns.
	/// </summary>
	/// <param name="this">The pattern to be checked.</param>
	/// <param name="conclusions">The conclusions.</param>
	/// <returns>The <see cref="Technique"/> field categorized.</returns>
	public static Technique GetTechnique(this ChainPattern @this, ConclusionSet conclusions)
		=> @this switch
		{
			Chain
			{
				SatisfyXRule: var isX,
				SatisfyYRule: var isY,
				IsGrouped: var isGrouped,
				Links: var links,
				Links.Length: var length
			} instance => (isX, isY, length) switch
			{
				(true, _, 3) => links switch
				{
#pragma warning disable format
					[
						{ FirstNode.Map.Cells: var cells11, SecondNode.Map.Cells: var cells12 },
						_,
						{ FirstNode.Map.Cells: var cells21, SecondNode.Map.Cells: var cells22 }
					] => (
						TrailingZeroCount((cells11 | cells12).SharedHouses).ToHouseType(),
						TrailingZeroCount((cells21 | cells22).SharedHouses).ToHouseType()
					) switch
					{
						(HouseType.Block, _) or (_, HouseType.Block)
							=> isGrouped ? Technique.GroupedTurbotFish : Technique.TurbotFish,
						(HouseType.Row, HouseType.Column) or (HouseType.Column, HouseType.Row)
							=> isGrouped ? Technique.GroupedTwoStringKite : Technique.TwoStringKite,
						_ => isGrouped ? Technique.GroupedSkyscraper : Technique.Skyscraper
					}
#pragma warning restore format
				},
				(true, _, _) => isGrouped ? Technique.GroupedXChain : Technique.XChain,
				(_, true, _) => isGrouped ? Technique.GroupedXyChain : Technique.XyChain,
				_ => instance switch
				{
					{ ContainsOverlappedNodes: true, First.Map: [var candidate1], Last.Map: [var candidate2] }
						when candidate1 == candidate2 => isGrouped ? Technique.GroupedSelfConstraint : Technique.SelfConstraint,
					{ ContainsOverlappedNodes: true } => Technique.NodeCollision,
					{ IsWoodsWing: true } => isGrouped ? Technique.GroupedWWing : Technique.WWing,
					{ IsMedusaWing: true } => isGrouped ? Technique.GroupedMWing : Technique.MWing,
					{ IsSplitWing: true } => isGrouped ? Technique.GroupedSWing : Technique.SWing,
					{ IsLocalWing: true } => isGrouped ? Technique.GroupedLWing : Technique.LWing,
					{ IsHybridWing: true } => isGrouped ? Technique.GroupedHWing : Technique.HWing,
					{ Links.Length: 5 } => isGrouped ? Technique.GroupedPurpleCow : Technique.PurpleCow,
					{ First: var first, Last: var last } => (first, last) switch
					{
						({ Map.Digits: var digits1 }, { Map.Digits: var digits2 }) => (digits1 == digits2) switch
						{
							true => isGrouped ? Technique.GroupedAlternatingInferenceChain : Technique.AlternatingInferenceChain,
							_ => conclusions.Count switch
							{
								1 => isGrouped ? Technique.GroupedDiscontinuousNiceLoop : Technique.DiscontinuousNiceLoop,
								_ => isGrouped ? Technique.GroupedXyXChain : Technique.XyXChain
							}
						}
					}
				}
			},
			Loop { SatisfyXRule: var isX, SatisfyYRule: var isY, IsGrouped: var isGrouped } => (isX, isY) switch
			{
				(true, false) => isGrouped ? Technique.GroupedFishyCycle : Technique.FishyCycle,
				(false, true) => isGrouped ? Technique.GroupedXyCycle : Technique.XyCycle,
				_ => isGrouped ? Technique.GroupedContinuousNiceLoop : Technique.ContinuousNiceLoop
			}
		};

	/// <summary>
	/// Try to get sort key of the pattern.
	/// </summary>
	/// <param name="this">The pattern.</param>
	/// <param name="grid">Indicates the grid to be used.</param>
	/// <returns>The pattern sort key.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static byte GetSortKey(this ChainPattern @this, scoped ref readonly Grid grid)
		=> @this.GetSortKey(@this.GetConclusions(in grid));

	/// <summary>
	/// Try to get sort key of the pattern.
	/// </summary>
	/// <param name="this">The pattern.</param>
	/// <param name="conclusions">Indicates conclusions to be used.</param>
	/// <returns>The pattern sort key.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static byte GetSortKey(this ChainPattern @this, ConclusionSet conclusions)
		=> SortKeyDictionary[@this.GetTechnique(conclusions)];
}
