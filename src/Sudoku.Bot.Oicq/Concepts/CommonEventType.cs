#pragma warning disable CS1591

namespace Sudoku.Bot.Oicq.Concepts;

/// <summary>
/// The basic event type of QQ.
/// </summary>
public enum CommonEventType
{
	None = -1,

	OnlineTmp = 0,

	/// <summary>
	/// C2C message for a QQ friend.
	/// </summary>
	Friend = 1,

	/// <summary>
	/// Group message.
	/// </summary>
	Group = 2,

	/// <summary>
	/// Temporary message from a group.
	/// </summary>
	GroupTmp = 4,

	/// <summary>
	/// C2C message for QQ adding friend window.
	/// </summary>
	AddFriendReply = 7,

	/// <summary>
	/// Interact message.
	/// </summary>
	Money = 6,

	SomeoneWantAddFriend = 101,

	BeRemovedFriend = 104,

	FriendReceivedFile = 105,

	BeCommented = 107,

	SomeoneLeaveGroup = 201,

	BanSpeak = 203,

	UnBanSpeak = 204,

	AllBanSpeak = 205,

	AllUnBanSpeak = 206,

	SomeoneBeAllowedToGroup = 212,

	SomeoneWantAddGroup = 213,

	SelfBeInvitedToGroup = 214,

	SomeoneBeInvitedToGroup = 215,

	GroupDissolved = 216,

	GroupCardChanged = 217,

	SomeoneHasBeenInvitedIntoGroup = 219,

	GroupNameChanged = 220,

	BeRefusedGroup = 221,

	QQLogin = 1101,

	PluginLoaded = 12000,

	PluginEnable = 12001,

	PluginClicked = 12003,
}
