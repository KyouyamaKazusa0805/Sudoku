namespace Sudoku.Bot.Oicq.Concepts;

/// <summary>
/// Indicates the event type of the Amiable framework.
/// </summary>
public enum AmiableEventType
{
	/// <summary>
	/// Indicate the event that the code cannot be parsed successfully. The event is not supported at present.
	/// </summary>
	Error,

	/// <summary>
	/// Indicates the event that is triggered in C2C.
	/// </summary>
	Private,

	/// <summary>
	/// Indicates the event that is triggered in QQ group.
	/// </summary>
	Group,

	/// <summary>
	/// Indicates the event that is triggered while adding a friend.
	/// </summary>
	AddFriend,

	/// <summary>
	/// Indicates the event that is triggered while joining in a QQ group.
	/// </summary>
	AddGroup,

	/// <summary>
	/// Indicates the event that is triggered when the plugin is enabled.
	/// </summary>
	PluginEnable,

	/// <summary>
	/// Indicates the event that is triggered when the plugin is loaded.
	/// </summary>
	PluginLoaded,

	/// <summary>
	/// Indicates the event that is triggered when the plugin menu is invoked.
	/// </summary>
	PluginMenu,

	/// <summary>
	/// Indicates the Amiable framework is loaded.
	/// </summary>
	AmiableLoaded
}
