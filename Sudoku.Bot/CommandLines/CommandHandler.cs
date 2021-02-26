#pragma warning disable IDE1006

using System.Threading.Tasks;
using HuajiTech.Mirai.Http;
using HuajiTech.Mirai.Http.Events;

namespace Sudoku.Bot.CommandLines
{
	/// <summary>
	/// The command handler.
	/// </summary>
	/// <param name="args">The arguments.</param>
	/// <param name="sender">The session to trigger the command.</param>
	/// <param name="e">The arguments.</param>
	/// <returns>The task.</returns>
	public delegate Task CommandHandler(string[] args, Session sender, GroupMessageReceivedEventArgs e);
}
