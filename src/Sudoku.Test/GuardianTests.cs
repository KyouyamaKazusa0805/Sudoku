using Sudoku.Collections;
using Xunit;
using Xunit.Abstractions;

namespace Sudoku.Test;

/// <summary>
/// Defines a set of tests that plays with testing about guardian searching.
/// </summary>
public sealed class GuardianTests
{
	/// <summary>
	/// Indicates the test code.
	/// </summary>
	private const string TestCode = "1+4000800508005020+100+5001040+8+947+1+6+5+23300005+1+7+4+5+7+1+34+29+8+605000901+20+18+5200072001004+50:931 935";


	/// <summary>
	/// Indicates the output helper instance.
	/// </summary>
	private readonly ITestOutputHelper _output;


	/// <summary>
	/// Initializes an <see cref="GuardianTests"/> instance
	/// via the specified output helper instance.
	/// </summary>
	/// <param name="output">The output helper instance.</param>
	public GuardianTests(ITestOutputHelper output) => _output = output;


	/// <summary>
	/// Indicates the sample test.
	/// </summary>
	[Fact(Timeout = 20000)]
	public void Test()
	{
		var grid = Grid.Parse(TestCode);
		var searcher = new GuardianSearcher(_output);
		searcher.GetAll(grid);
	}
}
