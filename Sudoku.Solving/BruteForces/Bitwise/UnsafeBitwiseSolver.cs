#pragma warning disable CA1401

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Sudoku.Data;
using static System.Runtime.InteropServices.CallingConvention;

namespace Sudoku.Solving.BruteForces.Bitwise
{
	/// <summary>
	/// Define an unsafe bitwise solver.
	/// </summary>
	public sealed unsafe partial class UnsafeBitwiseSolver : Solver
	{
		/// <summary>
		/// All pencil marks set - 27 bits per band.
		/// </summary>
		private const int BitSet27 = 0x7FFFFFF;


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
		private bool _singleApplied;

		/// <summary>
		/// Pointer to where to store the first solution. This value can be <see langword="null"/>.
		/// </summary>
		private char* _solution;


		/// <inheritdoc/>
		public override string SolverName => "Bitwise (Unsafe)";


		/// <inheritdoc/>
		public override AnalysisResult Solve(IReadOnlyGrid grid)
		{
			const int bufferLength = 82;
			var stopwatch = new Stopwatch();

			string puzzle = grid.ToString("0");
			fixed (char* p = puzzle)
			{
				char* solutionStr = stackalloc char[82];
				for (int i = 0; i < 81; i++)
				{
					solutionStr[i] = '0';
				}
				//solutionStr[81] = '\0';

				stopwatch.Start();
				s(in p, solutionStr, 2);
				stopwatch.Stop();

				return _numSolutions switch
				{
					0 => throw new NoSolutionException(grid),
					1 => new
					(
						solverName: SolverName,
						puzzle: grid,
						solution: Grid.Parse(new Span<char>(solutionStr, bufferLength).ToString()),
						hasSolved: true,
						elapsedTime: stopwatch.Elapsed,
						additional: null
					),
					_ => throw new MultipleSolutionsException(grid)
				};
			}

			// Core function to solve the puzzle.
			long s(in char* puzzle, char* solutionPtr, int limit)
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
			if ((_g->bands[band] & mask) == 0)
			{
				return false;
			}

			_g->bands[band] &= TblSelfMask[cell];
			int tblMask = TblOtherMask[cell];
			_g->bands[TblAnother1[band]] &= tblMask;
			_g->bands[TblAnother2[band]] &= tblMask;
			mask = ~mask;
			_g->unsolvedCells[subBand] &= mask;
			int rowBit = digit * 9 + CellToRow[cell];
			_g->unsolvedRows[rowBit / 27] &= ~(1 << (Mod27[rowBit]));
			_g->bands[subBand] &= mask;
			_g->bands[subBand + 3] &= mask;
			_g->bands[subBand + 6] &= mask;
			_g->bands[subBand + 9] &= mask;
			_g->bands[subBand + 12] &= mask;
			_g->bands[subBand + 15] &= mask;
			_g->bands[subBand + 18] &= mask;
			_g->bands[subBand + 21] &= mask;
			_g->bands[subBand + 24] &= mask;
			_g->bands[band] |= (long)~mask;
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
			if ((_g->bands[band] & mask) == 0)
			{
				return true;
			}

			_g->bands[band] &= ~mask;
			return true;
		}

		/// <summary>
		/// Set a cell as solved - used in various guess routines.
		/// </summary>
		/// <param name="band">The band.</param>
		/// <param name="mask">The mask.</param>
		/// <returns>The <see cref="bool"/> result.</returns>
		private bool SetSolvedMask(int band, long mask)
		{
			if ((_g->bands[band] & mask) == 0)
			{
				return false;
			}

			int subBand = Mod3[band];
			int cell = subBand * 27 + BitPos(mask);
			_g->bands[band] &= TblSelfMask[cell];
#if false
			_g->bands[TblAnother1[band]] &= TblOtherMask[cell];
			_g->bands[TblAnother2[band]] &= TblOtherMask[cell];
			mask = ~mask;
			_g->unsolvedCells[subBand] &= mask;
			int rowBit = (band / 3) * 9 + CellToRow[cell];
			_g->unsolvedRows[rowBit / 27] &= ~(1 << (Mod27[rowBit]));
			for (int indx = subBand; indx < 27; indx += 3)
			{
				if (indx == band) continue;
				_g->bands[indx] &= mask;
			}
#endif
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
				try
				{
					static void @for(State* g, int start, int end)
					{
						for (int band = start; band < end; band++)
						{
							g->bands[band] = BitSet27;
						}
					}

					_numSolutions = 0;
					@for(g, 0, 27);
					Memset(g->prevBands, 0, sizeof(long)); // sizeof(_g->prevBands)
					g->unsolvedCells[0] = g->unsolvedCells[1] = g->unsolvedCells[2] = BitSet27;
					g->unsolvedRows[0] = g->unsolvedRows[1] = g->unsolvedRows[2] = BitSet27;
					g->pairs[0] = g->pairs[1] = g->pairs[2] = 0;
				}
				finally
				{
					_g = g;
				}
			}

