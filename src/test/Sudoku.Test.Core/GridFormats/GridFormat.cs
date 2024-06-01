namespace Sudoku.Test;

[TestClass]
public class GridFormat
{
	[TestMethod("SusserFormat")]
	public void TestMethod1()
	{
		var grid = Grid.Parse("..67.35......4....5.......29.......7.3.....4.8.......11.......4..........5926731.");
		var susserStr = grid.ToString(new SusserGridFormatInfo());
		var susserZeroStr = grid.ToString(new SusserGridFormatInfo { Placeholder = '0' });
		Assert.IsTrue(susserStr == "..67.35......4....5.......29.......7.3.....4.8.......11.......4..........5926731.");
		Assert.IsTrue(susserZeroStr == "006703500000040000500000002900000007030000040800000001100000004000000000059267310");
	}

	[TestMethod("InvariantCulture")]
	public void TestMethod2()
	{
		var grid = Grid.Parse("..67.35......4....5.......29.......7.3.....4.8.......11.......4..........5926731.");
		var s = grid.ToString(GridFormatInfo.InvariantInfo);
		Assert.IsTrue(s == grid.ToString(new PencilmarkGridFormatInfo()));
	}

	[TestMethod("CurrentCulture")]
	public void TestMethod3()
	{
		var grid = Grid.Parse("..67.35......4....5.......29.......7.3.....4.8.......11.......4..........5926731.");
		var s = grid.ToString(GridFormatInfo.CurrentInfo);
		Assert.IsTrue(s == "..67.35......4....5.......29.......7.3.....4.8.......11.......4..........5926731.");
	}

	[TestMethod("ComparisonOnCulture")]
	public void TestMethod4()
	{
		var grid = Grid.Parse("..67.35......4....5.......29.......7.3.....4.8.......11.......4..........5926731.");
		var s = grid.ToString(CultureInfo.InvariantCulture);
		Assert.IsTrue(s == grid.ToString(new PencilmarkGridFormatInfo()));
	}
}
