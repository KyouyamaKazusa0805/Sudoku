using System.Numerics;
using System.Runtime.CompilerServices;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Metadata;
using Sudoku.Analytics.Steps;
using Sudoku.Analytics.StepSearcherModules;
using Sudoku.Concepts;
using Sudoku.Rendering;
using Sudoku.Rendering.Nodes;
using Sudoku.Runtime.CompilerServices;
using static System.Numerics.BitOperations;
using static Sudoku.Analytics.CachedFields;
using static Sudoku.Analytics.ConclusionType;
using static Sudoku.Concepts.Intersection;
using static Sudoku.SolutionWideReadOnlyFields;

namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with a <b>Unique Rectangle</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>
/// Basic types:
/// <list type="bullet">
/// <item>Unique Rectangle Type 1-6</item>
/// <item>Avoidable Rectangle Type 1-3, 5</item>
/// <item>Hidden Unique Rectangle</item>
/// <item>Hidden Avoidable Rectangle</item>
/// </list>
/// </item>
/// <item>
/// Constructed types:
/// <list type="bullet">
/// <item>Unique Rectangle + Conjugate Pair (also called "Unique Rectangle + Strong Link")</item>
/// <item>Avoidable Rectangle + Hidden Single</item>
/// <item>Unique Rectangle + Baba Grouping</item>
/// <item>Unique Rectangle + Sue de Coq</item>
/// <item>Unique Rectangle + XY-Wing, XYZ-Wing and WXYZ-Wing</item>
/// <item>Unique Rectangle + W-Wing</item>
/// <item>
/// Unique Rectangle + Guardian (This program call it "Unique Rectangle External Types"):
/// <list type="bullet">
/// <item>Unique Rectangle External Type 1-4</item>
/// <item>Unique Rectangle External XY-Wing</item>
/// <item>Unique Rectangle External W-Wing</item>
/// <item>Unique Rectangle External ALS-XZ</item>
/// <item>Unique Rectangle External Turbot Fish</item>
/// </list>
/// </item>
/// <item>Avoidable Rectangle + Guardian (Sub-types are same like above)</item>
/// </list>
/// </item>
/// <item>
/// Miscellaneous types:
/// <list type="bullet">
/// <item>Unique Rectangle 2D, 3X</item>
/// </list>
/// </item>
/// </list>
/// </summary>
[StepSearcher(
	Technique.UniqueRectangleType1, Technique.UniqueRectangleType2, Technique.UniqueRectangleType3, Technique.UniqueRectangleType4,
	Technique.UniqueRectangleType5, Technique.UniqueRectangleType6, Technique.HiddenUniqueRectangle, Technique.HiddenAvoidableRectangle,
	Technique.UniqueRectangleExternalType1, Technique.UniqueRectangleExternalType2, Technique.UniqueRectangleExternalType3,
	Technique.UniqueRectangleExternalType4, Technique.UniqueRectangleExternalXyWing, Technique.UniqueRectangleExternalTurbotFish,
	Technique.UniqueRectangleExternalWWing, Technique.UniqueRectangleExternalAlmostLockedSetsXz, Technique.UniqueRectangleBabaGrouping,
	Technique.UniqueRectangleSueDeCoq, Technique.UniqueRectangleXyWing, Technique.UniqueRectangleXyzWing, Technique.UniqueRectangleWxyzWing,
	Technique.UniqueRectangleWWing, Technique.UniqueRectangle2B1, Technique.UniqueRectangle2D, Technique.UniqueRectangle2D1, Technique.UniqueRectangle3E2,
	Technique.UniqueRectangle3N2, Technique.UniqueRectangle3U2, Technique.UniqueRectangle3X, Technique.UniqueRectangle3X1L,
	Technique.UniqueRectangle3X1U, Technique.UniqueRectangle3X2, Technique.UniqueRectangle4C3, Technique.UniqueRectangle4X1L,
	Technique.UniqueRectangle4X1U, Technique.UniqueRectangle4X2L, Technique.UniqueRectangle4X2U, Technique.UniqueRectangle4X3,
	Technique.AvoidableRectangleType1, Technique.AvoidableRectangleType2, Technique.AvoidableRectangleType3, Technique.AvoidableRectangleType5,
	Technique.AvoidableRectangleExternalType1, Technique.AvoidableRectangleExternalType2, Technique.AvoidableRectangleExternalType3,
	Technique.AvoidableRectangleExternalType4, Technique.AvoidableRectangleExternalXyWing, Technique.AvoidableRectangleExternalWWing,
	Technique.AvoidableRectangleExternalAlmostLockedSetsXz, Technique.AvoidableRectangle2D, Technique.AvoidableRectangle3X,
	Technique.AvoidableRectangleBrokenWing, Technique.AvoidableRectangleHiddenSingleBlock, Technique.AvoidableRectangleHiddenSingleRow,
	Technique.AvoidableRectangleHiddenSingleColumn, Technique.AvoidableRectangleSueDeCoq, Technique.AvoidableRectangleXyWing,
	Technique.AvoidableRectangleXyzWing, Technique.AvoidableRectangleWxyzWing, Technique.AvoidableRectangleWWing,
	Flags = ConditionalFlags.Standard)]
[StepSearcherRuntimeName("StepSearcherName_UniqueRectangleStepSearcher")]
public sealed partial class UniqueRectangleStepSearcher : StepSearcher
{
#pragma warning disable format
	/// <summary>
	/// The table of all <b>Unique Rectangle</b> cells.
	/// </summary>
	private static readonly Cell[][] UniqueRectanglePatterns = [
		[0, 3, 9, 12], [0, 4, 9, 13], [0, 5, 9, 14], [0, 6, 9, 15], [0, 7, 9, 16], [0, 8, 9, 17], [1, 3, 10, 12], [1, 4, 10, 13], [1, 5, 10, 14],
		[1, 6, 10, 15], [1, 7, 10, 16], [1, 8, 10, 17], [2, 3, 11, 12], [2, 4, 11, 13], [2, 5, 11, 14], [2, 6, 11, 15], [2, 7, 11, 16], [2, 8, 11, 17],
		[3, 6, 12, 15], [3, 7, 12, 16], [3, 8, 12, 17], [4, 6, 13, 15], [4, 7, 13, 16], [4, 8, 13, 17], [5, 6, 14, 15], [5, 7, 14, 16], [5, 8, 14, 17],
		[0, 3, 18, 21], [0, 4, 18, 22], [0, 5, 18, 23], [0, 6, 18, 24], [0, 7, 18, 25], [0, 8, 18, 26], [1, 3, 19, 21], [1, 4, 19, 22], [1, 5, 19, 23],
		[1, 6, 19, 24], [1, 7, 19, 25], [1, 8, 19, 26], [2, 3, 20, 21], [2, 4, 20, 22], [2, 5, 20, 23], [2, 6, 20, 24], [2, 7, 20, 25], [2, 8, 20, 26],
		[3, 6, 21, 24], [3, 7, 21, 25], [3, 8, 21, 26], [4, 6, 22, 24], [4, 7, 22, 25], [4, 8, 22, 26], [5, 6, 23, 24], [5, 7, 23, 25], [5, 8, 23, 26],
		[9, 12, 18, 21], [9, 13, 18, 22], [9, 14, 18, 23], [9, 15, 18, 24], [9, 16, 18, 25], [9, 17, 18, 26], [10, 12, 19, 21], [10, 13, 19, 22], [10, 14, 19, 23],
		[10, 15, 19, 24], [10, 16, 19, 25], [10, 17, 19, 26], [11, 12, 20, 21], [11, 13, 20, 22], [11, 14, 20, 23], [11, 15, 20, 24], [11, 16, 20, 25], [11, 17, 20, 26],
		[12, 15, 21, 24], [12, 16, 21, 25], [12, 17, 21, 26], [13, 15, 22, 24], [13, 16, 22, 25], [13, 17, 22, 26], [14, 15, 23, 24], [14, 16, 23, 25], [14, 17, 23, 26],
		[27, 30, 36, 39], [27, 31, 36, 40], [27, 32, 36, 41], [27, 33, 36, 42], [27, 34, 36, 43], [27, 35, 36, 44], [28, 30, 37, 39], [28, 31, 37, 40], [28, 32, 37, 41],
		[28, 33, 37, 42], [28, 34, 37, 43], [28, 35, 37, 44], [29, 30, 38, 39], [29, 31, 38, 40], [29, 32, 38, 41], [29, 33, 38, 42], [29, 34, 38, 43], [29, 35, 38, 44],
		[30, 33, 39, 42], [30, 34, 39, 43], [30, 35, 39, 44], [31, 33, 40, 42], [31, 34, 40, 43], [31, 35, 40, 44], [32, 33, 41, 42], [32, 34, 41, 43], [32, 35, 41, 44],
		[27, 30, 45, 48], [27, 31, 45, 49], [27, 32, 45, 50], [27, 33, 45, 51], [27, 34, 45, 52], [27, 35, 45, 53], [28, 30, 46, 48], [28, 31, 46, 49], [28, 32, 46, 50],
		[28, 33, 46, 51], [28, 34, 46, 52], [28, 35, 46, 53], [29, 30, 47, 48], [29, 31, 47, 49], [29, 32, 47, 50], [29, 33, 47, 51], [29, 34, 47, 52], [29, 35, 47, 53],
		[30, 33, 48, 51], [30, 34, 48, 52], [30, 35, 48, 53], [31, 33, 49, 51], [31, 34, 49, 52], [31, 35, 49, 53], [32, 33, 50, 51], [32, 34, 50, 52], [32, 35, 50, 53],
		[36, 39, 45, 48], [36, 40, 45, 49], [36, 41, 45, 50], [36, 42, 45, 51], [36, 43, 45, 52], [36, 44, 45, 53], [37, 39, 46, 48], [37, 40, 46, 49], [37, 41, 46, 50],
		[37, 42, 46, 51], [37, 43, 46, 52], [37, 44, 46, 53], [38, 39, 47, 48], [38, 40, 47, 49], [38, 41, 47, 50], [38, 42, 47, 51], [38, 43, 47, 52], [38, 44, 47, 53],
		[39, 42, 48, 51], [39, 43, 48, 52], [39, 44, 48, 53], [40, 42, 49, 51], [40, 43, 49, 52], [40, 44, 49, 53], [41, 42, 50, 51], [41, 43, 50, 52], [41, 44, 50, 53],
		[54, 57, 63, 66], [54, 58, 63, 67], [54, 59, 63, 68], [54, 60, 63, 69], [54, 61, 63, 70], [54, 62, 63, 71], [55, 57, 64, 66], [55, 58, 64, 67], [55, 59, 64, 68],
		[55, 60, 64, 69], [55, 61, 64, 70], [55, 62, 64, 71], [56, 57, 65, 66], [56, 58, 65, 67], [56, 59, 65, 68], [56, 60, 65, 69], [56, 61, 65, 70], [56, 62, 65, 71],
		[57, 60, 66, 69], [57, 61, 66, 70], [57, 62, 66, 71], [58, 60, 67, 69], [58, 61, 67, 70], [58, 62, 67, 71], [59, 60, 68, 69], [59, 61, 68, 70], [59, 62, 68, 71],
		[54, 57, 72, 75], [54, 58, 72, 76], [54, 59, 72, 77], [54, 60, 72, 78], [54, 61, 72, 79], [54, 62, 72, 80], [55, 57, 73, 75], [55, 58, 73, 76], [55, 59, 73, 77],
		[55, 60, 73, 78], [55, 61, 73, 79], [55, 62, 73, 80], [56, 57, 74, 75], [56, 58, 74, 76], [56, 59, 74, 77], [56, 60, 74, 78], [56, 61, 74, 79], [56, 62, 74, 80],
		[57, 60, 75, 78], [57, 61, 75, 79], [57, 62, 75, 80], [58, 60, 76, 78], [58, 61, 76, 79], [58, 62, 76, 80], [59, 60, 77, 78], [59, 61, 77, 79], [59, 62, 77, 80],
		[63, 66, 72, 75], [63, 67, 72, 76], [63, 68, 72, 77], [63, 69, 72, 78], [63, 70, 72, 79], [63, 71, 72, 80], [64, 66, 73, 75], [64, 67, 73, 76], [64, 68, 73, 77],
		[64, 69, 73, 78], [64, 70, 73, 79], [64, 71, 73, 80], [65, 66, 74, 75], [65, 67, 74, 76], [65, 68, 74, 77], [65, 69, 74, 78], [65, 70, 74, 79], [65, 71, 74, 80],
		[66, 69, 75, 78], [66, 70, 75, 79], [66, 71, 75, 80], [67, 69, 76, 78], [67, 70, 76, 79], [67, 71, 76, 80], [68, 69, 77, 78], [68, 70, 77, 79], [68, 71, 77, 80],
		[0, 1, 27, 28], [0, 1, 36, 37], [0, 1, 45, 46], [0, 1, 54, 55], [0, 1, 63, 64], [0, 1, 72, 73], [9, 10, 27, 28], [9, 10, 36, 37], [9, 10, 45, 46],
		[9, 10, 54, 55], [9, 10, 63, 64], [9, 10, 72, 73], [18, 19, 27, 28], [18, 19, 36, 37], [18, 19, 45, 46], [18, 19, 54, 55], [18, 19, 63, 64], [18, 19, 72, 73],
		[27, 28, 54, 55], [27, 28, 63, 64], [27, 28, 72, 73], [36, 37, 54, 55], [36, 37, 63, 64], [36, 37, 72, 73], [45, 46, 54, 55], [45, 46, 63, 64], [45, 46, 72, 73],
		[0, 2, 27, 29], [0, 2, 36, 38], [0, 2, 45, 47], [0, 2, 54, 56], [0, 2, 63, 65], [0, 2, 72, 74], [9, 11, 27, 29], [9, 11, 36, 38], [9, 11, 45, 47],
		[9, 11, 54, 56], [9, 11, 63, 65], [9, 11, 72, 74], [18, 20, 27, 29], [18, 20, 36, 38], [18, 20, 45, 47], [18, 20, 54, 56], [18, 20, 63, 65], [18, 20, 72, 74],
		[27, 29, 54, 56], [27, 29, 63, 65], [27, 29, 72, 74], [36, 38, 54, 56], [36, 38, 63, 65], [36, 38, 72, 74], [45, 47, 54, 56], [45, 47, 63, 65], [45, 47, 72, 74],
		[1, 2, 28, 29], [1, 2, 37, 38], [1, 2, 46, 47], [1, 2, 55, 56], [1, 2, 64, 65], [1, 2, 73, 74], [10, 11, 28, 29], [10, 11, 37, 38], [10, 11, 46, 47],
		[10, 11, 55, 56], [10, 11, 64, 65], [10, 11, 73, 74], [19, 20, 28, 29], [19, 20, 37, 38], [19, 20, 46, 47], [19, 20, 55, 56], [19, 20, 64, 65], [19, 20, 73, 74],
		[28, 29, 55, 56], [28, 29, 64, 65], [28, 29, 73, 74], [37, 38, 55, 56], [37, 38, 64, 65], [37, 38, 73, 74], [46, 47, 55, 56], [46, 47, 64, 65], [46, 47, 73, 74],
		[3, 4, 30, 31], [3, 4, 39, 40], [3, 4, 48, 49], [3, 4, 57, 58], [3, 4, 66, 67], [3, 4, 75, 76], [12, 13, 30, 31], [12, 13, 39, 40], [12, 13, 48, 49],
		[12, 13, 57, 58], [12, 13, 66, 67], [12, 13, 75, 76], [21, 22, 30, 31], [21, 22, 39, 40], [21, 22, 48, 49], [21, 22, 57, 58], [21, 22, 66, 67], [21, 22, 75, 76],
		[30, 31, 57, 58], [30, 31, 66, 67], [30, 31, 75, 76], [39, 40, 57, 58], [39, 40, 66, 67], [39, 40, 75, 76], [48, 49, 57, 58], [48, 49, 66, 67], [48, 49, 75, 76],
		[3, 5, 30, 32], [3, 5, 39, 41], [3, 5, 48, 50], [3, 5, 57, 59], [3, 5, 66, 68], [3, 5, 75, 77], [12, 14, 30, 32], [12, 14, 39, 41], [12, 14, 48, 50],
		[12, 14, 57, 59], [12, 14, 66, 68], [12, 14, 75, 77], [21, 23, 30, 32], [21, 23, 39, 41], [21, 23, 48, 50], [21, 23, 57, 59], [21, 23, 66, 68], [21, 23, 75, 77],
		[30, 32, 57, 59], [30, 32, 66, 68], [30, 32, 75, 77], [39, 41, 57, 59], [39, 41, 66, 68], [39, 41, 75, 77], [48, 50, 57, 59], [48, 50, 66, 68], [48, 50, 75, 77],
		[4, 5, 31, 32], [4, 5, 40, 41], [4, 5, 49, 50], [4, 5, 58, 59], [4, 5, 67, 68], [4, 5, 76, 77], [13, 14, 31, 32], [13, 14, 40, 41], [13, 14, 49, 50],
		[13, 14, 58, 59], [13, 14, 67, 68], [13, 14, 76, 77], [22, 23, 31, 32], [22, 23, 40, 41], [22, 23, 49, 50], [22, 23, 58, 59], [22, 23, 67, 68], [22, 23, 76, 77],
		[31, 32, 58, 59], [31, 32, 67, 68], [31, 32, 76, 77], [40, 41, 58, 59], [40, 41, 67, 68], [40, 41, 76, 77], [49, 50, 58, 59], [49, 50, 67, 68], [49, 50, 76, 77],
		[6, 7, 33, 34], [6, 7, 42, 43], [6, 7, 51, 52], [6, 7, 60, 61], [6, 7, 69, 70], [6, 7, 78, 79], [15, 16, 33, 34], [15, 16, 42, 43], [15, 16, 51, 52],
		[15, 16, 60, 61], [15, 16, 69, 70], [15, 16, 78, 79], [24, 25, 33, 34], [24, 25, 42, 43], [24, 25, 51, 52], [24, 25, 60, 61], [24, 25, 69, 70], [24, 25, 78, 79],
		[33, 34, 60, 61], [33, 34, 69, 70], [33, 34, 78, 79], [42, 43, 60, 61], [42, 43, 69, 70], [42, 43, 78, 79], [51, 52, 60, 61], [51, 52, 69, 70], [51, 52, 78, 79],
		[6, 8, 33, 35], [6, 8, 42, 44], [6, 8, 51, 53], [6, 8, 60, 62], [6, 8, 69, 71], [6, 8, 78, 80], [15, 17, 33, 35], [15, 17, 42, 44], [15, 17, 51, 53],
		[15, 17, 60, 62], [15, 17, 69, 71], [15, 17, 78, 80], [24, 26, 33, 35], [24, 26, 42, 44], [24, 26, 51, 53], [24, 26, 60, 62], [24, 26, 69, 71], [24, 26, 78, 80],
		[33, 35, 60, 62], [33, 35, 69, 71], [33, 35, 78, 80], [42, 44, 60, 62], [42, 44, 69, 71], [42, 44, 78, 80], [51, 53, 60, 62], [51, 53, 69, 71], [51, 53, 78, 80],
		[7, 8, 34, 35], [7, 8, 43, 44], [7, 8, 52, 53], [7, 8, 61, 62], [7, 8, 70, 71], [7, 8, 79, 80], [16, 17, 34, 35], [16, 17, 43, 44], [16, 17, 52, 53],
		[16, 17, 61, 62], [16, 17, 70, 71], [16, 17, 79, 80], [25, 26, 34, 35], [25, 26, 43, 44], [25, 26, 52, 53], [25, 26, 61, 62], [25, 26, 70, 71], [25, 26, 79, 80],
		[34, 35, 61, 62], [34, 35, 70, 71], [34, 35, 79, 80], [43, 44, 61, 62], [43, 44, 70, 71], [43, 44, 79, 80], [52, 53, 61, 62], [52, 53, 70, 71], [52, 53, 79, 80]
	];
#pragma warning restore format


	/// <summary>
	/// Indicates whether the UR can be incomplete. In other words,
	/// some of UR candidates can be removed before the pattern forms.
	/// </summary>
	/// <remarks>
	/// For example, the complete pattern is:
	/// <code><![CDATA[
	/// ab | ab
	/// ab | ab
	/// ]]></code>
	/// This is a complete pattern, and we may remove an <c>ab</c> in a certain corner.
	/// The incomplete pattern may not contain all four <c>ab</c>s in the pattern.
	/// </remarks>
	[RuntimeIdentifier(RuntimeIdentifier.AllowIncompleteUniqueRectangles)]
	public bool AllowIncompleteUniqueRectangles { get; set; }

	/// <summary>
	/// Indicates whether the searcher can search for extended URs.
	/// </summary>
	/// <remarks>
	/// The basic types are type 1 to type 6, all other types are extended ones.
	/// </remarks>
	[RuntimeIdentifier(RuntimeIdentifier.SearchForExtendedUniqueRectangles)]
	public bool SearchForExtendedUniqueRectangles { get; set; }


	/// <inheritdoc/>
	protected internal override Step? Collect(scoped ref AnalysisContext context)
	{
		var list = new List<UniqueRectangleStep>();

		scoped ref readonly var grid = ref context.Grid;

		// Iterate on mode (whether use AR or UR mode to search).
		Collect(list, in grid, ref context, false);
		Collect(list, in grid, ref context, true);

		if (list.Count == 0)
		{
			return null;
		}

		// Sort and remove duplicate instances if worth.
		var resultList = EquatableStep.Distinct(list) switch
		{
			UniqueRectangleStep[] a => [.. a],
			HashSet<UniqueRectangleStep> s => [.. s],
			List<UniqueRectangleStep> l => l
		};
		ComparableStep.Order(resultList);

		if (context.OnlyFindOne)
		{
			return resultList[0];
		}

		context.Accumulator.AddRange(resultList);
		return null;
	}

