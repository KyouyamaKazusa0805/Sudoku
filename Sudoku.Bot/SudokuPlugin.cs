using System;
using System.Text;
using System.Threading.Tasks;
using HuajiTech.Mirai;
using HuajiTech.Mirai.Events;
using HuajiTech.Mirai.Messaging;
using Sudoku.Bot.Extensions;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Globalization;
using Sudoku.Solving.Checking;
using Sudoku.Solving.Extensions;
using Sudoku.Solving.Manual;
using R = Sudoku.Bot.Resources;

namespace Sudoku.Bot
{
	/// <summary>
	/// Encapsulates a plugin for sudoku.
	/// </summary>
	public sealed class SudokuPlugin
	{
		/// <summary>
		/// Indicates the drawing size.
		/// </summary>
		private const int Size = 800;


		/// <summary>
		/// The inner grid painter.
		/// </summary>
		private static GridPainter? _painter;


		/// <summary>
		/// Initializes an instance with the specified user event source.
		/// </summary>
		/// <param name="currentUserEventSource">The current user event source.</param>
		public SudokuPlugin(CurrentUserEventSource currentUserEventSource) =>
			currentUserEventSource.AddMessageReceivedEventHandler(OnReceivingMessageAsync);

		/// <summary>
		/// The method that invoked on receiving messages.
		/// </summary>
		/// <param name="session">The current session.</param>
		/// <param name="e">The event arguments.</param>
		/// <returns>The task of this method.</returns>
		private static async Task OnReceivingMessageAsync(Session session, MessageReceivedEventArgs e)
		{
			var complexStr = e.Message.Content;
			if (complexStr is not { Count: not 0 } || complexStr[0] is not PlainText pl)
			{
				return;
			}

			string info = pl.ToString();
			if (info.Contains(R.GetValue("MyName")) && info.Contains(R.GetValue("Morning")))
			{
				await GreetingAsync(e);
			}
			else if (info.Trim() == R.GetValue("HelpCommand"))
			{
				await ShowHelperTextAsync(e);
			}
			else if (info.Trim() == R.GetValue("IntroducingCommand"))
			{
				await IntroduceAsync(e);
			}
			else if (info.StartsWith(R.GetValue("AnalysisCommand")))
			{
				await AnalysisAsync(info, e);
			}
			else if (info.StartsWith(R.GetValue("DrawingCommand")))
			{
				await DrawImageAsync(info, e);
			}
			else if (info.StartsWith(R.GetValue("GenerateEmptyGridCommand")))
			{
				await GenerateEmptyGridAsync(e);
			}
			else if (info.StartsWith(R.GetValue("CleaningGridCommand")))
			{
				await CleanGridAsync(info, e);
			}
			else
			{
				string[] s = info.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				if (s.Length < 1)
				{
					return;
				}

				if (s[0] == R.GetValue("StartDrawingCommand"))
				{
					await StartDrawingAsync(s, e);
				}
				else if (s[0] == R.GetValue("EndDrawingCommand"))
				{
					await DisposePainterAsync(e);
				}
				else if (s[0] == R.GetValue("FillCommand") && s.Length >= 2)
				{
					if (s[1] == R.GetValue("Given"))
					{
						await FillAsync(s, e, true);
					}
					else if (s[1] == R.GetValue("Modifiable"))
					{
						await FillAsync(s, e, false);
					}
				}
			}
		}

		/// <summary>
		/// Greet with somebody.
		/// </summary>
		/// <param name="e">The event arguments.</param>
		/// <returns>The task of the method.</returns>
		private static async Task GreetingAsync(MessageReceivedEventArgs e) =>
			await e.Reply(R.GetValue("MorningToo"));

		/// <summary>
		/// Show helper text.
		/// </summary>
		/// <param name="e">The event arguments.</param>
		/// <returns>The task of this method.</returns>
		private static async Task ShowHelperTextAsync(MessageReceivedEventArgs e) =>
			await e.Source.SendAsync(
				new StringBuilder(R.GetValue("Help1"))
				.AppendLine()
				.AppendLine(R.GetValue("Help2"))
				.AppendLine()
				.AppendLine(R.GetValue("HelpHelp"))
				.AppendLine(R.GetValue("HelpAnalyze"))
				.AppendLine(R.GetValue("HelpGeneratePicture"))
				.AppendLine(R.GetValue("HelpClean"))
				.AppendLine(R.GetValue("HelpEmpty"))
				.AppendLine(R.GetValue("HelpStartDrawing"))
				.AppendLine(R.GetValue("HelpFillGiven"))
				.AppendLine(R.GetValue("HelpFillModifiable"))
				//.AppendLine(R.GetValue("HelpFillCandidate"))
				//.AppendLine(R.GetValue("HelpDrawCell"))
				//.AppendLine(R.GetValue("HelpDrawCandidate"))
				//.AppendLine(R.GetValue("HelpDrawRegion"))
				//.AppendLine(R.GetValue("HelpDrawRow"))
				//.AppendLine(R.GetValue("HelpDrawColumn"))
				//.AppendLine(R.GetValue("HelpDrawBlock"))
				//.AppendLine(R.GetValue("HelpDrawChain"))
				//.AppendLine(R.GetValue("HelpDrawCross"))
				//.AppendLine(R.GetValue("HelpDrawCircle"))
				.AppendLine(R.GetValue("HelpClose"))
				.AppendLine(R.GetValue("HelpIntroduceMyself"))
				.AppendLine()
				.AppendLine(R.GetValue("Help3"))
				.AppendLine(R.GetValue("Help4"))
				.AppendLine()
				.ToString());

