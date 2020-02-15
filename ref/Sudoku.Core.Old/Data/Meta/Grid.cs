using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Text;
using Sudoku.Checking;
using Sudoku.Data.Extensions;
using Sudoku.Data.Literals;
using Sudoku.Linq;

namespace Sudoku.Data.Meta
{
	public sealed class Grid : ICloneable<Grid>, IEnumerable<CellInfo>, IEquatable<Grid>, IFormattable
	{
		public static readonly Grid Empty = new Grid();

		private readonly CellInfo[,] _grid = new CellInfo[9, 9];


		public Grid(int[,] gridValues)
		{
			Contract.Requires(gridValues.GetLength(0) == 9 && gridValues.GetLength(1) == 9);

			for (int i = 0; i < 9; i++)
			{
				for (int j = 0; j < 9; j++)
				{
					int value = gridValues[i, j];
					_grid[i, j] = value == 0
						? new CellInfo(new Cell(i, j))
						: new CellInfo(new Cell(i, j), gridValues[i, j] - 1, CellType.Given);

					AddEventHandler(ref _grid[i, j]);
				}
			}

			InitializeCandidates();
		}

		public Grid((int value, CellType cellType)[,] gridValues) : this(gridValues, null)
		{
		}

		public Grid(
			(int value, CellType cellType)[,] gridValues, IEnumerable<Candidate>? eliminations)
		{
			Contract.Assert(!(gridValues is null));
			Contract.Requires(gridValues.GetLength(0) == 9 && gridValues.GetLength(1) == 9);

			for (int i = 0; i < 9; i++)
			{
				for (int j = 0; j < 9; j++)
				{
					var (value, cellType) = gridValues[i, j];
					_grid[i, j] = new CellInfo(new Cell(i, j), value, cellType);

					AddEventHandler(ref _grid[i, j]);
				}
			}

			InitializeCandidates();

			// Custom eliminations (if exists).
			if (!(eliminations is null))
			{
				foreach (var candidate in eliminations)
				{
					this[candidate] = false;
				}
			}
		}

		private Grid() { }

		private Grid(object[,] gridValues)
		{
			Contract.Requires(gridValues.GetLength(0) == 9 && gridValues.GetLength(1) == 9);

			for (int i = 0; i < 9; i++)
			{
				for (int j = 0; j < 9; j++)
				{
					_grid[i, j] = gridValues[i, j] switch
					{
						int value => new CellInfo(new Cell(i, j), value),
						CandidateField cands => new CellInfo(new Cell(i, j), default, default, cands),
						CellInfo info => info,
						_ => throw new InvalidCastException(
							$"The type is not {nameof(Int32)} or {nameof(CandidateField)}.")
					};

					AddEventHandler(ref _grid[i, j]);
				}
			}
		}

		private Grid(CellInfo[,] cellInfos)
		{
			Contract.Requires(cellInfos.GetLength(0) == 9 && cellInfos.GetLength(1) == 9);

			for (int i = 0; i < 9; i++)
			{
				for (int j = 0; j < 9; j++)
				{
					_grid[i, j] = cellInfos[i, j];

					AddEventHandler(ref _grid[i, j]);
				}
			}
		}


		public bool IsSolved => Series.All(info => info.IsValueCell);

		public int GivensCount => Givens.Count();

		public int ValuesCount => Series.Count(info => info.IsValueCell);

		public int CandidatesCount
		{
			get
			{
				return Series.Count(
					info => !info.IsValueCell,
					info => info.CandidateCount);
			}
		}

		public IEnumerable<(Cell cell, int value)> Givens
		{
			get
			{
				return from info in Series
					   where info.CellType == CellType.Given
					   select (info.Cell, info.Value);
			}
		}

		public IEnumerable<(Cell cell, int value)> Modifiables
		{
			get
			{
				return from info in Series
					   where info.CellType == CellType.Modifiable
					   select (info.Cell, info.Value);
			}
		}

		public IEnumerable<(Cell cell, CandidateField candidates)> Candidates
		{
			get
			{
				return from info in Series
					   where info.CellType == CellType.Empty
					   select (info.Cell, info.Candidates);
			}
		}

		public IEnumerable<Candidate> CandidateList
		{
			get
			{
				return from info in Series
					   from i in Values.DigitRange
					   where !info.IsValueCell && info[i]
					   select new Candidate(info.Cell, i);
			}
		}

		private IEnumerable<CellInfo> Series => _grid.Cast<CellInfo>();


