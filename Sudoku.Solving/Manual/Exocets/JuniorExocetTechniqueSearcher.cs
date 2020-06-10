using System;
using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using Sudoku.Solving.Manual.Exocets.Eliminations;
using static Sudoku.Constants.Processings;
using static Sudoku.Constants.RegionLabel;
using static Sudoku.Data.CellStatus;
using static Sudoku.Data.ConclusionType;
using static Sudoku.Data.GridMap.InitializeOption;

namespace Sudoku.Solving.Manual.Exocets
{
	/// <summary>
	/// Encapsulates a <b>junior exocet</b> (JE) technique searcher.
	/// </summary>
	[TechniqueDisplay("Junior Exocet")]
	public sealed class JuniorExocetTechniqueSearcher : ExocetTechniqueSearcher
	{
		/// <summary>
		/// The iterator for Bi-bi pattern.
		/// </summary>
		private static readonly int[,] BibiIter =
		{
			{4, 5, 7, 8}, {3, 5, 6, 8}, {3, 4, 6, 7},
			{1, 2, 7, 8}, {0, 2, 6, 8}, {0, 1, 6, 7},
			{1, 2, 4, 5}, {0, 2, 3, 5}, {0, 1, 3, 4}
		};


		/// <inheritdoc/>
		public JuniorExocetTechniqueSearcher(bool checkAdvanced) : base(checkAdvanced)
		{
		}


		/// <summary>
		/// Indicates the priority of this technique.
		/// </summary>
		public static int Priority { get; set; } = 94;

		/// <summary>
		/// Indicates whether the technique is enabled.
		/// </summary>
		public static bool IsEnabled { get; set; } = true;


