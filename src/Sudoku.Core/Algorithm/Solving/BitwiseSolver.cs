#pragma warning disable IDE0011, IDE0032
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Sudoku.Concepts;

namespace Sudoku.Algorithm.Solving;

/// <summary>
/// Indicates the solver that is able to solve a sudoku puzzle, and then get the solution of that sudoku.
/// </summary>
/// <remarks>
/// The reason why the type name contains the word <i>bitwise</i> is that the solver uses the bitwise algorithm
/// to handle a sudoku grid, which is more efficient.
/// </remarks>
public sealed unsafe class BitwiseSolver : ISolver
{
	/// <summary>
	/// The buffer length of a solution puzzle.
	/// </summary>
	private const int BufferLength = 82;

	/// <summary>
	/// All pencil marks set - 27 bits per band.
	/// </summary>
	private const int BitSet27 = 0x7FFFFFF;


#pragma warning disable format
	private static readonly byte[]
		CellToRow = [
			0, 0, 0, 0, 0, 0, 0, 0, 0,
			1, 1, 1, 1, 1, 1, 1, 1, 1,
			2, 2, 2, 2, 2, 2, 2, 2, 2,
			3, 3, 3, 3, 3, 3, 3, 3, 3,
			4, 4, 4, 4, 4, 4, 4, 4, 4,
			5, 5, 5, 5, 5, 5, 5, 5, 5,
			6, 6, 6, 6, 6, 6, 6, 6, 6,
			7, 7, 7, 7, 7, 7, 7, 7, 7,
			8, 8, 8, 8, 8, 8, 8, 8, 8
		],
		Cell2Floor = [
			0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0,
			1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1,
			2, 2, 2, 2, 2, 2, 2, 2, 2,
			2, 2, 2, 2, 2, 2, 2, 2, 2,
			2, 2, 2, 2, 2, 2, 2, 2, 2
		],
		ShrinkMaskTable = [
			0, 1, 1, 1, 1, 1, 1, 1, 2, 3, 3, 3, 3, 3, 3, 3, 2, 3, 3, 3, 3, 3, 3, 3, 2, 3, 3, 3, 3, 3, 3, 3,
			2, 3, 3, 3, 3, 3, 3, 3, 2, 3, 3, 3, 3, 3, 3, 3, 2, 3, 3, 3, 3, 3, 3, 3, 2, 3, 3, 3, 3, 3, 3, 3,
			4, 5, 5, 5, 5, 5, 5, 5, 6, 7, 7, 7, 7, 7, 7, 7, 6, 7, 7, 7, 7, 7, 7, 7, 6, 7, 7, 7, 7, 7, 7, 7,
			6, 7, 7, 7, 7, 7, 7, 7, 6, 7, 7, 7, 7, 7, 7, 7, 6, 7, 7, 7, 7, 7, 7, 7, 6, 7, 7, 7, 7, 7, 7, 7,
			4, 5, 5, 5, 5, 5, 5, 5, 6, 7, 7, 7, 7, 7, 7, 7, 6, 7, 7, 7, 7, 7, 7, 7, 6, 7, 7, 7, 7, 7, 7, 7,
			6, 7, 7, 7, 7, 7, 7, 7, 6, 7, 7, 7, 7, 7, 7, 7, 6, 7, 7, 7, 7, 7, 7, 7, 6, 7, 7, 7, 7, 7, 7, 7,
			4, 5, 5, 5, 5, 5, 5, 5, 6, 7, 7, 7, 7, 7, 7, 7, 6, 7, 7, 7, 7, 7, 7, 7, 6, 7, 7, 7, 7, 7, 7, 7,
			6, 7, 7, 7, 7, 7, 7, 7, 6, 7, 7, 7, 7, 7, 7, 7, 6, 7, 7, 7, 7, 7, 7, 7, 6, 7, 7, 7, 7, 7, 7, 7,
			4, 5, 5, 5, 5, 5, 5, 5, 6, 7, 7, 7, 7, 7, 7, 7, 6, 7, 7, 7, 7, 7, 7, 7, 6, 7, 7, 7, 7, 7, 7, 7,
			6, 7, 7, 7, 7, 7, 7, 7, 6, 7, 7, 7, 7, 7, 7, 7, 6, 7, 7, 7, 7, 7, 7, 7, 6, 7, 7, 7, 7, 7, 7, 7,
			4, 5, 5, 5, 5, 5, 5, 5, 6, 7, 7, 7, 7, 7, 7, 7, 6, 7, 7, 7, 7, 7, 7, 7, 6, 7, 7, 7, 7, 7, 7, 7,
			6, 7, 7, 7, 7, 7, 7, 7, 6, 7, 7, 7, 7, 7, 7, 7, 6, 7, 7, 7, 7, 7, 7, 7, 6, 7, 7, 7, 7, 7, 7, 7,
			4, 5, 5, 5, 5, 5, 5, 5, 6, 7, 7, 7, 7, 7, 7, 7, 6, 7, 7, 7, 7, 7, 7, 7, 6, 7, 7, 7, 7, 7, 7, 7,
			6, 7, 7, 7, 7, 7, 7, 7, 6, 7, 7, 7, 7, 7, 7, 7, 6, 7, 7, 7, 7, 7, 7, 7, 6, 7, 7, 7, 7, 7, 7, 7,
			4, 5, 5, 5, 5, 5, 5, 5, 6, 7, 7, 7, 7, 7, 7, 7, 6, 7, 7, 7, 7, 7, 7, 7, 6, 7, 7, 7, 7, 7, 7, 7,
			6, 7, 7, 7, 7, 7, 7, 7, 6, 7, 7, 7, 7, 7, 7, 7, 6, 7, 7, 7, 7, 7, 7, 7, 6, 7, 7, 7, 7, 7, 7, 7
		],
		RowUniqueTable = [
			7, 6, 6, 6, 6, 6, 6, 6, 5, 4, 4, 4, 4, 4, 4, 4, 5, 4, 4, 4, 4, 4, 4, 4, 5, 4, 4, 4, 4, 4, 4, 4,
			5, 4, 4, 4, 4, 4, 4, 4, 5, 4, 4, 4, 4, 4, 4, 4, 5, 4, 4, 4, 4, 4, 4, 4, 5, 4, 4, 4, 4, 4, 4, 4,
			3, 2, 2, 2, 2, 2, 2, 2, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0,
			1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0,
			3, 2, 2, 2, 2, 2, 2, 2, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0,
			1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0,
			3, 2, 2, 2, 2, 2, 2, 2, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0,
			1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0,
			3, 2, 2, 2, 2, 2, 2, 2, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0,
			1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0,
			3, 2, 2, 2, 2, 2, 2, 2, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0,
			1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0,
			3, 2, 2, 2, 2, 2, 2, 2, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0,
			1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0,
			3, 2, 2, 2, 2, 2, 2, 2, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0,
			1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0
		],
		Digit2BaseBand = [0, 3, 6, 9, 12, 15, 18, 21, 24],
		Another1Table = [
			0x1, 0x0, 0x0, 0x4, 0x3, 0x3, 0x7, 0x6, 0x6, 0xA, 0x9, 0x9, 0xD, 0xC, 0xC, 0x10,
			0xF, 0xF, 0x13, 0x12, 0x12, 0x16, 0x15, 0x15, 0x19, 0x18, 0x18
		],
		Another2Table = [
			0x2, 0x2, 0x1, 0x5, 0x5, 0x4, 0x8, 0x8, 0x7, 0xB, 0xB, 0xA, 0xE, 0xE, 0xD, 0x11,
			0x11, 0x10, 0x14, 0x14, 0x13, 0x17, 0x17, 0x16, 0x1A, 0x1A, 0x19
		],
		Mod3 = [0, 1, 2, 0, 1, 2, 0, 1, 2, 0, 1, 2, 0, 1, 2, 0, 1, 2, 0, 1, 2, 0, 1, 2, 0, 1, 2],
		Mod27 = [
			0, 1, 2, 3, 4, 5, 6, 7, 8,
			9, 10, 11, 12, 13, 14, 15, 16, 17,
			18, 19, 20, 21, 22, 23, 24, 25, 26,
			0, 1, 2, 3, 4, 5, 6, 7, 8,
			9, 10, 11, 12, 13, 14, 15, 16, 17,
			18, 19, 20, 21, 22, 23, 24, 25, 26,
			0, 1, 2, 3, 4, 5, 6, 7, 8,
			9, 10, 11, 12, 13, 14, 15, 16, 17,
			18, 19, 20, 21, 22, 23, 24, 25, 26
		],
		MultiplyDeBruijnBitPosition32 = [
			0, 1, 28, 2, 29, 14, 24, 3, 30, 22, 20, 15, 25, 17, 4, 8,
			31, 27, 13, 23, 21, 19, 16, 7, 26, 12, 18, 6, 11, 5, 10, 9
		];