	/// <summary>
	/// Get all possible hints from the grid.
	/// </summary>
	/// <param name="gathered"><inheritdoc cref="AnalysisContext.Accumulator" path="/summary"/></param>
	/// <param name="grid"><inheritdoc cref="AnalysisContext.Grid" path="/summary"/></param>
	/// <param name="context">
	/// <inheritdoc cref="Collect(ref AnalysisContext)" path="/param[@name='context']"/>
	/// </param>
	/// <param name="arMode">Indicates whether the current mode is searching for ARs.</param>
	private void Collect(List<UniqueRectangleStep> gathered, scoped ref readonly Grid grid, scoped ref AnalysisContext context, bool arMode)
	{
		// Search for ALSes. This result will be used by UR External ALS-XZ structures.
		scoped var alses = AlmostLockedSetsModule.CollectAlmostLockedSets(in grid);

		// Iterate on each possible UR pattern.
		for (var index = 0; index < UniqueRectanglePatterns.Length; index++)
		{
			var urCells = UniqueRectanglePatterns[index];

			// Check preconditions.
			if (!CheckPreconditions(in grid, urCells, arMode))
			{
				continue;
			}

			// Get all candidates that all four cells appeared.
			var mask = grid[[.. urCells]];

			// Iterate on each possible digit combination.
			scoped var allDigitsInThem = mask.GetAllSets();
			for (var (i, length) = (0, allDigitsInThem.Length); i < length - 1; i++)
			{
				var d1 = allDigitsInThem[i];
				for (var j = i + 1; j < length; j++)
				{
					var d2 = allDigitsInThem[j];

					// All possible UR patterns should contain at least one cell that contains both 'd1' and 'd2'.
					var comparer = (Mask)(1 << d1 | 1 << d2);
					var isNotPossibleUr = true;
					foreach (var cell in urCells)
					{
						if (PopCount((uint)(grid.GetCandidates(cell) & comparer)) == 2)
						{
							isNotPossibleUr = false;
							break;
						}
					}
					if (!arMode && isNotPossibleUr)
					{
						continue;
					}

					if (SearchForExtendedUniqueRectangles)
					{
						CheckBabaGroupingUnique(gathered, in grid, ref context, urCells, comparer, d1, d2, index);
						CheckExternalType1Or2(gathered, in grid, ref context, urCells, d1, d2, index, arMode);
						CheckExternalType3(gathered, in grid, ref context, urCells, comparer, d1, d2, index, arMode);
						CheckExternalType4(gathered, in grid, ref context, urCells, comparer, d1, d2, index, arMode);
						CheckExternalXyWing(gathered, in grid, ref context, urCells, comparer, d1, d2, index, arMode);
						CheckExternalAlmostLockedSetsXz(gathered, in grid, ref context, urCells, alses, comparer, d1, d2, index, arMode);

						if (!arMode)
						{
							CheckExternalTurbotFish(gathered, in grid, ref context, urCells, comparer, d1, d2, index);
							CheckExternalWWing(gathered, in grid, ref context, urCells, comparer, d1, d2, index);
						}
					}

					// Iterate on each corner of four cells.
					for (var c1 = 0; c1 < 4; c1++)
					{
						var corner1 = urCells[c1];
						var otherCellsMap = (CellMap)urCells - corner1;

						CheckType1(gathered, in grid, ref context, urCells, arMode, comparer, d1, d2, corner1, in otherCellsMap, index);
						CheckType5(gathered, in grid, ref context, urCells, arMode, comparer, d1, d2, corner1, in otherCellsMap, index);
						CheckHidden(gathered, in grid, ref context, urCells, arMode, comparer, d1, d2, corner1, in otherCellsMap, index);

						if (!arMode && SearchForExtendedUniqueRectangles)
						{
							Check3X(gathered, in grid, ref context, urCells, false, comparer, d1, d2, corner1, in otherCellsMap, index);
							Check3X2SL(gathered, in grid, ref context, urCells, false, comparer, d1, d2, corner1, in otherCellsMap, index);
							Check3N2SL(gathered, in grid, ref context, urCells, false, comparer, d1, d2, corner1, in otherCellsMap, index);
							Check3U2SL(gathered, in grid, ref context, urCells, false, comparer, d1, d2, corner1, in otherCellsMap, index);
							Check3E2SL(gathered, in grid, ref context, urCells, false, comparer, d1, d2, corner1, in otherCellsMap, index);
						}

						// If we aim to a single cell, all four cells should be checked.
						// Therefore, the 'break' clause should be written here, rather than continuing the execution.
						// In addition, I think you may ask me a question why the outer for loop is limited
						// the variable 'c1' from 0 to 4 instead of 0 to 3. If so, we'll miss the cases for checking the last cell.
						if (c1 == 3)
						{
							break;
						}

						for (var c2 = c1 + 1; c2 < 4; c2++)
						{
							var corner2 = urCells[c2];
							var tempOtherCellsMap = otherCellsMap - corner2;

							// Both diagonal and non-diagonal.
							CheckType2(gathered, in grid, ref context, urCells, arMode, comparer, d1, d2, corner1, corner2, in tempOtherCellsMap, index);

							if (SearchForExtendedUniqueRectangles)
							{
								CheckRegularWing(gathered, in grid, ref context, urCells, arMode, comparer, d1, d2, corner1, corner2, in tempOtherCellsMap, index, (c1, c2) is (0, 3) or (1, 2));
								//CheckWWing(gathered, in grid, ref context, urCells, arMode, comparer, d1, d2, corner1, corner2, in tempOtherCellsMap, index);
							}

							switch (c1, c2)
							{
								// Diagonal type.
								case (0, 3) or (1, 2):
								{
									if (arMode)
									{
										if (SearchForExtendedUniqueRectangles)
										{
											CheckHiddenSingleAvoidable(gathered, in grid, ref context, urCells, d1, d2, corner1, corner2, in tempOtherCellsMap, index);
										}
									}
									else
									{
										CheckType6(gathered, in grid, ref context, urCells, false, comparer, d1, d2, corner1, corner2, in tempOtherCellsMap, index);

										if (SearchForExtendedUniqueRectangles)
										{
											Check2D(gathered, in grid, ref context, urCells, false, comparer, d1, d2, corner1, corner2, in tempOtherCellsMap, index);
											Check2D1SL(gathered, in grid, ref context, urCells, false, comparer, d1, d2, corner1, corner2, in tempOtherCellsMap, index);
										}
									}

									break;
								}

								// Non-diagonal type.
								default:
								{
									CheckType3(gathered, in grid, ref context, urCells, arMode, comparer, d1, d2, corner1, corner2, in tempOtherCellsMap, index);

									if (!arMode)
									{
										CheckType4(gathered, in grid, ref context, urCells, false, comparer, d1, d2, corner1, corner2, in tempOtherCellsMap, index);

										if (SearchForExtendedUniqueRectangles)
										{
											Check2B1SL(gathered, in grid, ref context, urCells, false, comparer, d1, d2, corner1, corner2, in tempOtherCellsMap, index);
											Check4X3SL(gathered, in grid, ref context, urCells, false, comparer, d1, d2, corner1, corner2, in tempOtherCellsMap, index);
											Check4C3SL(gathered, in grid, ref context, urCells, false, comparer, d1, d2, corner1, corner2, in tempOtherCellsMap, index);
										}
									}

									if (SearchForExtendedUniqueRectangles)
									{
										CheckSueDeCoq(gathered, in grid, ref context, urCells, arMode, comparer, d1, d2, corner1, corner2, in tempOtherCellsMap, index);
									}

									break;
								}
							}
						}
					}
				}
			}
		}
	}


	/// <summary>
	/// Check type 1.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="cornerCell">The corner cell.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The pattern:
	/// <code><![CDATA[
	///   ↓ cornerCell
	/// (abc) ab
	///  ab   ab
	/// ]]></code>
	/// </remarks>
	private void CheckType1(
		List<UniqueRectangleStep> accumulator,
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
		Cell[] urCells,
		bool arMode,
		Mask comparer,
		Digit d1,
		Digit d2,
		Cell cornerCell,
		scoped ref readonly CellMap otherCellsMap,
		int index
	)
	{
		// Get the summary mask.
		var mask = grid[in otherCellsMap];
		if (mask != comparer)
		{
			return;
		}

		// Type 1 found. Now check elimination.
		var d1Exists = CandidatesMap[d1].Contains(cornerCell);
		var d2Exists = CandidatesMap[d2].Contains(cornerCell);
		if (!d1Exists && !d2Exists)
		{
			return;
		}

		var conclusions = new List<Conclusion>(2);
		if (d1Exists)
		{
			conclusions.Add(new(Elimination, cornerCell, d1));
		}
		if (d2Exists)
		{
			conclusions.Add(new(Elimination, cornerCell, d2));
		}
		if (conclusions.Count == 0)
		{
			return;
		}

		var candidateOffsets = new List<CandidateViewNode>(6);
		foreach (var cell in otherCellsMap)
		{
			foreach (var digit in grid.GetCandidates(cell))
			{
				candidateOffsets.Add(new(WellKnownColorIdentifier.Normal, cell * 9 + digit));
			}
		}

		if (!AllowIncompleteUniqueRectangles && (candidateOffsets.Count, conclusions.Count) != (6, 2))
		{
			return;
		}

		accumulator.Add(
			new UniqueRectangleType1Step(
				[.. conclusions],
				[[.. arMode ? GetHighlightCells(urCells) : [], .. arMode ? [] : candidateOffsets]],
				context.PredefinedOptions,
				d1,
				d2,
				[.. urCells],
				arMode,
				index,
				PopCount((uint)grid.GetCandidates(cornerCell)) - conclusions.Count,
				urCells.Count(EmptyCells.Contains)
			)
		);
	}

	/// <summary>
	/// Check type 2.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="corner1">The corner cell 1.</param>
	/// <param name="corner2">The corner cell 2.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The pattern:
	/// <code><![CDATA[
	///   ↓ corner1, corner2
	/// (abc) (abc)
	///  ab    ab
	/// ]]></code>
	/// </remarks>
	private void CheckType2(
		List<UniqueRectangleStep> accumulator,
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
		Cell[] urCells,
		bool arMode,
		Mask comparer,
		Digit d1,
		Digit d2,
		Cell corner1,
		Cell corner2,
		scoped ref readonly CellMap otherCellsMap,
		int index
	)
	{
		// Get the summary mask.
		var mask = grid[in otherCellsMap];
		if (mask != comparer)
		{
			return;
		}

		// Gets the extra mask.
		// If the mask is the power of 2, the type 2 will be formed.
		var extraMask = (grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) ^ comparer;
		if (extraMask == 0 || (extraMask & extraMask - 1) != 0)
		{
			return;
		}

		// Type 2 or 5 found. Now check elimination.
		var extraDigit = TrailingZeroCount(extraMask);
		var elimMap = (CellsMap[corner1] + corner2).PeerIntersection & CandidatesMap[extraDigit];
		if (!elimMap)
		{
			return;
		}

		var candidateOffsets = new List<CandidateViewNode>();
		var extraCells = CellMap.Empty;
		foreach (var cell in urCells)
		{
			if (grid.GetState(cell) == CellState.Empty)
			{
				foreach (var digit in grid.GetCandidates(cell))
				{
					candidateOffsets.Add(
						new(
							digit == extraDigit ? WellKnownColorIdentifier.Auxiliary1 : WellKnownColorIdentifier.Normal,
							cell * 9 + digit
						)
					);
				}

				if (CandidatesMap[extraDigit].Contains(cell))
				{
					extraCells.Add(cell);
				}
			}
		}

		if (IsIncomplete(AllowIncompleteUniqueRectangles, candidateOffsets))
		{
			return;
		}

		var isType5 = !(CellsMap[corner1] + corner2).InOneHouse(out _);
		accumulator.Add(
			new UniqueRectangleType2Step(
				[.. from cell in elimMap select new Conclusion(Elimination, cell, extraDigit)],
				[[.. arMode ? GetHighlightCells(urCells) : [], .. candidateOffsets]],
				context.PredefinedOptions,
				d1,
				d2,
				(arMode, isType5) switch
				{
					(true, true) => Technique.AvoidableRectangleType5,
					(true, false) => Technique.AvoidableRectangleType2,
					(false, true) => Technique.UniqueRectangleType5,
					_ => Technique.UniqueRectangleType2
				},
				[.. urCells],
				arMode,
				extraDigit,
				index,
				in extraCells,
				urCells.Count(EmptyCells.Contains)
			)
		);
	}