		/// <inheritdoc/>
		public override void GetAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			foreach (var exocet in Patterns)
			{
				var (baseMap, targetMap, _) = exocet;
				var (b1, b2, tq1, tq2, tr1, tr2, s, mq1, mq2, mr1, mr2) = exocet;

				// The base cells cannot be given or modifiable.
				if ((baseMap - EmptyMap).IsNotEmpty)
				{
					continue;
				}

				// The number of different candidates in base cells cannot be greater than 5.
				short baseCandidatesMask = (short)(grid.GetCandidateMask(b1) | grid.GetCandidateMask(b2));
				if (baseCandidatesMask.CountSet() > 5)
				{
					continue;
				}

				// At least one cell should be empty.
				if (!targetMap.Overlaps(EmptyMap))
				{
					continue;
				}

				// Then check target eliminations.
				// Here 'nonBaseQ' and 'nonBaseR' are the conjugate pair in target Q and target R cells pair,
				// different with 'lockedMemberQ' and 'lockedMemberR'.
				if (!CheckTarget(grid, tq1, tq2, baseCandidatesMask, out short nonBaseQ, out _)
					|| !CheckTarget(grid, tr1, tr2, baseCandidatesMask, out short nonBaseR, out _))
				{
					continue;
				}

				// Get all locked members.
				int v1 = grid.GetCandidateMask(mq1.SetAt(0)) | grid.GetCandidateMask(mq1.SetAt(1));
				int v2 = grid.GetCandidateMask(mq2.SetAt(0)) | grid.GetCandidateMask(mq2.SetAt(1));
				short temp = (short)(v1 | v2);
				short needChecking = (short)(baseCandidatesMask & temp);
				short lockedMemberQ = (short)(baseCandidatesMask & ~needChecking);

				v1 = grid.GetCandidateMask(mr1.SetAt(0)) | grid.GetCandidateMask(mr1.SetAt(1));
				v2 = grid.GetCandidateMask(mr2.SetAt(0)) | grid.GetCandidateMask(mr2.SetAt(1));
				temp = (short)(v1 | v2);
				needChecking &= temp;
				short lockedMemberR = (short)(baseCandidatesMask & ~(baseCandidatesMask & temp));

				// Check crossline.
				if (!CheckCrossline(s, needChecking))
				{
					continue;
				}

				// Gather highlight cells and candidates.
				var cellOffsets = new List<(int, int)> { (0, b1), (0, b2) };
				var candidateOffsets = new List<(int, int)>();
				foreach (int digit in grid.GetCandidates(b1))
				{
					candidateOffsets.Add((0, b1 * 9 + digit));
				}
				foreach (int digit in grid.GetCandidates(b2))
				{
					candidateOffsets.Add((0, b2 * 9 + digit));
				}

				// Check target eliminations.
				// '|' first, '&&' second. (Do you know my meaning?)
				TargetEliminations targetElims = default;
				temp = (short)(nonBaseQ > 0 ? baseCandidatesMask | nonBaseQ : baseCandidatesMask);
				if (GatheringTargetEliminations(tq1, grid, baseCandidatesMask, temp, ref targetElims)
					| GatheringTargetEliminations(tq2, grid, baseCandidatesMask, temp, ref targetElims)
					&& nonBaseQ != 0 && grid.GetStatus(tq1) == Empty && grid.GetStatus(tq2) == Empty)
				{
					int conjugatPairDigit = nonBaseQ.FindFirstSet();
					if (grid.Exists(tq1, conjugatPairDigit) is true)
					{
						candidateOffsets.Add((1, tq1 * 9 + conjugatPairDigit));
					}
					if (grid.Exists(tq2, conjugatPairDigit) is true)
					{
						candidateOffsets.Add((1, tq2 * 9 + conjugatPairDigit));
					}
				}

				temp = (short)(nonBaseR > 0 ? baseCandidatesMask | nonBaseR : baseCandidatesMask);
				if (GatheringTargetEliminations(tr1, grid, baseCandidatesMask, temp, ref targetElims)
					| GatheringTargetEliminations(tr2, grid, baseCandidatesMask, temp, ref targetElims)
					&& nonBaseR != 0 && grid.GetStatus(tr1) == Empty && grid.GetStatus(tr2) == Empty)
				{
					int conjugatPairDigit = nonBaseR.FindFirstSet();
					if (grid.Exists(tr1, conjugatPairDigit) is true)
					{
						candidateOffsets.Add((1, tr1 * 9 + conjugatPairDigit));
					}
					if (grid.Exists(tr2, conjugatPairDigit) is true)
					{
						candidateOffsets.Add((1, tr2 * 9 + conjugatPairDigit));
					}
				}

				bool isRow = baseMap.CoveredLine < 18;
				var (tar1, mir1) =
					GatheringMirrorEliminations(
						tq1, tq2, tr1, tr2, mq1, mq2, nonBaseQ, 0, grid,
						baseCandidatesMask, cellOffsets, candidateOffsets);
				var (tar2, mir2) =
					GatheringMirrorEliminations(
						tr1, tr2, tq1, tq2, mr1, mr2, nonBaseR, 1, grid,
						baseCandidatesMask, cellOffsets, candidateOffsets);
				var targetEliminations = TargetEliminations.MergeAll(targetElims, tar1, tar2);
				var mirrorEliminations = MirrorEliminations.MergeAll(mir1, mir2);
				BibiPatternEliminations bibiEliminations = default;
				TargetPairEliminations targetPairEliminations = default;
				SwordfishEliminations swordfishEliminations = default;
				if (_checkAdvanced && baseCandidatesMask.CountSet() > 2)
				{
					CheckBibiPattern(
						grid, baseCandidatesMask, b1, b2, tq1, tq2, tr1, tr2, s,
						isRow, nonBaseQ, nonBaseR, targetMap, out bibiEliminations,
						out targetPairEliminations, out swordfishEliminations);
				}

				if (_checkAdvanced && targetEliminations.Count == 0 && mirrorEliminations.Count == 0
					&& bibiEliminations.Count == 0 || !_checkAdvanced && targetEliminations.Count == 0)
				{
					continue;
				}

				cellOffsets.Add((1, tq1));
				cellOffsets.Add((1, tq2));
				cellOffsets.Add((1, tr1));
				cellOffsets.Add((1, tr2));
				foreach (int cell in s)
				{
					cellOffsets.Add((2, cell));
				}

				accumulator.Add(
					new JuniorExocetTechniqueInfo(
						conclusions: new List<Conclusion>(),
						views: new[]
						{
							new View(
								cellOffsets,
								candidateOffsets,
								regionOffsets: null,
								links: null)
						},
						exocet,
						digits: baseCandidatesMask.GetAllSets(),
						lockedMemberQ: lockedMemberQ == 0 ? null : lockedMemberQ.GetAllSets(),
						lockedMemberR: lockedMemberR == 0 ? null : lockedMemberR.GetAllSets(),
						targetEliminations,
						mirrorEliminations: _checkAdvanced ? mirrorEliminations : default,
						bibiEliminations,
						targetPairEliminations,
						swordfishEliminations));
			}
		}

