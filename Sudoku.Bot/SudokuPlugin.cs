using System;
using System.Text;
using System.Threading.Tasks;
using HuajiTech.Mirai;
using HuajiTech.Mirai.Events;
using HuajiTech.Mirai.Messaging;
using Sudoku.Bot.Extensions;
using Sudoku.Constants;
using Sudoku.Data;
using Sudoku.Data.Extensions;
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
			switch (info)
			{
				case var _ when info.Contains(R.GetValue("MyName")) && info.Contains(R.GetValue("Morning")):
				{
					await GreetingAsync(e);
					break;
				}
				default:
				{
					switch (info.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
					{
						case { Length: >= 1 } s:
						{
							switch (s[0])
							{
								case "！帮助": await ShowHelperTextAsync(e); break;
								case "！分析" when s.Length == 2:
								{
									switch (s[1])
									{
										case "？": await AnalyzeHelpAsync(e); break;
										default: await AnalysisAsync(info, e); break;
									}
									break;
								}
								case "！生成图片" when s.Length == 2:
								{
									switch (s[1])
									{
										case "？": await GeneratePictureHelpAsync(e); break;
										default: await DrawImageAsync(info, e); break;
									}
									break;
								}
								case "！生成空盘":
								{
									switch (s.Length)
									{
										case 1: await GenerateEmptyGridAsync(e); break;
										case >= 2 when s[1] is "？": await GenerateEmptyGridHelpAsync(e); break;
									}
									break;
								}
								case "！清盘" when s.Length == 2:
								{
									switch (s[1])
									{
										case "？": await CleanGridHelpAsync(e); break;
										default: await CleanGridAsync(info, e); break;
									}
									break;
								}
								case "！关于":
								{
									switch (s.Length)
									{
										case 1: await IntroduceAsync(e); break;
										case 2 when s[1] == "？": await IntroduceHelpAsync(e); break;
									}
									break;
								}
								case "！开始绘图" when s.Length >= 2:
								{
									switch (s[1])
									{
										case "？": await StartDrawingHelpAsync(e); break;
										default: await StartDrawingAsync(info, s, e); break;
									}
									break;
								}
								case "！结束绘图":
								{
									switch (s.Length)
									{
										case 1: await DisposePainterAsync(e); break;
										case 2 when s[1] == "？": await DisposePainterHelpAsync(e); break;
									}
									break;
								}
								case "！填入" when s.Length >= 2:
								{
									switch (s[1])
									{
										case "？": await FillHelpAsync(e); break;
										case "提示数": await FillAsync(s, e, true); break;
										case "填入数": await FillAsync(s, e, false); break;
									}
									break;
								}
								case "！画" when s.Length >= 2:
								{
									switch (s[1])
									{
										case "？": await DrawHelpAsync(e, false); break;
										case "单元格": await DrawCellAsync(s, e, false); break;
										case "候选数": await DrawCandidateAsync(s, e, false); break;
										case "行": await DrawRowAsync(s, e, false); break;
										case "列": await DrawColumnAsync(s, e, false); break;
										case "宫": await DrawBlockAsync(s, e, false); break;
										case "链": await DrawChainAsync(s, e, false); break;
										case "叉叉": await DrawCrossAsync(s, e, false); break;
										case "圆圈": await DrawCircleAsync(s, e, false); break;
									}
									break;
								}
								case "！去掉" when s.Length >= 2:
								{
									switch (s[1])
									{
										case "？": await DrawHelpAsync(e, true); break;
										case "单元格": await DrawCellAsync(s, e, true); break;
										case "候选数": await DrawCandidateAsync(s, e, true); break;
										case "行": await DrawRowAsync(s, e, true); break;
										case "列": await DrawColumnAsync(s, e, true); break;
										case "宫": await DrawBlockAsync(s, e, true); break;
										case "链": await DrawChainAsync(s, e, true); break;
										case "叉叉": await DrawCrossAsync(s, e, true); break;
										case "圆圈": await DrawCircleAsync(s, e, true); break;
									}
									break;
								}
							}
							break;
						}
					}
					break;
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

		private static async Task AnalyzeHelpAsync(MessageReceivedEventArgs e) =>
			await e.Source.SendAsync(
				new StringBuilder()
				.AppendLine("格式：！分析 <题目>。")
				.Append("题目代码目前可支持普通文本格式、Hodoku 的中间盘面格式等。")
				.ToString());

		private static async Task GeneratePictureHelpAsync(MessageReceivedEventArgs e) =>
			await e.Source.SendAsync(
				new StringBuilder()
				.AppendLine("格式：！生成图片 <题目>。")
				.Append("题目代码目前可支持普通文本格式、Hodoku 的中间盘面格式等。")
				.ToString());

		private static async Task GenerateEmptyGridHelpAsync(MessageReceivedEventArgs e) =>
			await e.Source.SendAsync("格式：！生成空盘。");

		private static async Task CleanGridHelpAsync(MessageReceivedEventArgs e) =>
			await e.Source.SendAsync(
				new StringBuilder()
				.AppendLine("格式：！清盘 <题目>。")
				.Append("题目代码目前可支持普通文本格式、Hodoku 的中间盘面格式等。")
				.ToString());

		private static async Task IntroduceHelpAsync(MessageReceivedEventArgs e) =>
			await e.Source.SendAsync("格式：！关于。");

		private static async Task StartDrawingHelpAsync(MessageReceivedEventArgs e) =>
			await e.Source.SendAsync(
				new StringBuilder()
				.AppendLine("格式：！开始绘图[ 大小 <大小>][ 盘面 <题目>]。")
				.AppendLine("图片大小需要是一个不超过 1000 的正整数，表示多大的图片，以像素为单位；")
				.AppendLine("题目代码目前可支持普通文本格式、Hodoku 的中间盘面格式等。")
				.Append("中括号项为可选内容，可以不写。")
				.ToString());

		private static async Task DisposePainterHelpAsync(MessageReceivedEventArgs e) =>
			await e.Source.SendAsync("格式：！结束绘图。");

		private static async Task FillHelpAsync(MessageReceivedEventArgs e) =>
			await e.Source.SendAsync(
				new StringBuilder()
				.AppendLine("格式：！填入 (提示数|填入数) <数字> 到 <单元格>。")
				.AppendLine("小括号里的内容是可选项，但必须从小括号里以竖线分隔的项里选择一个。")
				.ToString());

		private static async Task DrawHelpAsync(MessageReceivedEventArgs e, bool remove) =>
			await e.Source.SendAsync(
				new StringBuilder()
				.AppendLine("格式：")
				.AppendLine($"！{(remove ? "去掉" : "画")} 单元格 <单元格> <颜色>；")
				.AppendLine($"！{(remove ? "去掉" : "画")} 候选数 <单元格> <数字> <颜色>；")
				.AppendLine($"！{(remove ? "去掉" : "画")} 行 <行编号> <颜色>；")
				.AppendLine($"！{(remove ? "去掉" : "画")} 列 <列编号> <颜色>；")
				.AppendLine($"！{(remove ? "去掉" : "画")} 宫 <宫编号> <颜色>；")
				.AppendLine($"！{(remove ? "去掉" : "画")} 链 从 <单元格> <数字> 到 <单元格> <数字>")
				.AppendLine($"！{(remove ? "去掉" : "画")} 叉叉 <单元格>；")
				.AppendLine($"！{(remove ? "去掉" : "画")} 圆圈 <单元格>。")
				.AppendLine("其中“颜色”项可设置为红、橙、黄、绿、青、蓝、紫七种颜色以及对应的浅色；但黄色除外。")
				.AppendLine("“颜色”同样支持 ARGB 颜色序列（透明分量、红色度、绿色度、蓝色度）。")
				.ToString());


		/// <summary>
		/// Show helper text.
		/// </summary>
		/// <param name="e">The event arguments.</param>
		/// <returns>The task of this method.</returns>
		private static async Task ShowHelperTextAsync(MessageReceivedEventArgs e) =>
			await e.Source.SendAsync(
				new StringBuilder()
				.AppendLine("格式：！帮助。")
				.AppendLine("机器人可识别的功能有以下一些：")
				.AppendLine("！帮助：显示此帮助信息。")
				.AppendLine("！分析 <盘面>：显示题目的分析结果。")
				.AppendLine("！生成图片 <盘面>：将题目文本转为图片显示。")
				.AppendLine("！清盘 <盘面>：将盘面前期的排除、唯一余数、区块和数组技巧全部应用。")
				.AppendLine("！生成空盘：给一个空盘的图片。")
				.AppendLine("！开始绘图[ 大小 <图片大小>][ 盘面 <盘面>]：开始从空盘画盘面图，随后可以添加其它的操作，例如添加候选数涂色等。")
				.AppendLine("！结束绘图：指定画图过程结束，清除画板。")
				.AppendLine("！关于：我 介 绍 我 自 己")
				.AppendLine()
				.AppendLine("注：为和普通聊天信息作区分，命令前面的叹号“！”也是命令的一部分；")
				.AppendLine("每一个命令的详情信息请输入“命令 ？”来获取。")
				.AppendLine()
				.Append(R.GetValue("MyName"))
				.ToString());

		/// <summary>
		/// Introduce myself.
		/// </summary>
		/// <param name="e">The event arguments.</param>
		/// <returns>The task of this method.</returns>
		private static async Task IntroduceAsync(MessageReceivedEventArgs e) =>
			await e.Source.SendAsync(
				new StringBuilder()
				.AppendLine("Hello 大家好！我是一个机器人，叫小蛋蛋，是女孩子哦 (✿◡‿◡)")
				.Append("我爸还在给我加别的功能，所以我现在还需要大家的鼓励和支持哦，蟹蟹~")
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
			await e.Source.SendAsync(analysisResult.ToString("-!", CountryCode.ZhCn));
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

			string GeneratePictureCommand = R.GetValue("GeneratePictureCommand");
			if (!SudokuGrid.TryParse(info[GeneratePictureCommand.Length..].Trim(), out var grid)
				|| !grid.IsValid())
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
		/// <param name="info">The command arguments.</param>
		/// <param name="s">The command arguments.</param>
		/// <param name="e">The event arguments.</param>
		/// <returns>The task of this method.</returns>
		private static async Task StartDrawingAsync(string info, string[] s, MessageReceivedEventArgs e)
		{
			// 开始绘图 [大小 <大小>] [盘面 <盘面>]
			int size = Size;
			var grid = SudokuGrid.Undefined;
			int pos = info.IndexOf(R.GetValue("Size"));
			if (pos != -1)
			{
				int startPos = pos + R.GetValue("Size").Length;
				if (info[startPos] != ' ')
				{
					return;
				}

				int index = startPos + 1;
				for (; info[index] is >= '0' and <= '9'; index++) ;

				string sizeStr = info[startPos..index];
				if (!int.TryParse(sizeStr, out int result))
				{
					return;
				}

				size = result;
			}

			pos = info.IndexOf(R.GetValue("Grid"));
			if (pos != -1)
			{
				int startPos = pos + R.GetValue("Grid").Length;
				if (info[startPos] != ' ')
				{
					return;
				}

				int index = startPos + 1;
				string gridStr = info[startPos..];
				if (!SudokuGrid.TryParse(gridStr, out var result))
				{
					return;
				}

				grid = result;
			}

			var pointConverter = new PointConverter(size, size);
			_painter = new(pointConverter, new(), grid);

			using var image = _painter.Draw();
			await e.ReplyImageAsync(image);
		}

		/// <summary>
		/// Fill the given and modifiable.
		/// </summary>
		/// <param name="s">The command arguments.</param>
		/// <param name="e">The event arguments.</param>
		/// <param name="appendGiven">Indicates whether we should add given values.</param>
		/// <returns>The task of this method.</returns>
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

			if (!Parser.TryParseCells(s[4], out var map))
			{
				return;
			}

			foreach (int cell in map)
			{
				if (((SudokuGrid)_painter.Grid).Duplicate(cell, digit))
				{
					continue;
				}

				_painter.Grid[cell] = digit - 1;
				if (appendGiven)
				{
					_painter.Grid.SetStatus(cell, CellStatus.Given);
				}
			}

			using var image = _painter.Draw();
			await e.ReplyImageAsync(image);
		}

		/// <summary>
		/// Draw a cell.
		/// </summary>
		/// <param name="s">The command arguments.</param>
		/// <param name="e">The event arguments.</param>
		/// <param name="remove">Indicates whether the current operation is removing the element.</param>
		/// <returns>The task of this method.</returns>
		private static async Task DrawCellAsync(string[] s, MessageReceivedEventArgs e, bool remove)
		{
			if (_painter is null)
			{
				return;
			}

			if (s.Length != 4 - (remove ? 1 : 0))
			{
				return;
			}

			if (!Parser.TryParseCells(s[2], out var map))
			{
				return;
			}

			long colorId = default;
			if (!remove && !Parser.TryParseColorId(s[3], true, out colorId))
			{
				return;
			}

			if (_painter.CustomView is null)
			{
				_painter = _painter with { CustomView = new() };
			}

			foreach (int cell in map)
			{
				if (remove)
				{
					_painter.CustomView.RemoveCell(cell);
				}
				else
				{
					_painter.CustomView.AddCell(colorId, cell);
				}
			}

			using var image = _painter.Draw();
			await e.ReplyImageAsync(image);
		}

		/// <summary>
		/// Draw a candidate.
		/// </summary>
		/// <param name="s">The command arguments.</param>
		/// <param name="e">The event arguments.</param>
		/// <param name="remove">Indicates whether the current operation is removing the element.</param>
		/// <returns>The task of this method.</returns>
		private static async Task DrawCandidateAsync(string[] s, MessageReceivedEventArgs e, bool remove)
		{
			if (_painter is null)
			{
				return;
			}

			if (s.Length != 5 - (remove ? 1 : 0))
			{
				return;
			}

			if (!Parser.TryParseCells(s[2], out var map))
			{
				return;
			}

			if (!byte.TryParse(s[3], out byte digit) || digit is <= 0 or > 9)
			{
				return;
			}

			long colorId = default;
			if (!remove && !Parser.TryParseColorId(s[4], false, out colorId))
			{
				return;
			}

			if (_painter.CustomView is null)
			{
				_painter = _painter with { CustomView = new() };
			}

			foreach (int cell in map)
			{
				int candidate = cell * 9 + digit - 1;
				if (remove)
				{
					_painter.CustomView.RemoveCandidate(candidate);
				}
				else
				{
					_painter.CustomView.AddCandidate(colorId, candidate);
				}
			}

			using var image = _painter.Draw();
			await e.ReplyImageAsync(image);
		}

		/// <summary>
		/// Draw a row.
		/// </summary>
		/// <param name="s">The command arguments.</param>
		/// <param name="e">The event arguments.</param>
		/// <param name="remove">Indicates whether the current operation is removing the element.</param>
		/// <returns>The task of this method.</returns>
		private static async Task DrawRowAsync(string[] s, MessageReceivedEventArgs e, bool remove)
		{
			if (s.Length != 4 - (remove ? 1 : 0))
			{
				return;
			}

			if (!byte.TryParse(s[2], out byte region) || region is <= 0 or > 9)
			{
				return;
			}

			long colorId = default;
			if (!remove && !Parser.TryParseColorId(s[3], true, out colorId))
			{
				return;
			}

			await DrawRegionAsync(RegionLabel.Row, region - 1, colorId, e, remove);
		}

		/// <summary>
		/// Draw a column.
		/// </summary>
		/// <param name="s">The command arguments.</param>
		/// <param name="e">The event arguments.</param>
		/// <param name="remove">Indicates whether the current operation is removing the element.</param>
		/// <returns>The task of this method.</returns>
		private static async Task DrawColumnAsync(string[] s, MessageReceivedEventArgs e, bool remove)
		{
			if (s.Length != 4 - (remove ? 1 : 0))
			{
				return;
			}

			if (!byte.TryParse(s[2], out byte region) || region is <= 0 or > 9)
			{
				return;
			}

			long colorId = default;
			if (!remove && !Parser.TryParseColorId(s[3], true, out colorId))
			{
				return;
			}

			await DrawRegionAsync(RegionLabel.Column, region - 1, colorId, e, remove);
		}

		/// <summary>
		/// Draw a block.
		/// </summary>
		/// <param name="s">The command arguments.</param>
		/// <param name="e">The event arguments.</param>
		/// <param name="remove">Indicates whether the current operation is removing the element.</param>
		/// <returns>The task of this method.</returns>
		private static async Task DrawBlockAsync(string[] s, MessageReceivedEventArgs e, bool remove)
		{
			if (s.Length != 4 - (remove ? 1 : 0))
			{
				return;
			}

			if (!byte.TryParse(s[2], out byte region) || region is <= 0 or > 9)
			{
				return;
			}

			long colorId = default;
			if (!remove && !Parser.TryParseColorId(s[3], true, out colorId))
			{
				return;
			}

			await DrawRegionAsync(RegionLabel.Block, region - 1, colorId, e, remove);
		}

		/// <summary>
		/// Draw a region.
		/// </summary>
		/// <param name="label">The region label.</param>
		/// <param name="region">The region.</param>
		/// <param name="colorId">The color ID.</param>
		/// <param name="e">The event arguments.</param>
		/// <param name="remove">Indicates whether the current operation is removing the element.</param>
		/// <returns>The task of this method.</returns>
		private static async Task DrawRegionAsync(
			RegionLabel label, int region, long colorId, MessageReceivedEventArgs e, bool remove)
		{
			if (_painter is null)
			{
				return;
			}

			if (_painter.CustomView is null)
			{
				_painter = _painter with { CustomView = new() };
			}

			int r = (int)label * 9 + region;
			if (remove)
			{
				_painter.CustomView.RemoveRegion(r);
			}
			else
			{
				_painter.CustomView.AddRegion(colorId, r);
			}

			using var image = _painter.Draw();
			await e.ReplyImageAsync(image);
		}

		/// <summary>
		/// Draw a chain.
		/// </summary>
		/// <param name="s">The command arguments.</param>
		/// <param name="e">The event arguments.</param>
		/// <param name="remove">Indicates whether the current operation is removing the element.</param>
		/// <returns>The task of this method.</returns>
		private static async Task DrawChainAsync(string[] s, MessageReceivedEventArgs e, bool remove)
		{
			if (_painter is null)
			{
				return;
			}

			if (s.Length != 8)
			{
				return;
			}

			if (s[2] != R.GetValue("From"))
			{
				return;
			}

			if (!Parser.TryParseCell(s[3], out byte r1, out byte c1)
				|| !Parser.TryParseCell(s[6], out byte r2, out byte c2))
			{
				return;
			}

			if (!byte.TryParse(s[4], out byte d1) || !byte.TryParse(s[7], out byte d2)
				|| d1 is <= 0 or > 9 || d2 is <= 0 or > 9)
			{
				return;
			}

			if (_painter.CustomView is null)
			{
				_painter = _painter with { CustomView = new() };
			}

			var link = new Link(r1 * 81 + c1 * 9 + (d1 - 1), r2 * 81 + c2 * 9 + (d2 - 1), LinkType.Default);
			if (remove)
			{
				_painter.CustomView.RemoveLink(link);
			}
			else
			{
				_painter.CustomView.AddLink(link);
			}

			using var image = _painter.Draw();
			await e.ReplyImageAsync(image);
		}

		/// <summary>
		/// Draw a cross sign.
		/// </summary>
		/// <param name="s">The command arguments.</param>
		/// <param name="e">The event arguments.</param>
		/// <param name="remove">Indicates whether the current operation is removing the element.</param>
		/// <returns>The task of this method.</returns>
		private static async Task DrawCrossAsync(string[] s, MessageReceivedEventArgs e, bool remove)
		{
			if (_painter is null)
			{
				return;
			}

			if (s.Length != 3)
			{
				return;
			}

			if (!Parser.TryParseCells(s[2], out var map))
			{
				return;
			}

			if (_painter.CustomView is null)
			{
				_painter = _painter with { CustomView = new() };
			}

			foreach (int cell in map)
			{
				if (remove)
				{
					_painter.CustomView.RemoveDirectLine(GridMap.Empty, new() { cell });
				}
				else
				{
					_painter.CustomView.AddDirectLine(GridMap.Empty, new() { cell });
				}
			}

			using var image = _painter.Draw();
			await e.ReplyImageAsync(image);
		}

		/// <summary>
		/// Draw a circle sign.
		/// </summary>
		/// <param name="s">The command arguments.</param>
		/// <param name="e">The event arguments.</param>
		/// <param name="remove">Indicates whether the current operation is removing the element.</param>
		/// <returns>The task of this method.</returns>
		private static async Task DrawCircleAsync(string[] s, MessageReceivedEventArgs e, bool remove)
		{
			if (_painter is null)
			{
				return;
			}

			if (s.Length != 3)
			{
				return;
			}

			if (!Parser.TryParseCells(s[2], out var map))
			{
				return;
			}

			if (_painter.CustomView is null)
			{
				_painter = _painter with { CustomView = new() };
			}

			foreach (int cell in map)
			{
				if (remove)
				{
					_painter.CustomView.RemoveDirectLine(new() { cell }, GridMap.Empty);
				}
				else
				{
					_painter.CustomView.AddDirectLine(new() { cell }, GridMap.Empty);
				}
			}

			using var image = _painter.Draw();
			await e.ReplyImageAsync(image);
		}

		/// <summary>
		/// Dispose the painter.
		/// </summary>
		/// <param name="e">The event arguments.</param>
		/// <returns>The task of this method.</returns>
		private static async Task DisposePainterAsync(MessageReceivedEventArgs e)
		{
			_painter = null;

			await e.Reply(R.GetValue("Success"));
		}
	}
}
