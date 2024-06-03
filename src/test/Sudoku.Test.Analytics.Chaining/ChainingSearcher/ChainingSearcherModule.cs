namespace Sudoku.Test;

[TestClass]
public class ChainingSearcherModule
{
	[TestMethod("ChainConstruction")]
	public void TestMethod1()
	{
		var grid = Grid.Parse("7.+12.6...+2+65.3+8.1+7.8.1...+26.1.+6237.8..+28...+6+1+8.6+51.+24.1..+7.5+63+2.2..6+1.+7.+6+57+3.+2+1.4:417 435 451 452");
		var foundChains = ChainingDriver.CollectChainPatterns(in grid, [new XChainingRule(), new YChainingRule()], out _, out _);
		Assert.IsTrue(foundChains.Length != 0);
	}
}
