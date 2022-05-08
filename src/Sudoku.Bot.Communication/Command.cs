namespace Sudoku.Bot.Communication;

/// <summary>
/// 指令对象
/// <para>提前封装的指令类，用于方便的处理指令</para>
/// </summary>
public class Command
{
	/// <summary>
	/// 构造指令对象
	/// <para>
	/// 匹配指令将调用 Rule 属性进行正则匹配<br/>
	/// 若rule未赋值，默认为 "^name(?=\s|\n|&lt;@!\d+&gt;|$)"
	/// </para>
	/// </summary>
	/// <param name="name">指令名称</param>
	/// <param name="callBack">回调函数</param>
	/// <param name="rule">匹配指令用的正则表达式<para>默认值：Regex("^name(?=\s|\n|&lt;@!\d+&gt;|$)")</para></param>
	/// <param name="needAdmin">需要管理员权限</param>
	/// <param name="note">备注,用户自定义属性功能用途</param>
	public Command(
		string name, Action<Sender, string>? callBack = null, Regex? rule = null,
		bool needAdmin = false, string? note = null)
	{
		Name = name;
		rule ??= new Regex($@"^{Regex.Escape(name)}\s*(?=\s|\d|\n|<@!\d+>|$)");
		CompiledRule = new Regex(rule.ToString(), rule.Options | RegexOptions.Compiled);
		CallBack = callBack;
		NeedAdmin = needAdmin;
		Note = note;
	}

	/// <summary>
	/// 指令名称
	/// </summary>
	public string Name { get; set; }

	/// <summary>
	/// 指令命中后的回调函数
	/// <para>例：(sender, args)=>{}</para>
	/// </summary>
	public Action<Sender, string>? CallBack { get; set; }

	/// <summary>
	/// 编译为程序集的正则表达式（可加快正则匹配速度）
	/// </summary>
	private Regex CompiledRule { get; set; }

	/// <summary>
	/// 匹配规则
	/// </summary>
	public Regex Rule
	{
		get => CompiledRule;

		set => CompiledRule = new(value.ToString(), value.Options | RegexOptions.Compiled);
	}

	/// <summary>
	/// 管理员权限
	/// <para>若设定为true，将只有管理员才能触发该指令</para>
	/// </summary>
	public bool NeedAdmin { get; set; } = false;

	/// <summary>
	/// 备注
	/// <para>用户自定义属性功能用途</para>
	/// </summary>
	public string? Note { get; set; }
}
