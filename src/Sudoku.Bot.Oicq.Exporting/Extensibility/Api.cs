namespace Sudoku.Bot.Oicq.Extensibility;

/// <summary>
/// Provides with the MyQQ framework-wide dynamic linking library.
/// </summary>
internal static class Api
{
	[DllImport("MyQQApi", EntryPoint = "Api_SendMsgEx", CharSet = CharSet.Ansi)]
	public static extern IntPtr SendMessage(
		string botQQ, int any, int type, string group, string qq, string msg, int bubbleId);

	[DllImport("MyQQApi", EntryPoint = "Api_OutPut", CharSet = CharSet.Ansi)]
	public static extern IntPtr Output(string note);

	[DllImport("MyQQApi", EntryPoint = "Api_SendXML", CharSet = CharSet.Ansi)]
	public static extern IntPtr SendXml(
		string botQQ, int sendType, int type, string group, string qq, string xml, int SubType);

	[DllImport("MyQQApi", EntryPoint = "Api_SendJson", CharSet = CharSet.Ansi)]
	public static extern IntPtr SendJson(string botQQ, int sendType, int type, string group, string qq, string json);

	[DllImport("MyQQApi", EntryPoint = "Api_UpVote", CharSet = CharSet.Ansi)]
	public static extern IntPtr Upvote(string botQQ, string beQQ);

	[DllImport("MyQQApi", EntryPoint = "Api_GetCookies", CharSet = CharSet.Ansi)]
	public static extern IntPtr GetCookies(string botQQ);

	[DllImport("MyQQApi", EntryPoint = "Api_GetBlogPsKey", CharSet = CharSet.Ansi)]
	public static extern IntPtr GetBlogPsKey(string botQQ);

	[DllImport("MyQQApi", EntryPoint = "Api_GetZonePsKey", CharSet = CharSet.Ansi)]
	public static extern IntPtr GetZonePsKey(string botQQ);

	[DllImport("MyQQApi", EntryPoint = "Api_GetGroupPsKey", CharSet = CharSet.Ansi)]
	public static extern IntPtr GetGroupPsKey(string botQQ);

	[DllImport("MyQQApi", EntryPoint = "Api_GetClassRoomPsKey", CharSet = CharSet.Ansi)]
	public static extern IntPtr GetClassroomPsKey(string botQQ);

	[DllImport("MyQQApi", EntryPoint = "Api_GetBkn", CharSet = CharSet.Ansi)]
	public static extern IntPtr GetBkn(string botQQ);

	[DllImport("MyQQApi", EntryPoint = "Api_GetBkn32", CharSet = CharSet.Ansi)]
	public static extern IntPtr GetBkn32(string botQQ);

	[DllImport("MyQQApi", EntryPoint = "Api_GetLongLdw", CharSet = CharSet.Ansi)]
	public static extern long GetLongLdw(string botQQ);

	[DllImport("MyQQApi", EntryPoint = "Api_GetClientkey", CharSet = CharSet.Ansi)]
	public static extern IntPtr GetClientKey(string botQQ);

	[DllImport("MyQQApi", EntryPoint = "Api_GetLongClientkey", CharSet = CharSet.Ansi)]
	public static extern IntPtr GetLongClientkey(string botQQ);

	[DllImport("MyQQApi", EntryPoint = "Api_AdminInviteGroup", CharSet = CharSet.Ansi)]
	public static extern IntPtr OwnerInviteGroup(string botQQ, string beQQ, string group);

	[DllImport("MyQQApi", EntryPoint = "Api_NoAdminInviteGroup", CharSet = CharSet.Ansi)]
	public static extern IntPtr NoOwnerInviteGroup(string botQQ, string beQQ, string group);

	[DllImport("MyQQApi", EntryPoint = "Api_GetNick", CharSet = CharSet.Ansi)]
	public static extern IntPtr GetNickname(string botQQ, string beQQ);

	[DllImport("MyQQApi", EntryPoint = "Api_GetGroupCard", CharSet = CharSet.Ansi)]
	public static extern IntPtr GetGroupCard(string botQQ, string group, string beQQ);

	[DllImport("MyQQApi", EntryPoint = "Api_GetObjLevel", CharSet = CharSet.Ansi)]
	public static extern IntPtr GetObjectLevel(string botQQ, string beQQ);

