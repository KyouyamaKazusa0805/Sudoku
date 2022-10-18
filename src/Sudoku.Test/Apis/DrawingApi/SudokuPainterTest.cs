namespace Sudoku.Test.Apis.DrawingApi;

[TestClass]
public sealed class SudokuPainterTest
{
	[TestMethod]
	[SupportedOSPlatform("windows")]
	public void TestCase_Normal()
	{
		var grid = (Grid)"6...8..4..+4.....862..4.67+5.+1+5397+486+2.7...+3+51+4+462+5183+9+7..61.2+4+7572+4+8...+3..1..4+7.+28:314 916 921 325 925 932 933 371 985 991";
		var sudokuPainter = ISudokuPainter.CreatePainter(620)
			.WithGrid(grid)
			.WithRenderingCandidates(true);

		const string fileName = "test.png";
		Assert.IsTrue(sudokuPainter.TrySaveTo($@"{GetFolderPath(SpecialFolder.Desktop)}\{fileName}"));
	}
}