	/// <summary>
	/// Check type 3.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="corner1">The corner cell 1.</param>
	/// <param name="corner2">The corner cell 2.</param>
	/// <param name="otherCellsMap">
	/// The map of other cells during the current UR searching.
	/// </param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The pattern:
	/// <code><![CDATA[
	///  ↓ corner1, corner2
	/// (ab ) (ab )
	///  abx   aby
	/// ]]></code>
	/// </remarks>
	private void CheckType3(
		List<UniqueRectangleStep> accumulator,
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
		Cell[] urCells,
		bool arMode,
		Mask comparer,
		Digit d1,
		Digit d2,
		Cell corner1,
		Cell corner2,
		scoped ref readonly CellMap otherCellsMap,
		int index
	)
	{
		var notSatisfiedType3 = false;
		foreach (var cell in otherCellsMap)
		{
			var currentMask = grid.GetCandidates(cell);
			if ((currentMask & comparer) == 0 // The current cell does not contain a valid digit appeared in UR.
				|| currentMask == comparer // The current cell contains both digits appeared in UR.
				|| !arMode && grid.GetState(cell) != CellState.Empty) // The current cell is not empty.
			{
				notSatisfiedType3 = true;
				break;
			}
		}
		if (notSatisfiedType3)
		{
			return;
		}

		if ((grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) != comparer)
		{
			return;
		}

		var mask = grid[in otherCellsMap];
		if ((mask & comparer) != comparer)
		{
			return;
		}

		var otherDigitsMask = (Mask)(mask ^ comparer);
		foreach (var houseIndex in otherCellsMap.CoveredHouses)
		{
			if ((ValuesMap[d1] || ValuesMap[d2]) && HousesMap[houseIndex])
			{
				return;
			}

			var iterationMap = (HousesMap[houseIndex] & EmptyCells) - otherCellsMap;
			for (var size = PopCount((uint)otherDigitsMask) - 1; size < iterationMap.Count; size++)
			{
				foreach (ref readonly var iteratedCells in iterationMap.GetSubsets(size))
				{
					var tempMask = grid[in iteratedCells];
					if ((tempMask & comparer) != 0 || PopCount((uint)tempMask) - 1 != size || (tempMask & otherDigitsMask) != otherDigitsMask)
					{
						continue;
					}

					var conclusions = new List<Conclusion>(16);
					foreach (var digit in tempMask)
					{
						foreach (var cell in (iterationMap - iteratedCells) & CandidatesMap[digit])
						{
							conclusions.Add(new(Elimination, cell, digit));
						}
					}
					if (conclusions.Count == 0)
					{
						continue;
					}

					var cellOffsets = new List<CellViewNode>();
					foreach (var cell in urCells)
					{
						if (grid.GetState(cell) != CellState.Empty)
						{
							cellOffsets.Add(new(WellKnownColorIdentifier.Normal, cell));
						}
					}

					var candidateOffsets = new List<CandidateViewNode>();
					foreach (var cell in urCells)
					{
						if (grid.GetState(cell) == CellState.Empty)
						{
							foreach (var digit in grid.GetCandidates(cell))
							{
								candidateOffsets.Add(
									new(
										(tempMask >> digit & 1) != 0 ? WellKnownColorIdentifier.Auxiliary1 : WellKnownColorIdentifier.Normal,
										cell * 9 + digit
									)
								);
							}
						}
					}
					foreach (var cell in iteratedCells)
					{
						foreach (var digit in grid.GetCandidates(cell))
						{
							candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary1, cell * 9 + digit));
						}
					}

					accumulator.Add(
						new UniqueRectangleType3Step(
							[.. conclusions],
							[[.. arMode ? cellOffsets : [], .. candidateOffsets, new HouseViewNode(WellKnownColorIdentifier.Normal, houseIndex)]],
							context.PredefinedOptions,
							d1,
							d2,
							[.. urCells],
							in iteratedCells,
							otherDigitsMask,
							houseIndex,
							arMode,
							urCells.Count(EmptyCells.Contains),
							index
						)
					);
				}
			}
		}
	}

	/// <summary>
	/// Check type 4.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="corner1">The corner cell 1.</param>
	/// <param name="corner2">The corner cell 2.</param>
	/// <param name="otherCellsMap">
	/// The map of other cells during the current UR searching.
	/// </param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The pattern:
	/// <code><![CDATA[
	///  ↓ corner1, corner2
	/// (ab ) ab
	///  abx  aby
	/// ]]></code>
	/// </remarks>
	private void CheckType4(
		List<UniqueRectangleStep> accumulator,
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
		Cell[] urCells,
		bool arMode,
		Mask comparer,
		Digit d1,
		Digit d2,
		Cell corner1,
		Cell corner2,
		scoped ref readonly CellMap otherCellsMap,
		int index
	)
	{
		if ((grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) != comparer)
		{
			return;
		}

		foreach (var houseIndex in otherCellsMap.CoveredHouses)
		{
			if (houseIndex < 9)
			{
				// Process the case in lines.
				continue;
			}

			foreach (var digit in (d1, d2))
			{
				if (!IsConjugatePair(digit, in otherCellsMap, houseIndex))
				{
					continue;
				}

				// Yes, Type 4 found.
				// Now check elimination.
				var elimDigit = TrailingZeroCount(comparer ^ (1 << digit));
				if ((otherCellsMap & CandidatesMap[elimDigit]) is not (var elimMap and not []))
				{
					continue;
				}

				var candidateOffsets = new List<CandidateViewNode>(6);
				foreach (var cell in urCells)
				{
					if (grid.GetState(cell) != CellState.Empty)
					{
						continue;
					}

					if (otherCellsMap.Contains(cell))
					{
						if (d1 != elimDigit && CandidatesMap[d1].Contains(cell))
						{
							candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary1, cell * 9 + d1));
						}
						if (d2 != elimDigit && CandidatesMap[d2].Contains(cell))
						{
							candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary1, cell * 9 + d2));
						}
					}
					else
					{
						// Corner1 and corner2.
						foreach (var d in grid.GetCandidates(cell))
						{
							candidateOffsets.Add(new(WellKnownColorIdentifier.Normal, cell * 9 + d));
						}
					}
				}

				var conclusions = from cell in elimMap select new Conclusion(Elimination, cell, elimDigit);
				if (!AllowIncompleteUniqueRectangles && (candidateOffsets.Count, conclusions.Length) != (6, 2))
				{
					continue;
				}

				accumulator.Add(
					new UniqueRectangleWithConjugatePairStep(
						[.. conclusions],
						[
							[
								.. arMode ? GetHighlightCells(urCells) : [],
								.. candidateOffsets,
								new HouseViewNode(WellKnownColorIdentifier.Normal, houseIndex)
							]
						],
						context.PredefinedOptions,
						Technique.UniqueRectangleType4,
						d1,
						d2,
						[.. urCells],
						arMode,
						[new(otherCellsMap[0], otherCellsMap[1], digit)],
						index
					)
				);
			}
		}
	}

	/// <summary>
	/// Check type 5.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="cornerCell">The corner cell.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The pattern:
	/// <code><![CDATA[
	///  ↓ cornerCell
	/// (ab ) abc
	///  abc  abc
	/// ]]></code>
	/// </remarks>
	private void CheckType5(
		List<UniqueRectangleStep> accumulator,
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
		Cell[] urCells,
		bool arMode,
		Mask comparer,
		Digit d1,
		Digit d2,
		Cell cornerCell,
		scoped ref readonly CellMap otherCellsMap,
		int index
	)
	{
		if (grid.GetCandidates(cornerCell) != comparer)
		{
			return;
		}

		// Get the summary mask.
		var otherCellsMask = grid[in otherCellsMap];

		// Degenerate to type 1.
		var extraMask = (Mask)(otherCellsMask ^ comparer);
		if ((extraMask & extraMask - 1) != 0)
		{
			return;
		}

		// Type 5 found. Now check elimination.
		var extraDigit = TrailingZeroCount(extraMask);
		var cellsThatContainsExtraDigit = otherCellsMap & CandidatesMap[extraDigit];

		// Degenerate to type 1.
		if (cellsThatContainsExtraDigit.Count == 1)
		{
			return;
		}

		if ((cellsThatContainsExtraDigit.PeerIntersection & CandidatesMap[extraDigit]) is not (var elimMap and not []))
		{
			return;
		}

		var candidateOffsets = new List<CandidateViewNode>(16);
		var extraCells = CellMap.Empty;
		foreach (var cell in urCells)
		{
			if (grid.GetState(cell) != CellState.Empty)
			{
				continue;
			}

			foreach (var digit in grid.GetCandidates(cell))
			{
				candidateOffsets.Add(new(digit == extraDigit ? WellKnownColorIdentifier.Auxiliary1 : WellKnownColorIdentifier.Normal, cell * 9 + digit));
			}

			if (CandidatesMap[extraDigit].Contains(cell))
			{
				extraCells.Add(cell);
			}
		}
		if (IsIncomplete(AllowIncompleteUniqueRectangles, candidateOffsets))
		{
			return;
		}

		accumulator.Add(
			new UniqueRectangleType2Step(
				[.. from cell in elimMap select new Conclusion(Elimination, cell, extraDigit)],
				[[.. arMode ? GetHighlightCells(urCells) : [], .. candidateOffsets]],
				context.PredefinedOptions,
				d1,
				d2,
				arMode ? Technique.AvoidableRectangleType5 : Technique.UniqueRectangleType5,
				[.. urCells],
				arMode,
				extraDigit,
				index,
				in extraCells,
				urCells.Count(EmptyCells.Contains)
			)
		);
	}

	/// <summary>
	/// Check type 6.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="corner1">The corner cell 1.</param>
	/// <param name="corner2">The corner cell 2.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The pattern:
	/// <code><![CDATA[
	///  ↓ corner1
	/// (ab )  aby
	///  abx  (ab)
	///        ↑corner2
	/// ]]></code>
	/// </remarks>
	private void CheckType6(
		List<UniqueRectangleStep> accumulator,
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
		Cell[] urCells,
		bool arMode,
		Mask comparer,
		Digit d1,
		Digit d2,
		Cell corner1,
		Cell corner2,
		scoped ref readonly CellMap otherCellsMap,
		int index
	)
	{
		if ((grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) != comparer)
		{
			return;
		}

		var o1 = otherCellsMap[0];
		var o2 = otherCellsMap[1];
		var r1 = corner1.ToHouseIndex(HouseType.Row);
		var c1 = corner1.ToHouseIndex(HouseType.Column);
		var r2 = corner2.ToHouseIndex(HouseType.Row);
		var c2 = corner2.ToHouseIndex(HouseType.Column);
		foreach (var digit in (d1, d2))
		{
			foreach (var (h1, h2) in ((r1, r2), (c1, c2)))
			{
				gather(in grid, ref context, in otherCellsMap, h1 is >= 9 and < 18, digit, h1, h2);
			}
		}


		void gather(scoped ref readonly Grid grid, scoped ref AnalysisContext context, scoped ref readonly CellMap otherCellsMap, bool isRow, Digit digit, House house1, House house2)
		{
			var precheck = isRow && IsConjugatePair(digit, [corner1, o1], house1) && IsConjugatePair(digit, [corner2, o2], house2)
				|| !isRow && IsConjugatePair(digit, [corner1, o2], house1) && IsConjugatePair(digit, [corner2, o1], house2);
			if (!precheck)
			{
				return;
			}

			// Check eliminations.
			if ((otherCellsMap & CandidatesMap[digit]) is not (var elimMap and not []))
			{
				return;
			}

			var candidateOffsets = new List<CandidateViewNode>(6);
			foreach (var cell in urCells)
			{
				if (otherCellsMap.Contains(cell))
				{
					if (d1 != digit && CandidatesMap[d1].Contains(cell))
					{
						candidateOffsets.Add(new(WellKnownColorIdentifier.Normal, cell * 9 + d1));
					}
					if (d2 != digit && CandidatesMap[d2].Contains(cell))
					{
						candidateOffsets.Add(new(WellKnownColorIdentifier.Normal, cell * 9 + d2));
					}
				}
				else
				{
					foreach (var d in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(new(d == digit ? WellKnownColorIdentifier.Auxiliary1 : WellKnownColorIdentifier.Normal, cell * 9 + d));
					}
				}
			}

			var conclusions = from cell in elimMap select new Conclusion(Elimination, cell, digit);
			if (!AllowIncompleteUniqueRectangles && (candidateOffsets.Count, conclusions.Length) != (6, 2))
			{
				return;
			}

			accumulator.Add(
				new UniqueRectangleWithConjugatePairStep(
					[.. conclusions],
					[
						[
							.. arMode ? GetHighlightCells(urCells) : [],
							.. candidateOffsets,
							new HouseViewNode(WellKnownColorIdentifier.Normal, house1),
							new HouseViewNode(WellKnownColorIdentifier.Normal, house2)
						]
					],
					context.PredefinedOptions,
					Technique.UniqueRectangleType6,
					d1,
					d2,
					[.. urCells],
					false,
					[new(corner1, isRow ? o1 : o2, digit), new(corner2, isRow ? o2 : o1, digit)],
					index
				)
			);
		}
	}

	/// <summary>
	/// Check hidden UR.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="cornerCell">The corner cell.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The pattern:
	/// <code><![CDATA[
	///  ↓ cornerCell
	/// (ab ) abx
	///  aby  abz
	/// ]]></code>
	/// </remarks>
	private void CheckHidden(
		List<UniqueRectangleStep> accumulator,
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
		Cell[] urCells,
		bool arMode,
		Mask comparer,
		Digit d1,
		Digit d2,
		Cell cornerCell,
		scoped ref readonly CellMap otherCellsMap,
		int index
	)
	{
		var cells = (CellMap)urCells;
		if (!arMode && grid.GetCandidates(cornerCell) != comparer || arMode && (EmptyCells & cells) != otherCellsMap)
		{
			return;
		}

		var abzCell = GetDiagonalCell(urCells, cornerCell);
		var adjacentCellsMap = otherCellsMap - abzCell;
		var abxCell = adjacentCellsMap[0];
		var abyCell = adjacentCellsMap[1];
		var r = abzCell.ToHouseIndex(HouseType.Row);
		var c = abzCell.ToHouseIndex(HouseType.Column);
		foreach (var digit in (d1, d2))
		{
			var map1 = CellsMap[abzCell] + abxCell;
			var map2 = CellsMap[abzCell] + abyCell;
			if (map1.CoveredLine is not (var m1cl and not InvalidTrailingZeroCountMethodFallback)
				|| map2.CoveredLine is not (var m2cl and not InvalidTrailingZeroCountMethodFallback))
			{
				// There's no common covered line to display.
				continue;
			}

			if (!IsConjugatePair(digit, in map1, m1cl) || !IsConjugatePair(digit, in map2, m2cl))
			{
				continue;
			}

			// Determine whether the Hidden ARs don't use unrelated digits.
			if (arMode && ((1 << grid.GetDigit(cornerCell)) | 1 << digit) != comparer)
			{
				continue;
			}

			// Hidden UR/AR found. Now check eliminations.
			var elimDigit = TrailingZeroCount(comparer ^ (1 << digit));
			if (!CandidatesMap[elimDigit].Contains(abzCell))
			{
				continue;
			}

			var candidateOffsets = new List<CandidateViewNode>();
			foreach (var cell in urCells)
			{
				if (grid.GetState(cell) != CellState.Empty)
				{
					continue;
				}

				if (otherCellsMap.Contains(cell))
				{
					if ((cell != abzCell || d1 != elimDigit) && CandidatesMap[d1].Contains(cell))
					{
						candidateOffsets.Add(new(d1 != elimDigit ? WellKnownColorIdentifier.Auxiliary1 : WellKnownColorIdentifier.Normal, cell * 9 + d1));
					}
					if ((cell != abzCell || d2 != elimDigit) && CandidatesMap[d2].Contains(cell))
					{
						candidateOffsets.Add(new(d2 != elimDigit ? WellKnownColorIdentifier.Auxiliary1 : WellKnownColorIdentifier.Normal, cell * 9 + d2));
					}
				}
				else
				{
					foreach (var d in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(new(WellKnownColorIdentifier.Normal, cell * 9 + d));
					}
				}
			}

			if (!AllowIncompleteUniqueRectangles && candidateOffsets.Count != 7)
			{
				continue;
			}

			accumulator.Add(
				new HiddenUniqueRectangleStep(
					[new(Elimination, abzCell, elimDigit)],
					[
						[
							.. arMode ? GetHighlightCells(urCells) : [],
							.. candidateOffsets,
							new HouseViewNode(WellKnownColorIdentifier.Normal, r),
							new HouseViewNode(WellKnownColorIdentifier.Normal, c)
						]
					],
					context.PredefinedOptions,
					d1,
					d2,
					[.. urCells],
					arMode,
					[new(abzCell, abxCell, digit), new(abzCell, abyCell, digit)],
					index
				)
			);
		}
	}

	/// <summary>
	/// Check UR + 2D.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="corner1">The corner cell 1.</param>
	/// <param name="corner2">The corner cell 2.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The pattern:
	/// <code><![CDATA[
	///   ↓ corner1
	/// (ab )  abx
	///  aby  (ab )  xy  *
	///         ↑ corner2
	/// ]]></code>
	/// </remarks>
	private void Check2D(
		List<UniqueRectangleStep> accumulator,
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
		Cell[] urCells,
		bool arMode,
		Mask comparer,
		Digit d1,
		Digit d2,
		Cell corner1,
		Cell corner2,
		scoped ref readonly CellMap otherCellsMap,
		int index
	)
	{
		if ((grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) != comparer)
		{
			return;
		}

		var o1 = grid.GetCandidates(otherCellsMap[0]);
		var o2 = grid.GetCandidates(otherCellsMap[1]);
		var o = (Mask)(o1 | o2);
		if (PopCount((uint)o) != 4 || PopCount((uint)o1) > 3 || PopCount((uint)o2) > 3
			|| (o1 & comparer) == 0 || (o2 & comparer) == 0
			|| (o & comparer) != comparer)
		{
			return;
		}

		var xyMask = (Mask)(o ^ comparer);
		var x = TrailingZeroCount(xyMask);
		var y = xyMask.GetNextSet(x);
		var inter = otherCellsMap.PeerIntersection - (CellMap)urCells;
		foreach (var possibleXyCell in inter)
		{
			if (grid.GetCandidates(possibleXyCell) != xyMask)
			{
				continue;
			}

			// 'xy' found.
			// Now check eliminations.
			var elimMap = inter & PeersMap[possibleXyCell];
			var conclusions = new List<Conclusion>(10);
			foreach (var cell in elimMap)
			{
				if (CandidatesMap[x].Contains(cell))
				{
					conclusions.Add(new(Elimination, cell, x));
				}
				if (CandidatesMap[y].Contains(cell))
				{
					conclusions.Add(new(Elimination, cell, y));
				}
			}
			if (conclusions.Count == 0)
			{
				continue;
			}

			var candidateOffsets = new List<CandidateViewNode>(10);
			foreach (var cell in urCells)
			{
				if (otherCellsMap.Contains(cell))
				{
					foreach (var digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(new((comparer >> digit & 1) == 0 ? WellKnownColorIdentifier.Auxiliary1 : WellKnownColorIdentifier.Normal, cell * 9 + digit));
					}
				}
				else
				{
					foreach (var digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(new(WellKnownColorIdentifier.Normal, cell * 9 + digit));
					}
				}
			}
			foreach (var digit in xyMask)
			{
				candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary1, possibleXyCell * 9 + digit));
			}

			if (IsIncomplete(AllowIncompleteUniqueRectangles, candidateOffsets))
			{
				return;
			}

			accumulator.Add(
				new UniqueRectangle2DOr3XStep(
					[.. conclusions],
					[[.. arMode ? GetHighlightCells(urCells) : [], .. candidateOffsets]],
					context.PredefinedOptions,
					arMode ? Technique.AvoidableRectangle2D : Technique.UniqueRectangle2D,
					d1,
					d2,
					[.. urCells],
					arMode,
					x,
					y,
					possibleXyCell,
					index
				)
			);
		}
	}

	/// <summary>
	/// Check UR + 2B/1SL.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="corner1">The corner cell 1.</param>
	/// <param name="corner2">The corner cell 2.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The pattern:
	/// <code><![CDATA[
	///   ↓ corner1, corner2
	/// (ab )  (ab )
	///  |
	///  | a
	///  |
	///  abx    aby
	/// ]]></code>
	/// </remarks>
	private void Check2B1SL(
		List<UniqueRectangleStep> accumulator,
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
		Cell[] urCells,
		bool arMode,
		Mask comparer,
		Digit d1,
		Digit d2,
		Cell corner1,
		Cell corner2,
		scoped ref readonly CellMap otherCellsMap,
		int index
	)
	{
		if ((grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) != comparer)
		{
			return;
		}

		scoped var digits = (ReadOnlySpan<Digit>)([d1, d2]);
		foreach (var cell in (corner1, corner2))
		{
			foreach (var otherCell in otherCellsMap)
			{
				if (!IsSameHouseCell(cell, otherCell, out var houses))
				{
					continue;
				}

				foreach (var house in houses)
				{
					if (house < 9)
					{
						continue;
					}

					for (var digitIndex = 0; digitIndex < 2; digitIndex++)
					{
						var digit = digits[digitIndex];
						if (!IsConjugatePair(digit, [cell, otherCell], house))
						{
							continue;
						}

						var elimCell = (otherCellsMap - otherCell)[0];
						if (!CandidatesMap[digit].Contains(otherCell))
						{
							continue;
						}

						var elimDigit = TrailingZeroCount(comparer ^ (1 << digit));
						var conclusions = new List<Conclusion>(4);
						if (CandidatesMap[elimDigit].Contains(elimCell))
						{
							conclusions.Add(new(Elimination, elimCell, elimDigit));
						}
						if (conclusions.Count == 0)
						{
							continue;
						}

						var candidateOffsets = new List<CandidateViewNode>(10);
						foreach (var urCell in urCells)
						{
							if (urCell == corner1 || urCell == corner2)
							{
								var coveredHouses = (CellsMap[urCell] + otherCell).CoveredHouses;
								if ((coveredHouses >> house & 1) != 0)
								{
									foreach (var d in grid.GetCandidates(urCell))
									{
										candidateOffsets.Add(
											new(
												d == digit ? WellKnownColorIdentifier.Auxiliary1 : WellKnownColorIdentifier.Normal,
												urCell * 9 + d
											)
										);
									}
								}
								else
								{
									foreach (var d in grid.GetCandidates(urCell))
									{
										candidateOffsets.Add(new(WellKnownColorIdentifier.Normal, urCell * 9 + d));
									}
								}
							}
							else if (urCell == otherCell || urCell == elimCell)
							{
								if (CandidatesMap[d1].Contains(urCell))
								{
									if (urCell != elimCell || d1 != elimDigit)
									{
										candidateOffsets.Add(
											new(
												urCell == elimCell
													? WellKnownColorIdentifier.Normal
													: d1 == digit ? WellKnownColorIdentifier.Auxiliary1 : WellKnownColorIdentifier.Normal,
												urCell * 9 + d1
											)
										);
									}
								}
								if (CandidatesMap[d2].Contains(urCell))
								{
									if (urCell != elimCell || d2 != elimDigit)
									{
										candidateOffsets.Add(
											new(
												urCell == elimCell
													? WellKnownColorIdentifier.Normal
													: d2 == digit ? WellKnownColorIdentifier.Auxiliary1 : WellKnownColorIdentifier.Normal,
												urCell * 9 + d2
											)
										);
									}
								}
							}
						}

						if (!AllowIncompleteUniqueRectangles && candidateOffsets.Count != 7)
						{
							continue;
						}

						accumulator.Add(
							new UniqueRectangleWithConjugatePairStep(
								[.. conclusions],
								[
									[
										.. arMode ? GetHighlightCells(urCells) : [],
										.. candidateOffsets,
										new HouseViewNode(WellKnownColorIdentifier.Normal, house)
									]
								],
								context.PredefinedOptions,
								Technique.UniqueRectangle2B1,
								d1,
								d2,
								[.. urCells],
								arMode,
								[new(cell, otherCell, digit)],
								index
							)
						);
					}
				}
			}
		}
	}

	/// <summary>
	/// Check UR + 2D/1SL.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="corner1">The corner cell 1.</param>
	/// <param name="corner2">The corner cell 2.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The pattern:
	/// <code><![CDATA[
	///   ↓ corner1
	/// (ab )   aby
	///  |
	///  | a
	///  |
	///  abx   (ab )
	///          ↑ corner2
	/// ]]></code>
	/// </remarks>
	private void Check2D1SL(
		List<UniqueRectangleStep> accumulator,
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
		Cell[] urCells,
		bool arMode,
		Mask comparer,
		Digit d1,
		Digit d2,
		Cell corner1,
		Cell corner2,
		scoped ref readonly CellMap otherCellsMap,
		int index
	)
	{
		if ((grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) != comparer)
		{
			return;
		}

		foreach (var cell in (corner1, corner2))
		{
			foreach (var otherCell in otherCellsMap)
			{
				if (!IsSameHouseCell(cell, otherCell, out var houses))
				{
					continue;
				}

				foreach (var house in houses)
				{
					if (house < 9)
					{
						continue;
					}

					foreach (var digit in (d1, d2))
					{
						if (!IsConjugatePair(digit, [cell, otherCell], house))
						{
							continue;
						}

						var elimCell = (otherCellsMap - otherCell)[0];
						if (!CandidatesMap[digit].Contains(otherCell))
						{
							continue;
						}

						var conclusions = new List<Conclusion>(4);
						if (CandidatesMap[digit].Contains(elimCell))
						{
							conclusions.Add(new(Elimination, elimCell, digit));
						}
						if (conclusions.Count == 0)
						{
							continue;
						}

						var candidateOffsets = new List<CandidateViewNode>(10);
						foreach (var urCell in urCells)
						{
							if (urCell == corner1 || urCell == corner2)
							{
								var flag = false;
								foreach (var r in (CellsMap[urCell] + otherCell).CoveredHouses)
								{
									if (r == house)
									{
										flag = true;
										break;
									}
								}

								if (flag)
								{
									foreach (var d in grid.GetCandidates(urCell))
									{
										candidateOffsets.Add(
											new(
												d == digit ? WellKnownColorIdentifier.Auxiliary1 : WellKnownColorIdentifier.Normal,
												urCell * 9 + d
											)
										);
									}
								}
								else
								{
									foreach (var d in grid.GetCandidates(urCell))
									{
										candidateOffsets.Add(new(WellKnownColorIdentifier.Normal, urCell * 9 + d));
									}
								}
							}
							else if (urCell == otherCell || urCell == elimCell)
							{
								if (CandidatesMap[d1].Contains(urCell) && (urCell != elimCell || d1 != digit))
								{
									candidateOffsets.Add(
										new(
											urCell == elimCell
												? WellKnownColorIdentifier.Normal
												: d1 == digit ? WellKnownColorIdentifier.Auxiliary1 : WellKnownColorIdentifier.Normal,
											urCell * 9 + d1
										)
									);
								}
								if (CandidatesMap[d2].Contains(urCell) && (urCell != elimCell || d2 != digit))
								{
									candidateOffsets.Add(
										new(
											urCell == elimCell
												? WellKnownColorIdentifier.Normal
												: d2 == digit ? WellKnownColorIdentifier.Auxiliary1 : WellKnownColorIdentifier.Normal,
											urCell * 9 + d2
										)
									);
								}
							}
						}

						if (!AllowIncompleteUniqueRectangles && candidateOffsets.Count != 7)
						{
							continue;
						}

						accumulator.Add(
							new UniqueRectangleWithConjugatePairStep(
								[.. conclusions],
								[
									[
										.. arMode ? GetHighlightCells(urCells) : [],
										.. candidateOffsets,
										new HouseViewNode(WellKnownColorIdentifier.Normal, house)
									]
								],
								context.PredefinedOptions,
								Technique.UniqueRectangle2D1,
								d1,
								d2,
								[.. urCells],
								arMode,
								[new(cell, otherCell, digit)],
								index
							)
						);
					}
				}
			}
		}
	}

	/// <summary>
	/// Check UR + 3X.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="cornerCell">The corner cell.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The pattern:
	/// <code><![CDATA[
	///  ↓ cornerCell
	/// (ab )  abx
	///  aby   abz   xy  *
	/// ]]></code>
	/// Note: <c>z</c> is <c>x</c> or <c>y</c>.
	/// </remarks>
	private void Check3X(
		List<UniqueRectangleStep> accumulator,
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
		Cell[] urCells,
		bool arMode,
		Mask comparer,
		Digit d1,
		Digit d2,
		Cell cornerCell,
		scoped ref readonly CellMap otherCellsMap,
		int index
	)
	{
		if (grid.GetCandidates(cornerCell) != comparer)
		{
			return;
		}

		var c1 = otherCellsMap[0];
		var c2 = otherCellsMap[1];
		var c3 = otherCellsMap[2];
		var m1 = grid.GetCandidates(c1);
		var m2 = grid.GetCandidates(c2);
		var m3 = grid.GetCandidates(c3);
		var mask = (Mask)((Mask)(m1 | m2) | m3);

		if (PopCount((uint)mask) != 4
			|| PopCount((uint)m1) > 3 || PopCount((uint)m2) > 3 || PopCount((uint)m3) > 3
			|| (m1 & comparer) == 0 || (m2 & comparer) == 0 || (m3 & comparer) == 0
			|| (mask & comparer) != comparer)
		{
			return;
		}

		var xyMask = (Mask)(mask ^ comparer);
		var x = TrailingZeroCount(xyMask);
		var y = xyMask.GetNextSet(x);
		var inter = otherCellsMap.PeerIntersection - [.. urCells];
		foreach (var possibleXyCell in inter)
		{
			if (grid.GetCandidates(possibleXyCell) != xyMask)
			{
				continue;
			}

			// Possible XY cell found.
			// Now check eliminations.
			var conclusions = new List<Conclusion>(10);
			foreach (var cell in inter & PeersMap[possibleXyCell])
			{
				if (CandidatesMap[x].Contains(cell))
				{
					conclusions.Add(new(Elimination, cell, x));
				}
				if (CandidatesMap[y].Contains(cell))
				{
					conclusions.Add(new(Elimination, cell, y));
				}
			}
			if (conclusions.Count == 0)
			{
				continue;
			}

			var candidateOffsets = new List<CandidateViewNode>(10);
			foreach (var cell in urCells)
			{
				if (otherCellsMap.Contains(cell))
				{
					foreach (var digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(
							new(
								(comparer >> digit & 1) == 0 ? WellKnownColorIdentifier.Auxiliary1 : WellKnownColorIdentifier.Normal,
								cell * 9 + digit
							)
						);
					}
				}
				else
				{
					foreach (var digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(new(WellKnownColorIdentifier.Normal, cell * 9 + digit));
					}
				}
			}
			foreach (var digit in xyMask)
			{
				candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary1, possibleXyCell * 9 + digit));
			}
			if (IsIncomplete(AllowIncompleteUniqueRectangles, candidateOffsets))
			{
				return;
			}

			accumulator.Add(
				new UniqueRectangle2DOr3XStep(
					[.. conclusions],
					[[.. arMode ? GetHighlightCells(urCells) : [], .. candidateOffsets]],
					context.PredefinedOptions,
					arMode ? Technique.AvoidableRectangle3X : Technique.UniqueRectangle3X,
					d1,
					d2,
					[.. urCells],
					arMode,
					x,
					y,
					possibleXyCell,
					index
				)
			);
		}
	}

	/// <summary>
	/// Check UR + 3X/2SL.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="cornerCell">The corner cell.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The pattern:
	/// <code><![CDATA[
	///  ↓ cornerCell
	/// (ab )    abx
	///           |
	///           | b
	///       a   |
	///  aby-----abz
	/// ]]></code>
	/// </remarks>
	private void Check3X2SL(
		List<UniqueRectangleStep> accumulator,
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
		Cell[] urCells,
		bool arMode,
		Mask comparer,
		Digit d1,
		Digit d2,
		Cell cornerCell,
		scoped ref readonly CellMap otherCellsMap,
		int index
	)
	{
		if (grid.GetCandidates(cornerCell) != comparer)
		{
			return;
		}

		var abzCell = GetDiagonalCell(urCells, cornerCell);
		var adjacentCellsMap = otherCellsMap - abzCell;
		foreach (var (a, b) in ((d1, d2), (d2, d1)))
		{
			var abxCell = adjacentCellsMap[0];
			var abyCell = adjacentCellsMap[1];
			var map1 = CellsMap[abzCell] + abxCell;
			var map2 = CellsMap[abzCell] + abyCell;
			if (!IsConjugatePair(b, in map1, map1.CoveredLine) || !IsConjugatePair(a, in map2, map2.CoveredLine))
			{
				continue;
			}

			var conclusions = new List<Conclusion>(2);
			if (CandidatesMap[a].Contains(abxCell))
			{
				conclusions.Add(new(Elimination, abxCell, a));
			}
			if (CandidatesMap[b].Contains(abyCell))
			{
				conclusions.Add(new(Elimination, abyCell, b));
			}
			if (conclusions.Count == 0)
			{
				continue;
			}

			var candidateOffsets = new List<CandidateViewNode>(6);
			foreach (var digit in grid.GetCandidates(abxCell))
			{
				if ((digit == d1 || digit == d2) && digit != a)
				{
					candidateOffsets.Add(new(digit == b ? WellKnownColorIdentifier.Auxiliary1 : WellKnownColorIdentifier.Normal, abxCell * 9 + digit));
				}
			}
			foreach (var digit in grid.GetCandidates(abyCell))
			{
				if ((digit == d1 || digit == d2) && digit != b)
				{
					candidateOffsets.Add(new(digit == a ? WellKnownColorIdentifier.Auxiliary1 : WellKnownColorIdentifier.Normal, abyCell * 9 + digit));
				}
			}
			foreach (var digit in grid.GetCandidates(abzCell))
			{
				if (digit == a || digit == b)
				{
					candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary1, abzCell * 9 + digit));
				}
			}
			foreach (var digit in comparer)
			{
				candidateOffsets.Add(new(WellKnownColorIdentifier.Normal, cornerCell * 9 + digit));
			}
			if (!AllowIncompleteUniqueRectangles && candidateOffsets.Count != 6)
			{
				continue;
			}

			accumulator.Add(
				new UniqueRectangleWithConjugatePairStep(
					[.. conclusions],
					[
						[
							.. arMode ? GetHighlightCells(urCells) : [],
							.. candidateOffsets,
							new HouseViewNode(WellKnownColorIdentifier.Normal, map1.CoveredLine),
							new HouseViewNode(WellKnownColorIdentifier.Auxiliary1, map2.CoveredLine)
						]
					],
					context.PredefinedOptions,
					Technique.UniqueRectangle3X2,
					d1,
					d2,
					[.. urCells],
					arMode,
					[new(abxCell, abzCell, b), new(abyCell, abzCell, a)],
					index
				)
			);
		}
	}

	/// <summary>
	/// Check UR + 3N/2SL.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="cornerCell">The corner cell.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The pattern:
	/// <code><![CDATA[
	///  ↓ cornerCell
	/// (ab )-----abx
	///        a   |
	///            | b
	///            |
	///  aby      abz
	/// ]]></code>
	/// </remarks>
	private void Check3N2SL(
		List<UniqueRectangleStep> accumulator,
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
		Cell[] urCells,
		bool arMode,
		Mask comparer,
		Digit d1,
		Digit d2,
		Cell cornerCell,
		scoped ref readonly CellMap otherCellsMap,
		int index
	)
	{
		if (grid.GetCandidates(cornerCell) != comparer)
		{
			return;
		}

		// Step 1: Get the diagonal cell of 'cornerCell' and determine the existence of strong link.
		var abzCell = GetDiagonalCell(urCells, cornerCell);
		var adjacentCellsMap = otherCellsMap - abzCell;
		var abxCell = adjacentCellsMap[0];
		var abyCell = adjacentCellsMap[1];
		scoped var digitPairs = (ReadOnlySpan<(Digit, Digit)>)([(d1, d2), (d2, d1)]);
		scoped var digits = (ReadOnlySpan<Digit>)([d1, d2]);
		foreach (var (begin, end) in ((abxCell, abyCell), (abyCell, abxCell)))
		{
			var linkMap = CellsMap[begin] + abzCell;
			foreach (var (a, b) in digitPairs)
			{
				if (!IsConjugatePair(b, in linkMap, linkMap.CoveredLine))
				{
					continue;
				}

				// Step 2: Get the link cell that is adjacent to 'cornerCell' and check the strong link.
				var secondLinkMap = CellsMap[cornerCell] + begin;
				if (!IsConjugatePair(a, in secondLinkMap, secondLinkMap.CoveredLine))
				{
					continue;
				}

				// Step 3: Check eliminations.
				if (!CandidatesMap[a].Contains(end))
				{
					continue;
				}

				// Step 4: Check highlight candidates.
				var candidateOffsets = new List<CandidateViewNode>(7);
				foreach (var d in comparer)
				{
					candidateOffsets.Add(new(d == a ? WellKnownColorIdentifier.Auxiliary1 : WellKnownColorIdentifier.Normal, cornerCell * 9 + d));
				}
				foreach (var d in digits)
				{
					if (CandidatesMap[d].Contains(abzCell))
					{
						candidateOffsets.Add(new(d == b ? WellKnownColorIdentifier.Auxiliary1 : WellKnownColorIdentifier.Normal, abzCell * 9 + d));
					}
				}
				foreach (var d in grid.GetCandidates(begin))
				{
					if (d == d1 || d == d2)
					{
						candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary1, begin * 9 + d));
					}
				}
				foreach (var d in grid.GetCandidates(end))
				{
					if ((d == d1 || d == d2) && d != a)
					{
						candidateOffsets.Add(new(WellKnownColorIdentifier.Normal, end * 9 + d));
					}
				}
				if (!AllowIncompleteUniqueRectangles && candidateOffsets.Count != 7)
				{
					continue;
				}

				var conjugatePairs = (Conjugate[])[new(cornerCell, begin, a), new(begin, abzCell, b)];
				accumulator.Add(
					new UniqueRectangleWithConjugatePairStep(
						[new(Elimination, end, a)],
						[
							[
								.. arMode ? GetHighlightCells(urCells) : [],
								.. candidateOffsets,
								new HouseViewNode(WellKnownColorIdentifier.Normal, conjugatePairs[0].Line),
								new HouseViewNode(WellKnownColorIdentifier.Auxiliary1, conjugatePairs[1].Line)
							]
						],
						context.PredefinedOptions,
						Technique.UniqueRectangle3N2,
						d1,
						d2,
						[.. urCells],
						arMode,
						conjugatePairs,
						index
					)
				);
			}
		}
	}

	/// <summary>
	/// Check UR + 3U/2SL.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="cornerCell">The corner cell.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The pattern:
	/// <code><![CDATA[
	///  ↓ cornerCell
	/// (ab )-----abx
	///        a
	///
	///        b
	///  aby -----abz
	/// ]]></code>
	/// </remarks>
	private void Check3U2SL(
		List<UniqueRectangleStep> accumulator,
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
		Cell[] urCells,
		bool arMode,
		Mask comparer,
		Digit d1,
		Digit d2,
		Cell cornerCell,
		scoped ref readonly CellMap otherCellsMap,
		int index
	)
	{
		if (grid.GetCandidates(cornerCell) != comparer)
		{
			return;
		}

		var abzCell = GetDiagonalCell(urCells, cornerCell);
		var adjacentCellsMap = otherCellsMap - abzCell;
		var abxCell = adjacentCellsMap[0];
		var abyCell = adjacentCellsMap[1];
		foreach (var (begin, end) in ((abxCell, abyCell), (abyCell, abxCell)))
		{
			var linkMap = CellsMap[begin] + abzCell;
			foreach (var (a, b) in ((d1, d2), (d2, d1)))
			{
				if (!IsConjugatePair(b, in linkMap, linkMap.CoveredLine))
				{
					continue;
				}

				var secondLinkMap = CellsMap[cornerCell] + end;
				if (!IsConjugatePair(a, in secondLinkMap, secondLinkMap.CoveredLine))
				{
					continue;
				}

				if (!CandidatesMap[a].Contains(begin))
				{
					continue;
				}

				var candidateOffsets = new List<CandidateViewNode>(7);
				foreach (var d in comparer)
				{
					candidateOffsets.Add(new(d == a ? WellKnownColorIdentifier.Auxiliary1 : WellKnownColorIdentifier.Normal, cornerCell * 9 + d));
				}
				foreach (var d in grid.GetCandidates(begin))
				{
					if ((d == d1 || d == d2) && d != a)
					{
						candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary1, begin * 9 + d));
					}
				}
				foreach (var d in grid.GetCandidates(end))
				{
					if (d == d1 || d == d2)
					{
						candidateOffsets.Add(new(d == a ? WellKnownColorIdentifier.Auxiliary1 : WellKnownColorIdentifier.Normal, end * 9 + d));
					}
				}
				foreach (var d in grid.GetCandidates(abzCell))
				{
					if (d == d1 || d == d2)
					{
						candidateOffsets.Add(new(d == b ? WellKnownColorIdentifier.Auxiliary1 : WellKnownColorIdentifier.Normal, abzCell * 9 + d));
					}
				}
				if (!AllowIncompleteUniqueRectangles && candidateOffsets.Count != 7)
				{
					continue;
				}

				var conjugatePairs = (Conjugate[])[new(cornerCell, end, a), new(begin, abzCell, b)];
				accumulator.Add(
					new UniqueRectangleWithConjugatePairStep(
						[new(Elimination, begin, a)],
						[
							[
								.. arMode ? GetHighlightCells(urCells) : [],
								.. candidateOffsets,
								new HouseViewNode(WellKnownColorIdentifier.Normal, conjugatePairs[0].Line),
								new HouseViewNode(WellKnownColorIdentifier.Auxiliary1, conjugatePairs[1].Line)
							]
						],
						context.PredefinedOptions,
						Technique.UniqueRectangle3U2,
						d1,
						d2,
						[.. urCells],
						arMode,
						conjugatePairs,
						index
					)
				);
			}
		}
	}

	/// <summary>
	/// Check UR + 3E/2SL.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="cornerCell">The corner cell.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The pattern:
	/// <code><![CDATA[
	///   ↓ cornerCell
	/// (ab )-----abx
	///        a
	///
	///        a
	///  aby -----abz
	/// ]]></code>
	/// </remarks>
	private void Check3E2SL(
		List<UniqueRectangleStep> accumulator,
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
		Cell[] urCells,
		bool arMode,
		Mask comparer,
		Digit d1,
		Digit d2,
		Cell cornerCell,
		scoped ref readonly CellMap otherCellsMap,
		int index
	)
	{
		if (grid.GetCandidates(cornerCell) != comparer)
		{
			return;
		}

		var abzCell = GetDiagonalCell(urCells, cornerCell);
		var adjacentCellsMap = otherCellsMap - abzCell;
		var abxCell = adjacentCellsMap[0];
		var abyCell = adjacentCellsMap[1];
		foreach (var (begin, end) in ((abxCell, abyCell), (abyCell, abxCell)))
		{
			var linkMap = CellsMap[begin] + abzCell;
			foreach (var (a, b) in ((d1, d2), (d2, d1)))
			{
				if (!IsConjugatePair(a, in linkMap, linkMap.CoveredLine))
				{
					continue;
				}

				var secondLinkMap = CellsMap[cornerCell] + end;
				if (!IsConjugatePair(a, in secondLinkMap, secondLinkMap.CoveredLine))
				{
					continue;
				}

				if (!CandidatesMap[b].Contains(abzCell))
				{
					continue;
				}

				var candidateOffsets = new List<CandidateViewNode>(7);
				foreach (var d in comparer)
				{
					candidateOffsets.Add(new(d == a ? WellKnownColorIdentifier.Auxiliary1 : WellKnownColorIdentifier.Normal, cornerCell * 9 + d));
				}
				foreach (var d in grid.GetCandidates(begin))
				{
					if (d == d1 || d == d2)
					{
						candidateOffsets.Add(new(d == a ? WellKnownColorIdentifier.Auxiliary1 : WellKnownColorIdentifier.Normal, begin * 9 + d));
					}
				}
				foreach (var d in grid.GetCandidates(end))
				{
					if (d == d1 || d == d2)
					{
						candidateOffsets.Add(new(d == a ? WellKnownColorIdentifier.Auxiliary1 : WellKnownColorIdentifier.Normal, end * 9 + d));
					}
				}
				foreach (var d in grid.GetCandidates(abzCell))
				{
					if ((d == d1 || d == d2) && d != b)
					{
						candidateOffsets.Add(new(d == a ? WellKnownColorIdentifier.Auxiliary1 : WellKnownColorIdentifier.Normal, abzCell * 9 + d));
					}
				}
				if (!AllowIncompleteUniqueRectangles && candidateOffsets.Count != 7)
				{
					continue;
				}

				var conjugatePairs = (Conjugate[])[new(cornerCell, end, a), new(begin, abzCell, a)];
				accumulator.Add(
					new UniqueRectangleWithConjugatePairStep(
						[new(Elimination, abzCell, b)],
						[
							[
								.. arMode ? GetHighlightCells(urCells) : [],
								.. candidateOffsets,
								new HouseViewNode(WellKnownColorIdentifier.Normal, conjugatePairs[0].Line),
								new HouseViewNode(WellKnownColorIdentifier.Auxiliary1, conjugatePairs[1].Line)
							]
						],
						context.PredefinedOptions,
						Technique.UniqueRectangle3E2,
						d1,
						d2,
						[.. urCells],
						arMode,
						conjugatePairs,
						index
					)
				);
			}
		}
	}

	/// <summary>
	/// Check UR + 4X/3SL.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="corner1">The corner cell 1.</param>
	/// <param name="corner2">The corner cell 2.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The pattern:
	/// <code><![CDATA[
	///   ↓ corner1, corner2
	/// (abx)-----(aby)
	///        a    |
	///             | b
	///        a    |
	///  abz ----- abw
	/// ]]></code>
	/// </remarks>
	private void Check4X3SL(
		List<UniqueRectangleStep> accumulator,
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
		Cell[] urCells,
		bool arMode,
		Mask comparer,
		Digit d1,
		Digit d2,
		Cell corner1,
		Cell corner2,
		scoped ref readonly CellMap otherCellsMap,
		int index
	)
	{
		var link1Map = CellsMap[corner1] + corner2;
		foreach (var (a, b) in ((d1, d2), (d2, d1)))
		{
			if (!IsConjugatePair(a, in link1Map, link1Map.CoveredLine))
			{
				continue;
			}

			var abwCell = GetDiagonalCell(urCells, corner1);
			var abzCell = (otherCellsMap - abwCell)[0];
			foreach (var (head, begin, end, extra) in ((corner2, corner1, abzCell, abwCell), (corner1, corner2, abwCell, abzCell)))
			{
				var link2Map = CellsMap[begin] + end;
				if (!IsConjugatePair(b, in link2Map, link2Map.CoveredLine))
				{
					continue;
				}

				var link3Map = CellsMap[end] + extra;
				if (!IsConjugatePair(a, in link3Map, link3Map.CoveredLine))
				{
					continue;
				}

				var conclusions = new List<Conclusion>(2);
				if (CandidatesMap[b].Contains(head))
				{
					conclusions.Add(new(Elimination, head, b));
				}
				if (CandidatesMap[b].Contains(extra))
				{
					conclusions.Add(new(Elimination, extra, b));
				}
				if (conclusions.Count == 0)
				{
					continue;
				}

				var candidateOffsets = new List<CandidateViewNode>(6);
				foreach (var d in grid.GetCandidates(head))
				{
					if ((d == d1 || d == d2) && d != b)
					{
						candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary1, head * 9 + d));
					}
				}
				foreach (var d in grid.GetCandidates(extra))
				{
					if ((d == d1 || d == d2) && d != b)
					{
						candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary1, extra * 9 + d));
					}
				}
				foreach (var d in grid.GetCandidates(begin))
				{
					if (d == d1 || d == d2)
					{
						candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary1, begin * 9 + d));
					}
				}
				foreach (var d in grid.GetCandidates(end))
				{
					if (d == d1 || d == d2)
					{
						candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary1, end * 9 + d));
					}
				}
				if (!AllowIncompleteUniqueRectangles && (candidateOffsets.Count, conclusions.Count) != (6, 2))
				{
					continue;
				}

				var conjugatePairs = (Conjugate[])[new(head, begin, a), new(begin, end, b), new(end, extra, a)];
				accumulator.Add(
					new UniqueRectangleWithConjugatePairStep(
						[.. conclusions],
						[
							[
								.. arMode ? GetHighlightCells(urCells) : [],
								.. candidateOffsets,
								new HouseViewNode(WellKnownColorIdentifier.Normal, conjugatePairs[0].Line),
								new HouseViewNode(WellKnownColorIdentifier.Auxiliary1, conjugatePairs[1].Line),
								new HouseViewNode(WellKnownColorIdentifier.Normal, conjugatePairs[2].Line)
							]
						],
						context.PredefinedOptions,
						Technique.UniqueRectangle4X3,
						d1,
						d2,
						[.. urCells],
						arMode,
						conjugatePairs,
						index
					)
				);
			}
		}
	}

	/// <summary>
	/// Check UR + 4C/3SL.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="corner1">The corner cell 1.</param>
	/// <param name="corner2">The corner cell 2.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// <para>The structures:</para>
	/// <para>
	/// Subtype 1:
	/// <code><![CDATA[
	///   ↓ corner1, corner2
	/// (abx)-----(aby)
	///        a    |
	///             | a
	///        b    |
	///  abz ----- abw
	/// ]]></code>
	/// </para>
	/// <para>
	/// Subtype 2:
	/// <code><![CDATA[
	///   ↓ corner1, corner2
	/// (abx)-----(aby)
	///   |    a    |
	///   | b       | a
	///   |         |
	///  abz       abw
	/// ]]></code>
	/// </para>
	/// </remarks>
	private void Check4C3SL(
		List<UniqueRectangleStep> accumulator,
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
		Cell[] urCells,
		bool arMode,
		Mask comparer,
		Digit d1,
		Digit d2,
		Cell corner1,
		Cell corner2,
		scoped ref readonly CellMap otherCellsMap,
		int index
	)
	{
		var link1Map = CellsMap[corner1] + corner2;
		scoped var innerMaps = (stackalloc CellMap[2]);
		foreach (var (a, b) in ((d1, d2), (d2, d1)))
		{
			if (!IsConjugatePair(a, in link1Map, link1Map.CoveredLine))
			{
				continue;
			}

			var end = GetDiagonalCell(urCells, corner1);
			var extra = (otherCellsMap - end)[0];
			foreach (var (abx, aby, abw, abz) in ((corner2, corner1, extra, end), (corner1, corner2, end, extra)))
			{
				var link2Map = CellsMap[aby] + abw;
				if (!IsConjugatePair(a, in link2Map, link2Map.CoveredLine))
				{
					continue;
				}

				var link3Map1 = CellsMap[abw] + abz;
				var link3Map2 = CellsMap[abx] + abz;
				innerMaps[0] = link3Map1;
				innerMaps[1] = link3Map2;
				for (var i = 0; i < 2; i++)
				{
					var linkMap = innerMaps[i];
					if (!IsConjugatePair(b, in link3Map1, link3Map1.CoveredLine))
					{
						continue;
					}

					if (!CandidatesMap[b].Contains(aby))
					{
						continue;
					}

					var candidateOffsets = new List<CandidateViewNode>(7);
					foreach (var d in grid.GetCandidates(abx))
					{
						if (d == d1 || d == d2)
						{
							candidateOffsets.Add(
								new(
									i == 0 ? d == a ? WellKnownColorIdentifier.Auxiliary1 : WellKnownColorIdentifier.Normal : WellKnownColorIdentifier.Auxiliary1,
									abx * 9 + d
								)
							);
						}
					}
					foreach (var d in grid.GetCandidates(abz))
					{
						if (d == d1 || d == d2)
						{
							candidateOffsets.Add(new(d == b ? WellKnownColorIdentifier.Auxiliary1 : WellKnownColorIdentifier.Normal, abz * 9 + d));
						}
					}
					foreach (var d in grid.GetCandidates(aby))
					{
						if ((d == d1 || d == d2) && d != b)
						{
							candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary1, aby * 9 + d));
						}
					}
					foreach (var d in grid.GetCandidates(abw))
					{
						if (d == d1 || d == d2)
						{
							candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary1, abw * 9 + d));
						}
					}
					if (!AllowIncompleteUniqueRectangles && candidateOffsets.Count != 7)
					{
						continue;
					}

					var conjugatePairs = (Conjugate[])[new(abx, aby, a), new(aby, abw, a), new(linkMap[0], linkMap[1], b)];
					accumulator.Add(
						new UniqueRectangleWithConjugatePairStep(
							[new(Elimination, aby, b)],
							[
								[
									.. arMode ? GetHighlightCells(urCells) : [],
									.. candidateOffsets,
									new HouseViewNode(WellKnownColorIdentifier.Normal, conjugatePairs[0].Line),
									new HouseViewNode(WellKnownColorIdentifier.Normal, conjugatePairs[1].Line),
									new HouseViewNode(WellKnownColorIdentifier.Auxiliary1, conjugatePairs[2].Line)
								]
							],
							context.PredefinedOptions,
							Technique.UniqueRectangle4C3,
							d1,
							d2,
							[.. urCells],
							arMode,
							conjugatePairs,
							index
						)
					);
				}
			}
		}
	}

	/// <summary>
	/// Check UR-XY-Wing, UR-XYZ-Wing, UR-WXYZ-Wing and AR-XY-Wing, AR-XYZ-Wing and AR-WXYZ-Wing.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="corner1">The corner cell 1.</param>
	/// <param name="corner2">The corner cell 2.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <param name="areCornerCellsAligned">Indicates whether the corner cells cannot see each other.</param>
	/// <remarks>
	/// <para>The structures:</para>
	/// <para>
	/// Subtype 1:
	/// <code><![CDATA[
	///   ↓ corner1
	/// (ab )  abxy  yz  xz
	/// (ab )  abxy  *
	///   ↑ corner2
	/// ]]></code>
	/// Note that the pair of cells <c>abxy</c> should be in the same house.
	/// </para>
	/// <para>
	/// Subtype 2:
	/// <code><![CDATA[
	///   ↓ corner1
	/// (ab )  abx   xz
	///  aby  (ab )  *   yz
	///         ↑ corner2
	/// ]]></code>
	/// </para>
	/// </remarks>
	private void CheckRegularWing(
		List<UniqueRectangleStep> accumulator,
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
		Cell[] urCells,
		bool arMode,
		Mask comparer,
		Digit d1,
		Digit d2,
		Cell corner1,
		Cell corner2,
		scoped ref readonly CellMap otherCellsMap,
		int index,
		bool areCornerCellsAligned
	)
	{
		// Firstly, we should check whether the 2 corner cells should contain both a and b, and only contain a and b.
		// This expression only uses candidates to check digits appearing, so it doesn't determine whether the pattern is a UR or not.
		// ARs can also be passed of course.
		if ((grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) != comparer)
		{
			return;
		}

		var otherCell1Mask = grid.GetCandidates(otherCellsMap[0]);
		var otherCell2Mask = grid.GetCandidates(otherCellsMap[1]);
		if ((otherCell1Mask & otherCell2Mask & comparer) == 0 || ((otherCell1Mask | otherCell2Mask) & comparer) != comparer)
		{
			return;
		}

		// Now we check for other 2 cells, collecting digits not being UR/AR digits.
		var otherDigitsMask = (Mask)((otherCell1Mask | otherCell2Mask) & ~comparer);

		// Merge incomplete and complete wing logic into one loop.
		// Here we don't know what digit will be selected as a pivot, so we should iterate all digits.
		// The last case -1 is for complete wing.
		var cells = (CellMap)urCells;
		var pivotDigits = (Digit[])[.. otherDigitsMask.GetAllSets(), -1];
		foreach (var pivotDigit in pivotDigits)
		{
			Cell[][] cellsGroups;
			if (pivotDigit == -1)
			{
				// No pivot digit should be checked. Due to no way to check intersection, we should delay the checking.
				(cellsGroups, var tempIndex) = (new Cell[PopCount((uint)otherDigitsMask)][], 0);
				foreach (var lastDigit in otherDigitsMask)
				{
					cellsGroups[tempIndex++] = [.. cells % CandidatesMap[lastDigit] & BivalueCells];
				}
			}
			else
			{
				// Pivot digit is specified. We should check for cells that contain that digit.
				var lastDigitsMask = (Mask)(otherDigitsMask & ~(1 << pivotDigit));
				if (PopCount((uint)lastDigitsMask) is 0 or 1)
				{
					continue;
				}

				(cellsGroups, var tempIndex, var atLeastOneGroupIsEmpty) = (new Cell[PopCount((uint)lastDigitsMask)][], 0, false);
				foreach (var lastDigit in lastDigitsMask)
				{
					scoped ref var currentCellGroup = ref cellsGroups[tempIndex++];
					currentCellGroup = [.. cells % CandidatesMap[lastDigit] & CandidatesMap[pivotDigit] & BivalueCells];
					if (currentCellGroup.Length == 0)
					{
						atLeastOneGroupIsEmpty = true;
						break;
					}
				}
				if (atLeastOneGroupIsEmpty)
				{
					// If a cells group does not contain such cells, the current pivot digit will be considered invalid.
					// Now just skip for this case.
					continue;
				}
			}

			// Extract one element for each cells group.
			var finalPivotDigit = pivotDigit;
			foreach (CellMap combination in cellsGroups.GetExtractedCombinations())
			{
				if (cellsGroups.Length != combination.Count)
				{
					// The selected items cannot duplicate with others.
					continue;
				}

				// Here we should check for pivot digit for case 'pivotDigit == -1'.
				if (pivotDigit == -1)
				{
					var mergedMask = grid[in combination, false, GridMaskMergingMethod.And];
					if (!IsPow2(mergedMask))
					{
						// No pivot digit can be found, meaning no eliminations can be found.
						continue;
					}

					finalPivotDigit = TrailingZeroCount(mergedMask);
				}

				var maskToCompare = pivotDigit == -1 ? grid[in combination] & ~(1 << finalPivotDigit) : grid[in combination];
				if (((otherCell1Mask | otherCell2Mask) & ~comparer) != maskToCompare)
				{
					// Digits are not matched.
					continue;
				}

				var elimMapBase = pivotDigit == -1 ? combination : combination | cells & CandidatesMap[finalPivotDigit];
				var elimMap = elimMapBase.PeerIntersection & CandidatesMap[finalPivotDigit];
				if (!elimMap)
				{
					// No eliminations.
					continue;
				}

				var candidateOffsets = new List<CandidateViewNode>();
				foreach (var cell in cells & EmptyCells)
				{
					foreach (var digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(
							new(
								(comparer >> digit & 1) != 0
									? WellKnownColorIdentifier.Normal
									: digit == finalPivotDigit ? WellKnownColorIdentifier.Auxiliary2 : WellKnownColorIdentifier.Auxiliary1,
								cell * 9 + digit
							)
						);
					}
				}
				foreach (var cell in combination)
				{
					foreach (var digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(
							new(
								digit != finalPivotDigit ? WellKnownColorIdentifier.Auxiliary1 : WellKnownColorIdentifier.Auxiliary2,
								cell * 9 + digit
							)
						);
					}
				}

				var cellOffsets = new List<CellViewNode>();
				foreach (var cell in urCells)
				{
					if (!EmptyCells.Contains(cell))
					{
						cellOffsets.Add(new(WellKnownColorIdentifier.Normal, cell));
					}
				}

				var isIncomplete = IsIncomplete(AllowIncompleteUniqueRectangles, candidateOffsets);
				if (!AllowIncompleteUniqueRectangles && isIncomplete)
				{
					continue;
				}

				accumulator.Add(
					new UniqueRectangleWithWingStep(
						[.. from cell in elimMap select new Conclusion(Elimination, cell, finalPivotDigit)],
						[[.. candidateOffsets, .. cellOffsets]],
						context.PredefinedOptions,
						(arMode, pivotDigit, combination.Count) switch
						{
							(false, -1, 2) => Technique.UniqueRectangleXyWing,
							(false, -1, 3) => Technique.UniqueRectangleXyzWing,
							(false, -1, 4) => Technique.UniqueRectangleWxyzWing,
							(false, _, 2) => Technique.UniqueRectangleXyzWing,
							(false, _, 3) => Technique.UniqueRectangleWxyzWing,
							(_, -1, 2) => Technique.AvoidableRectangleXyWing,
							(_, -1, 3) => Technique.AvoidableRectangleXyzWing,
							(_, -1, 4) => Technique.AvoidableRectangleWxyzWing,
							(_, _, 2) => Technique.AvoidableRectangleXyzWing,
							(_, _, 3) => Technique.AvoidableRectangleWxyzWing
						},
						d1,
						d2,
						in cells,
						arMode,
						in combination,
						in otherCellsMap,
						otherDigitsMask,
						index
					)
				);
			}
		}
	}

	/// <summary>
	/// Check UR-W-Wing and AR-W-Wing.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="corner1">The corner cell 1.</param>
	/// <param name="corner2">The corner cell 2.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// <para>The structures:</para>
	/// <para>
	/// Subtype 1:
	/// <code><![CDATA[
	///         ↓ corner1
	/// xz   abx  (ab )
	///     (ab )  aby   yz
	///       ↑ corner2
	/// ]]></code>
	/// </para>
	/// <para>
	/// Subtype 2:
	/// <code><![CDATA[
	///       ↓ corner1
	/// xz  (ab )  abx
	///      abx  (ab )  xz
	///             ↑ corner2
	/// ]]></code>
	/// </para>
	/// <para>Please note that corner cells may be aligned as a same row or column.</para>
	/// <para>
	/// <i>Also, this method is useless because it may be replaced with another techniques such as UR-XY-Wing and UR External Type 2.</i>
	/// </para>
	/// </remarks>
	private void CheckWWing(
		List<UniqueRectangleStep> accumulator,
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
		Cell[] urCells,
		bool arMode,
		Mask comparer,
		Digit d1,
		Digit d2,
		Cell corner1,
		Cell corner2,
		scoped ref readonly CellMap otherCellsMap,
		int index
	)
	{
		// Firstly, we should check whether the 2 corner cells should contain both a and b, and only contain a and b.
		// This expression only uses candidates to check digits appearing, so it doesn't determine whether the pattern is a UR or not.
		// ARs can also be passed of course.
		if ((grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) != comparer)
		{
			return;
		}

		var otherCell1Mask = grid.GetCandidates(otherCellsMap[0]);
		var otherCell2Mask = grid.GetCandidates(otherCellsMap[1]);
		if ((otherCell1Mask & otherCell2Mask & comparer) == 0 || ((otherCell1Mask | otherCell2Mask) & comparer) != comparer)
		{
			return;
		}

		var otherDigits1 = (Mask)(otherCell1Mask & (Mask)~comparer);
		if (!IsPow2(otherDigits1))
		{
			return;
		}
		var otherDigits2 = (Mask)(otherCell2Mask & (Mask)~comparer);
		if (!IsPow2(otherDigits2))
		{
			return;
		}

		var otherDigit1 = TrailingZeroCount(otherDigits1);
		var otherDigit2 = TrailingZeroCount(otherDigits2);

		// Now we check for other 2 cells, collecting digits not being UR/AR digits.
		var cells = (CellMap)urCells;
		foreach (var endCell1 in PeersMap[otherCellsMap[0]] & BivalueCells & CandidatesMap[otherDigit1])
		{
			foreach (var endCell2 in (PeersMap[otherCellsMap[1]] & BivalueCells & CandidatesMap[otherDigit2]) - endCell1)
			{
				// Check whether two cells are same, or in a same house. If so, the pattern will be degenerated to a normal type 3.
				if ((CellsMap[endCell1] + endCell2).InOneHouse(out _))
				{
					continue;
				}

				var mergedMask = (Mask)(grid.GetCandidates(endCell1) & grid.GetCandidates(endCell2));
				if (!IsPow2(mergedMask))
				{
					continue;
				}

				var wDigit = TrailingZeroCount(mergedMask);
				if (otherDigit1 == wDigit || otherDigit2 == wDigit)
				{
					continue;
				}

				var elimMap = (CellsMap[endCell1] + endCell2).PeerIntersection & CandidatesMap[wDigit];
				if (!elimMap)
				{
					// No eliminations.
					continue;
				}

				// A W-Wing found.
				var candidateOffsets = new List<CandidateViewNode>();
				foreach (var cell in urCells)
				{
					foreach (var digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(
							new(
								digit == d1 || digit == d2 ? WellKnownColorIdentifier.Normal : WellKnownColorIdentifier.Auxiliary1,
								cell * 9 + digit
							)
						);
					}
				}
				foreach (var digit in grid.GetCandidates(endCell1))
				{
					candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary1, endCell1 * 9 + digit));
				}
				foreach (var digit in grid.GetCandidates(endCell2))
				{
					candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary1, endCell2 * 9 + digit));
				}

				if (!AllowIncompleteUniqueRectangles && IsIncomplete(AllowIncompleteUniqueRectangles, candidateOffsets))
				{
					continue;
				}

				var isAvoidable = arMode && (EmptyCells & cells).Count != 4;
				accumulator.Add(
					new UniqueRectangleWithWWingStep(
						[.. from cell in elimMap select new Conclusion(Elimination, cell, wDigit)],
						[[.. candidateOffsets]],
						context.PredefinedOptions,
						isAvoidable ? Technique.AvoidableRectangleWWing : Technique.UniqueRectangleWWing,
						d1,
						d2,
						in cells,
						isAvoidable,
						wDigit,
						in otherCellsMap,
						[endCell1, endCell2],
						index
					)
				);
			}
		}
	}

	/// <summary>
	/// Check UR + SdC.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="corner1">The corner cell 1.</param>
	/// <param name="corner2">The corner cell 2.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The pattern:
	/// <code><![CDATA[
	///           |   xyz
	///  ab+ ab+  | abxyz abxyz
	///           |   xyz
	/// ----------+------------
	/// (ab)(ab)  |
	///  ↑ corner1, corner2
	/// ]]></code>
	/// </remarks>
	private void CheckSueDeCoq(
		List<UniqueRectangleStep> accumulator,
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
		Cell[] urCells,
		bool arMode,
		Mask comparer,
		Digit d1,
		Digit d2,
		Cell corner1,
		Cell corner2,
		scoped ref readonly CellMap otherCellsMap,
		int index
	)
	{
		var notSatisfiedType3 = false;
		var mergedMaskInOtherCells = (Mask)0;
		foreach (var cell in otherCellsMap)
		{
			var currentMask = grid.GetCandidates(cell);
			mergedMaskInOtherCells |= currentMask;
			if ((currentMask & comparer) == 0 || currentMask == comparer || arMode && grid.GetState(cell) != CellState.Empty)
			{
				notSatisfiedType3 = true;
				break;
			}
		}

		if ((grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) != comparer || notSatisfiedType3
			|| (mergedMaskInOtherCells & comparer) != comparer)
		{
			return;
		}

		// Check whether the corners spanned two blocks. If so, UR + SdC can't be found.
		var blockMaskInOtherCells = otherCellsMap.BlockMask;
		if (!IsPow2(blockMaskInOtherCells))
		{
			return;
		}

		var otherDigitsMask = (Mask)(mergedMaskInOtherCells & ~comparer);
		var line = (byte)otherCellsMap.CoveredLine;
		var block = (byte)TrailingZeroCount(otherCellsMap.CoveredHouses & ~(1 << line));
		var (a, _, _, d) = IntersectionMaps[new(line, block)];
		var list = new List<CellMap>(4);
		foreach (var cannibalMode in (false, true))
		{
			foreach (var otherBlock in d)
			{
				var emptyCellsInInterMap = HousesMap[otherBlock] & HousesMap[line] & EmptyCells;
				if (emptyCellsInInterMap.Count < 2)
				{
					// The intersection needs at least two empty cells.
					continue;
				}

				var b = HousesMap[otherBlock] - HousesMap[line];
				var c = a & b;

				list.Clear();
				switch (emptyCellsInInterMap)
				{
					case { Count: 2 }:
					{
						list.AddRef(in emptyCellsInInterMap);
						break;
					}
					case [var i, var j, var k]:
					{
						list.AddRef([i, j]);
						list.AddRef([j, k]);
						list.AddRef([i, k]);
						list.AddRef(in emptyCellsInInterMap);
						break;
					}
				}

				// Iterate on each intersection combination.
				foreach (var currentInterMap in list)
				{
					var selectedInterMask = grid[in currentInterMap];
					if (PopCount((uint)selectedInterMask) <= currentInterMap.Count + 1)
					{
						// The intersection combination is an ALS or a normal subset,
						// which is invalid in SdCs.
						continue;
					}

					var blockMap = (b | c - currentInterMap) & EmptyCells;
					var lineMap = (a & EmptyCells) - otherCellsMap;

					// Iterate on the number of the cells that should be selected in block.
					for (var i = 1; i <= blockMap.Count - 1; i++)
					{
						// Iterate on each combination in block.
						foreach (ref readonly var selectedCellsInBlock in blockMap.GetSubsets(i))
						{
							var flag = false;
							foreach (var digit in otherDigitsMask)
							{
								foreach (var cell in selectedCellsInBlock)
								{
									if (CandidatesMap[digit].Contains(cell))
									{
										flag = true;
										break;
									}
								}
							}
							if (flag)
							{
								continue;
							}

							var currentBlockMap = selectedCellsInBlock;
							var elimMapBlock = CellMap.Empty;
							var elimMapLine = CellMap.Empty;

							// Get the links of the block.
							var blockMask = grid[in selectedCellsInBlock];

							// Get the elimination map in the block.
							foreach (var digit in blockMask)
							{
								elimMapBlock |= CandidatesMap[digit];
							}
							elimMapBlock &= blockMap - currentBlockMap;

							foreach (var digit in otherDigitsMask)
							{
								elimMapLine |= CandidatesMap[digit];
							}
							elimMapLine &= lineMap - currentInterMap;

							checkGeneralizedSdc(
								accumulator, in grid, ref context, arMode, cannibalMode, d1, d2, urCells,
								line, otherBlock, otherDigitsMask, blockMask, selectedInterMask,
								otherDigitsMask, in elimMapLine, in elimMapBlock, in otherCellsMap, in currentBlockMap,
								in currentInterMap, i, 0, index
							);
						}
					}
				}
			}
		}


		static void checkGeneralizedSdc(
			List<UniqueRectangleStep> accumulator,
			scoped ref readonly Grid grid,
			scoped ref AnalysisContext context,
			bool arMode,
			bool cannibalMode,
			Digit digit1,
			Digit digit2,
			Cell[] urCells,
			House line,
			House block,
			Mask lineMask,
			Mask blockMask,
			Mask selectedInterMask,
			Mask otherDigitsMask,
			scoped ref readonly CellMap elimMapLine,
			scoped ref readonly CellMap elimMapBlock,
			scoped ref readonly CellMap currentLineMap,
			scoped ref readonly CellMap currentBlockMap,
			scoped ref readonly CellMap currentInterMap,
			int i,
			int j,
			int index
		)
		{
			var maskOnlyInInter = (Mask)(selectedInterMask & ~(blockMask | lineMask));
			var maskIsolated = (Mask)(cannibalMode ? (lineMask & blockMask & selectedInterMask) : maskOnlyInInter);
			if (!cannibalMode && ((blockMask & lineMask) != 0 || maskIsolated != 0 && !IsPow2(maskIsolated))
				|| cannibalMode && !IsPow2(maskIsolated))
			{
				return;
			}

			var elimMapIsolated = CellMap.Empty;
			var digitIsolated = TrailingZeroCount(maskIsolated);
			if (digitIsolated != InvalidTrailingZeroCountMethodFallback)
			{
				elimMapIsolated = (cannibalMode ? currentBlockMap | currentLineMap : currentInterMap)
					% CandidatesMap[digitIsolated]
					& EmptyCells;
			}

			if (currentInterMap.Count + i + j + 1 == PopCount((uint)blockMask) + PopCount((uint)lineMask) + PopCount((uint)maskOnlyInInter)
				&& !!(elimMapBlock | elimMapLine | elimMapIsolated))
			{
				// Check eliminations.
				var conclusions = new List<Conclusion>(10);
				foreach (var cell in elimMapBlock)
				{
					foreach (var digit in grid.GetCandidates(cell))
					{
						if ((blockMask >> digit & 1) != 0)
						{
							conclusions.Add(new(Elimination, cell, digit));
						}
					}
				}
				foreach (var cell in elimMapLine)
				{
					foreach (var digit in grid.GetCandidates(cell))
					{
						if ((lineMask >> digit & 1) != 0)
						{
							conclusions.Add(new(Elimination, cell, digit));
						}
					}
				}
				foreach (var cell in elimMapIsolated)
				{
					conclusions.Add(new(Elimination, cell, digitIsolated));
				}
				if (conclusions.Count == 0)
				{
					return;
				}

				// Record highlight candidates and cells.
				var candidateOffsets = new List<CandidateViewNode>();
				foreach (var cell in urCells)
				{
					foreach (var digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(
							new(
								(otherDigitsMask >> digit & 1) != 0 ? WellKnownColorIdentifier.Auxiliary1 : WellKnownColorIdentifier.Normal,
								cell * 9 + digit
							)
						);
					}
				}
				foreach (var cell in currentBlockMap)
				{
					foreach (var digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(
							new(
								!cannibalMode && digit == digitIsolated ? WellKnownColorIdentifier.Auxiliary3 : WellKnownColorIdentifier.Auxiliary2,
								cell * 9 + digit
							)
						);
					}
				}
				foreach (var cell in currentInterMap)
				{
					foreach (var digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(
							new(
								digitIsolated == digit
									? WellKnownColorIdentifier.Auxiliary3
									: (otherDigitsMask >> digit & 1) != 0 ? WellKnownColorIdentifier.Auxiliary1 : WellKnownColorIdentifier.Auxiliary2,
								cell * 9 + digit
							)
						);
					}
				}

				accumulator.Add(
					new UniqueRectangleWithSueDeCoqStep(
						[.. conclusions],
						[
							[
								.. arMode ? GetHighlightCells(urCells) : [],
								.. candidateOffsets,
								new HouseViewNode(WellKnownColorIdentifier.Normal, block),
								new HouseViewNode(WellKnownColorIdentifier.Auxiliary2, line)
							]
						],
						context.PredefinedOptions,
						digit1,
						digit2,
						[.. urCells],
						arMode,
						block,
						line,
						blockMask,
						lineMask,
						selectedInterMask,
						cannibalMode,
						maskIsolated,
						in currentBlockMap,
						in currentLineMap,
						in currentInterMap,
						index
					)
				);
			}
		}
	}

	/// <summary>
	/// Check UR + baba grouping.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="comparer">The comparer.</param>
	/// <param name="d1">The digit 1.</param>
	/// <param name="d2">The digit 2.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// <para>The structures:</para>
	/// <para>
	/// Subtype 1:
	/// <code><![CDATA[
	///      ↓urCellInSameBlock
	/// ab  abc      abc  ←anotherCell
	///
	///     abcx-----abcy ←resultCell
	///           c
	///      ↑targetCell
	/// ]]></code>
	/// Where the digit <c>a</c> and <c>b</c> in the bottom-left cell <c>abcx</c> can be removed.
	/// </para>
	/// <para>
	/// Subtype 2:
	/// <code><![CDATA[
	/// abcx   | ab  abc
	///  |     |
	///  | c   |
	///  |     |
	/// abcy   |     abc
	/// ]]></code>
	/// </para>
	/// </remarks>
	private void CheckBabaGroupingUnique(
		List<UniqueRectangleStep> accumulator,
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
		Cell[] urCells,
		Mask comparer,
		Digit d1,
		Digit d2,
		int index
	)
	{
		checkType1(in grid, ref context);
#if IMPLEMENTED
		checkType2(in grid, ref context);
#endif


		void checkType1(scoped ref readonly Grid grid, scoped ref AnalysisContext context)
		{
			var cells = (CellMap)urCells;

			// Check all cells are empty.
			var containsValueCells = false;
			foreach (var cell in cells)
			{
				if (grid.GetState(cell) != CellState.Empty)
				{
					containsValueCells = true;
					break;
				}
			}
			if (containsValueCells)
			{
				return;
			}

			// Iterate on each cell.
			foreach (var targetCell in cells)
			{
				var block = targetCell.ToHouseIndex(HouseType.Block);
				var bivalueCellsToCheck = (PeersMap[targetCell] & HousesMap[block] & BivalueCells) - cells;
				if (!bivalueCellsToCheck)
				{
					continue;
				}

				// Check all bi-value cells.
				foreach (var bivalueCellToCheck in bivalueCellsToCheck)
				{
					if ((CellsMap[bivalueCellToCheck] + targetCell).CoveredLine != InvalidTrailingZeroCountMethodFallback)
					{
						// 'targetCell' and 'bivalueCellToCheck' can't lie on a same line.
						continue;
					}

					if (grid.GetCandidates(bivalueCellToCheck) != comparer)
					{
						// 'bivalueCell' must contain both 'd1' and 'd2'.
						continue;
					}

					var urCellInSameBlock = ((HousesMap[block] & cells) - targetCell)[0];
					var coveredLine = (CellsMap[bivalueCellToCheck] + urCellInSameBlock).CoveredLine;
					if (coveredLine == InvalidTrailingZeroCountMethodFallback)
					{
						// The bi-value cell 'bivalueCellToCheck' should be lie on a same house
						// as 'urCellInSameBlock'.
						continue;
					}

					var anotherCell = (cells - urCellInSameBlock & HousesMap[coveredLine])[0];
					foreach (var extraDigit in (Mask)(grid.GetCandidates(targetCell) & ~comparer))
					{
						var abcMask = (Mask)(comparer | (Mask)(1 << extraDigit));
						if (grid.GetCandidates(anotherCell) != abcMask)
						{
							continue;
						}

						// Check the conjugate pair of the extra digit.
						var resultCell = (cells - urCellInSameBlock - anotherCell - targetCell)[0];
						var map = CellsMap[targetCell] + resultCell;
						var line = map.CoveredLine;
						if (!IsConjugatePair(extraDigit, in map, line))
						{
							continue;
						}

						if (grid.GetCandidates(urCellInSameBlock) != abcMask)
						{
							goto SubType2;
						}

						// Here, is the basic sub-type having passed the checking.
						// Gather conclusions.
						var conclusions = new List<Conclusion>();
						foreach (var digit in grid.GetCandidates(targetCell))
						{
							if (digit == d1 || digit == d2)
							{
								conclusions.Add(new(Elimination, targetCell, digit));
							}
						}
						if (conclusions.Count == 0)
						{
							goto SubType2;
						}

						// Gather views.
						var candidateOffsets = new List<CandidateViewNode> { new(WellKnownColorIdentifier.Auxiliary1, targetCell * 9 + extraDigit) };
						if (CandidatesMap[d1].Contains(resultCell))
						{
							candidateOffsets.Add(new(WellKnownColorIdentifier.Normal, resultCell * 9 + d1));
						}
						if (CandidatesMap[d2].Contains(resultCell))
						{
							candidateOffsets.Add(new(WellKnownColorIdentifier.Normal, resultCell * 9 + d2));
						}
						if (CandidatesMap[extraDigit].Contains(resultCell))
						{
							candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary1, resultCell * 9 + extraDigit));
						}

						foreach (var digit in (Mask)(grid.GetCandidates(urCellInSameBlock) & abcMask))
						{
							candidateOffsets.Add(new(WellKnownColorIdentifier.Normal, urCellInSameBlock * 9 + digit));
						}
						foreach (var digit in grid.GetCandidates(anotherCell))
						{
							candidateOffsets.Add(new(WellKnownColorIdentifier.Normal, anotherCell * 9 + digit));
						}
						var _xOr_yMask = grid.GetCandidates(bivalueCellToCheck);
						foreach (var digit in _xOr_yMask)
						{
							candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary2, bivalueCellToCheck * 9 + digit));
						}

						// Add into the list.
						var extraDigitId = (byte)(char)(extraDigit + '1');
						var extraDigitMask = (Mask)(1 << extraDigit);
						accumulator.Add(
							new UniqueRectangleWithBabaGroupingStep(
								[.. conclusions],
								[
									[
										new CellViewNode(WellKnownColorIdentifier.Normal, targetCell),
										.. candidateOffsets,
										new HouseViewNode(WellKnownColorIdentifier.Normal, block),
										new HouseViewNode(WellKnownColorIdentifier.Auxiliary1, line)
									],
									[
										new CandidateViewNode(WellKnownColorIdentifier.Auxiliary1, resultCell * 9 + extraDigit),
										new CandidateViewNode(WellKnownColorIdentifier.Auxiliary1, targetCell * 9 + extraDigit),
										new BabaGroupViewNode(WellKnownColorIdentifier.Normal, bivalueCellToCheck, (byte)'y', _xOr_yMask),
										new BabaGroupViewNode(WellKnownColorIdentifier.Normal, targetCell, (byte)'x', _xOr_yMask),
										new BabaGroupViewNode(WellKnownColorIdentifier.Normal, urCellInSameBlock, extraDigitId, extraDigitMask),
										new BabaGroupViewNode(WellKnownColorIdentifier.Normal, anotherCell, (byte)'x', _xOr_yMask),
										new BabaGroupViewNode(WellKnownColorIdentifier.Normal, resultCell, extraDigitId, extraDigitMask)
									]
								],
								context.PredefinedOptions,
								d1,
								d2,
								[.. urCells],
								targetCell,
								extraDigit,
								index
							)
						);

					SubType2:
						// Sub-type 2.
						// The extra digit should form a conjugate pair in that line.
						var anotherMap = CellsMap[urCellInSameBlock] + anotherCell;
						var anotherLine = anotherMap.CoveredLine;
						if (!IsConjugatePair(extraDigit, in anotherMap, anotherLine))
						{
							continue;
						}

						// Gather conclusions.
						var conclusionsAnotherSubType = new List<Conclusion>();
						foreach (var digit in grid.GetCandidates(targetCell))
						{
							if (digit == d1 || digit == d2)
							{
								conclusionsAnotherSubType.Add(new(Elimination, targetCell, digit));
							}
						}
						if (conclusionsAnotherSubType.Count == 0)
						{
							continue;
						}

						// Gather views.
						var candidateOffsetsAnotherSubtype = new List<CandidateViewNode>
						{
							new(WellKnownColorIdentifier.Auxiliary1, targetCell * 9 + extraDigit)
						};
						if (CandidatesMap[d1].Contains(resultCell))
						{
							candidateOffsetsAnotherSubtype.Add(new(WellKnownColorIdentifier.Normal, resultCell * 9 + d1));
						}
						if (CandidatesMap[d2].Contains(resultCell))
						{
							candidateOffsetsAnotherSubtype.Add(new(WellKnownColorIdentifier.Normal, resultCell * 9 + d2));
						}
						if (CandidatesMap[extraDigit].Contains(resultCell))
						{
							candidateOffsetsAnotherSubtype.Add(new(WellKnownColorIdentifier.Auxiliary1, resultCell * 9 + extraDigit));
						}

						var candidateOffsetsAnotherSubtypeLighter = new List<CandidateViewNode>
						{
							new(WellKnownColorIdentifier.Auxiliary1, resultCell * 9 + extraDigit),
							new(WellKnownColorIdentifier.Auxiliary1, targetCell * 9 + extraDigit)
						};
						foreach (var digit in (Mask)(grid.GetCandidates(urCellInSameBlock) & abcMask))
						{
							if (digit == extraDigit)
							{
								candidateOffsetsAnotherSubtype.Add(new(WellKnownColorIdentifier.Auxiliary1, urCellInSameBlock * 9 + digit));
								candidateOffsetsAnotherSubtypeLighter.Add(new(WellKnownColorIdentifier.Auxiliary1, urCellInSameBlock * 9 + digit));
							}
							else
							{
								candidateOffsetsAnotherSubtype.Add(new(WellKnownColorIdentifier.Normal, urCellInSameBlock * 9 + digit));
							}
						}
						foreach (var digit in grid.GetCandidates(anotherCell))
						{
							if (digit == extraDigit)
							{
								candidateOffsetsAnotherSubtype.Add(new(WellKnownColorIdentifier.Auxiliary1, anotherCell * 9 + digit));
								candidateOffsetsAnotherSubtypeLighter.Add(new(WellKnownColorIdentifier.Auxiliary1, anotherCell * 9 + digit));
							}
							else
							{
								candidateOffsetsAnotherSubtype.Add(new(WellKnownColorIdentifier.Normal, anotherCell * 9 + digit));
							}
						}
						var _xOr_yMask2 = grid.GetCandidates(bivalueCellToCheck);
						foreach (var digit in _xOr_yMask2)
						{
							candidateOffsetsAnotherSubtype.Add(new(WellKnownColorIdentifier.Auxiliary2, bivalueCellToCheck * 9 + digit));
						}

						// Add into the list.
						var extraDigitId2 = (Utf8Char)(char)(extraDigit + '1');
						var extraDigitMask2 = (Mask)(1 << extraDigit);
						accumulator.Add(
							new UniqueRectangleWithBabaGroupingStep(
								[.. conclusionsAnotherSubType],
								[
									[
										new CellViewNode(WellKnownColorIdentifier.Normal, targetCell),
										.. candidateOffsetsAnotherSubtype,
										new HouseViewNode(WellKnownColorIdentifier.Normal, block),
										new HouseViewNode(WellKnownColorIdentifier.Auxiliary1, line),
										new HouseViewNode(WellKnownColorIdentifier.Auxiliary1, anotherLine)
									],
									[
										.. candidateOffsetsAnotherSubtypeLighter,
										new BabaGroupViewNode(WellKnownColorIdentifier.Normal, bivalueCellToCheck, (Utf8Char)'y', _xOr_yMask2),
										new BabaGroupViewNode(WellKnownColorIdentifier.Normal, targetCell, (Utf8Char)'x', _xOr_yMask2),
										new BabaGroupViewNode(WellKnownColorIdentifier.Normal, urCellInSameBlock, extraDigitId2, extraDigitMask2),
										new BabaGroupViewNode(WellKnownColorIdentifier.Normal, anotherCell, (Utf8Char)'x', _xOr_yMask2),
										new BabaGroupViewNode(WellKnownColorIdentifier.Normal, resultCell, extraDigitId2, extraDigitMask2)
									]
								],
								context.PredefinedOptions,
								d1,
								d2,
								[.. urCells],
								targetCell,
								extraDigit,
								index
							)
						);
					}
				}
			}
		}

