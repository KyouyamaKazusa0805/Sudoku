using Sudoku.Collections;
using Xunit.Abstractions;
using static Sudoku.Constants.Tables;

namespace Sudoku.Test;

internal sealed class GuardianSearcher
{
	private readonly Dictionary<(short Candidate, Region Region), HashSet<int>?> _candidates = new();

	private readonly List<((short Candidate, byte Region)[], Cells Guardians)> _foundGuardians = new();

	/// <summary>
	/// Indicates the output instance that can allow displaying the customized items onto the test explorer.
	/// </summary>
	private readonly ITestOutputHelper _output;


	/// <summary>
	/// Initializes a <see cref="AicSearcher"/> instance via the <see cref="ITestOutputHelper"/> instance
	/// to allow displaying the customized items onto the test explorer.
	/// </summary>
	/// <param name="output">
	/// The <see cref="ITestOutputHelper"/> instance
	/// that allows displaying the customized items onto the test explorer.
	/// </param>
	/// <seealso cref="ITestOutputHelper"/>
	public GuardianSearcher(ITestOutputHelper output) => _output = output;


	/// <summary>
	/// Get all possible chain steps.
	/// </summary>
	/// <param name="grid">The grid used.</param>
	public void GetAll(in Grid grid)
	{
		_candidates.Clear();

		foreach (short candidate in grid)
		{
			GetWeak(candidate, grid);
		}

		Bfs(grid);

		if (_foundGuardians.Count != 0)
		{
			foreach (var (candidates, guardians) in _foundGuardians)
			{
				byte digit = (byte)(candidates[0].Candidate % 9);
				var elimMap = !guardians & grid.CandidatesMap[digit];
				if (elimMap is [])
				{
					continue;
				}

				string candidatesLoopStr = string.Join(
					" -> ",
					from c in candidates
					select $"{digit}r{c.Candidate / 9 / 9}c{c.Candidate / 9 % 9}"
				);

				var conclusions = new ConclusionCollection(
					from cell in elimMap
					select new Conclusion(ConclusionType.Elimination, cell, digit));

				_output.WriteLine($"{candidatesLoopStr}, Guardians {guardians} => {conclusions.ToString()}");
			}
		}
	}

	private void GetWeak(short candidate, in Grid grid)
	{
		byte cell = (byte)(candidate / 9), digit = (byte)(candidate % 9);

		var targetCells = (PeerMaps[cell] & grid.CandidatesMap[digit]) - cell;
		foreach (var label in Regions)
		{
			HashSet<int>? list = null;
			foreach (byte anotherCell in targetCells & RegionMaps[cell.ToRegionIndex(label)])
			{
				(list ??= new(8)).Add((short)(anotherCell * 9 + digit));
			}

			_candidates.Add((candidate, label), list);
		}
	}

	private void Bfs(in Grid grid)
	{
		var loop = new List<(short Candidate, byte Region)>();
		foreach (var ((candidate, region), nextCandidates) in _candidates)
		{
			if (nextCandidates is null)
			{
				continue;
			}

			var pair = (candidate, (byte)(candidate / 9).ToRegionIndex(region));
			loop.Add(pair);

			foreach (short nextCandidate in nextCandidates)
			{
				var nextPair = (nextCandidate, (byte)(nextCandidate / 9).ToRegionIndex(region));
				loop.Add(nextPair);

				bfs(grid, region, loop, nextCandidate);

				loop.Remove(nextPair);
			}

			loop.Remove(pair);
		}


		void bfs(in Grid grid, Region region, List<(short Candidate, byte Region)> loop, short candidate)
		{
			foreach (var label in Regions)
			{
				if (label == region || _candidates[(candidate, label)] is not { } nextCandidates)
				{
					continue;
				}

				foreach (short nextCandidate in nextCandidates)
				{
					byte currentRegion = (byte)(nextCandidate / 9).ToRegionIndex(label);
					if (loop.FindIndex(p => p.Region == currentRegion) != -1)
					{
						continue;
					}

					if (loop[0].Candidate == nextCandidate && loop.Count >= 5 && (loop.Count & 1) != 0)
					{
						// Found.
						var foundCandidates = new (short, byte)[loop.Count + 1];
						loop.CopyTo(foundCandidates, 0);
						foundCandidates[^1] = (nextCandidate, currentRegion);
						var guardians = GetGuardians(grid, foundCandidates);
						if (!guardians is not [])
						{
							_foundGuardians.Add((foundCandidates, guardians));

							return;
						}
					}

					if (loop.Contains((nextCandidate, currentRegion)))
					{
						continue;
					}

					loop.Add((nextCandidate, currentRegion));

					bfs(grid, label, loop, nextCandidate);

					loop.Remove((nextCandidate, currentRegion));
				}
			}
		}
	}

	private Cells GetGuardians(in Grid grid, (short Candidate, byte)[] foundCandidates)
	{
		var result = Cells.Empty;
		for (int i = 0, length = foundCandidates.Length - 1; i < length; i++)
		{
			short a = foundCandidates[i].Candidate, b = foundCandidates[i + 1].Candidate;
			result = getGuardians(grid, (byte)(a / 9), (byte)(b / 9), (byte)(a % 9), result);
		}

		return result;


		static Cells getGuardians(in Grid grid, byte c1, byte c2, byte digit, in Cells guardians)
		{
			var tempMap = Cells.Empty;
			foreach (int coveredRegion in new Cells { c1 / 9, c2 / 9 }.CoveredRegions)
			{
				tempMap |= RegionMaps[coveredRegion];
			}

			tempMap &= grid.CandidatesMap[digit];
			tempMap |= guardians;

			return tempMap - c1 - c2;
		}
	}
}
