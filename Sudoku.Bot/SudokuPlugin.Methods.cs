#pragma warning disable IDE0060

using System;
using System.Extensions;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HuajiTech.Mirai.Http;
using HuajiTech.Mirai.Http.Events;
using Sudoku.Bot.Extensions;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Globalization;
using Sudoku.Solving;
using Sudoku.Solving.Checking;
using Sudoku.Solving.Manual;
using Sudoku.Solving.Manual.Chaining;
using Sudoku.Solving.Manual.Extensions;
using Sudoku.Techniques;
using FormattingOptions = Sudoku.Solving.AnalysisResultFormattingOptions;

namespace Sudoku.Bot
{
	partial class SudokuPlugin
	{
		private static async partial Task HelpAsync(string[] args, Session sender, GroupMessageReceivedEventArgs e)
		{
			string info = new StringBuilder()
				.AppendLine(X.CommandFormatHelp)
				.AppendLine(X.CommandDescriptionHelp1)
				.AppendLine(X.CommandDescriptionHelpHelp1)
				.AppendLine(X.CommandDescriptionHelpAnalyze)
				.AppendLine(X.CommandDescriptionHelpGeneratePicture)
				.AppendLine(X.CommandDescriptionHelpClean)
				.AppendLine(X.CommandDescriptionHelpExtractPuzzle)
				.AppendLine(X.CommandDescriptionHelpAbout)
				.AppendLine(X.CommandDescriptionHelpJinx)
				.AppendLine(X.CommandDescriptionHelpUnjinx)
				.AppendLine()
				.AppendLine(X.CommandDescriptionHelpHelp2)
				.AppendLine(X.CommandDescriptionHelpHelp3)
				.AppendLine(X.CommandDescriptionHelpHelp4)
				.AppendLine()
				.Append(DateTime.Today.ToString("yyyy-MM-dd ddd", null))
				.ToString();
			await e.Source.SendAsync(info);
		}
		private static async partial Task AnalyzeAsync(string[] args, Session sender, GroupMessageReceivedEventArgs e)
		{
			switch (args.Length)
			{
				case 2 when args[1] == X.ChineseQuestionMark: // Analyze ?
				{
					string info = new StringBuilder()
						.AppendLine(X.CommandFormatAnalyze)
						.Append(X.CommandDescriptionAnalyze1)
						.ToString();
					await e.Source.SendAsync(info);

					break;
				}
				case 2: // Analyze <puzzle>
				{
					if (!SudokuGrid.TryParse(args[1].Trim(), out var grid) || !grid.IsValid())
					{
						return;
					}

					await e.Source.SendAsync(
						(await new ManualSolver().SolveAsync(grid, null))!.ToString(
							FormattingOptions.ShowSeparators | FormattingOptions.ShowDifficulty,
							CountryCode.ZhCn
						)
					);

					break;
				}
			}
		}
		private static async partial Task GeneratePictureAsync(string[] args, Session sender, GroupMessageReceivedEventArgs e)
		{
			switch (args.Length)
			{
				case 2 when args[1] == X.ChineseQuestionMark: // GeneratePicture ?
				{
					string info = new StringBuilder()
						.AppendLine(X.CommandFormatGeneratePicture)
						.Append(X.CommandDescriptionGeneratePicture)
						.ToString();
					await e.Source.SendAsync(info);

					break;
				}
				case 2: // GeneratePicture <puzzle>
				{
					await i(true);

					break;
				}
				case 3: // GeneratePicture <puzzle> (+candidates|-candidates)
				{
					string with1 = X.OptionsWithCandidates1, with2 = X.OptionsWithCandidates2;
					string without1 = X.OptionsWithoutCandidates1, without2 = X.OptionsWithoutCandidates2;
					if (args.Contains(with1, with2))
					{
						await i(true);
					}
					else if (args.Contains(without1, without2))
					{
						await i(false);
					}

					break;
				}
			}

			async Task<bool> i(bool withCands)
			{
				bool containsGrid = false;
				SudokuGrid targetGrid;
				foreach (string arg in args)
				{
					if (SudokuGrid.TryParse(arg, out targetGrid))
					{
						containsGrid = true;
						break;
					}
				}
				if (!containsGrid)
				{
					return false;
				}

				Settings.ShowCandidates = withCands;
				var painter = new GridPainter(new(Size, Size), Settings, targetGrid);
				using var image = painter.Draw();
				await e.ReplyImageAsync(image, Desktop);

				return true;
			}
		}
		private static async partial Task CleanAsync(string[] args, Session sender, GroupMessageReceivedEventArgs e)
		{
			switch (args.Length)
			{
				case 2 when args[1] == X.ChineseQuestionMark: // Clean ?
				{
					string info = new StringBuilder()
						.AppendLine(X.CommandFormatClean)
						.Append(X.CommandDescriptionClean)
						.ToString();
					await e.Source.SendAsync(info);

					break;
				}
				case 2: // Clean <puzzle>
				{
					if (!SudokuGrid.TryParse(args[1].Trim(), out var grid) || !grid.IsValid())
					{
						return;
					}

					grid.Clean();
					var painter = new GridPainter(new(Size, Size), new(Settings) { ShowCandidates = true }, grid);

					using var image = painter.Draw();
					await e.ReplyImageAsync(image, Desktop);

					break;
				}
			}
		}
		private static async partial Task GenerateEmptyAsync(string[] args, Session sender, GroupMessageReceivedEventArgs e)
		{
			switch (args.Length)
			{
				case 1: // GenerateEmpty
				{
					var painter = new GridPainter(new(Size, Size), Settings, SudokuGrid.Undefined);
					using var image = painter.Draw();
					await e.ReplyImageAsync(image, Desktop);

					break;
				}
				case 2 when args[1] == X.ChineseQuestionMark: // GenerateEmpty ?
				{
					await e.Source.SendAsync((string)X.CommandFormatGenerateEmpty);

					break;
				}
			}
		}
		private static async partial Task AboutAsync(string[] args, Session sender, GroupMessageReceivedEventArgs e)
		{
			switch (args.Length)
			{
				case 1: // About
				{
					string info = new StringBuilder()
						.AppendLine(X.CommandValueAbout1)
						.Append(X.CommandValueAbout2)
						.ToString();
					await e.Source.SendAsync(info);

					break;
				}
				case 2 when args[1] == X.ChineseQuestionMark: // About ?
				{
					await e.Source.SendAsync((string)X.CommandFormatAbout);

					break;
				}
			}
		}
		private static async partial Task ExtractPuzzleAsync(string[] args, Session sender, GroupMessageReceivedEventArgs e)
		{
			switch (args.Length)
			{
				case 1: // ExtractPuzzle
				{
					await outputPuzzlePictureAsync(true);

					break;
				}
				case 2 when args[1] == X.ChineseQuestionMark: // ExtractPuzzle ?
				{
					string info = new StringBuilder()
						.AppendLine(X.CommandFormatExtractPuzzle)
						.Append(X.CommandDescriptionExtractPuzzle)
						.ToString();
					await e.Source.SendAsync(info);

					break;
				}
				case 2: // ExtractPuzzle (+candidates|-candidates|+fast|-fast)
				{
					if (args[1] == X.OptionsWithCandidates1 || args[1] == X.OptionsWithCandidates2)
					{
						await outputPuzzlePictureAsync(true);
					}
					else if (args[1] == X.OptionsWithoutCandidates1 || args[1] == X.OptionsWithoutCandidates2)
					{
						await outputPuzzlePictureAsync(false);
					}
					else if (args.Contains((string)X.SettingsTextFastSearch))
					{
						await outputPuzzlePictureAsync(true, true);
					}

					break;
				}
				case 3: // ExtractPuzzle (+candidate|-candidates) (+fast|-fast)
				{
					bool fastSearch = args.Contains((string)X.SettingsTextFastSearch);
					if (args.Contains((string)X.OptionsWithCandidates1, (string)X.OptionsWithCandidates2))
					{
						await outputPuzzlePictureAsync(true, fastSearch);
					}
					else if (args.Contains((string)X.OptionsWithoutCandidates1, (string)X.OptionsWithoutCandidates2))
					{
						await outputPuzzlePictureAsync(true, fastSearch);
					}

					break;
				}
			}

			async Task outputPuzzlePictureAsync(bool showCandidates, bool fastSearch = false)
			{
				// Check whether the database path exists.
				string dirDatabase = Path.Combine(BasePath, X.PathDatabaseDefault);
				if (!Directory.Exists(dirDatabase))
				{
					return;
				}

				// Check the database files exists.
				string[] files = Directory.GetFiles(dirDatabase);
				if (files.Length == 0)
				{
					return;
				}

				// Get the database that is used by the target group.
				long groupNumber = e.Source.Number;
				string correspondingPath = $@"{dirDatabase}\{groupNumber}.txt";
				string dirDatabaseFinished = Path.Combine(BasePath, X.PathDatabaseFinished);
				string finishedPath = $@"{dirDatabaseFinished}\{groupNumber}.txt";
				if (!File.Exists(correspondingPath))
				{
					await e.ReplyAsync((string)X.CommandValueExtractPuzzleFailed2);
					return;
				}

				// File found.
				// Now try to get the puzzle randomly.
				// The algorithm will try 10 times to get the result puzzle.
				// Why trying 10 times? Because the puzzle will be checked after extracted.
				// If and only if the puzzle satisfies the specified condition, the puzzle will be returned.
				// Of course, if the condition file doesn't found, we'll return the puzzle directly.
				await e.Source.SendAsync((string)X.CommandValueExtractPuzzleNowGenerating);
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
					if (
						// Base condition: if the file has already been finished, skip it.
						basicCondition(finishedPath, exists, str)
						// Custom condition: specified in the file.
						&& customCondition(groupNumber, grid, out analysisResult, fastSearch))
					{
						break;
					}
				}

				if (trial == 10)
				{
					await e.ReplyAsync((string)X.CommandValueExtractPuzzleFailed);
				}
				else
				{
					// If with candidates, clean the grid. 
					if (showCandidates)
					{
						grid.Clean();
					}

					// Mark the puzzle as "finished", and save it to the "solved" folder.
					DirectoryEx.CreateIfDoesNotExist(dirDatabaseFinished);
					File.AppendAllText(finishedPath, $"{grid}{Environment.NewLine}");

					// Output the analysis result.
					analysisResult ??= new ManualSolver { FastSearch = fastSearch }.Solve(grid);
					await e.ReplyAsync(
						analysisResult.ToString(
							FormattingOptions.ShowDifficulty | FormattingOptions.ShowSeparators,
							CountryCode.ZhCn
						)
					);

					// Output the picture.
					var painter = new GridPainter(
						new(Size, Size), new(Settings) { ShowCandidates = showCandidates }, grid
					);
					using var image = painter.Draw();
					await e.ReplyImageAsync(image, Desktop);
				}
			}