	private static readonly int[]
		Cell2Mask = [
			0x1, 0x2, 0x4, 0x8, 0x10, 0x20, 0x40, 0x80, 0x100,
			0x200, 0x400, 0x800, 0x1000, 0x2000, 0x4000, 0x8000, 0x10000, 0x20000,
			0x40000, 0x80000, 0x100000, 0x200000, 0x400000, 0x800000, 0x1000000, 0x2000000, 0x4000000,
			0x1, 0x2, 0x4, 0x8, 0x10, 0x20, 0x40, 0x80, 0x100,
			0x200, 0x400, 0x800, 0x1000, 0x2000, 0x4000, 0x8000, 0x10000, 0x20000,
			0x40000, 0x80000, 0x100000, 0x200000, 0x400000, 0x800000, 0x1000000, 0x2000000, 0x4000000,
			0x1, 0x2, 0x4, 0x8, 0x10, 0x20, 0x40, 0x80, 0x100,
			0x200, 0x400, 0x800, 0x1000, 0x2000, 0x4000, 0x8000, 0x10000, 0x20000,
			0x40000, 0x80000, 0x100000, 0x200000, 0x400000, 0x800000, 0x1000000, 0x2000000, 0x4000000
		],
		ComplexMaskTable = [
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0x381C71C0, 0x3F1C71C0, 0x381FF1C0, 0x3F1FF1C0,
			0, 0, 0, 0, 0x38FC71C0, 0x3FFC71C0, 0x3FFFF1C0, 0x3FFFF1C0,
			0, 0, 0x381F8038, 0x38FF8038, 0, 0, 0x381FF038, 0x38FFF038,
			0, 0, 0x3F1F8038, 0x3FFF8038, 0, 0, 0x3FFFF038, 0x3FFFF038,
			0, 0, 0x381F81F8, 0x3FFF81F8, 0x381C71F8, 0x3FFC71F8, 0x381FF1F8, 0x3FFFF1F8,
			0, 0, 0x3F1F81F8, 0x3FFF81F8, 0x38FC71F8, 0x3FFC71F8, 0x3FFFF1F8, 0x3FFFF1F8,
			0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0x38E00FC0, 0x38E38FC0, 0x3FE00FC0, 0x3FE38FC0,
			0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0x38FC0FC0, 0x3FFF8FC0, 0x3FFC0FC0, 0x3FFF8FC0,
			0, 0x38E38007, 0, 0x38FF8007, 0, 0x38E38E07, 0, 0x38FF8E07,
			0, 0x38E381C7, 0, 0x3FFF81C7, 0x38E00FC7, 0x38E38FC7, 0x3FFC0FC7, 0x3FFF8FC7,
			0, 0x3FE38007, 0, 0x3FFF8007, 0, 0x3FFF8E07, 0, 0x3FFF8E07,
			0, 0x3FE381C7, 0, 0x3FFF81C7, 0x38FC0FC7, 0x3FFF8FC7, 0x3FFC0FC7, 0x3FFF8FC7,
			0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0x38E07FC0, 0x38E3FFC0, 0x3FE3FFC0, 0x3FE3FFC0,
			0, 0, 0, 0, 0x381C7FC0, 0x3F1FFFC0, 0x381FFFC0, 0x3F1FFFC0,
			0, 0, 0, 0, 0x38FC7FC0, 0x3FFFFFC0, 0x3FFFFFC0, 0x3FFFFFC0,
			0, 0x38E3803F, 0x381F803F, 0x38FF803F, 0, 0x38E3FE3F, 0x381FFE3F, 0x38FFFE3F,
			0, 0x38E381FF, 0x3F1F81FF, 0x3FFF81FF, 0x38E07FFF, 0x38E3FFFF, 0x3FFFFFFF, 0x3FFFFFFF,
			0, 0x3FE381FF, 0x381F81FF, 0x3FFF81FF, 0x381C7FFF, 0x3FFFFFFF, 0x381FFFFF, 0x3FFFFFFF,
			0, 0x3FE381FF, 0x3F1F81FF, 0x3FFF81FF, 0x38FC7FFF, 0x3FFFFFFF, 0x3FFFFFFF, 0x3FFFFFFF,
			0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0x3F000E38, 0x3F007E38, 0, 0, 0x3FE00E38, 0x3FE07E38,
			0, 0x3F007007, 0, 0x3F007E07, 0, 0x3F1C7007, 0, 0x3F1C7E07,
			0, 0x3F00703F, 0x3F000E3F, 0x3F007E3F, 0, 0x3FFC703F, 0x3FFC0E3F, 0x3FFC7E3F,
			0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0x3F1C0E38, 0x3FFC7E38, 0, 0, 0x3FFC0E38, 0x3FFC7E38,
			0, 0x3FE07007, 0, 0x3FFC7E07, 0, 0x3FFC7007, 0, 0x3FFC7E07,
			0, 0x3FE0703F, 0x3F1C0E3F, 0x3FFC7E3F, 0, 0x3FFC703F, 0x3FFC0E3F, 0x3FFC7E3F,
			0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0x3F038E38, 0x3F03FE38, 0, 0, 0x3FE3FE38, 0x3FE3FE38,
			0, 0x3F0071C7, 0, 0x3F03FFC7, 0x381C71C7, 0x3F1C71C7, 0x381FFFC7, 0x3F1FFFC7,
			0, 0x3F0071FF, 0x3F038FFF, 0x3F03FFFF, 0x38FC71FF, 0x3FFC71FF, 0x3FFFFFFF, 0x3FFFFFFF,
			0, 0, 0x381F8E38, 0x38FFFE38, 0, 0, 0x381FFE38, 0x38FFFE38,
			0, 0, 0x3F1F8E38, 0x3FFFFE38, 0, 0, 0x3FFFFE38, 0x3FFFFE38,
			0, 0x3FE071FF, 0x381F8FFF, 0x3FFFFFFF, 0x381C71FF, 0x3FFC71FF, 0x381FFFFF, 0x3FFFFFFF,
			0, 0x3FE071FF, 0x3F1F8FFF, 0x3FFFFFFF, 0x38FC71FF, 0x3FFC71FF, 0x3FFFFFFF, 0x3FFFFFFF,
			0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0x3F000FF8, 0x3F03FFF8, 0x38E00FF8, 0x38E3FFF8, 0x3FE00FF8, 0x3FE3FFF8,
			0, 0x3F03F007, 0, 0x3F03FE07, 0, 0x3F1FFE07, 0, 0x3F1FFE07,
			0, 0x3F03F1FF, 0x3F000FFF, 0x3F03FFFF, 0x38FC0FFF, 0x3FFFFFFF, 0x3FFC0FFF, 0x3FFFFFFF,
			0, 0x38E3F007, 0, 0x38FFFE07, 0, 0x38E3FE07, 0, 0x38FFFE07,
			0, 0x38E3F1FF, 0x3F1C0FFF, 0x3FFFFFFF, 0x38E00FFF, 0x38E3FFFF, 0x3FFC0FFF, 0x3FFFFFFF,
			0, 0x3FE3F007, 0, 0x3FFFFE07, 0, 0x3FFFFE07, 0, 0x3FFFFE07,
			0, 0x3FE3F1FF, 0x3F1C0FFF, 0x3FFFFFFF, 0x38FC0FFF, 0x3FFFFFFF, 0x3FFC0FFF, 0x3FFFFFFF,
			0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0x3F038FF8, 0x3F03FFF8, 0x38E07FF8, 0x38E3FFF8, 0x3FE3FFF8, 0x3FE3FFF8,
			0, 0x3F03F1C7, 0, 0x3F03FFC7, 0x381C7FC7, 0x3F1FFFC7, 0x381FFFC7, 0x3F1FFFC7,
			0, 0x3F03F1FF, 0x3F038FFF, 0x3F03FFFF, 0x38FC7FFF, 0x3FFFFFFF, 0x3FFFFFFF, 0x3FFFFFFF,
			0, 0x38E3F03F, 0x381F8E3F, 0x38FFFE3F, 0, 0x38E3FE3F, 0x381FFE3F, 0x38FFFE3F,
			0, 0x38E3F1FF, 0x3F1F8FFF, 0x3FFFFFFF, 0x38E07FFF, 0x38E3FFFF, 0x3FFFFFFF, 0x3FFFFFFF,
			0, 0x3FE3F1FF, 0x381F8FFF, 0x3FFFFFFF, 0x381C7FFF, 0x3FFFFFFF, 0x381FFFFF, 0x3FFFFFFF,
			0, 0x3FE3F1FF, 0x3F1F8FFF, 0x3FFFFFFF, 0x38FC7FFF, 0x3FFFFFFF, 0x3FFFFFFF, 0x3FFFFFFF
		],
		MaskSingleTable = [
			0x3FFFFFFF, 0x3FFBFDFE, 0x3FF7FBFD, 0x3FFFFFFF, 0x3FEFF7FB, 0x3FFFFFFF, 0x3FFFFFFF, 0x3FFFFFFF,
			0x3FDFEFF7, 0x3FDBEDF6, 0x3FD7EBF5, 0x3FDFEFF7, 0x3FCFE7F3, 0x3FDFEFF7, 0x3FDFEFF7, 0x3FDFEFF7,
			0x3FBFDFEF, 0x3FBBDDEE, 0x3FB7DBED, 0x3FBFDFEF, 0x3FAFD7EB, 0x3FBFDFEF, 0x3FBFDFEF, 0x3FBFDFEF,
			0x3FFFFFFF, 0x3FFBFDFE, 0x3FF7FBFD, 0x3FFFFFFF, 0x3FEFF7FB, 0x3FFFFFFF, 0x3FFFFFFF, 0x3FFFFFFF,
			0x3F7FBFDF, 0x3F7BBDDE, 0x3F77BBDD, 0x3F7FBFDF, 0x3F6FB7DB, 0x3F7FBFDF, 0x3F7FBFDF, 0x3F7FBFDF,
			0x3FFFFFFF, 0x3FFBFDFE, 0x3FF7FBFD, 0x3FFFFFFF, 0x3FEFF7FB, 0x3FFFFFFF, 0x3FFFFFFF, 0x3FFFFFFF,
			0x3FFFFFFF, 0x3FFBFDFE, 0x3FF7FBFD, 0x3FFFFFFF, 0x3FEFF7FB, 0x3FFFFFFF, 0x3FFFFFFF, 0x3FFFFFFF,
			0x3FFFFFFF, 0x3FFBFDFE, 0x3FF7FBFD, 0x3FFFFFFF, 0x3FEFF7FB, 0x3FFFFFFF, 0x3FFFFFFF, 0x3FFFFFFF,
			0x3EFF7FBF, 0x3EFB7DBE, 0x3EF77BBD, 0x3EFF7FBF, 0x3EEF77BB, 0x3EFF7FBF, 0x3EFF7FBF, 0x3EFF7FBF,
			0x3EDF6FB7, 0x3EDB6DB6, 0x3ED76BB5, 0x3EDF6FB7, 0x3ECF67B3, 0x3EDF6FB7, 0x3EDF6FB7, 0x3EDF6FB7,
			0x3EBF5FAF, 0x3EBB5DAE, 0x3EB75BAD, 0x3EBF5FAF, 0x3EAF57AB, 0x3EBF5FAF, 0x3EBF5FAF, 0x3EBF5FAF,
			0x3EFF7FBF, 0x3EFB7DBE, 0x3EF77BBD, 0x3EFF7FBF, 0x3EEF77BB, 0x3EFF7FBF, 0x3EFF7FBF, 0x3EFF7FBF,
			0x3E7F3F9F, 0x3E7B3D9E, 0x3E773B9D, 0x3E7F3F9F, 0x3E6F379B, 0x3E7F3F9F, 0x3E7F3F9F, 0x3E7F3F9F,
			0x3EFF7FBF, 0x3EFB7DBE, 0x3EF77BBD, 0x3EFF7FBF, 0x3EEF77BB, 0x3EFF7FBF, 0x3EFF7FBF, 0x3EFF7FBF,
			0x3EFF7FBF, 0x3EFB7DBE, 0x3EF77BBD, 0x3EFF7FBF, 0x3EEF77BB, 0x3EFF7FBF, 0x3EFF7FBF, 0x3EFF7FBF,
			0x3EFF7FBF, 0x3EFB7DBE, 0x3EF77BBD, 0x3EFF7FBF, 0x3EEF77BB, 0x3EFF7FBF, 0x3EFF7FBF, 0x3EFF7FBF,
			0x3DFEFF7F, 0x3DFAFD7E, 0x3DF6FB7D, 0x3DFEFF7F, 0x3DEEF77B, 0x3DFEFF7F, 0x3DFEFF7F, 0x3DFEFF7F,
			0x3DDEEF77, 0x3DDAED76, 0x3DD6EB75, 0x3DDEEF77, 0x3DCEE773, 0x3DDEEF77, 0x3DDEEF77, 0x3DDEEF77,
			0x3DBEDF6F, 0x3DBADD6E, 0x3DB6DB6D, 0x3DBEDF6F, 0x3DAED76B, 0x3DBEDF6F, 0x3DBEDF6F, 0x3DBEDF6F,
			0x3DFEFF7F, 0x3DFAFD7E, 0x3DF6FB7D, 0x3DFEFF7F, 0x3DEEF77B, 0x3DFEFF7F, 0x3DFEFF7F, 0x3DFEFF7F,
			0x3D7EBF5F, 0x3D7ABD5E, 0x3D76BB5D, 0x3D7EBF5F, 0x3D6EB75B, 0x3D7EBF5F, 0x3D7EBF5F, 0x3D7EBF5F,
			0x3DFEFF7F, 0x3DFAFD7E, 0x3DF6FB7D, 0x3DFEFF7F, 0x3DEEF77B, 0x3DFEFF7F, 0x3DFEFF7F, 0x3DFEFF7F,
			0x3DFEFF7F, 0x3DFAFD7E, 0x3DF6FB7D, 0x3DFEFF7F, 0x3DEEF77B, 0x3DFEFF7F, 0x3DFEFF7F, 0x3DFEFF7F,
			0x3DFEFF7F, 0x3DFAFD7E, 0x3DF6FB7D, 0x3DFEFF7F, 0x3DEEF77B, 0x3DFEFF7F, 0x3DFEFF7F, 0x3DFEFF7F,
			0x3FFFFFFF, 0x3FFBFDFE, 0x3FF7FBFD, 0x3FFFFFFF, 0x3FEFF7FB, 0x3FFFFFFF, 0x3FFFFFFF, 0x3FFFFFFF,
			0x3FDFEFF7, 0x3FDBEDF6, 0x3FD7EBF5, 0x3FDFEFF7, 0x3FCFE7F3, 0x3FDFEFF7, 0x3FDFEFF7, 0x3FDFEFF7,
			0x3FBFDFEF, 0x3FBBDDEE, 0x3FB7DBED, 0x3FBFDFEF, 0x3FAFD7EB, 0x3FBFDFEF, 0x3FBFDFEF, 0x3FBFDFEF,
			0x3FFFFFFF, 0x3FFBFDFE, 0x3FF7FBFD, 0x3FFFFFFF, 0x3FEFF7FB, 0x3FFFFFFF, 0x3FFFFFFF, 0x3FFFFFFF,
			0x3F7FBFDF, 0x3F7BBDDE, 0x3F77BBDD, 0x3F7FBFDF, 0x3F6FB7DB, 0x3F7FBFDF, 0x3F7FBFDF, 0x3F7FBFDF,
			0x3FFFFFFF, 0x3FFBFDFE, 0x3FF7FBFD, 0x3FFFFFFF, 0x3FEFF7FB, 0x3FFFFFFF, 0x3FFFFFFF, 0x3FFFFFFF,
			0x3FFFFFFF, 0x3FFBFDFE, 0x3FF7FBFD, 0x3FFFFFFF, 0x3FEFF7FB, 0x3FFFFFFF, 0x3FFFFFFF, 0x3FFFFFFF,
			0x3FFFFFFF, 0x3FFBFDFE, 0x3FF7FBFD, 0x3FFFFFFF, 0x3FEFF7FB, 0x3FFFFFFF, 0x3FFFFFFF, 0x3FFFFFFF,
			0x3BFDFEFF, 0x3BF9FCFE, 0x3BF5FAFD, 0x3BFDFEFF, 0x3BEDF6FB, 0x3BFDFEFF, 0x3BFDFEFF, 0x3BFDFEFF,
			0x3BDDEEF7, 0x3BD9ECF6, 0x3BD5EAF5, 0x3BDDEEF7, 0x3BCDE6F3, 0x3BDDEEF7, 0x3BDDEEF7, 0x3BDDEEF7,
			0x3BBDDEEF, 0x3BB9DCEE, 0x3BB5DAED, 0x3BBDDEEF, 0x3BADD6EB, 0x3BBDDEEF, 0x3BBDDEEF, 0x3BBDDEEF,
			0x3BFDFEFF, 0x3BF9FCFE, 0x3BF5FAFD, 0x3BFDFEFF, 0x3BEDF6FB, 0x3BFDFEFF, 0x3BFDFEFF, 0x3BFDFEFF,
			0x3B7DBEDF, 0x3B79BCDE, 0x3B75BADD, 0x3B7DBEDF, 0x3B6DB6DB, 0x3B7DBEDF, 0x3B7DBEDF, 0x3B7DBEDF,
			0x3BFDFEFF, 0x3BF9FCFE, 0x3BF5FAFD, 0x3BFDFEFF, 0x3BEDF6FB, 0x3BFDFEFF, 0x3BFDFEFF, 0x3BFDFEFF,
			0x3BFDFEFF, 0x3BF9FCFE, 0x3BF5FAFD, 0x3BFDFEFF, 0x3BEDF6FB, 0x3BFDFEFF, 0x3BFDFEFF, 0x3BFDFEFF,
			0x3BFDFEFF, 0x3BF9FCFE, 0x3BF5FAFD, 0x3BFDFEFF, 0x3BEDF6FB, 0x3BFDFEFF, 0x3BFDFEFF, 0x3BFDFEFF,
			0x3FFFFFFF, 0x3FFBFDFE, 0x3FF7FBFD, 0x3FFFFFFF, 0x3FEFF7FB, 0x3FFFFFFF, 0x3FFFFFFF, 0x3FFFFFFF,
			0x3FDFEFF7, 0x3FDBEDF6, 0x3FD7EBF5, 0x3FDFEFF7, 0x3FCFE7F3, 0x3FDFEFF7, 0x3FDFEFF7, 0x3FDFEFF7,
			0x3FBFDFEF, 0x3FBBDDEE, 0x3FB7DBED, 0x3FBFDFEF, 0x3FAFD7EB, 0x3FBFDFEF, 0x3FBFDFEF, 0x3FBFDFEF,
			0x3FFFFFFF, 0x3FFBFDFE, 0x3FF7FBFD, 0x3FFFFFFF, 0x3FEFF7FB, 0x3FFFFFFF, 0x3FFFFFFF, 0x3FFFFFFF,
			0x3F7FBFDF, 0x3F7BBDDE, 0x3F77BBDD, 0x3F7FBFDF, 0x3F6FB7DB, 0x3F7FBFDF, 0x3F7FBFDF, 0x3F7FBFDF,
			0x3FFFFFFF, 0x3FFBFDFE, 0x3FF7FBFD, 0x3FFFFFFF, 0x3FEFF7FB, 0x3FFFFFFF, 0x3FFFFFFF, 0x3FFFFFFF,
			0x3FFFFFFF, 0x3FFBFDFE, 0x3FF7FBFD, 0x3FFFFFFF, 0x3FEFF7FB, 0x3FFFFFFF, 0x3FFFFFFF, 0x3FFFFFFF,
			0x3FFFFFFF, 0x3FFBFDFE, 0x3FF7FBFD, 0x3FFFFFFF, 0x3FEFF7FB, 0x3FFFFFFF, 0x3FFFFFFF, 0x3FFFFFFF,
			0x3FFFFFFF, 0x3FFBFDFE, 0x3FF7FBFD, 0x3FFFFFFF, 0x3FEFF7FB, 0x3FFFFFFF, 0x3FFFFFFF, 0x3FFFFFFF,
			0x3FDFEFF7, 0x3FDBEDF6, 0x3FD7EBF5, 0x3FDFEFF7, 0x3FCFE7F3, 0x3FDFEFF7, 0x3FDFEFF7, 0x3FDFEFF7,
			0x3FBFDFEF, 0x3FBBDDEE, 0x3FB7DBED, 0x3FBFDFEF, 0x3FAFD7EB, 0x3FBFDFEF, 0x3FBFDFEF, 0x3FBFDFEF,
			0x3FFFFFFF, 0x3FFBFDFE, 0x3FF7FBFD, 0x3FFFFFFF, 0x3FEFF7FB, 0x3FFFFFFF, 0x3FFFFFFF, 0x3FFFFFFF,
			0x3F7FBFDF, 0x3F7BBDDE, 0x3F77BBDD, 0x3F7FBFDF, 0x3F6FB7DB, 0x3F7FBFDF, 0x3F7FBFDF, 0x3F7FBFDF,
			0x3FFFFFFF, 0x3FFBFDFE, 0x3FF7FBFD, 0x3FFFFFFF, 0x3FEFF7FB, 0x3FFFFFFF, 0x3FFFFFFF, 0x3FFFFFFF,
			0x3FFFFFFF, 0x3FFBFDFE, 0x3FF7FBFD, 0x3FFFFFFF, 0x3FEFF7FB, 0x3FFFFFFF, 0x3FFFFFFF, 0x3FFFFFFF,
			0x3FFFFFFF, 0x3FFBFDFE, 0x3FF7FBFD, 0x3FFFFFFF, 0x3FEFF7FB, 0x3FFFFFFF, 0x3FFFFFFF, 0x3FFFFFFF,
			0x3FFFFFFF, 0x3FFBFDFE, 0x3FF7FBFD, 0x3FFFFFFF, 0x3FEFF7FB, 0x3FFFFFFF, 0x3FFFFFFF, 0x3FFFFFFF,
			0x3FDFEFF7, 0x3FDBEDF6, 0x3FD7EBF5, 0x3FDFEFF7, 0x3FCFE7F3, 0x3FDFEFF7, 0x3FDFEFF7, 0x3FDFEFF7,
			0x3FBFDFEF, 0x3FBBDDEE, 0x3FB7DBED, 0x3FBFDFEF, 0x3FAFD7EB, 0x3FBFDFEF, 0x3FBFDFEF, 0x3FBFDFEF,
			0x3FFFFFFF, 0x3FFBFDFE, 0x3FF7FBFD, 0x3FFFFFFF, 0x3FEFF7FB, 0x3FFFFFFF, 0x3FFFFFFF, 0x3FFFFFFF,
			0x3F7FBFDF, 0x3F7BBDDE, 0x3F77BBDD, 0x3F7FBFDF, 0x3F6FB7DB, 0x3F7FBFDF, 0x3F7FBFDF, 0x3F7FBFDF,
			0x3FFFFFFF, 0x3FFBFDFE, 0x3FF7FBFD, 0x3FFFFFFF, 0x3FEFF7FB, 0x3FFFFFFF, 0x3FFFFFFF, 0x3FFFFFFF,
			0x3FFFFFFF, 0x3FFBFDFE, 0x3FF7FBFD, 0x3FFFFFFF, 0x3FEFF7FB, 0x3FFFFFFF, 0x3FFFFFFF, 0x3FFFFFFF,
			0x3FFFFFFF, 0x3FFBFDFE, 0x3FF7FBFD, 0x3FFFFFFF, 0x3FEFF7FB, 0x3FFFFFFF, 0x3FFFFFFF, 0x3FFFFFFF
		],
		ShrinkSingleTable = [
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0x54, 0x54, 0x54, 0x54, 0, 0, 0, 0, 0x54, 0x54, 0x54, 0x54,
			0, 0, 0x62, 0x62, 0, 0, 0x62, 0x62, 0, 0, 0x62, 0x62, 0, 0, 0x62, 0x62, 0, 0, 0x62, 0x62, 0x54, 0x54, 0x40, 0x40, 0, 0, 0x62, 0x62, 0x54, 0x54, 0x40, 0x40,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0x8C, 0x8C, 0x8C, 0x8C, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0x8C, 0x8C, 0x8C, 0x8C,
			0, 0xA1, 0, 0xA1, 0, 0xA1, 0, 0xA1, 0, 0xA1, 0, 0xA1, 0x8C, 0x80, 0x8C, 0x80, 0, 0xA1, 0, 0xA1, 0, 0xA1, 0, 0xA1, 0, 0xA1, 0, 0xA1, 0x8C, 0x80, 0x8C, 0x80,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0x8C, 0x8C, 0x8C, 0x8C, 0, 0, 0, 0, 0x54, 0x54, 0x54, 0x54, 0, 0, 0, 0, 0x4, 0x4, 0x4, 0x4,
			0, 0xA1, 0x62, 0x20, 0, 0xA1, 0x62, 0x20, 0, 0xA1, 0x62, 0x20, 0x8C, 0x80, 0, 0, 0, 0xA1, 0x62, 0x20, 0x54, 0, 0x40, 0, 0, 0xA1, 0x62, 0x20, 0x4, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0x10A, 0x10A, 0, 0, 0x10A, 0x10A, 0, 0x111, 0, 0x111, 0, 0x111, 0, 0x111, 0, 0x111, 0x10A, 0x100, 0, 0x111, 0x10A, 0x100,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0x10A, 0x10A, 0, 0, 0x10A, 0x10A, 0, 0x111, 0, 0x111, 0, 0x111, 0, 0x111, 0, 0x111, 0x10A, 0x100, 0, 0x111, 0x10A, 0x100,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0x10A, 0x10A, 0, 0, 0x10A, 0x10A, 0, 0x111, 0, 0x111, 0x54, 0x10, 0x54, 0x10, 0, 0x111, 0x10A, 0x100, 0x54, 0x10, 0, 0,
			0, 0, 0x62, 0x62, 0, 0, 0x62, 0x62, 0, 0, 0x2, 0x2, 0, 0, 0x2, 0x2, 0, 0x111, 0x62, 0, 0x54, 0x10, 0x40, 0, 0, 0x111, 0x2, 0, 0x54, 0x10, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0x10A, 0x10A, 0x8C, 0x8C, 0x8, 0x8, 0, 0x111, 0, 0x111, 0, 0x111, 0, 0x111, 0, 0x111, 0x10A, 0x100, 0x8C, 0, 0x8, 0,
			0, 0xA1, 0, 0xA1, 0, 0xA1, 0, 0xA1, 0, 0xA1, 0x10A, 0, 0x8C, 0x80, 0x8, 0, 0, 0x1, 0, 0x1, 0, 0x1, 0, 0x1, 0, 0x1, 0x10A, 0, 0x8C, 0, 0x8, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0x10A, 0x10A, 0x8C, 0x8C, 0x8, 0x8, 0, 0x111, 0, 0x111, 0x54, 0x10, 0x54, 0x10, 0, 0x111, 0x10A, 0x100, 0x4, 0, 0, 0,
			0, 0xA1, 0x62, 0x20, 0, 0xA1, 0x62, 0x20, 0, 0xA1, 0x2, 0, 0x8C, 0x80, 0, 0, 0, 0x1, 0x62, 0, 0x54, 0, 0x40, 0, 0, 0x1, 0x2, 0, 0x4, 0, 0, 0
		],
		ColumnSingleTable = [
			0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0,
			0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0,
			0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0,
			0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0,
			0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x1FF, 0x1FF, 0x1B6, 0x1FF, 0x1B6, 0x1B6, 0x1B6,
			0x0, 0x1FF, 0x1FF, 0x1B6, 0x1FF, 0x1B6, 0x1B6, 0x1B6, 0x0, 0x16D, 0x16D, 0x124, 0x16D, 0x124, 0x124, 0x124,
			0x0, 0x1FF, 0x1FF, 0x1B6, 0x1FF, 0x1B6, 0x1B6, 0x1B6, 0x0, 0x16D, 0x16D, 0x124, 0x16D, 0x124, 0x124, 0x124,
			0x0, 0x16D, 0x16D, 0x124, 0x16D, 0x124, 0x124, 0x124, 0x0, 0x16D, 0x16D, 0x124, 0x16D, 0x124, 0x124, 0x124,
			0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x1FF, 0x1FF, 0x1B6, 0x1FF, 0x1B6, 0x1B6, 0x1B6,
			0x0, 0x1FF, 0x1FF, 0x1B6, 0x1FF, 0x1B6, 0x1B6, 0x1B6, 0x0, 0x16D, 0x16D, 0x124, 0x16D, 0x124, 0x124, 0x124,
			0x0, 0x1FF, 0x1FF, 0x1B6, 0x1FF, 0x1B6, 0x1B6, 0x1B6, 0x0, 0x16D, 0x16D, 0x124, 0x16D, 0x124, 0x124, 0x124,
			0x0, 0x16D, 0x16D, 0x124, 0x16D, 0x124, 0x124, 0x124, 0x0, 0x16D, 0x16D, 0x124, 0x16D, 0x124, 0x124, 0x124,
			0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0xDB, 0xDB, 0x92, 0xDB, 0x92, 0x92, 0x92,
			0x0, 0xDB, 0xDB, 0x92, 0xDB, 0x92, 0x92, 0x92, 0x0, 0x49, 0x49, 0x0, 0x49, 0x0, 0x0, 0x0,
			0x0, 0xDB, 0xDB, 0x92, 0xDB, 0x92, 0x92, 0x92, 0x0, 0x49, 0x49, 0x0, 0x49, 0x0, 0x0, 0x0,
			0x0, 0x49, 0x49, 0x0, 0x49, 0x0, 0x0, 0x0, 0x0, 0x49, 0x49, 0x0, 0x49, 0x0, 0x0, 0x0,
			0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x1FF, 0x1FF, 0x1B6, 0x1FF, 0x1B6, 0x1B6, 0x1B6,
			0x0, 0x1FF, 0x1FF, 0x1B6, 0x1FF, 0x1B6, 0x1B6, 0x1B6, 0x0, 0x16D, 0x16D, 0x124, 0x16D, 0x124, 0x124, 0x124,
			0x0, 0x1FF, 0x1FF, 0x1B6, 0x1FF, 0x1B6, 0x1B6, 0x1B6, 0x0, 0x16D, 0x16D, 0x124, 0x16D, 0x124, 0x124, 0x124,
			0x0, 0x16D, 0x16D, 0x124, 0x16D, 0x124, 0x124, 0x124, 0x0, 0x16D, 0x16D, 0x124, 0x16D, 0x124, 0x124, 0x124,
			0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0xDB, 0xDB, 0x92, 0xDB, 0x92, 0x92, 0x92,
			0x0, 0xDB, 0xDB, 0x92, 0xDB, 0x92, 0x92, 0x92, 0x0, 0x49, 0x49, 0x0, 0x49, 0x0, 0x0, 0x0,
			0x0, 0xDB, 0xDB, 0x92, 0xDB, 0x92, 0x92, 0x92, 0x0, 0x49, 0x49, 0x0, 0x49, 0x0, 0x0, 0x0,
			0x0, 0x49, 0x49, 0x0, 0x49, 0x0, 0x0, 0x0, 0x0, 0x49, 0x49, 0x0, 0x49, 0x0, 0x0, 0x0,
			0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0xDB, 0xDB, 0x92, 0xDB, 0x92, 0x92, 0x92,
			0x0, 0xDB, 0xDB, 0x92, 0xDB, 0x92, 0x92, 0x92, 0x0, 0x49, 0x49, 0x0, 0x49, 0x0, 0x0, 0x0,
			0x0, 0xDB, 0xDB, 0x92, 0xDB, 0x92, 0x92, 0x92, 0x0, 0x49, 0x49, 0x0, 0x49, 0x0, 0x0, 0x0,
			0x0, 0x49, 0x49, 0x0, 0x49, 0x0, 0x0, 0x0, 0x0, 0x49, 0x49, 0x0, 0x49, 0x0, 0x0, 0x0,
			0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0xDB, 0xDB, 0x92, 0xDB, 0x92, 0x92, 0x92,
			0x0, 0xDB, 0xDB, 0x92, 0xDB, 0x92, 0x92, 0x92, 0x0, 0x49, 0x49, 0x0, 0x49, 0x0, 0x0, 0x0,
			0x0, 0xDB, 0xDB, 0x92, 0xDB, 0x92, 0x92, 0x92, 0x0, 0x49, 0x49, 0x0, 0x49, 0x0, 0x0, 0x0,
			0x0, 0x49, 0x49, 0x0, 0x49, 0x0, 0x0, 0x0, 0x0, 0x49, 0x49, 0x0, 0x49, 0x0, 0x0, 0x0
		],
		OtherMaskTable = [
			0x3FFBFDFE, 0x3FF7FBFD, 0x3FEFF7FB, 0x3FDFEFF7, 0x3FBFDFEF, 0x3F7FBFDF, 0x3EFF7FBF, 0x3DFEFF7F,
			0x3BFDFEFF, 0x3FFBFDFE, 0x3FF7FBFD, 0x3FEFF7FB, 0x3FDFEFF7, 0x3FBFDFEF, 0x3F7FBFDF, 0x3EFF7FBF,
			0x3DFEFF7F, 0x3BFDFEFF, 0x3FFBFDFE, 0x3FF7FBFD, 0x3FEFF7FB, 0x3FDFEFF7, 0x3FBFDFEF, 0x3F7FBFDF,
			0x3EFF7FBF, 0x3DFEFF7F, 0x3BFDFEFF, 0x3FFBFDFE, 0x3FF7FBFD, 0x3FEFF7FB, 0x3FDFEFF7, 0x3FBFDFEF,
			0x3F7FBFDF, 0x3EFF7FBF, 0x3DFEFF7F, 0x3BFDFEFF, 0x3FFBFDFE, 0x3FF7FBFD, 0x3FEFF7FB, 0x3FDFEFF7,
			0x3FBFDFEF, 0x3F7FBFDF, 0x3EFF7FBF, 0x3DFEFF7F, 0x3BFDFEFF, 0x3FFBFDFE, 0x3FF7FBFD, 0x3FEFF7FB,
			0x3FDFEFF7, 0x3FBFDFEF, 0x3F7FBFDF, 0x3EFF7FBF, 0x3DFEFF7F, 0x3BFDFEFF, 0x3FFBFDFE, 0x3FF7FBFD,
			0x3FEFF7FB, 0x3FDFEFF7, 0x3FBFDFEF, 0x3F7FBFDF, 0x3EFF7FBF, 0x3DFEFF7F, 0x3BFDFEFF, 0x3FFBFDFE,
			0x3FF7FBFD, 0x3FEFF7FB, 0x3FDFEFF7, 0x3FBFDFEF, 0x3F7FBFDF, 0x3EFF7FBF, 0x3DFEFF7F, 0x3BFDFEFF,
			0x3FFBFDFE, 0x3FF7FBFD, 0x3FEFF7FB, 0x3FDFEFF7, 0x3FBFDFEF, 0x3F7FBFDF, 0x3EFF7FBF, 0x3DFEFF7F,
			0x3BFDFEFF
		];

