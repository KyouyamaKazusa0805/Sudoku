using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Sudoku.Data;
using Sudoku.Drawing;
using PointConverter = Sudoku.Drawing.PointConverter;

namespace Sudoku.IO
{
	/// <summary>
	/// Encapsulates a batch code executor. This class stands for a instance
	/// that can execute a specified step, such as draw a picture, save a picture, etc..
	/// </summary>
	/// <remarks>
	/// All available batches are below:
	/// <list type="table">
	/// <item>
	/// <term><c>Create workspace with width &lt;width&gt; height &lt;height&gt;</c></term>
	/// <description>
	/// Create an empty and new picture workspace with the specified width and height.
	/// </description>
	/// </item>
	/// <item>
	/// <term><c>Fill given &lt;digit&gt; in &lt;cell&gt;</c></term>
	/// <description>Draw a specified given in a specified cell.</description>
	/// </item>
	/// <item>
	/// <term><c>Fill modifiable &lt;digit&gt; in &lt;cell&gt;</c></term>
	/// <description>Draw a specified modifiable in a specified cell.</description>
	/// </item>
	/// <item>
	/// <term><c>Fill candidate &lt;digit&gt; in &lt;cell&gt;</c></term>
	/// <description>Draw a specified candidate in a specified cell.</description>
	/// </item>
	/// <item>
	/// <term><c>Draw cell &lt;cell&gt; with color a &lt;a&gt; r &lt;r&gt; g &lt;g&gt; b &lt;b&gt;</c></term>
	/// <description>Paint a color for a specified cell.</description>
	/// </item>
	/// <item>
	/// <term>
	/// <c>Draw candidate &lt;cell&gt; with color a &lt;a&gt; r &lt;r&gt; g &lt;g&gt; b &lt;b&gt;</c>
	/// </term>
	/// <description>Paint a color for a specified candidate.</description>
	/// </item>
	/// <item>
	/// <term>
	/// <c>Draw region &lt;region&gt; with color a &lt;a&gt; r &lt;r&gt; g &lt;g&gt; b &lt;b&gt;</c>
	/// </term>
	/// <description>Draw a color for a specified region.</description>
	/// </item>
	/// <item>
	/// <term>
	/// <c>Draw row &lt;row&gt; with color a &lt;a&gt; r &lt;r&gt; g &lt;g&gt; b &lt;b&gt;</c>
	/// </term>
	/// <description>Draw a color for a specified row.</description>
	/// </item>
	/// <item>
	/// <term>
	/// <c>Draw column &lt;column&gt; with color a &lt;a&gt; r &lt;r&gt; g &lt;g&gt; b &lt;b&gt;</c>
	/// </term>
	/// <description>Draw a color for a specified column.</description>
	/// </item>
	/// <item>
	/// <term>
	/// <c>Draw block &lt;block&gt; with color a &lt;a&gt; r &lt;r&gt; g &lt;g&gt; b &lt;b&gt;</c>
	/// </term>
	/// <description>Draw a color for a specified block.</description>
	/// </item>
	/// <item>
	/// <term>
	/// <c>Draw chain from &lt;candidate1&gt; to &lt;candidate2&gt; type (line|strong|weak|chain)</c>
	/// </term>
	/// <description>Draw a chain.</description>
	/// </item>
	/// <item>
	/// <term>
	/// <c>Draw cross &lt;cell&gt;</c>
	/// </term>
	/// <description>Draw a cross sign (used for direct lines).</description>
	/// </item>
	/// <item>
	/// <term>
	/// <c>Draw circle &lt;cell&gt;</c>
	/// </term>
	/// <description>Draw a circle sign (used for direct lines).</description>
	/// </item>
	/// <item>
	/// <term><c>Save to &lt;path&gt;</c></term>
	/// <description>
	/// Save the current picture to the specified path. If the current memory doesn't contain
	/// any pictures here, it'll do nothing.
	/// </description>
	/// </item>
	/// <item>
	/// <term><c>Close</c></term>
	/// <description>
	/// Close the current workspace. If the current memory doesn't contain any pictures here,
	/// it'll do nothing.
	/// </description>
	/// </item>
	/// </list>
	/// </remarks>
	public sealed class BatchExecutor
	{
		/// <summary>
		/// Indicates the empty handler.
		/// </summary>
		private static readonly BatchExecutingEventHandler EmptyHandler = static () => { };


