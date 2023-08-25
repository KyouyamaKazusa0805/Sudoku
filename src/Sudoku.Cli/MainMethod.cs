using SpecialFolder = System.Environment.SpecialFolder;

const int totalPuzzlesCount = 200000;
var desktop = Environment.GetFolderPath(SpecialFolder.Desktop);
for (var emptyCellsCount = 35; emptyCellsCount <= 55; emptyCellsCount += 5)
{
	var filePath = $@"{desktop}\空格数 {emptyCellsCount} 题库.txt";
	await using var sw = new StreamWriter(filePath);
	for (var count = 0; count < totalPuzzlesCount; count++)
	{
		await sw.WriteLineAsync(makeGrid(emptyCellsCount).ToString("!."));

		Console.Clear();
		Console.WriteLine(
			$"""
			Empty cells count {emptyCellsCount}, progress:
			{count}/{totalPuzzlesCount} ({(double)count / totalPuzzlesCount:P2})
			"""
		);
	}
}

static Grid makeGrid(int emptyCellsCount)
{
	while (true)
	{
		var grid = HodokuPuzzleGenerator.Generate();
		var count = grid.EmptiesCount;
		if (count < emptyCellsCount)
		{
			continue;
		}

		if (Analyzer.Analyze(grid) is not { IsSolved: true, DifficultyLevel: DifficultyLevel.Easy, Steps: var steps })
		{
			continue;
		}

		if (!Array.TrueForAll(steps, static step => step.Code is Technique.FullHouse or Technique.LastDigit or Technique.HiddenSingleBlock))
		{
			continue;
		}

		if (count == emptyCellsCount)
		{
			return grid;
		}

		foreach (var step in steps[..(count - emptyCellsCount)])
		{
			grid.Apply(step);
		}

		return grid;
	}
}


return 0;

file static partial class Program { public static readonly Analyzer Analyzer = PredefinedAnalyzers.SstsOnly; }
