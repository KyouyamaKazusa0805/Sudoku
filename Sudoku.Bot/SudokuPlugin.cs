#if AUTHOR_RESERVED

using System;
using System.IO;
using System.Linq;
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
using Sudoku.Solving;
using Sudoku.Solving.Checking;
using Sudoku.Solving.Extensions;
using Sudoku.Solving.Manual;
using Sudoku.Solving.Manual.Alses.Basic;
using Sudoku.Solving.Manual.Chaining;
using Sudoku.Solving.Manual.LastResorts;
using R = Sudoku.Bot.Resources;

namespace Sudoku.Bot
{
	/// <summary>
	/// 封装一个关于数独相关操作的插件。
	/// </summary>
	public sealed class SudokuPlugin
	{
		/// <summary>
		/// 默认的图片绘制大小。
		/// </summary>
		private const int Size = 800;

		/// <summary>
		/// 需要刷题的群的题库文件的文件夹路径。
		/// </summary>
		private const string PuzzleLibDir = @"S:\题库\机器人题库";

		/// <summary>
		/// 刷题的群已经完成了的题目的存储文件夹路径。
		/// </summary>
		private const string FinishedPuzzleDir = @"S:\题库\机器人题库_已完成";


		/// <summary>
		/// 随机数生成器。用来获取随机数。
		/// </summary>
		private static readonly Random Rng = new();


		/// <summary>
		/// 在程序内使用的绘图工具类。
		/// </summary>
		private static GridPainter? _painter;


		/// <summary>
		/// 直接实例化一个插件对象，包含相关的执行事件。
		/// </summary>
		/// <param name="currentUserEventSource">执行事件操作的事件源。</param>
		public SudokuPlugin(CurrentUserEventSource currentUserEventSource) =>
			currentUserEventSource.GroupMessageReceivedEvent += OnReceivingMessageAsync;

