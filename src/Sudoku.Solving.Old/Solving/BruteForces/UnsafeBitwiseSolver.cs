namespace Sudoku.Solving.BruteForces;

/// <summary>
/// Define an unsafe bitwise solver.
/// </summary>
/// <remarks>
/// This solver is implemented by Zhou Yundong, but he wrote this solver using C.
/// I change the programming language to C# to decrease the native calling.
/// </remarks>
public sealed unsafe partial class UnsafeBitwiseSolver : ISolver
{
	/// <summary>
	/// All pencil marks set - 27 bits per band.
	/// </summary>
	private const int BitSet27 = 0x7FFFFFF;

	/// <summary>
	/// The buffer length of a solution puzzle.
	/// </summary>
	private const int BufferLength = 82;


	/// <summary>
	/// Stack to store current and previous states.
	/// </summary>
	private readonly State[] _stack = new State[50];

	/// <summary>
	/// Pointer to the currently active slot.
	/// </summary>
	private State* _g;

	/// <summary>
	/// The number of solutions found so far.
	/// </summary>
	private long _numSolutions;

	/// <summary>
	/// The max number of solution we're looking for.
	/// </summary>
	private long _limitSolutions;

	/// <summary>
	/// Nasty global flag telling if <see cref="ApplySingleOrEmptyCells"/> found anything.
	/// </summary>
	/// <seealso cref="ApplySingleOrEmptyCells"/>
	private bool _singleApplied;

	/// <summary>
	/// Pointer to where to store the first solution. This value can be <see langword="null"/>.
	/// </summary>
	private char* _solution;


	/// <inheritdoc/>
	public string SolverName => TextResources.Current.Bitwise;


	/// <inheritdoc/>
	/// <exception cref="NoSolutionException">Throws when the puzzle has no valid solution.</exception>
	/// <exception cref="MultipleSolutionsException">
	/// Throws when the puzzle has multiple solutions.
	/// </exception>
	public AnalysisResult Solve(in SudokuGrid grid)
	{
		var stopwatch = new Stopwatch();

		string puzzle = grid.ToString("0");
		fixed (char* p = puzzle)
		{
			char* solutionStr = stackalloc char[BufferLength];

			stopwatch.Start();
			InternalSolve(p, solutionStr, 2);
			stopwatch.Stop();

			return _numSolutions switch
			{
				0 => throw new NoSolutionException(grid),
				1 => new(SolverName, grid, true, stopwatch.Elapsed)
				{
					Solution = SudokuGrid.Parse(new ReadOnlySpan<char>(solutionStr, BufferLength)),
				},
				_ => throw new MultipleSolutionsException(grid),
			};
		}
	}

	/// <summary>
	/// The inner solver.
	/// </summary>
	/// <param name="puzzle">The puzzle.</param>
	/// <param name="solution">
	/// The solution. The value keeps <see langword="null"/>
	/// if you doesn't want to use this result.
	/// </param>
	/// <param name="limit">The limit.</param>
	/// <returns>The number of all solutions.</returns>
	public long Solve(string puzzle, out string solution, int limit)
	{
		fixed (char* p = puzzle)
		{
			char* solutionStr = stackalloc char[BufferLength];
			long result = InternalSolve(p, solutionStr, limit);

			solution = new(solutionStr);

			return result;
		}
	}

	/// <summary>
	/// The inner solver.
	/// </summary>
	/// <param name="puzzle">The puzzle.</param>
	/// <param name="solution">
	/// The solution. <see langword="null"/> if you don't want to use the value.
	/// </param>
	/// <param name="limit">The limit.</param>
	/// <returns>The number of all solutions.</returns>
	public long Solve(string puzzle, ref StringHandler solution, int limit)
	{
		fixed (char* p = puzzle)
		{
			char* solutionStr = stackalloc char[BufferLength];
			long result = InternalSolve(p, solutionStr, limit);

			_ = solution.ToStringAndClear();
			solution = new(BufferLength - 1);
			solution.Append(solutionStr, BufferLength - 1);

			return result;
		}
	}

