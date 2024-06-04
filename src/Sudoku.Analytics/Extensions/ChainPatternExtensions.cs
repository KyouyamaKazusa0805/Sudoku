namespace Sudoku.Analytics.Chaining;

/// <summary>
/// Provides with extension methods on <see cref="ChainPattern"/> instances.
/// </summary>
/// <seealso cref="ChainPattern"/>
public static class ChainPatternExtensions
{
	/// <summary>
	/// Try to categorize the pattern and return an equivalent <see cref="Technique"/> field representing such patterns.
	/// </summary>
	/// <param name="this">The pattern to be checked.</param>
	/// <param name="grid">The grid to check for eliminations.</param>
	/// <returns>The <see cref="Technique"/> field categorized.</returns>
	public static Technique GetTechnique(this ChainPattern @this, scoped ref readonly Grid grid)
	{
		var conclusions = @this.GetConclusions(in grid);
		return @this switch
		{
			Chain { SatisfyXRule: var isX, SatisfyYRule: var isY, IsGrouped: var isGrouped, Links.Length: var length } instance
				=> (isX, isY, length) switch
				{
					(true, _, 3) => isGrouped ? Technique.GroupedTurbotFish : Technique.TurbotFish,
					(true, _, _) => isGrouped ? Technique.GroupedXChain : Technique.XChain,
					(_, true, _) => isGrouped ? Technique.GroupedXyChain : Technique.XyChain,
					_ => instance switch
					{
						{ ContainsOverlappedNodes: true, First.Map.Count: not 1, Last.Map.Count: not 1 } => Technique.NodeCollision,
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
			Loop { SatisfyXRule: var isX, SatisfyYRule: var isY, IsGrouped: var isGrouped }
				=> (isX, isY) switch
				{
					(true, false) => isGrouped ? Technique.GroupedFishyCycle : Technique.FishyCycle,
					(false, true) => isGrouped ? Technique.GroupedXyCycle : Technique.XyCycle,
					_ => isGrouped ? Technique.GroupedContinuousNiceLoop : Technique.ContinuousNiceLoop
				}
		};
	}

	/// <summary>
	/// Try to get sort key of the pattern.
	/// </summary>
	/// <param name="this">The pattern key.</param>
	/// <returns>The pattern sort key.</returns>
	internal static int GetSortKey(this ChainPattern @this)
	{
		return 42;
	}
}
