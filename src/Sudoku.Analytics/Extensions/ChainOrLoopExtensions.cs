namespace Sudoku.Analytics.Chaining;

/// <summary>
/// Provides with extension methods on <see cref="ChainOrLoop"/> instances.
/// </summary>
/// <seealso cref="ChainOrLoop"/>
public static class ChainOrLoopExtensions
{
	/// <summary>
	/// Try to categorize the pattern and return an equivalent <see cref="Technique"/> field representing such patterns.
	/// </summary>
	/// <param name="this">The pattern to be checked.</param>
	/// <param name="grid">The grid to calculate on conclusions for the pattern.</param>
	/// <returns>The <see cref="Technique"/> field categorized.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Technique GetTechnique(this ChainOrLoop @this, ref readonly Grid grid)
		=> @this.GetTechnique(@this.GetConclusions(in grid));

	/// <summary>
	/// Try to categorize the pattern and return an equivalent <see cref="Technique"/> field representing such patterns.
	/// </summary>
	/// <param name="this">The pattern to be checked.</param>
	/// <param name="conclusions">The conclusions.</param>
	/// <returns>The <see cref="Technique"/> field categorized.</returns>
	public static Technique GetTechnique(this ChainOrLoop @this, ConclusionSet conclusions)
		=> @this switch
		{
			Chain { SatisfyXRule: var isX, IsGrouped: var isGrouped, Links: var links } instance => instance switch
			{
				{ ContainsOverlappedNodes: true, First.Map: var map1, Last.Map: var map2 } when map1 == map2
					=> isGrouped ? Technique.GroupedSelfConstraint : Technique.SelfConstraint,
				{ SatisfyXRule: true } => isGrouped ? Technique.GroupedXChain : Technique.XChain,
				{ SatisfyYRule: true } => isGrouped ? Technique.GroupedXyChain : Technique.XyChain,
				{ ContainsOverlappedNodes: true } => Technique.NodeCollision,
#if false
				{ IsWoodsWing: true } => isGrouped ? Technique.GroupedWWing : Technique.WWing,
				{ IsMedusaWing: true } => isGrouped ? Technique.GroupedMWing : Technique.MWing,
#endif
				{ IsSplitWing: true } => isGrouped ? Technique.GroupedSWing : Technique.SWing,
				{ IsLocalWing: true } => isGrouped ? Technique.GroupedLWing : Technique.LWing,
				{ IsHybridWing: true } => isGrouped ? Technique.GroupedHWing : Technique.HWing,
				{ Links.Length: 5, IsWoodsWing: false, IsMedusaWing: false }
					=> isGrouped ? Technique.GroupedPurpleCow : Technique.PurpleCow,
				{ Links.Length: 3 } when isX => links switch
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
			},
			Loop { SatisfyXRule: var isX, SatisfyYRule: var isY, IsGrouped: var isGrouped } => (isX, isY) switch
			{
				(true, false) => isGrouped ? Technique.GroupedFishyCycle : Technique.FishyCycle,
				(false, true) => isGrouped ? Technique.GroupedXyCycle : Technique.XyCycle,
				_ => isGrouped ? Technique.GroupedContinuousNiceLoop : Technique.ContinuousNiceLoop
			}
		};
}