		/// <summary>
		/// The settings.
		/// </summary>
		private readonly Settings _settings;


		/// <summary>
		/// The internal point converter.
		/// </summary>
		private PointConverter? _pointConverter;

		/// <summary>
		/// Indicates the inner grid painter.
		/// </summary>
		private GridPainter? _painter;

		/// <summary>
		/// The list of methods that should be executed.
		/// </summary>
		private BatchExecutingEventHandler? _batchExecuting;


		/// <summary>
		/// Initializes an instance with the specified settings.
		/// </summary>
		/// <param name="settings">The settings.</param>
		public BatchExecutor(Settings settings) => _settings = settings;


		/// <summary>
		/// Execute the batch.
		/// </summary>
		public void Execute() => _batchExecuting?.Invoke();

		/// <summary>
		/// Clear all executions.
		/// </summary>
		public void Clear() => _batchExecuting = null;

		/// <summary>
		/// Create a workspace.
		/// </summary>
		/// <param name="args">The arguments.</param>
		private void Create(string[] args)
		{
			if (args.Length != 7)
			{
				return;
			}

			if (args[1] != "workspace" || args[2] != "with" || args[3] != "width" || args[5] != "height")
			{
				return;
			}

			if (!int.TryParse(args[4], out int width) || width is < 0 or > 1000
				|| !int.TryParse(args[6], out int height) || height is < 0 or > 1000)
			{
				return;
			}

			_pointConverter = new(width, height);
			_painter = new(_pointConverter, _settings, SudokuGrid.Undefined);
		}

		/// <summary>
		/// Close the workspace.
		/// </summary>
		private void Close()
		{
			_painter = null;
			_pointConverter = null;
		}

		/// <summary>
		/// Draw a color for a cell.
		/// </summary>
		/// <param name="args">The arguments.</param>
		private void DrawCell(string[] args)
		{
			if (_painter is null)
			{
				return;
			}

			_painter.CustomView ??= new();
			DrawInternal(args, 0, 81, _painter.CustomView.AddCell);
		}

		/// <summary>
		/// Draw a color for a candidate.
		/// </summary>
		/// <param name="args">The arguments.</param>
		private void DrawCandidate(string[] args)
		{
			if (_painter is null)
			{
				return;
			}

			_painter.CustomView ??= new();
			DrawInternal(args, 0, 729, _painter.CustomView.AddCandidate);
		}

		/// <summary>
		/// Draw a color for a region.
		/// </summary>
		/// <param name="args">The arguments.</param>
		private void DrawRegion(string[] args)
		{
			if (_painter is null)
			{
				return;
			}

			_painter.CustomView ??= new();
			DrawInternal(args, 0, 27, _painter.CustomView.AddRegion);
		}

		/// <summary>
		/// Draw a color for a row.
		/// </summary>
		/// <param name="args">The arguments.</param>
		private void DrawRow(string[] args)
		{
			if (_painter is null)
			{
				return;
			}

			_painter.CustomView ??= new();
			DrawInternal(args, 9, 18, _painter.CustomView.AddRegion);
		}

		/// <summary>
		/// Draw a color for a column.
		/// </summary>
		/// <param name="args">The arguments.</param>
		private void DrawColumn(string[] args)
		{
			if (_painter is null)
			{
				return;
			}

			_painter.CustomView ??= new();
			DrawInternal(args, 18, 27, _painter.CustomView.AddRegion);
		}

