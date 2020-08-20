using System;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Data.Literals;
using Sudoku.Data.Meta;
using Sudoku.Drawing;
using Sudoku.Solving.Tools;
using static Sudoku.Solving.ConclusionType;

namespace Sudoku.Solving.Subsets
{
	/// <summary>
	/// Represents a subset technique step finder.
	/// Technique contains:
	/// <list type="bullet">
	/// <item><term>Locked Subsets</term><description>2.0 or 2.5</description></item>
	/// <item><term>Naked Subsets</term><description>3.0, 3.6 or 5.0</description></item>
	/// <item><term>Naked Subsets Extension</term><description>3.1, 3.7 or 5.2</description></item>
	/// <item><term>Hidden Subsets</term><description>3.4, 4.0 or 5.4</description></item>
	/// </list>
	/// </summary>
	public sealed class SubsetStepFinder : StepFinder
	{
		private static readonly int[] IterationSizes = { 2, 3, 4 };


		public override IEnumerable<TechniqueInfo> TakeAll(Grid grid) =>
			(
				// Iterate on naked subset steps.
				from size in IterationSizes
				from region in Values.RegionRange
				from cellPositionMask in Values.SubsetTable[size]
				where grid.GetEmptyCellsCount(region) > size
				let structInfos = grid.SelectInfos(region, cellPositionMask)
				let structCands = structInfos.Select(i => i.Candidates)
				let commonCands = Utility.FindCommonSubset(structCands, size)
				where commonCands is not null && structInfos.Count() == size
				let digits = commonCands.Trues
				let candsToDisplay = GetCandsToDisplayNaked(structInfos, digits)
				let fixes = structInfos.Select(i => i.Cell)
				let cellInfos = grid[region]
				let elims = GetElimsNaked(cellInfos, fixes, commonCands)
				where elims.Any()
				let lockedRegions = CheckLockedRegionTypes(structInfos, digits)
				let isLocked = lockedRegions.All(pair => pair.digitRegions.Count >= 2)
				let extraElims = GetExtraElimsNaked(grid, elims, fixes, lockedRegions, commonCands)
				let view = GetView(region, candsToDisplay)
				select (SubsetInfo)GetInfoNaked(size, region, digits, elims, extraElims, isLocked, view)
			).Concat(
				// Iterate on hidden subset steps.
				from size in IterationSizes
				from region in Values.RegionRange
				from digitMask in Values.SubsetTable[size]
				where grid.GetEmptyCellsCount(region) <= 9 - size
				let digits = digitMask.Trues
				let cellPositionMasks = grid.SelectCellPositions(region, digitMask)
				let commonCands = Utility.FindCommonSubset(cellPositionMasks, size)
				where commonCands is not null && !HasValueCell(grid, region, digits)
				let structInfos = grid.SelectInfos(region, commonCands)
				let elims = GetElimsHidden(structInfos, digits)
				where elims.Any()
				let candsToDisplay = GetCandsToDisplayHidden(structInfos, digits)
				let view = GetView(region, candsToDisplay)
				select (SubsetInfo)GetInfoHidden(size, region, digits, elims, view)
			);


		private static bool HasValueCell(Grid grid, Region region, IEnumerable<int> digits) =>
			digits.Any(digit => grid[region].Any(info => info.IsValueCell && info.Value == digit));

		private static NakedSubsetInfo GetInfoNaked(
			int size, Region region, IEnumerable<int> digits, IEnumerable<Candidate> elims,
			IEnumerable<Candidate> extraElims, bool isLocked, View view)
		{
			if (isLocked)
			{
				return new LockedSubsetInfo(
					conclusion: new(Elimination, elims.Concat(extraElims)),
					views: new List<View> { view }, region, digits, size);
			}
			else if (extraElims.Any())
			{
				return new NakedSubsetPlusInfo(
					conclusion: new(Elimination, elims.Concat(extraElims)),
					views: new List<View> { view }, region, digits, size);
			}
			else
			{
				return new(
					conclusion: new(Elimination, elims),
					views: new List<View> { view }, region, digits, size);
			}
		}

		private static HiddenSubsetInfo GetInfoHidden(
			int size, Region region, IEnumerable<int> digits, IEnumerable<Candidate> elims, View view) =>
			new(
				conclusion: new(Elimination, elims),
				views: new List<View> { view }, region, digits, size);

		private static View GetView(Region region, IEnumerable<(Id, Candidate)> candsToDisplay) =>
			new(
				cells: null,
				candidates: candsToDisplay,
				regions: new List<(Id, Region)> { (0, region) },
				inferences: null);

		private static IEnumerable<(int digit, ISet<Region> digitRegions)> CheckLockedRegionTypes(
			IEnumerable<CellInfo> structInfos, IEnumerable<int> digits)
		{
			foreach (var group in
				from digit in digits
				from info in structInfos
				where info.Contains(digit)
				group info by digit)
			{
				var list = group.ToList();
				int count = list.Count;
				var regions = list[0].Cell.Regions;
				for (int i = 1; i < count; i++)
				{
					regions.IntersectWith(list[i].Cell.Regions);
				}

				yield return (group.Key, regions);
			}
		}

		private static IEnumerable<(Id, Candidate)> GetCandsToDisplayNaked(
			IEnumerable<CellInfo> structInfos, IEnumerable<int> digits) =>
			from info in structInfos
			from digit in digits
			where info[digit]
			select ((Id)0, new Candidate(info.Cell, digit));

		private static IEnumerable<(Id, Candidate)> GetCandsToDisplayHidden(
			IEnumerable<CellInfo> structInfos, IEnumerable<int> digits) =>
			from info in structInfos
			from digit in digits
			where !info.IsValueCell && info.Contains(digit)
			select ((Id)0, new Candidate(info.Cell, digit));

		private static IEnumerable<Candidate> GetElimsNaked(
			IEnumerable<CellInfo> cellInfos, IEnumerable<Cell> fixes, CandidateField commonCandsMask) =>
			from info in cellInfos
			from setValue in commonCandsMask.Trues
			let cell = info.Cell
			where !fixes.Contains(cell) && !info.IsValueCell && info[setValue]
			select new Candidate(cell, setValue);

		private static IEnumerable<Candidate> GetElimsHidden(IEnumerable<CellInfo> cellInfos, IEnumerable<int> digits) =>
			from info in cellInfos
			from digit in Values.DigitRange.Except(digits)
			where !info.IsValueCell && info.Contains(digit)
			select new Candidate(info.Cell, digit);

		private static IEnumerable<Candidate> GetExtraElimsNaked(
			Grid grid, IEnumerable<Candidate> elims, IEnumerable<Cell> fixes,
			IEnumerable<(int, ISet<Region>)> lockedRegions, CandidateField commonCandsMask)
		{
			for (int i = 0; i < 9; i++)
			{
				if (commonCandsMask[i])
				{
					// If the current bit is set,
					// we will check this digit is locked or not.
					var list = new List<Candidate>();
					foreach (var (digit, regions) in lockedRegions)
					{
						foreach (var region in regions)
						{
							list.AddRange(
								from info in grid[region]
								let cand = new Candidate(info.Cell, i)
								where digit == i && !fixes.Contains(info.Cell) && !info.IsValueCell
								&& info[i] && !elims.Contains(cand) && !list.Contains(cand)
								select cand);
						}
					}

					foreach (var cand in list)
					{
						yield return cand;
					}
				}
			}
		}
	}
}