#if false
		void checkType2(scoped ref readonly Grid grid)
		{
			// TODO: Check type 2.
		}
#endif
	}

	/// <summary>
	/// <para>Check UR/AR + Guardian (i.e. UR External Type 2) and UR External Type 1.</para>
	/// <para>
	/// A UR external type 1 is a special case for type 2, which means only one guardian cell will be used.
	/// </para>
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="index">The index.</param>
	/// <param name="arMode"></param>
	private void CheckExternalType1Or2(
		List<UniqueRectangleStep> accumulator,
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
		Cell[] urCells,
		Digit d1,
		Digit d2,
		int index,
		bool arMode
	)
	{
		var cells = (CellMap)urCells;
		if (!CheckPreconditionsOnIncomplete(in grid, urCells, d1, d2))
		{
			return;
		}

		if (!arMode && (EmptyCells & cells) != cells)
		{
			return;
		}

		// Iterate on two houses used.
		foreach (var houseCombination in cells.Houses.GetAllSets().GetSubsets(2))
		{
			var houseCells = HousesMap[houseCombination[0]] | HousesMap[houseCombination[1]];
			if ((houseCells & cells) != cells)
			{
				// The houses must contain all 4 UR cells.
				continue;
			}

			var guardian1 = houseCells - cells & CandidatesMap[d1];
			var guardian2 = houseCells - cells & CandidatesMap[d2];
			if (!guardian1 ^ !guardian2)
			{
				var guardianDigit = -1;
				var targetElimMap = default(CellMap?);
				var targetGuardianMap = default(CellMap?);
				if (!!guardian1 && (guardian1.PeerIntersection & CandidatesMap[d1]) is var a and not [])
				{
					targetElimMap = a;
					guardianDigit = d1;
					targetGuardianMap = guardian1;
				}
				else if (!!guardian2 && (guardian2.PeerIntersection & CandidatesMap[d2]) is var b and not [])
				{
					targetElimMap = b;
					guardianDigit = d2;
					targetGuardianMap = guardian2;
				}

				if (targetElimMap is not { } elimMap || targetGuardianMap is not { } guardianMap || guardianDigit == -1)
				{
					continue;
				}

				var candidateOffsets = new List<CandidateViewNode>(16);
				var cellOffsets = new List<CellViewNode>();
				foreach (var cell in urCells)
				{
					if (CandidatesMap[d1].Contains(cell))
					{
						candidateOffsets.Add(new(WellKnownColorIdentifier.Normal, cell * 9 + d1));
					}
					if (CandidatesMap[d2].Contains(cell))
					{
						candidateOffsets.Add(new(WellKnownColorIdentifier.Normal, cell * 9 + d2));
					}

					if (grid.GetState(cell) == CellState.Modifiable)
					{
						cellOffsets.Add(new(WellKnownColorIdentifier.Normal, cell));
					}
				}
				foreach (var cell in guardianMap)
				{
					candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary1, cell * 9 + guardianDigit));
				}

				var isIncomplete = IsIncomplete(AllowIncompleteUniqueRectangles, candidateOffsets);
				if (!AllowIncompleteUniqueRectangles && isIncomplete)
				{
					continue;
				}

				accumulator.Add(
					new UniqueRectangleExternalType1Or2Step(
						[.. from cell in elimMap select new Conclusion(Elimination, cell, guardianDigit)],
						[
							[
								.. cellOffsets,
								.. candidateOffsets,
								new HouseViewNode(WellKnownColorIdentifier.Normal, houseCombination[0]),
								new HouseViewNode(WellKnownColorIdentifier.Normal, houseCombination[1])
							]
						],
						context.PredefinedOptions,
						d1,
						d2,
						[.. urCells],
						in guardianMap,
						guardianDigit,
						isIncomplete,
						arMode,
						index
					)
				);
			}
		}
	}

	/// <summary>
	/// Check UR/AR + Guardian, with external subset (i.e. UR External Type 3).
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="index">The index.</param>
	/// <param name="arMode"></param>
	private void CheckExternalType3(
		List<UniqueRectangleStep> accumulator,
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
		Cell[] urCells,
		Mask comparer,
		Digit d1,
		Digit d2,
		int index,
		bool arMode
	)
	{
		var cells = (CellMap)urCells;
		if (!CheckPreconditionsOnIncomplete(in grid, urCells, d1, d2))
		{
			return;
		}

		if (!arMode && (EmptyCells & cells) != cells)
		{
			return;
		}

		// Iterate on two houses used.
		foreach (var houseCombination in cells.Houses.GetAllSets().GetSubsets(2))
		{
			var guardianMap = HousesMap[houseCombination[0]] | HousesMap[houseCombination[1]];
			if ((guardianMap & cells) != cells)
			{
				// The houses must contain all 4 UR cells.
				continue;
			}

			var guardianCells = guardianMap - cells & EmptyCells;
			foreach (ref readonly var guardianCellPair in guardianCells.GetSubsets(2))
			{
				var c1 = guardianCellPair[0];
				var c2 = guardianCellPair[1];
				if (!IsSameHouseCell(c1, c2, out var houses))
				{
					// Those two cells must lie in a same house.
					continue;
				}

				var mask = (Mask)(grid.GetCandidates(c1) | grid.GetCandidates(c2));
				if ((mask & comparer) != comparer)
				{
					// The two cells must contain both two digits.
					continue;
				}

				if ((grid.GetCandidates(c1) & comparer) == 0 || (grid.GetCandidates(c2) & comparer) == 0)
				{
					// Both two cells chosen must contain at least one of two UR digits.
					continue;
				}

				if ((guardianCells & (CandidatesMap[d1] | CandidatesMap[d2])) != guardianCellPair)
				{
					// The current map must be equal to the whole guardian full map.
					continue;
				}

				foreach (var house in houses)
				{
					var houseCells = HousesMap[house] - cells - guardianCellPair & EmptyCells;
					for (var size = 2; size <= houseCells.Count; size++)
					{
						foreach (ref readonly var otherCells in houseCells.GetSubsets(size - 1))
						{
							var subsetDigitsMask = (Mask)(grid[in otherCells] | comparer);
							if (PopCount((uint)subsetDigitsMask) != size)
							{
								// The subset cannot formed.
								continue;
							}

							// UR Guardian External Subsets found. Now check eliminations.
							var elimMap = houseCells - otherCells;
							var conclusions = new List<Conclusion>();
							foreach (var cell in elimMap)
							{
								var elimDigitsMask = guardianCellPair.Contains(cell) ? (Mask)(subsetDigitsMask & ~comparer) : subsetDigitsMask;
								foreach (var digit in elimDigitsMask)
								{
									if (CandidatesMap[digit].Contains(cell))
									{
										conclusions.Add(new(Elimination, cell, digit));
									}
								}
							}
							if (conclusions.Count == 0)
							{
								continue;
							}

							var candidateOffsets = new List<CandidateViewNode>();
							var cellOffsets = new List<CellViewNode>();
							foreach (var cell in urCells)
							{
								if (CandidatesMap[d1].Contains(cell))
								{
									candidateOffsets.Add(new(WellKnownColorIdentifier.Normal, cell * 9 + d1));
								}
								if (CandidatesMap[d2].Contains(cell))
								{
									candidateOffsets.Add(new(WellKnownColorIdentifier.Normal, cell * 9 + d2));
								}

								if (grid.GetState(cell) == CellState.Modifiable)
								{
									cellOffsets.Add(new(WellKnownColorIdentifier.Normal, cell));
								}
							}
							foreach (var cell in guardianCellPair)
							{
								if (CandidatesMap[d1].Contains(cell))
								{
									candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary2, cell * 9 + d1));
								}
								if (CandidatesMap[d2].Contains(cell))
								{
									candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary2, cell * 9 + d2));
								}
							}
							foreach (var cell in otherCells)
							{
								foreach (var digit in grid.GetCandidates(cell))
								{
									candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary1, cell * 9 + digit));
								}
							}

							var isIncomplete = IsIncomplete(AllowIncompleteUniqueRectangles, candidateOffsets);
							if (!AllowIncompleteUniqueRectangles && isIncomplete)
							{
								continue;
							}

							accumulator.Add(
								new UniqueRectangleExternalType3Step(
									[.. conclusions],
									[
										[
											.. cellOffsets,
											.. candidateOffsets,
											new HouseViewNode(WellKnownColorIdentifier.Normal, house),
											new HouseViewNode(WellKnownColorIdentifier.Auxiliary2, houseCombination[0]),
											new HouseViewNode(WellKnownColorIdentifier.Auxiliary2, houseCombination[1])
										]
									],
									context.PredefinedOptions,
									d1,
									d2,
									in cells,
									in guardianCellPair,
									in otherCells,
									subsetDigitsMask,
									isIncomplete,
									arMode,
									index
								)
							);
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// Check UR/AR + Guardian, with external conjugate pair (i.e. UR External Type 4).
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="index">The index.</param>
	/// <param name="arMode"></param>
	private void CheckExternalType4(
		List<UniqueRectangleStep> accumulator,
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
		Cell[] urCells,
		Mask comparer,
		Digit d1,
		Digit d2,
		int index,
		bool arMode
	)
	{
		var cells = (CellMap)urCells;
		if (!CheckPreconditionsOnIncomplete(in grid, urCells, d1, d2))
		{
			return;
		}

		if (!arMode && (EmptyCells & cells) != cells)
		{
			return;
		}

		// Iterate on two houses used.
		foreach (var houseCombination in cells.Houses.GetAllSets().GetSubsets(2))
		{
			var guardianMap = HousesMap[houseCombination[0]] | HousesMap[houseCombination[1]];
			if ((guardianMap & cells) != cells)
			{
				// The houses must contain all 4 UR cells.
				continue;
			}

			var guardianCells = guardianMap - cells & EmptyCells;
			foreach (ref readonly var guardianCellPair in guardianCells.GetSubsets(2))
			{
				var c1 = guardianCellPair[0];
				var c2 = guardianCellPair[1];
				if (!IsSameHouseCell(c1, c2, out var houses))
				{
					// Those two cells must lie in a same house.
					continue;
				}

				var mask = (Mask)(grid.GetCandidates(c1) | grid.GetCandidates(c2));
				if ((mask & comparer) != comparer)
				{
					// The two cells must contain both two digits.
					continue;
				}

				if ((grid.GetCandidates(c1) & comparer) == 0 || (grid.GetCandidates(c2) & comparer) == 0)
				{
					// Both two cells chosen must contain at least one of two UR digits.
					continue;
				}

				if ((guardianCells & (CandidatesMap[d1] | CandidatesMap[d2])) != guardianCellPair)
				{
					// The current map must be equal to the whole guardian full map.
					continue;
				}

				var possibleConjugatePairDigitsMask = (Mask)(grid[in guardianCellPair] & ~comparer);
				foreach (var house in houses)
				{
					foreach (var conjugatePairDigit in possibleConjugatePairDigitsMask)
					{
						if (!CandidatesMap[conjugatePairDigit].Contains(c1) || !CandidatesMap[conjugatePairDigit].Contains(c2))
						{
							// The conjugate pair can be formed if and only if both guardian cells
							// must contain that digit.
							continue;
						}

						if ((CandidatesMap[conjugatePairDigit] & HousesMap[house]) != guardianCellPair)
						{
							// The house cannot contain any other cells containing that digit.
							continue;
						}

						var conclusions = new List<Conclusion>();
						var elimDigitsMask = (Mask)(possibleConjugatePairDigitsMask & ~(1 << conjugatePairDigit));
						foreach (var elimDigit in elimDigitsMask)
						{
							foreach (var cell in CandidatesMap[elimDigit] & guardianCellPair)
							{
								conclusions.Add(new(Elimination, cell, elimDigit));
							}
						}
						if (conclusions.Count == 0)
						{
							// No eliminations found.
							continue;
						}

						var candidateOffsets = new List<CandidateViewNode>();
						var cellOffsets = new List<CellViewNode>();
						foreach (var cell in urCells)
						{
							if (CandidatesMap[d1].Contains(cell))
							{
								candidateOffsets.Add(new(WellKnownColorIdentifier.Normal, cell * 9 + d1));
							}
							if (CandidatesMap[d2].Contains(cell))
							{
								candidateOffsets.Add(new(WellKnownColorIdentifier.Normal, cell * 9 + d2));
							}

							if (grid.GetState(cell) == CellState.Modifiable)
							{
								cellOffsets.Add(new(WellKnownColorIdentifier.Normal, cell));
							}
						}
						foreach (var cell in guardianCellPair)
						{
							if (CandidatesMap[d1].Contains(cell))
							{
								candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary2, cell * 9 + d1));
							}
							if (CandidatesMap[d2].Contains(cell))
							{
								candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary2, cell * 9 + d2));
							}

							candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary1, cell * 9 + conjugatePairDigit));
						}

						var isIncomplete = IsIncomplete(AllowIncompleteUniqueRectangles, candidateOffsets);
						if (!AllowIncompleteUniqueRectangles && isIncomplete)
						{
							continue;
						}

						accumulator.Add(
							new UniqueRectangleExternalType4Step(
								[.. conclusions],
								[
									[
										.. cellOffsets,
										.. candidateOffsets,
										new HouseViewNode(WellKnownColorIdentifier.Normal, house),
										new HouseViewNode(WellKnownColorIdentifier.Auxiliary2, houseCombination[0]),
										new HouseViewNode(WellKnownColorIdentifier.Auxiliary2, houseCombination[1])
									]
								],
								context.PredefinedOptions,
								d1,
								d2,
								in cells,
								in guardianCellPair,
								new(in guardianCellPair, conjugatePairDigit),
								isIncomplete,
								arMode,
								index
							)
						);
					}
				}
			}
		}
	}

	/// <summary>
	/// Check UR + Guardian, with external turbot fish.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="comparer">The comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="index">The mask index.</param>
	private void CheckExternalTurbotFish(
		List<UniqueRectangleStep> accumulator,
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
		Cell[] urCells,
		Mask comparer,
		Digit d1,
		Digit d2,
		int index
	)
	{
		var cells = (CellMap)urCells;
		if (!CheckPreconditionsOnIncomplete(in grid, urCells, d1, d2))
		{
			return;
		}

		// Iterates on each digit, checking whether the current digit forms a guardian pattern but the other digit not.
		foreach (var (guardianDigit, nonGuardianDigit) in ((d1, d2), (d2, d1)))
		{
			// Iterates on each pair of houses where the UR pattern located.
			foreach (var houses in cells.Houses.GetAllSets().GetSubsets(2))
			{
				if (HousesMap[houses[0]] & HousesMap[houses[1]])
				{
					// Two houses iterated must contain no intersection.
					continue;
				}

				var housesFullMap = HousesMap[houses[0]] | HousesMap[houses[1]];

				// Gets the guardian cells in both houses.
				// Here guardian cells may contain multiple cells. We don't check for it because it can be used as grouped turbot fish.
				var guardianCells = (housesFullMap & CandidatesMap[guardianDigit]) - cells;

				// Then check whether the other digit is locked in the UR pattern.
				var flag = true;
				foreach (var house in houses)
				{
					var tempMap = HousesMap[house] & CandidatesMap[nonGuardianDigit];
					if ((cells & tempMap) != tempMap || tempMap.Count != 2)
					{
						// The current house may not form a valid conjugate pair
						// because the current house may contain at least 3 cells can appear that digit.
						flag = false;
						break;
					}
				}
				if (!flag)
				{
					continue;
				}

				if (guardianCells.CoveredHouses != 0)
				{
					// It is a UR type 4 because all possible guardian cells lie in a same house.
					continue;
				}

				// Gets the last cell that the houses iterated don't contain.
				// For example, if the houses iterated are a row and a column, then the houses may cover 3 cells in the UR pattern.
				// Here we should get the cell uncovered, to check whether it is a bi-value cell and only contains the digits used by UR.
				//
				// I give a diagram to show what I want to tell you.
				// If the last cell not covered contains not only the first and the second digit in UR:
				//
				//         a
				//         ↓
				//   ab → abx | aby
				//    b → abz | abw
				//
				// Where the arrow means the current row or column contains a strong link of that digit.
				// Here the cell 'abw' isn't covered for strong links of digit a.
				// If we suppose that both cells 'aby' and 'abz' is filled with digit b, the pattern will be reduced:
				//
				//         a
				//         ↓
				//   ab →  a | b
				//    b →  b | ?
				//
				// But what digit will be filled with 'abw'? We don't know! We cannot decide which digit will be filled with the cell,
				// so the final digit a (causing a deadly pattern) may not be formed.
				// We should exclude this case to make the pattern be strict, which is what I want to tell you.
				if ((cells - housesFullMap)[0] is var lastCell and not -1
					&& (!BivalueCells.Contains(lastCell) || comparer != grid.GetCandidates(lastCell)))
				{
					continue;
				}

				// Check whether guardian cells cannot create links to form a turbot fish.
				var (a, b) = (getAvailableHouses(houses[0], in guardianCells), getAvailableHouses(houses[1], in guardianCells));
				if (a == 0 && b == 0)
				{
					continue;
				}

				foreach (var weakLinkHouse in a | b)
				{
					var otherCellsInWeakLinkHouse = (HousesMap[weakLinkHouse] & CandidatesMap[guardianDigit]) - guardianCells;
					if (!otherCellsInWeakLinkHouse)
					{
						// Cannot continue the turbot fish.
						continue;
					}

					foreach (var otherCellInWeakLinkHouse in otherCellsInWeakLinkHouse)
					{
						foreach (var strongLinkHouse in CellsMap[otherCellInWeakLinkHouse].Houses)
						{
							if ((HousesMap[strongLinkHouse] & CandidatesMap[guardianDigit]) - otherCellInWeakLinkHouse is not [var finalCell])
							{
								// No eliminations will exist in this case.
								continue;
							}

							// A turbot fish found. Now check eliminations.
							var elimMap = (guardianCells - (HousesMap[weakLinkHouse] & guardianCells)).PeerIntersection & PeersMap[finalCell] & CandidatesMap[guardianDigit];
							if (!elimMap)
							{
								// No eliminations.
								continue;
							}

							var candidateOffsets = new List<CandidateViewNode>();
							foreach (var cell in cells)
							{
								foreach (var digit in (Mask)(grid.GetCandidates(cell) & comparer))
								{
									candidateOffsets.Add(new(WellKnownColorIdentifier.Normal, cell * 9 + digit));
								}
							}
							foreach (var cell in guardianCells)
							{
								candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary1, cell * 9 + guardianDigit));
							}
							candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary2, otherCellInWeakLinkHouse * 9 + guardianDigit));
							candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary2, finalCell * 9 + guardianDigit));

							var isIncomplete = IsIncomplete(AllowIncompleteUniqueRectangles, candidateOffsets);
							if (!AllowIncompleteUniqueRectangles && isIncomplete)
							{
								continue;
							}

							accumulator.Add(
								new UniqueRectangleExternalTurbotFishStep(
									[.. from cell in elimMap select new Conclusion(Elimination, cell, guardianDigit)],
									[
										[
											.. candidateOffsets,
											new HouseViewNode(WellKnownColorIdentifier.Normal, houses[0]),
											new HouseViewNode(WellKnownColorIdentifier.Normal, houses[1]),
											new HouseViewNode(WellKnownColorIdentifier.Auxiliary1, weakLinkHouse),
											new HouseViewNode(WellKnownColorIdentifier.Auxiliary2, strongLinkHouse)
										]
									],
									context.PredefinedOptions,
									d1,
									d2,
									in cells,
									in guardianCells,
									isIncomplete,
									index
								)
							);
						}
					}
				}
			}
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static HouseMask getAvailableHouses(House house, scoped ref readonly CellMap guardianCells)
		{
			var intersection = guardianCells & HousesMap[house];
			return house switch
			{
				< 9 => (TrailingZeroCount(intersection.RowMask), TrailingZeroCount(intersection.ColumnMask)) switch
				{
					(var a and not InvalidTrailingZeroCountMethodFallback, var b and not InvalidTrailingZeroCountMethodFallback)
						=> 1 << a + 9 | 1 << b + 18,
					(var a and not InvalidTrailingZeroCountMethodFallback, _) => 1 << a + 9,
					(_, var b and not InvalidTrailingZeroCountMethodFallback) => 1 << b + 18,
					_ => 0
				},
				_ => TrailingZeroCount(intersection.BlockMask) switch
				{
					var result and not InvalidTrailingZeroCountMethodFallback => 1 << result,
					_ => 0
				}
			};
		}
	}

	/// <summary>
	/// Check UR + Guardian, with external W-Wing.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="comparer">The comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="index">The mask index.</param>
	private void CheckExternalWWing(
		List<UniqueRectangleStep> accumulator,
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
		Cell[] urCells,
		Mask comparer,
		Digit d1,
		Digit d2,
		int index
	)
	{
		// Collect all digits that all bi-value cells in the current grid used.
		// W-Wing should contain a pair of cells which contain same 2 digits.
		// This step will collect all digits used, in order to check which cells will be skipped.
		var (bivalueCellsDigitsMask, bivalueCellsFiltered) = ((Mask)0, CellMap.Empty);
		foreach (var cell in BivalueCells)
		{
			// Check whether the current cell contain either the first or the second digit appeared in UR.
			// This step is important, because the external W-Wing must hold at least one digit appeared in UR.
			// According to the following W-Wing pattern:
			//
			//   (w=x)-(x=x)-(x=w)
			//
			// this method will start the chaining from the middle of the pattern. From this pattern, we can learn that
			// digit x must be a guardian digit, so it must be the first or the second digit appeared in UR.
			// Therefore, no matter what digit w will be, the digit x must be a UR digit.
			var digitsMask = grid.GetCandidates(cell);
			if ((digitsMask & comparer) == 0)
			{
				continue;
			}

			bivalueCellsFiltered.Add(cell);
			bivalueCellsDigitsMask |= digitsMask;
		}

		// Check whether at least one bi-value cell has at least one UR digit.
		if ((bivalueCellsDigitsMask & comparer) == 0)
		{
			return;
		}

		var cells = (CellMap)urCells;
		if (!CheckPreconditionsOnIncomplete(in grid, urCells, d1, d2))
		{
			return;
		}

		// Iterates on each digit, checking whether the current digit forms a guardian pattern but the other digit not.
		// Here the variable 'xDigit' has a same concept as guardian digit.
		foreach (var (xDigit, nonGuardianDigit) in ((d1, d2), (d2, d1)))
		{
			// Iterates on each pair of houses where the UR pattern located.
			foreach (var houses in cells.Houses.GetAllSets().GetSubsets(2))
			{
				if (HousesMap[houses[0]] & HousesMap[houses[1]])
				{
					// Two houses iterated must contain no intersection.
					continue;
				}

				var housesFullMap = HousesMap[houses[0]] | HousesMap[houses[1]];

				// Gets the guardian cells in both houses.
				// Here guardian cells may contain multiple cells. We don't check for it because it can be used as grouped turbot fish.
				var guardianCells = (housesFullMap & CandidatesMap[xDigit]) - cells;

				// Then check whether the other digit is locked in the UR pattern.
				var flag = true;
				foreach (var house in houses)
				{
					var tempMap = HousesMap[house] & CandidatesMap[nonGuardianDigit];
					if ((cells & tempMap) != tempMap || tempMap.Count != 2)
					{
						// The current house may not form a valid conjugate pair
						// because the current house may contain at least 3 cells can appear that digit.
						flag = false;
						break;
					}
				}
				if (!flag)
				{
					continue;
				}

				if (guardianCells.CoveredHouses != 0)
				{
					// It is a UR type 4 because all possible guardian cells lie in a same house.
					continue;
				}

				// Gets the last cell that the houses iterated don't contain.
				// Why we should do this? Please see the comments above this method (same statement in the method "UR external turbot fish").
				if ((cells - housesFullMap)[0] is var lastCell and not -1
					&& (!BivalueCells.Contains(lastCell) || comparer != grid.GetCandidates(lastCell)))
				{
					continue;
				}

				// Check whether guardian cells cannot create links to form a W-Wing.
				var (a, b) = (getAvailableHouses(houses[0], in guardianCells), getAvailableHouses(houses[1], in guardianCells));
				if (a == 0 || b == 0)
				{
					continue;
				}

				foreach (var weakLinkHouses in (a | b).GetAllSets().GetSubsets(2))
				{
					if (((HousesMap[weakLinkHouses[0]] | HousesMap[weakLinkHouses[1]]) & guardianCells) != guardianCells)
					{
						continue;
					}

					var bivalueCellsFoundStart = ((HousesMap[weakLinkHouses[0]] & CandidatesMap[xDigit]) - guardianCells) & BivalueCells;
					var bivalueCellsFoundEnd = ((HousesMap[weakLinkHouses[1]] & CandidatesMap[xDigit]) - guardianCells) & BivalueCells;
					foreach (var startCell in bivalueCellsFoundStart)
					{
						var startCellDigitsMask = grid.GetCandidates(startCell);
						foreach (var endCell in bivalueCellsFoundEnd - startCell)
						{
							var endCellDigitsMask = grid.GetCandidates(endCell);
							if (startCellDigitsMask != endCellDigitsMask)
							{
								continue;
							}

							// A valid pattern is found. Now check eliminations.
							var wDigit = TrailingZeroCount((Mask)(startCellDigitsMask & ~(1 << xDigit)));
							var elimMap = (CellsMap[startCell] + endCell).PeerIntersection & CandidatesMap[wDigit];
							if (!elimMap)
							{
								// No eliminations found.
								continue;
							}

							var candidateOffsets = new List<CandidateViewNode>();
							foreach (var cell in urCells)
							{
								foreach (var digit in comparer & grid.GetCandidates(cell))
								{
									candidateOffsets.Add(new(WellKnownColorIdentifier.Normal, cell * 9 + digit));
								}
							}
							foreach (var cell in guardianCells)
							{
								candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary1, cell * 9 + xDigit));
							}
							candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary2, startCell * 9 + xDigit));
							candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary2, startCell * 9 + wDigit));
							candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary2, endCell * 9 + xDigit));
							candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary2, endCell * 9 + wDigit));

							var isIncomplete = IsIncomplete(AllowIncompleteUniqueRectangles, candidateOffsets);
							if (!AllowIncompleteUniqueRectangles && isIncomplete)
							{
								continue;
							}

							accumulator.Add(
								new UniqueRectangleExternalWWingStep(
									[.. from cell in elimMap select new Conclusion(Elimination, cell, wDigit)],
									[
										[
											.. candidateOffsets,
											new HouseViewNode(WellKnownColorIdentifier.Normal, houses[0]),
											new HouseViewNode(WellKnownColorIdentifier.Normal, houses[1]),
											new HouseViewNode(WellKnownColorIdentifier.Auxiliary1, weakLinkHouses[0]),
											new HouseViewNode(WellKnownColorIdentifier.Auxiliary1, weakLinkHouses[1])
										]
									],
									context.PredefinedOptions,
									d1,
									d2,
									in cells,
									in guardianCells,
									[startCell, endCell],
									isIncomplete,
									false,
									index
								)
							);
						}
					}
				}
			}
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static HouseMask getAvailableHouses(House house, scoped ref readonly CellMap guardianCells)
		{
			var intersection = guardianCells & HousesMap[house];
			return house switch
			{
				< 9 => (TrailingZeroCount(intersection.RowMask), TrailingZeroCount(intersection.ColumnMask)) switch
				{
					(var a and not InvalidTrailingZeroCountMethodFallback, var b and not InvalidTrailingZeroCountMethodFallback)
						=> 1 << a + 9 | 1 << b + 18,
					(var a and not InvalidTrailingZeroCountMethodFallback, _) => 1 << a + 9,
					(_, var b and not InvalidTrailingZeroCountMethodFallback) => 1 << b + 18,
					_ => 0
				},
				_ => TrailingZeroCount(intersection.BlockMask) switch
				{
					var result and not InvalidTrailingZeroCountMethodFallback => 1 << result,
					_ => 0
				}
			};
		}
	}

	/// <summary>
	/// Check UR/AR + Guardian, with external XY-Wing.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="comparer">The comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="index">The mask index.</param>
	/// <param name="arMode"></param>
	private void CheckExternalXyWing(
		List<UniqueRectangleStep> accumulator,
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
		Cell[] urCells,
		Mask comparer,
		Digit d1,
		Digit d2,
		int index,
		bool arMode
	)
	{
		var cells = (CellMap)urCells;
		if (!CheckPreconditionsOnIncomplete(in grid, urCells, d1, d2))
		{
			return;
		}

		if (!arMode && (EmptyCells & cells) != cells)
		{
			return;
		}

		// Iterate on two houses used.
		foreach (var houseCombination in cells.Houses.GetAllSets().GetSubsets(2))
		{
			var guardianMap = HousesMap[houseCombination[0]] | HousesMap[houseCombination[1]];
			if ((guardianMap & cells) != cells)
			{
				// The houses must contain all 4 UR cells.
				continue;
			}

			var guardianCells = guardianMap - cells & (CandidatesMap[d1] | CandidatesMap[d2]);
			if (!(guardianCells & CandidatesMap[d1]) || !(guardianCells & CandidatesMap[d2]))
			{
				// Guardian cells must contain both two digits; otherwise, skip the current case.
				continue;
			}

			var cellsToEnumerate = guardianCells.ExpandedPeers - guardianCells & (CandidatesMap[d1] | CandidatesMap[d2]);
			if (cellsToEnumerate.Count < 2)
			{
				// No valid combinations.
				continue;
			}

			//forOneEndoLeaf(in grid, ref context, in cellsToEnumerate, in guardianCells, houseCombination);
			forBothExoLeaves(in grid, ref context, in cellsToEnumerate, in guardianCells, houseCombination);
		}


#pragma warning disable CS8321
		void forOneEndoLeaf(
			scoped ref readonly Grid grid,
			scoped ref AnalysisContext context,
			scoped ref readonly CellMap cellsToEnumerate,
			scoped ref readonly CellMap guardianCells,
			House[] houseCombination
		)
		{
			foreach (var cell1 in guardianCells)
			{
				foreach (var cell2 in cellsToEnumerate)
				{
					if (!PeersMap[cell1].Contains(cell2))
					{
						// Two cells cannot see each other.
						continue;
					}

					var (mask1, mask2) = (grid.GetCandidates(cell1), grid.GetCandidates(cell2));
					var intersectionMask = (Mask)(mask1 & mask2);
					if (!IsPow2(intersectionMask))
					{
						// No eliminations can be found in this pattern.
						continue;
					}

					var unionMask = (Mask)(mask1 | mask2);
					if ((unionMask & comparer) != comparer)
					{
						// The two cells must contain both two digits.
						continue;
					}

					if ((unionMask & ~(comparer | intersectionMask)) != 0)
					{
						// These two cells may contain extra cells, which is disallowed in this pattern.
						continue;
					}

					var elimDigit = TrailingZeroCount(intersectionMask);
					if ((mask1 >> elimDigit & 1) == 0)
					{
						// No eliminations found.
						continue;
					}

					var (candidateOffsets, cellOffsets) = (new List<CandidateViewNode>(), new List<CellViewNode>());
					foreach (var cell in urCells)
					{
						switch (grid.GetState(cell))
						{
							case CellState.Empty:
							{
								foreach (var digit in (Mask)(grid.GetCandidates(cell) & comparer))
								{
									candidateOffsets.Add(new(WellKnownColorIdentifier.Normal, cell * 9 + digit));
								}
								break;
							}
							case CellState.Modifiable:
							{
								cellOffsets.Add(new(WellKnownColorIdentifier.Normal, cell));
								break;
							}
						}
					}
					foreach (var cell in guardianCells)
					{
						foreach (var digit in grid.GetCandidates(cell))
						{
							if (digit == d1 || digit == d2)
							{
								candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary2, cell * 9 + digit));
							}
						}
					}

					var cellPair = CellsMap[cell1] + cell2;
					foreach (var cell in cellPair)
					{
						foreach (var digit in grid.GetCandidates(cell))
						{
							// Elimination cannot be colorized.
							if (cell != cell1 || digit != elimDigit)
							{
								candidateOffsets.Add(
									new(
										digit != d1 && digit != d2 ? WellKnownColorIdentifier.Auxiliary1 : WellKnownColorIdentifier.Auxiliary2,
										cell * 9 + digit
									)
								);
							}
						}
					}

					var isIncomplete = IsIncomplete(AllowIncompleteUniqueRectangles, candidateOffsets);
					if (!AllowIncompleteUniqueRectangles && isIncomplete)
					{
						continue;
					}

					accumulator.Add(
						new UniqueRectangleExternalXyWingStep(
							[new(Elimination, cell1, elimDigit)],
							[[.. cellOffsets, .. candidateOffsets]],
							context.PredefinedOptions,
							d1,
							d2,
							in cells,
							in guardianCells,
							in cellPair,
							isIncomplete,
							arMode,
							index
						)
					);
				}
			}
		}
