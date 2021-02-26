using System.Collections.Generic;
using System.Threading.Tasks;
using HuajiTech.Mirai.Http;
using HuajiTech.Mirai.Http.Events;
using GroupReceived = HuajiTech.Mirai.Http.Events.GroupMessageReceivedEventArgs;

namespace Sudoku.Bot.CommandLines
{
	/// <summary>
	/// Encapsulates a command line router.
	/// </summary>
	public sealed class CommandLine
	{
		/// <summary>
		/// Indicates the inner dictionary.
		/// </summary>
		private readonly IDictionary<string, CommandHandler> _dic = new Dictionary<string, CommandHandler>();


		/// <summary>
		/// To handle the command.
		/// </summary>
		/// <param name="command">The command to handle.</param>
		/// <param name="args">The arguments.</param>
		/// <param name="sender">The session that trigger the command.</param>
		/// <param name="e">The detail arguments.</param>
		/// <returns>The task.</returns>
		public async Task HandleAsync(string command, string[] args, Session sender, GroupReceived e)
		{
			if (_dic.TryGetValue(command, out var handler))
			{
				await handler(args, sender, e);
			}
		}

		/// <summary>
		/// Register the command.
		/// </summary>
		/// <param name="commandName">The command name.</param>
		/// <param name="handler">The handler.</param>
		private void RegisterCommand(string commandName, CommandHandler handler)
		{
			if (_dic.ContainsKey(commandName))
			{
				_dic[commandName] = handler;
			}
			else
			{
				_dic.Add(commandName, handler);
			}
		}

		/// <summary>
		/// Register the commands.
		/// </summary>
		/// <param name="commandNames">The command names.</param>
		/// <param name="handler">The handler.</param>
		private void RegisterCommands(string[] commandNames, CommandHandler handler)
		{
			foreach (string commandName in commandNames)
			{
				if (_dic.ContainsKey(commandName))
				{
					_dic[commandName] = handler;
				}
				else
				{
					_dic.Add(commandName, handler);
				}
			}
		}

		/// <summary>
		/// Remove a command.
		/// </summary>
		/// <param name="commandName">The command name.</param>
		private void RemoveCommand(string commandName) => _dic.Remove(commandName);


		/// <summary>
		/// Add a command router.
		/// </summary>
		/// <param name="this">The command line instance.</param>
		/// <param name="router">The command router.</param>
		/// <returns>The reference same as <paramref name="this"/>.</returns>
		public static CommandLine operator +(CommandLine @this, CommandRouter router)
		{
			@this.RegisterCommand(router.Command, router.Handler);
			return @this;
		}

		/// <summary>
		/// Add a command router.
		/// </summary>
		/// <param name="this">The command line instance.</param>
		/// <param name="router">The command router.</param>
		/// <returns>The reference same as <paramref name="this"/>.</returns>
		public static CommandLine operator +(CommandLine @this, ComplexCommandRouter router)
		{
			@this.RegisterCommands(router.Commands, router.Handler);
			return @this;
		}

		/// <summary>
		/// Remove a command router.
		/// </summary>
		/// <param name="this">The command line instance.</param>
		/// <returns>The reference same as <paramref name="this"/>.</returns>
		public static CommandLine operator -(CommandLine @this, string command)
		{
			@this.RemoveCommand(command);
			return @this;
		}
	}
}
