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
		/// </para>
		/// </param>
		/// <returns>The instance of <see langword="this"/>.</returns>
		public static MessageDialog WithCommand(
			this MessageDialog @this, string label, UICommandInvokedHandler handler) =>
			@this.WithCommand(new UICommand(label, handler));

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
		/// </para>
		/// </param>
		/// <param name="commandId">The command ID to set.</param>
		/// <returns>The instance of <see langword="this"/>.</returns>
		public static MessageDialog WithCommand(
			this MessageDialog @this, string label, UICommandInvokedHandler handler, object commandId) =>
			@this.WithCommand(new UICommand(label, handler, commandId));

		/// <summary>
		/// To set the default command a specified ID value.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The current dialog instance.</param>
		/// <param name="id">
		/// <para>The ID to set.</para>
		/// <para>
		/// This method should be called after you have set an ID that bound with a specified command.
		/// For example, you should call
		/// <see cref="WithCommand(MessageDialog, string, UICommandInvokedHandler, object)"/>
		/// first, and the third parameter is the ID that introduced above.
		/// </para>
		/// </param>
		/// <returns>The instance of <see langword="this"/>.</returns>
		/// <seealso cref="WithCommand(MessageDialog, string, UICommandInvokedHandler, object)"/>
		public static MessageDialog WithDefaultId(this MessageDialog @this, uint id)
		{
			@this.DefaultCommandIndex = id;
			return @this;
		}

		/// <summary>
		/// To set the cancel command a specified ID value.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The current dialog instance.</param>
		/// <param name="id">
		/// <para>The ID to set.</para>
		/// <para>
		/// This method should be called after you have set an ID that bound with a specified command.
		/// For example, you should call
		/// <see cref="WithCommand(MessageDialog, string, UICommandInvokedHandler, object)"/>
		/// first, and the third parameter is the ID that introduced above.
		/// </para>
		/// </param>
		/// <returns>The instance of <see langword="this"/>.</returns>
		/// <seealso cref="WithCommand(MessageDialog, string, UICommandInvokedHandler, object)"/>
		public static MessageDialog WithCancelId(this MessageDialog @this, uint id)
		{
			@this.CancelCommandIndex = id;
			return @this;
		}
	}
}
