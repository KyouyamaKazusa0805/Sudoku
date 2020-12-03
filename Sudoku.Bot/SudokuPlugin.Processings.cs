#if AUTHOR_RESERVED

using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HuajiTech.Mirai;
using HuajiTech.Mirai.Events;
using Sudoku.Bot.Extensions;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Drawing.Extensions;
using Sudoku.Globalization;
using Sudoku.Solving;
using Sudoku.Solving.Checking;
using Sudoku.Solving.Extensions;
using Sudoku.Solving.Manual;
using PointConverter = Sudoku.Drawing.PointConverter;

namespace Sudoku.Bot
{
	partial class SudokuPlugin
	{
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
							: e.ReplyAsync("您没有权限使用此功能。"));
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
				await e.ReplyAsync("我没有管理员权限，无法使用禁言功能。");
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
							: e.ReplyAsync("您没有权限使用此功能。"));
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
				await e.ReplyAsync("我没有管理员权限，无法使用解除禁言功能。");
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
		/// 调整背景色。
		/// </summary>
		private static async Task AdjustBackgroundAsync(string[] s, GroupMessageReceivedEventArgs e)
		{
			if (s.Length != 3)
			{
				return;
			}

			if (!ParseColorId(s[2], false, out long colorId))
			{
				return;
			}

			byte a = (byte)(colorId >> 24 & 255);
			byte r = (byte)(colorId >> 16 & 255);
			byte g = (byte)(colorId >> 8 & 255);
			byte b = (byte)(colorId & 255);
			Settings.BackgroundColor = Color.FromArgb(a, r, g, b);

			await e.Source.SendAsync($"颜色已修改为：[{a}, {r}, {g}, {b}]。");
		}

		/// <summary>
		/// 调整候选数大小。
		/// </summary>
		private static async Task AdjustCandidateAsync(string[] s, GroupMessageReceivedEventArgs e)
		{
			if (s.Length != 3)
			{
				return;
			}

			if (!decimal.TryParse(s[2], out decimal scale))
			{
				return;
			}

			Settings.CandidateScale = scale;
			await e.Source.SendAsync($"候选数的比例已调整为 {scale:0.0}。");
		}

		/// <summary>
		/// 调整确定值大小。
		/// </summary>
		private static async Task AdjustValueAsync(string[] s, GroupMessageReceivedEventArgs e)
		{
			if (s.Length != 3)
			{
				return;
			}

			if (!decimal.TryParse(s[2], out decimal scale))
			{
				return;
			}

			Settings.ValueScale = scale;

			await e.Source.SendAsync($"确定值的比例已调整为 {scale:0.0}。");
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
					await e.ReplyAsync("本群尚未拥有刷题题库，抽取题目失败。");
					return;
				}

				await e.Source.SendAsync("正在抽取题目，请耐心等待……");
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
					await e.ReplyAsync("随机抽题失败，可能是脸不好；请重试一次。");
				}
				else
				{
					// 如果带候选数的时候，那么需要清盘。
					if (withCandidates.Value is var b && b)
					{
						grid.Clean();
					}

					// 把当前题目标记为已做过，放到已经完成的文件夹里。
					DirectoryEx.CreateIfDoesNotExist(FinishedPuzzleDir);
					File.AppendAllText(finishedPath, $"{grid}\r\n");

					// 输出相关信息。
					analysisResult ??= new ManualSolver().Solve(grid);
					await e.ReplyAsync(analysisResult.ToString("-!", CountryCode.ZhCn));

					// 输出图片信息。
					var painter = new GridPainter(
						new(Size, Size),
						new(Settings) { ShowCandidates = b },
						grid);
					await e.ReplyImageAsync(painter.Draw());
				}
			}

			static bool basicCondition(string finishedPath, bool exists, string str) =>
				exists && File.ReadLines(finishedPath).All(
					l =>
					{
						string[] z = l.Split('\t', StringSplitOptions.RemoveEmptyEntries);
						return z.Length >= 1 && z[0] is var s && s != str;
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
						}) is var resultLine && resultLine is null)
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

				uint chainCountMin = default, chainCountMax = default;
				decimal diffMin = default, diffMax = default;
				if (currentLineSplits[2] == "any") chainCountMin = 0;
				if (currentLineSplits[4] == "any") chainCountMax = 1000;
				if (currentLineSplits[6] == "any") diffMin = 0;
				if (currentLineSplits[8] == "any") diffMax = 20;
				if (currentLineSplits[2] is var k && k != "any" && !uint.TryParse(k, out chainCountMin)
					|| currentLineSplits[4] is var l && l != "any" && !uint.TryParse(l, out chainCountMax)
					|| currentLineSplits[6] is var m && m != "any" && !decimal.TryParse(m, out diffMin)
					|| currentLineSplits[8] is var n && n != "any" && !decimal.TryParse(n, out diffMax)
					|| currentLineSplits[10] is not ("有" or "无" or "any"))
				{
					analysisResult = null;
					return true;
				}

				analysisResult = new ManualSolver().Solve(grid);
				decimal max = analysisResult.MaxDifficulty;
				int chainingTechniquesCount = analysisResult.SolvingSteps!.Count(
					static step => step.IsAlsTechnique() || step.IsChainingTechnique());

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
					"any" => null
				};

				bool realContainFcs = analysisResult.SolvingSteps.Any(TechniqueInfoEx.IsForcingChainsTechnique);
				if (containFcs is false && realContainFcs || containFcs is not false && !realContainFcs)
				{
					analysisResult = null;
					return false;
				}

				return true;
			}
		}

		/// <summary>
		/// 分析题目。
		/// </summary>
		private static async Task AnalysisAsync(string info, MessageReceivedEventArgs e)
		{
			if (!SudokuGrid.TryParse(info["！分析".Length..].Trim(), out var grid) || !grid.IsValid())
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

			int? i =
				info.LastIndexOf("带候选数") is var r and not -1
				? r
				: info.LastIndexOf("不带候选数") is var z and not -1 ? z : null;
			if (!SudokuGrid.TryParse(info["！生成图片".Length..(i ?? ^0)].Trim(), out var grid)
				|| !grid.IsValid())
			{
				return;
			}

			var painter = new GridPainter(new(Size, Size), Settings, grid);
			using var image = painter.Draw();
			await e.ReplyImageAsync(image);
		}

		/// <summary>
		/// 生成空盘。
		/// </summary>
		private static async Task GenerateEmptyGridAsync(MessageReceivedEventArgs e)
		{
			var painter = new GridPainter(new(Size, Size), Settings, SudokuGrid.Undefined);
			using var image = painter.Draw();
			await e.ReplyImageAsync(image);
		}

		/// <summary>
		/// 清盘。
		/// </summary>
		private static async Task CleanGridAsync(string info, MessageReceivedEventArgs e)
		{
			if (!SudokuGrid.TryParse(info["！清盘".Length..].Trim(), out var grid) || !grid.IsValid())
			{
				return;
			}

			grid.Clean();
			var painter = new GridPainter(new(Size, Size), Settings, grid);

			using var image = painter.Draw();
			await e.ReplyImageAsync(image);
		}

		/// <summary>
		/// 开始画图。
		/// </summary>
		private static async Task StartDrawingAsync(string info, string[] s, MessageReceivedEventArgs e)
		{
			const string gridKeyword = "盘面", sizeKeyword = "大小";

			int size = Size;
			var grid = SudokuGrid.Undefined;
			int pos = info.IndexOf(sizeKeyword);
			if (pos != -1)
			{
				int startPos = pos + sizeKeyword.Length;
				if (info[startPos] != ' ')
				{
					return;
				}

				int index = startPos + 1;
				for (; char.IsDigit(info[index]); index++) ;

				string sizeStr = info[startPos..index];
				if (!int.TryParse(sizeStr, out int result))
				{
					return;
				}

				size = result;
			}

			pos = info.IndexOf(gridKeyword);
			if (pos != -1)
			{
				int startPos = pos + gridKeyword.Length;
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
			_painter = new(pointConverter, Settings, grid);

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

			if (s[3] != "到")
			{
				return;
			}

			if (!byte.TryParse(s[2], out byte digit) || digit is <= 0 or > 9)
			{
				return;
			}

			if (!CellParser.TryParse(s[4], out var map, new[] { ',', '，' }))
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

			if (!CellParser.TryParse(s[2], out var map, new[] { ',', '，' }))
			{
				return;
			}

			long colorId = default;
			if (!remove && !ParseColorId(s[3], true, out colorId))
			{
				return;
			}

			GridPainter.InitializeCustomViewIfNull(ref _painter);

			foreach (int cell in map)
			{
				if (remove)
				{
					_painter.CustomView!.RemoveCell(cell);
				}
				else
				{
					_painter.CustomView!.AddCell(colorId, cell);
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

			if (!CellParser.TryParse(s[2], out var map, new[] { ',', '，' }))
			{
				return;
			}

			if (!byte.TryParse(s[3], out byte digit) || digit is <= 0 or > 9)
			{
				return;
			}

			long colorId = default;
			if (!remove && !ParseColorId(s[4], false, out colorId))
			{
				return;
			}

			GridPainter.InitializeCustomViewIfNull(ref _painter);

			foreach (int cell in map)
			{
				int candidate = cell * 9 + digit - 1;
				if (remove)
				{
					_painter.CustomView!.RemoveCandidate(candidate);
				}
				else
				{
					_painter.CustomView!.AddCandidate(colorId, candidate);
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
			if (!remove && !ParseColorId(s[3], true, out colorId))
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
			if (!remove && !ParseColorId(s[3], true, out colorId))
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
			if (!remove && !ParseColorId(s[3], true, out colorId))
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

			GridPainter.InitializeCustomViewIfNull(ref _painter);

			int r = (int)label * 9 + region;
			if (remove)
			{
				_painter.CustomView!.RemoveRegion(r);
			}
			else
			{
				_painter.CustomView!.AddRegion(colorId, r);
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

			if (s[2] != "从")
			{
				return;
			}

			if (!CellParser.TryParse(s[3], out byte cell1) || !CellParser.TryParse(s[6], out byte cell2))
			{
				return;
			}

			if (!byte.TryParse(s[4], out byte d1) || !byte.TryParse(s[7], out byte d2)
				|| d1 is <= 0 or > 9 || d2 is <= 0 or > 9)
			{
				return;
			}

			GridPainter.InitializeCustomViewIfNull(ref _painter);

			var link = new Link(cell1 * 9 + d1 - 1, cell2 * 9 + d2 - 1, LinkType.Default);
			if (remove)
			{
				_painter.CustomView!.RemoveLink(link);
			}
			else
			{
				_painter.CustomView!.AddLink(link);
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

			if (!CellParser.TryParse(s[2], out var map, new[] { ',', '，' }))
			{
				return;
			}

			GridPainter.InitializeCustomViewIfNull(ref _painter);

			foreach (int cell in map)
			{
				if (remove)
				{
					_painter.CustomView!.RemoveDirectLine(GridMap.Empty, new() { cell });
				}
				else
				{
					_painter.CustomView!.AddDirectLine(GridMap.Empty, new() { cell });
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

			if (!CellParser.TryParse(s[2], out var map, new[] { ',', '，' }))
			{
				return;
			}

			GridPainter.InitializeCustomViewIfNull(ref _painter);

			foreach (int cell in map)
			{
				if (remove)
				{
					_painter.CustomView!.RemoveDirectLine(new() { cell }, GridMap.Empty);
				}
				else
				{
					_painter.CustomView!.AddDirectLine(new() { cell }, GridMap.Empty);
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

			await e.ReplyAsync("清除画板资源成功。");
		}

		/// <summary>
		/// 尝试解析字符串，并转换为一个表示颜色的 ID。这个颜色 ID 一般用来封装和保存到 <see cref="DrawingInfo"/>
		/// 对象里。
		/// </summary>
		/// <param name="str">字符串。</param>
		/// <param name="withTransparency">表示当前解析后的颜色是否保留透明度。</param>
		/// <param name="colorId">(<see langword="out"/> 参数) 转换后的颜色 ID。</param>
		/// <returns>表示是否转换成功。</returns>
		private static bool ParseColorId(string str, bool withTransparency, out long colorId)
		{
			switch (str)
			{
				case "红色" or "红": colorId = ColorId.ToCustomColorId(withTransparency ? 128 : 255, 235, 0, 0); return true;
				case "浅红色" or "浅红": colorId = ColorId.ToCustomColorId(withTransparency ? 64 : 255, 247, 165, 167); return true;
				case "橙色" or "橙": colorId = ColorId.ToCustomColorId(withTransparency ? 64 : 255, 255, 192, 89); return true;
				case "浅橙色" or "浅橙": colorId = ColorId.ToCustomColorId(withTransparency ? 64 : 255, 247, 222, 143); return true;
				case "黄色" or "黄": colorId = ColorId.ToCustomColorId(withTransparency ? 64 : 255, 255, 255, 150); return true;
				case "绿色" or "绿": colorId = ColorId.ToCustomColorId(withTransparency ? 64 : 255, 134, 242, 128); return true;
				case "浅绿色" or "浅绿": colorId = ColorId.ToCustomColorId(withTransparency ? 64 : 255, 215, 255, 215); return true;
				case "青色" or "青": colorId = ColorId.ToCustomColorId(withTransparency ? 64 : 255, 134, 232, 208); return true;
				case "浅青色" or "浅青": colorId = ColorId.ToCustomColorId(withTransparency ? 64 : 255, 206, 251, 237); return true;
				case "蓝色" or "蓝": colorId = ColorId.ToCustomColorId(withTransparency ? 64 : 255, 0, 0, 255); return true;
				case "浅蓝色" or "浅蓝": colorId = ColorId.ToCustomColorId(withTransparency ? 64 : 255, 127, 187, 255); return true;
				case "紫色" or "紫": colorId = ColorId.ToCustomColorId(withTransparency ? 64 : 255, 177, 165, 243); return true;
				case "浅紫色" or "浅紫": colorId = ColorId.ToCustomColorId(withTransparency ? 64 : 255, 220, 212, 252); return true;
				default: return ColorIdParser.TryParse(str, out colorId, withTransparency ? 64 : null, new[] { ',', '，' });
			}
		}
	}
}

#endif