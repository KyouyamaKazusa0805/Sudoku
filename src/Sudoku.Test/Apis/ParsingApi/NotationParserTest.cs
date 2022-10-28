namespace Sudoku.Test.Apis.ParsingApi;

[TestClass]
public sealed class NotationParserTest
{
	[TestMethod]
	public void TestCase_CellRxCy()
	{
		var s = "r1c23";
		var cells = RxCyNotation.ParseCells(s);

		Assert.IsTrue(cells == CellMap.Empty + 1 + 2);
	}

	[TestMethod]
	public void TestCase_CandidateRxCy()
	{
		var s1 = "r1c23(15)";
		var s2 = "{r1c2,r1c3}(15)";
		var s3 = "15r1c23";

		var c1 = RxCyNotation.ParseCandidates(s1);
		var c2 = RxCyNotation.ParseCandidates(s2);
		var c3 = RxCyNotation.ParseCandidates(s3);

		Assert.IsTrue(c1 == Candidates.Empty + 9 + 13 + 18 + 22);
		Assert.IsTrue(c2 == Candidates.Empty + 9 + 13 + 18 + 22);
		Assert.IsTrue(c3 == Candidates.Empty + 9 + 13 + 18 + 22);
	}

	[TestMethod]
	public void TestCase_CellK9()
	{
		var s = "C356";
		var cells = K9Notation.ParseCells(s);

		Assert.IsTrue(cells == CellMap.Empty + 20 + 22 + 23);
	}

	[TestMethod]
	public void TestCase_HodokuElimination()
	{
		var s = "123 456";
		var c = EliminationNotation.ParseCandidates(s);

		Assert.IsTrue(c == Candidates.Empty + 99 + 372);
	}
}
