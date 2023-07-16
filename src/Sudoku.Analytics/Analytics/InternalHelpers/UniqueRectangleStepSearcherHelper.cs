namespace Sudoku.Analytics.InternalHelpers;

/// <summary>
/// Used by <see cref="UniqueRectangleStepSearcher"/>.
/// </summary>
/// <seealso cref="UniqueRectangleStepSearcher"/>
internal static class UniqueRectangleStepSearcherHelper
{
	/// <summary>
	/// Indicates the number of all total possible UR patterns.
	/// The value is same as the length of field <see cref="UniqueRectanglePatterns"/>.
	/// </summary>
	/// <seealso cref="UniqueRectanglePatterns"/>
	public const int CountOfPatterns = 486;


	/// <summary>
	/// The table of all <b>Unique Rectangle</b> cells.
	/// </summary>
	public static readonly Cell[][] UniqueRectanglePatterns =
	{
		new[] {  0,  3,  9, 12 }, new[] {  0,  4,  9, 13 }, new[] {  0,  5,  9, 14 }, new[] {  0,  6,  9, 15 }, new[] {  0,  7,  9, 16 }, new[] {  0,  8,  9, 17 }, new[] {  1,  3, 10, 12 }, new[] {  1,  4, 10, 13 }, new[] {  1,  5, 10, 14 },
		new[] {  1,  6, 10, 15 }, new[] {  1,  7, 10, 16 }, new[] {  1,  8, 10, 17 }, new[] {  2,  3, 11, 12 }, new[] {  2,  4, 11, 13 }, new[] {  2,  5, 11, 14 }, new[] {  2,  6, 11, 15 }, new[] {  2,  7, 11, 16 }, new[] {  2,  8, 11, 17 },
		new[] {  3,  6, 12, 15 }, new[] {  3,  7, 12, 16 }, new[] {  3,  8, 12, 17 }, new[] {  4,  6, 13, 15 }, new[] {  4,  7, 13, 16 }, new[] {  4,  8, 13, 17 }, new[] {  5,  6, 14, 15 }, new[] {  5,  7, 14, 16 }, new[] {  5,  8, 14, 17 },
		new[] {  0,  3, 18, 21 }, new[] {  0,  4, 18, 22 }, new[] {  0,  5, 18, 23 }, new[] {  0,  6, 18, 24 }, new[] {  0,  7, 18, 25 }, new[] {  0,  8, 18, 26 }, new[] {  1,  3, 19, 21 }, new[] {  1,  4, 19, 22 }, new[] {  1,  5, 19, 23 },
		new[] {  1,  6, 19, 24 }, new[] {  1,  7, 19, 25 }, new[] {  1,  8, 19, 26 }, new[] {  2,  3, 20, 21 }, new[] {  2,  4, 20, 22 }, new[] {  2,  5, 20, 23 }, new[] {  2,  6, 20, 24 }, new[] {  2,  7, 20, 25 }, new[] {  2,  8, 20, 26 },
		new[] {  3,  6, 21, 24 }, new[] {  3,  7, 21, 25 }, new[] {  3,  8, 21, 26 }, new[] {  4,  6, 22, 24 }, new[] {  4,  7, 22, 25 }, new[] {  4,  8, 22, 26 }, new[] {  5,  6, 23, 24 }, new[] {  5,  7, 23, 25 }, new[] {  5,  8, 23, 26 },
		new[] {  9, 12, 18, 21 }, new[] {  9, 13, 18, 22 }, new[] {  9, 14, 18, 23 }, new[] {  9, 15, 18, 24 }, new[] {  9, 16, 18, 25 }, new[] {  9, 17, 18, 26 }, new[] { 10, 12, 19, 21 }, new[] { 10, 13, 19, 22 }, new[] { 10, 14, 19, 23 },
		new[] { 10, 15, 19, 24 }, new[] { 10, 16, 19, 25 }, new[] { 10, 17, 19, 26 }, new[] { 11, 12, 20, 21 }, new[] { 11, 13, 20, 22 }, new[] { 11, 14, 20, 23 }, new[] { 11, 15, 20, 24 }, new[] { 11, 16, 20, 25 }, new[] { 11, 17, 20, 26 },
		new[] { 12, 15, 21, 24 }, new[] { 12, 16, 21, 25 }, new[] { 12, 17, 21, 26 }, new[] { 13, 15, 22, 24 }, new[] { 13, 16, 22, 25 }, new[] { 13, 17, 22, 26 }, new[] { 14, 15, 23, 24 }, new[] { 14, 16, 23, 25 }, new[] { 14, 17, 23, 26 },
		new[] { 27, 30, 36, 39 }, new[] { 27, 31, 36, 40 }, new[] { 27, 32, 36, 41 }, new[] { 27, 33, 36, 42 }, new[] { 27, 34, 36, 43 }, new[] { 27, 35, 36, 44 }, new[] { 28, 30, 37, 39 }, new[] { 28, 31, 37, 40 }, new[] { 28, 32, 37, 41 },
		new[] { 28, 33, 37, 42 }, new[] { 28, 34, 37, 43 }, new[] { 28, 35, 37, 44 }, new[] { 29, 30, 38, 39 }, new[] { 29, 31, 38, 40 }, new[] { 29, 32, 38, 41 }, new[] { 29, 33, 38, 42 }, new[] { 29, 34, 38, 43 }, new[] { 29, 35, 38, 44 },
		new[] { 30, 33, 39, 42 }, new[] { 30, 34, 39, 43 }, new[] { 30, 35, 39, 44 }, new[] { 31, 33, 40, 42 }, new[] { 31, 34, 40, 43 }, new[] { 31, 35, 40, 44 }, new[] { 32, 33, 41, 42 }, new[] { 32, 34, 41, 43 }, new[] { 32, 35, 41, 44 },
		new[] { 27, 30, 45, 48 }, new[] { 27, 31, 45, 49 }, new[] { 27, 32, 45, 50 }, new[] { 27, 33, 45, 51 }, new[] { 27, 34, 45, 52 }, new[] { 27, 35, 45, 53 }, new[] { 28, 30, 46, 48 }, new[] { 28, 31, 46, 49 }, new[] { 28, 32, 46, 50 },
		new[] { 28, 33, 46, 51 }, new[] { 28, 34, 46, 52 }, new[] { 28, 35, 46, 53 }, new[] { 29, 30, 47, 48 }, new[] { 29, 31, 47, 49 }, new[] { 29, 32, 47, 50 }, new[] { 29, 33, 47, 51 }, new[] { 29, 34, 47, 52 }, new[] { 29, 35, 47, 53 },
		new[] { 30, 33, 48, 51 }, new[] { 30, 34, 48, 52 }, new[] { 30, 35, 48, 53 }, new[] { 31, 33, 49, 51 }, new[] { 31, 34, 49, 52 }, new[] { 31, 35, 49, 53 }, new[] { 32, 33, 50, 51 }, new[] { 32, 34, 50, 52 }, new[] { 32, 35, 50, 53 },
		new[] { 36, 39, 45, 48 }, new[] { 36, 40, 45, 49 }, new[] { 36, 41, 45, 50 }, new[] { 36, 42, 45, 51 }, new[] { 36, 43, 45, 52 }, new[] { 36, 44, 45, 53 }, new[] { 37, 39, 46, 48 }, new[] { 37, 40, 46, 49 }, new[] { 37, 41, 46, 50 },
		new[] { 37, 42, 46, 51 }, new[] { 37, 43, 46, 52 }, new[] { 37, 44, 46, 53 }, new[] { 38, 39, 47, 48 }, new[] { 38, 40, 47, 49 }, new[] { 38, 41, 47, 50 }, new[] { 38, 42, 47, 51 }, new[] { 38, 43, 47, 52 }, new[] { 38, 44, 47, 53 },
		new[] { 39, 42, 48, 51 }, new[] { 39, 43, 48, 52 }, new[] { 39, 44, 48, 53 }, new[] { 40, 42, 49, 51 }, new[] { 40, 43, 49, 52 }, new[] { 40, 44, 49, 53 }, new[] { 41, 42, 50, 51 }, new[] { 41, 43, 50, 52 }, new[] { 41, 44, 50, 53 },
		new[] { 54, 57, 63, 66 }, new[] { 54, 58, 63, 67 }, new[] { 54, 59, 63, 68 }, new[] { 54, 60, 63, 69 }, new[] { 54, 61, 63, 70 }, new[] { 54, 62, 63, 71 }, new[] { 55, 57, 64, 66 }, new[] { 55, 58, 64, 67 }, new[] { 55, 59, 64, 68 },
		new[] { 55, 60, 64, 69 }, new[] { 55, 61, 64, 70 }, new[] { 55, 62, 64, 71 }, new[] { 56, 57, 65, 66 }, new[] { 56, 58, 65, 67 }, new[] { 56, 59, 65, 68 }, new[] { 56, 60, 65, 69 }, new[] { 56, 61, 65, 70 }, new[] { 56, 62, 65, 71 },
		new[] { 57, 60, 66, 69 }, new[] { 57, 61, 66, 70 }, new[] { 57, 62, 66, 71 }, new[] { 58, 60, 67, 69 }, new[] { 58, 61, 67, 70 }, new[] { 58, 62, 67, 71 }, new[] { 59, 60, 68, 69 }, new[] { 59, 61, 68, 70 }, new[] { 59, 62, 68, 71 },
		new[] { 54, 57, 72, 75 }, new[] { 54, 58, 72, 76 }, new[] { 54, 59, 72, 77 }, new[] { 54, 60, 72, 78 }, new[] { 54, 61, 72, 79 }, new[] { 54, 62, 72, 80 }, new[] { 55, 57, 73, 75 }, new[] { 55, 58, 73, 76 }, new[] { 55, 59, 73, 77 },
		new[] { 55, 60, 73, 78 }, new[] { 55, 61, 73, 79 }, new[] { 55, 62, 73, 80 }, new[] { 56, 57, 74, 75 }, new[] { 56, 58, 74, 76 }, new[] { 56, 59, 74, 77 }, new[] { 56, 60, 74, 78 }, new[] { 56, 61, 74, 79 }, new[] { 56, 62, 74, 80 },
		new[] { 57, 60, 75, 78 }, new[] { 57, 61, 75, 79 }, new[] { 57, 62, 75, 80 }, new[] { 58, 60, 76, 78 }, new[] { 58, 61, 76, 79 }, new[] { 58, 62, 76, 80 }, new[] { 59, 60, 77, 78 }, new[] { 59, 61, 77, 79 }, new[] { 59, 62, 77, 80 },
		new[] { 63, 66, 72, 75 }, new[] { 63, 67, 72, 76 }, new[] { 63, 68, 72, 77 }, new[] { 63, 69, 72, 78 }, new[] { 63, 70, 72, 79 }, new[] { 63, 71, 72, 80 }, new[] { 64, 66, 73, 75 }, new[] { 64, 67, 73, 76 }, new[] { 64, 68, 73, 77 },
		new[] { 64, 69, 73, 78 }, new[] { 64, 70, 73, 79 }, new[] { 64, 71, 73, 80 }, new[] { 65, 66, 74, 75 }, new[] { 65, 67, 74, 76 }, new[] { 65, 68, 74, 77 }, new[] { 65, 69, 74, 78 }, new[] { 65, 70, 74, 79 }, new[] { 65, 71, 74, 80 },
		new[] { 66, 69, 75, 78 }, new[] { 66, 70, 75, 79 }, new[] { 66, 71, 75, 80 }, new[] { 67, 69, 76, 78 }, new[] { 67, 70, 76, 79 }, new[] { 67, 71, 76, 80 }, new[] { 68, 69, 77, 78 }, new[] { 68, 70, 77, 79 }, new[] { 68, 71, 77, 80 },
		new[] {  0,  1, 27, 28 }, new[] {  0,  1, 36, 37 }, new[] {  0,  1, 45, 46 }, new[] {  0,  1, 54, 55 }, new[] {  0,  1, 63, 64 }, new[] {  0,  1, 72, 73 }, new[] {  9, 10, 27, 28 }, new[] {  9, 10, 36, 37 }, new[] {  9, 10, 45, 46 },
		new[] {  9, 10, 54, 55 }, new[] {  9, 10, 63, 64 }, new[] {  9, 10, 72, 73 }, new[] { 18, 19, 27, 28 }, new[] { 18, 19, 36, 37 }, new[] { 18, 19, 45, 46 }, new[] { 18, 19, 54, 55 }, new[] { 18, 19, 63, 64 }, new[] { 18, 19, 72, 73 },
		new[] { 27, 28, 54, 55 }, new[] { 27, 28, 63, 64 }, new[] { 27, 28, 72, 73 }, new[] { 36, 37, 54, 55 }, new[] { 36, 37, 63, 64 }, new[] { 36, 37, 72, 73 }, new[] { 45, 46, 54, 55 }, new[] { 45, 46, 63, 64 }, new[] { 45, 46, 72, 73 },
		new[] {  0,  2, 27, 29 }, new[] {  0,  2, 36, 38 }, new[] {  0,  2, 45, 47 }, new[] {  0,  2, 54, 56 }, new[] {  0,  2, 63, 65 }, new[] {  0,  2, 72, 74 }, new[] {  9, 11, 27, 29 }, new[] {  9, 11, 36, 38 }, new[] {  9, 11, 45, 47 },
		new[] {  9, 11, 54, 56 }, new[] {  9, 11, 63, 65 }, new[] {  9, 11, 72, 74 }, new[] { 18, 20, 27, 29 }, new[] { 18, 20, 36, 38 }, new[] { 18, 20, 45, 47 }, new[] { 18, 20, 54, 56 }, new[] { 18, 20, 63, 65 }, new[] { 18, 20, 72, 74 },
		new[] { 27, 29, 54, 56 }, new[] { 27, 29, 63, 65 }, new[] { 27, 29, 72, 74 }, new[] { 36, 38, 54, 56 }, new[] { 36, 38, 63, 65 }, new[] { 36, 38, 72, 74 }, new[] { 45, 47, 54, 56 }, new[] { 45, 47, 63, 65 }, new[] { 45, 47, 72, 74 },
		new[] {  1,  2, 28, 29 }, new[] {  1,  2, 37, 38 }, new[] {  1,  2, 46, 47 }, new[] {  1,  2, 55, 56 }, new[] {  1,  2, 64, 65 }, new[] {  1,  2, 73, 74 }, new[] { 10, 11, 28, 29 }, new[] { 10, 11, 37, 38 }, new[] { 10, 11, 46, 47 },
		new[] { 10, 11, 55, 56 }, new[] { 10, 11, 64, 65 }, new[] { 10, 11, 73, 74 }, new[] { 19, 20, 28, 29 }, new[] { 19, 20, 37, 38 }, new[] { 19, 20, 46, 47 }, new[] { 19, 20, 55, 56 }, new[] { 19, 20, 64, 65 }, new[] { 19, 20, 73, 74 },
		new[] { 28, 29, 55, 56 }, new[] { 28, 29, 64, 65 }, new[] { 28, 29, 73, 74 }, new[] { 37, 38, 55, 56 }, new[] { 37, 38, 64, 65 }, new[] { 37, 38, 73, 74 }, new[] { 46, 47, 55, 56 }, new[] { 46, 47, 64, 65 }, new[] { 46, 47, 73, 74 },
		new[] {  3,  4, 30, 31 }, new[] {  3,  4, 39, 40 }, new[] {  3,  4, 48, 49 }, new[] {  3,  4, 57, 58 }, new[] {  3,  4, 66, 67 }, new[] {  3,  4, 75, 76 }, new[] { 12, 13, 30, 31 }, new[] { 12, 13, 39, 40 }, new[] { 12, 13, 48, 49 },
		new[] { 12, 13, 57, 58 }, new[] { 12, 13, 66, 67 }, new[] { 12, 13, 75, 76 }, new[] { 21, 22, 30, 31 }, new[] { 21, 22, 39, 40 }, new[] { 21, 22, 48, 49 }, new[] { 21, 22, 57, 58 }, new[] { 21, 22, 66, 67 }, new[] { 21, 22, 75, 76 },
		new[] { 30, 31, 57, 58 }, new[] { 30, 31, 66, 67 }, new[] { 30, 31, 75, 76 }, new[] { 39, 40, 57, 58 }, new[] { 39, 40, 66, 67 }, new[] { 39, 40, 75, 76 }, new[] { 48, 49, 57, 58 }, new[] { 48, 49, 66, 67 }, new[] { 48, 49, 75, 76 },
		new[] {  3,  5, 30, 32 }, new[] {  3,  5, 39, 41 }, new[] {  3,  5, 48, 50 }, new[] {  3,  5, 57, 59 }, new[] {  3,  5, 66, 68 }, new[] {  3,  5, 75, 77 }, new[] { 12, 14, 30, 32 }, new[] { 12, 14, 39, 41 }, new[] { 12, 14, 48, 50 },
		new[] { 12, 14, 57, 59 }, new[] { 12, 14, 66, 68 }, new[] { 12, 14, 75, 77 }, new[] { 21, 23, 30, 32 }, new[] { 21, 23, 39, 41 }, new[] { 21, 23, 48, 50 }, new[] { 21, 23, 57, 59 }, new[] { 21, 23, 66, 68 }, new[] { 21, 23, 75, 77 },
		new[] { 30, 32, 57, 59 }, new[] { 30, 32, 66, 68 }, new[] { 30, 32, 75, 77 }, new[] { 39, 41, 57, 59 }, new[] { 39, 41, 66, 68 }, new[] { 39, 41, 75, 77 }, new[] { 48, 50, 57, 59 }, new[] { 48, 50, 66, 68 }, new[] { 48, 50, 75, 77 },
		new[] {  4,  5, 31, 32 }, new[] {  4,  5, 40, 41 }, new[] {  4,  5, 49, 50 }, new[] {  4,  5, 58, 59 }, new[] {  4,  5, 67, 68 }, new[] {  4,  5, 76, 77 }, new[] { 13, 14, 31, 32 }, new[] { 13, 14, 40, 41 }, new[] { 13, 14, 49, 50 },
		new[] { 13, 14, 58, 59 }, new[] { 13, 14, 67, 68 }, new[] { 13, 14, 76, 77 }, new[] { 22, 23, 31, 32 }, new[] { 22, 23, 40, 41 }, new[] { 22, 23, 49, 50 }, new[] { 22, 23, 58, 59 }, new[] { 22, 23, 67, 68 }, new[] { 22, 23, 76, 77 },
		new[] { 31, 32, 58, 59 }, new[] { 31, 32, 67, 68 }, new[] { 31, 32, 76, 77 }, new[] { 40, 41, 58, 59 }, new[] { 40, 41, 67, 68 }, new[] { 40, 41, 76, 77 }, new[] { 49, 50, 58, 59 }, new[] { 49, 50, 67, 68 }, new[] { 49, 50, 76, 77 },
		new[] {  6,  7, 33, 34 }, new[] {  6,  7, 42, 43 }, new[] {  6,  7, 51, 52 }, new[] {  6,  7, 60, 61 }, new[] {  6,  7, 69, 70 }, new[] {  6,  7, 78, 79 }, new[] { 15, 16, 33, 34 }, new[] { 15, 16, 42, 43 }, new[] { 15, 16, 51, 52 },
		new[] { 15, 16, 60, 61 }, new[] { 15, 16, 69, 70 }, new[] { 15, 16, 78, 79 }, new[] { 24, 25, 33, 34 }, new[] { 24, 25, 42, 43 }, new[] { 24, 25, 51, 52 }, new[] { 24, 25, 60, 61 }, new[] { 24, 25, 69, 70 }, new[] { 24, 25, 78, 79 },
		new[] { 33, 34, 60, 61 }, new[] { 33, 34, 69, 70 }, new[] { 33, 34, 78, 79 }, new[] { 42, 43, 60, 61 }, new[] { 42, 43, 69, 70 }, new[] { 42, 43, 78, 79 }, new[] { 51, 52, 60, 61 }, new[] { 51, 52, 69, 70 }, new[] { 51, 52, 78, 79 },
		new[] {  6,  8, 33, 35 }, new[] {  6,  8, 42, 44 }, new[] {  6,  8, 51, 53 }, new[] {  6,  8, 60, 62 }, new[] {  6,  8, 69, 71 }, new[] {  6,  8, 78, 80 }, new[] { 15, 17, 33, 35 }, new[] { 15, 17, 42, 44 }, new[] { 15, 17, 51, 53 },
		new[] { 15, 17, 60, 62 }, new[] { 15, 17, 69, 71 }, new[] { 15, 17, 78, 80 }, new[] { 24, 26, 33, 35 }, new[] { 24, 26, 42, 44 }, new[] { 24, 26, 51, 53 }, new[] { 24, 26, 60, 62 }, new[] { 24, 26, 69, 71 }, new[] { 24, 26, 78, 80 },
		new[] { 33, 35, 60, 62 }, new[] { 33, 35, 69, 71 }, new[] { 33, 35, 78, 80 }, new[] { 42, 44, 60, 62 }, new[] { 42, 44, 69, 71 }, new[] { 42, 44, 78, 80 }, new[] { 51, 53, 60, 62 }, new[] { 51, 53, 69, 71 }, new[] { 51, 53, 78, 80 },
		new[] {  7,  8, 34, 35 }, new[] {  7,  8, 43, 44 }, new[] {  7,  8, 52, 53 }, new[] {  7,  8, 61, 62 }, new[] {  7,  8, 70, 71 }, new[] {  7,  8, 79, 80 }, new[] { 16, 17, 34, 35 }, new[] { 16, 17, 43, 44 }, new[] { 16, 17, 52, 53 },
		new[] { 16, 17, 61, 62 }, new[] { 16, 17, 70, 71 }, new[] { 16, 17, 79, 80 }, new[] { 25, 26, 34, 35 }, new[] { 25, 26, 43, 44 }, new[] { 25, 26, 52, 53 }, new[] { 25, 26, 61, 62 }, new[] { 25, 26, 70, 71 }, new[] { 25, 26, 79, 80 },
		new[] { 34, 35, 61, 62 }, new[] { 34, 35, 70, 71 }, new[] { 34, 35, 79, 80 }, new[] { 43, 44, 61, 62 }, new[] { 43, 44, 70, 71 }, new[] { 43, 44, 79, 80 }, new[] { 52, 53, 61, 62 }, new[] { 52, 53, 70, 71 }, new[] { 52, 53, 79, 80 }
	};