	/// <summary>
	/// Same as <see cref="CheckValidity(string, out string?)"/>, but doesn't contain
	/// any <see langword="out"/> parameters.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <returns>The <see cref="bool"/> result. <see langword="true"/> for unique solution.</returns>
	/// <seealso cref="CheckValidity(string, out string?)"/>
	public bool CheckValidity(string grid)
	{
		fixed (char* puzzle = grid)
		{
			return InternalSolve(puzzle, null, 2) == 1;
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
		fixed (char* puzzle = grid)
		{
			char* result = stackalloc char[BufferLength];
			if (InternalSolve(puzzle, result, 2) == 1)
			{
				solutionIfUnique = new Span<char>(result, BufferLength).ToString();
				return true;
			}
			else
			{
				solutionIfUnique = null;
				return false;
			}
		}
	}

	/// <summary>
	/// Set a cell as solved - used in <see cref="InitSudoku"/>.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="digit">The digit.</param>
	/// <returns>The <see cref="bool"/> result.</returns>
	private bool SetSolvedDigit(int cell, int digit)
	{
		int subBand = CellToSubBand[cell];
		int band = DigitToBaseBand[digit] + subBand;
		int mask = CellToMask[cell];
		if ((_g->Bands[band] & mask) == 0)
		{
			return false;
		}

		_g->Bands[band] &= TblSelfMask[cell];
		int tblMask = TblOtherMask[cell];
		_g->Bands[TblAnother1[band]] &= (uint)tblMask;
		_g->Bands[TblAnother2[band]] &= (uint)tblMask;
		mask = ~mask;
		_g->UnsolvedCells[subBand] &= (uint)mask;
		int rowBit = digit * 9 + CellToRow[cell];
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
	private bool EliminateDigit(int cell, int digit)
	{
		int subBand = CellToSubBand[cell];
		int band = DigitToBaseBand[digit] + subBand;
		int mask = CellToMask[cell];
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

		int subBand = Mod3[band];
		int cell = subBand * 27 + BitPos(mask);
		_g->Bands[band] &= TblSelfMask[cell];
		return true;
	}

	/// <summary>
	/// Setup everything and load the puzzle.
	/// </summary>
	/// <param name="board">The puzzle string.</param>
	/// <returns>The <see cref="bool"/> result.</returns>
	private bool InitSudoku(char* board)
	{
		fixed (State* g = _stack)
		{
			_numSolutions = 0;
			for (int band = 0; band < 27; band++)
			{
				g->Bands[band] = BitSet27;
			}

			Unsafe.InitBlock(g->PrevBands, 0, 108); // 108: sizeof(_g->PrevBands)
			g->UnsolvedCells[0] = g->UnsolvedCells[1] = g->UnsolvedCells[2] = BitSet27;
			g->UnsolvedRows[0] = g->UnsolvedRows[1] = g->UnsolvedRows[2] = BitSet27;
			g->Pairs[0] = g->Pairs[1] = g->Pairs[2] = 0;

			_g = g;
		}

		switch (PointerMarshal.StringLengthOf(board))
		{
			case 81:
			{
				for (int cell = 0; cell < 81; cell++, board++)
				{
					if (*board is > '0' and <= '9')
					{
						if (!SetSolvedDigit(cell, *board - '1'))
						{
							return false;
						}
					}
					else if (*board == 0)
					{
						// End of string before end of puzzle!
						return false;
					}
				}

				return true;
			}
			case 729:
			{
				for (int cell = 0; cell < 81; cell++)
				{
					short mask = 0;
					for (int digit = 0; digit < 9; digit++, board++)
					{
						if (*board == '0')
						{
							mask |= (short)(1 << digit);
						}
					}

					for (int digit = 0, temp = mask; digit < 9; digit++, temp >>= 1)
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
				uint ar = _g->UnsolvedRows[0];  // valid for Digits 0,1,2
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
				uint ar = _g->UnsolvedRows[1];  // valid for Digits 3,4,5
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
				uint ar = _g->UnsolvedRows[2];  // valid for Digits 6,7,8
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

		// The core Update routine from zhouyundong.
		// This copy has been optimized by champagne and JasonLion in minor ways.
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		bool updn(uint* s, uint i, uint j, uint k, uint l)
		{
			a = _g->Bands[i * 3 + j];
			shrink = (uint)(TblShrinkMask[a & 0x1FF] | TblShrinkMask[(a >> 9) & 0x1FF] << 3 | TblShrinkMask[a >> 18] << 6);
			if ((a &= (uint)TblComplexMask[shrink]) == 0)
			{
				return false;
			}

			b = _g->Bands[i * 3 + k];
			c = _g->Bands[i * 3 + l];
			*s = (a | a >> 9 | a >> 18) & 0x1FF;
			_g->Bands[i * 3 + l] &= (uint)TblMaskSingle[*s];
			_g->Bands[i * 3 + k] &= (uint)TblMaskSingle[*s];
			*s = TblRowUniq[TblShrinkSingle[shrink] & TblColumnSingle[*s]];
			_g->PrevBands[i * 3 + j] = _g->Bands[i * 3 + j] = a;

			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void upwcl(uint s, uint i, uint p, uint q, uint r, uint t, uint u, uint v, uint w, uint x, uint a)
		{
			cl = ~(a & TblRowMask[s]);
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
		for (int subBand = 0; subBand < 3; subBand++)
		{
			// Loop unrolling really helps.
			uint r1 = _g->Bands[subBand];           // r1 - Cells in band with pencil one or more times.
			uint bandData = _g->Bands[subBand + 3]; // bandData - Hint to save value in register.
			uint r2 = r1 & bandData;                // r2 - Pencil mark in cell two or more times.
			r1 |= bandData;
			bandData = _g->Bands[subBand + 6];
			uint r3 = r2 & bandData;                // r3 - Pencil mark in cell three or more times.
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
				uint bit = r1 & (uint)-(int)r1;     // Process once cell at a time.
				r1 ^= bit;
				int digit;
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
		for (int cell = 0; cell < 81; cell++)
		{
			int mask = CellToMask[cell];
			int offset = CellToSubBand[cell];
			for (int digit = 0; digit < 9; digit++)
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
		for (int subBand = 0; subBand < 3; subBand++)
		{
			uint map = _g->Pairs[subBand];
			if (map != 0)
			{
				map &= (uint)-(int)map;
				int tries = 2;
				int band = subBand;
				for (int digit = 0; digit < 9; digit++, band += 3)
				{
					if ((_g->Bands[band] & map) != 0)
					{
						if (--tries != 0)
						{
							// First of pair.
							Unsafe.CopyBlock(_g + 1, _g, (uint)sizeof(State));
							_g->Bands[band] ^= map;
							_g++;
							SetSolvedMask(band, map);
							if (FullUpdate() != 0) Guess();
							_g--;
						}
						else
						{
							// Second of pair.
							SetSolvedMask(band, map);
							if (FullUpdate() != 0) Guess();
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
		for (int subBand = 0; subBand < 3; subBand++)
		{
			if (_g->UnsolvedCells[subBand] == 0) continue;
			uint cellMask = _g->UnsolvedCells[subBand];
			cellMask &= (uint)-(int)cellMask;
			int band = subBand;
			for (int digit = 0; digit < 9; digit++, band += 3)
			{
				if ((_g->Bands[band] & cellMask) != 0)
				{
					// Eliminate option in the current stack entry.
					Unsafe.CopyBlock(_g + 1, _g, (uint)sizeof(State));
					_g->Bands[band] ^= cellMask;
					_g++;
					SetSolvedMask(band, cellMask); // And try it out in a nested stack entry.
					if (FullUpdate() != 0) Guess();
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
	/// <returns>An <see cref="byte"/> result.</returns>
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
}
