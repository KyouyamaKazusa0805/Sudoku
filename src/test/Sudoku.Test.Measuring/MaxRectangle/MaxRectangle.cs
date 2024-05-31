namespace Sudoku.Test;

[TestClass]
public class MaxRectangle
{
	[TestMethod("Normal1")]
	public void TestMethod1()
	{
		var grid = Grid.Parse("..2.5...7.8...63..3......2..3...1.5....9.2..49...3.2....5..37...2..8...14..5.....");
		Assert.IsTrue(grid.GetMaxEmptyArea() == 9);
	}

	[TestMethod("Normal2")]
	public void TestMethod2()
	{
		var grid = Grid.Parse(".2...5.8...867...46....47..1...5.....378..29.8....1..3.............1...72..3...6.");
		Assert.IsTrue(grid.GetMaxEmptyArea() == 9);
	}

	// http://www.matrix67.com/blog/archives/725
	[TestMethod("LargeEmptyArea")]
	public void TestMethod3()
	{
		var grid = Grid.Parse("..67.35......4....5.......29.......7.3.....4.8.......11.......4..........5926731.");
		Assert.IsTrue(grid.GetMaxEmptyArea() == 30);
	}
}