		/// <summary>
		/// Gathering mirror eliminations. This method is an entry for the method check mirror in base class.
		/// </summary>
		/// <param name="tq1">The target Q1 cell.</param>
		/// <param name="tq2">The target Q2 cell.</param>
		/// <param name="tr1">The target R1 cell.</param>
		/// <param name="tr2">The target R2 cell.</param>
		/// <param name="m1">The mirror 1 cell.</param>
		/// <param name="m2">The mirror 2 cell.</param>
		/// <param name="lockedNonTarget">The locked digits that is not the target digits.</param>
		/// <param name="x">The X digit.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="baseCandidatesMask">The base candidates mask.</param>
		/// <param name="cellOffsets">The highlight cells.</param>
		/// <param name="candidateOffsets">The highliht candidates.</param>
		/// <returns>The result.</returns>
		private (TargetEliminations, MirrorEliminations) GatheringMirrorEliminations(
			int tq1, int tq2, int tr1, int tr2, GridMap m1, GridMap m2, short lockedNonTarget,
			int x, IReadOnlyGrid grid, short baseCandidatesMask, List<(int, int)> cellOffsets,
			List<(int, int)> candidateOffsets)
		{
			if ((grid.GetCandidateMask(tq1) & baseCandidatesMask) != 0)
			{
				short mask1 = grid.GetCandidateMask(tr1), mask2 = grid.GetCandidateMask(tr2);
				var (target, target2, mirror) = (tq1, tq2, m1);
				return CheckMirror(
					grid, target, target2, lockedNonTarget > 0 ? lockedNonTarget : (short)0,
					baseCandidatesMask, mirror, x,
					(mask1 & baseCandidatesMask) != 0 && (mask2 & baseCandidatesMask) == 0
						? tr1
						: (mask1 & baseCandidatesMask) == 0 && (mask2 & baseCandidatesMask) != 0 ? tr2 : -1,
					cellOffsets, candidateOffsets);
			}
			else if ((grid.GetCandidateMask(tq2) & baseCandidatesMask) != 0)
			{
				short mask1 = grid.GetCandidateMask(tq1), mask2 = grid.GetCandidateMask(tq2);
				var (target, target2, mirror) = (tq2, tq1, m2);
				return CheckMirror(
					grid, target, target2, lockedNonTarget > 0 ? lockedNonTarget : (short)0,
					baseCandidatesMask, mirror, x,
					(mask1 & baseCandidatesMask) != 0 && (mask2 & baseCandidatesMask) == 0
						? tr1
						: (mask1 & baseCandidatesMask) == 0 && (mask2 & baseCandidatesMask) != 0 ? tr2 : -1,
					cellOffsets, candidateOffsets);
			}
			else
			{
				return default;
			}
		}

