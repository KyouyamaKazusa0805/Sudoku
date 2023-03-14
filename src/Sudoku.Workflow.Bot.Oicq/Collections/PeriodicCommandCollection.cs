namespace Sudoku.Workflow.Bot.Oicq.Collections;

/// <summary>
/// 表示一个列表，存储的是一系列的 <see cref="PeriodicCommand"/> 的实例。
/// </summary>
/// <seealso cref="PeriodicCommand"/>
public sealed class PeriodicCommandCollection : List<PeriodicCommand>
{
	/// <summary>
	/// 表示一系列的 <see cref="Timer"/> 实例，专门用于触发。
	/// </summary>
	private readonly List<Timer> _timers = new();


	/// <summary>
	/// 表示内置的所有 <see cref="PeriodicCommand"/> 实例。
	/// </summary>
	public static PeriodicCommandCollection BuiltIn
	{
		get
		{
			var result = new PeriodicCommandCollection();
			result.AddRange(
				from type in typeof(PeriodicCommandCollection).Assembly.GetDerivedTypes<PeriodicCommand>()
				where type.GetConstructor(Type.EmptyTypes) is not null && type.IsDefined(typeof(PeriodicCommandAttribute))
				select (PeriodicCommand)Activator.CreateInstance(type)!
			);

			return result;
		}
	}


	/// <inheritdoc cref="List{T}.Add(T)"/>
	public new void Add(PeriodicCommand command)
	{
		_ = DateTime.Now is { Year: var y, Month: var m, Day: var d } now;

		if (command is not { TriggeringTime: { Hour: var h, Minute: var mm, Second: var s, Millisecond: var mmm } })
		{
			return;
		}

		var firstRun = new DateTime(y, m, d, h, mm, s, mmm);
		if (now > firstRun)
		{
			firstRun = firstRun.AddDays(1);
		}

		var timeToGo = firstRun - now;
		if (timeToGo <= TimeSpan.Zero)
		{
			timeToGo = TimeSpan.Zero;
		}

		_timers.Add(new(async _ => await command.ExecuteAsync(), null, timeToGo, TimeSpan.FromDays(1)));
	}

	/// <inheritdoc cref="List{T}.AddRange(IEnumerable{T})"/>
	public new void AddRange(IEnumerable<PeriodicCommand> commands)
	{
		foreach (var command in commands)
		{
			Add(command);
		}
	}
}