		public bool this[Candidate candidate]
		{
			get => this[candidate.Cell][candidate.Digit];
			set
			{
				var info = this[candidate.Cell];
				if (!info.IsValueCell)
				{
					info[candidate.Digit] = value;
				}
			}
		}

		public CellInfo this[int row, int column] => _grid[row, column];

		public ref CellInfo this[Cell cell] => ref this[cell.GlobalOffset];

		public IEnumerable<CellInfo> this[Region region] =>
			region.Cells.Select(cell => this[cell]);

		internal ref CellInfo this[int offset] => ref _grid[offset / 9, offset % 9];


		public void FixValues()
		{
			for (int i = 0; i < 81; i++)
			{
				ref var info = ref this[i];
				if (info.IsValueCell)
				{
					info.CellType = CellType.Given;
				}
			}
		}

		public void UnfixValues()
		{
			for (int i = 0; i < 81; i++)
			{
				ref var info = ref this[i];
				if (info.IsValueCell)
				{
					info.CellType = CellType.Modifiable;
				}
			}
		}

		public void RecomputeCandidates() => InitializeCandidates();

		public override bool Equals(object? obj) =>
			obj is Grid comparer && Equals(comparer);

		public bool Equals(Grid other) => SimplyEquals(other);

		public bool CustomEquals(Grid other, GridEqualityOptions option)
		{
			if (option == GridEqualityOptions.All)
				return DeeplyEquals(other);
			if (option == GridEqualityOptions.None)
				return true;

			Contract.Assume(!(other is null));

			bool result = true;
			if (option.HasFlag(GridEqualityOptions.CheckGivens))
			{
				result = result && JoinSequentialEquals(
					leftTable: Givens,
					rightTable: other.Givens,
					predicate: triplet => triplet.leftValue == triplet.rightValue);
			}
			if (option.HasFlag(GridEqualityOptions.CheckModifiables))
			{
				result = result && JoinSequentialEquals(
					leftTable: Modifiables,
					rightTable: other.Modifiables,
					predicate: triplet => triplet.leftValue == triplet.rightValue);
			}
			if (option.HasFlag(GridEqualityOptions.CheckCandidates))
			{
				result = result && JoinSequentialEquals(
					leftTable: Candidates,
					rightTable: other.Candidates,
					predicate: triplet => triplet.leftValue == triplet.rightValue);
			}
			return result;
		}

		public bool SimplyEquals(Grid other) => CustomEquals(other, GridEqualityOptions.CheckValues);

		public bool DeeplyEquals(Grid other)
		{
			Contract.Assume(!(other is null));

			return GetHashCode() == other.GetHashCode();
		}

		public bool SimplyValidate()
		{
			return GivensCount >= 17 && Series.All(
				info =>
				{
					return GetPeerInfosOf(info.Cell).All(
						peerInfo =>
						{
							return info.IsValueCell
								&& peerInfo.IsValueCell
								&& peerInfo.Value != info.Value;
						});
				});
		}

		public bool DeeplyValidate() => this.IsUnique(out var _);

		public override int GetHashCode()
		{
			int result = GetType().GetHashCode();
			foreach (var info in _grid)
			{
				result ^= info.GetHashCode();
			}
			return result;
		}

		public int GetEmptyCellsCount(Region region) =>
			this[region].Count(info => !info.IsValueCell);

		public int[,] ToArray()
		{
			int[,] result = new int[9, 9];
			string s = ToString(GridFormats.SusserZero, null);
			for (int i = 0; i < 9; i++)
			{
				for (int j = 0; j < 9; j++)
				{
					result[i, j] = s[i * 9 + j] - '0';
				}
			}

			return result;
		}

		public override string ToString() => ToString(null, null);

		public string ToString(string format) => ToString(format, null);

