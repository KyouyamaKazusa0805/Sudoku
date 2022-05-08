namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// 根据时间字符串构建禁言时间
/// </summary>
public class JinxMaker : JinxTime
{
	private readonly Regex _typeTimeStamp = new(@"(\d{4})[-年](\d\d)[-月](\d\d)[\s日]*(\d\d)[:点时](\d\d)[:分](\d\d)秒?");

	private readonly Regex _typeTimeDelay = new(@"(\d+)\s*(年|星期|周|日|天|小?时|分钟?|秒钟?)");

	/// <summary>
	/// 构造禁言时间
	/// <para>
	/// 支持以下正则匹配的格式 (优先使用时间戳模式)：<br/>
	/// 时间戳模式 - "^(\d{4})[-年](\d\d)[-月](\d\d)[\s日]*(\d\d)[:点时](\d\d)[:分](\d\d)秒?\s*$"<br/>
	/// 倒计时模式 - "^(\d+)\s*(年|星期|周|日|天|小?时|分钟?|秒钟?)?\s*$"
	/// </para>
	/// </summary>
	public JinxMaker(string timeString = "1分钟")
	{
		if (string.IsNullOrEmpty(timeString))
		{
			return;
		}

		if (
#pragma warning disable IDE0055
			_typeTimeStamp.Match(timeString) is
			{
				Success: true,
				Groups: [
					_,
					{ Value: var a },
					{ Value: var b },
					{ Value: var c },
					{ Value: var d },
					{ Value: var e },
					{ Value: var f }
				]
			}
#pragma warning restore IDE0055
		)
		{
			string timstamp = $"{a}-{b}-{c} {d}:{e}:{f}";
			JinxEndTimestamp = new DateTimeOffset(Convert.ToDateTime(timstamp)).ToUnixTimeSeconds().ToString();
		}
		else if (
			_typeTimeDelay.Match(timeString) is
			{
				Success: true,
				Groups: [_, { Value: var timeRawValue }, { Value: var jinxTimeSpanUnit }]
			}
		)
		{
			int seconds = jinxTimeSpanUnit switch
			{
				"年" => 60 * 60 * 24 * 365,
				"星期" => 60 * 60 * 24 * 7,
				"周" => 60 * 60 * 24 * 7,
				"日" => 60 * 60 * 24,
				"天" => 60 * 60 * 24,
				"小时" => 60 * 60,
				"时" => 60 * 60,
				"分钟" => 60,
				"分" => 60,
				_ => 1
			};

			if (int.TryParse(timeRawValue, out int timeVal))
			{
				seconds *= timeVal;
			}

			JinxSeconds = seconds.ToString();
		}
	}
}