	[DllImport("MyQQApi", EntryPoint = "Api_GetFriendList", CharSet = CharSet.Ansi)]
	public static extern IntPtr GetFriendList(string botQQ);

	[DllImport("MyQQApi", EntryPoint = "Api_GetGroupAdmin", CharSet = CharSet.Ansi)]
	public static extern IntPtr GetGroupOwner(string botQQ, string group);

	[DllImport("MyQQApi", EntryPoint = "Api_GetGroupList", CharSet = CharSet.Ansi)]
	public static extern IntPtr GetGroupList(string botQQ);

	[DllImport("MyQQApi", EntryPoint = "Api_GetGroupMemberList", CharSet = CharSet.Ansi)]
	public static extern IntPtr GetGroupMemberList(string botQQ, string group);

	[DllImport("MyQQApi", EntryPoint = "Api_IsShutUp", CharSet = CharSet.Ansi)]
	public static extern IntPtr IsJinxed(string botQQ, string group, string beQQ);

	[DllImport("MyQQApi", EntryPoint = "Api_ShutUP", CharSet = CharSet.Ansi)]
	public static extern IntPtr Jinx(string botQQ, string group, string beQQ, int Time);

	[DllImport("MyQQApi", EntryPoint = "Api_JoinGroup", CharSet = CharSet.Ansi)]
	public static extern IntPtr JoinInGroup(string botQQ, string group, int text);

	[DllImport("MyQQApi", EntryPoint = "Api_QuitGroup", CharSet = CharSet.Ansi)]
	public static extern IntPtr QuitFromGroup(string botQQ, string group);

	[DllImport("MyQQApi", EntryPoint = "Api_UpLoadPic", CharSet = CharSet.Ansi)]
	public static extern IntPtr UploadPicture(string botQQ, int upType, string group, byte[] str);

	[DllImport("MyQQApi", EntryPoint = "Api_GetPicLink", CharSet = CharSet.Ansi)]
	public static extern IntPtr GetPictureLink(string botQQ, int pictureType, string from, string guid);

	[DllImport("MyQQApi", EntryPoint = "Api_HandleGroupEvent", CharSet = CharSet.Ansi)]
	public static extern IntPtr HandleGroupEvent(
		string botQQ, int type, string beQQ, string group, string seq, int handle, string note);

	[DllImport("MyQQApi", EntryPoint = "Api_Tea\u52A0\u5BC6", CharSet = CharSet.Ansi)]
	public static extern IntPtr TeaEncode(string str, string key);

	[DllImport("MyQQApi", EntryPoint = "Api_Tea\u89E3\u5BC6", CharSet = CharSet.Ansi)]
	public static extern IntPtr TeaDecode(string str, string key);

	[DllImport("MyQQApi", EntryPoint = "Api_SessionKey", CharSet = CharSet.Ansi)]
	public static extern IntPtr SessionKey(string botQQ);

	[DllImport("MyQQApi", EntryPoint = "Api_GNTransGID", CharSet = CharSet.Ansi)]
	public static extern IntPtr Gn2Gid(string group);

	[DllImport("MyQQApi", EntryPoint = "Api_GIDTransGN", CharSet = CharSet.Ansi)]
	public static extern IntPtr Gid2Gn(string groupID);

	[DllImport("MyQQApi", EntryPoint = "Api_PBGroupNotic", CharSet = CharSet.Ansi)]
	public static extern IntPtr PbGroupNotice(string botQQ, string group, string title, string note);

	[DllImport("MyQQApi", EntryPoint = "Api_GetNotice", CharSet = CharSet.Ansi)]
	public static extern IntPtr GetNotice(string botQQ, string group);

	[DllImport("MyQQApi", EntryPoint = "Api_ShakeWindow", CharSet = CharSet.Ansi)]
	public static extern IntPtr Shake(string botQQ, string qq);

	[DllImport("MyQQApi", EntryPoint = "Api_SetAnon", CharSet = CharSet.Ansi)]
	public static extern IntPtr SetAnonymous(string botQQ, string group, bool BoolEan);

	[DllImport("MyQQApi", EntryPoint = "Api_SetGroupCard", CharSet = CharSet.Ansi)]
	public static extern IntPtr SetGroupCard(string botQQ, string group, string qq, string card);

	[DllImport("MyQQApi", EntryPoint = "Api_CreateDisGroup", CharSet = CharSet.Ansi)]
	public static extern IntPtr CreateDisccussionGroup(string botQQ, string disName);