		public string ToString(string? format, IFormatProvider? formatProvider)
		{
			if (formatProvider?.GetFormat(GetType()) is ICustomFormatter fmt)
				return fmt.Format(format, this, formatProvider);

			var sb = new StringBuilder();
			switch (format?.ToLower(CultureInfo.CurrentCulture) ?? string.Empty)
			{
				//case null:
				case "":
				case "g.": // `.` for blank cells.
					foreach (var info in _grid)
					{
						sb.Append(info.CellType == CellType.Given ? $"{info.Value + 1}" : ".");
					}
					return sb.ToString();
				case "g":
				case "g0": // `0` for blank cells.
					foreach (var info in _grid)
					{
						sb.Append(info.CellType == CellType.Given ? $"{info.Value + 1}" : "0");
					}
					return sb.ToString();
				case "g+.":
				case "g.+": // `+value` for modifiable cell values and `.` for blank cells.
					foreach (var info in _grid)
					{
						sb.Append(info.CellType switch
						{
							CellType.Given => $"{info.Value + 1}",
							CellType.Modifiable => $"+{info.Value + 1}",
							_ => ".",
						});
					}
					return sb.ToString();
				case "g+0":
				case "g0+": // `+value` for modifiable cell values and `0` for blank cells.
					foreach (var info in _grid)
					{
						sb.Append(info.CellType switch
						{
							CellType.Given => $"{info.Value + 1}",
							CellType.Modifiable => $"+{info.Value + 1}",
							_ => "0",
						});
					}
					return sb.ToString();
				case "g.:":
				case "g.+:":
				case "g+.:":
					GetOutputWithEliminations(sb, format: "g.+");
					return sb.RemoveFromLast(1).ToString();
				case "g:":
				case "g0:":
				case "g0+:":
				case "g+0:":
					GetOutputWithEliminations(sb, format: "g0+");
					return sb.RemoveFromLast(1).ToString();
				case "c": // Candidate grid (PM grid).
					OnAppending(sb);
					return sb.ToString();
				default:
					throw new FormatException(
						$"Parameter {nameof(format)} is not a valid " +
						$"formatting string.{Environment.NewLine}" +
						$"Pay attention please, you cannot add " +
						$"any Nonprinting Characters anywhere, " +
						$@"such as ' ' and '\t' etc.");
			}
		}

		public CandidateField GetDigitPositionsOf(Region region, int digit)
		{
			var result = new CandidateField();

			int i = 0;
			foreach (var info in this[region])
			{
				var cellType = info.CellType;
				if (!info.IsValueCell && info[digit])
				{
					result[i] = false;
				}

				i++;
			}

			result.ReverseAll();
			return result;
		}

		public Grid Clone()
		{
			var result = new CellInfo[9, 9];
			for (int i = 0; i < 9; i++)
			{
				for (int j = 0; j < 9; j++)
				{
					var (cell, value, cellType, candidates) = _grid[i, j];
					result[i, j] = new CellInfo(cell, value, cellType, candidates);
				}
			}

			return new Grid(result);
		}

		public IEnumerable<CellInfo> GetPotentialInfosFor(Region region, int digit)
		{
			return from CellInfo info in this[region]
				   where !info.IsValueCell && info[digit]
				   select info;
		}

		public IEnumerable<CellInfo> GetPeerInfosOf(Cell cell)
		{
			return from info in Series
				   where cell.Peers.Contains(info.Cell)
				   select info;
		}

		public IEnumerator<CellInfo> GetEnumerator() => Series.GetEnumerator();

		object ICloneable.Clone() => Clone();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		private void OnValueChanging(object sender, ValueChangingEventArgs e)
		{
			Contract.Assume(!(e is null));

			if (!e.Cancel)
			{
				var (cell, value) = e.Candidate;
				foreach (var info in GetPeerInfosOf(cell))
				{
					if (!info.IsValueCell)
					{
						this[info.Cell][value] = false;
					}
				}
			}
		}

		private void InitializeCandidates()
		{
			foreach (var (info, peerInfo) in from info in Series
											 where info.IsValueCell
											 from peerInfo in GetPeerInfosOf(info.Cell)
											 where !peerInfo.IsValueCell
											 select (info, peerInfo))
			{
				peerInfo.Candidates[info.Value] = false;
			}
		}

		private void AddEventHandler(ref CellInfo info) =>
			info.ValueChanging += OnValueChanging;

		private void GetOutputWithEliminations(StringBuilder sb, string format)
		{
			bool isStarted = true;
			string s = ToString(format, null);

			sb.Append($"{s}: ");
			foreach (var (cell, digit) in from cand in Parse(s).CandidateList
										  where !this[cand.Cell].IsValueCell && !this[cand]
										  select cand)
			{
				if (isStarted)
				{
					sb.Append(": ");
					isStarted = !isStarted;
				}

				sb.Append($"{digit + 1}{cell.Row + 1}{cell.Column + 1} ");
			}
		}


		public static bool TryParse(string str, [NotNullWhen(true)] out Grid? result)
		{
			try
			{
				result = Parse(str);
				return true;
			}
			catch
			{
				result = default;
				return false;
			}
		}

		public static Grid Parse(string str)
		{
			if (TryParseSusserFormat(str, out var resultNormal))
				return resultNormal;
			if (TryParsePMGridFormat(str, out var resultPMGrid))
				return resultPMGrid;

			throw new FormatException(
				$"Parameter {nameof(str)} cannot parse to target type {nameof(Grid)}.");
		}

