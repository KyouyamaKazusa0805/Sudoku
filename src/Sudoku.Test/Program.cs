using Sudoku.Solving.Manual.Buffers;

foreach (var stepSearcher in StepSearcherPool.Searchers)
{
	Console.WriteLine(stepSearcher);
}