	[DllImport("MyQQApi", EntryPoint = "Api_KickDisGroupMBR", CharSet = CharSet.Ansi)]
	public static extern IntPtr KickDisccussionGroupMember(string botQQ, string disID, string qq);

	[DllImport("MyQQApi", EntryPoint = "Api_InviteDisGroup", CharSet = CharSet.Ansi)]
	public static extern IntPtr InviteDisccussionGroup(string botQQ, string disID, string qq);

	[DllImport("MyQQApi", EntryPoint = "Api_GetDisGroupList", CharSet = CharSet.Ansi)]
	public static extern IntPtr GetDisccussionGroupList(string botQQ);

	[DllImport("MyQQApi", EntryPoint = "Api_GetDisGroupMemberList", CharSet = CharSet.Ansi)]
	public static extern IntPtr GetDisccussionGroupMemberList(string botQQ, string disID);

	[DllImport("MyQQApi", EntryPoint = "Api_SetDisGroupName", CharSet = CharSet.Ansi)]
	public static extern IntPtr SetDisccussionGroupName(string botQQ, string disID, string disName);

	[DllImport("MyQQApi", EntryPoint = "Api_KickGroupMBR", CharSet = CharSet.Ansi)]
	public static extern IntPtr KickGroupMember(string botQQ, string group, string beQQ, bool black);

	[DllImport("MyQQApi", EntryPoint = "Api_GetObjVote", CharSet = CharSet.Ansi)]
	public static extern IntPtr GetObjectVote(string botQQ, string qq);

	[DllImport("MyQQApi", EntryPoint = "Api_UpLoadVoice", CharSet = CharSet.Ansi)]
	public static extern IntPtr UploadVoice(string botQQ, string type, string group, byte[] arr);

	[DllImport("MyQQApi", EntryPoint = "Api_GetVoiLink", CharSet = CharSet.Ansi)]
	public static extern IntPtr GetVoiceLink(string botQQ, string guid);

	[DllImport("MyQQApi", EntryPoint = "Api_GetTimeStamp", CharSet = CharSet.Ansi)]
	public static extern IntPtr GetTimestamp();

	[DllImport("MyQQApi", EntryPoint = "Api_SendPack", CharSet = CharSet.Ansi)]
	public static extern IntPtr SendPack(string botQQ, string note);

	[DllImport("MyQQApi", EntryPoint = "Api_GetObjInfo", CharSet = CharSet.Ansi)]
	public static extern IntPtr GetObjectInfo(string botQQ, string qq);

	[DllImport("MyQQApi", EntryPoint = "Api_GetGender", CharSet = CharSet.Ansi)]
	public static extern IntPtr GetGender(string botQQ, string qq);

	[DllImport("MyQQApi", EntryPoint = "Api_GetQQAge", CharSet = CharSet.Ansi)]
	public static extern IntPtr GetQqAge(string botQQ, string qq);

	[DllImport("MyQQApi", EntryPoint = "Api_GetAge", CharSet = CharSet.Ansi)]
	public static extern IntPtr GetAge(string botQQ, string qq);

	[DllImport("MyQQApi", EntryPoint = "Api_GetPerExp", CharSet = CharSet.Ansi)]
	public static extern IntPtr GetPerExp(string botQQ, string qq);

	[DllImport("MyQQApi", EntryPoint = "Api_GetSign", CharSet = CharSet.Ansi)]
	public static extern IntPtr GetSign(string botQQ, string qq);

	[DllImport("MyQQApi", EntryPoint = "Api_GetEmail", CharSet = CharSet.Ansi)]
	public static extern IntPtr GetEmail(string botQQ, string qq);

	[DllImport("MyQQApi", EntryPoint = "Api_GetGroupName", CharSet = CharSet.Ansi)]
	public static extern IntPtr GetGroupName(string botQQ, string group);

	[DllImport("MyQQApi", EntryPoint = "Api_GetVer", CharSet = CharSet.Ansi)]
	public static extern IntPtr GetVersion();

	[DllImport("MyQQApi", EntryPoint = "Api_GetQQList", CharSet = CharSet.Ansi)]
	public static extern IntPtr GetQqList();

	[DllImport("MyQQApi", EntryPoint = "Api_GetOnLineList", CharSet = CharSet.Ansi)]
	public static extern IntPtr GetOnlineList();