		private static bool JoinSequentialEquals<TCell, TComparing>(
			IEnumerable<(TCell cell, TComparing comparing)> leftTable,
			IEnumerable<(TCell cell, TComparing comparing)> rightTable,
			Func<(TCell cell, TComparing leftValue, TComparing rightValue), bool> predicate)
		{
			return (
				from leftInfo in leftTable
				let cell = leftInfo.cell
				join rightInfo in rightTable on cell equals rightInfo.cell
				select (cell, leftInfo.comparing, rightInfo.comparing)
			).All(predicate);
		}

		private static bool TryParsePMGridFormat(
			string str, [NotNullWhen(returnValue: true)] out Grid? result)
		{
			string[] matches = str.MatchAll(Regexes.PMGridValuesGroup);
			if (matches.Length < 81)
			{
				result = default;
				return false;
			}

			// The length of variable `matches` must be 81,
			// so we don't need to enable this assertion.
			//Contract.Assert(matches.Length == 81);

			try
			{
				bool hasGivenSign = matches.Any(m => m.Contains('<', StringComparison.Ordinal));
				object[,] array = new object[9, 9];

				if (hasGivenSign)
				{
					// At least one of matches is surrounded
					// by given sign `<` and `>`.
					// We should only regard them as givens,
					// others will be regarded as candidate lists
					// even if there is only one digit in some list.
					for (int i = 0; i < 81; i++)
					{
						int r = i / 9, c = i % 9;
						string match = matches[i];
						if (match[0] == '<')
						{
							// Less than sign. Given digit.
							int value = match[1] - '1';

							array[r, c] = value;
						}
						else
						{
							// Candidates.
							var list = new List<int>(9);
							foreach (char ch in match)
							{
								list.Add(ch - '1');
							}

							array[r, c] = new CandidateField(list);
						}
					}
				}
				else
				{
					// All matches has no given sign.
					// We should regard a digit which is unique
					// in a list as a given digit, and others will
					// be candidate lists.
					for (int i = 0; i < 81; i++)
					{
						int r = i / 9, c = i % 9;
						string match = matches[i];
						if (match.Length == 1)
						{
							// Given digit.
							int value = match[0] - '1';

							array[r, c] = value;
						}
						else
						{
							// Candidates.
							var list = new List<int>(9);
							foreach (char ch in match)
							{
								list.Add(ch - '1');
							}

							array[r, c] = new CandidateField(list);
						}
					}
				}

				result = new Grid(array);
				return true;
			}
			catch
			{
				result = default;
				return false;
			}
		}

		private static bool TryParseSusserFormat(
			string str, [NotNullWhen(returnValue: true)] out Grid? result)
		{
			string? match = str.Match(Regexes.ExtendedSusser);
			if (match is null)
			{
				result = default;
				return false;
			}
			else
			{
				try
				{
					var array = new (int, CellType)[9, 9];
					string? gridStr = match.Match(Regexes.Susser);
					if (gridStr is null)
					{
						result = default;
						return false;
					}
					else
					{
						// Variable `current` is a iteration variable, while `real`
						// is the real index of the grid offset (range from 0 to 80),
						// and the variable `real` has no relation to
						// the parsing string `str`.
						for (int current = 0, real = 0; current < gridStr.Length; current++, real++)
						{
							int row = real / 9, column = real % 9;
							char c = str[current];
							if (c == '+')
							{
								// Value cell sign.
								// Check the next character is value or not (or out of boundary).
								if (current + 1 >= str.Length)
								{
									// Out of boundary. Throw an exception.
									throw new FormatException(
										$"Parameter {nameof(str)} cannot parse" +
										$" to target type {nameof(Grid)}.{Environment.NewLine}" +
										$"Pay attention please, '+' cannot be the last character.");
								}
								else
								{
									char d = str[++current];
									array[row, column] = d switch
									{
										_ when d >= '1' && d <= '9' => (d - '1', CellType.Modifiable),
										_ => throw new FormatException(
											$"Parameter {nameof(str)} cannot parse" +
											$" to target type {nameof(Grid)}." +
											$"{Environment.NewLine}" +
											$"Pay attention please, the character" +
											$" after '+' sign must be a value."),
									};
								}
							}
							else
							{
								array[row, column] = c switch
								{
									_ when c == '0' || c == '.' => default,
									_ when c >= '1' && c <= '9' => (c - '1', CellType.Given),
									_ => throw new FormatException(
										$"Parameter {nameof(str)} cannot parse" +
										$" to target type {nameof(Grid)}."),
								};
							}
						}

						// Check eliminations string is not null.
						string? elimPart = match.Match(Regexes.SusserElims);
						if (elimPart is null)
						{
							// No custom elimination, return directly.
							result = new Grid(array);
						}
						else
						{
							// Custom eliminations is not null,
							// we should add them one by one.
							string[] elimsStr = str.MatchAll(Regexes.SusserElimsGroup);
							var elimList = new HashSet<Candidate>();

							// We have checked the elimination string,
							// and we matched at least one elimination,
							// so here elimsStr will not be null forever.
							// We may use null-suppression operator, instead of
							// using contract to validate its nullability.
							//Contract.Assert(!(elimsStr is null));
							foreach (string elimStr in elimsStr!)
							{
								elimList.Add(
									new Candidate(
										row: elimStr[1] - '1',
										column: elimStr[2] - '1',
										digit: elimStr[0] - '1'));
							}

							result = new Grid(array, elimList);
						}

						return true;
					}
				}
				catch
				{
					result = default;
					return false;
				}
			}
		}