		/// <summary>
		/// 当接收到消息的时候，触发该事件。
		/// </summary>
		/// <param name="session">当前会话。</param>
		/// <param name="e">群消息事件参数。</param>
		/// <returns>当前事件处理方法的封装 <see cref="Task"/> 对象。</returns>
		private static async Task OnReceivingMessageAsync(Session session, GroupMessageReceivedEventArgs e)
		{
			var complexStr = e.Message.Content;
			if (complexStr is not { Count: not 0 } || complexStr[0] is not PlainText pl)
			{
				return;
			}

			string info = pl.ToString();
			string[] splits = info.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			switch (info[0])
			{
				case '#':
				{
					switch (splits)
					{
						case { Length: >= 1 } s:
						{
							switch (splits[0][1..])
							{
								case "禁言": await JinxGroupAsync(s, e); break;
								case "解除禁言" or "取消禁言": await UnjinxGroupAsync(s, e); break;
							}
							break;
						}
					}
					break;
				}
				case '！' or '!':
				{
					switch (splits)
					{
						case { Length: >= 1 } s:
						{
							switch (s[0][1..])
							{
								case "帮助": await ShowHelperTextAsync(e); break;
								case "分析" when s.Length == 2:
								{
									switch (s[1])
									{
										case "？": await AnalyzeHelpAsync(e); break;
										default: await AnalysisAsync(info, e); break;
									}
									break;
								}
								case "生成图片" when s.Length == 2:
								{
									switch (s[1])
									{
										case "？": await GeneratePictureHelpAsync(e); break;
										default: await DrawImageAsync(info, e); break;
									}
									break;
								}
								case "生成空盘":
								{
									switch (s.Length)
									{
										case 1: await GenerateEmptyGridAsync(e); break;
										case >= 2 when s[1] == "？": await GenerateEmptyGridHelpAsync(e); break;
									}
									break;
								}
								case "清盘" when s.Length == 2:
								{
									switch (s[1])
									{
										case "？": await CleanGridHelpAsync(e); break;
										default: await CleanGridAsync(info, e); break;
									}
									break;
								}
								case "关于":
								{
									switch (s.Length)
									{
										case 1: await IntroduceAsync(e); break;
										case 2 when s[1] == "？": await IntroduceHelpAsync(e); break;
									}
									break;
								}
								case "开始绘图" when s.Length >= 2:
								{
									switch (s[1])
									{
										case "？": await StartDrawingHelpAsync(e); break;
										default: await StartDrawingAsync(info, s, e); break;
									}
									break;
								}
								case "结束绘图":
								{
									switch (s.Length)
									{
										case 1: await DisposePainterAsync(e); break;
										case 2 when s[1] == "？": await DisposePainterHelpAsync(e); break;
									}
									break;
								}
								case "填入" when s.Length >= 2:
								{
									switch (s[1])
									{
										case "？": await FillHelpAsync(e); break;
										case "提示数": await FillAsync(s, e, true); break;
										case "填入数": await FillAsync(s, e, false); break;
									}
									break;
								}
								case "画" when s.Length >= 2:
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
								case "去掉" when s.Length >= 2:
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
								case "抽题":
								{
									switch (s.Length)
									{
										case >= 2 when s[1] == "？": await ExtractPuzzleHelpAsync(e); break;
										default: await ExtractPuzzleAsync(s, e); break;
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
		/// 抽取题目。
		/// </summary>
		private static async Task ExtractPuzzleAsync(string[] s, GroupMessageReceivedEventArgs e)
		{
			if (!Directory.Exists(PuzzleLibDir))
			{
				return;
			}

			string[] files = Directory.GetFiles(PuzzleLibDir);
			if (files.Length == 0)
			{
				return;
			}

			switch (s.Length)
			{
				case 1:
				{
					await outputPuzzlePictureAsync(true);
					break;
				}
				case 2:
				{
					await outputPuzzlePictureAsync(
						s[1] switch { "带候选数" => true, "不带候选数" => false, _ => null });
					break;
				}
			}

			async Task outputPuzzlePictureAsync(bool? withCandidates)
			{
				if (!withCandidates.HasValue)
				{
					return;
				}

				long groupNumber = e.Source.Number;
				string correspondingPath = $@"{PuzzleLibDir}\{groupNumber}.txt";
				string finishedPath = $@"{FinishedPuzzleDir}\{groupNumber}.txt";
				if (!File.Exists(correspondingPath))
				{
					await e.Reply("本群尚未拥有刷题题库，抽取题目失败。");
					return;
				}

				string[] fileLines = File.ReadAllLines(correspondingPath);

				AnalysisResult? analysisResult = null;
				SudokuGrid grid;
				int trial = 0;
				for (; trial < 10; trial++)
				{
					int lineChosen = Rng.Next(0, fileLines.Length);
					string puzzle = fileLines[lineChosen];
					bool parsed = SudokuGrid.TryParse(puzzle, out grid);
					if (!parsed)
					{
						continue;
					}

					bool exists = File.Exists(finishedPath);
					string str = grid.ToString();
					if (basicCondition(finishedPath, exists, str)
						&& customCondition(groupNumber, grid, out analysisResult))
					{
						break;
					}
				}

				if (trial == 10)
				{
					await e.Reply("随机抽题失败，可能是脸不好；请重试一次。");
				}
				else
				{
					// Clean the grid if the puzzle should show candidates.
					if (withCandidates.Value is var b && b)
					{
						grid.Clean();
					}

					// Save the puzzle to finished directory.
					if (!Directory.Exists(FinishedPuzzleDir))
					{
						Directory.CreateDirectory(FinishedPuzzleDir);
					}
					File.AppendAllText(finishedPath, $"{grid}\r\n");

					// Output the analysis result.
					analysisResult ??= new ManualSolver().Solve(grid);
					await e.Reply(analysisResult.ToString("-!", CountryCode.ZhCn));

					// Output the picture.
					var painter = new GridPainter(new(Size, Size), new() { ShowCandidates = b }, grid);
					await e.ReplyImageAsync(painter.Draw());
				}
			}

			static bool basicCondition(string finishedPath, bool exists, string str) =>
				exists && File.ReadLines(finishedPath).All(
					l =>
					{
						string[] z = l.Split('\t');
						return z.Length >= 1 && z[0] is var s && !string.IsNullOrEmpty(s) && s != str;
					}) || !exists;

			static bool customCondition(long groupNumber, in SudokuGrid grid, out AnalysisResult? analysisResult)
			{
				string path = $@"{PuzzleLibDir}\设置.txt";
				if (!File.Exists(path))
				{
					analysisResult = null;
					return true;
				}

				string[] lines = File.ReadAllLines(path);
				if (
					lines.FirstOrDefault(
						line =>
						{
							string[] sp = line.Split(' ');
							return sp.Length > 0 && sp[0] == groupNumber.ToString();
						}) is var resultLine || resultLine is null)
				{
					analysisResult = null;
					return true;
				}

				string[] currentLineSplits = resultLine.Split(' ');
				if (currentLineSplits.Length != 11
					|| currentLineSplits[1] != "链数"
					|| currentLineSplits[3] != "到"
					|| currentLineSplits[5] != "难度"
					|| currentLineSplits[7] != "到"
					|| currentLineSplits[9] != "强制链")
				{
					analysisResult = null;
					return true;
				}

				// Why 4144959? The only reason is that it's an abnormal value.
				uint chainCountMin = 4144959, chainCountMax = 4144959;
				decimal diffMin = 4144959, diffMax = 4144959;
				if (currentLineSplits[2] == "any") chainCountMin = 0;
				if (currentLineSplits[4] == "any") chainCountMax = 1000;
				if (currentLineSplits[6] == "any") diffMin = 0;
				if (currentLineSplits[8] == "any") diffMax = 20;

				if (chainCountMin != 4144959 && !uint.TryParse(currentLineSplits[2], out chainCountMin)
					|| chainCountMax != 4144959 && !uint.TryParse(currentLineSplits[4], out chainCountMax)
					|| diffMin != 4144959 && !decimal.TryParse(currentLineSplits[6], out diffMin)
					|| diffMax != 4144959 && !decimal.TryParse(currentLineSplits[8], out diffMax)
					|| currentLineSplits[10] is not ("有" or "无" or "any"))
				{
					analysisResult = null;
					return true;
				}

				analysisResult = new ManualSolver().Solve(grid);
				decimal max = analysisResult.MaxDifficulty;
				int chainingTechniquesCount = analysisResult.SolvingSteps!.Count(
					static step =>
						step is ChainingTechniqueInfo
						or AlsXzTechniqueInfo or AlsXyWingTechniqueInfo or AlsWWingTechniqueInfo
						or DeathBlossomTechniqueInfo or BowmanBingoTechniqueInfo);

				if (max < diffMin || max > diffMax
					|| chainingTechniquesCount < chainCountMin || chainingTechniquesCount > chainCountMax)
				{
					analysisResult = null;
					return false;
				}

				bool? containFcs = currentLineSplits[10] switch
				{
					"有" => true,
					"无" => false,
					"any" => null,
					_ => throw Throwings.ImpossibleCase
				};

				bool realContainFcs = analysisResult.SolvingSteps.Any(
					static step =>
						step is BowmanBingoTechniqueInfo
						or RegionChainingTechniqueInfo or CellChainingTechniqueInfo);

				if (containFcs is false && realContainFcs || containFcs is not false && !realContainFcs)
				{
					analysisResult = null;
					return false;
				}

				return true;
			}
		}

		/// <summary>
		/// 禁言整个群或指定的群内成员。
		/// </summary>
		private static async Task JinxGroupAsync(string[] s, GroupMessageReceivedEventArgs e)
		{
			try
			{
				switch (s.Length)
				{
					case 1:
					{
						await (
							e.Sender is { Role: MemberRole.Administrator or MemberRole.Owner }
							? e.Source.MuteAsync()
							: e.Reply("您没有权限使用此功能。"));
						break;
					}
					//case >= 5 and var length when s[2] == "时长": // #禁言 <人> 时长 <数字> (天|周|小时|分钟|秒)
					//{
					//	await JinxOrUnjinxMemberAsync(s, length, e, true);
					//	break;
					//}
				}
			}
			catch (ApiException ex) when (ex.ErrorCode == 10)
			{
				await e.Reply("我没有管理员权限，无法使用禁言功能。");
			}
		}

		/// <summary>
		/// 取消对群的禁言，或对群内成员的禁言。
		/// </summary>
		private static async Task UnjinxGroupAsync(string[] s, GroupMessageReceivedEventArgs e)
		{
			try
			{
				switch (s.Length)
				{
					case 1:
					{
						await (
							e.Sender is { Role: MemberRole.Administrator or MemberRole.Owner }
							? e.Source.UnmuteAsync()
							: e.Reply("您没有权限使用此功能。"));
						break;
					}
					//case >= 5 and var length when s[2] == "时长": // #解除禁言 <人> 时长 <数字> (天|周|小时|分钟|秒)
					//{
					//	await JinxOrUnjinxMemberAsync(s, length, e, false);
					//	break;
					//}
				}
			}
			catch (ApiException ex) when (ex.ErrorCode == 10)
			{
				await e.Reply("我没有管理员权限，无法使用解除禁言功能。");
			}
		}

		/// <summary>
		/// 根据禁言的信息（被禁言人号码、昵称或群名）禁言对象。
		/// </summary>
		private static async Task<bool> JinxOrUnjinxMemberAsync(
			string[] s, int length, GroupMessageReceivedEventArgs e, bool jinx)
		{
			var member = (
				from m in await e.Source.GetMembersAsync()
				where m.Alias == s[1] || m.DisplayName == s[1] || m.Number.ToString() == s[1]
				select m).FirstOrDefault();
			if (member is null)
			{
				return false;
			}

			if (jinx)
			{
				var timeSpan = TimeSpan.Zero;
				for (int index = 3; index < length; index += 2)
				{
					if (!uint.TryParse(s[index], out uint current) || index + 1 >= length)
					{
						return false;
					}

					TimeSpan? currentUnit = s[index + 1] switch
					{
						"天" => TimeSpan.FromDays(current),
						"周" => TimeSpan.FromDays(7 * current),
						"小时" => TimeSpan.FromHours(current),
						"分钟" => TimeSpan.FromMinutes(current),
						"秒" => TimeSpan.FromSeconds(current),
						_ => null
					};
					if (!currentUnit.HasValue)
					{
						return false;
					}

					timeSpan += currentUnit.Value;
				}

				await member.MuteAsync(timeSpan);
				return true;
			}
			else
			{
				await member.UnmuteAsync();
				return true;
			}
		}

		/// <summary>
		/// 分析题目帮助。
		/// </summary>
		private static async Task AnalyzeHelpAsync(MessageReceivedEventArgs e) =>
			await e.Source.SendAsync(
				new StringBuilder()
				.AppendLine("格式：！分析 <题目>。")
				.Append("题目代码目前可支持普通文本格式、Hodoku 的中间盘面格式等。")
				.ToString());

		/// <summary>
		/// 生成图片帮助。
		/// </summary>
		private static async Task GeneratePictureHelpAsync(MessageReceivedEventArgs e) =>
			await e.Source.SendAsync(
				new StringBuilder()
				.AppendLine("格式：！生成图片 <题目>。")
				.Append("题目代码目前可支持普通文本格式、Hodoku 的中间盘面格式等。")
				.ToString());

		/// <summary>
		/// 生成空盘帮助。
		/// </summary>
		private static async Task GenerateEmptyGridHelpAsync(MessageReceivedEventArgs e) =>
			await e.Source.SendAsync("格式：！生成空盘。");

		/// <summary>
		/// 清盘帮助。
		/// </summary>
		private static async Task CleanGridHelpAsync(MessageReceivedEventArgs e) =>
			await e.Source.SendAsync(
				new StringBuilder()
				.AppendLine("格式：！清盘 <题目>。")
				.Append("题目代码目前可支持普通文本格式、Hodoku 的中间盘面格式等。")
				.ToString());

		/// <summary>
		/// 介绍程序帮助。
		/// </summary>
		private static async Task IntroduceHelpAsync(MessageReceivedEventArgs e) =>
			await e.Source.SendAsync("格式：！关于。");

		/// <summary>
		/// 开始绘图帮助。
		/// </summary>
		private static async Task StartDrawingHelpAsync(MessageReceivedEventArgs e) =>
			await e.Source.SendAsync(
				new StringBuilder()
				.AppendLine("格式：！开始绘图[ 大小 <大小>][ 盘面 <题目>]。")
				.AppendLine("图片大小需要是一个不超过 1000 的正整数，表示多大的图片，以像素为单位；")
				.AppendLine("题目代码目前可支持普通文本格式、Hodoku 的中间盘面格式等。")
				.Append(R.GetValue("OptionalArg"))
				.ToString());

		/// <summary>
		/// 完成绘图帮助。
		/// </summary>
		private static async Task DisposePainterHelpAsync(MessageReceivedEventArgs e) =>
			await e.Source.SendAsync("格式：！结束绘图。");

		/// <summary>
		/// 填充数字帮助。
		/// </summary>
		private static async Task FillHelpAsync(MessageReceivedEventArgs e) =>
			await e.Source.SendAsync(
				new StringBuilder()
				.AppendLine("格式：！填入 (提示数|填入数) <数字> 到 <单元格>。")
				.AppendLine(R.GetValue("RequiredArg"))
				.ToString());

		/// <summary>
		/// 画画功能帮助。
		/// </summary>
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

#if AUTHOR_RESERVED
		/// <summary>
		/// 抽取题目帮助。
		/// </summary>
		private static async Task ExtractPuzzleHelpAsync(GroupMessageReceivedEventArgs e) =>
			await e.Source.SendAsync(
				new StringBuilder()
				.AppendLine("格式：！抽题 [不带候选数|带候选数]。")
				.AppendLine(R.GetValue("OptionalArg"))
				.Append("默认为带候选数。")
				.ToString());
#endif

		/// <summary>
		/// 帮助功能帮助。
		/// </summary>
		private static async Task ShowHelperTextAsync(MessageReceivedEventArgs e) =>
			await e.Source.SendAsync(
				new StringBuilder()
				.AppendLine("格式：！帮助。")
				.AppendLine("机器人可识别的功能有以下一些：")
				.AppendLine("！帮助：显示此帮助信息。")
				.AppendLine("！分析 <盘面>：显示题目的分析结果。")
				.AppendLine("！生成图片 <盘面>：将题目文本转为图片显示。")
				.AppendLine("！填入 (提示数|填入数) <数字> 到 <单元格>：填入提示数或填入数。")
				.AppendLine("！清盘 <盘面>：将盘面前期的排除、唯一余数、区块和数组技巧全部应用。")
				.AppendLine("！生成空盘：给一个空盘的图片。")
				.AppendLine("！开始绘图[ 大小 <图片大小>][ 盘面 <盘面>]：开始从空盘或指定盘面开始为盘面进行绘制，比如添加候选数涂色等。")
				.AppendLine("！结束绘图：指定画图过程结束，清除画板。")
				.AppendLine("！抽题：抽取一道题库里的题目。")
				.AppendLine("！关于：我 介 绍 我 自 己")
				.AppendLine()
				.AppendLine("注：为和普通聊天信息作区分，命令前面的叹号“！”也是命令的一部分；")
				.AppendLine("每一个命令的详情信息请输入“命令 ？”来获取。")
				.AppendLine()
				.Append(R.GetValue("MyName"))
				.ToString());

		/// <summary>
		/// 介绍自己。
		/// </summary>
		private static async Task IntroduceAsync(MessageReceivedEventArgs e) =>
			await e.Source.SendAsync(
				new StringBuilder()
				.AppendLine("Hello 大家好！我是一个机器人，叫小蛋蛋，是女孩子哦 (✿◡‿◡)")
				.Append("我爸还在给我加别的功能，所以我现在还需要大家的鼓励和支持哦，蟹蟹~")
				.ToString());

		/// <summary>
		/// 分析题目。
		/// </summary>
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
		/// 生成图片。
		/// </summary>
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
		/// 生成空盘。
		/// </summary>
		private static async Task GenerateEmptyGridAsync(MessageReceivedEventArgs e)
		{
			var painter = new GridPainter(new(Size, Size), new(), SudokuGrid.Undefined);
			using var image = painter.Draw();
			await e.ReplyImageAsync(image);
		}

		/// <summary>
		/// 清盘。
		/// </summary>
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
		/// 开始画图。
		/// </summary>
		private static async Task StartDrawingAsync(string info, string[] s, MessageReceivedEventArgs e)
		{
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
		/// 填入提示数或填入数。
		/// </summary>
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
		/// 为单元格涂色。
		/// </summary>
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
		/// 为候选数涂色。
		/// </summary>
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
		/// 为一行涂色。
		/// </summary>
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
		/// 为一列涂色。
		/// </summary>
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
		/// 为一个宫涂色。
		/// </summary>
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
		/// 为一个区域涂色。
		/// </summary>
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
		/// 画一条链。
		/// </summary>
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
		/// 画叉叉标记。
		/// </summary>
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
		/// 画圆圈标记。
		/// </summary>
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
		/// 清除画板。
		/// </summary>
		private static async Task DisposePainterAsync(MessageReceivedEventArgs e)
		{
			_painter = null;

			await e.Reply(R.GetValue("Success"));
		}
	}
}

#endif