			static bool basicCondition(string finishedPath, bool exists, string str) =>
				exists && File.ReadLines(finishedPath).All(
					l =>
					{
						string[] z = l.Split('\t', StringSplitOptions.RemoveEmptyEntries);
						return z.Length >= 1 && z[0] is var s && s != str;
					}
				)
				|| !exists;

			static bool customCondition(
				long groupNumber, in SudokuGrid grid, out AnalysisResult? analysisResult, bool fastSearch)
			{
				string path = Path.Combine(BasePath, X.PathDatabaseSettings);
				if (!File.Exists(path))
				{
					analysisResult = null;
					return true;
				}

				string[] lines = File.ReadAllLines(path);
				if (
					lines.FirstOrDefault(line =>
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
					|| currentLineSplits[1] != X.SettingsTextChainsCount
					|| currentLineSplits[3] != X.SettingsTextTo
					|| currentLineSplits[5] != X.SettingsTextDifficulty
					|| currentLineSplits[7] != X.SettingsTextTo
					|| currentLineSplits[9] != X.SettingsTextForcingChains)
				{
					analysisResult = null;
					return true;
				}

				uint chainCountMin = default, chainCountMax = default;
				decimal diffMin = default, diffMax = default;
				if (currentLineSplits[2] == X.SettingsTextAny) chainCountMin = 0;
				if (currentLineSplits[4] == X.SettingsTextAny) chainCountMax = 1000;
				if (currentLineSplits[6] == X.SettingsTextAny) diffMin = 0;
				if (currentLineSplits[8] == X.SettingsTextAny) diffMax = 20;
				if (currentLineSplits[2] is var k && k != X.SettingsTextAny
					&& !uint.TryParse(k, out chainCountMin)
					|| currentLineSplits[4] is var l && l != X.SettingsTextAny
					&& !uint.TryParse(l, out chainCountMax)
					|| currentLineSplits[6] is var m && m != X.SettingsTextAny
					&& !decimal.TryParse(m, out diffMin)
					|| currentLineSplits[8] is var n && n != X.SettingsTextAny
					&& !decimal.TryParse(n, out diffMax)
					|| currentLineSplits[10] is var t
					&& t != X.SettingsTextWith && t != X.SettingsTextWithout && t != X.SettingsTextAny)
				{
					analysisResult = null;
					return true;
				}

				analysisResult = new ManualSolver { FastSearch = fastSearch }.Solve(grid);
				decimal max = analysisResult.MaxDifficulty;
				int chainingTechniquesCount = analysisResult.Steps!.Count(isChaining);

				if (max < diffMin || max > diffMax
					|| chainingTechniquesCount < chainCountMin || chainingTechniquesCount > chainCountMax)
				{
					analysisResult = null;
					return false;
				}

				bool? containFcs =
					currentLineSplits[10] == X.SettingsTextWith
					? true
					: currentLineSplits[10] == X.SettingsTextWithout ? false : null;

				bool realContainFcs = analysisResult.Steps!.Any(isFc);
				switch (containFcs)
				{
					case false when realContainFcs:
					case not false when !realContainFcs:
					{
						analysisResult = null;
						return false;
					}
					default:
					{
						return true;
					}
				}
			}

			static bool isFc(StepInfo step) => step.HasTag(TechniqueTags.LongChaining) && step is not AicStepInfo;
			static bool isChaining(StepInfo step) => step.HasTag(TechniqueTags.Als | TechniqueTags.Wings | TechniqueTags.ShortChaining | TechniqueTags.LongChaining);
		}
		private static async partial Task JinxAsync(string[] args, Session sender, GroupMessageReceivedEventArgs e)
		{
			switch (args.Length)
			{
				case 1: // Jinx
				{
					await (
						e.Sender is { Role: MemberRole.Administrator or MemberRole.Owner }
						? e.Source.MuteAsync()
						: e.ReplyAsync((string)X.CommandValueJinxFailed1)
					);

					break;
				}
				case 2 when args[1] == X.ChineseQuestionMark: // Jinx ?
				{
					await e.Source.SendAsync((string)X.CommandFormatJinx);

					break;
				}
				case >= 5 when args[2] == X.SettingsTextSpan: // Jinx <someone> span (<number> (d|w|h|m|s))+
				{
					await TryJinxOrUnjinxAsync(args, e, true);

					break;
				}
			}
		}
		private static async partial Task UnjinxAsync(string[] args, Session sender, GroupMessageReceivedEventArgs e)
		{
			switch (args.Length)
			{
				case 1: // Unjinx
				{
					await (
						e.Sender is { Role: MemberRole.Administrator or MemberRole.Owner }
						? e.Source.UnmuteAsync()
						: e.ReplyAsync((string)X.CommandValueUnjinxFailed1)
					);

					break;
				}
				case >= 2: // Unjinx <someone>
				{
					await TryJinxOrUnjinxAsync(args, e, false);

					break;
				}
			}
		}
		private static async partial Task SetTitleAsync(string[] args, Session sender, GroupMessageReceivedEventArgs e)
		{
			switch (args.Length)
			{
				case 2 when args[1] == X.ChineseQuestionMark: // SetTitle ?
				{
					string info = new StringBuilder()
						.Append((string)X.CommandFormatSetTitle)
						.Append((string)X.CommandDescriptionSetTitle)
						.ToString();

					await e.Source.SendAsync(info);

					break;
				}
				case 3 when args[2].Length <= 6 && e.Sender.Number == 747507738L: // SetTitle <user> <title>
				{
					var member = await GetMemberAsync(args[1], e);
					if (member is null)
					{
						return;
					}

					try
					{
						await member.SetTitleAsync(args[2]);
					}
					catch (ApiException ex) when (ex.ErrorCode == 10)
					{
						await e.ReplyAsync((string)X.CommandValueSetTitleFailed);
					}

					break;
				}
			}
		}
		private static async partial Task RemoveTitleAsync(string[] args, Session sender, GroupMessageReceivedEventArgs e)
		{
			switch (args.Length)
			{
				case 2 when args[1] == X.ChineseQuestionMark: // RemoveTitle ?
				{
					string info = new StringBuilder()
						.Append((string)X.CommandFormatRemoveTitle)
						.Append((string)X.CommandDescriptionRemoveTitle)
						.ToString();

					await e.Source.SendAsync(info);

					break;
				}
				case 2 when e.Sender.Number == 747507738L: // RemoveTitle <user>
				{
					var member = await GetMemberAsync(args[1], e);
					if (member is null)
					{
						return;
					}

					try
					{
						await member.SetTitleAsync(string.Empty);
					}
					catch (ApiException ex) when (ex.ErrorCode == 10)
					{
						await e.ReplyAsync((string)X.CommandValueRemoveTitleFailed);
					}

					break;
				}
			}
		}
	}
}
