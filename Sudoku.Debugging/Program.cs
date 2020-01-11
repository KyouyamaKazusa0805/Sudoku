namespace Sudoku.Debugging
{
	internal static class Program
	{
		private static void Main()
		{
			//var solver = new Sudoku.Solving.Manual.ManualSolver
			//{
			//	OptimizedApplyingOrder = true,
			//	EnableFullHouse = true,
			//	EnableLastDigit = true
			//};
			var grid = Sudoku.Data.Meta.Grid.Parse("320009+100+40000+150006130040004903+6+801+60+3812+90+48+10090360004008210+106000+70000010+3+649:515 615 724 825 228 229 731 235 738 748 563 972 574 882 484 584 792 295");
			System.Console.WriteLine(grid.ToString("@!"));
			//var analysisResult = solver.Solve(grid);
			//System.Console.WriteLine(analysisResult);
		}
	}
}