	[DllImport("MyQQApi", EntryPoint = "Api_GetOffLineList", CharSet = CharSet.Ansi)]
	public static extern IntPtr GetOfflineList();

	[DllImport("MyQQApi", EntryPoint = "Api_AddQQ", CharSet = CharSet.Ansi)]
	public static extern IntPtr AddQq(string botQQ, string pwd, bool login);

	[DllImport("MyQQApi", EntryPoint = "Api_DelQQ", CharSet = CharSet.Ansi)]
	public static extern IntPtr DeleteQq(string botQQ);

	[DllImport("MyQQApi", EntryPoint = "Api_LoginQQ", CharSet = CharSet.Ansi)]
	public static extern IntPtr LoginQq(string botQQ);

	[DllImport("MyQQApi", EntryPoint = "Api_OffLineQQ", CharSet = CharSet.Ansi)]
	public static extern IntPtr OfflineQq(string botQQ);

	[DllImport("MyQQApi", EntryPoint = "Api_IfFriend", CharSet = CharSet.Ansi)]
	public static extern IntPtr IfFriend(string botQQ, string qq);

	[DllImport("MyQQApi", EntryPoint = "Api_SetRInf", CharSet = CharSet.Ansi)]
	public static extern IntPtr SetRInf(string botQQ, int type, string note);

	[DllImport("MyQQApi", EntryPoint = "Api_GetRInf", CharSet = CharSet.Ansi)]
	public static extern IntPtr GetRInf(string botQQ);

	[DllImport("MyQQApi", EntryPoint = "Api_DelFriend", CharSet = CharSet.Ansi)]
	public static extern IntPtr DeleteFriend(string botQQ, string qq);

	[DllImport("MyQQApi", EntryPoint = "Api_AddBkList", CharSet = CharSet.Ansi)]
	public static extern IntPtr AddToBlackList(string botQQ, string qq);

	[DllImport("MyQQApi", EntryPoint = "Api_DelBkList", CharSet = CharSet.Ansi)]
	public static extern IntPtr DeleteFromBlackList(string botQQ, string qq);

	[DllImport("MyQQApi", EntryPoint = "Api_SetShieldedGroup", CharSet = CharSet.Ansi)]
	public static extern IntPtr SetShieldGroup(string botQQ, string group, bool banSpeak);

	[DllImport("MyQQApi", EntryPoint = "Api_SendVoice", CharSet = CharSet.Ansi)]
	public static extern IntPtr SendVoice(string botQQ, string qq, byte[] arr);

	[DllImport("MyQQApi", EntryPoint = "Api_SetAdmin", CharSet = CharSet.Ansi)]
	public static extern IntPtr SetOwner(string botQQ, string group, string qq, bool adm);

	[DllImport("MyQQApi", EntryPoint = "Api_PBHomeWork", CharSet = CharSet.Ansi)]
	public static extern IntPtr PbHomework(string botQQ, string group, string work, string title, string note);

	[DllImport("MyQQApi", EntryPoint = "Api_GetLog", CharSet = CharSet.Ansi)]
	public static extern IntPtr GetLog();

	[DllImport("MyQQApi", EntryPoint = "Api_IsEnable", CharSet = CharSet.Ansi)]
	public static extern IntPtr IsEnabled();

	[DllImport("MyQQApi", EntryPoint = "Api_DisabledPlugin", CharSet = CharSet.Ansi)]
	public static extern IntPtr DisablePlugin();

	[DllImport("MyQQApi", EntryPoint = "Api_WithdrawMsg", CharSet = CharSet.Ansi)]
	public static extern IntPtr RecallMessage(string botQQ, string group, string msgNum, string msgID);

	[DllImport("MyQQApi", EntryPoint = "Api_BeInput", CharSet = CharSet.Ansi)]
	public static extern IntPtr Input(string botQQ, string qq);

	[DllImport("MyQQApi", EntryPoint = "Api_GetQQAddMode", CharSet = CharSet.Ansi)]
	public static extern IntPtr GetAddFriendMode(string botQQ, string qq);

	[DllImport("MyQQApi", EntryPoint = "Api_IsOnline", CharSet = CharSet.Ansi)]
	public static extern IntPtr IsOnline(string botQQ, string qq);

	[DllImport("MyQQApi", EntryPoint = "Api_GetOnlineState", CharSet = CharSet.Ansi)]
	public static extern IntPtr GetOnlineState(string botQQ, string qq);