	/// <summary>
	/// Check preconditions.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is searching for ARs.</param>
	/// <returns>Indicates whether the UR is passed to check.</returns>
	public static bool CheckPreconditions(scoped in Grid grid, Cell[] urCells, bool arMode)
	{
		var emptyCountWhenArMode = (byte)0;
		var modifiableCount = (byte)0;
		foreach (var urCell in urCells)
		{
			switch (grid.GetStatus(urCell))
			{
				case CellStatus.Given:
				case CellStatus.Modifiable when !arMode:
				{
					return false;
				}
				case CellStatus.Empty when arMode:
				{
					emptyCountWhenArMode++;
					break;
				}
				case CellStatus.Modifiable:
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
	public static bool CheckPreconditionsOnIncomplete(scoped in Grid grid, Cell[] urCells, Digit d1, Digit d2)
	{
		// Same-sided cells cannot contain only one digit of two digits 'd1' and 'd2'.
		foreach (var (a, b) in stackalloc[] { (0, 1), (2, 3), (0, 2), (1, 3) })
		{
			var mask1 = grid.GetCandidates(urCells[a]);
			var mask2 = grid.GetCandidates(urCells[b]);
			var gatheredMask = (Mask)(mask1 | mask2);
			var intersectedMask = (Mask)(mask1 & mask2);
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
	public static bool IsConjugatePair(Digit digit, scoped in CellMap map, House houseIndex)
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
	public static bool IsSameHouseCell(Cell cell1, Cell cell2, out HouseMask houses)
	{
		var v = (CellsMap[cell1] + cell2).CoveredHouses;
		(var r, houses) = v != 0 ? (true, v) : (false, 0);
		return r;
	}

	/// <summary>
	/// Check whether the highlight UR candidates is incomplete.
	/// </summary>
	/// <param name="allowIncomplete"><inheritdoc cref="UniqueRectangleStepSearcher.AllowIncompleteUniqueRectangles" path="/summary"/></param>
	/// <param name="list">The list to check.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	/// <remarks>
	/// This method uses a trick to check a UR structure: to count up the number of "Normal colored"
	/// candidates used in the current UR structure. If and only if the full structure uses 8 candidates
	/// colored with normal one, the structure will be complete.
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsIncomplete(bool allowIncomplete, List<CandidateViewNode> list)
		=> allowIncomplete && list.Count(static d => d.Identifier is WellKnownColorIdentifier { Kind: WellKnownColorIdentifierKind.Normal }) != 8;

	/// <summary>
	/// Get a cell that can't see each other.
	/// </summary>
	/// <param name="urCells">The UR cells.</param>
	/// <param name="cell">The current cell.</param>
	/// <returns>The diagonal cell.</returns>
	/// <exception cref="ArgumentException">
	/// Throws when the specified argument <paramref name="cell"/> is invalid.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Cell GetDiagonalCell(Cell[] urCells, Cell cell)
		=> cell == urCells[0] ? urCells[3] : cell == urCells[1] ? urCells[2] : cell == urCells[2] ? urCells[1] : urCells[0];

	/// <summary>
	/// Get all highlight cells.
	/// </summary>
	/// <param name="urCells">The all UR cells used.</param>
	/// <returns>The list of highlight cells.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellViewNode[] GetHighlightCells(Cell[] urCells)
		=> new CellViewNode[]
		{
			new(WellKnownColorIdentifier.Normal, urCells[0]),
			new(WellKnownColorIdentifier.Normal, urCells[1]),
			new(WellKnownColorIdentifier.Normal, urCells[2]),
			new(WellKnownColorIdentifier.Normal, urCells[3])
		};
}
