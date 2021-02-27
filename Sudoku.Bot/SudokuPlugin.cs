using System;
using System.Diagnostics.CodeAnalysis;
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
	public static partial class SudokuPlugin
	{
		/// <summary>
		/// Indicates the desktop path.
		/// </summary>
		private static readonly string Desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

		/// <summary>
		/// Indicates the command hanlder.
		/// </summary>
		private static readonly CommandLine Handler;

		/// <summary>
		/// Indicates the random number generator.
		/// </summary>
		private static readonly Random Rng = new();


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
			new CommandRouter(ExtractPuzzleAsync, (string)X.CommandExtractPuzzle);


		/// <summary>
		/// Indicates whether the mode is the config mode.
		/// </summary>
		public static bool ConfigMode { get; private set; }

		/// <summary>
		/// Indicates the default size that is used for drawing.
		/// </summary>
		public static int Size { get; private set; }

		/// <summary>
		/// Indicates the directory name of the base settings path.
		/// </summary>
		public static string BasePath { get; private set; } = null!;

		/// <summary>
		/// Indicates the settings.
		/// </summary>
		public static Settings Settings { get; set; } = new();

		/// <summary>
		/// The short form of invocation <c><see cref="TextResources.Current"/></c>.
		/// </summary>
		/// <seealso cref="TextResources.Current"/>
		private static dynamic X => TextResources.Current;


		/// <summary>
		/// Start a sudoku plugin, with the specified event source that triggers
		/// when the group has received a message, the base path of the settings and a <see cref="bool"/> value
		/// indicating whether the mode is the config mode.
		/// </summary>
		/// <param name="basePath">The base path.</param>
		/// <param name="configMode">Indicates whether the current mode is the config mode.</param>
		/// <param name="groupReceivedSource">The event source.</param>
		[MemberNotNull(nameof(BasePath))]
		public static void Start(
			GroupMessageReceivedEventSource groupReceivedSource, string basePath,
			bool configMode = false, int size = 800)
		{
			// Assign values.
			ConfigMode = configMode;
			Size = size;
			BasePath = basePath;

			// Change the language to Chinese.
			CoreTextResources.Current.ChangeLanguage(CountryCode.ZhCn);

			// Add handler.
			groupReceivedSource.Event += RouteAsync;
		}


		/// <summary>
		/// Indicates the base route method. The method will re-direct to the specified method
		/// via the command name specified as the received message.
		/// </summary>
		/// <param name="sender">The session.</param>
		/// <param name="e">The event arguments.</param>
		/// <returns>The task of the handling.</returns>
		private static async Task RouteAsync(Session sender, GroupMessageReceivedEventArgs e)
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
		}


		private static partial Task HelpAsync(string[] args, Session sender, GroupMessageReceivedEventArgs e);
		private static partial Task AnalyzeAsync(string[] args, Session sender, GroupMessageReceivedEventArgs e);
		private static partial Task GeneratePictureAsync(string[] args, Session sender, GroupMessageReceivedEventArgs e);
		private static partial Task GenerateEmptyAsync(string[] args, Session sender, GroupMessageReceivedEventArgs e);
		private static partial Task CleanAsync(string[] args, Session sender, GroupMessageReceivedEventArgs e);
		private static partial Task AboutAsync(string[] args, Session sender, GroupMessageReceivedEventArgs e);
		private static partial Task ExtractPuzzleAsync(string[] args, Session sender, GroupMessageReceivedEventArgs e);
	}
}
