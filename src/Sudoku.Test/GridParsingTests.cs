using Sudoku.Collections;
using Xunit;

namespace Sudoku.Test;

/// <summary>
/// Defines a set of tests that plays with parsing <see cref="Grid"/> instances.
/// </summary>
public sealed class GridParsingTests
{
	/// <summary>
	/// Indicates the sample test.
	/// </summary>
	[Fact]
	public void SampleTest()
	{
		bool result = Grid.TryParse(
			"009300000+603819005870050000000000006408090102200000000000080053300526400000003600:717 718 719 444",
			out _
		);

		Assert.True(result);
	}
}