		/// <summary>
		/// Introduce myself.
		/// </summary>
		/// <param name="e">The event arguments.</param>
		/// <returns>The task of this method.</returns>
		private static async Task IntroduceAsync(MessageReceivedEventArgs e) =>
			await e.Source.SendAsync(
				new StringBuilder(R.GetValue("Introduce1"))
				.AppendLine()
				.Append(R.GetValue("Introduce2"))
				.ToString());

		/// <summary>
		/// Analyze the puzzle.
		/// </summary>
		/// <param name="info">The command arguments.</param>
		/// <param name="e">The event arguments.</param>
		/// <returns>The task of this method.</returns>
		private static async Task AnalysisAsync(string info, MessageReceivedEventArgs e)
		{
			string analysisCommand = R.GetValue("AnalysisCommand");
			if (!SudokuGrid.TryParse(info[analysisCommand.Length..].Trim(), out var grid) || !grid.IsValid())
			{
				return;
			}

			var analysisResult = await new ManualSolver().SolveAsync(grid, null);
			await e.Reply(analysisResult.ToString("-!", CountryCode.ZhCn));

			var painter = new GridPainter(new(Size, Size), new(), analysisResult.Solution!);
			using var image = painter.Draw();
			await e.ReplyImageWithTextAsync(image, $"{R.GetValue("Analysis1")}{Environment.NewLine}");
		}

		/// <summary>
		/// Draw the image.
		/// </summary>
		/// <param name="info">The information.</param>
		/// <param name="e">The event arguments.</param>
		/// <returns>The task of this method.</returns>
		private static async Task DrawImageAsync(string info, MessageReceivedEventArgs e)
		{
			if (e.Message.Content.Count != 1)
			{
				return;
			}

			string drawingCommand = R.GetValue("DrawingCommand");
			if (!SudokuGrid.TryParse(info[drawingCommand.Length..].Trim(), out var grid) || !grid.IsValid())
			{
				return;
			}

			var painter = new GridPainter(new(Size, Size), new(), grid);
			using var image = painter.Draw();
			await e.ReplyImageAsync(image);
		}

		/// <summary>
		/// Generate an empty grid.
		/// </summary>
		/// <param name="e">The event arguments.</param>
		/// <returns>The task of this method.</returns>
		private static async Task GenerateEmptyGridAsync(MessageReceivedEventArgs e)
		{
			var painter = new GridPainter(new(Size, Size), new(), SudokuGrid.Undefined);
			using var image = painter.Draw();
			await e.ReplyImageAsync(image);
		}

		/// <summary>
		/// Clean the grid, and send the grid picture.
		/// </summary>
		/// <param name="info">The command arguments.</param>
		/// <param name="e">The event arguments.</param>
		/// <returns>The task of this method.</returns>
		private static async Task CleanGridAsync(string info, MessageReceivedEventArgs e)
		{
			string cleanCommand = R.GetValue("CleaningGridCommand");
			if (!SudokuGrid.TryParse(info[cleanCommand.Length..].Trim(), out var grid) || !grid.IsValid())
			{
				return;
			}

			grid.Clean();
			var painter = new GridPainter(new(Size, Size), new(), grid);

			using var image = painter.Draw();
			await e.ReplyImageAsync(image);
		}

		/// <summary>
		/// Start drawing picture.
		/// </summary>
		/// <param name="s">The command arguments.</param>
		/// <param name="e">The event arguments.</param>
		/// <returns>The task of this method.</returns>
		private static async Task StartDrawingAsync(string[] s, MessageReceivedEventArgs e)
		{
			if (s[1] != R.GetValue("Size"))
			{
				return;
			}

			if (!uint.TryParse(s[2], out uint result) || result > 1000U)
			{
				return;
			}

			var pointConverter = new PointConverter(result, result);
			_painter = new(pointConverter, new(), SudokuGrid.Undefined);

			using var image = _painter.Draw();
			await e.ReplyImageAsync(image);
		}

		private static async Task FillAsync(string[] s, MessageReceivedEventArgs e, bool appendGiven)
		{
			if (_painter is null)
			{
				return;
			}

			if (s.Length != 5)
			{
				return;
			}

			if (s[3] != R.GetValue("To"))
			{
				return;
			}

			if (!byte.TryParse(s[2], out byte digit) || digit is <= 0 or > 9)
			{
				return;
			}

			string str = s[4];
			switch (str.Length)
			{
				case 2
				when str[0] is var a && a is >= 'A' and <= 'I' or >= 'a' and <= 'i'
				&& str[1] is var b and >= '1' and <= '9': // A-I 1-9
				{
					int row, column;
					row = a is >= 'A' and <= 'I' ? a - 'A' : a - 'a';
					column = b - '1';

					int cell = row * 9 + column;

					_painter.Grid[cell] = digit - 1;
					if (appendGiven)
					{
						_painter.Grid.SetStatus(cell, CellStatus.Given);
					}

					using var image = _painter.Draw();
					await e.ReplyImageAsync(image);

					break;
				}
				case 4
				when str[0] is 'r' or 'R' && str[2] is 'c' or 'C'
				&& str[1] is var r and >= '1' and <= '9'
				&& str[3] is var c and >= '1' and <= '9': // rxcy
				{
					int row = r - '1', column = c - '1';
					int cell = row * 9 + column;

					_painter.Grid[cell] = digit - 1;
					if (appendGiven)
					{
						_painter.Grid.SetStatus(cell, CellStatus.Given);
					}

					using var image = _painter.Draw();
					await e.ReplyImageAsync(image);

					break;
				}
			}
		}

		private static async Task DisposePainterAsync(MessageReceivedEventArgs e)
		{
			_painter = null;

			await e.Reply(R.GetValue("Success"));
		}
	}
}
