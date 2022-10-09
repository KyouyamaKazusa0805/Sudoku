namespace Sudoku.Test.Serialization;

[TestClass]
public sealed class JsonSerializationTest
{
	[TestMethod(nameof(CellMap))]
	public void TestCase_CellMap()
	{
		var cells = CellsMap[0] + 1 + 2 + 5 + 9 + 10;

		var json = JsonSerializer.Serialize(cells);
		Console.WriteLine(json);

		var newInstance = JsonSerializer.Deserialize<CellMap>(json);
		Assert.IsTrue(cells == newInstance);
	}

	[TestMethod(nameof(Grid))]
	public void TestCase_Grid()
	{
		const string code = "6...8..4..+4.....862..4.67+5.+1+5397+486+2.7...+3+51+4+462+5183+9+7..61.2+4+7572+4+8...+3..1..4+7.+28:314 916 921 325 925 932 933 371 985 991";
		var grid = Grid.Parse(code);

		var json = JsonSerializer.Serialize(grid, new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
		Assert.IsTrue(json[1..^1] == code);
	}
}
