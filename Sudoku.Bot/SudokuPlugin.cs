using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HuajiTech.Mirai.Http;
using HuajiTech.Mirai.Http.Events;
using HuajiTech.Mirai.Http.Messaging;
using Sudoku.Bot.CommandLines;
using Sudoku.Bot.Resources;
using Sudoku.DocComments;
using Sudoku.Drawing;
using Sudoku.Globalization;
using CoreTextResources = Sudoku.Resources.TextResources;

namespace Sudoku.Bot
{
	/// <summary>
	/// Encapsulates a sudoku plugin.
	/// </summary>
	public sealed partial class SudokuPlugin
	{
		/// <summary>
		/// Indicates the default size that is used for drawing.
		/// </summary>
		private const int Size = 800;


		/// <summary>
		/// Indicates the settings used.
		/// </summary>
		internal static Settings Settings = new();


		/// <summary>
		/// Indicates the command hanlder.
		/// </summary>
		private static readonly CommandLine Handler;

		/// <summary>
		/// Indicates the random number generator.
		/// </summary>
		private static readonly Random Rng = new();


		/// <summary>
		/// Initializes an instance with the specified event source.
		/// </summary>
		/// <param name="currentUserEventSource">The event source.</param>
		public SudokuPlugin(GroupMessageReceivedEventSource currentUserEventSource)
		{
			// Change the language to Chinese.
			CoreTextResources.Current.ChangeLanguage(CountryCode.ZhCn);

			// Add handler.
			currentUserEventSource.Event += static async (sender, e) =>
			{
				// Check the validity.
				var complexMessage = e.Message.Content;
				if (complexMessage is not { Count: not 0 } || complexMessage[0] is not PlainText pl)
				{
					return;
				}

				// Check whether the command is valid.
				string info = pl.ToString();
				if (string.IsNullOrEmpty(info))
				{
					return;
				}

				// Creates the regex to match a command.
				var regex = new Regex(@"(?<=\s*)((?<=“).+?(?=”)|[^:\s])+((\s*:\s*((?<=“).+?(?=”)|[^\s])+)|)|((?<=“).+?(?=”)|[^“”\s])+");
				string[] args = (from match in regex.Matches(info) select match.Value).ToArray();
				await Handler.HandleAsync(args[0], args, sender, e);
			};
		}


		/// <inheritdoc cref="StaticConstructor"/>
		static SudokuPlugin() =>
			Handler =
			new CommandLine() +
			new CommandRouter(HelpAsync, (string)X.CommandHelp) +
			new CommandRouter(AnalyzeAsync, (string)X.CommandAnalyze) +
			new CommandRouter(GeneratePictureAsync, (string)X.CommandGeneratePicture) +
			new CommandRouter(GenerateEmptyAsync, (string)X.CommandGenerateEmpty) +
			new CommandRouter(CleanAsync, (string)X.CommandClean) +
			new CommandRouter(AboutAsync, (string)X.CommandAbout) +
			new CommandRouter(ExtractPuzzleAsync, (string)X.CommandExtractPuzzle) +
			new CommandRouter(JinxAsync, (string)X.CommandJinx) +
			new CommandRouter(UnjinxAsync, (string)X.CommandUnjinx);


		/// <summary>
		/// The short form of invocation <c>TextResources.Current</c>.
		/// </summary>
		private static dynamic X => TextResources.Current;


		/// <summary>
		/// Try to jinx or unjinx someone.
		/// </summary>
		/// <param name="args">The arguments.</param>
		/// <param name="e">The event arguments.</param>
		/// <param name="jinx">Indicates whether the mode is to jinx somebody.</param>
		/// <returns>The task with the result value of type <see cref="bool"/>.</returns>
		private static async Task<bool> TryJinxOrUnjinxMemberAsync(
			string[] args, GroupMessageReceivedEventArgs e, bool jinx)
		{
			var member = await GetMemberAsync(args[1], e);
			if (member is null)
			{
				return false;
			}

			if (jinx)
			{
				var timeSpan = TimeSpan.Zero;
				for (int index = 3; index < args.Length - 1; index += 2)
				{
					if (!uint.TryParse(args[index], out uint current) || index + 1 >= args.Length)
					{
						return false;
					}

					string unit = args[index + 1];

					TimeSpan currentUnit;
					if (unit == X.SettingsTextWeek) currentUnit = TimeSpan.FromDays(current);
					else if (unit == X.SettingsTextDay) currentUnit = TimeSpan.FromDays(7 * current);
					else if (unit == X.SettingsTextHour) currentUnit = TimeSpan.FromHours(current);
					else if (unit == X.SettingsTextMinute) currentUnit = TimeSpan.FromMinutes(current);
					else if (unit == X.SettingsTextSecond) currentUnit = TimeSpan.FromSeconds(current);
					else return false;

					timeSpan += currentUnit;
				}

				try
				{
					await member.MuteAsync(timeSpan);
					return true;
				}
				catch (ApiException ex) when (ex.ErrorCode == 10)
				{
					await e.Source.SendAsync((string)X.CommandValueJinxFailed2);
					return false;
				}
			}
			else
			{
				try
				{
					await member.UnmuteAsync();
					return true;
				}
				catch (ApiException ex) when (ex.ErrorCode == 10)
				{
					await e.Source.SendAsync((string)X.CommandValueUnjinxFailed2);
					return false;
				}
			}
		}

		/// <summary>
		/// Try to get member information via his/her ID (number or nickname).
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="e">The event arguments.</param>
		/// <returns>The member information.</returns>
		private static async Task<Member?> GetMemberAsync(string name, GroupMessageReceivedEventArgs e) =>
		(
			from m in await e.Source.GetMembersAsync()
			where string.Equals(m.Alias, name, StringComparison.OrdinalIgnoreCase)
				|| string.Equals(m.DisplayName, name, StringComparison.OrdinalIgnoreCase)
				|| m.Number.ToString() == name
			select m
		).FirstOrDefault();


		private static partial Task HelpAsync(string[] args, Session sender, GroupMessageReceivedEventArgs e);
		private static partial Task AnalyzeAsync(string[] args, Session sender, GroupMessageReceivedEventArgs e);
		private static partial Task GeneratePictureAsync(string[] args, Session sender, GroupMessageReceivedEventArgs e);
		private static partial Task GenerateEmptyAsync(string[] args, Session sender, GroupMessageReceivedEventArgs e);
		private static partial Task CleanAsync(string[] args, Session sender, GroupMessageReceivedEventArgs e);
		private static partial Task AboutAsync(string[] args, Session sender, GroupMessageReceivedEventArgs e);
		private static partial Task ExtractPuzzleAsync(string[] args, Session sender, GroupMessageReceivedEventArgs e);
		private static partial Task JinxAsync(string[] args, Session sender, GroupMessageReceivedEventArgs e);
		private static partial Task UnjinxAsync(string[] args, Session sender, GroupMessageReceivedEventArgs e);
	}
}
