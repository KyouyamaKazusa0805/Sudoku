using Sudoku.Collections;
using Xunit;
using Xunit.Abstractions;

namespace Sudoku.Test;

/// <summary>
/// Defines a set of tests that plays with testing about AIC searching.
/// </summary>
public sealed class AicTests
{
	/// <summary>
	/// Indicates the test code.
	/// </summary>
	/// <remarks>
	/// Full format:
	/// <code>
	/// :x:0000:009300000+603819005870050000000000006408090102200000000000080053300526400000003600:717 718 719 444:::
	/// </code>
	/// </remarks>
	private const string TestCode = "009300000+603819005870050000000000006408090102200000000000080053300526400000003600:717 718 719 444";

	/// <summary>
	/// Indicates the output helper instance.
	/// </summary>
	private readonly ITestOutputHelper _output;


	/// <summary>
	/// Initializes an <see cref="AicTests"/> instance
	/// via the specified output helper instance.
	/// </summary>
	/// <param name="output">The output helper instance.</param>
	public AicTests(ITestOutputHelper output) => _output = output;


	/// <summary>
	/// Indicates the sample test.
	/// </summary>
	[Fact(Timeout = 20000)]
	public void Test()
	{
		var grid = Grid.Parse(TestCode);
		var searcher = new AicSearcher(_output);
		searcher.GetAll(grid);
	}
}