	private static readonly uint[]
		RowMaskTable = [0x7FFFFFF, 0x7FFFE00, 0x7FC01FF, 0x7FC0000, 0x3FFFF, 0x3FE00, 0x1FF, 0x0],
		SelfMaskTable = [
			0x37E3F001, 0x37E3F002, 0x37E3F004, 0x371F8E08, 0x371F8E10, 0x371F8E20, 0x30FC7E40, 0x30FC7E80,
			0x30FC7F00, 0x2FE003F8, 0x2FE005F8, 0x2FE009F8, 0x2F1C11C7, 0x2F1C21C7, 0x2F1C41C7, 0x28FC803F,
			0x28FD003F, 0x28FE003F, 0x1807F1F8, 0x180BF1F8, 0x1813F1F8, 0x18238FC7, 0x18438FC7, 0x18838FC7,
			0x19007E3F, 0x1A007E3F, 0x1C007E3F, 0x37E3F001, 0x37E3F002, 0x37E3F004, 0x371F8E08, 0x371F8E10,
			0x371F8E20, 0x30FC7E40, 0x30FC7E80, 0x30FC7F00, 0x2FE003F8, 0x2FE005F8, 0x2FE009F8, 0x2F1C11C7,
			0x2F1C21C7, 0x2F1C41C7, 0x28FC803F, 0x28FD003F, 0x28FE003F, 0x1807F1F8, 0x180BF1F8, 0x1813F1F8,
			0x18238FC7, 0x18438FC7, 0x18838FC7, 0x19007E3F, 0x1A007E3F, 0x1C007E3F, 0x37E3F001, 0x37E3F002,
			0x37E3F004, 0x371F8E08, 0x371F8E10, 0x371F8E20, 0x30FC7E40, 0x30FC7E80, 0x30FC7F00, 0x2FE003F8,
			0x2FE005F8, 0x2FE009F8, 0x2F1C11C7, 0x2F1C21C7, 0x2F1C41C7, 0x28FC803F, 0x28FD003F, 0x28FE003F,
			0x1807F1F8, 0x180BF1F8, 0x1813F1F8, 0x18238FC7, 0x18438FC7, 0x18838FC7, 0x19007E3F, 0x1A007E3F,
			0x1C007E3F
		];
#pragma warning restore format