#pragma warning restore CS8321

		void forBothExoLeaves(
			scoped ref readonly Grid grid,
			scoped ref AnalysisContext context,
			scoped ref readonly CellMap cellsToEnumerate,
			scoped ref readonly CellMap guardianCells,
			House[] houseCombination
		)
		{
			foreach (ref readonly var cellPair in cellsToEnumerate.GetSubsets(2))
			{
				var (cell1, cell2) = (cellPair[0], cellPair[1]);
				var (mask1, mask2) = (grid.GetCandidates(cell1), grid.GetCandidates(cell2));
				var intersectionMask = (Mask)(mask1 & mask2);
				if (!IsPow2(intersectionMask))
				{
					// No eliminations can be found in this pattern.
					continue;
				}

				var unionMask = (Mask)(mask1 | mask2);
				if ((unionMask & comparer) != comparer)
				{
					// The two cells must contain both two digits.
					continue;
				}

				if ((unionMask & ~(comparer | intersectionMask)) != 0)
				{
					// These two cells may contain extra cells, which is disallowed in this pattern.
					continue;
				}

				var cell1UrDigit = TrailingZeroCount((Mask)(mask1 & ~intersectionMask));
				var cell2UrDigit = TrailingZeroCount((Mask)(mask2 & ~intersectionMask));
				var guardianCellsThatContainsDigit1 = guardianCells & CandidatesMap[cell1UrDigit];
				var guardianCellsThatContainsDigit2 = guardianCells & CandidatesMap[cell2UrDigit];
				if ((PeersMap[cell1] & guardianCellsThatContainsDigit1) != guardianCellsThatContainsDigit1
					|| (PeersMap[cell2] & guardianCellsThatContainsDigit2) != guardianCellsThatContainsDigit2)
				{
					// Two cells must see all guardian cells.
					continue;
				}

				// UR External XY-Wing found. Now check for eliminations.
				var elimDigit = TrailingZeroCount(intersectionMask);
				var elimMap = cellPair.PeerIntersection & CandidatesMap[elimDigit];
				if (!elimMap)
				{
					// No elimination cell.
					continue;
				}

				var (candidateOffsets, cellOffsets) = (new List<CandidateViewNode>(), new List<CellViewNode>());
				foreach (var cell in urCells)
				{
					switch (grid.GetState(cell))
					{
						case CellState.Empty:
						{
							foreach (var digit in (Mask)(grid.GetCandidates(cell) & comparer))
							{
								candidateOffsets.Add(new(WellKnownColorIdentifier.Normal, cell * 9 + digit));
							}
							break;
						}
						case CellState.Modifiable:
						{
							cellOffsets.Add(new(WellKnownColorIdentifier.Normal, cell));
							break;
						}
					}
				}
				foreach (var cell in guardianCells)
				{
					foreach (var digit in grid.GetCandidates(cell))
					{
						if (digit == d1 || digit == d2)
						{
							candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary2, cell * 9 + digit));
						}
					}
				}
				foreach (var cell in cellPair)
				{
					foreach (var digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(
							new(
								digit != d1 && digit != d2 ? WellKnownColorIdentifier.Auxiliary1 : WellKnownColorIdentifier.Auxiliary2,
								cell * 9 + digit
							)
						);
					}
				}

				var isIncomplete = IsIncomplete(AllowIncompleteUniqueRectangles, candidateOffsets);
				if (!AllowIncompleteUniqueRectangles && isIncomplete)
				{
					continue;
				}

				accumulator.Add(
					new UniqueRectangleExternalXyWingStep(
						[.. from cell in elimMap select new Conclusion(Elimination, cell, elimDigit)],
						[
							[
								.. cellOffsets,
								.. candidateOffsets,
								new HouseViewNode(WellKnownColorIdentifier.Normal, houseCombination[0]),
								new HouseViewNode(WellKnownColorIdentifier.Normal, houseCombination[1])
							]
						],
						context.PredefinedOptions,
						d1,
						d2,
						in cells,
						in guardianCells,
						in cellPair,
						isIncomplete,
						arMode,
						index
					)
				);
			}
		}
	}

	/// <summary>
	/// Check UR/AR + Guardian, with external ALS-XZ rule.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="alses">The ALS structures.</param>
	/// <param name="comparer">The comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="index">The mask index.</param>
	/// <param name="arMode"></param>
	private void CheckExternalAlmostLockedSetsXz(
		List<UniqueRectangleStep> accumulator,
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
		Cell[] urCells,
		scoped ReadOnlySpan<AlmostLockedSet> alses,
		Mask comparer,
		Digit d1,
		Digit d2,
		int index,
		bool arMode
	)
	{
		var cells = (CellMap)urCells;
		if (!CheckPreconditionsOnIncomplete(in grid, urCells, d1, d2))
		{
			return;
		}

		if (!arMode && (EmptyCells & cells) != cells)
		{
			return;
		}

		// Iterate on two houses used.
		foreach (var houseCombination in cells.Houses.GetAllSets().GetSubsets(2))
		{
			var guardianMap = HousesMap[houseCombination[0]] | HousesMap[houseCombination[1]];
			if ((guardianMap & cells) != cells)
			{
				// The houses must contain all 4 UR cells.
				continue;
			}

			var guardianCells = guardianMap - cells & (CandidatesMap[d1] | CandidatesMap[d2]);
			if (guardianCells is { Count: not 1, CoveredHouses: 0 })
			{
				// All guardian cells must lie in one house.
				continue;
			}

			if (!(guardianCells & CandidatesMap[d1]) || !(guardianCells & CandidatesMap[d2]))
			{
				// Guardian cells must contain both two digits; otherwise, skip the current case.
				continue;
			}

			foreach (var (zDigit, xDigit) in ((d1, d2), (d2, d1)))
			{
				var xDigitGuardianCells = CandidatesMap[xDigit] & guardianCells;
				var zDigitGuardianCells = CandidatesMap[zDigit] & guardianCells;
				foreach (var als in alses)
				{
					var alsHouse = als.House;
					var alsMask = als.DigitsMask;
					var alsMap = als.Cells;
					if ((cells.Houses >> alsHouse & 1) != 0)
					{
						// The current ALS cannot lie in the house that UR cells covered.
						continue;
					}

					if ((alsMask >> d1 & 1) == 0 || (alsMask >> d2 & 1) == 0)
					{
						// The ALS must uses both two digits.
						continue;
					}

					if (!!(alsMap & xDigitGuardianCells) || zDigitGuardianCells == (alsMap & CandidatesMap[zDigit]))
					{
						// The ALS cannot only use X or Z digits that all appears in guardian cells.
						continue;
					}

					var elimMap = (alsMap | zDigitGuardianCells) % CandidatesMap[zDigit];
					if (!elimMap)
					{
						// No eliminations found.
						continue;
					}

					var xDigitMap = (alsMap | xDigitGuardianCells) & CandidatesMap[xDigit];
					if (!xDigitMap.InOneHouse(out _))
					{
						// The X digit must be connected.
						continue;
					}

					// ALS-XZ formed.
					var candidateOffsets = new List<CandidateViewNode>();
					var cellOffsets = new List<CellViewNode>();
					foreach (var urCell in urCells)
					{
						switch (grid.GetState(urCell))
						{
							case CellState.Empty:
							{
								foreach (var digit in comparer)
								{
									if (CandidatesMap[digit].Contains(urCell))
									{
										candidateOffsets.Add(new(WellKnownColorIdentifier.Normal, urCell * 9 + digit));
									}
								}
								break;
							}
							case CellState.Modifiable:
							{
								cellOffsets.Add(new(WellKnownColorIdentifier.Normal, urCell));
								break;
							}
						}
					}
					foreach (var xDigitCell in xDigitGuardianCells)
					{
						candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary2, xDigitCell * 9 + xDigit));
					}
					foreach (var zDigitCell in zDigitGuardianCells)
					{
						candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary2, zDigitCell * 9 + zDigit));
					}
					foreach (var alsCell in alsMap)
					{
						foreach (var digit in grid.GetCandidates(alsCell))
						{
							candidateOffsets.Add(
								new(
									digit == d1 || digit == d2 ? WellKnownColorIdentifier.Auxiliary1 : WellKnownColorIdentifier.AlmostLockedSet1,
									alsCell * 9 + digit
								)
							);
						}
					}

					var isIncomplete = IsIncomplete(AllowIncompleteUniqueRectangles, candidateOffsets);
					if (!AllowIncompleteUniqueRectangles && isIncomplete)
					{
						continue;
					}

					accumulator.Add(
						new UniqueRectangleExternalAlmostLockedSetsXzStep(
							[.. from cell in elimMap select new Conclusion(Elimination, cell, zDigit)],
							[[.. candidateOffsets, .. cellOffsets, new HouseViewNode(WellKnownColorIdentifier.AlmostLockedSet1, alsHouse)]],
							context.PredefinedOptions,
							d1,
							d2,
							in cells,
							in guardianCells,
							als,
							isIncomplete,
							arMode,
							index
						)
					);
				}
			}
		}
	}

	/// <summary>
	/// Check AR + Hidden single.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="context">The context.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="corner1">The corner cell 1.</param>
	/// <param name="corner2">The corner cell 2.</param>
	/// <param name="otherCellsMap">
	/// The map of other cells during the current UR searching.
	/// </param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// <para>The pattern:</para>
	/// <para>
	/// <code><![CDATA[
	/// ↓corner1
	/// a   | aby  -  -
	/// abx | a    -  b
	///     | -    -  -
	///       ↑corner2(cell 'a')
	/// ]]></code>
	/// There's only one cell can be filled with the digit 'b' besides the cell 'aby'.
	/// </para>
	/// </remarks>
	private void CheckHiddenSingleAvoidable(
		List<UniqueRectangleStep> accumulator,
		scoped ref readonly Grid grid,
		scoped ref AnalysisContext context,
		Cell[] urCells,
		Digit d1,
		Digit d2,
		Cell corner1,
		Cell corner2,
		scoped ref readonly CellMap otherCellsMap,
		int index
	)
	{
		if (grid.GetState(corner1) != CellState.Modifiable || grid.GetState(corner2) != CellState.Modifiable
			|| grid.GetDigit(corner1) != grid.GetDigit(corner2) || grid.GetDigit(corner1) != d1 && grid.GetDigit(corner1) != d2)
		{
			return;
		}

		// Get the base digit ('a') and the other digit ('b').
		// Here 'b' is the digit that we should check the possible hidden single.
		var baseDigit = grid.GetDigit(corner1);
		var otherDigit = baseDigit == d1 ? d2 : d1;
		var cellsThatTwoOtherCellsBothCanSee = otherCellsMap.PeerIntersection & CandidatesMap[otherDigit];

		// Iterate on two cases (because we holds two other cells,
		// and both those two cells may contain possible elimination).
		for (var i = 0; i < 2; i++)
		{
			var (baseCell, anotherCell) = i == 0 ? (otherCellsMap[0], otherCellsMap[1]) : (otherCellsMap[1], otherCellsMap[0]);

			// Iterate on each house type.
			foreach (var houseType in HouseTypes)
			{
				var houseIndex = baseCell.ToHouseIndex(houseType);

				// If the house doesn't overlap with the specified house, just skip it.
				if (!(cellsThatTwoOtherCellsBothCanSee & HousesMap[houseIndex]))
				{
					continue;
				}

				var otherCells = HousesMap[houseIndex] & CandidatesMap[otherDigit] & PeersMap[anotherCell];
				var sameHouses = (otherCells + anotherCell).CoveredHouses;
				foreach (var sameHouse in sameHouses)
				{
					// Check whether all possible positions of the digit 'b' in this house only
					// lies in the given cells above ('cellsThatTwoOtherCellsBothCanSee').
					if ((HousesMap[sameHouse] - anotherCell & CandidatesMap[otherDigit]) != otherCells)
					{
						continue;
					}

					// Possible hidden single found.
					// If the elimination doesn't exist, just skip it.
					if (!CandidatesMap[otherDigit].Contains(baseCell))
					{
						continue;
					}

					var cellOffsets = new List<CellViewNode>();
					foreach (var cell in urCells)
					{
						cellOffsets.Add(new(WellKnownColorIdentifier.Normal, cell));
					}

					var candidateOffsets = new List<CandidateViewNode> { new(WellKnownColorIdentifier.Normal, anotherCell * 9 + otherDigit) };
					foreach (var cell in otherCells)
					{
						candidateOffsets.Add(new(WellKnownColorIdentifier.Auxiliary1, cell * 9 + otherDigit));
					}

					accumulator.Add(
						new AvoidableRectangleWithHiddenSingleStep(
							[new(Elimination, baseCell, otherDigit)],
							[[.. cellOffsets, .. candidateOffsets, new HouseViewNode(WellKnownColorIdentifier.Normal, sameHouse)]],
							context.PredefinedOptions,
							d1,
							d2,
							[.. urCells],
							baseCell,
							anotherCell,
							sameHouse,
							index
						)
					);
				}
			}
		}
	}


	/// <summary>
	/// Check preconditions.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is searching for ARs.</param>
	/// <returns>Indicates whether the UR is passed to check.</returns>
	private static bool CheckPreconditions(scoped ref readonly Grid grid, Cell[] urCells, bool arMode)
	{
		var emptyCountWhenArMode = (byte)0;
		var modifiableCount = (byte)0;
		foreach (var urCell in urCells)
		{
			switch (grid.GetState(urCell))
			{
				case CellState.Given:
				case CellState.Modifiable when !arMode:
				{
					return false;
				}
				case CellState.Empty when arMode:
				{
					emptyCountWhenArMode++;
					break;
				}
				case CellState.Modifiable:
				{
					modifiableCount++;
					break;
				}
			}
		}

		return modifiableCount != 4 && emptyCountWhenArMode != 4;
	}

	/// <summary>
	/// Checks whether the specified UR cells satisfies the precondition of an incomplete UR.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">The UR cells.</param>
	/// <param name="d1">The first digit used.</param>
	/// <param name="d2">The second digit used.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	private static bool CheckPreconditionsOnIncomplete(scoped ref readonly Grid grid, Cell[] urCells, Digit d1, Digit d2)
	{
		// Same-sided cells cannot contain only one digit of two digits 'd1' and 'd2'.
		foreach (var (a, b) in ((0, 1), (2, 3), (0, 2), (1, 3)))
		{
			var gatheredMask = (Mask)(grid.GetCandidates(urCells[a]) | grid.GetCandidates(urCells[b]));
			if ((gatheredMask >> d1 & 1) == 0 || (gatheredMask >> d2 & 1) == 0)
			{
				return false;
			}
		}

		// All four cells must contain at least one digit appeared in the UR.
		var comparer = (Mask)(1 << d1 | 1 << d2);
		foreach (var cell in urCells)
		{
			if ((grid.GetCandidates(cell) & comparer) == 0)
			{
				return false;
			}
		}

		return true;
	}

	/// <summary>
	/// To determine whether the specified house forms a conjugate pair
	/// of the specified digit, and the cells where they contain the digit
	/// is same as the given map contains.
	/// </summary>
	/// <param name="digit">The digit.</param>
	/// <param name="map">The map.</param>
	/// <param name="houseIndex">The house index.</param>
	/// <returns>A <see cref="bool"/> value.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool IsConjugatePair(Digit digit, scoped ref readonly CellMap map, House houseIndex)
		=> (HousesMap[houseIndex] & CandidatesMap[digit]) == map;

	/// <summary>
	/// Get whether two cells are in a same house.
	/// </summary>
	/// <param name="cell1">The cell 1 to check.</param>
	/// <param name="cell2">The cell 2 to check.</param>
	/// <param name="houses">
	/// The result houses that both two cells lie in. If the cell can't be found, this argument will be 0.
	/// </param>
	/// <returns>
	/// The <see cref="bool"/> value indicating whether the another cell is same house as the current one.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool IsSameHouseCell(Cell cell1, Cell cell2, out HouseMask houses)
	{
		var v = (CellsMap[cell1] + cell2).CoveredHouses;
		(var r, houses) = v != 0 ? (true, v) : (false, 0);
		return r;
	}

	/// <summary>
	/// Check whether the highlight UR candidates is incomplete.
	/// </summary>
	/// <param name="allowIncomplete"><inheritdoc cref="AllowIncompleteUniqueRectangles" path="/summary"/></param>
	/// <param name="list">The list to check.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	/// <remarks>
	/// This method uses a trick to check a UR pattern: to count up the number of "Normal colored"
	/// candidates used in the current UR pattern. If and only if the full pattern uses 8 candidates
	/// colored with normal one, the pattern will be complete.
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool IsIncomplete(bool allowIncomplete, List<CandidateViewNode> list)
		=> allowIncomplete && list.Count(static d => d.Identifier is WellKnownColorIdentifier { Kind: WellKnownColorIdentifierKind.Normal }) != 8;

	/// <summary>
	/// Get a cell that can't see each other.
	/// </summary>
	/// <param name="urCells">The UR cells.</param>
	/// <param name="cell">The current cell.</param>
	/// <returns>The diagonal cell.</returns>
	/// <exception cref="ArgumentException">Throws when the specified argument <paramref name="cell"/> is invalid.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static Cell GetDiagonalCell(Cell[] urCells, Cell cell)
		=> cell == urCells[0] ? urCells[3] : cell == urCells[1] ? urCells[2] : cell == urCells[2] ? urCells[1] : urCells[0];

	/// <summary>
	/// Get all highlight cells.
	/// </summary>
	/// <param name="urCells">The all UR cells used.</param>
	/// <returns>The list of highlight cells.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static CellViewNode[] GetHighlightCells(Cell[] urCells)
		=> from urCell in urCells select new CellViewNode(WellKnownColorIdentifier.Normal, urCell);
}