		/// <summary>
		/// Draw a color for a block.
		/// </summary>
		/// <param name="args">The arguments.</param>
		private void DrawBlock(string[] args)
		{
			if (_painter is null)
			{
				return;
			}

			_painter.CustomView ??= new();
			DrawInternal(args, 0, 9, _painter.CustomView.AddRegion);
		}

		/// <summary>
		/// Draw a chain.
		/// </summary>
		/// <param name="args">The arguments.</param>
		private void DrawChain(string[] args)
		{
			if (_painter is null)
			{
				return;
			}

			if (args.Length != 8 || args[2] != "from" || args[4] != "to" || args[6] != "type")
			{
				return;
			}

			if (!int.TryParse(args[3], out int start) || start is < 1 or > 729
				|| !int.TryParse(args[5], out int end) || end is < 1 or > 729)
			{
				return;
			}

			_painter.CustomView ??= new();
			_painter.CustomView.AddLink(
				new(
					start - 1,
					end - 1,
					args[7] switch
					{
						"line" => LinkType.Line,
						"chain" or "strong" => LinkType.Strong,
						"weak" => LinkType.Weak,
						_ => LinkType.Default
					}
				)
			);
		}

		/// <summary>
		/// Draw cross sign.
		/// </summary>
		/// <param name="args">The arguments.</param>
		private void DrawCross(string[] args)
		{
			if (_painter is null)
			{
				return;
			}

			if (args.Length != 3)
			{
				return;
			}

			if (!int.TryParse(args[2], out int c) || c is < 1 or > 81)
			{
				return;
			}

			_painter.CustomView ??= new();
			_painter.CustomView.AddDirectLine(Cells.Empty, new() { c - 1 });
		}

		/// <summary>
		/// Draw circle sign.
		/// </summary>
		/// <param name="args">The arguments.</param>
		private void DrawCircle(string[] args)
		{
			if (_painter is null)
			{
				return;
			}

			if (args.Length != 3)
			{
				return;
			}

			if (!int.TryParse(args[2], out int c) || c is < 1 or > 81)
			{
				return;
			}

			_painter.CustomView ??= new();
			_painter.CustomView.AddDirectLine(new() { c - 1 }, Cells.Empty);
		}

		/// <summary>
		/// The internal drawing method.
		/// </summary>
		/// <param name="args">The arguments.</param>
		/// <param name="min">The minimum value to check.</param>
		/// <param name="max">The maximum value to check.</param>
		/// <param name="action">The action.</param>
		private void DrawInternal(string[] args, int min, int max, Action<long, int> action)
		{
			if (args.Length != 13)
			{
				return;
			}

			if (args[3] != "with" || args[4] != "color"
				|| args[5] != "a" || args[7] != "r" || args[9] != "g" || args[11] != "b")
			{
				return;
			}

			if (!int.TryParse(args[2], out int c) || c - 1 < min || c - 1 >= max)
			{
				return;
			}

			if (!int.TryParse(args[6], out int a) || !int.TryParse(args[8], out int r)
				|| !int.TryParse(args[10], out int g) || !int.TryParse(args[12], out int b))
			{
				return;
			}

			action(0xDEAD << 32 | a << 24 | r << 16 | g << 8 | b, c - 1);
		}

		/// <summary>
		/// Draw given values.
		/// </summary>
		/// <param name="args">The arguments.</param>
		private void FillGiven(string[] args) =>
			FillValueInternal(args, (cell, digit) =>
			{
				if (_painter is not null)
				{
					var grid = _painter.Grid;
					grid[cell] = digit;
					grid.SetStatus(cell, CellStatus.Given);
				}
			});

		/// <summary>
		/// Draw modifiable values.
		/// </summary>
		/// <param name="args">The arguments.</param>
		private void FillModifiable(string[] args) =>
			FillValueInternal(args, (cell, digit) =>
			{
				if (_painter is not null)
				{
					_painter.Grid[cell] = digit;
				}
			});