	/// <summary>
	/// Stack to store current and previous states.
	/// </summary>
	private readonly BitwiseSolverState[] _stack = new BitwiseSolverState[50];

	/// <summary>
	/// Nasty global flag telling if <see cref="ApplySingleOrEmptyCells"/> found anything.
	/// </summary>
	/// <seealso cref="ApplySingleOrEmptyCells"/>
	private bool _singleApplied;

	/// <summary>
	/// Pointer to where to store the first solution. This value can be <see langword="null"/>.
	/// </summary>
	private char* _solution;

	/// <summary>
	/// The number of solutions found so far.
	/// </summary>
	private long _numSolutions;

	/// <summary>
	/// The max number of solution we're looking for.
	/// </summary>
	/// <remarks>
	/// For the consideration on the performance, I have refused to use auto-implemented property instead.
	/// </remarks>
	private long _limitSolutions;

	/// <summary>
	/// Pointer to the currently active slot.
	/// </summary>
	private BitwiseSolverState* _g;


	/// <inheritdoc/>
	public static string? UriLink => null;


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool? Solve(scoped ref readonly Grid grid, out Grid result)
	{
		ClearStack();

		var puzzleStr = grid.ToString("0");
		var solutionStr = stackalloc char[BufferLength];
		long solutions;
		fixed (char* pPuzzleStr = puzzleStr)
		{
			solutions = InternalSolve(pPuzzleStr, solutionStr, 2);
		}

		Unsafe.SkipInit(out result);
		var (_, @return) = solutions switch
		{
			0 => (Grid.Undefined, null),
			1 => (result = Grid.Parse(new ReadOnlySpan<char>(solutionStr, BufferLength)), true),
			_ => (Grid.Undefined, (bool?)false)
		};

		return @return;
	}