		public static bool operator ==(Grid left, Grid right)
		{
			Contract.Assume(!(left is null));

			return left.Equals(right);
		}

		public static bool operator !=(Grid left, Grid right)
		{
			Contract.Assume(!(left is null));

			return !left.Equals(right);
		}

		private void OnAppending(StringBuilder sb)
		{
			// Get the maximum length of output characters by columns
			// in order that we can add tabulator characters.
			int[] maxLengths = new int[9];
			{
				int i = 0;
				foreach (var group in from info in Series
									  orderby info.Column
									  group info by info.Column)
				{
					// Replace the smaller one.
					int maxLength = 0;
					maxLengths[i++] = group.Max(info =>
					{
						return Math.Max(info.CellType == CellType.Given
							? Math.Max(info.CandidateCount, 3)
							: info.CandidateCount, maxLength);
					});
				}
			} // This bracket is for protecting local variable `i`.

			// 13 is for 13 rows to output.
			for (int i = 0; i < 13; i++)
			{
				switch (i)
				{
					case 0: // Print tabs of the first line.
						PrintTabLine(sb, maxLengths, '.', '.');
						break;
					case 4:
					case 8: // Print tabs of mediate lines.
						PrintTabLine(sb, maxLengths, ':', '+');
						break;
					case 12: // Print tabs of the foot line.
						PrintTabLine(sb, maxLengths, '\'', '\'');
						break;
					default: // Print values and tabs.
						PrintValuesLine(
							sb, maxLengths,
							this[new Region(RegionType.Row, i switch
							{
								_ when i >= 1 && i <= 3 => i - 1,
								_ when i >= 5 && i <= 7 => i - 2,
								_ when i >= 9 && i <= 11 => i - 3,
								_ => throw new Exception("On the border, here will do nothing")
							})], '|', '|');
						break;
				}
			}
		}

		private static void PrintValuesLine(
			StringBuilder sb, int[] maxLengths, IEnumerable<CellInfo> cellInfos, char c1, char c2)
		{
			sb.Append(c1);
			OutputValues(sb, maxLengths, cellInfos, 0, 2);
			sb.Append(c2);
			OutputValues(sb, maxLengths, cellInfos, 3, 5);
			sb.Append(c2);
			OutputValues(sb, maxLengths, cellInfos, 6, 8);
			sb.Append(c1);
			sb.AppendLine();
		}

		private static void OutputValues(
			StringBuilder sb, int[] maxLengths, IEnumerable<CellInfo> values, int start, int end)
		{
			sb.Append(" ");
			for (int i = start; i <= end; i++)
			{
				var info = values.ElementAt(i);
				sb.Append(info.CellType != CellType.Given
					? info.Candidates.ToString().PadRight(maxLengths[i])
					: $"<{info.Value + 1}>".PadRight(maxLengths[i]));
				sb.Append(i != end ? "  " : " ");
			}
		}

		private static void PrintTabLine(StringBuilder sb, int[] maxLengths, char c1, char c2)
		{
			sb.Append(c1);
			sb.Append("".PadRight(maxLengths[0] + maxLengths[1] + maxLengths[2] + 6, '-'));
			sb.Append(c2);
			sb.Append("".PadRight(maxLengths[3] + maxLengths[4] + maxLengths[5] + 6, '-'));
			sb.Append(c2);
			sb.Append("".PadRight(maxLengths[6] + maxLengths[7] + maxLengths[8] + 6, '-'));
			sb.Append(c1);
			sb.AppendLine();
		}
	}
}