		/// <summary>
		/// Draw candidates.
		/// </summary>
		/// <param name="args">The arguments.</param>
		private void FillCandidate(string[] args) =>
			FillValueInternal(args, (cell, digit) =>
			{
				if (_painter is not null)
				{
					_painter.Grid[cell, digit] = false;
				}
			});

		/// <summary>
		/// Save the current picture to the specified path.
		/// </summary>
		/// <param name="args">The arguments.</param>
		private void SavePictureToSpecifiedPath(string[] args)
		{
			if (_painter is null)
			{
				return;
			}

			if (args.Length != 3 || args[1] != "to")
			{
				return;
			}

			try
			{
				string s = args[2];
				if (Path.GetDirectoryName(s) is { } directoryName)
				{
					DirectoryEx.CreateIfDoesNotExist(directoryName);

					_painter.Draw().Save(s);
				}
			}
			catch
			{
			}
		}

		/// <summary>
		/// The internal method for filling a value.
		/// </summary>
		/// <param name="args">The arguments.</param>
		/// <param name="valueFill">The value filling method.</param>
		private void FillValueInternal(string[] args, Action<int, int> valueFill)
		{
			if (args.Length != 5 || args[3] != "in")
			{
				return;
			}

			if (!int.TryParse(args[2], out int d) || d is < 1 or > 9)
			{
				return;
			}

			if (!int.TryParse(args[4], out int c) || c is < 1 or > 81)
			{
				return;
			}

			valueFill(c - 1, d - 1);
		}


		/// <summary>
		/// Try to parse batch codes and convert to the 
		/// </summary>
		/// <param name="batch">The batch code.</param>
		/// <param name="settings">The settings.</param>
		/// <param name="result">(<see langword="out"/> parameter) The result.</param>
		/// <returns>The <see cref="bool"/> result.</returns>
		public static bool TryParse(
			string batch, Settings settings, [NotNullWhen(true)] out BatchExecutor? result)
		{
			string[] lines = batch.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
			if (lines.Length == 0)
			{
				result = null;
				return false;
			}

			var resultExecutor = new BatchExecutor(settings);
			foreach (string line in lines)
			{
				string target = line.Trim(new[] { '\n', '\r', ' ' });
				if (string.IsNullOrWhiteSpace(target))
				{
					continue;
				}

				string[] args = target.Split(' ', StringSplitOptions.RemoveEmptyEntries);
				if (args.Length == 0)
				{
					result = null;
					return false;
				}

				for (int i = 0, length = args.Length; i < length; i++)
				{
					args[i] = args[i].ToLower();
				}

				resultExecutor._batchExecuting += args[0] switch
				{
					"create" => () => resultExecutor.Create(args),
					"close" => () => resultExecutor.Close(),
					"draw" when args.Length >= 2 => args[1] switch
					{
						"cell" => () => resultExecutor.DrawCell(args),
						"candidate" => () => resultExecutor.DrawCandidate(args),
						"region" => () => resultExecutor.DrawRegion(args),
						"row" => () => resultExecutor.DrawRow(args),
						"column" => () => resultExecutor.DrawColumn(args),
						"block" => () => resultExecutor.DrawBlock(args),
						"chain" => () => resultExecutor.DrawChain(args),
						"cross" => () => resultExecutor.DrawCross(args),
						"circle" => () => resultExecutor.DrawCircle(args),
						_ => EmptyHandler
					},
					"fill" when args.Length >= 2 => args[1] switch
					{
						"given" => () => resultExecutor.FillGiven(args),
						"modifiable" => () => resultExecutor.FillModifiable(args),
						"candidate" => () => resultExecutor.FillCandidate(args),
						_ => EmptyHandler
					},
					"save" => () => resultExecutor.SavePictureToSpecifiedPath(args),
					_ => EmptyHandler
				};
			}

			result = resultExecutor;
			return true;
		}
	}
}
