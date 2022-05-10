namespace Sudoku.Bot.Communication.Triggering;

/// <summary>
/// Provides with the event data for <see cref="AuthoizationPassedEventHandler"/>.
/// </summary>
/// <seealso cref="AuthoizationPassedEventHandler"/>
public sealed class AuthoizationPassedEventArgs : EventArgs
{
	/// <summary>
	/// Initializes an <see cref="AuthoizationPassedEventArgs"/> instance via the user instance.
	/// </summary>
	/// <param name="user">The user instance.</param>
	public AuthoizationPassedEventArgs(User user) => User = user;


	/// <summary>
	/// Indicates the data of the bot. Currently the instance will only be assigned
	/// properties <see cref="User.Id"/>, <see cref="User.UserName"/> and <see cref="User.IsBot"/>.
	/// </summary>
	public User User { get; }
}
