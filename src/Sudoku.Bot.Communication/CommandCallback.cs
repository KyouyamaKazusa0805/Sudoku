namespace Sudoku.Bot.Communication;

/// <summary>
/// Indicates the callback delegate type that is used for handling on a command.
/// </summary>
/// <param name="sender">The sender instance who sends the message.</param>
/// <param name="message">The message content. Here the argument will remove the extra whitespaces.</param>
[SuppressMessage("Naming", "IDE1006:Naming Styles", Justification = "<Pending>")]
public delegate void CommandCallback(Sender sender, string message);