			switch (Strlen(board))
			{
				case 81:
				{
					for (int cell = 0; cell < 81; ++cell, ++board)
					{
						if (char.IsDigit(*board) && *board != '0')
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
					for (int cell = 0; cell < 81; ++cell)
					{
						short mask = 0;
						for (int digit = 0; digit < 9; ++digit, ++board)
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
			long shrink = 1, s = default, a, b, c, cl;
			while (shrink != 0)
			{
				shrink = 0;
				if (_g->unsolvedRows[0] == 0) goto Digit3;
				{
					long ar = _g->unsolvedRows[0];  // valid for Digits 0,1,2
					if ((ar & 0x1FF) == 0) goto Digit1;
					if (_g->bands[0 * 3 + 0] == _g->prevBands[0 * 3 + 0]) goto Digit0b;
					if (updn(0, 0, 1, 2)) return false;
					if ((ar & 7) != s)
					{
						ar &= 0x7FFFFF8 | s;
						upwcl(0, 3, 6, 9, 12, 15, 18, 21, 24);
					}
				Digit0b:
					if (_g->bands[0 * 3 + 1] == _g->prevBands[0 * 3 + 1]) goto Digit0c;
					if (updn(0, 1, 0, 2)) return false;
					if (((ar >> 3) & 7) != s)
					{
						ar &= 0x7FFFFC7 | (s << 3);
						upwcl(1, 4, 7, 10, 13, 16, 19, 22, 25);
					}
				Digit0c:
					if (_g->bands[0 * 3 + 2] == _g->prevBands[0 * 3 + 2]) goto Digit1;
					if (updn(0, 2, 0, 1)) return false;
					if (((ar >> 6) & 7) != s)
					{
						ar &= 0x7FFFE3F | (s << 6);
						upwcl(2, 5, 8, 11, 14, 17, 20, 23, 26);
					}
				Digit1:
					if (((ar >> 9) & 0x1FF) == 0) goto Digit2;
					if (_g->bands[1 * 3 + 0] == _g->prevBands[1 * 3 + 0]) goto Digit1b;
					if (updn(1, 0, 1, 2)) return false;
					if (((ar >> 9) & 7) != s)
					{
						ar &= 0x7FFF1FF | (s << 9);
						upwcl(0, 0, 6, 9, 12, 15, 18, 21, 24);
					}
				Digit1b:
					if (_g->bands[1 * 3 + 1] == _g->prevBands[1 * 3 + 1]) goto Digit1c;
					if (updn(1, 1, 0, 2)) return false;
					if (((ar >> 12) & 7) != s)
					{
						ar &= 0x7FF8FFF | (s << 12);
						upwcl(1, 1, 7, 10, 13, 16, 19, 22, 25);
					}
				Digit1c:
					if (_g->bands[1 * 3 + 2] == _g->prevBands[1 * 3 + 2]) goto Digit2;
					if (updn(1, 2, 0, 1)) return false;
					if (((ar >> 15) & 7) != s)
					{
						ar &= 0x7FC7FFF | (s << 15);
						upwcl(2, 2, 8, 11, 14, 17, 20, 23, 26);
					}
				Digit2:
					if (((ar >> 18) & 0x1FF) == 0) goto End012;
					if (_g->bands[2 * 3 + 0] == _g->prevBands[2 * 3 + 0]) goto Digit2b;
					if (updn(2, 0, 1, 2)) return false;
					if (((ar >> 18) & 7) != s)
					{
						ar &= 0x7E3FFFF | (s << 18);
						upwcl(0, 0, 3, 9, 12, 15, 18, 21, 24);
					}
				Digit2b:
					if (_g->bands[2 * 3 + 1] == _g->prevBands[2 * 3 + 1]) goto Digit2c;
					if (updn(2, 1, 0, 2)) return false;
					if (((ar >> 21) & 7) != s)
					{
						ar &= 0x71FFFFF | (s << 21);
						upwcl(1, 1, 4, 10, 13, 16, 19, 22, 25);
					}
				Digit2c:
					if (_g->bands[2 * 3 + 2] == _g->prevBands[2 * 3 + 2]) goto End012;
					if (updn(2, 2, 0, 1)) return false;
					if (((ar >> 24) & 7) != s)
					{
						ar &= 0xFFFFFF | (s << 24);
						upwcl(2, 2, 5, 11, 14, 17, 20, 23, 26);
					}
				End012:
					_g->unsolvedRows[0] = ar;
				}
			Digit3:
				if (_g->unsolvedRows[1] == 0) goto Digit6;
				{
					long ar = _g->unsolvedRows[1];  // valid for Digits 3,4,5
					if ((ar & 0x1FF) == 0) goto Digit4;
					if (_g->bands[3 * 3 + 0] == _g->prevBands[3 * 3 + 0]) goto Digit3b;
					if (updn(3, 0, 1, 2)) return false;
					if ((ar & 7) != s)
					{
						ar &= 0x7FFFFF8 | s;
						upwcl(0, 0, 3, 6, 12, 15, 18, 21, 24);
					}
				Digit3b:
					if (_g->bands[3 * 3 + 1] == _g->prevBands[3 * 3 + 1]) goto Digit3c;
					if (updn(3, 1, 0, 2)) return false;
					if (((ar >> 3) & 7) != s)
					{
						ar &= 0x7FFFFC7 | (s << 3);
						upwcl(1, 1, 4, 7, 13, 16, 19, 22, 25);
					}
				Digit3c:
					if (_g->bands[3 * 3 + 2] == _g->prevBands[3 * 3 + 2]) goto Digit4;
					if (updn(3, 2, 0, 1)) return false;
					if (((ar >> 6) & 7) != s)
					{
						ar &= 0x7FFFE3F | (s << 6);
						upwcl(2, 2, 5, 8, 14, 17, 20, 23, 26);
					}
				Digit4:
					if (((ar >> 9) & 0x1FF) == 0) goto Digit5;
					if (_g->bands[4 * 3 + 0] == _g->prevBands[4 * 3 + 0]) goto Digit4b;
					if (updn(4, 0, 1, 2)) return false;
					if (((ar >> 9) & 7) != s)
					{
						ar &= 0x7FFF1FF | (s << 9);
						upwcl(0, 0, 3, 6, 9, 15, 18, 21, 24);
					}
				Digit4b:
					if (_g->bands[4 * 3 + 1] == _g->prevBands[4 * 3 + 1]) goto Digit4c;
					if (updn(4, 1, 0, 2)) return false;
					if (((ar >> 12) & 7) != s)
					{
						ar &= 0x7FF8FFF | (s << 12);
						upwcl(1, 1, 4, 7, 10, 16, 19, 22, 25);
					}
				Digit4c:
					if (_g->bands[4 * 3 + 2] == _g->prevBands[4 * 3 + 2]) goto Digit5;
					if (updn(4, 2, 0, 1)) return false;
					if (((ar >> 15) & 7) != s)
					{
						ar &= 0x7FC7FFF | (s << 15);
						upwcl(2, 2, 5, 8, 11, 17, 20, 23, 26);
					}
				Digit5:
					if (((ar >> 18) & 0x1FF) == 0) goto End345;
					if (_g->bands[5 * 3 + 0] == _g->prevBands[5 * 3 + 0]) goto Digit5b;
					if (updn(5, 0, 1, 2)) return false;
					if (((ar >> 18) & 7) != s)
					{
						ar &= 0x7E3FFFF | (s << 18);
						upwcl(0, 0, 3, 6, 9, 12, 18, 21, 24);
					}
				Digit5b:
					if (_g->bands[5 * 3 + 1] == _g->prevBands[5 * 3 + 1]) goto Digit5c;
					if (updn(5, 1, 0, 2)) return false;
					if (((ar >> 21) & 7) != s)
					{
						ar &= 0x71FFFFF | (s << 21);
						upwcl(1, 1, 4, 7, 10, 13, 19, 22, 25);
					}
				Digit5c:
					if (_g->bands[5 * 3 + 2] == _g->prevBands[5 * 3 + 2]) goto End345;
					if (updn(5, 2, 0, 1)) return false;
					if (((ar >> 24) & 7) != s)
					{
						ar &= 0xFFFFFF | (s << 24);
						upwcl(2, 2, 5, 8, 11, 14, 20, 23, 26);
					}
				End345:
					_g->unsolvedRows[1] = ar;
				}
			Digit6:
				if (_g->unsolvedRows[2] == 0) continue;
				{
					long ar = _g->unsolvedRows[2];  // valid for Digits 6,7,8
					if ((ar & 0x1FF) == 0) goto Digit7;
					if (_g->bands[6 * 3 + 0] == _g->prevBands[6 * 3 + 0]) goto Digit6b;
					if (updn(6, 0, 1, 2)) return false;
					if ((ar & 7) != s)
					{
						ar &= 0x7FFFFF8 | s;
						upwcl(0, 0, 3, 6, 9, 12, 15, 21, 24);
					}
				Digit6b:
					if (_g->bands[6 * 3 + 1] == _g->prevBands[6 * 3 + 1]) goto Digit6c;
					if (updn(6, 1, 0, 2)) return false;
					if (((ar >> 3) & 7) != s)
					{
						ar &= 0x7FFFFC7 | (s << 3);
						upwcl(1, 1, 4, 7, 10, 13, 16, 22, 25);
					}
				Digit6c:
					if (_g->bands[6 * 3 + 2] == _g->prevBands[6 * 3 + 2]) goto Digit7;
					if (updn(6, 2, 0, 1)) return false;
					if (((ar >> 6) & 7) != s)
					{
						ar &= 0x7FFFE3F | (s << 6);
						upwcl(2, 2, 5, 8, 11, 14, 17, 23, 26);
					}
				Digit7:
					if (((ar >> 9) & 0x1FF) == 0) goto Digit8;
					if (_g->bands[7 * 3 + 0] == _g->prevBands[7 * 3 + 0]) goto Digit7b;
					if (updn(7, 0, 1, 2)) return false;
					if (((ar >> 9) & 7) != s)
					{
						ar &= 0x7FFF1FF | (s << 9);
						upwcl(0, 0, 3, 6, 9, 12, 15, 18, 24);
					}
				Digit7b:
					if (_g->bands[7 * 3 + 1] == _g->prevBands[7 * 3 + 1]) goto Digit7c;
					if (updn(7, 1, 0, 2)) return false;
					if (((ar >> 12) & 7) != s)
					{
						ar &= 0x7FF8FFF | (s << 12);
						upwcl(1, 1, 4, 7, 10, 13, 16, 19, 25);
					}
				Digit7c:
					if (_g->bands[7 * 3 + 2] == _g->prevBands[7 * 3 + 2]) goto Digit8;
					if (updn(7, 2, 0, 1)) return false;
					if (((ar >> 15) & 7) != s)
					{
						ar &= 0x7FC7FFF | (s << 15);
						upwcl(2, 2, 5, 8, 11, 14, 17, 20, 26);
					}
				Digit8:
					if (((ar >> 18) & 0x1FF) == 0) goto End678;
					if (_g->bands[8 * 3 + 0] == _g->prevBands[8 * 3 + 0]) goto Digit8b;
					if (updn(8, 0, 1, 2)) return false;
					if (((ar >> 18) & 7) != s)
					{
						ar &= 0x7E3FFFF | (s << 18);
						upwcl(0, 0, 3, 6, 9, 12, 15, 18, 21);
					}
				Digit8b:
					if (_g->bands[8 * 3 + 1] == _g->prevBands[8 * 3 + 1]) goto Digit8c;
					if (updn(8, 1, 0, 2)) return false;
					if (((ar >> 21) & 7) != s)
					{
						ar &= 0x71FFFFF | (s << 21);
						upwcl(1, 1, 4, 7, 10, 13, 16, 19, 22);
					}
				Digit8c:
					if (_g->bands[8 * 3 + 2] == _g->prevBands[8 * 3 + 2]) goto End678;
					if (updn(8, 2, 0, 1)) return false;
					if (((ar >> 24) & 7) != s)
					{
						ar &= 0xFFFFFF | (s << 24);
						upwcl(2, 2, 5, 8, 11, 14, 17, 20, 23);
					}
				End678:
					_g->unsolvedRows[2] = ar;
				}
			}
			return true;

			// The core Update routine from zhouyundong.
			// This copy has been optimized by champagne and JasonLion in minor ways.
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			bool updn(int i, int j, int k, int l)
			{
				a = _g->bands[i * 3 + j];
				shrink = TblShrinkMask[a & 0x1FF] | TblShrinkMask[(a >> 9) & 0x1FF] << 3 | TblShrinkMask[a >> 18] << 6;
				if ((a &= TblComplexMask[shrink]) == 0)
				{
					return false;
				}

				b = _g->bands[i * 3 + k];
				c = _g->bands[i * 3 + l];
				s = (a | a >> 9 | a >> 18) & 0x1FF;
				_g->bands[i * 3 + l] &= TblMaskSingle[s];
				_g->bands[i * 3 + k] &= TblMaskSingle[s];
				s = TblRowUniq[TblShrinkSingle[shrink] & TblColumnSingle[s]];
				_g->prevBands[i * 3 + j] = _g->bands[i * 3 + j] = a;

				return true;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			void upwcl(int i, int p, int q, int r, int t, int u, int v, int w, int x)
			{
				cl = ~(a & TblRowMask[s]);
				_g->unsolvedCells[i] &= cl;
				_g->bands[p] &= cl;
				_g->bands[q] &= cl;
				_g->bands[r] &= cl;
				_g->bands[t] &= cl;
				_g->bands[u] &= cl;
				_g->bands[v] &= cl;
				_g->bands[w] &= cl;
				_g->bands[x] &= cl;
			}
		}

		/// <summary>
		/// Find singles, bi-value cells, and impossible cells.
		/// </summary>
		/// <returns>A <see cref="bool"/> result.</returns>
		private bool ApplySingleOrEmptyCells()
		{
			_singleApplied = false;
			for (int subBand = 0; subBand < 3; ++subBand)
			{
#if false
				uint r1 = 0, r2 = 0, r3 = 0;
				for (int band = subBand; band < 27; band += 3)
				{
					unsigned int bandData = g->bands[band];
					r3 |= r2 & bandData;
					r2 |= r1 & bandData;
					r1 |= bandData;
				}
#else

				// Loop unrolling really helps.
				long r1 = _g->bands[subBand];           // r1 - Cells in band with pencil one or more times.
				long bandData = _g->bands[subBand + 3]; // bandData - Hint to save value in register.
				long r2 = r1 & bandData;                // r2 - Pencil mark in cell two or more times.
				r1 |= bandData;
				bandData = _g->bands[subBand + 6];
				long r3 = r2 & bandData;                // r3 - Pencil mark in cell three or more times.
				r2 |= r1 & bandData;
				r1 |= bandData;
				bandData = _g->bands[subBand + 9];
				r3 |= r2 & bandData;
				r2 |= r1 & bandData;
				r1 |= bandData;
				bandData = _g->bands[subBand + 12];
				r3 |= r2 & bandData;
				r2 |= r1 & bandData;
				r1 |= bandData;
				bandData = _g->bands[subBand + 15];
				r3 |= r2 & bandData;
				r2 |= r1 & bandData;
				r1 |= bandData;
				bandData = _g->bands[subBand + 18];
				r3 |= r2 & bandData;
				r2 |= r1 & bandData;
				r1 |= bandData;
				bandData = _g->bands[subBand + 21];
				r3 |= r2 & bandData;
				r2 |= r1 & bandData;
				r1 |= bandData;
				bandData = _g->bands[subBand + 24];
				r3 |= r2 & bandData;
				r2 |= r1 & bandData;
				r1 |= bandData;
#endif
				if (r1 != BitSet27)
				{
					// Something is locked, can't be solved.
					return true;
				}

				_g->pairs[subBand] = r2 & ~r3;          // Exactly two pencil marks in cell.
				r1 &= ~r2;                              // Exactly one pencil mark in cell.
				r1 &= _g->unsolvedCells[subBand];       // Ignore already solved cells.
				while (r1 != 0)
				{
					// Set all the single pencil mark cells
					_singleApplied = true;
					long bit = r1 & -(int)r1;           // Process once cell at a time.
					r1 ^= bit;
					int digit;
					for (digit = 0; digit < 9; ++digit)
					{
						// Requires finding which digit they are for
						if ((_g->bands[digit * 3 + subBand] & bit) != 0)
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
		/// Extract solution as a character string.
		/// </summary>
		/// <param name="solution">
		/// The solution pointer. <b>The buffer should be at least 82 of length.</b>
		/// </param>
		private void ExtractSolution(char* solution)
		{
			for (int cell = 0; cell < 81; ++cell)
			{
				int mask = CellToMask[cell];
				int offset = CellToSubBand[cell];
				for (int digit = 0; digit < 9; ++digit)
				{
					if ((_g->bands[offset] & mask) != 0)
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
			for (int subBand = 0; subBand < 3; ++subBand)
			{
				long map = _g->pairs[subBand];
				if (map != 0)
				{
					map &= -(int)map;
					int tries = 2;
					int band = subBand;
					for (int digit = 0; digit < 9; ++digit, band += 3)
					{
						if ((_g->bands[band] & map) != 0)
						{
							if (--tries != 0)
							{
								// First of pair
								MemCopy(_g + 1, _g, sizeof(State));
								_g->bands[band] ^= map;
								++_g;
								SetSolvedMask(band, map);
								if (FullUpdate() != 0) Guess();
								--_g;
							}
							else
							{
								// Second of pair
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
			// Kind of dumb, but _way_ fast code
			for (int subBand = 0; subBand < 3; ++subBand)
			{
				if (_g->unsolvedCells[subBand] == 0) continue;
				long cellMask = _g->unsolvedCells[subBand];
				cellMask &= -(int)cellMask;
				int band = subBand;
				for (int digit = 0; digit < 9; ++digit, band += 3)
				{
					if ((_g->bands[band] & cellMask) != 0)
					{
						MemCopy(_g + 1, _g, sizeof(State)); // Eliminate option in the current stack entry.
						_g->bands[band] ^= cellMask;
						++_g;
						SetSolvedMask(band, cellMask);      // And try it out in a nested stack entry.
						if (FullUpdate() != 0) Guess();
						--_g;
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
			if ((_g->unsolvedRows[0] | _g->unsolvedRows[1] | _g->unsolvedRows[2]) == 0)
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

				if ((_g->unsolvedCells[0] | _g->unsolvedCells[1] | _g->unsolvedCells[2]) == 0)
				{
					return 2;
				}

				// locked empty cell or conflict singles in cells
				if (ApplySingleOrEmptyCells())
				{
					return 0;
				}

				// Found a single, run Update again
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
		private static byte BitPos(long map) => MultiplyDeBruijnBitPosition32[map * 0x077CB531 >> 27];

		/// <summary>
		/// Get the length of the specified string which is represented by a <see cref="char"/>*.
		/// </summary>
		/// <param name="ptr">The pointer.</param>
		/// <returns>The total length.</returns>
		/// <remarks>
		/// In C#, this function is unsafe because the implementation of
		/// <see cref="string"/> types between C and C# is totally different.
		/// In C, <see cref="string"/> is like a <see cref="char"/>* or a
		/// <see cref="char"/>[], they ends with the terminator symbol <c>'\0'</c>.
		/// However, C# not.
		/// </remarks>
		private static int Strlen(char* ptr)
		{
			int result = 0;
			for (char* p = ptr; *p != '\0'; p++, result++) ;
			return result;
		}

		/// <summary>
		/// Function <c>memset</c> in C.
		/// </summary>
		/// <param name="dest">The destination pointer.</param>
		/// <param name="c">The start offset.</param>
		/// <param name="count">The size of the unit.</param>
		/// <returns>The pointer.</returns>
		[DllImport("msvcrt.dll", EntryPoint = "memset", CallingConvention = Cdecl, SetLastError = false)]
		private static extern void* Memset([Out] void* dest, [In] nint c, [In] nint count);

		/// <summary>
		/// Function <c>memcpy</c> in C.
		/// </summary>
		/// <param name="dest">The destination pointer.</param>
		/// <param name="src">The source pointer.</param>
		/// <param name="count">The number of unit you want to copy.</param>
		[DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = Cdecl, SetLastError = false)]
		private static extern void MemCopy([Out] void* dest, [In] void* src, [In] int count);
	}
}