		/// <summary>
		/// The method for gathering target eliminations.
		/// </summary>
		/// <param name="cell">The cell.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="baseCandidatesMask">The base candidates mask.</param>
		/// <param name="temp">The temp mask.</param>
		/// <param name="targetElims">(<see langword="ref"/> parameter) The target eliminations.</param>
		/// <returns>A <see cref="bool"/> value indicating whether this method has been found eliminations.</returns>
		private static bool GatheringTargetEliminations(
			int cell, IReadOnlyGrid grid, short baseCandidatesMask, short temp, ref TargetEliminations targetElims)
		{
			short candidateMask = (short)(grid.GetCandidateMask(cell) & ~temp);
			if (grid.GetStatus(cell) == Empty && candidateMask != 0
				&& (grid.GetCandidateMask(cell) & baseCandidatesMask) != 0)
			{
				foreach (int digit in candidateMask.GetAllSets())
				{
					targetElims.Add(new Conclusion(Elimination, cell, digit));
				}

				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Check the cross-line cells.
		/// </summary>
		/// <param name="crossline">The cross line cells.</param>
		/// <param name="digitsNeedChecking">The digits that need checking.</param>
		/// <returns>
		/// A <see cref="bool"/> value indicating whether the structure passed the validation.
		/// </returns>
		private bool CheckCrossline(GridMap crossline, short digitsNeedChecking)
		{
			foreach (int digit in digitsNeedChecking.GetAllSets())
			{
				var crosslinePerCandidate = crossline & DigitMaps[digit];
				int r = crosslinePerCandidate.RowMask, c = crosslinePerCandidate.ColumnMask;
				if (r.CountSet() <= 2 || c.CountSet() <= 2)
				{
					continue;
				}

				bool flag = false;
				foreach (int d1 in r.GetAllSets())
				{
					foreach (int d2 in c.GetAllSets())
					{
						if ((crosslinePerCandidate - (RegionMaps[d1 + 9] | RegionMaps[d2 + 18])).IsEmpty)
						{
							flag = true;
							break;
						}
					}

					if (flag)
					{
						break;
					}
				}

				if (!flag)
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Check the target cells.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="pos1">The cell 1 to determine.</param>
		/// <param name="pos2">The cell 2 to determine.</param>
		/// <param name="baseCandidatesMask">The base candidate mask.</param>
		/// <param name="otherCandidatesMask">
		/// (<see langword="out"/> parameter) The other candidate mask.
		/// </param>
		/// <param name="otherRegion">(<see langword="out"/> parameter) The other region.</param>
		/// <returns>The <see cref="bool"/> value.</returns>
		private bool CheckTarget(
			IReadOnlyGrid grid, int pos1, int pos2, int baseCandidatesMask,
			out short otherCandidatesMask, out int otherRegion)
		{
			otherRegion = -1;
			otherCandidatesMask = -1;

			short m1 = grid.GetCandidateMask(pos1);
			short m2 = grid.GetCandidateMask(pos2);
			if ((baseCandidatesMask & m1) != 0 ^ (baseCandidatesMask & m2) != 0)
			{
				// One cell contains the digit that base candidate holds,
				// and another one does not contain.
				return true;
			}

			if ((m1 & baseCandidatesMask) == 0 && (m2 & baseCandidatesMask) == 0
				|| (m1 & ~baseCandidatesMask) == 0 && (m2 & ~baseCandidatesMask) == 0)
			{
				// Two cells don't contain any digits in the base cells neither,
				// or both contains only digits from base cells,
				// which is not allowed in the exocet rule (or does not contain
				// any eliminations).
				return false;
			}

			// Now we check the special cases, in other words, the last two cells both contain
			// digits from base cells, and at least one cell contains non-base digits.
			// Therefore, we should check on non-base digits, whether the non-base digits
			// covers only one of two last cells; otherwise, false.
			short candidatesMask = (short)((m1 | m2) & ~baseCandidatesMask);
			int r1 = GetRegion(pos1, Row);
			var span = (Span<int>)stackalloc[]
			{
				GetRegion(pos1, Block),
				r1 == GetRegion(pos2, Row) ? r1 : GetRegion(pos1, Column)
			};
			foreach (short mask in GetCombinations(candidatesMask))
			{
				for (int i = 0; i < 2; i++)
				{
					int count = 0;
					for (int j = 0; j < 9; j++)
					{
						int p = RegionCells[span[i]][j];
						if (p == pos1 || p == pos2 || grid.GetStatus(p) != Empty
							|| (grid.GetCandidateMask(p) & mask) == 0)
						{
							continue;
						}

						count++;
					}

					if (count == mask.CountSet() - 1)
					{
						for (int j = 0; j < 9; j++)
						{
							int p = RegionCells[span[i]][j];
							if (grid.GetStatus(p) != Empty || (grid.GetCandidateMask(p) & mask) == 0
								|| (grid.GetCandidateMask(p) & ~mask) == 0 || p == pos1 || p == pos2)
							{
								continue;
							}
						}

						otherCandidatesMask = mask;
						otherRegion = span[i];
						return true;
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Check Bi-bi pattern eliminations.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="baseCandidatesMask">The base candidate mask.</param>
		/// <param name="b1">The base cell 1.</param>
		/// <param name="b2">The base cell 2.</param>
		/// <param name="tq1">The target Q1 cell.</param>
		/// <param name="tq2">The target Q2 cell.</param>
		/// <param name="tr1">The target R1 cell.</param>
		/// <param name="tr2">The target R2 cell.</param>
		/// <param name="crossline">The cross-line cells.</param>
		/// <param name="isRow">
		/// Indicates whether the specified exocet is in the horizontal direction.
		/// </param>
		/// <param name="lockedQ">The locked member Q.</param>
		/// <param name="lockedR">The locked member R.</param>
		/// <param name="targetMap">The target map.</param>
		/// <param name="bibiElims">
		/// (<see langword="out"/> parameter) The Bi-bi pattern eliminations.
		/// </param>
		/// <param name="targetPairElims">
		/// (<see langword="out"/> parameter) The target pair eliminations.
		/// </param>
		/// <param name="swordfishElims">
		/// (<see langword="out"/> parameter) The swordfish eliminations.
		/// </param>
		/// <returns>A <see cref="bool"/> value indicating whether the pattern exists.</returns>
		private bool CheckBibiPattern(
			IReadOnlyGrid grid, short baseCandidatesMask, int b1, int b2,
			int tq1, int tq2, int tr1, int tr2, GridMap crossline, bool isRow,
			short lockedQ, short lockedR, GridMap targetMap,
			out BibiPatternEliminations bibiElims, out TargetPairEliminations targetPairElims,
			out SwordfishEliminations swordfishElims)
		{
			bibiElims = default;
			targetPairElims = default;
			swordfishElims = default;
			var playground = (Span<short>)stackalloc short[3];
			int block = GetRegion(b1, Block);
			short[] temp = new short[4];
			for (int i = 0; i < 9; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					int p = RegionCells[BibiIter[block, j]][i];
					if (!grid.GetCandidateMask(p).IsPowerOfTwo())
					{
						continue;
					}

					temp[j] |= grid.GetCandidateMask(p);
				}
			}

			short commonCandidatesMask = (short)((temp[0] | temp[3]) & (temp[1] | temp[2]) & baseCandidatesMask);
			playground[1] = (short)(
				temp[0] & temp[3] & baseCandidatesMask & ~commonCandidatesMask & baseCandidatesMask);
			playground[2] = (short)(
				temp[1] & temp[2] & baseCandidatesMask & ~commonCandidatesMask & baseCandidatesMask);
			if (playground[1] == 0 || playground[2] == 0)
			{
				// Does not contain Bi-bi pattern.
				return false;
			}

			var dic = new Dictionary<int, short>
			{
				[b1] = grid.GetCandidateMask(b1),
				[b2] = grid.GetCandidateMask(b2)
			};
			for (int i = 1; i <= 2; i++)
			{
				for (int j = 1; j <= 2; j++)
				{
					var (pos1, pos2) = j == 1 ? (b1, b2) : (b2, b1);
					short ck = (short)(grid.GetCandidateMask(pos1) & playground[i]);
					if (ck != 0 && !ck.IsPowerOfTwo()
						|| (grid.GetCandidateMask(pos1) & ~(ck | playground[i == 1 ? 2 : 1])) != 0)
					{
						continue;
					}

					short candidateMask = ck != 0 ? ck : playground[i];
					candidateMask &= grid.GetCandidateMask(pos2);
					if (candidateMask == 0)
					{
						continue;
					}

					foreach (int digit in candidateMask.GetAllSets())
					{
						bibiElims.Add(new Conclusion(Elimination, pos2, digit));
						dic[pos2] &= (short)~(1 << digit);
					}
				}
			}

			// Now check all base digits last.
			short last = (short)(dic[b1] | dic[b2]);
			foreach (int digit in (Grid.MaxCandidatesMask & ~last & ~lockedQ).GetAllSets())
			{
				if (grid.Exists(tq1, digit) is true)
				{
					bibiElims.Add(new Conclusion(Elimination, tq1, digit));
				}
				if (grid.Exists(tq2, digit) is true)
				{
					bibiElims.Add(new Conclusion(Elimination, tq2, digit));
				}
			}
			foreach (int digit in (Grid.MaxCandidatesMask & ~last & ~lockedR).GetAllSets())
			{
				if (grid.Exists(tr1, digit) is true)
				{
					bibiElims.Add(new Conclusion(Elimination, tr1, digit));
				}
				if (grid.Exists(tr2, digit) is true)
				{
					bibiElims.Add(new Conclusion(Elimination, tr2, digit));
				}
			}

			// Then check target pairs if worth.
			if (last.CountSet() == 2)
			{
				var elimMap = (targetMap & EmptyMap).PeerIntersection;
				if (elimMap.IsEmpty)
				{
					// Exit the method.
					return true;
				}

				var digits = last.GetAllSets();
				foreach (int digit in digits)
				{
					foreach (int cell in elimMap & CandMaps[digit])
					{
						targetPairElims.Add(new Conclusion(Elimination, cell, digit));
					}
				}
				elimMap = new GridMap(stackalloc[] { b1, b2 }, ProcessPeersWithoutItself);
				if (elimMap.IsEmpty)
				{
					return true;
				}

				foreach (int digit in digits)
				{
					foreach (int cell in elimMap & CandMaps[digit])
					{
						targetPairElims.Add(new Conclusion(Elimination, cell, digit));
					}
				}

				// Then check swordfish pattern.
				foreach (int digit in digits)
				{
					short mask = isRow ? crossline.RowMask : crossline.ColumnMask;
					foreach (int offset in mask.GetAllSets())
					{
						int region = offset + (isRow ? 9 : 18);
						if ((crossline & RegionMaps[region] & CandMaps[digit]).IsNotEmpty)
						{
							foreach (int cell in (RegionMaps[region] & CandMaps[digit]) - crossline)
							{
								swordfishElims.Add(new Conclusion(Elimination, cell, digit));
							}
						}
					}
				}
			}

			return true;
		}
	}
}
