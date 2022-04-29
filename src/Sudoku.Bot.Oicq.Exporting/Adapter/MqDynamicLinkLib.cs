namespace Sudoku.Bot.Oicq.Adapter;

internal static class MqDynamicLinkLib
{
	private const string DllName = "MyQQApi.dll";

	[DllImport(DllName, EntryPoint = "Api_SendMsgEx", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_SendMsgEx(string rotbotQQ, int any, int type, string group, string qq, string Msg, int bubbleId);

	[DllImport(DllName, EntryPoint = "Api_OutPut", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_OutPutLog(string note);

	[DllImport(DllName, EntryPoint = "Api_SendXML", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_SendXML(string rotbotQQ, int SendType, int type, string group, string qq, string xml, int SubType);

	[DllImport(DllName, EntryPoint = "Api_SendJson", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_SendJson(string rotbotQQ, int SendType, int type, string group, string qq, string json);

	[DllImport(DllName, EntryPoint = "Api_UpVote", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_UpVote(string rotbotQQ, string BeQQ);

	[DllImport(DllName, EntryPoint = "Api_GetCookies", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_GetCookies(string rotbotQQ);

	[DllImport(DllName, EntryPoint = "Api_GetBlogPsKey", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_GetBlogPsKey(string rotbotQQ);

	[DllImport(DllName, EntryPoint = "Api_GetZonePsKey", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_GetZonePsKey(string rotbotQQ);

	[DllImport(DllName, EntryPoint = "Api_GetGroupPsKey", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_GetGroupPsKey(string rotbotQQ);

	[DllImport(DllName, EntryPoint = "Api_GetClassRoomPsKey", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_GetClassRoomPsKey(string rotbotQQ);

	[DllImport(DllName, EntryPoint = "Api_GetBkn", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_GetBkn(string rotbotQQ);

	[DllImport(DllName, EntryPoint = "Api_GetBkn32", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_GetBkn32(string rotbotQQ);

	[DllImport(DllName, EntryPoint = "Api_GetLongLdw", CharSet = CharSet.Ansi)]
	public static extern long Api_GetLongLdw(string rotbotQQ);

	[DllImport(DllName, EntryPoint = "Api_GetClientkey", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_GetClientkey(string rotbotQQ);

	[DllImport(DllName, EntryPoint = "Api_GetLongClientkey", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_GetLongClientkey(string rotbotQQ);

	[DllImport(DllName, EntryPoint = "Api_AdminInviteGroup", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_AdminInviteGroup(string rotbotQQ, string beQQ, string group);

	[DllImport(DllName, EntryPoint = "Api_NoAdminInviteGroup", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_NoAdminInviteGroup(string rotbotQQ, string beQQ, string group);

	[DllImport(DllName, EntryPoint = "Api_GetNick", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_GetNick(string rotbotQQ, string beQQ);

	[DllImport(DllName, EntryPoint = "Api_GetGroupCard", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_GetGroupCard(string rotbotQQ, string group, string beQQ);

	[DllImport(DllName, EntryPoint = "Api_GetObjLevel", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_GetObjLevel(string rotbotQQ, string beQQ);

	[DllImport(DllName, EntryPoint = "Api_GetFriendList", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_GetFriendList(string rotbotQQ);

	[DllImport(DllName, EntryPoint = "Api_GetGroupAdmin", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_GetGroupAdmin(string rotbotQQ, string group);

	[DllImport(DllName, EntryPoint = "Api_GetGroupList", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_GetGroupList(string rotbotQQ);

	[DllImport(DllName, EntryPoint = "Api_GetGroupMemberList", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_GetGroupMemberList(string rotbotQQ, string group);

	[DllImport(DllName, EntryPoint = "Api_IsShutUp", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_IsShutUp(string rotbotQQ, string group, string beqq);

	[DllImport(DllName, EntryPoint = "Api_ShutUP", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_ShutUP(string rotbotQQ, string group, string beqq, int Time);

	[DllImport(DllName, EntryPoint = "Api_JoinGroup", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_JoinGroup(string rotbotQQ, string group, int text);

	[DllImport(DllName, EntryPoint = "Api_QuitGroup", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_QuitGroup(string rotbotQQ, string group);

	[DllImport(DllName, EntryPoint = "Api_UpLoadPic", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_UpLoadPic(string rotbotQQ, int upType, string group, byte[] str);

	[DllImport(DllName, EntryPoint = "Api_GetPicLink", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_GetPicLink(string rotbotQQ, int ImgType, string from, string Guid);


	[DllImport(DllName, EntryPoint = "Api_HandleGroupEvent", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_HandleGroupEvent(string rotbotQQ, int type, string beqq, string group, string seq, int handle, string note);

	[DllImport(DllName, EntryPoint = "Api_Tea加密", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_Tea加密(string str, string key);

	[DllImport(DllName, EntryPoint = "Api_Tea解密", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_Tea解密(string str, string key);

	[DllImport(DllName, EntryPoint = "Api_SessionKey", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_SessionKey(string rotbotQQ);

	[DllImport(DllName, EntryPoint = "Api_GNTransGID", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_GNTransGID(string group);

	[DllImport(DllName, EntryPoint = "Api_GIDTransGN", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_GIDTransGN(string groupID);

	[DllImport(DllName, EntryPoint = "Api_PBGroupNotic", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_PBGroupNotic(string robotQQ, string group, string title, string note);

	[DllImport(DllName, EntryPoint = "Api_GetNotice", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_GetNotice(string robotQQ, string group);

	[DllImport(DllName, EntryPoint = "Api_ShakeWindow", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_ShakeWindow(string robotQQ, string qq);

	[DllImport(DllName, EntryPoint = "Api_SetAnon", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_SetAnon(string robotQQ, string group, bool BoolEan);

	[DllImport(DllName, EntryPoint = "Api_SetGroupCard", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_SetGroupCard(string robotQQ, string group, string qq, string card);

	[DllImport(DllName, EntryPoint = "Api_CreateDisGroup", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_CreateDisGroup(string rotbotQQ, string disName);

	[DllImport(DllName, EntryPoint = "Api_KickDisGroupMBR", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_KickDisGroupMBR(string rotbotQQ, string disID, string qq);

	[DllImport(DllName, EntryPoint = "Api_InviteDisGroup", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_InviteDisGroup(string rotbotQQ, string disID, string qq);

	[DllImport(DllName, EntryPoint = "Api_GetDisGroupList", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_GetDisGroupList(string rotbotQQ);

	[DllImport(DllName, EntryPoint = "Api_GetDisGroupMemberList", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_GetDisGroupMemberList(string rotbotQQ, string disID);

	[DllImport(DllName, EntryPoint = "Api_SetDisGroupName", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_SetDisGroupName(string rotbotQQ, string disID, string disName);

	[DllImport(DllName, EntryPoint = "Api_KickGroupMBR", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_KickGroupMBR(string robotQQ, string group, string beqq, bool black);

	[DllImport(DllName, EntryPoint = "Api_GetObjVote", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_GetObjVote(string robotQQ, string qq);

	[DllImport(DllName, EntryPoint = "Api_UpLoadVoice", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_UpLoadVoice(string rotbotQQ, string type, string group, byte[] arr);

	[DllImport(DllName, EntryPoint = "Api_GetVoiLink", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_GetVoiLink(string rotbotQQ, string guid);

	[DllImport(DllName, EntryPoint = "Api_GetTimeStamp", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_GetTimeStamp();

	[DllImport(DllName, EntryPoint = "Api_SendPack", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_SendPack(string rotbotQQ, string note);

	[DllImport(DllName, EntryPoint = "Api_GetObjInfo", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_GetObjInfo(string rotbotQQ, string qq);

	[DllImport(DllName, EntryPoint = "Api_GetGender", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_GetGender(string rotbotQQ, string qq);

	[DllImport(DllName, EntryPoint = "Api_GetQQAge", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_GetQQAge(string robotQQ, string qq);

	[DllImport(DllName, EntryPoint = "Api_GetAge", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_GetAge(string robotQQ, string qq);

	[DllImport(DllName, EntryPoint = "Api_GetPerExp", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_GetPerExp(string robotQQ, string qq);

	[DllImport(DllName, EntryPoint = "Api_GetSign", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_GetSign(string robotQQ, string qq);

	[DllImport(DllName, EntryPoint = "Api_GetEmail", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_GetEmail(string robotQQ, string qq);

	[DllImport(DllName, EntryPoint = "Api_GetGroupName", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_GetGroupName(string rotbotQQ, string group);

	[DllImport(DllName, EntryPoint = "Api_GetVer", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_GetVer();

	[DllImport(DllName, EntryPoint = "Api_GetQQList", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_GetQQList();

	[DllImport(DllName, EntryPoint = "Api_GetOnLineList", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_GetOnLineList();

	[DllImport(DllName, EntryPoint = "Api_GetOffLineList", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_GetOffLineList();

	[DllImport(DllName, EntryPoint = "Api_AddQQ", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_AddQQ(string robotQQ, string pwd, bool login);

	[DllImport(DllName, EntryPoint = "Api_DelQQ", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_DelQQ(string robotQQ);

	[DllImport(DllName, EntryPoint = "Api_LoginQQ", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_LoginQQ(string rotbotQQ);

	[DllImport(DllName, EntryPoint = "Api_OffLineQQ", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_OffLineQQ(string rotbotQQ);

	[DllImport(DllName, EntryPoint = "Api_IfFriend", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_IfFriend(string robotQQ, string qq);

	[DllImport(DllName, EntryPoint = "Api_SetRInf", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_SetRInf(string rotbotQQ, int type, string note);

	[DllImport(DllName, EntryPoint = "Api_GetRInf", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_GetRInf(string rotbotQQ);

	[DllImport(DllName, EntryPoint = "Api_DelFriend", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_DelFriend(string rotbotQQ, string qq);

	[DllImport(DllName, EntryPoint = "Api_AddBkList", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_AddBkList(string rotbotQQ, string qq);

	[DllImport(DllName, EntryPoint = "Api_DelBkList", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_DelBkList(string rotbotQQ, string qq);

	[DllImport(DllName, EntryPoint = "Api_SetShieldedGroup", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_SetShieldedGroup(string rotbotQQ, string group, bool banSpeak);

	[DllImport(DllName, EntryPoint = "Api_SendVoice", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_SendVoice(string rotbotQQ, string qq, byte[] arr);

	[DllImport(DllName, EntryPoint = "Api_SetAdmin", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_SetAdmin(string robotQQ, string group, string qq, bool adm);

	[DllImport(DllName, EntryPoint = "Api_PBHomeWork", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_PBHomeWork(string robotQQ, string group, string work, string title, string note);

	[DllImport(DllName, EntryPoint = "Api_GetLog", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_GetLog();

	[DllImport(DllName, EntryPoint = "Api_IsEnable", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_IsEnable();

	[DllImport(DllName, EntryPoint = "Api_DisabledPlugin", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_DisabledPlugin();

	[DllImport(DllName, EntryPoint = "Api_WithdrawMsg", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_WithdrawMsg(string rotbotQQ, string group, string msgNum, string msgID);

	[DllImport(DllName, EntryPoint = "Api_BeInput", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_BeInput(string robotQQ, string qq);

	[DllImport(DllName, EntryPoint = "Api_GetQQAddMode", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_GetQQAddMode(string robotQQ, string qq);

	[DllImport(DllName, EntryPoint = "Api_IsOnline", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_IsOnline(string robotQQ, string qq);

	[DllImport(DllName, EntryPoint = "Api_GetOnlineState", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_GetOnlineState(string robotQQ, string qq);

	[DllImport(DllName, EntryPoint = "Api_GetGroupMemberNum", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_GetGroupMemberNum(string robotQQ, string group);

	[DllImport(DllName, EntryPoint = "Api_GetWpa", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_GetWpa(string robotQQ, string qq);

	[DllImport(DllName, EntryPoint = "Api_GetGroupAddMode", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_GetGroupAddMode(string robotQQ, string group);

	[DllImport(DllName, EntryPoint = "Api_GetGroupLv", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_GetGroupLv(string rotbotQQ, string group);

	[DllImport(DllName, EntryPoint = "Api_SetFriendsRemark", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_SetFriendsRemark(string robotQQ, string qq, string card);

	[DllImport(DllName, EntryPoint = "Api_GetFriendsRemark", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_GetFriendsRemark(string rotbotQQ, string qq);

	[DllImport(DllName, EntryPoint = "Api_SignIn", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_SignIn(string rotbotQQ, string group, string addr, string note);

	[DllImport(DllName, EntryPoint = "Api_TakeGift", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_TakeGift(string rotbotQQ);

	[DllImport(DllName, EntryPoint = "Api_CheckGift", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_CheckGift(string rotbotQQ);

	[DllImport(DllName, EntryPoint = "Api_GiveGift", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_GiveGift(string rotbotQQ, string group, string beqq, string pid);

	[DllImport(DllName, EntryPoint = "Api_GetGroupChatLv", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_GetGroupChatLv(string rotbotQQ, string group, string qq);

	[DllImport(DllName, EntryPoint = "Api_GetExpertDays", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_GetExpertDays(string rotbotQQ, string qq);

	[DllImport(DllName, EntryPoint = "Api_InviteGroup_other", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_InviteGroup_other(string rotbotQQ, string group, string begroup, string qq);

	[DllImport(DllName, EntryPoint = "Api_GetShieldedState", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_GetShieldedState(string robotQQ, int type);

	[DllImport(DllName, EntryPoint = "Api_DelFriend_A", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_DelFriend_A(string robotQQ, string beqq, int delType);

	[DllImport(DllName, EntryPoint = "Api_HandleFriendEvent", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_HandleFriendEvent(string robotQQ, string beqq, int handleType, string note);

	[DllImport(DllName, EntryPoint = "Api_AddFriend", CharSet = CharSet.Ansi)]
	public static extern bool Api_AddFriend(string robotQQ, string qq, string note, int type);

	[DllImport(DllName, EntryPoint = "Api_UpDate", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_UpDate(string fromArr, string temp, string url);

	[DllImport(DllName, EntryPoint = "Api_GetQrcode", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_GetQrcode();

	[DllImport(DllName, EntryPoint = "Api_CheckQrcode", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_CheckQrcode(string id);

	[DllImport(DllName, EntryPoint = "Api_SetGroupCation", CharSet = CharSet.Ansi)]
	public static extern IntPtr Api_SetGroupCation(string robotQQ, string group, int type, string program, string note);
}
