using System.Collections.Generic;
using System.Linq;
using Sudoku.Data.Literals;
using Sudoku.Data.Meta;
using Sudoku.Drawing;

namespace Sudoku.Solving.Singles
{
	/// <summary>
	/// Represents a single technique step finder.
	/// Technique contains:
	/// <list type="bullet">
	/// <item><term>Hidden Single</term><description>1.0, 1.2 or 1.5</description></item>
	/// <item><term>Naked Single</term><description>2.3</description></item>
	/// </list>
	/// </summary>
	public sealed class SingleStepFinder : StepFinder
	{
		public override IEnumerable<TechniqueInfo> TakeAll(Grid grid)
		{
			return (
				// Iterate on hidden single steps.
				from quad in
					from region in Values.RegionRange
					from digit in Values.DigitRange
					let emptyInfos = grid[region].Where(i => !i.IsValueCell)
					let checkedInfos = emptyInfos.Where(i => i[digit])
					where checkedInfos.Count() == 1
					select (digit, region, emptyInfos.Count() == 1, checkedInfos.First().Cell)
				select (SingleInfo)GetInfoHidden(quad, GetViewHidden(quad))
			).Concat(
				// Iterate on naked single steps.
				from info in grid
				where info.IsNakedSingle
				select (SingleInfo)GetInfoNaked(GetViewNaked(info, info.Cell), info)
			);
		}


		private static NakedSingleInfo GetInfoNaked(View view, CellInfo info)
		{
			return new NakedSingleInfo(
				conclusion: new Conclusion(
					conclusionType: ConclusionType.Assignment,
					candidates: new[] { new Candidate(info.Cell, info.TrendValue) }),
				views: new List<View> { view });
		}

		private static View GetViewNaked(CellInfo info, Cell cell)
		{
			return new View(
				cells: new List<(Id, Cell)> { (0, cell) },
				candidates: new List<(Id, Candidate)> { (0, new Candidate(cell, info.TrendValue)) },
				regions: null,
				inferences: null);
		}

		private static HiddenSingleInfo GetInfoHidden(
			(int digit, Region region, bool isFullHouse, Cell cell) quad, View view)
		{
			return new HiddenSingleInfo(
				conclusion: new Conclusion(
					conclusionType: ConclusionType.Assignment,
					candidates: new[] { new Candidate(quad.cell, quad.digit) }),
				views: new List<View> { view },
				region: quad.region,
				digit: quad.digit,
				isFullHouse: quad.isFullHouse);
		}

		private static View GetViewHidden((int digit, Region region, bool, Cell cell) quad)
		{
			return new View(
				cells: null,
				candidates: new List<(Id, Candidate)> { (0, new Candidate(quad.cell, quad.digit)) },
				regions: new List<(Id, Region)> { (0, quad.region) },
				inferences: null);
		}
	}
}
