using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using HuajiTech.Mirai;
using HuajiTech.Mirai.Events;
using HuajiTech.Mirai.Messaging;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Globalization;
using Sudoku.Solving.Checking;
using Sudoku.Solving.Extensions;
using Sudoku.Solving.Manual;
using HImage = HuajiTech.Mirai.Messaging.Image;

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
			if (info.ToLower().Trim() == ResourceDictionary.GetValue("HelpCommand"))
			{
				await ShowHelperTextAsync(e);
			}
			else if (info.Trim() == ResourceDictionary.GetValue("IntroducingCommand"))
			{
				await IntroduceAsync(e);
			}
			else if (info.StartsWith(ResourceDictionary.GetValue("AnalysisCommand")))
			{
				await AnalysisAsync(info, e);
			}
			else if (info.StartsWith(ResourceDictionary.GetValue("DrawingCommand")))
			{
				await DrawImageAsync(info, e);
			}
			else if (info.StartsWith(ResourceDictionary.GetValue("GenerateEmptyGridCommand")))
			{
				await GenerateEmptyGridAsync(e);
			}
			else if (info.StartsWith(ResourceDictionary.GetValue("CleaningGridCommand")))
			{
				await CleanGridAsync(info, e);
			}
		}

		/// <summary>
		/// Show helper text.
		/// </summary>
		/// <param name="e">The event arguments.</param>
		/// <returns>The task of this method.</returns>
		private static async Task ShowHelperTextAsync(MessageReceivedEventArgs e) =>
			await e.Source.SendAsync(
				new StringBuilder(ResourceDictionary.GetValue("Help1"))
				.AppendLine()
				.AppendLine(ResourceDictionary.GetValue("Help2"))
				.AppendLine()
				.AppendLine(ResourceDictionary.GetValue("HelpHelp"))
				.AppendLine(ResourceDictionary.GetValue("HelpAnalyze"))
				.AppendLine(ResourceDictionary.GetValue("HelpGeneratePicture"))
				.AppendLine(ResourceDictionary.GetValue("HelpClean"))
				.AppendLine(ResourceDictionary.GetValue("HelpEmpty"))
				.AppendLine(ResourceDictionary.GetValue("HelpIntroduceMyself"))
				.AppendLine()
				.AppendLine(ResourceDictionary.GetValue("Help3"))
				.AppendLine()
				.ToString());

		/// <summary>
		/// Introduce myself.
		/// </summary>
		/// <param name="e">The event arguments.</param>
		/// <returns>The task of this method.</returns>
		private static async Task IntroduceAsync(MessageReceivedEventArgs e) =>
			await e.Source.SendAsync(
				new StringBuilder(ResourceDictionary.GetValue("Introduce1"))
				.AppendLine()
				.Append(ResourceDictionary.GetValue("Introduce2"))
				.ToString());

		/// <summary>
		/// Analyze the puzzle.
		/// </summary>
		/// <param name="info">The command arguments.</param>
		/// <param name="e">The event arguments.</param>
		/// <returns>The task of this method.</returns>
		private static async Task AnalysisAsync(string info, MessageReceivedEventArgs e)
		{
			string analysisCommand = ResourceDictionary.GetValue("AnalysisCommand");
			if (!SudokuGrid.TryParse(info[analysisCommand.Length..].Trim(), out var grid)
				|| !grid.IsValid())
			{
				return;
			}

			var analysisResult = await new ManualSolver().SolveAsync(grid, null);
			await e.Reply(analysisResult.ToString("-!", CountryCode.ZhCn));

			var painter = new GridPainter(new(Size, Size), new(), analysisResult.Solution!);
			await ReplyPictureAsync(painter, e, $"答案盘：{Environment.NewLine}");
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

			string drawingCommand = ResourceDictionary.GetValue("DrawingCommand");
			if (!SudokuGrid.TryParse(info[drawingCommand.Length..].Trim(), out var grid) || !grid.IsValid())
			{
				return;
			}

			var painter = new GridPainter(new(Size, Size), new(), grid);
			await ReplyPictureAsync(painter, e, null);
		}

		/// <summary>
		/// Generate an empty grid.
		/// </summary>
		/// <param name="e">The event arguments.</param>
		/// <returns>The task of this method.</returns>
		private static async Task GenerateEmptyGridAsync(MessageReceivedEventArgs e)
		{
			var painter = new GridPainter(new(Size, Size), new(), SudokuGrid.Undefined);
			await ReplyPictureAsync(painter, e, null);
		}

		/// <summary>
		/// Clean the grid, and send the grid picture.
		/// </summary>
		/// <param name="info">The command arguments.</param>
		/// <param name="e">The event arguments.</param>
		/// <returns>The task of this method.</returns>
		private static async Task CleanGridAsync(string info, MessageReceivedEventArgs e)
		{
			string cleanCommand = ResourceDictionary.GetValue("CleaningGridCommand");
			if (!SudokuGrid.TryParse(info[cleanCommand.Length..].Trim(), out var grid) || !grid.IsValid())
			{
				return;
			}

			grid.Clean();
			var painter = new GridPainter(new(Size, Size), new(), grid);
			await ReplyPictureAsync(painter, e, null);
		}

		/// <summary>
		/// Reply picture.
		/// </summary>
		/// <param name="painter">The painter.</param>
		/// <param name="e">The event arguments.</param>
		/// <param name="otherMessage">Othe message to add.</param>
		/// <returns>The task of this method.</returns>
		private static async Task ReplyPictureAsync(
			GridPainter painter, MessageReceivedEventArgs e, string? otherMessage)
		{
			var image = painter.Draw();

			const string tempPath = @"C:\Users\Howdy\Desktop\Temp.png";
			image.Save(tempPath);

			var hImage = new HImage(new Uri(tempPath));
			await e.Reply((otherMessage ?? string.Empty) + hImage);

			if (File.Exists(tempPath))
			{
				File.Delete(tempPath);
			}
		}
	}
}
