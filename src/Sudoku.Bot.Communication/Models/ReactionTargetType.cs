namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// Indicates the type of the reaction.
/// </summary>
/// <remarks>
/// The data type is referenced from
/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/reaction/model.html#reactiontargettype">this link</see>.
/// </remarks>
public enum ReactionTargetType
{
	/// <summary>
	/// Indicates the reaction is applied to a message.
	/// </summary>
	Message,

	/// <summary>
	/// Indicates the reaction is applied to a post.
	/// </summary>
	Post,

	/// <summary>
	/// Indicates the reaction is applied to a comment.
	/// </summary>
	Comment,

	/// <summary>
	/// Indicates the reaction is applied to a reply.
	/// </summary>
	Reply
}
