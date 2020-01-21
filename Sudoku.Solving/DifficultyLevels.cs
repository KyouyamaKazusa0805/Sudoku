using System;

namespace Sudoku.Solving
{
	/// <summary>
	/// Provides a difficulty kind for a puzzle.
	/// </summary>
	[Flags]
	public enum DifficultyLevels : short
	{
		Unknown = 0,
		VeryEasy = 1,
		Easy = 2,
		Moderate = 4,
		Advanced = 8,
		Hard = 16,
		VeryHard = 32,
		Fiendish = 64,
		Diabolical = 128,
		Crazy = 256,
		Nightmare = 512,
		BeyondNightmare = 1024,
		LastResort = 2048,
	}
}
