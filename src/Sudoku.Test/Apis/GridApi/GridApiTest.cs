namespace Sudoku.Test.Apis.GridApi;

[TestClass]
public sealed class GridApiTest
{
	[TestMethod]
	public void TestCase_Equality()
	{
		var grid1 = (Grid)"6...8..4..+4.....862..4.67+5.+1+5397+486+2.7...+3+51+4+462+5183+9+7..61.2+4+7572+4+8...+3..1..4+7.+28:314 916 921 325 925 932 933 371 985 991";
		var grid2 = (Grid)":0000:x:6...8..4..+4.....862..4.67+5.+1+5397+486+2.7...+3+51+4+462+5183+9+7..61.2+4+7572+4+8...+3..1..4+7.+28:314 916 921 325 925 932 933 371 985 991::";

		Assert.IsTrue(grid1.Equals(grid2));
	}
}
