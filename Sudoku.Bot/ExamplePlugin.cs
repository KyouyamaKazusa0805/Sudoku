#if AUTHOR_RESERVED

using System.Threading.Tasks;
using Mirai_CSharp;
using Mirai_CSharp.Models;
using Mirai_CSharp.Plugin.Interfaces;

namespace Sudoku.Bot
{
	/// <summary>
	/// The example plugin.
	/// </summary>
	public partial class ExamplePlugin : IGroupMessage
	{
		/// <inheritdoc/>
		public async Task<bool> GroupMessage(MiraiHttpSession session, IGroupMessageEventArgs e)
		{
			var messages = new IMessageBase[] { new PlainMessage($"Hello, {e.Sender.Id}!") };

			await session.SendGroupMessageAsync(e.Sender.Group.Id, messages);
			return false; // Not to block the message transferring; otherwise, return true.
		}
	}
}

#endif