	[DllImport("MyQQApi", EntryPoint = "Api_GetGroupMemberNum", CharSet = CharSet.Ansi)]
	public static extern IntPtr GetGroupMemberNumber(string botQQ, string group);

	[DllImport("MyQQApi", EntryPoint = "Api_GetWpa", CharSet = CharSet.Ansi)]
	public static extern IntPtr GetWpa(string botQQ, string qq);

	[DllImport("MyQQApi", EntryPoint = "Api_GetGroupAddMode", CharSet = CharSet.Ansi)]
	public static extern IntPtr GetAddGroupMode(string botQQ, string group);

	[DllImport("MyQQApi", EntryPoint = "Api_GetGroupLv", CharSet = CharSet.Ansi)]
	public static extern IntPtr GetGroupLevel(string botQQ, string group);

	[DllImport("MyQQApi", EntryPoint = "Api_SetFriendsRemark", CharSet = CharSet.Ansi)]
	public static extern IntPtr SetFriendsRemark(string botQQ, string qq, string card);

	[DllImport("MyQQApi", EntryPoint = "Api_GetFriendsRemark", CharSet = CharSet.Ansi)]
	public static extern IntPtr GetFriendsRemark(string botQQ, string qq);

	[DllImport("MyQQApi", EntryPoint = "Api_SignIn", CharSet = CharSet.Ansi)]
	public static extern IntPtr SignIn(string botQQ, string group, string addr, string note);

	[DllImport("MyQQApi", EntryPoint = "Api_TakeGift", CharSet = CharSet.Ansi)]
	public static extern IntPtr TakeGift(string botQQ);

	[DllImport("MyQQApi", EntryPoint = "Api_CheckGift", CharSet = CharSet.Ansi)]
	public static extern IntPtr CheckGift(string botQQ);

	[DllImport("MyQQApi", EntryPoint = "Api_GiveGift", CharSet = CharSet.Ansi)]
	public static extern IntPtr GiveGift(string botQQ, string group, string beQQ, string pid);

	[DllImport("MyQQApi", EntryPoint = "Api_GetGroupChatLv", CharSet = CharSet.Ansi)]
	public static extern IntPtr GetGroupChattingLevel(string botQQ, string group, string qq);

	[DllImport("MyQQApi", EntryPoint = "Api_GetExpertDays", CharSet = CharSet.Ansi)]
	public static extern IntPtr GetExpertDayCount(string botQQ, string qq);

	[DllImport("MyQQApi", EntryPoint = "Api_InviteGroup_other", CharSet = CharSet.Ansi)]
	public static extern IntPtr InviteGroupMiscellaneous(string botQQ, string group, string begroup, string qq);

	[DllImport("MyQQApi", EntryPoint = "Api_GetShieldedState", CharSet = CharSet.Ansi)]
	public static extern IntPtr GetShieldState(string botQQ, int type);

	[DllImport("MyQQApi", EntryPoint = "Api_DelFriend_A", CharSet = CharSet.Ansi)]
	public static extern IntPtr DeleteFriendA(string botQQ, string beQQ, int delType);

	[DllImport("MyQQApi", EntryPoint = "Api_HandleFriendEvent", CharSet = CharSet.Ansi)]
	public static extern IntPtr HandleFriendEvent(string botQQ, string beQQ, int handleType, string note);

	[DllImport("MyQQApi", EntryPoint = "Api_AddFriend", CharSet = CharSet.Ansi)]
	public static extern bool AddFriend(string botQQ, string qq, string note, int type);

	[DllImport("MyQQApi", EntryPoint = "Api_UpDate", CharSet = CharSet.Ansi)]
	public static extern IntPtr Update(string fromArr, string temp, string url);

	[DllImport("MyQQApi", EntryPoint = "Api_GetQrcode", CharSet = CharSet.Ansi)]
	public static extern IntPtr GetQrCode();

	[DllImport("MyQQApi", EntryPoint = "Api_CheckQrcode", CharSet = CharSet.Ansi)]
	public static extern IntPtr CheckQrCode(string id);

	[DllImport("MyQQApi", EntryPoint = "Api_SetGroupCation", CharSet = CharSet.Ansi)]
	public static extern IntPtr SetGroupAction(string botQQ, string group, int type, string program, string note);
}
