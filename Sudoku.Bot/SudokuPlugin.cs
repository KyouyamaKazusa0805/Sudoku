using System;
using System.IO;
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
		/// Indicates the analysis command.
		/// </summary>
		private const string AnalysisCommand = "分析";

		/// <summary>
		/// Indicates the drawing command.
		/// </summary>
		private const string DrawingCommand = "生成图片";

		/// <summary>
		/// Indicates the generating empty grid command.
		/// </summary>
		private const string GenerateEmptyGridCommand = "生成空盘";

		/// <summary>
		/// Indicates the cleaning grid command.
		/// </summary>
		private const string CleaningGridCommand = "清盘";

		/// <summary>
		/// Indicates the introducing command.
		/// </summary>
		private const string IntroducingCommand = "小蛋蛋，介绍一下你吧";


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
			if (info.Trim() == IntroducingCommand)
			{
				await IntroduceAsync(e);
			}
			else if (info.StartsWith(AnalysisCommand))
			{
				await AnalysisAsync(info, e);
			}
			else if (info.StartsWith(DrawingCommand))
			{
				await DrawImageAsync(info, e);
			}
			else if (info.StartsWith(GenerateEmptyGridCommand))
			{
				await GenerateEmptyGridAsync(e);
			}
			else if (info.StartsWith(CleaningGridCommand))
			{
				await CleanGridAsync(info, e);
			}
		}

		private static async Task IntroduceAsync(MessageReceivedEventArgs e) =>
			await e.Source.SendAsync(
				"Hello 大家好！我是一个机器人，叫小蛋蛋，是女孩子哦 (✿◡‿◡)" +
				Environment.NewLine +
				"我爸还在给我加别的功能，所以我现在还需要大家的鼓励和支持哦，蟹蟹~");

		private static async Task AnalysisAsync(string info, MessageReceivedEventArgs e)
		{
			if (!SudokuGrid.TryParse(info[AnalysisCommand.Length..].Trim(), out var grid)
				|| !grid.IsValid())
			{
				return;
			}

			var analysisResult = await new ManualSolver().SolveAsync(grid, null);
			await e.Reply(analysisResult.ToString("-!", CountryCode.ZhCn));

			var painter = new GridPainter(new(Size, Size), new(), analysisResult.Solution!);
			await ReplyPictureAsync(painter, e, $"答案盘：{Environment.NewLine}");
		}

		private static async Task DrawImageAsync(string info, MessageReceivedEventArgs e)
		{
			if (e.Message.Content.Count != 1)
			{
				return;
			}

			if (!SudokuGrid.TryParse(info[DrawingCommand.Length..].Trim(), out var grid) || !grid.IsValid())
			{
				return;
			}

			var painter = new GridPainter(new(Size, Size), new(), grid);
			await ReplyPictureAsync(painter, e, null);
		}

		private static async Task GenerateEmptyGridAsync(MessageReceivedEventArgs e)
		{
			var painter = new GridPainter(new(Size, Size), new(), SudokuGrid.Undefined);
			await ReplyPictureAsync(painter, e, null);
		}

		private static async Task CleanGridAsync(string info, MessageReceivedEventArgs e)
		{
			if (!SudokuGrid.TryParse(info[CleaningGridCommand.Length..].Trim(), out var grid) || !grid.IsValid())
			{
				return;
			}

			grid.Clean();
			var painter = new GridPainter(new(Size, Size), new(), grid);
			await ReplyPictureAsync(painter, e, null);
		}

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