	/// <summary>
	/// The inner solver.
	/// </summary>
	/// <param name="puzzle">The pointer to the puzzle.</param>
	/// <param name="solution">The solution. <see langword="null"/> if you don't want to use the value.</param>
	/// <param name="limit">The limit.</param>
	/// <returns>The number of all solutions.</returns>
	/// <exception cref="ArgumentNullException">
	/// Throws when the argument <paramref name="puzzle"/> is <see langword="null"/>.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public long Solve(char* puzzle, char* solution, int limit)
	{
		ArgumentNullException.ThrowIfNull(puzzle);

		ClearStack();

		var solutionStr = stackalloc char[BufferLength];
		var solutionsCount = InternalSolve(puzzle, solutionStr, limit);
		if (solution != null)
		{
			Unsafe.CopyBlock(solution, solutionStr, sizeof(char) * BufferLength);
		}
		return solutionsCount;
	}

	/// <summary>
	/// The inner solver.
	/// </summary>
	/// <param name="puzzle">The puzzle.</param>
	/// <param name="solution">The solution. <see langword="null"/> if you don't want to use the value.</param>
	/// <param name="limit">The limit.</param>
	/// <returns>The number of all solutions.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public long Solve(string puzzle, char* solution, int limit)
	{
		ClearStack();

		fixed (char* p = puzzle)
		{
			var solutionStr = stackalloc char[BufferLength];
			var result = InternalSolve(p, solutionStr, limit);
			if (solution != null)
			{
				Unsafe.CopyBlock(solution, solutionStr, sizeof(char) * BufferLength);
			}
			return result;
		}
	}

	/// <summary>
	/// The inner solver.
	/// </summary>
	/// <param name="puzzle">The puzzle.</param>
	/// <param name="solution">
	/// The solution. The value keeps <see langword="null"/> if you doesn't want to use this result.
	/// </param>
	/// <param name="limit">The limit.</param>
	/// <returns>The number of all solutions.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public long Solve(string puzzle, out string solution, int limit)
	{
		ClearStack();

		fixed (char* p = puzzle)
		{
			var solutionStr = stackalloc char[BufferLength];
			var result = InternalSolve(p, solutionStr, limit);
			solution = new(solutionStr);
			return result;
		}
	}

