using Windows.UI.Popups;

namespace Sudoku.UI.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="MessageDialog"/>.
	/// </summary>
	public static class MessageDialogEx
	{
		/// <summary>
		/// Change the title to the specified value.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The current dialog instance.</param>
		/// <param name="title">The title to assign.</param>
		/// <returns>The instance of <see langword="this"/>.</returns>
		public static MessageDialog WithTitle(this MessageDialog @this, string title)
		{
			@this.Title = title;
			return @this;
		}

		/// <summary>
		/// Add a new command into the current instance.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The current dialog instance.</param>
		/// <param name="command">The command to assign.</param>
		/// <returns>The instance of <see langword="this"/>.</returns>
		/// <remarks>
		/// If you are stuck in the second parameter <paramref name="command"/>, please visit
		/// <see cref="WithCommand(MessageDialog, string, UICommandInvokedHandler)"/> for more information.
		/// </remarks>
		/// <seealso cref="WithCommand(MessageDialog, string, UICommandInvokedHandler)"/>
		public static MessageDialog WithCommand(this MessageDialog @this, IUICommand command)
		{
			@this.Commands.Add(command);
			return @this;
		}

		/// <summary>
		/// Add a new command into the current instance.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The current dialog instance.</param>
		/// <param name="label">The label of the new command (displayed as a button-like control).</param>
		/// <param name="handler">
		/// <para>A method that handles the command.</para>
		/// <para>
		/// The example value using lambda:
		/// <code>
		/// uiCommand => textBlock.Text = $"You just pressed: {uiCommand.Label}"
		/// </code>
		/// or using anonymous function:
		/// <code>
		/// deleagte(IUICommand uiCommand) { textBlock.Text = $"You just pressed: {uiCommand.Label}"; }
		/// </code>
		/// </para>
		/// </param>
		/// <returns>The instance of <see langword="this"/>.</returns>
		public static MessageDialog WithCommand(
			this MessageDialog @this, string label, UICommandInvokedHandler handler) =>
			@this.WithCommand(new UICommand(label, handler));
	}
}