	/// <summary>
	/// Same as <see cref="CheckValidity(string, out string?)"/>, but doesn't contain
	/// any <see langword="out"/> parameters.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <returns>The <see cref="bool"/> result. <see langword="true"/> for unique solution.</returns>
	/// <exception cref="ArgumentNullException">
	/// Throws when the argument <paramref name="grid"/> is <see langword="null"/>.
	/// </exception>
	/// <seealso cref="CheckValidity(string, out string?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool CheckValidity(char* grid)
	{
		ArgumentNullException.ThrowIfNull(grid);

		ClearStack();

		return InternalSolve(grid, null, 2) == 1;
	}

	/// <summary>
	/// Same as <see cref="CheckValidity(string, out string?)"/>, but doesn't contain
	/// any <see langword="out"/> parameters.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <returns>The <see cref="bool"/> result. <see langword="true"/> for unique solution.</returns>
	/// <seealso cref="CheckValidity(string, out string?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool CheckValidity(string grid)
	{
		fixed (char* puzzle = grid)
		{
			return CheckValidity(puzzle);
		}
	}

	/// <summary>
	/// Check the validity of the puzzle.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="solutionIfUnique">The solution if the puzzle is unique.</param>
	/// <returns>The <see cref="bool"/> result. <see langword="true"/> for unique solution.</returns>
	public bool CheckValidity(string grid, [NotNullWhen(true)] out string? solutionIfUnique)
	{
		ClearStack();

		fixed (char* puzzle = grid)
		{
			var result = stackalloc char[BufferLength];
			if (InternalSolve(puzzle, result, 2) == 1)
			{
				solutionIfUnique = new Span<char>(result, BufferLength).ToString();
				return true;
			}

			solutionIfUnique = null;
			return false;
		}
	}

	/// <summary>
	/// To solve the puzzle, and get the solution.
	/// </summary>
	/// <param name="puzzle">The puzzle to solve.</param>
	/// <returns>The solution. If failed to solve, <see cref="Grid.Undefined"/>.</returns>
	/// <seealso cref="Grid.Undefined"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Grid Solve(scoped ref readonly Grid puzzle) => Solve(in puzzle, out var result) is true ? result : Grid.Undefined;

	/// <summary>
	/// To clear the field <see cref="_stack"/>.
	/// </summary>
	/// <seealso cref="_stack"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void ClearStack()
#if true
		=> Array.Clear(_stack);
#else
		=> InitBlock(AsPointer(ref _stack[0]), 0, (uint)(sizeof(State) * 50));
#endif

	/// <summary>
	/// Set a cell as solved - used in <see cref="InitSudoku"/>.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="digit">The digit.</param>
	/// <returns>The <see cref="bool"/> result.</returns>
	private bool SetSolvedDigit(Cell cell, Digit digit)
	{
		var subBand = (int)Cell2Floor[cell];
		var band = Digit2BaseBand[digit] + subBand;
		var mask = Cell2Mask[cell];
		if ((_g->Bands[band] & mask) == 0)
		{
			return false;
		}

		_g->Bands[band] &= SelfMaskTable[cell];
		var tblMask = OtherMaskTable[cell];
		_g->Bands[Another1Table[band]] &= (uint)tblMask;
		_g->Bands[Another2Table[band]] &= (uint)tblMask;
		mask = ~mask;
		_g->UnsolvedCells[subBand] &= (uint)mask;
		var rowBit = digit * 9 + CellToRow[cell];
		_g->UnsolvedRows[rowBit / 27] &= (uint)~(1 << (Mod27[rowBit]));
		_g->Bands[subBand] &= (uint)mask;
		_g->Bands[subBand + 3] &= (uint)mask;
		_g->Bands[subBand + 6] &= (uint)mask;
		_g->Bands[subBand + 9] &= (uint)mask;
		_g->Bands[subBand + 12] &= (uint)mask;
		_g->Bands[subBand + 15] &= (uint)mask;
		_g->Bands[subBand + 18] &= (uint)mask;
		_g->Bands[subBand + 21] &= (uint)mask;
		_g->Bands[subBand + 24] &= (uint)mask;
		_g->Bands[band] |= (uint)~mask;
		return true;
	}

	/// <summary>
	/// Eliminate a digit - used in <see cref="InitSudoku"/>.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="digit">The digit.</param>
	/// <returns>The <see cref="bool"/> result.</returns>
	private bool EliminateDigit(Cell cell, Digit digit)
	{
		var subBand = Cell2Floor[cell];
		var band = Digit2BaseBand[digit] + subBand;
		var mask = Cell2Mask[cell];
		if ((_g->Bands[band] & mask) == 0)
		{
			// This candidate has been removed yet.
			return true;
		}

		_g->Bands[band] &= (uint)~mask;
		return true;
	}

	/// <summary>
	/// Set a cell as solved - used in various guess routines.
	/// </summary>
	/// <param name="band">The band.</param>
	/// <param name="mask">The mask.</param>
	/// <returns>The <see cref="bool"/> result.</returns>
	private bool SetSolvedMask(int band, uint mask)
	{
		if ((_g->Bands[band] & mask) == 0)
		{
			return false;
		}

		var subBand = Mod3[band];
		var cell = subBand * 27 + BitPos(mask);
		_g->Bands[band] &= SelfMaskTable[cell];
		return true;
	}

	/// <summary>
	/// Setup everything and load the puzzle.
	/// </summary>
	/// <param name="puzzle">The pointer that points to a puzzle buffer.</param>
	/// <returns>The <see cref="bool"/> result.</returns>
	private bool InitSudoku(char* puzzle)
	{
		fixed (BitwiseSolverState* g = _stack)
		{
			_numSolutions = 0;
			for (var band = 0; band < 27; band++)
			{
				g->Bands[band] = BitSet27;
			}

			Unsafe.InitBlock(g->PrevBands, 0, 27 * sizeof(uint));
			g->UnsolvedCells[0] = g->UnsolvedCells[1] = g->UnsolvedCells[2] = BitSet27;
			g->UnsolvedRows[0] = g->UnsolvedRows[1] = g->UnsolvedRows[2] = BitSet27;
			g->Pairs[0] = g->Pairs[1] = g->Pairs[2] = 0;

			_g = g;
		}

		switch (StringLengthOf(puzzle))
		{
			case 81:
			{
				for (var cell = 0; cell < 81; cell++, puzzle++)
				{
					if (*puzzle is > '0' and <= '9')
					{
						if (!SetSolvedDigit(cell, *puzzle - '1'))
						{
							return false;
						}
					}
					else if (*puzzle == 0)
					{
						// End of string before end of puzzle!
						return false;
					}
				}

				return true;
			}
			case 729:
			{
				for (var cell = 0; cell < 81; cell++)
				{
					var mask = (Mask)0;
					for (var digit = 0; digit < 9; digit++, puzzle++)
					{
						if (*puzzle == '0')
						{
							mask |= (Mask)(1 << digit);
						}
					}

					for (var (digit, temp) = (0, mask); digit < 9; digit++, temp >>= 1)
					{
						if ((temp & 1) != 0 && !EliminateDigit(cell, digit))
						{
							return false;
						}
					}
				}

				return true;
			}
			default:
			{
				return false;
			}
		}
	}

	/// <summary>
	/// Core of fast processing.
	/// </summary>
	/// <returns>The <see cref="bool"/> value.</returns>
	private bool Update()
	{
		uint shrink = 1, a, b, c, cl;
		while (shrink != 0)
		{
			uint s;
			shrink = 0;
			if (_g->UnsolvedRows[0] == 0) goto Digit3;
			{
				var ar = _g->UnsolvedRows[0];  // valid for Digits 0,1,2
				if ((ar & 0x1FF) == 0) goto Digit1;
				if (_g->Bands[0 * 3 + 0] == _g->PrevBands[0 * 3 + 0]) goto Digit0b;
				if (!updn(&s, 0, 0, 1, 2)) return false;
				if ((ar & 7) != *&s)
				{
					ar &= 0x7FFFFF8 | s;
					upwcl(s, 0, 3, 6, 9, 12, 15, 18, 21, 24, a);
				}
			Digit0b:
				if (_g->Bands[0 * 3 + 1] == _g->PrevBands[0 * 3 + 1]) goto Digit0c;
				if (!updn(&s, 0, 1, 0, 2)) return false;
				if (((ar >> 3) & 7) != *&s)
				{
					ar &= 0x7FFFFC7 | (s << 3);
					upwcl(s, 1, 4, 7, 10, 13, 16, 19, 22, 25, a);
				}
			Digit0c:
				if (_g->Bands[0 * 3 + 2] == _g->PrevBands[0 * 3 + 2]) goto Digit1;
				if (!updn(&s, 0, 2, 0, 1)) return false;
				if (((ar >> 6) & 7) != *&s)
				{
					ar &= 0x7FFFE3F | (s << 6);
					upwcl(s, 2, 5, 8, 11, 14, 17, 20, 23, 26, a);
				}
			Digit1:
				if (((ar >> 9) & 0x1FF) == 0) goto Digit2;
				if (_g->Bands[1 * 3 + 0] == _g->PrevBands[1 * 3 + 0]) goto Digit1b;
				if (!updn(&s, 1, 0, 1, 2)) return false;
				if (((ar >> 9) & 7) != *&s)
				{
					ar &= 0x7FFF1FF | (s << 9);
					upwcl(s, 0, 0, 6, 9, 12, 15, 18, 21, 24, a);
				}
			Digit1b:
				if (_g->Bands[1 * 3 + 1] == _g->PrevBands[1 * 3 + 1]) goto Digit1c;
				if (!updn(&s, 1, 1, 0, 2)) return false;
				if (((ar >> 12) & 7) != *&s)
				{
					ar &= 0x7FF8FFF | (s << 12);
					upwcl(s, 1, 1, 7, 10, 13, 16, 19, 22, 25, a);
				}
			Digit1c:
				if (_g->Bands[1 * 3 + 2] == _g->PrevBands[1 * 3 + 2]) goto Digit2;
				if (!updn(&s, 1, 2, 0, 1)) return false;
				if (((ar >> 15) & 7) != *&s)
				{
					ar &= 0x7FC7FFF | (s << 15);
					upwcl(s, 2, 2, 8, 11, 14, 17, 20, 23, 26, a);
				}
			Digit2:
				if (((ar >> 18) & 0x1FF) == 0) goto End012;
				if (_g->Bands[2 * 3 + 0] == _g->PrevBands[2 * 3 + 0]) goto Digit2b;
				if (!updn(&s, 2, 0, 1, 2)) return false;
				if (((ar >> 18) & 7) != *&s)
				{
					ar &= 0x7E3FFFF | (s << 18);
					upwcl(s, 0, 0, 3, 9, 12, 15, 18, 21, 24, a);
				}
			Digit2b:
				if (_g->Bands[2 * 3 + 1] == _g->PrevBands[2 * 3 + 1]) goto Digit2c;
				if (!updn(&s, 2, 1, 0, 2)) return false;
				if (((ar >> 21) & 7) != *&s)
				{
					ar &= 0x71FFFFF | (s << 21);
					upwcl(s, 1, 1, 4, 10, 13, 16, 19, 22, 25, a);
				}
			Digit2c:
				if (_g->Bands[2 * 3 + 2] == _g->PrevBands[2 * 3 + 2]) goto End012;
				if (!updn(&s, 2, 2, 0, 1)) return false;
				if (((ar >> 24) & 7) != *&s)
				{
					ar &= 0xFFFFFF | (s << 24);
					upwcl(s, 2, 2, 5, 11, 14, 17, 20, 23, 26, a);
				}
			End012:
				_g->UnsolvedRows[0] = ar;
			}
		Digit3:
			if (_g->UnsolvedRows[1] == 0) goto Digit6;
			{
				var ar = _g->UnsolvedRows[1];  // valid for Digits 3,4,5
				if ((ar & 0x1FF) == 0) goto Digit4;
				if (_g->Bands[3 * 3 + 0] == _g->PrevBands[3 * 3 + 0]) goto Digit3b;
				if (!updn(&s, 3, 0, 1, 2)) return false;
				if ((ar & 7) != *&s)
				{
					ar &= 0x7FFFFF8 | s;
					upwcl(s, 0, 0, 3, 6, 12, 15, 18, 21, 24, a);
				}
			Digit3b:
				if (_g->Bands[3 * 3 + 1] == _g->PrevBands[3 * 3 + 1]) goto Digit3c;
				if (!updn(&s, 3, 1, 0, 2)) return false;
				if (((ar >> 3) & 7) != *&s)
				{
					ar &= 0x7FFFFC7 | (s << 3);
					upwcl(s, 1, 1, 4, 7, 13, 16, 19, 22, 25, a);
				}
			Digit3c:
				if (_g->Bands[3 * 3 + 2] == _g->PrevBands[3 * 3 + 2]) goto Digit4;
				if (!updn(&s, 3, 2, 0, 1)) return false;
				if (((ar >> 6) & 7) != *&s)
				{
					ar &= 0x7FFFE3F | (s << 6);
					upwcl(s, 2, 2, 5, 8, 14, 17, 20, 23, 26, a);
				}
			Digit4:
				if (((ar >> 9) & 0x1FF) == 0) goto Digit5;
				if (_g->Bands[4 * 3 + 0] == _g->PrevBands[4 * 3 + 0]) goto Digit4b;
				if (!updn(&s, 4, 0, 1, 2)) return false;
				if (((ar >> 9) & 7) != *&s)
				{
					ar &= 0x7FFF1FF | (s << 9);
					upwcl(s, 0, 0, 3, 6, 9, 15, 18, 21, 24, a);
				}
			Digit4b:
				if (_g->Bands[4 * 3 + 1] == _g->PrevBands[4 * 3 + 1]) goto Digit4c;
				if (!updn(&s, 4, 1, 0, 2)) return false;
				if (((ar >> 12) & 7) != *&s)
				{
					ar &= 0x7FF8FFF | (s << 12);
					upwcl(s, 1, 1, 4, 7, 10, 16, 19, 22, 25, a);
				}
			Digit4c:
				if (_g->Bands[4 * 3 + 2] == _g->PrevBands[4 * 3 + 2]) goto Digit5;
				if (!updn(&s, 4, 2, 0, 1)) return false;
				if (((ar >> 15) & 7) != *&s)
				{
					ar &= 0x7FC7FFF | (s << 15);
					upwcl(s, 2, 2, 5, 8, 11, 17, 20, 23, 26, a);
				}
			Digit5:
				if (((ar >> 18) & 0x1FF) == 0) goto End345;
				if (_g->Bands[5 * 3 + 0] == _g->PrevBands[5 * 3 + 0]) goto Digit5b;
				if (!updn(&s, 5, 0, 1, 2)) return false;
				if (((ar >> 18) & 7) != *&s)
				{
					ar &= 0x7E3FFFF | (s << 18);
					upwcl(s, 0, 0, 3, 6, 9, 12, 18, 21, 24, a);
				}
			Digit5b:
				if (_g->Bands[5 * 3 + 1] == _g->PrevBands[5 * 3 + 1]) goto Digit5c;
				if (!updn(&s, 5, 1, 0, 2)) return false;
				if (((ar >> 21) & 7) != *&s)
				{
					ar &= 0x71FFFFF | (s << 21);
					upwcl(s, 1, 1, 4, 7, 10, 13, 19, 22, 25, a);
				}
			Digit5c:
				if (_g->Bands[5 * 3 + 2] == _g->PrevBands[5 * 3 + 2]) goto End345;
				if (!updn(&s, 5, 2, 0, 1)) return false;
				if (((ar >> 24) & 7) != *&s)
				{
					ar &= 0xFFFFFF | (s << 24);
					upwcl(s, 2, 2, 5, 8, 11, 14, 20, 23, 26, a);
				}
			End345:
				_g->UnsolvedRows[1] = ar;
			}
		Digit6:
			if (_g->UnsolvedRows[2] == 0) continue;
			{
				var ar = _g->UnsolvedRows[2];  // valid for Digits 6,7,8
				if ((ar & 0x1FF) == 0) goto Digit7;
				if (_g->Bands[6 * 3 + 0] == _g->PrevBands[6 * 3 + 0]) goto Digit6b;
				if (!updn(&s, 6, 0, 1, 2)) return false;
				if ((ar & 7) != *&s)
				{
					ar &= 0x7FFFFF8 | s;
					upwcl(s, 0, 0, 3, 6, 9, 12, 15, 21, 24, a);
				}
			Digit6b:
				if (_g->Bands[6 * 3 + 1] == _g->PrevBands[6 * 3 + 1]) goto Digit6c;
				if (!updn(&s, 6, 1, 0, 2)) return false;
				if (((ar >> 3) & 7) != *&s)
				{
					ar &= 0x7FFFFC7 | (s << 3);
					upwcl(s, 1, 1, 4, 7, 10, 13, 16, 22, 25, a);
				}
			Digit6c:
				if (_g->Bands[6 * 3 + 2] == _g->PrevBands[6 * 3 + 2]) goto Digit7;
				if (!updn(&s, 6, 2, 0, 1)) return false;
				if (((ar >> 6) & 7) != *&s)
				{
					ar &= 0x7FFFE3F | (s << 6);
					upwcl(s, 2, 2, 5, 8, 11, 14, 17, 23, 26, a);
				}
			Digit7:
				if (((ar >> 9) & 0x1FF) == 0) goto Digit8;
				if (_g->Bands[7 * 3 + 0] == _g->PrevBands[7 * 3 + 0]) goto Digit7b;
				if (!updn(&s, 7, 0, 1, 2)) return false;
				if (((ar >> 9) & 7) != *&s)
				{
					ar &= 0x7FFF1FF | (s << 9);
					upwcl(s, 0, 0, 3, 6, 9, 12, 15, 18, 24, a);
				}
			Digit7b:
				if (_g->Bands[7 * 3 + 1] == _g->PrevBands[7 * 3 + 1]) goto Digit7c;
				if (!updn(&s, 7, 1, 0, 2)) return false;
				if (((ar >> 12) & 7) != *&s)
				{
					ar &= 0x7FF8FFF | (s << 12);
					upwcl(s, 1, 1, 4, 7, 10, 13, 16, 19, 25, a);
				}
			Digit7c:
				if (_g->Bands[7 * 3 + 2] == _g->PrevBands[7 * 3 + 2]) goto Digit8;
				if (!updn(&s, 7, 2, 0, 1)) return false;
				if (((ar >> 15) & 7) != *&s)
				{
					ar &= 0x7FC7FFF | (s << 15);
					upwcl(s, 2, 2, 5, 8, 11, 14, 17, 20, 26, a);
				}
			Digit8:
				if (((ar >> 18) & 0x1FF) == 0) goto End678;
				if (_g->Bands[8 * 3 + 0] == _g->PrevBands[8 * 3 + 0]) goto Digit8b;
				if (!updn(&s, 8, 0, 1, 2)) return false;
				if (((ar >> 18) & 7) != *&s)
				{
					ar &= 0x7E3FFFF | (s << 18);
					upwcl(s, 0, 0, 3, 6, 9, 12, 15, 18, 21, a);
				}
			Digit8b:
				if (_g->Bands[8 * 3 + 1] == _g->PrevBands[8 * 3 + 1]) goto Digit8c;
				if (!updn(&s, 8, 1, 0, 2)) return false;
				if (((ar >> 21) & 7) != *&s)
				{
					ar &= 0x71FFFFF | (s << 21);
					upwcl(s, 1, 1, 4, 7, 10, 13, 16, 19, 22, a);
				}
			Digit8c:
				if (_g->Bands[8 * 3 + 2] == _g->PrevBands[8 * 3 + 2]) goto End678;
				if (!updn(&s, 8, 2, 0, 1)) return false;
				if (((ar >> 24) & 7) != *&s)
				{
					ar &= 0xFFFFFF | (s << 24);
					upwcl(s, 2, 2, 5, 8, 11, 14, 17, 20, 23, a);
				}
			End678:
				_g->UnsolvedRows[2] = ar;
			}
		}
		return true;


		// The core Update routine from Zhou Yundong.
		// This copy has been optimized by champagne and JasonLion in minor ways.
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		bool updn(uint* s, uint i, uint j, uint k, uint l)
		{
			a = _g->Bands[i * 3 + j];
			shrink = (uint)(ShrinkMaskTable[a & 0x1FF] | ShrinkMaskTable[(a >> 9) & 0x1FF] << 3 | ShrinkMaskTable[a >> 18] << 6);
			if ((a &= (uint)ComplexMaskTable[shrink]) == 0)
			{
				return false;
			}

			b = _g->Bands[i * 3 + k];
			c = _g->Bands[i * 3 + l];
			*s = (a | a >> 9 | a >> 18) & 0x1FF;
			_g->Bands[i * 3 + l] &= (uint)MaskSingleTable[*s];
			_g->Bands[i * 3 + k] &= (uint)MaskSingleTable[*s];
			*s = RowUniqueTable[ShrinkSingleTable[shrink] & ColumnSingleTable[*s]];
			_g->PrevBands[i * 3 + j] = _g->Bands[i * 3 + j] = a;

			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void upwcl(uint s, uint i, uint p, uint q, uint r, uint t, uint u, uint v, uint w, uint x, uint a)
		{
			cl = ~(a & RowMaskTable[s]);
			_g->UnsolvedCells[i] &= cl;
			_g->Bands[p] &= cl;
			_g->Bands[q] &= cl;
			_g->Bands[r] &= cl;
			_g->Bands[t] &= cl;
			_g->Bands[u] &= cl;
			_g->Bands[v] &= cl;
			_g->Bands[w] &= cl;
			_g->Bands[x] &= cl;
		}
	}

	/// <summary>
	/// Find singles, bi-value cells, and impossible cells.
	/// </summary>
	/// <returns>A <see cref="bool"/> result.</returns>
	private bool ApplySingleOrEmptyCells()
	{
		_singleApplied = false;
		for (var subBand = 0; subBand < 3; subBand++)
		{
			// Loop unrolling really helps.
			var r1 = _g->Bands[subBand];           // r1 - Cells in band with pencil one or more times.
			var bandData = _g->Bands[subBand + 3]; // bandData - Hint to save value in register.
			var r2 = r1 & bandData;                // r2 - Pencil mark in cell two or more times.
			r1 |= bandData;
			bandData = _g->Bands[subBand + 6];
			var r3 = r2 & bandData;                // r3 - Pencil mark in cell three or more times.
			r2 |= r1 & bandData;
			r1 |= bandData;
			bandData = _g->Bands[subBand + 9];
			r3 |= r2 & bandData;
			r2 |= r1 & bandData;
			r1 |= bandData;
			bandData = _g->Bands[subBand + 12];
			r3 |= r2 & bandData;
			r2 |= r1 & bandData;
			r1 |= bandData;
			bandData = _g->Bands[subBand + 15];
			r3 |= r2 & bandData;
			r2 |= r1 & bandData;
			r1 |= bandData;
			bandData = _g->Bands[subBand + 18];
			r3 |= r2 & bandData;
			r2 |= r1 & bandData;
			r1 |= bandData;
			bandData = _g->Bands[subBand + 21];
			r3 |= r2 & bandData;
			r2 |= r1 & bandData;
			r1 |= bandData;
			bandData = _g->Bands[subBand + 24];
			r3 |= r2 & bandData;
			r2 |= r1 & bandData;
			r1 |= bandData;

			if (r1 != BitSet27)
			{
				// Something is locked, can't be solved.
				return true;
			}

			_g->Pairs[subBand] = r2 & ~r3;          // Exactly two pencil marks in cell.
			r1 &= ~r2;                              // Exactly one pencil mark in cell.
			r1 &= _g->UnsolvedCells[subBand];       // Ignore already solved cells.
			while (r1 != 0)
			{
				// Set all the single pencil mark cells
				_singleApplied = true;
				var bit = r1 & (uint)-(int)r1;     // Process once cell at a time.
				r1 ^= bit;
				Digit digit;
				for (digit = 0; digit < 9; digit++)
				{
					// Requires finding for which digit they are.
					if ((_g->Bands[digit * 3 + subBand] & bit) != 0)
					{
						SetSolvedMask(digit * 3 + subBand, bit);
						break;
					}
				}
				if (digit == 9)
				{
					// Previous singles locked the cell.
					return true;
				}
			}
		}
		return false;
	}

	/// <summary>
	/// The internal solving method.
	/// </summary>
	/// <param name="puzzle">The pointer to the puzzle string.</param>
	/// <param name="solutionPtr">The pointer to the solution string.</param>
	/// <param name="limit">The limitation for the number of all final solutions.</param>
	/// <returns>The number of solutions found.</returns>
	private long InternalSolve(char* puzzle, char* solutionPtr, int limit)
	{
		_numSolutions = 0;
		_limitSolutions = limit;
		_solution = solutionPtr;

		if (!InitSudoku(puzzle))
		{
			return 0;
		}

		if (ApplySingleOrEmptyCells())
		{
			// Locked empty cell or conflict singles in cells.
			return 0;
		}

		if (FullUpdate() == 0)
		{
			return 0;
		}

		Guess();

		return _numSolutions;
	}

	/// <summary>
	/// Extract solution as a string.
	/// </summary>
	/// <param name="solution">
	/// The solution pointer. <b>The buffer should be at least <see cref="BufferLength"/>
	/// of value of length.</b>
	/// </param>
	private void ExtractSolution(char* solution)
	{
		for (var cell = 0; cell < 81; cell++)
		{
			var mask = Cell2Mask[cell];
			var offset = (int)Cell2Floor[cell];
			for (var digit = 0; digit < 9; digit++)
			{
				if ((_g->Bands[offset] & mask) != 0)
				{
					solution[cell] = (char)('1' + digit);
					break;
				}
				offset += 3;
			}
		}

		solution[81] = '\0';
	}

	/// <summary>
	/// Try both options for cells with exactly two pencil marks.
	/// </summary>
	/// <returns>A <see cref="bool"/> result.</returns>
	private bool GuessBiValueInCell()
	{
		// Uses pairs map, set in ApplySingleOrEmptyCells
		for (var subBand = 0; subBand < 3; subBand++)
		{
			var map = _g->Pairs[subBand];
			if (map != 0)
			{
				map &= (uint)-(int)map;
				var tries = 2;
				var band = subBand;
				for (var digit = 0; digit < 9; digit++, band += 3)
				{
					if ((_g->Bands[band] & map) != 0)
					{
						if (--tries != 0)
						{
							// First of pair.
							Unsafe.CopyBlock(_g + 1, _g, (uint)sizeof(BitwiseSolverState));
							_g->Bands[band] ^= map;
							_g++;
							SetSolvedMask(band, map);
							if (FullUpdate() != 0)
							{
								Guess();
							}

							_g--;
						}
						else
						{
							// Second of pair.
							SetSolvedMask(band, map);
							if (FullUpdate() != 0)
							{
								Guess();
							}

							return true;
						}
					}
				}
			}
		}

		return false;
	}

	/// <summary>
	/// Guess all possibilities in first unsolved cell.
	/// </summary>
	private void GuessFirstCell()
	{
		// Kind of dumb, but _way_ fast code.
		for (var subBand = 0; subBand < 3; subBand++)
		{
			if (_g->UnsolvedCells[subBand] == 0)
			{
				continue;
			}

			var cellMask = _g->UnsolvedCells[subBand];
			cellMask &= (uint)-(int)cellMask;
			var band = subBand;
			for (var digit = 0; digit < 9; digit++, band += 3)
			{
				if ((_g->Bands[band] & cellMask) != 0)
				{
					// Eliminate option in the current stack entry.
					Unsafe.CopyBlock(_g + 1, _g, (uint)sizeof(BitwiseSolverState));
					_g->Bands[band] ^= cellMask;
					_g++;
					SetSolvedMask(band, cellMask); // And try it out in a nested stack entry.
					if (FullUpdate() != 0)
					{
						Guess();
					}

					_g--;
				}
			}
			return;
		}
	}

	/// <summary>
	/// Either already solved, or guess and recurse.
	/// </summary>
	private void Guess()
	{
		if ((_g->UnsolvedRows[0] | _g->UnsolvedRows[1] | _g->UnsolvedRows[2]) == 0)
		{
			// Already solved.
			if (_solution != null && _numSolutions == 0)
			{
				// Store the first solution.
				ExtractSolution(_solution);
			}

			_numSolutions++;

			return;
		}

		if (!GuessBiValueInCell())
		{
			// Both of these recursions.
			GuessFirstCell();
		}
	}

	/// <summary>
	/// Get as far as possible without guessing.
	/// </summary>
	/// <returns>A <see cref="byte"/> result.</returns>
	private byte FullUpdate()
	{
		if (_numSolutions >= _limitSolutions)
		{
			return 0;
		}

		while (true)
		{
			if (!Update())
			{
				// Game locked in update.
				return 0;
			}

			if ((_g->UnsolvedCells[0] | _g->UnsolvedCells[1] | _g->UnsolvedCells[2]) == 0)
			{
				return 2;
			}

			// locked empty cell or conflict singles in cells.
			if (ApplySingleOrEmptyCells())
			{
				return 0;
			}

			// Found a single, run Update again.
			if (_singleApplied)
			{
				continue;
			}

			break;
		}

		return 1;
	}


	/// <summary>
	/// Get the bit position.
	/// </summary>
	/// <param name="map">The map.</param>
	/// <returns>The position.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static byte BitPos(uint map) => MultiplyDeBruijnBitPosition32[map * 0x077CB531U >> 27];

	/// <summary>
	/// Get the length of the specified string which is represented by a <see cref="char"/>*.
	/// </summary>
	/// <param name="ptr">The pointer.</param>
	/// <returns>The total length.</returns>
	/// <exception cref="ArgumentNullException">
	/// Throws when the argument <paramref name="ptr"/> is <see langword="null"/>.
	/// </exception>
	/// <remarks>
	/// In C#, this function is unsafe because the implementation of
	/// <see cref="string"/> types between C and C# is totally different.
	/// In C, <see cref="string"/> is like a <see cref="char"/>* or a
	/// <see cref="char"/>[], they ends with the terminator symbol <c>'\0'</c>.
	/// However, C# not.
	/// </remarks>
	private static int StringLengthOf(char* ptr)
	{
		ArgumentNullException.ThrowIfNull(ptr);

		var result = 0;
		for (var p = ptr; *p != '\0'; p++)
		{
			result++;
		}

		return result;
	}
}
